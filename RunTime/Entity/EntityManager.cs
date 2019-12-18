using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 实体管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Entity)]
    public sealed class EntityManager : InternalModuleBase
    {
        /// <summary>
        /// 当前定义的实体名称【请勿在代码中修改】
        /// </summary>
        public List<string> DefineEntityNames = new List<string>();
        /// <summary>
        /// 当前定义的实体对象【请勿在代码中修改】
        /// </summary>
        public List<GameObject> DefineEntityTargets = new List<GameObject>();

        //当前定义的实体与对象对应关系
        private Dictionary<string, GameObject> _defineEntities = new Dictionary<string, GameObject>();

        //所有实体列表
        private Dictionary<Type, List<EntityLogicBase>> _entities = new Dictionary<Type, List<EntityLogicBase>>();
        //所有实体组
        private Dictionary<Type, GameObject> _entitiesGroup = new Dictionary<Type, GameObject>();
        //所有实体对象池
        private Dictionary<Type, Queue<GameObject>> _objectPool = new Dictionary<Type, Queue<GameObject>>();
        //实体根节点
        private Transform _entityRoot;

        public override void OnInitialization()
        {
            base.OnInitialization();

            for (int i = 0; i < DefineEntityNames.Count; i++)
            {
                if (!_defineEntities.ContainsKey(DefineEntityNames[i]))
                {
                    _defineEntities.Add(DefineEntityNames[i], DefineEntityTargets[i]);
                }
            }

            _entityRoot = transform.Find("EntityRoot");

            //创建所有实体的逻辑对象
            List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(typeof(EntityLogicBase)))
                {
                    EntityResourceAttribute attribute = types[i].GetCustomAttribute<EntityResourceAttribute>();
                    if (attribute != null)
                    {
                        _entities.Add(types[i], new List<EntityLogicBase>());

                        GameObject group = new GameObject(types[i].Name + "[Group]");
                        group.transform.SetParent(_entityRoot);
                        group.transform.localPosition = Vector3.zero;
                        group.transform.localRotation = Quaternion.identity;
                        group.transform.localScale = Vector3.one;
                        group.SetActive(true);
                        _entitiesGroup.Add(types[i], group);

                        _objectPool.Add(types[i], new Queue<GameObject>());
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Entity, "创建实体逻辑对象失败：实体逻辑类 " + types[i].Name + " 丢失 EntityResourceAttribute 标记！");
                    }
                }
            }
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            foreach (KeyValuePair<Type, List<EntityLogicBase>> entities in _entities)
            {
                for (int i = 0; i < entities.Value.Count; i++)
                {
                    if (entities.Value[i].IsShowed)
                    {
                        entities.Value[i].OnUpdate();
                    }
                }
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            foreach (KeyValuePair<Type, GameObject> group in _entitiesGroup)
            {
                Destroy(group.Value);
            }
            foreach (KeyValuePair<Type, Queue<GameObject>> objectPool in _objectPool)
            {
                while (objectPool.Value.Count > 0)
                {
                    Destroy(objectPool.Value.Dequeue());
                }
            }
            _entities.Clear();
            _entitiesGroup.Clear();
            _objectPool.Clear();
        }

        /// <summary>
        /// 是否隐藏所有实体
        /// </summary>
        public bool IsHideAll
        {
            set
            {
                _entityRoot.gameObject.SetActive(!value);
            }
            get
            {
                return !_entityRoot.gameObject.activeSelf;
            }
        }

        /// <summary>
        /// 根据名称获取实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        /// <param name="entityName">实体名称</param>
        /// <returns>实体</returns>
        public T GetEntity<T>(string entityName) where T : EntityLogicBase
        {
            return GetEntity(typeof(T), entityName) as T;
        }
        /// <summary>
        /// 根据名称获取实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <param name="entityName">实体名称</param>
        /// <returns>实体</returns>
        public EntityLogicBase GetEntity(Type type, string entityName)
        {
            if (_entities.ContainsKey(type))
            {
                EntityLogicBase entityLogic = _entities[type].Find((entity) => { return entity.Name == entityName; });
                if (entityLogic == null)
                {
                    throw new HTFrameworkException(HTFrameworkModule.Entity, "获取实体失败：实体名称 " + entityName + " 并未存在！");
                }
                return entityLogic;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Entity, "获取实体失败：实体对象 " + type.Name + " 并未存在！");
            }
        }
        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        /// <returns>实体组</returns>
        public List<T> GetEntities<T>() where T : EntityLogicBase
        {
            List<T> entities = null;
            List<EntityLogicBase> entityLogics = GetEntities(typeof(T));
            if (entityLogics != null)
            {
                entities = entityLogics.ConvertAllAS<T, EntityLogicBase>();
            }
            return entities;
        }
        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <returns>实体组</returns>
        public List<EntityLogicBase> GetEntities(Type type)
        {
            if (_entities.ContainsKey(type))
            {
                return _entities[type];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Entity, "获取实体组失败：实体对象 " + type.Name + " 并未存在！");
            }
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        /// <param name="entityName">实体指定名称（为 <None> 时默认使用实体逻辑类名称）</param>
        /// <param name="loadingAction">创建实体过程进度回调</param>
        /// <param name="loadDoneAction">创建实体完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine CreateEntity<T>(string entityName = "<None>", HTFAction<float> loadingAction = null, HTFAction<EntityLogicBase> loadDoneAction = null) where T : EntityLogicBase
        {
            return ExtractEntity(typeof(T), entityName, loadingAction, loadDoneAction);
        }
        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <param name="entityName">实体指定名称（为 <None> 时默认使用实体逻辑类名称）</param>
        /// <param name="loadingAction">创建实体过程进度回调</param>
        /// <param name="loadDoneAction">创建实体完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine CreateEntity(Type type, string entityName = "<None>", HTFAction<float> loadingAction = null, HTFAction<EntityLogicBase> loadDoneAction = null)
        {
            return ExtractEntity(type, entityName, loadingAction, loadDoneAction);
        }
        
        /// <summary>
        /// 销毁实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        public void DestroyEntity(EntityLogicBase entityLogic)
        {
            RecoveryEntity(entityLogic);
        }
        /// <summary>
        /// 销毁指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void DestroyEntities<T>()
        {
            RecoveryEntities(typeof(T));
        }
        /// <summary>
        /// 销毁指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        public void DestroyEntities(Type type)
        {
            RecoveryEntities(type);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        public void ShowEntity(EntityLogicBase entityLogic)
        {
            if (entityLogic.IsShowed)
            {
                return;
            }

            entityLogic.Entity.SetActive(true);
            entityLogic.OnShow();
        }
        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        public void HideEntity(EntityLogicBase entityLogic)
        {
            if (!entityLogic.IsShowed)
            {
                return;
            }

            entityLogic.Entity.SetActive(false);
            entityLogic.OnHide();
        }
        /// <summary>
        /// 显示指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void ShowEntities<T>() where T : EntityLogicBase
        {
            ShowEntities(typeof(T));
        }
        /// <summary>
        /// 显示指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        public void ShowEntities(Type type)
        {
            if (_entities.ContainsKey(type))
            {
                for (int i = 0; i < _entities[type].Count; i++)
                {
                    if (_entities[type][i].IsShowed)
                    {
                        continue;
                    }

                    _entities[type][i].Entity.SetActive(true);
                    _entities[type][i].OnShow();
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Entity, "显示实体失败：实体对象 " + type.Name + " 并未存在！");
            }
        }
        /// <summary>
        /// 隐藏指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void HideEntities<T>() where T : EntityLogicBase
        {
            HideEntities(typeof(T));
        }
        /// <summary>
        /// 隐藏指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        public void HideEntities(Type type)
        {
            if (_entities.ContainsKey(type))
            {
                for (int i = 0; i < _entities[type].Count; i++)
                {
                    if (!_entities[type][i].IsShowed)
                    {
                        continue;
                    }

                    _entities[type][i].Entity.SetActive(false);
                    _entities[type][i].OnHide();
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Entity, "隐藏实体失败：实体对象 " + type.Name + " 并未存在！");
            }
        }

        //提取实体
        private Coroutine ExtractEntity(Type type, string entityName = "<None>", HTFAction<float> loadingAction = null, HTFAction<EntityLogicBase> loadDoneAction = null)
        {
            EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
            if (attribute != null)
            {
                if (_entities.ContainsKey(type))
                {
                    if (attribute.IsUseObjectPool && _objectPool[type].Count > 0)
                    {
                        EntityLogicBase entityLogic = Main.m_ReferencePool.Spawn(type) as EntityLogicBase;
                        _entities[type].Add(entityLogic);
                        entityLogic.Entity = _objectPool[type].Dequeue();
                        entityLogic.Entity.name = entityLogic.Name = entityName == "<None>" ? type.Name : entityName;
                        entityLogic.Entity.SetActive(true);
                        entityLogic.OnInit();
                        entityLogic.OnShow();

                        loadingAction?.Invoke(1);
                        loadDoneAction?.Invoke(entityLogic);
                        Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventCreateEntitySucceed>().Fill(entityLogic));
                        return null;
                    }
                    else
                    {
                        if (_defineEntities.ContainsKey(type.FullName) && _defineEntities[type.FullName] != null)
                        {
                            EntityLogicBase entityLogic = Main.m_ReferencePool.Spawn(type) as EntityLogicBase;
                            _entities[type].Add(entityLogic);
                            entityLogic.Entity = Instantiate(_defineEntities[type.FullName], _entitiesGroup[type].transform);
                            entityLogic.Entity.name = entityLogic.Name = entityName == "<None>" ? type.Name : entityName;
                            entityLogic.Entity.SetActive(true);
                            entityLogic.OnInit();
                            entityLogic.OnShow();

                            loadingAction?.Invoke(1);
                            loadDoneAction?.Invoke(entityLogic);
                            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventCreateEntitySucceed>().Fill(entityLogic));
                            return null;
                        }
                        else
                        {
                            return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _entitiesGroup[type].transform, loadingAction, (obj) =>
                            {
                                EntityLogicBase entityLogic = Main.m_ReferencePool.Spawn(type) as EntityLogicBase;
                                _entities[type].Add(entityLogic);
                                entityLogic.Entity = obj;
                                entityLogic.Entity.name = entityLogic.Name = entityName == "<None>" ? type.Name : entityName;
                                entityLogic.Entity.SetActive(true);
                                entityLogic.OnInit();
                                entityLogic.OnShow();

                                loadDoneAction?.Invoke(entityLogic);
                                Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventCreateEntitySucceed>().Fill(entityLogic));
                            });
                        }
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Entity, "创建实体失败：实体对象 " + type.Name + " 并未存在！");
                }
            }
            return null;
        }
        //回收实体
        private void RecoveryEntity(EntityLogicBase entityLogic)
        {
            Type type = entityLogic.GetType();
            EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
            if (attribute != null)
            {
                if (_entities.ContainsKey(type))
                {
                    if (attribute.IsUseObjectPool)
                    {
                        _entities[type].Remove(entityLogic);
                        entityLogic.OnDestroy();
                        Main.m_ReferencePool.Despawn(entityLogic);
                        _objectPool[type].Enqueue(entityLogic.Entity);
                        entityLogic.Entity.SetActive(false);
                        entityLogic.Entity = null;
                        entityLogic = null;
                    }
                    else
                    {
                        _entities[type].Remove(entityLogic);
                        entityLogic.OnDestroy();
                        Main.m_ReferencePool.Despawn(entityLogic);
                        Destroy(entityLogic.Entity);
                        entityLogic.Entity = null;
                        entityLogic = null;
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Entity, "销毁实体失败：实体对象 " + type.Name + " 并未存在！");
                }
            }
        }
        //批量回收实体
        private void RecoveryEntities(Type type)
        {
            EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
            if (attribute != null)
            {
                if (_entities.ContainsKey(type))
                {
                    if (attribute.IsUseObjectPool)
                    {
                        for (int i = 0; i < _entities[type].Count; i++)
                        {
                            EntityLogicBase entityLogic = _entities[type][i];
                            entityLogic.OnDestroy();
                            Main.m_ReferencePool.Despawn(entityLogic);
                            _objectPool[type].Enqueue(entityLogic.Entity);
                            entityLogic.Entity.SetActive(false);
                            entityLogic.Entity = null;
                        }
                        _entities[type].Clear();
                    }
                    else
                    {
                        for (int i = 0; i < _entities[type].Count; i++)
                        {
                            EntityLogicBase entityLogic = _entities[type][i];
                            entityLogic.OnDestroy();
                            Main.m_ReferencePool.Despawn(entityLogic);
                            Destroy(entityLogic.Entity);
                            entityLogic.Entity = null;
                        }
                        _entities[type].Clear();
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Entity, "销毁实体失败：实体对象 " + type.Name + " 并未存在！");
                }
            }
        }
    }
}