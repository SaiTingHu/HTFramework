using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// ECS的组件
    /// </summary>
    [RequireComponent(typeof(ECS_Entity))]
    public abstract class ECS_Component : HTBehaviour
    {
        /// <summary>
        /// 属于的实体
        /// </summary>
        public ECS_Entity Entity { get; private set; }

#if UNITY_EDITOR
        /// <summary>
        /// 组件的名称
        /// </summary>
        internal string Name;
#endif

        protected override void Awake()
        {
            base.Awake();

            Entity = GetComponent<ECS_Entity>();
            Entity.AppendComponent(this);

#if UNITY_EDITOR
            ComponentNameAttribute cna = GetType().GetCustomAttribute<ComponentNameAttribute>();
            Name = string.Format("{0} ({1})", cna != null ? cna.Name : "未命名", GetType().FullName);
#endif
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Entity) Entity.RemoveComponent(this);
        }
    }
}