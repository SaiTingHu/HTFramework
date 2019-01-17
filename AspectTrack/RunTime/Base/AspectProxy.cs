using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace HT.Framework
{
    /// <summary>
    /// 切面代理基类
    /// </summary>
    public abstract class AspectProxy<T> : RealProxy where T : IAspectTrackObject
    {
        protected T _realObject;

        /// <summary>
        /// 拦截方法的调用（注意，只拦截Void返回值的方法）
        /// </summary>
        public bool InterceptCall = false;
        
        public AspectProxy(T realObject) : base(typeof(T))
        {
            _realObject = realObject;
        }

        public override object GetTransparentProxy()
        {
            if (Main.m_AspectTrack.IsEnableAspectTrack)
            {
                return base.GetTransparentProxy();
            }
            else
            {
                return _realObject;
            }
        }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage callMsg = msg as IMethodCallMessage;

            bool isVoid = false;
            MethodInfo info = callMsg.MethodBase as MethodInfo;
            if (info != null)
            {
                isVoid = (info.ReturnType.Name == "Void");
                info = null;
            }

            object retValue = null;
            object[] args = BeforeInvoke(callMsg.MethodBase, callMsg.Args);
            
            try
            {
                if (InterceptCall && isVoid)
                {
                    retValue = null;
                    return msg;
                }
                else
                {
                    if (args != null && args.Length == callMsg.ArgCount)
                    {
                        retValue = callMsg.MethodBase.Invoke(_realObject, args);
                        return new ReturnMessage(retValue, args, callMsg.ArgCount - callMsg.InArgCount, callMsg.LogicalCallContext, callMsg);
                    }
                    else
                    {
                        GlobalTools.LogWarning("切面追踪：方法 " + callMsg.MethodBase.Name + " 经过修改后传入的实参与形参数量不匹配！");
                        retValue = callMsg.MethodBase.Invoke(_realObject, callMsg.Args);
                        return new ReturnMessage(retValue, callMsg.Args, callMsg.ArgCount - callMsg.InArgCount, callMsg.LogicalCallContext, callMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                retValue = null;
                return new ReturnMessage(ex, callMsg);
            }
            finally
            {
                AfterInvoke(callMsg.MethodBase, retValue);
            }
        }

        /// <summary>
        /// 方法调用前
        /// </summary>
        protected abstract object[] BeforeInvoke(MethodBase method, object[] args);

        /// <summary>
        /// 方法调用后
        /// </summary>
        protected abstract void AfterInvoke(MethodBase method, object returnValue);
    }
}
