using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 组件属性记录器
    /// </summary>
    /// <typeparam name="T">目标组件类型</typeparam>
    public abstract class PropertyRecorder<T> : Recorder<T> where T : Component
    {
        public PropertyRecorder(T target) : base(target)
        {

        }
    }
}