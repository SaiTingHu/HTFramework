using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HT.Framework.AssetBundleEditor
{
    public static class AssetBundleTool
    {
        private static readonly string[] _invalidFileFormats = new string[] { ".cs", ".js", ".shader", ".dll", ".db" };

        private static readonly string[] _invalidFolderName = new string[] { "Resources", "HTFramework", "Editor", "Gizmos", "StreamingAssets", "Editor Default Resources" };
        
        /// <summary>
        /// 读取资源文件夹下的所有子资源
        /// </summary>
        public static void ReadAssetsInChildren(FolderInfo asset, List<FileInfo> validFilesCache)
        {
            DirectoryInfo di = new DirectoryInfo(asset.FullPath);
            FileSystemInfo[] fileinfos = di.GetFileSystemInfos();
            foreach (FileSystemInfo fileinfo in fileinfos)
            {
                if (fileinfo is DirectoryInfo)
                {
                    if (IsValidFolder(fileinfo.Name))
                    {
                        FolderInfo fi = new FolderInfo(fileinfo.FullName, fileinfo.Name, fileinfo.Extension);
                        asset.ChildAssetInfo.Add(fi);
                        
                        ReadAssetsInChildren(fi, validFilesCache);
                    }
                }
                else
                {
                    if (fileinfo.Extension != ".meta")
                    {
                        FileInfo fi = new FileInfo(fileinfo.FullName, fileinfo.Name, fileinfo.Extension);
                        asset.ChildAssetInfo.Add(fi);

                        if (fi.IsValid)
                        {
                            validFilesCache.Add(fi);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取AB包配置信息
        /// </summary>
        public static void ReadAssetBundleConfig(AssetBundleInfo abInfo, List<FileInfo> validAssetsCache)
        {
            string[] names = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < names.Length; i++)
            {
                BundleInfo build = new BundleInfo(names[i]);
                string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(names[i]);
                for (int j = 0; j < assets.Length; j++)
                {
                    FileInfo fi = validAssetsCache.Find((f) => { return f.GUID == AssetDatabase.AssetPathToGUID(assets[j]); });
                    if (fi != null)
                    {
                        build.AddAsset(fi);
                        fi.IsChecked = false;
                    }
                }
                abInfo.AddBundle(build);
            }
        } 
        
        /// <summary>
        /// 获取所有被选中的有效资源
        /// </summary>
        public static List<FileInfo> GetCheckedAssets(this List<FileInfo> validAssetList)
        {
            List<FileInfo> checkedAssets = new List<FileInfo>();
            for (int i = 0; i < validAssetList.Count; i++)
            {
                if (validAssetList[i].IsChecked)
                {
                    checkedAssets.Add(validAssetList[i]);
                }
            }
            return checkedAssets;
        }
        
        /// <summary>
        /// 通过文件后缀名判断是否是有效的文件
        /// </summary>
        public static bool IsValidFile(string extension)
        {
            foreach (string name in _invalidFileFormats)
            {
                if (name == extension)
                {
                    return false;
                }
            }
            return true;
        } 

        /// <summary>
        /// 是否是有效的文件夹
        /// </summary>
        public static bool IsValidFolder(string folderName)
        {
            foreach (string name in _invalidFolderName)
            {
                if (name == folderName)
                {
                    return false;
                }
            }
            return true;
        } 
        
        /// <summary>
        /// 改变资源及其子资源的选中状态
        /// </summary>
        public static void ChangeCheckedInChildren(AssetInfo asset, bool isChecked)
        {
            asset.IsChecked = isChecked;
            if (asset is FolderInfo)
            {
                FolderInfo fi = asset as FolderInfo;
                if (fi.IsEmpty)
                {
                    fi.IsChecked = false;
                }
                else
                {
                    for (int i = 0; i < fi.ChildAssetInfo.Count; i++)
                    {
                        ChangeCheckedInChildren(fi.ChildAssetInfo[i], isChecked);
                    }
                }
            }
            else
            {
                FileInfo fi = asset as FileInfo;
                if (!fi.IsValid || fi.Bundled != null)
                {
                    fi.IsChecked = false;
                }
            }
        } 
        
        /// <summary>
        /// 打开文件夹
        /// </summary>
        public static void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        /// <summary>
        /// 打包资源
        /// </summary>
        [MenuItem("HTFramework/AssetBundle/Build AssetBundles")]
        public static void BuildAssetBundles()
        {
            string buildPath = EditorPrefs.GetString("BuildPath", "");
            if (!Directory.Exists(buildPath))
            {
                Debug.LogError("Please set build path！");
                return;
            }

            BuildTarget target = (BuildTarget)EditorPrefs.GetInt("BuildTarget", 5);

            BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.None, target);
        }
    }
}
