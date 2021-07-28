using System.Collections;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的任务点
    /// </summary>
    [TaskPoint("默认")]
    public sealed class TaskPointDefault : TaskPointBase
    {
        protected override void OnStart()
        {
            base.OnStart();

            Log.Info("任务点：[" + Name + "]开始!");
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            Log.Info("任务点：[" + Name + "]监控中......");
        }
        protected override void OnGuide()
        {
            base.OnGuide();

            Log.Info("任务点：[" + Name + "]指引!");
        }
        protected override IEnumerator OnComplete()
        {
            yield return base.OnComplete();

            Log.Info("任务点：[" + Name + "]完成!");
        }
        protected override void OnAutoComplete()
        {
            base.OnAutoComplete();

            Log.Info("任务点：[" + Name + "]自动完成!");
        }
        protected override void OnEnd()
        {
            base.OnEnd();

            Log.Info("任务点：[" + Name + "]结束!");
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