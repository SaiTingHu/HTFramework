using System;

namespace HT.Framework
{
    /// <summary>
    /// 记录器
    /// </summary>
    /// <typeparam name="T">目标对象类型</typeparam>
    public abstract class Recorder<T> : IDisposable where T : class
    {
        /// <summary>
        /// 记录的目标对象
        /// </summary>
        public T Target { get; private set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                return Target != null;
            }
        }

        public Recorder(T target)
        {
            Target = target;
        }

        /// <summary>
        /// 重新记录
        /// </summary>
        /// <param name="newTarget">新的目标，为空则继续记录原目标，不为空则重新记录新目标</param>
        public virtual void Rerecord(T newTarget = null)
        {
            if (newTarget != null)
            {
                Target = newTarget;
            }
        }
        /// <summary>
        /// 还原
        /// </summary>
        public virtual void Recovery()
        {

        }
        /// <summary>
        /// 将记录的内容赋予其他目标
        /// </summary>
        /// <param name="other">其他目标</param>
        public virtual void AttachTo(T other)
        {

        }
        /// <summary>
        /// 销毁记录器
        /// </summary>
        public virtual void Dispose()
        {
            Target = null;
        }
    }
}