using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的任务点
    /// </summary>
    [TaskPoint("Default")]
    public sealed class TaskPointDefault : TaskPointBase
    {
        public TaskPointDefault() : base()
        {

        }

        public override void OnStart()
        {
            base.OnStart();

            GlobalTools.LogInfo("任务点：[" + Name + "]开始!");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            IsDone = true;

            GlobalTools.LogInfo("任务点：[" + Name + "]监控中......");
        }

        public override void OnExecute()
        {
            base.OnExecute();

            GlobalTools.LogInfo("任务点：[" + Name + "]完成!");
        }
        
#if UNITY_EDITOR
        public override int OnPropertyGUI()
        {
            int height = base.OnPropertyGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("[" + GetType().Name + "]");
            GUILayout.EndHorizontal();

            height += 20;

            return height;
        }
#endif
    }
}