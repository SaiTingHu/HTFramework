using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 默认的ECS管理器助手
    /// </summary>
    public sealed class DefaultECSHelper : IECSHelper
    {
        /// <summary>
        /// 当前ECS环境是否是脏的
        /// </summary>
        private bool _isDirty = false;

        /// <summary>
        /// ECS管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 所有系统【系统类型，系统对象】
        /// </summary>
        public Dictionary<Type, ECS_System> Systems { get; private set; } = new Dictionary<Type, ECS_System>();
        /// <summary>
        /// 所有实体【实体ID，实体对象】
        /// </summary>
        public Dictionary<string, ECS_Entity> Entities { get; private set; } = new Dictionary<string, ECS_Entity>();
        
        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(ECS_System)) && !type.IsAbstract;
            });
            for (int i = 0; i < types.Count; i++)
            {
                Systems.Add(types[i], Activator.CreateInstance(types[i]) as ECS_System);
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {
            foreach (var system in Systems)
            {
                system.Value.OnStart();
            }
        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            if (_isDirty)
            {
                _isDirty = false;

                foreach (var system in Systems)
                {
                    if (system.Value.IsEnabled)
                    {
                        system.Value.StarEntities.Clear();
                        foreach (var entity in Entities)
                        {
                            if (entity.Value.IsExistComponents(system.Value.StarComponents))
                            {
                                system.Value.StarEntities.Add(entity.Value);
                            }
                        }
                    }
                }
            }

            foreach (var system in Systems)
            {
                if (system.Value.IsEnabled)
                {
                    system.Value.OnUpdate(system.Value.StarEntities);
                }
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            Systems.Clear();
            Entities.Clear();
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
        /// 设置ECS环境为脏的，触发ECS环境重新刷新
        /// </summary>
        public void SetDirty()
        {
            _isDirty = true;
        }
        /// <summary>
        /// 设置系统激活
        /// </summary>
        /// <param name="type">系统类型</param>
        /// <param name="isEnable">是否激活</param>
        public void SetSystemEnable(Type type, bool isEnable)
        {
            if (Systems.ContainsKey(type))
            {
                Systems[type].IsEnabled = isEnable;
                SetDirty();
            }
        }
        /// <summary>
        /// 获取系统
        /// </summary>
        /// <param name="type">系统类型</param>
        /// <returns>系统对象</returns>
        public ECS_System GetSystem(Type type)
        {
            if (Systems.ContainsKey(type))
            {
                return Systems[type];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void AddEntity(ECS_Entity entity)
        {
            if (entity.ID == "")
            {
                Log.Warning(string.Format("ECS：发现ID为空的实体 [{0}]，这是不被允许的！", entity.Name));
                return;
            }

            if (Entities.ContainsKey(entity.ID))
            {
                Log.Warning(string.Format("ECS：发现ID [{0}] 重复的实体 [{1}] 和 [{2}]，这是不被允许的！", entity.ID, entity.Name, Entities[entity.ID].Name));
            }
            else
            {
                Entities.Add(entity.ID, entity);
                SetDirty();
            }
        }
        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void RemoveEntity(ECS_Entity entity)
        {
            if (Entities.ContainsKey(entity.ID))
            {
                Entities.Remove(entity.ID);
                SetDirty();
            }
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>实体对象</returns>
        public ECS_Entity GetEntity(string id)
        {
            if (Entities.ContainsKey(id))
            {
                return Entities[id];
            }
            else
            {
                return null;
            }
        }
    }
}