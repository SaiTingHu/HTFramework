using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 网格高亮工具箱
    /// </summary>
    public static class HighlightableToolkit
    {
        private static List<HighlightableObject> HOCache = new List<HighlightableObject>();
        private static HashSet<HighlightableObject> HOs = new HashSet<HighlightableObject>();
        private static HashSet<HighlightableObject> FlashHOs = new HashSet<HighlightableObject>();
        private static HashSet<HighlightableObject> OccluderHOs = new HashSet<HighlightableObject>();

        /// <summary>
        /// 开启高亮一次，使用默认发光颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void OpenOnceHighLight(this GameObject target)
        {
            OpenOnceHighLight(target, Color.cyan);
        }
        /// <summary>
        /// 开启高亮一次，使用指定发光颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color">发光颜色</param>
        public static void OpenOnceHighLight(this GameObject target, Color color)
        {
            if (target == null)
                return;

            if (!Main.m_Controller.EnableHighlightingEffect)
                return;

            target.ClearHighLightInChildren();
            target.ClearHighLightInParent();

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            ho.OpenOnce(color);
        }

        /// <summary>
        /// 开启持续高光，使用默认发光颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void OpenHighLight(this GameObject target)
        {
            OpenHighLight(target, Color.cyan);
        }
        /// <summary>
        /// 开启持续高光，使用指定发光颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color">发光颜色</param>
        /// <param name="isImmediate">是否立即模式</param>
        public static void OpenHighLight(this GameObject target, Color color, bool isImmediate = true)
        {
            if (target == null)
                return;

            if (!Main.m_Controller.EnableHighlightingEffect)
                return;

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            if (HOs.Contains(ho))
                return;

            HOs.Add(ho);

            target.ClearHighLightInChildren();
            target.ClearHighLightInParent();
            
            if (isImmediate) ho.OpenConstantImmediate(color);
            else ho.OpenConstant(color);
        }
        /// <summary>
        /// 关闭持续高光
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseHighLight(this GameObject target, bool die = false)
        {
            if (target == null)
                return;

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) return;

            HOs.Remove(ho);

            ho.CloseConstant();
            if (die) ho.Die();
        }
        /// <summary>
        /// 关闭所有的持续高光
        /// </summary>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseAllHighLight(bool die = false)
        {
            foreach (var ho in HOs)
            {
                if (ho)
                {
                    ho.CloseConstant();
                    if (die) ho.Die();
                }
            }
            HOs.Clear();
        }

        /// <summary>
        /// 开启闪光，使用默认颜色和频率
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void OpenFlashHighLight(this GameObject target)
        {
            OpenFlashHighLight(target, Color.red, Color.white, 2);
        }
        /// <summary>
        /// 开启闪光，使用默认频率
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color1">颜色1</param>
        /// <param name="color2">颜色2</param>
        public static void OpenFlashHighLight(this GameObject target, Color color1, Color color2)
        {
            OpenFlashHighLight(target, color1, color2, 2);
        }
        /// <summary>
        /// 开启闪光，使用指定频率
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color1">颜色1</param>
        /// <param name="color2">颜色2</param>
        /// <param name="freq">频率</param>
        public static void OpenFlashHighLight(this GameObject target, Color color1, Color color2, float freq)
        {
            if (target == null)
                return;

            if (!Main.m_Controller.EnableHighlightingEffect)
                return;

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            if (FlashHOs.Contains(ho))
                return;

            FlashHOs.Add(ho);

            target.ClearHighLightInChildren();
            target.ClearHighLightInParent();

            ho.OpenFlashing(color1, color2, freq);
        }
        /// <summary>
        /// 关闭闪光
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseFlashHighLight(this GameObject target, bool die = false)
        {
            if (target == null)
                return;

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) return;

            FlashHOs.Remove(ho);

            ho.CloseFlashing();
            if (die) ho.Die();
        }
        /// <summary>
        /// 关闭所有的闪光
        /// </summary>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseAllFlashHighLight(bool die = false)
        {
            foreach (var ho in FlashHOs)
            {
                if (ho)
                {
                    ho.CloseFlashing();
                    if (die) ho.Die();
                }
            }
            FlashHOs.Clear();
        }

        /// <summary>
        /// 开启遮光板
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void OpenOccluder(this GameObject target)
        {
            if (target == null)
                return;

            if (!Main.m_Controller.EnableHighlightingEffect)
                return;

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            if (OccluderHOs.Contains(ho))
                return;

            OccluderHOs.Add(ho);

            target.ClearHighLightInChildren();
            target.ClearHighLightInParent();

            ho.OpenOccluder();
        }
        /// <summary>
        /// 关闭遮光板
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseOccluder(this GameObject target, bool die = false)
        {
            if (target == null)
                return;

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) return;

            OccluderHOs.Remove(ho);

            ho.CloseOccluder();
            if (die) ho.Die();
        }
        /// <summary>
        /// 关闭所有的遮光板
        /// </summary>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseAllOccluder(bool die = false)
        {
            foreach (var ho in OccluderHOs)
            {
                if (ho)
                {
                    ho.CloseOccluder();
                    if (die) ho.Die();
                }
            }
            OccluderHOs.Clear();
        }

        /// <summary>
        /// 清空所有子物体上的高光效果，不包括自身
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void ClearHighLightInChildren(this GameObject target, bool die = false)
        {
            if (target == null)
                return;

            HOCache.Clear();
            target.transform.GetComponentsInChildren(true, HOCache);
            for (int i = 0; i < HOCache.Count; i++)
            {
                if (HOCache[i].gameObject != target)
                {
                    HOCache[i].CloseAll();
                    if (die) HOCache[i].Die();
                }
            }
        }
        /// <summary>
        /// 清空所有父物体上的高光效果，不包括自身
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void ClearHighLightInParent(this GameObject target, bool die = false)
        {
            if (target == null)
                return;

            HOCache.Clear();
            target.transform.GetComponentsInParent(true, HOCache);
            for (int i = 0; i < HOCache.Count; i++)
            {
                if (HOCache[i].gameObject != target)
                {
                    HOCache[i].CloseAll();
                    if (die) HOCache[i].Die();
                }
            }
        }
    }
}