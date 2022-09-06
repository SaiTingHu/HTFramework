using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace HT.Framework
{
    /// <summary>
    /// 项目资源文件修改监控工具箱
    /// </summary>
    internal sealed class AssetModificationToolkit : UnityEditor.AssetModificationProcessor
    {
        private static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            if (path.EndsWith(".cs"))
            {
                bool isFix = false;
                List<string> codes = File.ReadAllLines(path).ToList();
                for (int i = 0; i < codes.Count; i++)
                {
                    if (!isFix)
                    {
                        if (codes[i].Contains(" class ") && codes[i].EndsWith("MonoBehaviour"))
                        {
                            codes[i] = codes[i].Replace("MonoBehaviour", "HTBehaviour");
                            isFix = true;
                        }
                    }
                    if (isFix)
                    {
                        if (codes[i].Contains("{"))
                        {
                            codes.Insert(i + 1, "    //启用自动化");
                            codes.Insert(i + 2, "    protected override bool IsAutomate => true;");
                            codes.Insert(i + 3, "    ");
                            break;
                        }
                    }
                }
                if (isFix)
                {
                    codes.Insert(0, "using DG.Tweening;");
                    codes.Insert(0, "using HT.Framework;");
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