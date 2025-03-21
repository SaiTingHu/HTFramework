﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// 反射工具箱
    /// </summary>
    public static class ReflectionToolkit
    {
        /// <summary>
        /// 当前的运行时程序集
        /// </summary>
        private static HashSet<string> RunTimeAssemblies = new HashSet<string>() {
            "Assembly-CSharp", "HTFramework.RunTime", "HTFramework.AI.RunTime", "HTFramework.Deployment.RunTime", "HTFramework.GC.RunTime",
            "UnityEngine", "UnityEngine.CoreModule", "UnityEngine.UI", "UnityEngine.PhysicsModule" };

        /// <summary>
        /// 添加自定义程序集到运行时程序域（建议调用时机：类的【静态构造方法】中，以使其位于框架的所有行为之前）
        /// </summary>
        /// <param name="assembly">运行时程序集</param>
        public static void AddRunTimeAssembly(string assembly)
        {
            if (!string.IsNullOrEmpty(assembly))
            {
                RunTimeAssemblies.Add(assembly);
            }
        }
        /// <summary>
        /// 从当前【程序域】的【运行时程序集】中获取所有类型
        /// </summary>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>所有类型</returns>
        public static List<Type> GetTypesInRunTimeAssemblies(bool isIncludeUnity = true)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                string name = assemblys[i].GetName().Name;
                if (RunTimeAssemblies.Contains(name) && (isIncludeUnity || (!name.StartsWith("UnityEngine") && !name.StartsWith("UnityEditor"))))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前【程序域】的【运行时程序集】中获取所有类型
        /// </summary>
        /// <param name="filter">类型筛选器</param>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>所有类型</returns>
        public static List<Type> GetTypesInRunTimeAssemblies(HTFFunc<Type, bool> filter, bool isIncludeUnity = true)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                string name = assemblys[i].GetName().Name;
                if (RunTimeAssemblies.Contains(name) && (isIncludeUnity || (!name.StartsWith("UnityEngine") && !name.StartsWith("UnityEditor"))))
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
        /// 从当前【程序域】的【运行时程序集】中获取指定类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>类型</returns>
        public static Type GetTypeInRunTimeAssemblies(string typeName, bool isIncludeUnity = true)
        {
            Type type = null;
            foreach (string assembly in RunTimeAssemblies)
            {
                if (isIncludeUnity || (!assembly.StartsWith("UnityEngine") && !assembly.StartsWith("UnityEditor")))
                {
                    type = Type.GetType($"{typeName},{assembly}");
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            Log.Error($"获取类型 {typeName} 失败！当前运行时程序集中不存在此类型！或此类型所在的程序集未使用 ReflectionToolkit.AddRunTimeAssembly(assembly) 添加到程序域！");
            return null;
        }
        /// <summary>
        /// 从当前【程序域】的【所有程序集】中获取所有类型
        /// </summary>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>所有类型</returns>
        public static List<Type> GetTypesInAllAssemblies(bool isIncludeUnity = true)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                string name = assemblys[i].GetName().Name;
                if (isIncludeUnity || (!name.StartsWith("UnityEngine") && !name.StartsWith("UnityEditor")))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前【程序域】的【所有程序集】中获取所有类型
        /// </summary>
        /// <param name="filter">类型筛选器</param>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>所有类型</returns>
        public static List<Type> GetTypesInAllAssemblies(HTFFunc<Type, bool> filter, bool isIncludeUnity = true)
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                string name = assemblys[i].GetName().Name;
                if (isIncludeUnity || (!name.StartsWith("UnityEngine") && !name.StartsWith("UnityEditor")))
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
        /// 从当前【程序域】的【所有程序集】中获取指定类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="isIncludeUnity">是否包含Unity系列的程序集</param>
        /// <returns>类型</returns>
        public static Type GetTypeInAllAssemblies(string typeName, bool isIncludeUnity = true)
        {
            Type type = null;
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                string name = assemblys[i].GetName().Name;
                if (isIncludeUnity || (!name.StartsWith("UnityEngine") && !name.StartsWith("UnityEditor")))
                {
                    type = Type.GetType($"{typeName},{name}");
                    if (type != null)
                    {
                        return type;
                    }
                }
            }
            return type;
        }

        /// <summary>
        /// 从当前类型中获取所有字段
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="filter">字段筛选器</param>
        /// <returns>所有字段</returns>
        public static List<FieldInfo> GetFields(this Type type, HTFFunc<FieldInfo, bool> filter)
        {
            List<FieldInfo> fields = new List<FieldInfo>();
            FieldInfo[] infos = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            for (int i = 0; i < infos.Length; i++)
            {
                if (filter(infos[i]))
                {
                    fields.Add(infos[i]);
                }
            }
            return fields;
        }
        /// <summary>
        /// 从当前类型中获取所有属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="filter">属性筛选器</param>
        /// <returns>所有属性</returns>
        public static List<PropertyInfo> GetProperties(this Type type, HTFFunc<PropertyInfo, bool> filter)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            PropertyInfo[] infos = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            for (int i = 0; i < infos.Length; i++)
            {
                if (filter(infos[i]))
                {
                    properties.Add(infos[i]);
                }
            }
            return properties;
        }
        /// <summary>
        /// 从当前类型中获取所有方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="filter">方法筛选器</param>
        /// <returns>所有方法</returns>
        public static List<MethodInfo> GetMethods(this Type type, HTFFunc<MethodInfo, bool> filter)
        {
            List<MethodInfo> methods = new List<MethodInfo>();
            MethodInfo[] infos = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            for (int i = 0; i < infos.Length; i++)
            {
                if (filter(infos[i]))
                {
                    methods.Add(infos[i]);
                }
            }
            return methods;
        }
    }
}