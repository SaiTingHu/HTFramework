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
        /// <summary>
        /// 当前定义的实体与对象对应关系
        /// </summary>
        private Dictionary<string, GameObject> _defineEntities = new Dictionary<string, GameObject>();
        /// <summary>
        /// 所有实体组
        /// </summary>
        private Dictionary<Type, GameObject> _entitiesGroup = new Dictionary<Type, GameObject>();
        /// <summary>
        /// 实体管理器
        /// </summary>
        private EntityManager _module;
        /// <summary>
        /// 实体根节点
        /// </summary>
        private Transform _entityRoot;

        /// <summary>
        /// 实体管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 所有实体列表
        /// </summary>
        public Dictionary<Type, List<EntityLogicBase>> Entities { get; private set; } = new Dictionary<Type, List<EntityLogicBase>>();
        /// <summary>
        /// 所有实体对象池
        /// </summary>
        public Dictionary<Type, Queue<GameObject>> ObjectPools { get; private set; } = new Dictionary<Type, Queue<GameObject>>();
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
        /// 初始化助手
        /// </summary>
        public void OnInitialization()
        {
            _module = Module as EntityManager;
            _entityRoot = _module.transform.Find("EntityRoot");

            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(EntityLogicBase)) && !type.IsAbstract;
            });
            for (int i = 0; i < types.Count; i++)
            {
                EntityResourceAttribute attribute = types[i].GetCustomAttribute<EntityResourceAttribute>();
                if (attribute != null)
                {
                    Entities.Add(types[i], new List<EntityLogicBase>());

                    GameObject group = new GameObject(types[i].Name + "[Group]");
                    group.transform.SetParent(_entityRoot);
                    group.transform.localPosition = Vector3.zero;
                    group.transform.localRotation = Quaternion.identity;
                    group.transform.localScale = Vector3.one;
                    group.SetActive(true);
                    _entitiesGroup.Add(types[i], group);

                    ObjectPools.Add(types[i], new Queue<GameObject>());
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Entity, "创建实体逻辑对象失败：实体逻辑类 " + types[i].Name + " 丢失 EntityResourceAttribute 标记！");
                }
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnPreparatory()
        {
            
        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnRefresh()
        {
            foreach (var entities in Entities)
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
        /// 终结助手
        /// </summary>
        public void OnTermination()
        {
            _defineEntities.Clear();

            foreach (var group in _entitiesGroup)
            {
                Main.Kill(group.Value);
            }
            _entitiesGroup.Clear();
            Entities.Clear();
            ObjectPools.Clear();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 设置预定义
        /// </summary>
        /// <param name="defineEntityNames">预定义的实体名称</param>
        /// <param name="defineEntityTargets">预定义的实体对象</param>
        public void SetDefine(List<string> defineEntityNames, List<GameObject> defineEntityTargets)
        {
            for (int i = 0; i < defineEntityNames.Count; i++)
            {
                if (!_defineEntities.ContainsKey(defineEntityNames[i]))
                {
                    _defineEntities.Add(defineEntityNames[i], defineEntityTargets[i]);
                }
            }
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
                if (Entities.ContainsKey(type))
                {
                    if (attribute.IsUseObjectPool && ObjectPools[type].Count > 0)
                    {
                        EntityLogicBase entityLogic = GenerateEntity(type, ObjectPools[type].Dequeue(), entityName == "<None>" ? type.Name : entityName);

                        loadingAction?.Invoke(1);
                        loadDoneAction?.Invoke(entityLogic);
                        return null;
                    }
                    else
                    {
                        if (_defineEntities.ContainsKey(type.FullName) && _defineEntities[type.FullName] != null)
                        {
                            EntityLogicBase entityLogic = GenerateEntity(type, Main.Clone(_defineEntities[type.FullName], _entitiesGroup[type].transform), entityName == "<None>" ? type.Name : entityName);

                            loadingAction?.Invoke(1);
                            loadDoneAction?.Invoke(entityLogic);
                            return null;
                        }
                        else
                        {
                            return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _entitiesGroup[type].transform, loadingAction, (obj) =>
                            {
                                EntityLogicBase entityLogic = GenerateEntity(type, obj, entityName == "<None>" ? type.Name : entityName);

                                loadDoneAction?.Invoke(entityLogic);
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
            if (Entities.ContainsKey(type))
            {
                EntityLogicBase entityLogic = Entities[type].Find((entity) => { return entity.Name == entityName; });
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
            if (Entities.ContainsKey(type))
            {
                return Entities[type];
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
            if (Entities.ContainsKey(type))
            {
                for (int i = 0; i < Entities[type].Count; i++)
                {
                    if (Entities[type][i].IsShowed)
                    {
                        continue;
                    }

                    Entities[type][i].Entity.SetActive(true);
                    Entities[type][i].OnShow();
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
            if (Entities.ContainsKey(type))
            {
                for (int i = 0; i < Entities[type].Count; i++)
                {
                    if (!Entities[type][i].IsShowed)
                    {
                        continue;
                    }

                    Entities[type][i].Entity.SetActive(false);
                    Entities[type][i].OnHide();
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Entity, "隐藏实体失败：实体对象 " + type.Name + " 并未存在！");
            }
        }

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="entity">实体对象</param>
        /// <param name="entityName">实体名称</param>
        /// <returns>实体逻辑类</returns>
        private EntityLogicBase GenerateEntity(Type type, GameObject entity, string entityName)
        {
            EntityLogicBase entityLogic = Main.m_ReferencePool.Spawn(type) as EntityLogicBase;
            Entities[type].Add(entityLogic);
            entityLogic.Entity = entity;
            entityLogic.Entity.name = entityLogic.Name = entityName;
            entityLogic.Entity.SetActive(true);
            entityLogic.OnInit();
            entityLogic.OnShow();
            Main.m_Event.Throw(Main.m_ReferencePool.Spawn<EventCreateEntitySucceed>().Fill(entityLogic));
            return entityLogic;
        }
        /// <summary>
        /// 回收实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑类</param>
        private void RecoveryEntity(EntityLogicBase entityLogic)
        {
            Type type = entityLogic.GetType();
            EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
            if (attribute != null)
            {
                if (Entities.ContainsKey(type))
                {
                    if (attribute.IsUseObjectPool)
                    {
                        Entities[type].Remove(entityLogic);
                        entityLogic.OnDestroy();
                        Main.m_ReferencePool.Despawn(entityLogic);
                        ObjectPools[type].Enqueue(entityLogic.Entity);
                        entityLogic.Entity.SetActive(false);
                        entityLogic.Entity = null;
                        entityLogic = null;
                    }
                    else
                    {
                        Entities[type].Remove(entityLogic);
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
        /// <summary>
        /// 批量回收实体
        /// </summary>
        /// <param name="type">实体类型</param>
        private void RecoveryEntities(Type type)
        {
            EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
            if (attribute != null)
            {
                if (Entities.ContainsKey(type))
                {
                    if (attribute.IsUseObjectPool)
                    {
                        for (int i = 0; i < Entities[type].Count; i++)
                        {
                            EntityLogicBase entityLogic = Entities[type][i];
                            entityLogic.OnDestroy();
                            Main.m_ReferencePool.Despawn(entityLogic);
                            ObjectPools[type].Enqueue(entityLogic.Entity);
                            entityLogic.Entity.SetActive(false);
                            entityLogic.Entity = null;
                        }
                        Entities[type].Clear();
                    }
                    else
                    {
                        for (int i = 0; i < Entities[type].Count; i++)
                        {
                            EntityLogicBase entityLogic = Entities[type][i];
                            entityLogic.OnDestroy();
                            Main.m_ReferencePool.Despawn(entityLogic);
                            Main.Kill(entityLogic.Entity);
                            entityLogic.Entity = null;
                        }
                        Entities[type].Clear();
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