using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HT.Framework.AssetBundleEditor
{
    public static class AssetBundleEditorUtility
    {
        private static readonly HashSet<string> _invalidFileFormats = new HashSet<string>() { ".cs", ".js", ".shader", ".dll", ".db", ".jslib", ".asmdef" };
        private static readonly HashSet<string> _invalidFolderName = new HashSet<string>() { "Resources", "Editor", "Gizmos", "StreamingAssets", "Editor Default Resources" };
        private static Dictionary<string, AssetFileInfo> _fileInfos = new Dictionary<string, AssetFileInfo>();
        private static Dictionary<string, AssetFolderInfo> _folderInfos = new Dictionary<string, AssetFolderInfo>();
        private static Dictionary<string, BundleInfo> _bundleInfos = new Dictionary<string, BundleInfo>();

        /// <summary>
        /// 当前的所有AB包对象
        /// </summary>
        public static List<BundleInfo> BundleInfosList { get; } = new List<BundleInfo>();
        /// <summary>
        /// 当前的所有资源文件对象
        /// </summary>
        public static Dictionary<string, AssetFileInfo> FileInfos
        {
            get
            {
                return _fileInfos;
            }
        }

        /// <summary>
        /// 通过路径获取资源文件对象
        /// </summary>
        public static AssetFileInfo GetFileInfoByPath(string filePath)
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
                AssetFileInfo file = new AssetFileInfo(fullPath, name, extension);
                _fileInfos.Add(filePath, file);
                return file;
            }
        }
        /// <summary>
        /// 通过全路径获取资源文件夹对象
        /// </summary>
        public static AssetFolderInfo GetFolderInfoByFullPath(string fullPath)
        {
            if (_folderInfos.ContainsKey(fullPath))
            {
                return _folderInfos[fullPath];
            }
            else
            {
                string name = Path.GetFileName(fullPath);
                AssetFolderInfo folder = new AssetFolderInfo(fullPath, name);
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
            if (_bundleInfos.ContainsKey(bundleName))
            {
                BundleInfo bundle = _bundleInfos[bundleName];
                bundle.ClearAsset();
                _bundleInfos.Remove(bundleName);
                BundleInfosList.Remove(bundle);
                AssetDatabase.RemoveAssetBundleName(bundleName, true);
            }
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
        public static void ReadAssetsInChildren(AssetFolderInfo folder)
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
                        AssetFolderInfo fi = GetFolderInfoByFullPath(fileinfos[i].FullName);
                        folder.ChildAsset.Add(fi);
                    }
                }
                else
                {
                    if (fileinfos[i].Extension != ".meta")
                    {
                        AssetFileInfo fi = GetFileInfoByPath(GetAssetPathByFullPath(fileinfos[i].FullName));
                        folder.ChildAsset.Add(fi);
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
            AssetFileInfo file = GetFileInfoByPath(filePath);
            string[] paths = AssetDatabase.GetDependencies(filePath);
            for (int i = 0; i < paths.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Hold On", "Collect Dependencies[" + i + "/" + paths.Length + "]", (float)i / paths.Length);

                if (filePath != paths[i])
                {
                    file.Dependencies.Add(paths[i]);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 通过文件夹名判断是否是有效的文件夹
        /// </summary>
        public static bool IsValidFolder(string folderName)
        {
            if (_invalidFolderName.Contains(folderName))
            {
                return false;
            }
            if (EditorGlobalTools.HTFrameworkFolder.Contains(folderName))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 通过文件后缀名判断是否是有效的文件
        /// </summary>
        public static bool IsValidFile(string extension)
        {
            if (_invalidFileFormats.Contains(extension))
            {
                return false;
            }
            return true;
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
        /// 【验证方法】打包资源
        /// </summary>
        [@MenuItem("HTFramework/AssetBundle/Build AssetBundles", true)]
        private static bool BuildAssetBundlesValidate()
        {
            return !EditorApplication.isPlaying;
        }
        /// <summary>
        /// 打包资源
        /// </summary>
        [@MenuItem("HTFramework/AssetBundle/Build AssetBundles")]
        public static void BuildAssetBundles()
        {
            if (EditorUtility.DisplayDialog("Prompt", "Are you sure build assetBundles？This could be a time consuming job.", "Yes", "No"))
            {
                string buildPath = EditorPrefs.GetString(EditorPrefsTable.AssetBundleEditor_BuildPath, Application.streamingAssetsPath);
                if (!Directory.Exists(buildPath))
                {
                    Directory.CreateDirectory(buildPath);
                }

                BuildTarget target = (BuildTarget)EditorPrefs.GetInt(EditorPrefsTable.AssetBundleEditor_BuildTarget, 5);
                BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.None, target);

                GlobalTools.LogInfo("Build assetBundle succeeded!");

                string variant = EditorPrefs.GetString(EditorPrefsTable.AssetBundleEditor_Variant, "");
                if (variant != "")
                {
                    DirectoryInfo di = new DirectoryInfo(buildPath);
                    FileSystemInfo[] fileinfos = di.GetFileSystemInfos();
                    for (int i = 0; i < fileinfos.Length; i++)
                    {
                        FileInfo file = fileinfos[i] as FileInfo;
                        if (file != null && file.Extension == "")
                        {
                            file.MoveTo(file.FullName + "." + variant);
                        }
                    }
                }

                GlobalTools.LogInfo("Additional variant succeeded!");

                OpenFolder(buildPath);
            }
        }
    }
}
