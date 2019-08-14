using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 单例模式 Behaviour 基类
    /// </summary>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : class
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

        protected virtual void Awake()
        {
            _current = GetComponent<T>();
        }
    }
}
