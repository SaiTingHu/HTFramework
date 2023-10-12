using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace HT.Framework
{
    /// <summary>
    /// 项目资源文件修改监控工具箱
    /// </summary>
    public sealed class AssetModificationToolkit : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// 新建脚本时默认引用的命名空间
        /// </summary>
        private static HashSet<string> UsingNamespaces = new HashSet<string>() { "HT.Framework", "DG.Tweening" };

        /// <summary>
        /// 添加新建脚本时默认引用的命名空间（建议在类的【静态构造方法】中添加）
        /// </summary>
        /// <param name="assembly">命名空间</param>
        public static void AddUsingNamespace(string usingNamespace)
        {
            UsingNamespaces.Add(usingNamespace);
        }

        private static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            if (path.EndsWith(".cs"))
            {
                bool isFix = false;
                bool isInAwake = false;
                List<string> codes = File.ReadAllLines(path).ToList();
                for (int i = 0; i < codes.Count; i++)
                {
                    if (!isFix)
                    {
                        if (codes[i].Contains(" class ") && codes[i].EndsWith(": MonoBehaviour"))
                        {
                            codes[i] = codes[i].Replace(": MonoBehaviour", ": HTBehaviour");
                            codes[i] = codes[i] + ", IUpdateFrame";
                            isFix = true;
                            continue;
                        }
                    }
                    if (isFix)
                    {
                        if (codes[i].Contains("// Start"))
                        {
                            codes[i] = "    //初始化操作在 Awake 中完成（必须确保 base.Awake() 的存在）";
                        }
                        else if (codes[i].Contains("void Start()"))
                        {
                            codes[i] = "    protected override void Awake()";
                            isInAwake = true;
                        }
                        else if (isInAwake && codes[i].Contains("{"))
                        {
                            codes.Insert(i + 1, "        base.Awake();");
                            isInAwake = false;
                        }
                        else if (codes[i].Contains("// Update"))
                        {
                            codes[i] = "    //等同于 Update 方法，不过当主框架进入暂停状态时，此方法也会停止调用（Main.Current.Pause = true）";
                        }
                        else if (codes[i].Contains("void Update()"))
                        {
                            codes[i] = "    public void OnUpdateFrame()";
                        }
                    }
                }
                if (isFix)
                {
                    foreach (var item in UsingNamespaces)
                    {
                        codes.Insert(0, $"using {item};");
                    }

                    File.WriteAllLines(path, codes.ToArray());
                    AssetDatabase.Refresh();
                }
            }
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