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
        private static HashSet<string> HotfixAssemblies = new HashSet<string>() { "Hotfix", "ILHotfix" };
        /// <summary>
        /// 当前的编辑器程序集
        /// </summary>
        private static HashSet<string> EditorAssemblies = new HashSet<string>() { "Assembly-CSharp-Editor", "UnityEditor" };
        /// <summary>
        /// 从当前程序域的热更新程序集中获取所有类型
        /// </summary>
        /// <returns>所有类型集合</returns>
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
        /// 从当前程序域的热更新程序集中获取所有类型
        /// </summary>
        /// <param name="filter">类型筛选器</param>
        /// <returns>所有类型集合</returns>
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
        /// 从当前程序域的热更新程序集中获取指定类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>类型</returns>
        public static Type GetTypeInHotfixAssemblies(string typeName)
        {
            Type type = null;
            foreach (string assembly in HotfixAssemblies)
            {
                type = Type.GetType(typeName + "," + assembly);
                if (type != null)
                {
                    return type;
                }
            }
            GlobalTools.LogError("获取类型 " + typeName + " 失败！当前热更新程序集中不存在此类型！");
            return null;
        }
        /// <summary>
        /// 从当前程序域的编辑器程序集中获取所有类型
        /// </summary>
        /// <returns>所有类型集合</returns>
        public static List<Type> GetTypesInEditorAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if (EditorAssemblies.Contains(assemblys[i].GetName().Name))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前程序域的编辑器程序集中获取所有类型
        /// </summary>
        /// <param name="filter">类型筛选器</param>
        /// <returns>所有类型集合</returns>
        public static List<Type> GetTypesInEditorAssemblies(HTFFunc<Type, bool> filter)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if (EditorAssemblies.Contains(assemblys[i].GetName().Name))
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
        /// 从当前程序域的编辑器程序集中获取指定类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>类型</returns>
        public static Type GetTypeInEditorAssemblies(string typeName)
        {
            Type type = null;
            foreach (string assembly in EditorAssemblies)
            {
                type = Type.GetType(typeName + "," + assembly);
                if (type != null)
                {
                    return type;
                }
            }
            GlobalTools.LogError("获取类型 " + typeName + " 失败！当前编辑器程序集中不存在此类型！");
            return null;
        }
    }
}