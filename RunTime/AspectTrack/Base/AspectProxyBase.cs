#if !DISABLE_ASPECTTRACK
using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace HT.Framework
{
    /// <summary>
    /// 切面代理者基类
    /// </summary>
    public abstract class AspectProxyBase<T> : RealProxy where T : IAspectTrackObject
    {
        private const string VOIDSIGN = "Void";

        protected T _realObject;

        /// <summary>
        /// 真实对象
        /// </summary>
        public T RealObject
        {
            get
            {
                return _realObject;
            }
        }

        public AspectProxyBase(T realObject) : base(typeof(T))
        {
            _realObject = realObject;
        }
        /// <summary>
        /// 获取代理对象
        /// </summary>
        /// <returns>代理对象</returns>
        public sealed override object GetTransparentProxy()
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
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="msg">执行消息</param>
        /// <returns>执行消息</returns>
        public sealed override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage callMsg = msg as IMethodCallMessage;

            bool isVoid = false;
            MethodInfo info = callMsg.MethodBase as MethodInfo;
            if (info != null)
            {
                isVoid = (info.ReturnType.Name == VOIDSIGN);
                info = null;
            }

            object returnValue = null;
            object[] args = OnBeforeInvoke(callMsg.MethodBase, callMsg.Args);
            
            try
            {
                if (isVoid && IsIntercept(callMsg.MethodBase, args))
                {
                    OnIntercept(callMsg.MethodBase);
                    returnValue = null;
                    return msg;
                }
                else
                {
                    if (args != null && args.Length == callMsg.ArgCount)
                    {
                        returnValue = callMsg.MethodBase.Invoke(_realObject, args);
                        return new ReturnMessage(returnValue, args, callMsg.ArgCount - callMsg.InArgCount, callMsg.LogicalCallContext, callMsg);
                    }
                    else
                    {
                        Log.Warning($"切面追踪：方法 {callMsg.MethodBase.Name} 经过修改后传入的实参与形参数量不匹配！");
                        returnValue = callMsg.MethodBase.Invoke(_realObject, callMsg.Args);
                        return new ReturnMessage(returnValue, callMsg.Args, callMsg.ArgCount - callMsg.InArgCount, callMsg.LogicalCallContext, callMsg);
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue = null;
                return new ReturnMessage(ex, callMsg);
            }
            finally
            {
                OnAfterInvoke(callMsg.MethodBase, returnValue);
            }
        }

        /// <summary>
        /// 方法被拦截
        /// </summary>
        /// <param name="method">方法</param>
        protected virtual void OnIntercept(MethodBase method)
        { }
        /// <summary>
        /// 方法调用前
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="args">参数</param>
        /// <returns>修正后的参数</returns>
        protected abstract object[] OnBeforeInvoke(MethodBase method, object[] args);
        /// <summary>
        /// 方法调用后
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="returnValue">返回值</param>
        protected abstract void OnAfterInvoke(MethodBase method, object returnValue);
        /// <summary>
        /// 是否拦截
        /// </summary>
        /// <param name="methodBase">方法</param>
        /// <param name="args">参数</param>
        /// <returns>是否被拦截</returns>
        private bool IsIntercept(MethodBase methodBase, object[] args)
        {
            if (Main.m_AspectTrack.IsEnableIntercept)
            {
                foreach (var condition in Main.m_AspectTrack.InterceptConditions)
                {
                    if (condition.Value != null)
                    {
                        if (condition.Value.Invoke(methodBase, args))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
#endif