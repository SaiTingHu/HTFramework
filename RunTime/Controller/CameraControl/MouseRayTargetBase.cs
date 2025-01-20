using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        private void OnDrawGizmosSelected()
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
#endif
    }
}