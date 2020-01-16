using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务游戏物体（场景中的）
    /// </summary>
    [Serializable]
    public sealed class TaskGameObject
    {
        /// <summary>
        /// 游戏物体实体
        /// </summary>
        public GameObject Entity;
        /// <summary>
        /// 游戏物体ID
        /// </summary>
        [SerializeField] internal string GUID = "<None>";
        /// <summary>
        /// 游戏物体路径
        /// </summary>
        [SerializeField] internal string Path = "";
    }
}