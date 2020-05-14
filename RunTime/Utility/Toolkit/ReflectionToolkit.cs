using System;
using System.Collections.Generic;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// 反射工具箱
    /// </summary>
    public static class ReflectionToolkit
    {
        static ReflectionToolkit()
        {
            List<Type> types = GetTypesInAllAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                FieldInfo[] fields = types[i].GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < fields.Length; j++)
                {
                    if (fields[j].FieldType == typeof(string) && fields[j].IsDefined(typeof(RunTimeAssemblyAttribute), false))
                    {
                        string value = fields[j].GetValue(null) as string;
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (!RunTimeAssemblies.Contains(value)) RunTimeAssemblies.Add(value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当前的运行时程序集
        /// </summary>
        private static HashSet<string> RunTimeAssemblies = new HashSet<string>() {
            "Assembly-CSharp", "HTFramework.RunTime", "HTFramework.AI.RunTime", "HTFramework.ILHotfix.RunTime", "HTFramework.GC.RunTime",
            "UnityEngine", "UnityEngine.CoreModule", "UnityEngine.UI", "UnityEngine.PhysicsModule" };
        /// <summary>
        /// 从当前程序域的运行时程序集中获取所有类型
        /// </summary>
        /// <returns>所有类型集合</returns>
        public static List<Type> GetTypesInRunTimeAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if (RunTimeAssemblies.Contains(assemblys[i].GetName().Name))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前程序域的运行时程序集中获取指定类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns>类型</returns>
        public static Type GetTypeInRunTimeAssemblies(string typeName)
        {
            Type type = null;
            foreach (string assembly in RunTimeAssemblies)
            {
                type = Type.GetType(typeName + "," + assembly);
                if (type != null)
                {
                    return type;
                }
            }
            GlobalTools.LogError("获取类型 " + typeName + " 失败！当前运行时程序集中不存在此类型！");
            return null;
        }
        /// <summary>
        /// 从当前程序域的所有程序集中获取所有类型
        /// </summary>
        /// <returns>所有类型集合</returns>
        public static List<Type> GetTypesInAllAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                types.AddRange(assemblys[i].GetTypes());
            }
            return types;
        }
    }
}