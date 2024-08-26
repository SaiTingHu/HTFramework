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
        /// 鼠标左键点击音效
        /// </summary>
        public AudioClip OnMouseClickSound;
        /// <summary>
        /// 鼠标左键点击事件
        /// </summary>
        public UnityEvent OnMouseClick;

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
    }
}