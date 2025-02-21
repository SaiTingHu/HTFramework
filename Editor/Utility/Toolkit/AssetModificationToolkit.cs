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
        /// 添加新建脚本时默认引用的命名空间（建议在类的【静态构造方法】中添加）
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
        /// 设置新建脚本时默认附加的命名空间（建议在类的【静态构造方法】中设置）
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
                    string className = ExtractClassName(path);
                    if (!string.IsNullOrEmpty(className))
                    {
                        string code = null;
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

                        StringToolkit.BeginConcat();
                        for (int i = 0; i < UsingNamespaces.Count; i++)
                        {
                            StringToolkit.Concat("using ");
                            StringToolkit.Concat(UsingNamespaces[i]);
                            StringToolkit.Concat(";", i != (UsingNamespaces.Count - 1));
                        }
                        string customUsing = StringToolkit.EndConcat();
                        code = code.Replace("#CUSTOMUSING#", customUsing);

                        File.WriteAllText(path, code);
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
        private static string ExtractClassName(string path)
        {
            string[] codes = File.ReadAllLines(path);
            for (int i = 0; i < codes.Length; i++)
            {
                if (codes[i].StartsWith("public class ") && codes[i].EndsWith(" : MonoBehaviour"))
                {
                    return codes[i].Replace("public class ", "").Replace(" : MonoBehaviour", "").Trim();
                }
            }
            return null;
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