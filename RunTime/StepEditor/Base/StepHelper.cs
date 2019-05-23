using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤的助手，步骤开始时自动创建，结束后自动销毁
    /// </summary>
    public abstract class StepHelper
    {
        /// <summary>
        /// 步骤目标
        /// </summary>
        public StepTarget Target;

        /// <summary>
        /// 助手当前执行的任务
        /// </summary>
        public StepHelperTask Task;

        /// <summary>
        /// 步骤辅助目标
        /// </summary>
        public List<GameObject> AuxiliaryTarget = new List<GameObject>();

        /// <summary>
        /// 跳过时生命周期（仅在跳过时生效）
        /// </summary>
        public virtual float SkipLifeTime
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 跳过步骤（仅在跳过时执行）
        /// </summary>
        public virtual void OnSkip()
        {

        }

        /// <summary>
        /// 恢复步骤（仅在倒退时执行）
        /// </summary>
        public virtual void OnRestore()
        {

        }

        /// <summary>
        /// 步骤指引
        /// </summary>
        public virtual void OnGuide()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {

        }

        /// <summary>
        /// 帧刷新（仅在步骤执行前生效）
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void OnTermination()
        {

        }

        /// <summary>
        /// 步骤执行
        /// </summary>
        protected void Execute()
        {
            Target.State = StepTargetState.Done;
        }
    }

    /// <summary>
    /// 步骤助手的任务
    /// </summary>
    public enum StepHelperTask
    {
        /// <summary>
        /// 常规执行任务
        /// </summary>
        Execute,
        /// <summary>
        /// 跳过任务
        /// </summary>
        Skip,
        /// <summary>
        /// 恢复任务
        /// </summary>
        Restore
    }
}
