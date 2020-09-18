using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
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
        /// <param name="stepContent">步骤内容</param>
        /// <param name="operationIndex">步骤操作的索引</param>
        internal void CreatePreviewTarget(StepContent stepContent, int operationIndex)
        {
            if (!PreviewTarget)
            {
                PreviewTarget = Main.CloneGameObject(Target, Target.rectTransform());
                PreviewTarget.name = "[Preview]" + Target.name + " - " + Name;
                PreviewTarget.AddComponent<StepPreview>();
                SetPreviewTransform(stepContent, operationIndex);
                InitPreviewTarget(stepContent);
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
        /// 设置预览目标的初始变换
        /// </summary>
        /// <param name="stepContent">步骤内容</param>
        /// <param name="operationIndex">步骤操作的索引</param>
        internal void SetPreviewTransform(StepContent stepContent, int operationIndex)
        {
            //已读取的节点
            HashSet<int> alreadyReads = new HashSet<int>();
            //是否已设置预览目标位置
            bool isSetPosition = false;
            //是否已设置预览目标旋转
            bool isSetRotation = false;
            //是否已设置预览目标缩放
            bool isSetScale = false;
            //当前的连线右端节点
            int index = operationIndex;

            while (true)
            {
                //已经完成预览目标的变换设置，结束遍历
                if (isSetPosition && isSetRotation && isSetScale)
                {
                    break;
                }
                //标记节点已读取
                alreadyReads.Add(index);
                //读取连线左侧节点
                int leftIndex = stepContent.GetLeftIndex(index);
                //左侧存在节点
                if (leftIndex >= 0)
                {
                    //如果重复读取到相同节点，则存在循环连线，结束遍历
                    if (alreadyReads.Contains(leftIndex))
                    {
                        break;
                    }

                    //获取左侧节点，并设置位置、旋转、缩放
                    StepOperation operation = stepContent.Operations[leftIndex];
                    if (operation.Target == Target)
                    {
                        if (!isSetPosition)
                        {
                            if (operation.OperationType == StepOperationType.Move)
                            {
                                PreviewTarget.transform.localPosition = operation.Vector3Value;
                                isSetPosition = true;
                            }
                            else if (operation.OperationType == StepOperationType.Transform)
                            {
                                PreviewTarget.transform.localPosition = operation.Vector3Value;
                                isSetPosition = true;
                            }
                        }
                        if (!isSetRotation)
                        {
                            if (operation.OperationType == StepOperationType.Rotate)
                            {
                                PreviewTarget.transform.localRotation = operation.Vector3Value.ToQuaternion();
                                isSetRotation = true;
                            }
                            else if (operation.OperationType == StepOperationType.Transform)
                            {
                                PreviewTarget.transform.localRotation = operation.Vector3Value2.ToQuaternion();
                                isSetRotation = true;
                            }
                        }
                        if (!isSetScale)
                        {
                            if (operation.OperationType == StepOperationType.Scale)
                            {
                                PreviewTarget.transform.localScale = operation.Vector3Value;
                                isSetScale = true;
                            }
                            else if (operation.OperationType == StepOperationType.Transform)
                            {
                                PreviewTarget.transform.localScale = operation.Vector3Value3;
                                isSetScale = true;
                            }
                        }
                    }
                    index = leftIndex;
                }
                //左侧已不存在节点，结束遍历
                else
                {
                    break;
                }
            }

            //未找到连线中的移动相关节点，设置位置为初始值
            if (!isSetPosition)
            {
                PreviewTarget.transform.localPosition = Target.transform.localPosition;
            }
            //未找到连线中的旋转相关节点，设置旋转为初始值
            if (!isSetRotation)
            {
                PreviewTarget.transform.localRotation = Target.transform.localRotation;
            }
            //未找到连线中的缩放相关节点，设置缩放为初始值
            if (!isSetScale)
            {
                PreviewTarget.transform.localScale = Target.transform.localScale;
            }

            alreadyReads = null;
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