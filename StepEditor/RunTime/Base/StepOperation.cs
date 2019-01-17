using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤操作
    /// </summary>
    [Serializable]
    public sealed partial class StepOperation
    {
        public string GUID = "";
        public Vector2 Anchor = Vector2.zero;
        public float ElapseTime = 1;
        public bool Instant = false;
        public GameObject Target = null;
        public string TargetGUID = "<None>";
        public string TargetPath = "<None>";
        public string Name = "New Step Operation";
        public StepOperationType OperationType = StepOperationType.Move;
    }
}
