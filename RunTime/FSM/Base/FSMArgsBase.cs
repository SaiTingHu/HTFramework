using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态机参数基类
    /// </summary>
    public abstract class FSMArgsBase : ScriptableObject
    {
        /// <summary>
        /// 所属状态机
        /// </summary>
        public FSM StateMachine { get; internal set; }
    }
}