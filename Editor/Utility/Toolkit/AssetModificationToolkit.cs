using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 项目资源文件修改监控工具箱
    /// </summary>
    public sealed class AssetModificationToolkit : AssetModificationProcessor
    {
        /// <summary>
        /// 新建脚本时默认引用的命名空间
        /// </summary>
        private static List<string> UsingNamespaces = new List<string>() { "HT.Framework", "DG.Tweening" };
        /// <summary>
        /// 新建脚本时默认附加的命名空间
        /// </summary>
        private static string AttachNamespace = null;

        /// <summary>
        /// 添加新建脚本时默认引用的命名空间（建议调用时机：标记了[InitializeOnLoad]类的【静态构造方法】中，标记了[InitializeOnLoadMethod]的静态方法中）
        /// </summary>
        /// <param name="usingNamespace">命名空间</param>
        public static void AddUsingNamespace(string usingNamespace)
        {
            if (!UsingNamespaces.Contains(usingNamespace))
            {
                UsingNamespaces.Add(usingNamespace);
            }
        }
        /// <summary>
        /// 设置新建脚本时默认附加的命名空间（建议调用时机：标记了[InitializeOnLoad]类的【静态构造方法】中，标记了[InitializeOnLoadMethod]的静态方法中）
        /// </summary>
        /// <param name="attachNamespace">命名空间</param>
        public static void SetAttachNamespace(string attachNamespace)
        {
            AttachNamespace = attachNamespace;
        }

        private static void OnWillCreateAsset(string path)
        {
            if (path.EndsWith(".meta"))
            {
                path = path.Replace(".meta", "");
                if (path.EndsWith(".cs"))
                {
                    string className = null;
                    string code = null;
                    HashSet<string> namespaces = new HashSet<string>();
                    bool isDefault = IsDefaultScript(path, namespaces, out className);
                    if (isDefault)
                    {
                        if (string.IsNullOrEmpty(AttachNamespace))
                        {
                            code = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/HTFramework/Editor/Utility/Template/DefaultScript1Template.txt").text;
                        }
                        else
                        {
                            code = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/HTFramework/Editor/Utility/Template/DefaultScript2Template.txt").text;
                            code = code.Replace("#NAMESPACE#", AttachNamespace);
                        }
                        code = code.Replace("#CLASSNAME#", className);
                    }
                    else
                    {
                        code = File.ReadAllText(path);
                        code = code.Replace("#NAMESPACE#", string.IsNullOrEmpty(AttachNamespace) ? "MyNamespace" : AttachNamespace);
                    }

                    StringToolkit.BeginConcat();
                    for (int i = 0; i < UsingNamespaces.Count; i++)
                    {
                        if (!namespaces.Contains(UsingNamespaces[i]))
                        {
                            StringToolkit.Concat("using ");
                            StringToolkit.Concat(UsingNamespaces[i]);
                            StringToolkit.Concat(";", true);
                        }
                    }
                    string customUsing = StringToolkit.EndConcat();
                    code = code.Replace("#CUSTOMUSING#", customUsing);

                    File.WriteAllText(path, code);
                    AssetDatabase.Refresh();
                }
            }
        }
        private static bool IsDefaultScript(string path, HashSet<string> namespaces, out string className)
        {
            bool isDefault = false;
            className = null;
            string[] codes = File.ReadAllLines(path);
            for (int i = 0; i < codes.Length; i++)
            {
                if (codes[i].StartsWith("public class ") && codes[i].EndsWith(" : MonoBehaviour"))
                {
                    isDefault = true;
                    className = codes[i].Replace("public class ", "").Replace(" : MonoBehaviour", "").Trim();
                }
                else if (codes[i].StartsWith("using ") && codes[i].EndsWith(";"))
                {
                    namespaces.Add(codes[i].Replace("using ", "").Replace(";", "").Trim());
                }
            }
            return isDefault;
        }
        private static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
        {
            if (oldPath == EditorPrefsTable.HTFrameworkRootPath)
            {
                Log.Error("Move or rename the HTFramework root folder is not allowed!");
                return AssetMoveResult.FailedMove;
            }
            return AssetMoveResult.DidNotMove;
        }
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
        {
            if (assetPath == EditorPrefsTable.HTFrameworkRootPath)
            {
                Log.Error("Delete the HTFramework root folder is not allowed!");
                return AssetDeleteResult.FailedDelete;
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}