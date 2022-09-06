﻿using System;
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
        [SerializeField] internal List<StepContent> Content = new List<StepContent>();
        [SerializeField] internal int StepIDSign = 1;
        [SerializeField] internal string StepIDName = "Step";

        /// <summary>
        /// 获取步骤内容
        /// </summary>
        public List<StepContent> GetContent
        {
            get
            {
                return Content;
            }
        }
    }
}