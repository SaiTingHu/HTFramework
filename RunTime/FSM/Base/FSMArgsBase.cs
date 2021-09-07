using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态机参数基类
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class FSMArgsBase : HTBehaviour
    {
        /// <summary>
        /// 所属状态机
        /// </summary>
        public FSM StateMachine { get; internal set; }
        /// <summary>
        /// 禁用自身自动化，参数的自动化由状态机来完成
        /// </summary>
        protected sealed override bool IsAutomate => false;
    }
}