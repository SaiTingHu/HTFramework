using System;

namespace HT.Framework
{
    /// <summary>
    /// 热更新流程状态标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HotfixProcedureStateAttribute : Attribute
    {
        public HotfixProcedureState State;

        public HotfixProcedureStateAttribute(HotfixProcedureState state)
        {
            State = state;
        }
    }

    /// <summary>
    /// 热更新流程状态
    /// </summary>
    public enum HotfixProcedureState
    {
        /// <summary>
        /// 入口流程
        /// </summary>
        Entrance,
        /// <summary>
        /// 常规流程
        /// </summary>
        Normal,
        /// <summary>
        /// 禁用的流程
        /// </summary>
        Disabled
    }
}