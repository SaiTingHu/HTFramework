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
        ProcedureBase CurrentProcedure { get; set; }
        /// <summary>
        /// 所有流程
        /// </summary>
        Dictionary<Type, ProcedureBase> Procedures { get; set; }
        /// <summary>
        /// 所有流程的类型
        /// </summary>
        List<Type> ProcedureTypes { get; set; }
        /// <summary>
        /// 任意流程切换事件（上一个离开的流程、下一个进入的流程）
        /// </summary>
        event HTFAction<ProcedureBase, ProcedureBase> AnyProcedureSwitchEvent;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="activatedProcedures">激活的流程列表</param>
        /// <param name="defaultProcedure">默认流程名称</param>
        void OnInitialization(List<string> activatedProcedures, string defaultProcedure);
        /// <summary>
        /// 准备工作
        /// </summary>
        void OnPreparatory();
        /// <summary>
        /// 刷新
        /// </summary>
        void OnRefresh();
        /// <summary>
        /// 终结
        /// </summary>
        void OnTermination();

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>流程对象</returns>
        ProcedureBase GetProcedure(Type type);
        /// <summary>
        /// 是否存在流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>是否存在</returns>
        bool IsExistProcedure(Type type);

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