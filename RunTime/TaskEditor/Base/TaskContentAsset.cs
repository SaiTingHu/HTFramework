using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务内容序列化资源
    /// </summary>
    [CreateAssetMenu(menuName = "HTFramework/Task Content Asset", order = 0)]
    [Serializable]
    public sealed class TaskContentAsset : DataSetBase
    {
        [SerializeField] public List<TaskContentBase> Content = new List<TaskContentBase>();
        [SerializeField] internal int TaskIDSign = 1;
        [SerializeField] internal string TaskIDName = "Task";
        [SerializeField] internal int TaskPointIDSign = 1;

        public override void Fill(JsonData data)
        {
            
        }

        public override JsonData Pack()
        {
            return null;
        }
    }
}