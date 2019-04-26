using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HT.Framework.AssetBundleEditor
{
    public static class AssetBundleEditorUtility
    {
        private static readonly string[] _invalidFileFormats = new string[] { ".cs", ".js", ".shader", ".dll", ".db" };
        private static readonly string[] _invalidFolderName = new string[] {
            "Resources", "Editor", "Gizmos", "StreamingAssets", "Editor Default Resources",
            "HTFramework", "HTFrameworkAI", "HTFrameworkAuxiliary"
        };
        private static Dictionary<string, FileInfo> _fileInfos = new Dictionary<string, FileInfo>();
        private static Dictionary<string, FolderInfo> _folderInfos = new Dictionary<string, FolderInfo>();
        private static Dictionary<string, BundleInfo> _bundleInfos = new Dictionary<string, BundleInfo>();
        /// <summary>
        /// 当前的所有AB包对象
        /// </summary>
        public static List<BundleInfo> BundleInfosList { get; } = new List<BundleInfo>();

        /// <summary>
        /// 通过路径获取资源文件对象
        /// </summary>
        public static FileInfo GetFileInfoByPath(string filePath)
        {
            if (_fileInfos.ContainsKey(filePath))
            {
                return _fileInfos[filePath];
            }
            else
            {
                string fullPath = Application.dataPath + filePath.Replace("Assets", "");
                string name = Path.GetFileName(fullPath);
                string extension = Path.GetExtension(fullPath);
                FileInfo file = new FileInfo(fullPath, name, extension);
                _fileInfos.Add(filePath, file);
                return file;
            }
        }
        /// <summary>
        /// 通过全路径获取资源文件夹对象
        /// </summary>
        public static FolderInfo GetFolderInfoByFullPath(string fullPath)
        {
            if (_folderInfos.ContainsKey(fullPath))
            {
                return _folderInfos[fullPath];
            }
            else
            {
                string name = Path.GetFileName(fullPath);
                FolderInfo folder = new FolderInfo(fullPath, name);
                _folderInfos.Add(fullPath, folder);
                return folder;
            }
        }
        /// <summary>
        /// 通过名称获取AB包对象
        /// </summary>
        public static BundleInfo GetBundleInfoByName(string bundleName)
        {
            if (_bundleInfos.ContainsKey(bundleName))
            {
                return _bundleInfos[bundleName];
            }
            else
            {
                BundleInfo bundle = new BundleInfo(bundleName);
                _bundleInfos.Add(bundleName, bundle);
                BundleInfosList.Add(bundle);
                return bundle;
            }
        }
        /// <summary>
        /// 重命名AB包对象
        /// </summary>
        public static void RenameBundleInfo(string name, string newName)
        {
            if (_bundleInfos.ContainsKey(name))
            {
                BundleInfo bundle = _bundleInfos[name];
                _bundleInfos.Remove(name);
                _bundleInfos.Add(newName, bundle);
                bundle.RenameBundle(newName);
            }
        }
        /// <summary>
        /// 通过名称删除AB包对象
        /// </summary>
        public static void DeleteBundleInfoByName(string bundleName)
        {
            BundleInfo bundle = GetBundleInfoByName(bundleName);
            bundle.ClearAsset();
            _bundleInfos.Remove(bundleName);
            BundleInfosList.Remove(bundle);
            AssetDatabase.RemoveAssetBundleName(bundleName, true);
        }
        /// <summary>
        /// 是否已存在指定的AB包对象
        /// </summary>
        public static bool IsExistBundleInfo(string bundleName)
        {
            return _bundleInfos.ContainsKey(bundleName);
        }
        /// <summary>
        /// 读取AB包配置信息
        /// </summary>
        public static void ReadAssetBundleConfig()
        {
            string[] names = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < names.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Hold On", "Collect AssetBundle Config[" + i + "/" + names.Length + "]......", (float)i / names.Length);

                BundleInfo build = GetBundleInfoByName(names[i]);
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(names[i]);
                for (int j = 0; j < assetPaths.Length; j++)
                {
                    build.AddAsset(assetPaths[j]);
                }
            }
            EditorUtility.ClearProgressBar();
        }
        /// <summary>
        /// 读取资源文件夹下的子资源
        /// </summary>
        public static void ReadAssetsInChildren(FolderInfo folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder.FullPath);
            FileSystemInfo[] fileinfos = di.GetFileSystemInfos();
            for (int i = 0; i < fileinfos.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Hold On", "Collect Assets[" + i + "/" + fileinfos.Length + "]......", (float)i / fileinfos.Length);

                if (fileinfos[i] is DirectoryInfo)
                {
                    if (IsValidFolder(fileinfos[i].Name))
                    {
                        FolderInfo fi = GetFolderInfoByFullPath(fileinfos[i].FullName);
                        folder.ChildAssetInfo.Add(fi);
                    }
                }
                else
                {
                    if (fileinfos[i].Extension != ".meta")
                    {
                        FileInfo fi = GetFileInfoByPath(GetAssetPathByFullPath(fileinfos[i].FullName));
                        folder.ChildAssetInfo.Add(fi);
                    }
                }
            }
            EditorUtility.ClearProgressBar();
        }
        /// <summary>
        /// 读取资源文件的依赖关系
        /// </summary>
        public static void ReadAssetDependencies(string filePath)
        {
            FileInfo file = GetFileInfoByPath(filePath);
            string[] paths = AssetDatabase.GetDependencies(filePath);
            for (int i = 0; i < paths.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Hold On", "Collect Dependencies[" + i + "/" + paths.Length + "]", (float)i / paths.Length);

                if (filePath != paths[i])
                {
                    file.Dependencies.Add(paths[i]);
                    FileInfo dFile = GetFileInfoByPath(paths[i]);
                    dFile.BeDependencies.Add(filePath);
                }
            }
            EditorUtility.ClearProgressBar();
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
        /// 判断资源文件是否是冗余资源
        /// </summary>
        public static void IsRedundantFile(FileInfo fileInfo)
        {
            if (fileInfo.Bundled != "")
            {
                if (fileInfo.IndirectBundled.Count < 1)
                {
                    fileInfo.IsRedundant = false;
                }
                else if (fileInfo.IndirectBundled.Count == 1)
                {
                    fileInfo.IsRedundant = !fileInfo.IndirectBundled.ContainsKey(fileInfo.Bundled);
                }
                else
                {
                    fileInfo.IsRedundant = true;
                }
            }
            else
            {
                fileInfo.IsRedundant = fileInfo.IndirectBundled.Count > 1;
            }
        }
        /// <summary>
        /// 通过资源全路径获取资源路径
        /// </summary>
        public static string GetAssetPathByFullPath(string fullPath)
        {
            fullPath = fullPath.Replace("\\", "/");
            fullPath = "Assets" + fullPath.Replace(Application.dataPath, "");
            return fullPath;
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
        /// 清理数据
        /// </summary>
        public static void ClearData()
        {
            _fileInfos.Clear();
            _folderInfos.Clear();
            _bundleInfos.Clear();
            BundleInfosList.Clear();
        }
        /// <summary>
        /// 打包资源
        /// </summary>
        [MenuItem("HTFramework/AssetBundle/Build AssetBundles")]
        public static void BuildAssetBundles()
        {
            string buildPath = EditorPrefs.GetString(Application.productName + ".AssetBundleEditor.BuildPath", Application.streamingAssetsPath);
            if (!Directory.Exists(buildPath))
            {
                GlobalTools.LogError("Please set build path!");
                return;
            }

            BuildTarget target = (BuildTarget)EditorPrefs.GetInt(Application.productName + ".AssetBundleEditor.BuildTarget", 5);
            BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.None, target);
            GlobalTools.LogInfo("Build completed!");
            OpenFolder(buildPath);
        }
    }
}
