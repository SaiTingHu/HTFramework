using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 模块切面代理
    /// </summary>
    public sealed class AspectProxyModule<T> : AspectProxy<T> where T : IAspectProxyModule
    {
        public AspectProxyModule(T realObject) : base(realObject) { }

        protected override object[] BeforeInvoke(MethodBase method, object[] args)
        {
            Debug.Log(_realObject + " 初始化开始！");
            return args;
        }

        protected override void AfterInvoke(MethodBase method, object returnValue)
        {
            Debug.Log(_realObject + " 初始化完成！");
        }
    }

    /// <summary>
    /// 模块切面代理接口
    /// </summary>
    public interface IAspectProxyModule : IAspectTrackObject
    {
        void Initialization();
    }
}
