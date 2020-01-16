using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务游戏物体目标
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TaskTarget : MonoBehaviour
    {
        /// <summary>
        /// 任务目标ID
        /// </summary>
        public string GUID = "<None>";
    }
}