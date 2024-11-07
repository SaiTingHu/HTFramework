using System;
using System.Collections.Generic;
using UnityEditor;

namespace HT.Framework
{
    /// <summary>
    /// C#脚本工具箱
    /// </summary>
    public static class CSharpScriptToolkit
    {
        private const string ScriptExtension = ".cs";
        private static Dictionary<string, string> Scripts = new Dictionary<string, string>();
        
        static CSharpScriptToolkit()
        {
            Scripts.Clear();
            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].EndsWith(ScriptExtension))
                {
                    string fileName = paths[i].Substring(paths[i].LastIndexOf("/") + 1).Replace(ScriptExtension, "");
                    if (!Scripts.ContainsKey(fileName))
                    {
                        Scripts.Add(fileName, paths[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 是否存在指定名称的C#脚本文件
        /// </summary>
        /// <param name="name">C#脚本文件名称（文件名称必须与类名一致）</param>
        /// <returns>是否存在</returns>
        public static bool IsExistScript(string name)
        {
            if (!Scripts.ContainsKey(name))
            {
                string[] values = name.Split('.');
                name = values[values.Length - 1];
            }

            return Scripts.ContainsKey(name);
        }
        /// <summary>
        /// 打开并编辑C#脚本文件
        /// </summary>
        /// <param name="name">C#脚本文件名称（文件名称必须与类名一致）</param>
        public static void OpenScript(string name)
        {
            if (!Scripts.ContainsKey(name))
            {
                string[] values = name.Split('.');
                name = values[values.Length - 1];
            }

            if (Scripts.ContainsKey(name))
            {
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(Scripts[name]);
                if (monoScript)
                {
                    AssetDatabase.OpenAsset(monoScript);
                }
                else
                {
                    Log.Error($"打开脚本文件失败：没有找到 {name} 脚本文件！");
                }
            }
            else
            {
                Log.Error($"打开脚本文件失败：没有找到 {name} 脚本文件！");
            }
        }
        /// <summary>
        /// 打开并编辑C#脚本文件
        /// </summary>
        /// <param name="name">C#脚本文件名称（文件名称必须与类名一致）</param>
        /// <param name="lineNumber">行号</param>
        /// <param name="columnNumber">列号</param>
        public static void OpenScript(string name, int lineNumber, int columnNumber)
        {
            if (!Scripts.ContainsKey(name))
            {
                string[] values = name.Split('.');
                name = values[values.Length - 1];
            }

            if (Scripts.ContainsKey(name))
            {
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(Scripts[name]);
                if (monoScript)
                {
                    AssetDatabase.OpenAsset(monoScript, lineNumber, columnNumber);
                }
                else
                {
                    Log.Error($"打开脚本文件失败：没有找到 {name} 脚本文件！");
                }
            }
            else
            {
                Log.Error($"打开脚本文件失败：没有找到 {name} 脚本文件！");
            }
        }
        /// <summary>
        /// 获取C#脚本的类名全称（包含命名空间）
        /// </summary>
        /// <param name="name">C#脚本文件名称（文件名称必须与类名一致）</param>
        /// <returns>C#类名全称（包含命名空间）</returns>
        public static string GetScriptFullName(string name)
        {
            if (!Scripts.ContainsKey(name))
            {
                string[] values = name.Split('.');
                name = values[values.Length - 1];
            }

            if (Scripts.ContainsKey(name))
            {
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(Scripts[name]);
                if (monoScript)
                {
                    Type type = monoScript.GetClass();
                    if (type != null)
                    {
                        return type.FullName;
                    }
                }
            }

            Log.Error($"获取脚本的类名全称失败：没有找到 {name} 脚本文件！");
            return name;
        }
    }
}