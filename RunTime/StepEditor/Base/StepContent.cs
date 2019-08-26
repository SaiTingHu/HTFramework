using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤内容
    /// </summary>
    [Serializable]
    public sealed class StepContent
    {
        public string GUID = "";
        public Vector2 EnterAnchor = Vector2.zero;
        public float ElapseTime = 1;
        public bool Instant = false;
        public GameObject Target = null;
        public string TargetGUID = "<None>";
        public string TargetPath = "<None>";
        public string Name = "Step Name";
        public StepTrigger Trigger = StepTrigger.StateChange;
        public string Prompt = "Step Prompt";
        public string Ancillary = "";
        public Vector3 BestView = new Vector3(90, 30, 2);
        public Vector3 ViewOffset = Vector3.zero;
        public Vector3 BestPos = Vector3.zero;
        public ControlMode InitialMode = ControlMode.FreeControl;
        public string Helper = "<None>";

        [SerializeField]
        public List<StepParameter> Parameters = new List<StepParameter>();
        [SerializeField]
        public List<StepOperation> Operations = new List<StepOperation>();
        [SerializeField]
        public List<StepWired> Wireds = new List<StepWired>();

        /// <summary>
        /// 克隆
        /// </summary>
        public StepContent Clone()
        {
            StepContent content = new StepContent();
            content.GUID = Guid.NewGuid().ToString();
            content.EnterAnchor = EnterAnchor;
            content.ElapseTime = ElapseTime;
            content.Instant = Instant;
            content.Target = Target;
            content.TargetGUID = TargetGUID;
            content.TargetPath = TargetPath;
            content.Name = Name;
            content.Trigger = Trigger;
            content.Prompt = Prompt;
            content.Ancillary = Ancillary;
            content.BestView = BestView;
            content.ViewOffset = ViewOffset;
            content.BestPos = BestPos;
            content.InitialMode = InitialMode;
            content.Helper = Helper;
            for (int i = 0; i < Parameters.Count; i++)
            {
                content.Parameters.Add(Parameters[i].Clone());
            }
            for (int i = 0; i < Operations.Count; i++)
            {
                content.Operations.Add(Operations[i].Clone());
            }
            for (int i = 0; i < Wireds.Count; i++)
            {
                content.Wireds.Add(Wireds[i].Clone());
            }
            return content;
        }

        /// <summary>
        /// 是否存在指定连线
        /// </summary>
        public bool IsExistWired(int left, int right)
        {
            for (int i = 0; i < Wireds.Count; i++)
            {
                if (Wireds[i].Left == left && Wireds[i].Right == right)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 当前步骤包含指定类型的操作数量
        /// </summary>
        public int GetOperationsCout(StepOperationType type)
        {
            int cout = 0;
            for (int i = 0; i < Operations.Count; i++)
            {
                if (Operations[i].OperationType == type)
                {
                    cout += 1;
                }
            }
            return cout;
        }

        /// <summary>
        /// 执行步骤内容
        /// </summary>
        public void Execute(StepMaster master)
        {
            for (int i = 0; i < Wireds.Count; i++)
            {
                if (Wireds[i].Left == -1)
                {
                    Execute(master, i);
                }
            }
        }
        private void Execute(StepMaster master, int index)
        {
            int stepIndex = Wireds[index].Right;
            Operations[stepIndex].Execute();

            if (Operations[stepIndex].Instant)
            {
                for (int i = 0; i < Wireds.Count; i++)
                {
                    if (Wireds[i].Left == stepIndex)
                    {
                        Execute(master, i);
                    }
                }
            }
            else
            {
                master.DelayExecute(() =>
                {
                    for (int i = 0; i < Wireds.Count; i++)
                    {
                        if (Wireds[i].Left == stepIndex)
                        {
                            Execute(master, i);
                        }
                    }
                }, Operations[stepIndex].ElapseTime);
            }
        }

        /// <summary>
        /// 跳过步骤内容
        /// </summary>
        public void Skip(StepMaster master)
        {
            for (int i = 0; i < Wireds.Count; i++)
            {
                if (Wireds[i].Left == -1)
                {
                    Skip(master, i);
                }
            }
        }
        private void Skip(StepMaster master, int index)
        {
            int stepIndex = Wireds[index].Right;
            Operations[stepIndex].Skip();

            if (Operations[stepIndex].Instant)
            {
                for (int i = 0; i < Wireds.Count; i++)
                {
                    if (Wireds[i].Left == stepIndex)
                    {
                        Skip(master, i);
                    }
                }
            }
            else
            {
                master.DelayExecute(() =>
                {
                    for (int i = 0; i < Wireds.Count; i++)
                    {
                        if (Wireds[i].Left == stepIndex)
                        {
                            Skip(master, i);
                        }
                    }
                }, Operations[stepIndex].ElapseTime / StepMaster.SkipMultiple);
            }
        }
    }
}