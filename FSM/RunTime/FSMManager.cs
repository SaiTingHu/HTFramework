using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态机管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FSMManager : ModuleManager
    {
        private List<FSM> _fsms = new List<FSM>();

        /// <summary>
        /// 注册状态机
        /// </summary>
        public void RegisterFSM(FSM fsm)
        {
            if (!_fsms.Contains(fsm))
            {
                if (IsExistFSM(fsm.Name))
                {
                    GlobalTools.LogError("注册状态机失败：已存在状态机 " + fsm.Name + "！");
                    return;
                }
                _fsms.Add(fsm);
            }
            else
            {
                GlobalTools.LogError("注册状态机失败：已存在状态机 " + fsm.Name + "！");
            }
        }

        /// <summary>
        /// 移除已注册的状态机
        /// </summary>
        public void UnRegisterFSM(FSM fsm)
        {
            if (_fsms.Contains(fsm))
            {
                _fsms.Remove(fsm);
            }
            else
            {
                GlobalTools.LogError("移除已注册的状态机失败：不存在状态机 " + fsm.Name + "！");
            }
        }

        /// <summary>
        /// 通过名称获取状态机
        /// </summary>
        public FSM GetFSMByName(string name)
        {
            return _fsms.Find((fsm) => { return fsm.Name == name; });
        }

        /// <summary>
        /// 是否存在指定的状态机
        /// </summary>
        public bool IsExistFSM(string name)
        {
            for (int i = 0; i < _fsms.Count; i++)
            {
                if (_fsms[i].Name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
