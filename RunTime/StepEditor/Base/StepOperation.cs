using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 步骤操作
    /// </summary>
    [Serializable]
    public sealed partial class StepOperation
    {
        #region Base Property
        /// <summary>
        /// 操作ID
        /// </summary>
        public string GUID = "";
        /// <summary>
        /// 节点的锚点
        /// </summary>
        [SerializeField] internal Vector2 Anchor = Vector2.zero;
        /// <summary>
        /// 操作执行到进入下一个操作的时间
        /// </summary>
        public float ElapseTime = 1;
        /// <summary>
        /// 立即执行模式
        /// </summary>
        public bool Instant = false;
        /// <summary>
        /// 操作目标
        /// </summary>
        public GameObject Target = null;
        [SerializeField] internal string TargetGUID = "<None>";
        [SerializeField] internal string TargetPath = "<None>";
        /// <summary>
        /// 操作简述名称
        /// </summary>
        public string Name = "New Step Operation";
        /// <summary>
        /// 操作类型
        /// </summary>
        public StepOperationType OperationType = StepOperationType.Move;
        #endregion

        #region EditorOnly
#if UNITY_EDITOR
        internal static float Width = 150;
        internal static float Height = 40;
        /// <summary>
        /// 预览的目标
        /// </summary>
        internal GameObject PreviewTarget = null;

        /// <summary>
        /// 节点的位置
        /// </summary>
        internal Rect Position
        {
            get
            {
                return new Rect(Anchor.x - Width / 2, Anchor.y - Height / 2, Width, Height);
            }
        }
        /// <summary>
        /// 左侧锚点位置
        /// </summary>
        internal Rect LeftPosition
        {
            get
            {
                return new Rect(Anchor.x - Width / 2 - 10, Anchor.y - 10, 20, 20);
            }
        }
        /// <summary>
        /// 右侧锚点位置
        /// </summary>
        internal Rect RightPosition
        {
            get
            {
                return new Rect(Anchor.x + Width / 2 - 10, Anchor.y - 10, 20, 20);
            }
        }
        /// <summary>
        /// 生成预览目标
        /// </summary>
        internal void CreatePreviewTarget()
        {
            if (!PreviewTarget)
            {
                PreviewTarget = Main.CloneGameObject(Target, Target.rectTransform());
                PreviewTarget.name = "[Preview]" + Target.name + " - " + Name;
                PreviewTarget.AddComponent<StepPreview>();
                InitPreviewTarget();
                FocusTarget();
            }
        }
        /// <summary>
        /// 删除预览目标
        /// </summary>
        internal void DeletePreviewTarget()
        {
            if (PreviewTarget)
            {
                UnityEngine.Object.DestroyImmediate(PreviewTarget);
                PreviewTarget = null;
            }
        }
        /// <summary>
        /// 焦点到操作目标
        /// </summary>
        internal void FocusTarget()
        {
            if (PreviewTarget)
            {
                if (Selection.activeGameObject != PreviewTarget)
                {
                    Selection.activeGameObject = PreviewTarget;
                    EditorGUIUtility.PingObject(PreviewTarget);
                }
            }
            else if (Target)
            {
                if (Selection.activeGameObject != Target)
                {
                    Selection.activeGameObject = Target;
                    EditorGUIUtility.PingObject(Target);
                }
            }
        }
#endif
        #endregion
    }
}