using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的任务内容
    /// </summary>
    [TaskContent("默认")]
    public sealed class TaskContentDefault : TaskContentBase
    {
        protected override void OnStart()
        {
            base.OnStart();

            Log.Info("任务内容：[" + Name + "]开始!");
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();

            Log.Info("任务内容：[" + Name + "]监控中......");
        }
        protected override void OnComplete()
        {
            base.OnComplete();

            Log.Info("任务内容：[" + Name + "]完成!");
        }

#if UNITY_EDITOR
        protected override int OnPropertyGUI()
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