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
        protected T _realObject;

        public AspectProxyBase(T realObject) : base(typeof(T))
        {
            _realObject = realObject;
        }

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

        /// <summary>
        /// 获取代理对象
        /// </summary>
        /// <returns>代理对象</returns>
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
                        GlobalTools.LogWarning(string.Format("切面追踪：方法 {0} 经过修改后传入的实参与形参数量不匹配！", callMsg.MethodBase.Name));
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
        protected virtual void OnIntercept(MethodBase method)
        { }

        /// <summary>
        /// 方法调用前
        /// </summary>
        protected abstract object[] OnBeforeInvoke(MethodBase method, object[] args);

        /// <summary>
        /// 方法调用后
        /// </summary>
        protected abstract void OnAfterInvoke(MethodBase method, object returnValue);

        //是否拦截
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
