using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态机管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FSMManager : ModuleManagerBase
    {
        private Dictionary<string, FSM> _fsms = new Dictionary<string, FSM>();

        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void RegisterFSM(FSM fsm)
        {
            if (!_fsms.ContainsKey(fsm.Name))
            {
                _fsms.Add(fsm.Name, fsm);
            }
            else
            {
                GlobalTools.LogWarning(string.Format("注册状态机失败：已存在状态机 {0}！", fsm.Name));
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
            }
            else
            {
                GlobalTools.LogWarning(string.Format("移除已注册的状态机失败：不存在状态机 {0}！", fsm.Name));
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
                GlobalTools.LogError(string.Format("获取状态机失败：不存在状态机 {0}！", name));
                return null;
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
    }
}
