using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UI控件动画
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIAnimation")]
    public sealed class UIAnimation : HTBehaviour
    {
        public UIAnimationType TheAnimationType = UIAnimationType.Move;
        public Ease TheEase = Ease.Linear;
        public LoopType TheLoopType = LoopType.Restart;
        public RotateMode TheRotateMode = RotateMode.LocalAxisAdd;
        public Vector3 TheTargetVector3 = Vector3.zero;
        public float TheTargetFloat = 0;
        public Color TheTargetColor = Color.white;
        public float TheTime = 1;
        public int TheLoops = -1;
        public bool PlayOnStart = true;

        private RectTransform _rectTransform;
        private Graphic _theGraphic;
        private Vector3 _positionRecord;
        private Vector3 _rotationRecord;
        private Vector3 _scaleRecord;
        private Color _colorRecord;
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

            _rectTransform = transform.rectTransform();
            _theGraphic = GetComponent<Graphic>();
            _positionRecord = transform.localPosition;
            _rotationRecord = transform.localRotation.eulerAngles;
            _scaleRecord = transform.localScale;
            _colorRecord = _theGraphic != null ? _theGraphic.color : Color.white;
        }
        private void Start()
        {
            if (_rectTransform == null)
            {
                Log.Error($"{name} 丢失组件rectTransform！不能执行UIAnimation！");
                Destroy(this);
                return;
            }
            if (_theGraphic == null)
            {
                if (TheAnimationType == UIAnimationType.Transparency || TheAnimationType == UIAnimationType.Color)
                {
                    Log.Error($"{name} 丢失组件Graphic！不能执行相应的UIAnimation！");
                    Destroy(this);
                    return;
                }
            }

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
                case UIAnimationType.Move:
                    _theTweener = _rectTransform.DOAnchorPos(TheTargetVector3, TheTime).SetEase(TheEase).SetLoops(TheLoops, TheLoopType);
                    break;
                case UIAnimationType.Rotate:
                    _theTweener = _rectTransform.DOLocalRotate(TheTargetVector3, TheTime, TheRotateMode).SetEase(TheEase).SetLoops(TheLoops, TheLoopType);
                    break;
                case UIAnimationType.Scale:
                    _theTweener = _rectTransform.DOScale(TheTargetVector3, TheTime).SetEase(TheEase).SetLoops(TheLoops, TheLoopType);
                    break;
                case UIAnimationType.Transparency:
                    _theTweener = _theGraphic.DOFade(TheTargetFloat, TheTime).SetEase(TheEase).SetLoops(TheLoops, TheLoopType);
                    break;
                case UIAnimationType.Color:
                    _theTweener = _theGraphic.DOColor(TheTargetColor, TheTime).SetEase(TheEase).SetLoops(TheLoops, TheLoopType);
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
            if (_theGraphic) _theGraphic.color = _colorRecord;

            IsPause = false;
        }
    }

    public enum UIAnimationType
    {
        /// <summary>
        /// 移动动画
        /// </summary>
        Move,
        /// <summary>
        /// 旋转动画
        /// </summary>
        Rotate,
        /// <summary>
        /// 缩放动画
        /// </summary>
        Scale,
        /// <summary>
        /// 透明度改变动画
        /// </summary>
        Transparency,
        /// <summary>
        /// 颜色改变动画
        /// </summary>
        Color
    }
}