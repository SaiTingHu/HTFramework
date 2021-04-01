using System;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态机管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.FSM)]
    public sealed class FSMManager : InternalModuleBase<IFSMHelper>
    {
        private FSMManager()
        {

        }

        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void RegisterFSM(FSM fsm)
        {
            _helper.RegisterFSM(fsm);
        }
        /// <summary>
        /// 移除已注册的状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void UnRegisterFSM(FSM fsm)
        {
            _helper.UnRegisterFSM(fsm);
        }

        /// <summary>
        /// 通过名称获取状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>状态机</returns>
        public FSM GetFSMByName(string name)
        {
            return _helper.GetFSMByName(name);
        }
        /// <summary>
        /// 是否存在指定的状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistFSM(string name)
        {
            return _helper.IsExistFSM(name);
        }

        /// <summary>
        /// 群体切换状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="group">组名称</param>
        public void SwitchStateOfGroup<T>(string group) where T : FiniteStateBase
        {
            _helper.SwitchStateOfGroup(group, typeof(T));
        }
        /// <summary>
        /// 群体切换状态
        /// </summary>
        /// <param name="group">组名称</param>
        /// <param name="type">状态类型</param>
        public void SwitchStateOfGroup(string group, Type type)
        {
            _helper.SwitchStateOfGroup(group, type);
        }

        /// <summary>
        /// 群体重生
        /// </summary>
        /// <param name="group">组名称</param>
        public void RenewalOfGroup(string group)
        {
            _helper.RenewalOfGroup(group);
        }
        /// <summary>
        /// 群体完结
        /// </summary>
        /// <param name="group">组名称</param>
        public void FinalOfGroup(string group)
        {
            _helper.FinalOfGroup(group);
        }
    }
}