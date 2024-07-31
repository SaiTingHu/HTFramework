using System;
using System.Collections.Generic;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// 编辑器反射工具箱
    /// </summary>
    public static class EditorReflectionToolkit
    {
        /// <summary>
        /// 当前的热更新程序集
        /// </summary>
        private static HashSet<string> HotfixAssemblies = new HashSet<string>() { "Hotfix" };
        /// <summary>
        /// 当前的编辑器程序集
        /// </summary>
        private static HashSet<string> EditorAssemblies = new HashSet<string>() {
            "Assembly-CSharp-Editor", "HTFramework.Editor", "HTFramework.AI.Editor", "HTFramework.Deployment.Editor", "HTFramework.GC.Editor",
            "UnityEditor", "UnityEditorInternal" };

        /// <summary>
        /// 从当前【程序域】的【热更新程序集】中获取所有类型
        /// </summary>
        /// <returns>所有类型</returns>
        public static List<Type> GetTypesInHotfixAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if (HotfixAssemblies.Contains(assemblys[i].GetName().Name))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前【程序域】的【热更新程序集】中获取所有类型
        /// </summary>
        /// <param name="filter">类型筛选器</param>
        /// <returns>所有类型</returns>
        public static List<Type> GetTypesInHotfixAssemblies(HTFFunc<Type, bool> filter)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if (HotfixAssemblies.Contains(assemblys[i].GetName().Name))
                {
                    Type[] ts = assemblys[i].GetTypes();
                    foreach (var t in ts)
                    {
                        if (filter(t))
                        {
                            types.Add(t);
                        }
                    }
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前【程序域】的【热更新程序集】中获取指定类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>类型</returns>
        public static Type GetTypeInHotfixAssemblies(string typeName)
        {
            Type type = null;
            foreach (string assembly in HotfixAssemblies)
            {
                type = Type.GetType($"{typeName},{assembly}");
                if (type != null)
                {
                    return type;
                }
            }
            Log.Error($"获取类型 {typeName} 失败！当前热更新程序集中不存在此类型！");
            return null;
        }
        /// <summary>
        /// 从当前【程序域】的【编辑器程序集】中获取所有类型
        /// </summary>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>所有类型</returns>
        public static List<Type> GetTypesInEditorAssemblies(bool isIncludeUnity = true)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                string name = assemblys[i].GetName().Name;
                if (EditorAssemblies.Contains(name) && (isIncludeUnity || !name.StartsWith("UnityEditor")))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前【程序域】的【编辑器程序集】中获取所有类型
        /// </summary>
        /// <param name="filter">类型筛选器</param>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>所有类型</returns>
        public static List<Type> GetTypesInEditorAssemblies(HTFFunc<Type, bool> filter, bool isIncludeUnity = true)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                string name = assemblys[i].GetName().Name;
                if (EditorAssemblies.Contains(name) && (isIncludeUnity || !name.StartsWith("UnityEditor")))
                {
                    Type[] ts = assemblys[i].GetTypes();
                    foreach (var t in ts)
                    {
                        if (filter(t))
                        {
                            types.Add(t);
                        }
                    }
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前【程序域】的【编辑器程序集】中获取指定类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>类型</returns>
        public static Type GetTypeInEditorAssemblies(string typeName, bool isIncludeUnity = true)
        {
            Type type = null;
            foreach (string assembly in EditorAssemblies)
            {
                if (isIncludeUnity || !assembly.StartsWith("UnityEditor"))
                {
                    type = Type.GetType($"{typeName},{assembly}");
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            Log.Error($"获取类型 {typeName} 失败！当前编辑器程序集中不存在此类型！");
            return null;
        }
    }
}