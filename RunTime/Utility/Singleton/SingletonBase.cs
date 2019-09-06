namespace HT.Framework
{
    /// <summary>
    /// 单例模式基类
    /// </summary>
    public abstract class SingletonBase<T> where T : class, new()
    {
        private static T _current;
        /// <summary>
        /// 当前实例
        /// </summary>
        public static T Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new T();
                }
                return _current;
            }
        }
    }
}
