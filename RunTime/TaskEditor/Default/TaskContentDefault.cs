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