using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的实体管理器助手
    /// </summary>
    public sealed class DefaultEntityHelper : IEntityHelper
    {
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

        /// <summary>
        /// 实体管理器
        /// </summary>
        public InternalModuleBase Module { get; set; }
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
        /// 初始化
        /// </summary>
        /// <param name="defineEntityNames">预定义的实体名称</param>
        /// <param name="defineEntityTargets">预定义的实体对象</param>
        public void OnInitialization(List<string> defineEntityNames, List<GameObject> defineEntityTargets)
        {
            for (int i = 0; i < defineEntityNames.Count; i++)
            {
                if (!_defineEntities.ContainsKey(defineEntityNames[i]))
                {
                    _defineEntities.Add(defineEntityNames[i], defineEntityTargets[i]);
                }
            }

            _entityRoot = Module.transform.Find("EntityRoot");

            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(EntityLogicBase));
            });
            for (int i = 0; i < types.Count; i++)
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
        /// <summary>
        /// 逻辑刷新
        /// </summary>
        public void OnRefresh()
        {
            foreach (var entities in _entities)
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
        /// <summary>
        /// 终结
        /// </summary>
        public void OnTermination()
        {
            _defineEntities.Clear();

            foreach (var group in _entitiesGroup)
            {
                Main.Kill(group.Value);
            }
            foreach (var objectPool in _objectPool)
            {
                while (objectPool.Value.Count > 0)
                {
                    Main.Kill(objectPool.Value.Dequeue());
                }
            }
            _entities.Clear();
            _entitiesGroup.Clear();
            _objectPool.Clear();
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <param name="entityName">实体指定名称（为 <None> 时默认使用实体逻辑类名称）</param>
        /// <param name="loadingAction">创建实体过程进度回调</param>
        /// <param name="loadDoneAction">创建实体完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine CreateEntity(Type type, string entityName, HTFAction<float> loadingAction, HTFAction<EntityLogicBase> loadDoneAction)
        {
            EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
            if (attribute != null)
            {
                if (_entities.ContainsKey(type))
                {
                    if (attribute.IsUseObjectPool && _objectPool[type].Count > 0)
                    {
                        EntityLogicBase entityLogic = GenerateEntity(type, _objectPool[type].Dequeue(), entityName == "<None>" ? type.Name : entityName);

                        loadingAction?.Invoke(1);
                        loadDoneAction?.Invoke(entityLogic);
                        Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventCreateEntitySucceed>().Fill(entityLogic));
                        return null;
                    }
                    else
                    {
                        if (_defineEntities.ContainsKey(type.FullName) && _defineEntities[type.FullName] != null)
                        {
                            EntityLogicBase entityLogic = GenerateEntity(type, Main.Clone(_defineEntities[type.FullName], _entitiesGroup[type].transform), entityName == "<None>" ? type.Name : entityName);

                            loadingAction?.Invoke(1);
                            loadDoneAction?.Invoke(entityLogic);
                            Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventCreateEntitySucceed>().Fill(entityLogic));
                            return null;
                        }
                        else
                        {
                            return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _entitiesGroup[type].transform, loadingAction, (obj) =>
                            {
                                EntityLogicBase entityLogic = GenerateEntity(type, obj, entityName == "<None>" ? type.Name : entityName);

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
        /// <param name="type">实体逻辑类</param>
        public void DestroyEntities(Type type)
        {
            RecoveryEntities(type);
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

        //生成实体
        private EntityLogicBase GenerateEntity(Type type, GameObject entity, string entityName)
        {
            EntityLogicBase entityLogic = Main.m_ReferencePool.Spawn(type) as EntityLogicBase;
            _entities[type].Add(entityLogic);
            entityLogic.Entity = entity;
            entityLogic.Entity.name = entityLogic.Name = entityName;
            entityLogic.Entity.SetActive(true);
            entityLogic.OnInit();
            entityLogic.OnShow();
            return entityLogic;
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
                        Main.Kill(entityLogic.Entity);
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
                            Main.Kill(entityLogic.Entity);
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