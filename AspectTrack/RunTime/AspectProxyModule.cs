using System.Reflection;

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
            GlobalTools.LogInfo(_realObject + " -> " + method.Name + " 开始执行！");
            return args;
        }

        protected override void AfterInvoke(MethodBase method, object returnValue)
        {
            GlobalTools.LogInfo(_realObject + " -> " + method.Name + " 执行完成！");
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
