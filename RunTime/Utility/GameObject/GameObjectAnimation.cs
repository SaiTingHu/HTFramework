using DG.Tweening;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 游戏物体动画
    /// </summary>
    [AddComponentMenu("HTFramework/GameObject/GameObject Animation")]
    public sealed class GameObjectAnimation : HTBehaviour
    {
        public GameObjectAnimationType TheAnimationType = GameObjectAnimationType.Move;
        public Ease TheEase = Ease.Linear;
        public LoopType TheLoopType = LoopType.Restart;
        public RotateMode TheRotateMode = RotateMode.LocalAxisAdd;
        public Vector3 TheTargetVector3 = Vector3.zero;
        public float TheTime = 1;
        public int TheLoops = -1;
        public bool PlayOnStart = true;

        private Vector3 _positionRecord;
        private Vector3 _rotationRecord;
        private Vector3 _scaleRecord;
        private Tweener _theTweener;

        /// <summary>
        /// 动画是否播放中
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                if (_theTweener != null)
                {
                    return _theTweener.IsPlaying();
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 动画是否暂停中
        /// </summary>
        public bool IsPause { get; private set; } = false;
        
        protected override void Awake()
        {
            base.Awake();

            _positionRecord = transform.localPosition;
            _rotationRecord = transform.localRotation.eulerAngles;
            _scaleRecord = transform.localScale;
        }
        private void Start()
        {
            if (PlayOnStart)
            {
                Play();
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        public void Play()
        {
            if (_theTweener != null)
            {
                _theTweener.Kill();
                _theTweener = null;
            }

            switch (TheAnimationType)
            {
                case GameObjectAnimationType.Move:
                    _theTweener = transform.DOLocalMove(TheTargetVector3, TheTime).SetEase(TheEase).SetLoops(TheLoops, TheLoopType);
                    break;
                case GameObjectAnimationType.Rotate:
                    _theTweener = transform.DOLocalRotate(TheTargetVector3, TheTime, TheRotateMode).SetEase(TheEase).SetLoops(TheLoops, TheLoopType);
                    break;
                case GameObjectAnimationType.Scale:
                    _theTweener = transform.DOScale(TheTargetVector3, TheTime).SetEase(TheEase).SetLoops(TheLoops, TheLoopType);
                    break;
                default:
                    break;
            }

            IsPause = false;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (_theTweener != null)
            {
                _theTweener.Pause();
            }

            IsPause = true;
        }
        /// <summary>
        /// 继续播放
        /// </summary>
        public void Resume()
        {
            if (_theTweener != null)
            {
                _theTweener.Play();
            }

            IsPause = false;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (_theTweener != null)
            {
                _theTweener.Kill();
                _theTweener = null;
            }

            transform.localPosition = _positionRecord;
            transform.localRotation = _rotationRecord.ToQuaternion();
            transform.localScale = _scaleRecord;

            IsPause = false;
        }
    }

    public enum GameObjectAnimationType
    {
        Move,
        Rotate,
        Scale
    }
}