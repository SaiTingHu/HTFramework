using DG.Tweening;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 游戏物体动画
    /// </summary>
    [AddComponentMenu("HTFramework/GameObject/GameObject Animation")]
    public sealed class GameObjectAnimation : MonoBehaviour
    {
        public GameObjectAnimationType TheAnimationType = GameObjectAnimationType.Move;
        public Ease TheEase = Ease.Linear;
        public LoopType TheLoopType = LoopType.Yoyo;
        public RotateMode TheRotateMode = RotateMode.LocalAxisAdd;
        public Vector3 TheTargetVector3 = Vector3.zero;
        public float TheTime = 1;
        public int TheLoops = -1;
        public bool PlayOnStart = true;

        private Tweener _theTweener;

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

    public enum GameObjectAnimationType
    {
        Move,
        Rotate,
        Scale
    }
}
