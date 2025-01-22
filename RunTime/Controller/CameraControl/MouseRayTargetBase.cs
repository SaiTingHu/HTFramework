using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 鼠标射线可捕获的目标
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class MouseRayTargetBase : HTBehaviour
    {
        /// <summary>
        /// 所有目标集合
        /// </summary>
        public static HashSet<MouseRayTargetBase> AllTargetSet { get; private set; } = new HashSet<MouseRayTargetBase>();

        /// <summary>
        /// 目标显示的名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 目标是否为步骤目标，若为步骤目标，则在步骤流程中点错会触发错误事件
        /// </summary>
        public bool IsStepTarget = false;
        /// <summary>
        /// 是否开启提示，当目标被射线捕获时
        /// </summary>
        public bool IsOpenPrompt = true;
        /// <summary>
        /// 是否开启高亮，当目标被射线捕获时
        /// </summary>
        public bool IsOpenHighlight = true;
        /// <summary>
        /// 注视时视角（自由视角）
        /// </summary>
        public Vector3 LookAtAngle = Vector3.zero;
        /// <summary>
        /// 鼠标左键点击音效
        /// </summary>
        public AudioClip OnMouseClickSound;
        /// <summary>
        /// 鼠标左键点击事件
        /// </summary>
        public UnityEvent OnMouseClick;

        protected override void Awake()
        {
            base.Awake();

            AllTargetSet.Add(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            AllTargetSet.Remove(this);
        }
        public override bool OnSafetyCheck(params object[] args)
        {
            if (!base.OnSafetyCheck(args))
            {
                return false;
            }

            if (IsOpenHighlight && transform.GetComponent<Collider>())
            {
                if (transform.parent)
                {
                    MouseRayTargetBase rayTargetP = transform.parent.GetComponentInParent<MouseRayTargetBase>();
                    if (rayTargetP && rayTargetP.IsOpenHighlight && rayTargetP.GetComponent<Collider>())
                    {
                        SafetyChecker.DoSafetyWarning($"由于高亮及轮廓发光组件的限制，物体【{transform.FullName()}】已挂载鼠标射线捕获目标组件【MouseRayTargetBase】，其父级物体【{rayTargetP.transform.name}】不应再挂载该组件！");
                        return false;
                    }
                }
            }
            return true;
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            if (LookAtAngle.Approximately(Vector3.zero))
                return;

            Gizmos.color = Color.white;
            Quaternion rot = Quaternion.Euler(LookAtAngle.y, LookAtAngle.x, 0f);
            Vector3 dis = new Vector3(0f, 0f, LookAtAngle.z < 0 ? 0 : -LookAtAngle.z);
            Vector3 camera = transform.position + rot * dis;
            Gizmos.DrawLine(transform.position, camera);
            Gizmos.DrawIcon(camera, "ViewToolOrbit On@2x");
        }
        protected void ShowOrHideGizmos(string className, bool isBuiltin)
        {
            Type type = Type.GetType("UnityEditor.AnnotationWindow,UnityEditor");
            FieldInfo annotations = type.GetField(isBuiltin ? "m_BuiltinAnnotations" : "m_ScriptAnnotations", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo showAtPosition = type.GetMethod("ShowAtPosition", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo setGizmoState = type.GetMethod("SetGizmoState", BindingFlags.Instance | BindingFlags.NonPublic);

            showAtPosition.Invoke(null, new object[] { Rect.zero, true });
            EditorWindow window = EditorWindow.GetWindow(type);
            List<GizmoInfo> gizmoInfos = annotations.GetValue(window) as List<GizmoInfo>;
            GizmoInfo gizmoInfo = gizmoInfos.Find((g) => { return g.name == className; });
            gizmoInfo.gizmoEnabled = !gizmoInfo.gizmoEnabled;
            setGizmoState.Invoke(window, new object[] { gizmoInfo, true });
            window.Close();
        }

        [Button("Show/Hide This Gizmos", ButtonAttribute.EnableMode.Always)]
        protected void ShowOrHideThisGizmos()
        {
            ShowOrHideGizmos(GetType().Name, false);
        }
        [Button("Show/Hide Collider Gizmos", ButtonAttribute.EnableMode.Always)]
        protected void ShowOrHideColliderGizmos()
        {
            Collider collider = GetComponent<Collider>();
            if (collider)
            {
                ShowOrHideGizmos(collider.GetType().Name, true);
            }
        }
        [Button("Look At This", ButtonAttribute.EnableMode.Playmode)]
        protected void LookAtThis()
        {
            if (Main.m_Controller)
            {
                Main.m_Controller.SetLookPoint(transform.position);
                if (!LookAtAngle.Approximately(Vector3.zero))
                {
                    Main.m_Controller.SetLookAngle(LookAtAngle);
                }
            }
        }
#endif
    }
}