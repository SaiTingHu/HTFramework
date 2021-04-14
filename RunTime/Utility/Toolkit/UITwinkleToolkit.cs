using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UI闪烁工具箱
    /// </summary>
    public static class UITwinkleToolkit
    {
        private static Dictionary<Graphic, TweenerCore<Color, Color, ColorOptions>> Graphics = new Dictionary<Graphic, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Selectable, TweenerCore<Color, Color, ColorOptions>> Selectables = new Dictionary<Selectable, TweenerCore<Color, Color, ColorOptions>>();
        
        /// <summary>
        /// 开启图像控件的闪烁
        /// </summary>
        /// <param name="graphic">图像控件</param>
        /// <param name="color">闪烁的目标颜色</param>
        /// <param name="time">闪烁一次的时间</param>
        public static void OpenTwinkle(this Graphic graphic, Color color, float time = 0.5f)
        {
            if (graphic == null)
                return;

            if (!Graphics.ContainsKey(graphic))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return graphic.color;
                },
                (c) =>
                {
                    graphic.color = c;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = graphic.color;
                Graphics.Add(graphic, tweener);
            }
        }
        /// <summary>
        /// 关闭图像控件的闪烁
        /// </summary>
        /// <param name="graphic">图像控件</param>
        public static void CloseTwinkle(this Graphic graphic)
        {
            if (graphic == null)
                return;

            if (Graphics.ContainsKey(graphic))
            {
                Color normalColor = Graphics[graphic].startValue;

                Graphics[graphic].Kill();
                Graphics.Remove(graphic);

                graphic.color = normalColor;
            }
        }
        /// <summary>
        /// 开启可选控件的闪烁（只在Normal状态）
        /// </summary>
        /// <param name="selectable">可选控件</param>
        /// <param name="color">闪烁的目标颜色</param>
        /// <param name="time">闪烁一次的时间</param>
        public static void OpenTwinkle(this Selectable selectable, Color color, float time = 0.5f)
        {
            if (selectable == null)
                return;

            if (!Selectables.ContainsKey(selectable) && selectable.targetGraphic != null)
            {
                if (selectable.transition == Selectable.Transition.ColorTint)
                {
                    TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                    () =>
                    {
                        return selectable.colors.normalColor;
                    },
                    (c) =>
                    {
                        ColorBlock block = selectable.colors;
                        block.normalColor = c;
                        selectable.colors = block;
                    }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                    tweener.startValue = selectable.colors.normalColor;
                    Selectables.Add(selectable, tweener);
                }
                else
                {
                    TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                    () =>
                    {
                        return selectable.targetGraphic.color;
                    },
                    (c) =>
                    {
                        selectable.targetGraphic.color = c;
                    }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                    tweener.startValue = selectable.targetGraphic.color;
                    Selectables.Add(selectable, tweener);
                }
            }
        }
        /// <summary>
        /// 关闭可选控件的闪烁
        /// </summary>
        /// <param name="selectable">可选控件</param>
        public static void CloseTwinkle(this Selectable selectable)
        {
            if (selectable == null)
                return;

            if (Selectables.ContainsKey(selectable))
            {
                if (selectable.transition == Selectable.Transition.ColorTint)
                {
                    ColorBlock block = selectable.colors;
                    block.normalColor = Selectables[selectable].startValue;

                    Selectables[selectable].Kill();
                    Selectables.Remove(selectable);

                    selectable.colors = block;
                }
                else
                {
                    Color normalColor = Selectables[selectable].startValue;

                    Selectables[selectable].Kill();
                    Selectables.Remove(selectable);

                    if (selectable.targetGraphic != null)
                        selectable.targetGraphic.color = normalColor;
                }
            }
        }
        /// <summary>
        /// 关闭所有控件的闪烁
        /// </summary>
        public static void CloseAllTwinkle()
        {
            foreach (var graphic in Graphics)
            {
                if (graphic.Key)
                {
                    Color normalColor = graphic.Value.startValue;
                    graphic.Value.Kill();
                    graphic.Key.color = normalColor;
                }
            }
            foreach (var selectable in Selectables)
            {
                if (selectable.Key)
                {
                    if (selectable.Key.transition == Selectable.Transition.ColorTint)
                    {
                        ColorBlock block = selectable.Key.colors;
                        block.normalColor = selectable.Value.startValue;
                        selectable.Value.Kill();
                        selectable.Key.colors = block;
                    }
                    else
                    {
                        Color normalColor = selectable.Value.startValue;
                        selectable.Value.Kill();
                        if (selectable.Key.targetGraphic != null)
                            selectable.Key.targetGraphic.color = normalColor;
                    }
                }
            }
            
            Graphics.Clear();
            Selectables.Clear();
        }
    }
}