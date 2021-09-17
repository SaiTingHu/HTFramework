using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 默认的任务点
    /// </summary>
    [TaskPoint("默认")]
    public sealed class TaskPointDefault : TaskPointBase
    {
        [SerializeField] private TaskTrigger _trigger = TaskTrigger.MouseClick;
        [SerializeField] private bool _highlighting = true;
        [SerializeField] private float _duration = 1;
        
        protected override void OnStart()
        {
            base.OnStart();

            if (GetTaskTarget != null)
            {
                if (_trigger == TaskTrigger.StateChange)
                {
                    GetTaskTarget.State = TaskTargetState.Normal;
                }
            }
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();

            switch (_trigger)
            {
                case TaskTrigger.MouseClick:
                    if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
                    {
                        if (Main.m_Controller.RayTargetObj && Main.m_Controller.RayTargetObj == GetTarget)
                        {
                            Complete();
                        }
                    }
                    break;
                case TaskTrigger.StateChange:
                    if (GetTaskTarget != null && GetTaskTarget.State == TaskTargetState.Done)
                    {
                        Complete();
                    }
                    break;
            }
        }
        protected override void OnGuide()
        {
            base.OnGuide();

            if (_highlighting && GetTarget)
            {
                Collider collider = GetTarget.GetComponent<Collider>();
                if (collider && collider.enabled)
                {
                    switch (Main.m_TaskMaster.GuideHighlighting)
                    {
                        case MouseRay.HighlightingType.Normal:
                            GetTarget.OpenHighLight(Main.m_TaskMaster.NormalColor);
                            break;
                        case MouseRay.HighlightingType.Flash:
                            GetTarget.OpenFlashHighLight(Main.m_TaskMaster.FlashColor1, Main.m_TaskMaster.FlashColor2);
                            break;
                        case MouseRay.HighlightingType.Outline:
                            GetTarget.OpenMeshOutline(Main.m_TaskMaster.NormalColor, Main.m_TaskMaster.OutlineIntensity);
                            break;
                    }
                }
            }
        }
        protected override IEnumerator OnComplete()
        {
            yield return base.OnComplete();

            if (!_duration.Approximately(0))
            {
                yield return YieldInstructioner.GetWaitForSeconds(_duration);
            }
        }

        /// <summary>
        /// 默认的任务点触发类型
        /// </summary>
        public enum TaskTrigger
        {
            /// <summary>
            /// 鼠标点击目标触发
            /// </summary>
            MouseClick,
            /// <summary>
            /// 目标状态变为Done时触发
            /// </summary>
            StateChange
        }

#if UNITY_EDITOR
        protected override int OnPropertyGUI()
        {
            int height = base.OnPropertyGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("触发方式:", GUILayout.Width(90));
            _trigger = (TaskTrigger)EditorGUILayout.EnumPopup(_trigger);
            GUILayout.EndHorizontal();

            height += 20;

            GUILayout.BeginHorizontal();
            GUILayout.Label("指引时高亮目标:", GUILayout.Width(90));
            _highlighting = EditorGUILayout.Toggle(_highlighting);
            GUILayout.EndHorizontal();

            height += 20;

            GUILayout.BeginHorizontal();
            GUILayout.Label("持续时间:", GUILayout.Width(90));
            _duration = EditorGUILayout.FloatField(_duration);
            GUILayout.EndHorizontal();

            height += 20;

            return height;
        }
#endif
    }
}