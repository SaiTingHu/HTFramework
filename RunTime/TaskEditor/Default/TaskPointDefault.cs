using System.Collections;
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

        protected override void OnStart()
        {
            base.OnStart();

            Log.Info("任务点：[" + Name + "]开始!");
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            Complete();

            Log.Info("任务点：[" + Name + "]监控中......");
        }

        protected override IEnumerator OnBeforeComplete()
        {
            yield return base.OnBeforeComplete();

            Log.Info("任务点：[" + Name + "]完成!");
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