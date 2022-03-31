using System.Collections.Generic;
using UnityEditor;

namespace HT.Framework
{
    /// <summary>
    /// MonoScript工具箱
    /// </summary>
    public static class MonoScriptToolkit
    {
        private static Dictionary<string, string> MonoScripts = new Dictionary<string, string>();
        
        static MonoScriptToolkit()
        {
            MonoScripts.Clear();
            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].EndsWith(".cs"))
                {
                    string className = paths[i].Substring(paths[i].LastIndexOf("/") + 1).Replace(".cs", "");
                    if (!MonoScripts.ContainsKey(className))
                    {
                        MonoScripts.Add(className, paths[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 是否已存在指定名称的MonoScript脚本对象
        /// </summary>
        /// <param name="name">MonoScript脚本对象名称</param>
        /// <returns>是否存在</returns>
        public static bool IsExistMonoScriptName(string name)
        {
            return MonoScripts.ContainsKey(name);
        }
        /// <summary>
        /// 打开并编辑MonoScript脚本对象
        /// </summary>
        /// <param name="scriptFileName">脚本文件名称</param>
        public static void OpenMonoScript(string scriptFileName)
        {
            string name = scriptFileName;
            if (!MonoScripts.ContainsKey(name))
            {
                string[] names = name.Split('.');
                name = names[names.Length - 1];
            }

            if (MonoScripts.ContainsKey(name))
            {
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(MonoScripts[name]);
                if (monoScript)
                {
                    AssetDatabase.OpenAsset(monoScript);
                }
                else
                {
                    Log.Error("没有找到 " + scriptFileName + " 脚本文件！");
                }
            }
            else
            {
                Log.Error("没有找到 " + scriptFileName + " 脚本文件！");
            }
        }
    }
}