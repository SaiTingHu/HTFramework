using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// 默认的FSM管理器助手
    /// </summary>
    public sealed class DefaultFSMHelper : IFSMHelper
    {
        /// <summary>
        /// FSM管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 已注册的状态机
        /// </summary>
        public Dictionary<string, FSM> FSMs { get; private set; } = new Dictionary<string, FSM>();
        /// <summary>
        /// 已注册的状态机组
        /// </summary>
        public Dictionary<string, List<FSM>> FSMGroups { get; private set; } = new Dictionary<string, List<FSM>>();

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void RegisterFSM(FSM fsm)
        {
            if (fsm == null)
                return;

            if (!FSMs.ContainsKey(fsm.Name))
            {
                FSMs.Add(fsm.Name, fsm);

                if (FSMGroups.ContainsKey(fsm.Group))
                {
                    FSMGroups[fsm.Group].Add(fsm);
                }
                else
                {
                    FSMGroups.Add(fsm.Group, new List<FSM>());
                    FSMGroups[fsm.Group].Add(fsm);
                }
            }
            else
            {
                Log.Warning($"注册状态机失败：已存在状态机 {fsm.Name} ！");
            }
        }
        /// <summary>
        /// 移除已注册的状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void UnRegisterFSM(FSM fsm)
        {
            if (fsm == null)
                return;

            if (FSMs.ContainsKey(fsm.Name))
            {
                FSMs.Remove(fsm.Name);

                if (FSMGroups.ContainsKey(fsm.Group))
                {
                    FSMGroups[fsm.Group].Remove(fsm);
                }
            }
            else
            {
                Log.Warning($"移除已注册的状态机失败：不存在状态机 {fsm.Name} ！");
            }
        }

        /// <summary>
        /// 通过名称获取状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>状态机</returns>
        public FSM GetFSMByName(string name)
        {
            if (FSMs.ContainsKey(name))
            {
                return FSMs[name];
            }
            else
            {
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
            return FSMs.ContainsKey(name);
        }
        
        /// <summary>
        /// 群体切换状态
        /// </summary>
        /// <param name="group">组名称</param>
        /// <param name="type">状态类型</param>
        public void SwitchStateOfGroup(string group, Type type)
        {
            if (FSMGroups.ContainsKey(group))
            {
                List<FSM> fsms = FSMGroups[group];
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
            if (FSMGroups.ContainsKey(group))
            {
                List<FSM> fsms = FSMGroups[group];
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
            if (FSMGroups.ContainsKey(group))
            {
                List<FSM> fsms = FSMGroups[group];
                for (int i = 0; i < fsms.Count; i++)
                {
                    fsms[i].Final();
                }
            }
        }
    }
}