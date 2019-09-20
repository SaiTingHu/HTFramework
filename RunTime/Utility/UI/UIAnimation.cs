using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UI控件动画
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIAnimation")]
    public sealed class UIAnimation : MonoBehaviour
    {
        public UIAnimationType TheAnimationType = UIAnimationType.Move;
        public Ease TheEase = Ease.Linear;
        public LoopType TheLoopType = LoopType.Yoyo;
        public RotateMode TheRotateMode = RotateMode.LocalAxisAdd;
        public Vector3 TheTargetVector3 = Vector3.zero;
        public float TheTargetFloat = 0;
        public Color TheTargetColor = Color.white;
        public float TheTime = 1;
        public int Loops = -1;
        public bool PlayOnStart = true;

        private Graphic _theGraphic;
        private Tweener _theTweener;

        private void Start()
        {
            if (!transform.rectTransform())
            {
                GlobalTools.LogError(name + " 丢失组件rectTransform！不能执行UIAnimation！");
                Destroy(this);
                return;
            }
            _theGraphic = GetComponent<Graphic>();
            if (!_theGraphic)
            {
                if (TheAnimationType == UIAnimationType.Transparency || TheAnimationType == UIAnimationType.Color)
                {
                    GlobalTools.LogError(name + " 丢失组件Graphic！不能执行UIAnimation！");
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
                    _theTweener = transform.rectTransform().DOAnchorPos(TheTargetVector3, TheTime).SetEase(TheEase).SetLoops(Loops, TheLoopType);
                    break;
                case UIAnimationType.Rotate:
                    _theTweener = transform.rectTransform().DOLocalRotate(TheTargetVector3, TheTime, TheRotateMode).SetEase(TheEase).SetLoops(Loops, TheLoopType);
                    break;
                case UIAnimationType.Scale:
                    _theTweener = transform.rectTransform().DOScale(TheTargetVector3, TheTime).SetEase(TheEase).SetLoops(Loops, TheLoopType);
                    break;
                case UIAnimationType.Transparency:
                    _theTweener = _theGraphic.DOFade(TheTargetFloat, TheTime).SetEase(TheEase).SetLoops(Loops, TheLoopType);
                    break;
                case UIAnimationType.Color:
                    _theTweener = _theGraphic.DOColor(TheTargetColor, TheTime).SetEase(TheEase).SetLoops(Loops, TheLoopType);
                    break;
                default:
                    break;
            }
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
