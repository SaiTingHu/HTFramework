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
        private static Dictionary<Button, TweenerCore<Color, Color, ColorOptions>> Buttons = new Dictionary<Button, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Toggle, TweenerCore<Color, Color, ColorOptions>> Toggles = new Dictionary<Toggle, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Image, TweenerCore<Color, Color, ColorOptions>> Images = new Dictionary<Image, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Text, TweenerCore<Color, Color, ColorOptions>> Texts = new Dictionary<Text, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Dropdown, TweenerCore<Color, Color, ColorOptions>> Dropdowns = new Dictionary<Dropdown, TweenerCore<Color, Color, ColorOptions>>();

        /// <summary>
        /// 开启按钮闪烁（只在Normal状态）
        /// </summary>
        /// <param name="button">按钮</param>
        /// <param name="color">闪烁的目标颜色</param>
        /// <param name="time">闪烁一次的时间</param>
        public static void OpenTwinkle(this Button button, Color color, float time = 0.5f)
        {
            if (!Buttons.ContainsKey(button))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return button.colors.normalColor;
                },
                (c) =>
                {
                    ColorBlock block = button.colors;
                    block.normalColor = c;
                    button.colors = block;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = button.colors.normalColor;
                Buttons.Add(button, tweener);
            }
        }
        /// <summary>
        /// 关闭按钮闪烁
        /// </summary>
        /// <param name="button">按钮</param>
        public static void CloseTwinkle(this Button button)
        {
            if (Buttons.ContainsKey(button))
            {
                ColorBlock block = button.colors;
                block.normalColor = Buttons[button].startValue;

                Buttons[button].Kill();
                Buttons.Remove(button);

                button.colors = block;
            }
        }
        /// <summary>
        /// 开启开关闪烁（只在Normal状态）
        /// </summary>
        /// <param name="toggle">开关</param>
        /// <param name="color">闪烁的目标颜色</param>
        /// <param name="time">闪烁一次的时间</param>
        public static void OpenTwinkle(this Toggle toggle, Color color, float time = 0.5f)
        {
            if (!Toggles.ContainsKey(toggle))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return toggle.colors.normalColor;
                },
                (c) =>
                {
                    ColorBlock block = toggle.colors;
                    block.normalColor = c;
                    toggle.colors = block;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = toggle.colors.normalColor;
                Toggles.Add(toggle, tweener);
            }
        }
        /// <summary>
        /// 关闭开关闪烁
        /// </summary>
        /// <param name="toggle">开关</param>
        public static void CloseTwinkle(this Toggle toggle)
        {
            if (Toggles.ContainsKey(toggle))
            {
                ColorBlock block = toggle.colors;
                block.normalColor = Toggles[toggle].startValue;

                Toggles[toggle].Kill();
                Toggles.Remove(toggle);

                toggle.colors = block;
            }
        }
        /// <summary>
        /// 开启图片闪烁
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="color">闪烁的目标颜色</param>
        /// <param name="time">闪烁一次的时间</param>
        public static void OpenTwinkle(this Image image, Color color, float time = 0.5f)
        {
            if (!Images.ContainsKey(image))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return image.color;
                },
                (c) =>
                {
                    image.color = c;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = image.color;
                Images.Add(image, tweener);
            }
        }
        /// <summary>
        /// 关闭图片闪烁
        /// </summary>
        /// <param name="image">图片</param>
        public static void CloseTwinkle(this Image image)
        {
            if (Images.ContainsKey(image))
            {
                Color normalColor = Images[image].startValue;

                Images[image].Kill();
                Images.Remove(image);

                image.color = normalColor;
            }
        }
        /// <summary>
        /// 开启文本框闪烁
        /// </summary>
        /// <param name="text">文本框</param>
        /// <param name="color">闪烁的目标颜色</param>
        /// <param name="time">闪烁一次的时间</param>
        public static void OpenTwinkle(this Text text, Color color, float time = 0.5f)
        {
            if (!Texts.ContainsKey(text))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return text.color;
                },
                (c) =>
                {
                    text.color = c;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = text.color;
                Texts.Add(text, tweener);
            }
        }
        /// <summary>
        /// 关闭文本框闪烁
        /// </summary>
        /// <param name="text">文本框</param>
        public static void CloseTwinkle(this Text text)
        {
            if (Texts.ContainsKey(text))
            {
                Color normalColor = Texts[text].startValue;

                Texts[text].Kill();
                Texts.Remove(text);

                text.color = normalColor;
            }
        }
        /// <summary>
        /// 开启下拉框闪烁（只在Normal状态）
        /// </summary>
        /// <param name="dropdown">下拉框</param>
        /// <param name="color">闪烁的目标颜色</param>
        /// <param name="time">闪烁一次的时间</param>
        public static void OpenTwinkle(this Dropdown dropdown, Color color, float time = 0.5f)
        {
            if (!Dropdowns.ContainsKey(dropdown))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return dropdown.colors.normalColor;
                },
                (c) =>
                {
                    ColorBlock block = dropdown.colors;
                    block.normalColor = c;
                    dropdown.colors = block;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = dropdown.colors.normalColor;
                Dropdowns.Add(dropdown, tweener);
            }
        }
        /// <summary>
        /// 关闭下拉框闪烁
        /// </summary>
        /// <param name="dropdown">下拉框</param>
        public static void CloseTwinkle(this Dropdown dropdown)
        {
            if (Dropdowns.ContainsKey(dropdown))
            {
                ColorBlock block = dropdown.colors;
                block.normalColor = Dropdowns[dropdown].startValue;

                Dropdowns[dropdown].Kill();
                Dropdowns.Remove(dropdown);

                dropdown.colors = block;
            }
        }
        /// <summary>
        /// 关闭所有控件的闪烁
        /// </summary>
        public static void CloseAllTwinkle()
        {
            foreach (var button in Buttons)
            {
                if (button.Key)
                {
                    ColorBlock block = button.Key.colors;
                    block.normalColor = button.Value.startValue;
                    button.Value.Kill();
                    button.Key.colors = block;
                }
            }
            foreach (var toggle in Toggles)
            {
                if (toggle.Key)
                {
                    ColorBlock block = toggle.Key.colors;
                    block.normalColor = toggle.Value.startValue;
                    toggle.Value.Kill();
                    toggle.Key.colors = block;
                }
            }
            foreach (var image in Images)
            {
                if (image.Key)
                {
                    Color normalColor = image.Value.startValue;
                    image.Value.Kill();
                    image.Key.color = normalColor;
                }
            }
            foreach (var text in Texts)
            {
                if (text.Key)
                {
                    Color normalColor = text.Value.startValue;
                    text.Value.Kill();
                    text.Key.color = normalColor;
                }
            }
            foreach (var dropdown in Dropdowns)
            {
                if (dropdown.Key)
                {
                    ColorBlock block = dropdown.Key.colors;
                    block.normalColor = dropdown.Value.startValue;
                    dropdown.Value.Kill();
                    dropdown.Key.colors = block;
                }
            }
            Buttons.Clear();
            Toggles.Clear();
            Images.Clear();
            Texts.Clear();
            Dropdowns.Clear();
        }
    }
}