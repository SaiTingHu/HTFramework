using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态机管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.FSM)]
    public sealed class FSMManager : InternalModuleBase
    {
        private Dictionary<string, FSM> _fsms = new Dictionary<string, FSM>();
        private Dictionary<string, List<FSM>> _fsmGroups = new Dictionary<string, List<FSM>>();

        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void RegisterFSM(FSM fsm)
        {
            if (!_fsms.ContainsKey(fsm.Name))
            {
                _fsms.Add(fsm.Name, fsm);

                if (_fsmGroups.ContainsKey(fsm.Group))
                {
                    _fsmGroups[fsm.Group].Add(fsm);
                }
                else
                {
                    _fsmGroups.Add(fsm.Group, new List<FSM>());
                    _fsmGroups[fsm.Group].Add(fsm);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "注册状态机失败：已存在状态机 " + fsm.Name + " ！");
            }
        }
        /// <summary>
        /// 移除已注册的状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void UnRegisterFSM(FSM fsm)
        {
            if (_fsms.ContainsKey(fsm.Name))
            {
                _fsms.Remove(fsm.Name);

                if (_fsmGroups.ContainsKey(fsm.Group))
                {
                    _fsmGroups[fsm.Group].Remove(fsm);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "移除已注册的状态机失败：不存在状态机 " + fsm.Name + " ！");
            }
        }

        /// <summary>
        /// 通过名称获取状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>状态机</returns>
        public FSM GetFSMByName(string name)
        {
            if (_fsms.ContainsKey(name))
            {
                return _fsms[name];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "获取状态机失败：不存在状态机 " + name + " ！");
            }
        }
        /// <summary>
        /// 是否存在指定的状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistFSM(string name)
        {
            return _fsms.ContainsKey(name);
        }

        /// <summary>
        /// 群体切换状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <param name="group">组名称</param>
        public void SwitchStateOfGroup<T>(string group) where T : FiniteStateBase
        {
            SwitchStateOfGroup(group, typeof(T));
        }
        /// <summary>
        /// 群体切换状态
        /// </summary>
        /// <param name="group">组名称</param>
        /// <param name="type">状态类型</param>
        public void SwitchStateOfGroup(string group, Type type)
        {
            if (_fsmGroups.ContainsKey(group))
            {
                List<FSM> fsms = _fsmGroups[group];
                for (int i = 0; i < fsms.Count; i++)
                {
                    fsms[i].SwitchState(type);
                }
            }
        }

        /// <summary>
        /// 群体重生
        /// </summary>
        /// <param name="group">组名称</param>
        public void RenewalOfGroup(string group)
        {
            if (_fsmGroups.ContainsKey(group))
            {
                List<FSM> fsms = _fsmGroups[group];
                for (int i = 0; i < fsms.Count; i++)
                {
                    fsms[i].Renewal();
                }
            }
        }
        /// <summary>
        /// 群体完结
        /// </summary>
        /// <param name="group">组名称</param>
        public void FinalOfGroup(string group)
        {
            if (_fsmGroups.ContainsKey(group))
            {
                List<FSM> fsms = _fsmGroups[group];
                for (int i = 0; i < fsms.Count; i++)
                {
                    fsms[i].Final();
                }
            }
        }
    }
}