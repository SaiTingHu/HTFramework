using System;
using System.Collections.Generic;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// ECS的系统
    /// </summary>
    public abstract class ECS_System
    {
        /// <summary>
        /// 是否激活
        /// </summary>
        internal bool IsEnabled;
        /// <summary>
        /// 关注的所有组件
        /// </summary>
        internal Type[] StarComponents;
        /// <summary>
        /// 关注的所有实体
        /// </summary>
        internal HashSet<ECS_Entity> StarEntities;

#if UNITY_EDITOR
        /// <summary>
        /// 系统的名称
        /// </summary>
        internal string Name;
#endif

        /// <summary>
        /// 系统初始化
        /// </summary>
        public virtual void OnStart()
        {
            IsEnabled = true;
            StarComponentAttribute sca = GetType().GetCustomAttribute<StarComponentAttribute>();
            StarComponents = sca != null ? sca.StarComponents : null;
            StarEntities = new HashSet<ECS_Entity>();

#if UNITY_EDITOR
            SystemNameAttribute mna = GetType().GetCustomAttribute<SystemNameAttribute>();
            Name = string.Format("{0} ({1})", mna != null ? mna.Name : "未命名", GetType().FullName);
#endif
        }

        /// <summary>
        /// 系统逻辑更新
        /// </summary>
        /// <param name="entities">系统关注的所有实体</param>
        public abstract void OnUpdate(HashSet<ECS_Entity> entities);
    }
}