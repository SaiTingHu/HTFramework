using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 流程管理器的助手接口
    /// </summary>
    public interface IProcedureHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 当前流程
        /// </summary>
        ProcedureBase CurrentProcedure { get; }
        /// <summary>
        /// 所有流程
        /// </summary>
        Dictionary<Type, ProcedureBase> Procedures { get; }
        /// <summary>
        /// 所有流程的类型
        /// </summary>
        List<Type> ProcedureTypes { get; }
        /// <summary>
        /// 任意流程切换事件（上一个离开的流程、下一个进入的流程）
        /// </summary>
        event HTFAction<ProcedureBase, ProcedureBase> AnyProcedureSwitchEvent;
        
        /// <summary>
        /// 获取流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>流程对象</returns>
        ProcedureBase GetProcedure(Type type);
        /// <summary>
        /// 是否存在指定类型的流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>是否存在</returns>
        bool IsExistProcedure(Type type);
        /// <summary>
        /// 是否存在指定序号的流程（依据编辑器面板的序号）
        /// </summary>
        /// <param name="index">流程序号</param>
        /// <returns>是否存在</returns>
        bool IsExistProcedure(int index);

        /// <summary>
        /// 切换流程
        /// </summary>
        /// <param name="type">目标流程</param>
        void SwitchProcedure(Type type);
        /// <summary>
        /// 切换至下一流程
        /// </summary>
        void SwitchNextProcedure();
        /// <summary>
        /// 切换至上一流程
        /// </summary>
        void SwitchLastProcedure();
        /// <summary>
        /// 切换至指定序号的流程（依据编辑器面板的序号）
        /// </summary>
        /// <param name="index">流程序号</param>
        void SwitchTargetProcedure(int index);
    }
}