using System.Collections.Generic;
using UnityEditor;

namespace HT.Framework.AssetBundleEditor
{
    /// <summary>
    /// AB包
    /// </summary>
    public sealed class BundleInfo
    {
        /// <summary>
        /// AB包的名称
        /// </summary>
        public string Name;

        /// <summary>
        /// AB包中的所有资源路径
        /// </summary>
        private List<string> _filePaths;

        public BundleInfo(string name)
        {
            Name = name;
            _filePaths = new List<string>();
        }

        /// <summary>
        /// AB包中的资源
        /// </summary>
        public string this[int i]
        {
            get
            {
                if (i >= 0 && i < _filePaths.Count)
                {
                    return _filePaths[i];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// AB包中的资源数量
        /// </summary>
        public int Count
        {
            get
            {
                return _filePaths.Count;
            }
        }

        /// <summary>
        /// 添加资源到AB包中
        /// </summary>
        public void AddAsset(string filePath)
        {
            AssetFileInfo file = AssetBundleEditorUtility.GetFileInfoByPath(filePath);
            if (file.Bundled != Name)
            {
                file.Bundled = Name;
                file.ReadDependenciesFile();
                _filePaths.Add(filePath);                

                for (int i = 0; i < file.Dependencies.Count; i++)
                {
                    AssetFileInfo depenFile = AssetBundleEditorUtility.GetFileInfoByPath(file.Dependencies[i]);
                    if (!depenFile.IndirectBundled.ContainsKey(Name))
                    {
                        depenFile.IndirectBundled.Add(Name, 0);
                    }
                    depenFile.IndirectBundled[Name] = depenFile.IndirectBundled[Name] + 1;
                }
            }

            AssetImporter import = AssetImporter.GetAtPath(filePath);
            import.assetBundleName = Name;
        }

        /// <summary>
        /// 从AB包中删除资源
        /// </summary>
        public void RemoveAsset(string filePath)
        {
            AssetFileInfo file = AssetBundleEditorUtility.GetFileInfoByPath(filePath);
            if (file.Bundled == Name)
            {
                file.Bundled = "";
                file.ReadDependenciesFile();
                _filePaths.Remove(filePath);

                for (int i = 0; i < file.Dependencies.Count; i++)
                {
                    AssetFileInfo depenFile = AssetBundleEditorUtility.GetFileInfoByPath(file.Dependencies[i]);
                    if (!depenFile.IndirectBundled.ContainsKey(Name))
                    {
                        continue;
                    }
                    depenFile.IndirectBundled[Name] = depenFile.IndirectBundled[Name] - 1;
                    if (depenFile.IndirectBundled[Name] <= 0)
                    {
                        depenFile.IndirectBundled.Remove(Name);
                    }
                }
            }

            AssetImporter import = AssetImporter.GetAtPath(filePath);
            import.assetBundleName = "";
        }

        /// <summary>
        /// 清空AB包中的资源
        /// </summary>
        public void ClearAsset()
        {
            for (int i = 0; i < _filePaths.Count; i++)
            {
                AssetFileInfo file = AssetBundleEditorUtility.GetFileInfoByPath(_filePaths[i]);
                file.ReadDependenciesFile();
                file.Bundled = "";

                for (int j = 0; j < file.Dependencies.Count; j++)
                {
                    AssetFileInfo depenFile = AssetBundleEditorUtility.GetFileInfoByPath(file.Dependencies[j]);
                    if (!depenFile.IndirectBundled.ContainsKey(Name))
                    {
                        continue;
                    }
                    depenFile.IndirectBundled[Name] = depenFile.IndirectBundled[Name] - 1;
                    if (depenFile.IndirectBundled[Name] <= 0)
                    {
                        depenFile.IndirectBundled.Remove(Name);
                    }
                }

                AssetImporter import = AssetImporter.GetAtPath(_filePaths[i]);
                import.assetBundleName = "";
            }
            _filePaths.Clear();
        }

        /// <summary>
        /// 重命名AB包
        /// </summary>
        public void RenameBundle(string name)
        {
            for (int i = 0; i < _filePaths.Count; i++)
            {
                AssetFileInfo file = AssetBundleEditorUtility.GetFileInfoByPath(_filePaths[i]);
                file.ReadDependenciesFile();
                file.Bundled = name;
                AssetImporter import = AssetImporter.GetAtPath(_filePaths[i]);
                import.assetBundleName = name;

                for (int j = 0; j < file.Dependencies.Count; j++)
                {
                    AssetFileInfo depenFile = AssetBundleEditorUtility.GetFileInfoByPath(file.Dependencies[j]);
                    if (!depenFile.IndirectBundled.ContainsKey(Name))
                    {
                        continue;
                    }
                    int number = depenFile.IndirectBundled[Name];
                    depenFile.IndirectBundled.Remove(Name);
                    depenFile.IndirectBundled.Add(name, number);
                }
            }

            AssetDatabase.RemoveAssetBundleName(Name, true);
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
