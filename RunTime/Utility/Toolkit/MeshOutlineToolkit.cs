using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 网格轮廓高亮工具箱
    /// </summary>
    public static class MeshOutlineToolkit
    {
        private static List<MeshOutlineObject> MOCache = new List<MeshOutlineObject>();
        private static HashSet<MeshOutlineObject> MOs = new HashSet<MeshOutlineObject>();

        /// <summary>
        /// 开启网格轮廓高亮，使用默认颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="intensity">强度</param>
        public static void OpenMeshOutline(this GameObject target, float intensity = 1)
        {
            OpenMeshOutline(target, Color.yellow, intensity, false, 1);
        }
        /// <summary>
        /// 开启网格轮廓高亮，使用指定颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color">高亮颜色</param>
        /// <param name="intensity">强度</param>
        public static void OpenMeshOutline(this GameObject target, Color color, float intensity = 1)
        {
            OpenMeshOutline(target, color, intensity, false, 1);
        }
        /// <summary>
        /// 开启网格轮廓高亮，使用闪烁效果
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color">高亮颜色</param>
        /// <param name="intensity">强度</param>
        /// <param name="freq">闪烁频率</param>
        public static void OpenMeshOutline(this GameObject target, Color color, float intensity, float freq)
        {
            OpenMeshOutline(target, color, intensity, true, freq);
        }
        /// <summary>
        /// 开启网格轮廓高亮
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color">高亮颜色</param>
        /// <param name="intensity">强度</param>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="freq">闪烁频率</param>
        public static void OpenMeshOutline(this GameObject target, Color color, float intensity, bool isFlash, float freq)
        {
            if (target == null)
                return;

            if (!Main.m_Controller.EnableHighlightingEffect)
                return;

            MeshOutlineObject mo = target.GetComponent<MeshOutlineObject>();
            if (mo == null) mo = target.AddComponent<MeshOutlineObject>();

            if (MOs.Contains(mo))
                return;

            MOs.Add(mo);

            target.ClearMeshOutlineInChildren();
            target.ClearMeshOutlineInParent();

            mo.Open(color, intensity, isFlash, freq);
        }
        /// <summary>
        /// 开启网格轮廓高亮
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="defaultColor">默认颜色</param>
        /// <param name="color">高亮颜色</param>
        /// <param name="intensity">强度</param>
        /// <param name="isFlash">是否闪烁</param>
        /// <param name="freq">闪烁频率</param>
        public static void OpenMeshOutline(this GameObject target, Color defaultColor, Color color, float intensity, bool isFlash, float freq)
        {
            if (target == null)
                return;

            if (!Main.m_Controller.EnableHighlightingEffect)
                return;

            MeshOutlineObject mo = target.GetComponent<MeshOutlineObject>();
            if (mo == null) mo = target.AddComponent<MeshOutlineObject>();

            if (MOs.Contains(mo))
                return;

            MOs.Add(mo);

            target.ClearMeshOutlineInChildren();
            target.ClearMeshOutlineInParent();

            mo.Open(defaultColor, color, intensity, isFlash, freq);
        }
        /// <summary>
        /// 重置高亮网格轮廓
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void ResetOutline(this GameObject target)
        {
            if (target == null)
                return;

            MeshOutlineObject mo = target.GetComponent<MeshOutlineObject>();
            if (mo != null)
            {
                mo.ResetOutline();
            }
        }
        /// <summary>
        /// 关闭网格轮廓发光
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁实例</param>
        public static void CloseMeshOutline(this GameObject target, bool die = false)
        {
            if (target == null)
                return;

            MeshOutlineObject mo = target.GetComponent<MeshOutlineObject>();
            if (mo == null) return;

            MOs.Remove(mo);

            mo.Close();
            if (die) mo.Die();
        }
        /// <summary>
        /// 关闭所有的网格轮廓发光
        /// </summary>
        /// <param name="die">是否销毁实例</param>
        public static void CloseAllMeshOutline(bool die = false)
        {
            foreach (var mo in MOs)
            {
                if (mo)
                {
                    mo.Close();
                    if (die) mo.Die();
                }
            }
            MOs.Clear();
        }

        /// <summary>
        /// 清空所有子物体上的轮廓高光效果，不包括自身
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void ClearMeshOutlineInChildren(this GameObject target, bool die = false)
        {
            if (target == null)
                return;

            MOCache.Clear();
            target.transform.GetComponentsInChildren(true, MOCache);
            for (int i = 0; i < MOCache.Count; i++)
            {
                if (MOCache[i].gameObject != target)
                {
                    MOCache[i].Close();
                    if (die) MOCache[i].Die();
                }
            }
        }
        /// <summary>
        /// 清空所有父物体上的轮廓高光效果，不包括自身
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void ClearMeshOutlineInParent(this GameObject target, bool die = false)
        {
            if (target == null)
                return;

            MOCache.Clear();
            target.transform.GetComponentsInParent(true, MOCache);
            for (int i = 0; i < MOCache.Count; i++)
            {
                if (MOCache[i].gameObject != target)
                {
                    MOCache[i].Close();
                    if (die) MOCache[i].Die();
                }
            }
        }
    }
}