using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务控制者
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.TaskEditor)]
    public sealed class TaskMaster : InternalModuleBase
    {
        /// <summary>
        /// 任务资源
        /// </summary>
        public TaskContentAsset ContentAsset;

        private TaskContentBase _currentContent;

    }
}