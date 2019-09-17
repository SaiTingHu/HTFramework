using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤内容序列化资源
    /// </summary>
    [CreateAssetMenu(menuName = "HTFramework/Step Content Asset", order = 0)]
    [Serializable]
    public sealed class StepContentAsset : ScriptableObject
    {
        [SerializeField]
        public List<StepContent> Content = new List<StepContent>();
        public int StepIDSign = 1;
        public string StepIDName = "Step";
    }
}
