using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的任务内容
    /// </summary>
    [TaskContent("Default")]
    public sealed class TaskContentDefault : TaskContentBase
    {
        public TaskContentDefault() : base()
        {

        }

        public override void OnStart()
        {
            base.OnStart();

            GlobalTools.LogInfo("任务内容：[" + Name + "]开始!");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            GlobalTools.LogInfo("任务内容：[" + Name + "]监控中......");
        }

        public override void OnExecute()
        {
            base.OnExecute();

            GlobalTools.LogInfo("任务内容：[" + Name + "]完成!");
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