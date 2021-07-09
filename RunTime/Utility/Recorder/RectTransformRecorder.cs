using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// RectTransform 组件属性记录器
    /// </summary>
    public sealed class RectTransformRecorder : PropertyRecorder<RectTransform>
    {
        /// <summary>
        /// 记录的父级
        /// </summary>
        public Transform Parent { get; private set; }
        /// <summary>
        /// 记录的锚点坐标
        /// </summary>
        public Vector2 AnchoredPosition { get; private set; }
        /// <summary>
        /// 记录的中心点
        /// </summary>
        public Vector2 Pivot { get; private set; }
        /// <summary>
        /// 记录的大小
        /// </summary>
        public Vector2 SizeDelta { get; private set; }
        /// <summary>
        /// 记录的最大偏移值
        /// </summary>
        public Vector2 OffsetMax { get; private set; }
        /// <summary>
        /// 记录的最小偏移值
        /// </summary>
        public Vector2 OffsetMin { get; private set; }
        /// <summary>
        /// 记录的最大锚点
        /// </summary>
        public Vector2 AnchorMax { get; private set; }
        /// <summary>
        /// 记录的最小锚点
        /// </summary>
        public Vector2 AnchorMin { get; private set; }
        /// <summary>
        /// 记录的局部旋转
        /// </summary>
        public Quaternion LocalRotation { get; private set; }
        /// <summary>
        /// 记录的局部缩放
        /// </summary>
        public Vector3 LocalScale { get; private set; }

        private bool _isRecordParent;

        public RectTransformRecorder(RectTransform target, bool isRecordParent = false) : base(target)
        {
            _isRecordParent = isRecordParent;

            Rerecord();
        }

        /// <summary>
        /// 重新记录
        /// </summary>
        /// <param name="newTarget">新的目标，为空则继续记录原目标，不为空则重新记录新目标</param>
        public override void Rerecord(RectTransform newTarget = null)
        {
            base.Rerecord(newTarget);

            if (IsValid)
            {
                Parent = _isRecordParent ? Target.parent : null;
                AnchoredPosition = Target.anchoredPosition;
                Pivot = Target.pivot;
                SizeDelta = Target.sizeDelta;
                OffsetMax = Target.offsetMax;
                OffsetMin = Target.offsetMin;
                AnchorMax = Target.anchorMax;
                AnchorMin = Target.anchorMin;
                LocalRotation = Target.localRotation;
                LocalScale = Target.localScale;
            }
        }
        /// <summary>
        /// 还原
        /// </summary>
        public override void Recovery()
        {
            base.Recovery();

            if (IsValid)
            {
                if (_isRecordParent)
                {
                    Target.SetParent(Parent);
                }
                Target.anchoredPosition = AnchoredPosition;
                Target.pivot = Pivot;
                Target.sizeDelta = SizeDelta;
                Target.offsetMax = OffsetMax;
                Target.offsetMin = OffsetMin;
                Target.anchorMax = AnchorMax;
                Target.anchorMin = AnchorMin;
                Target.localRotation = LocalRotation;
                Target.localScale = LocalScale;
            }
        }
        /// <summary>
        /// 将记录的内容赋予其他目标
        /// </summary>
        /// <param name="other">其他目标</param>
        public override void AttachTo(RectTransform other)
        {
            base.AttachTo(other);

            if (IsValid && other)
            {
                if (_isRecordParent)
                {
                    other.SetParent(Parent);
                }
                other.anchoredPosition = AnchoredPosition;
                other.pivot = Pivot;
                other.sizeDelta = SizeDelta;
                other.offsetMax = OffsetMax;
                other.offsetMin = OffsetMin;
                other.anchorMax = AnchorMax;
                other.anchorMin = AnchorMin;
                other.localRotation = LocalRotation;
                other.localScale = LocalScale;
            }
        }
        /// <summary>
        /// 销毁记录器
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            Parent = null;
        }
    }
}