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
    public sealed class StepContentAsset : DataSetBase
    {
        [SerializeField] public List<StepContent> Content = new List<StepContent>();
        [SerializeField] internal int StepIDSign = 1;
        [SerializeField] internal string StepIDName = "Step";

        public override void Fill(JsonData data)
        {

        }

        public override JsonData Pack()
        {
            return null;
        }
    }
}