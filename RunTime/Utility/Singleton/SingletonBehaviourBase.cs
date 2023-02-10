using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 单例模式 Behaviour 基类
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class SingletonBehaviourBase<T> : HTBehaviour where T : class
    {
        private static T _current;
        /// <summary>
        /// 当前实例
        /// </summary>
        public static T Current
        {
            get
            {
                return _current;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (_current == null)
            {
                _current = GetComponent<T>();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, $"单例类 {typeof(T).FullName} 发现两个或以上实例，这是不被允许的！");
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            _current = null;
        }
    }
}