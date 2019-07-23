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
    public sealed class EntityManager : ModuleManager
    {
        private Dictionary<Type, List<EntityLogic>> _entities = new Dictionary<Type, List<EntityLogic>>();
        private Dictionary<Type, GameObject> _entitiesGroup = new Dictionary<Type, GameObject>();
        private GameObject _entityRoot;

        public override void OnInitialization()
        {
            base.OnInitialization();

            _entityRoot = transform.FindChildren("EntityRoot");

            //创建所有实体的逻辑对象
            List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(typeof(EntityLogic)))
                {
                    EntityResourceAttribute attribute = types[i].GetCustomAttribute<EntityResourceAttribute>();
                    if (attribute != null)
                    {
                        _entities.Add(types[i], new List<EntityLogic>());

                        GameObject group = new GameObject(types[i].Name + "[Group]");
                        group.transform.SetParent(_entityRoot.transform);
                        group.transform.localPosition = Vector3.zero;
                        group.transform.localRotation = Quaternion.identity;
                        group.transform.localScale = Vector3.one;
                        group.SetActive(true);
                        _entitiesGroup.Add(types[i], group);
                    }
                    else
                    {
                        GlobalTools.LogError(string.Format("创建实体逻辑对象失败：实体逻辑类 {0} 丢失 EntityResourceAttribute 标记！", types[i].Name));
                    }
                }
            }
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            foreach (KeyValuePair<Type, List<EntityLogic>> entities in _entities)
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
            _entities.Clear();
            _entitiesGroup.Clear();
        }

        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        /// <returns>实体组</returns>
        public List<T> GetEntities<T>() where T : EntityLogic
        {
            List<T> entities = null;
            List<EntityLogic> entityLogics = GetEntities(typeof(T));
            if (entityLogics != null)
            {
                entities = entityLogics.ConvertAllAS<T, EntityLogic>();
            }
            return entities;
        }

        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <returns>实体组</returns>
        public List<EntityLogic> GetEntities(Type type)
        {
            if (_entities.ContainsKey(type))
            {
                return _entities[type];
            }
            else
            {
                GlobalTools.LogError(string.Format("获取实体组失败：实体对象 {0} 并未存在！", type.Name));
                return null;
            }
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void CreateEntity<T>(HTFAction<float> loadingAction = null) where T : EntityLogic
        {
            CreateEntity(typeof(T), loadingAction);
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        public void CreateEntity(Type type, HTFAction<float> loadingAction = null)
        {
            EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
            if (attribute != null)
            {
                if (_entities.ContainsKey(type))
                {
                    Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _entitiesGroup[type].transform, loadingAction, (obj) =>
                    {
                        EntityLogic entityLogic = Main.m_ReferencePool.Spawn(type) as EntityLogic;
                        entityLogic.Entity = obj;
                        entityLogic.Entity.SetActive(true);
                        entityLogic.OnInit();
                        entityLogic.OnShow();
                        _entities[type].Add(entityLogic);

                        Main.m_Event.Throw(this, Main.m_ReferencePool.Spawn<EventCreateEntitySucceed>().Fill(entityLogic));
                    });
                }
                else
                {
                    GlobalTools.LogError(string.Format("创建实体失败：实体对象 {0} 并未存在！", type.Name));
                }
            }
        }

        /// <summary>
        /// 销毁实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        public void DestroyEntity(EntityLogic entityLogic)
        {
            Type type = entityLogic.GetType();
            if (_entities.ContainsKey(type))
            {
                _entities[type].Remove(entityLogic);
                Main.m_ReferencePool.Despawn(entityLogic);
                entityLogic.OnDestroy();
                Destroy(entityLogic.Entity);
                entityLogic.Entity = null;
                entityLogic = null;
            }
            else
            {
                GlobalTools.LogError(string.Format("销毁实体失败：实体对象 {0} 并未存在！", type.Name));
            }
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        public void ShowEntity(EntityLogic entityLogic)
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
        public void HideEntity(EntityLogic entityLogic)
        {
            if (!entityLogic.IsShowed)
            {
                return;
            }

            entityLogic.Entity.SetActive(false);
            entityLogic.OnHide();
        }

        /// <summary>
        /// 销毁指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void DestroyEntities<T>()
        {
            DestroyEntities(typeof(T));
        }

        /// <summary>
        /// 销毁指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        public void DestroyEntities(Type type)
        {
            if (_entities.ContainsKey(type))
            {
                for (int i = 0; i < _entities[type].Count; i++)
                {
                    EntityLogic entityLogic = _entities[type][i];
                    Main.m_ReferencePool.Despawn(entityLogic);
                    entityLogic.OnDestroy();
                    Destroy(entityLogic.Entity);
                    entityLogic.Entity = null;
                }
                _entities[type].Clear();
            }
            else
            {
                GlobalTools.LogError(string.Format("销毁实体失败：实体对象 {0} 并未存在！", type.Name));
            }
        }

        /// <summary>
        /// 显示指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void ShowEntities<T>() where T : EntityLogic
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
                GlobalTools.LogError(string.Format("显示实体失败：实体对象 {0} 并未存在！", type.Name));
            }
        }

        /// <summary>
        /// 隐藏指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void HideEntities<T>() where T : EntityLogic
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
                GlobalTools.LogError(string.Format("隐藏实体失败：实体对象 {0} 并未存在！", type.Name));
            }
        }
    }
}
