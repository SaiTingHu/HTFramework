using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// Transform 组件属性记录器
    /// </summary>
    public sealed class TransformRecorder : PropertyRecorder<Transform>
    {
        /// <summary>
        /// 记录的父级
        /// </summary>
        public Transform Parent { get; private set; }
        /// <summary>
        /// 记录的局部位置
        /// </summary>
        public Vector3 LocalPosition { get; private set; }
        /// <summary>
        /// 记录的局部旋转
        /// </summary>
        public Quaternion LocalRotation { get; private set; }
        /// <summary>
        /// 记录的局部缩放
        /// </summary>
        public Vector3 LocalScale { get; private set; }

        private bool _isRecordParent;

        public TransformRecorder(Transform target, bool isRecordParent = false) : base(target)
        {
            _isRecordParent = isRecordParent;

            Rerecord();
        }

        /// <summary>
        /// 重新记录
        /// </summary>
        /// <param name="newTarget">新的目标，为空则继续记录原目标，不为空则重新记录新目标</param>
        public override void Rerecord(Transform newTarget = null)
        {
            base.Rerecord(newTarget);

            if (IsValid)
            {
                Parent = _isRecordParent ? Target.parent : null;
                LocalPosition = Target.localPosition;
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
                Target.localPosition = LocalPosition;
                Target.localRotation = LocalRotation;
                Target.localScale = LocalScale;
            }
        }
        /// <summary>
        /// 将记录的内容赋予其他目标
        /// </summary>
        /// <param name="other">其他目标</param>
        public override void AttachTo(Transform other)
        {
            base.AttachTo(other);

            if (IsValid && other)
            {
                if (_isRecordParent)
                {
                    other.SetParent(Parent);
                }
                other.localPosition = LocalPosition;
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