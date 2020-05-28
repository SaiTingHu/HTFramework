using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// FSM管理器的助手接口
    /// </summary>
    public interface IFSMHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 已注册的状态机
        /// </summary>
        Dictionary<string, FSM> FSMs { get; set; }
        /// <summary>
        /// 已注册的状态机组
        /// </summary>
        Dictionary<string, List<FSM>> FSMGroups { get; set; }

        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        void RegisterFSM(FSM fsm);
        /// <summary>
        /// 移除已注册的状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        void UnRegisterFSM(FSM fsm);

        /// <summary>
        /// 通过名称获取状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>状态机</returns>
        FSM GetFSMByName(string name);
        /// <summary>
        /// 是否存在指定的状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>是否存在</returns>
        bool IsExistFSM(string name);

        /// <summary>
        /// 群体切换状态
        /// </summary>
        /// <param name="group">组名称</param>
        /// <param name="type">状态类型</param>
        void SwitchStateOfGroup(string group, Type type);
        /// <summary>
        /// 群体重生
        /// </summary>
        /// <param name="group">组名称</param>
        void RenewalOfGroup(string group);
        /// <summary>
        /// 群体完结
        /// </summary>
        /// <param name="group">组名称</param>
        void FinalOfGroup(string group);
    }
}