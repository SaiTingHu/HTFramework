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