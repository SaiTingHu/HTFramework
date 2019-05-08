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
        /// AB包占硬盘总内存大小
        /// </summary>
        public long MemorySize;
        /// <summary>
        /// AB包占硬盘总内存大小（显示格式）
        /// </summary>
        public string MemorySizeFormat;

        /// <summary>
        /// AB包中的所有资源路径
        /// </summary>
        private List<string> _filePaths;
        /// <summary>
        /// 排序方式
        /// </summary>
        private BundleAssetSortMode _sortMode;

        public BundleInfo(string name)
        {
            Name = name;
            MemorySize = 0;
            MemorySizeFormat = EditorUtility.FormatBytes(MemorySize);
            _filePaths = new List<string>();
            _sortMode = BundleAssetSortMode.FromBig;
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
                MemorySize += file.MemorySize;
                file.ReadDependenciesFile();
                file.UpdateRedundantState();
                _filePaths.Add(filePath);                

                for (int i = 0; i < file.Dependencies.Count; i++)
                {
                    AssetFileInfo depenFile = AssetBundleEditorUtility.GetFileInfoByPath(file.Dependencies[i]);
                    if (depenFile.IsValid)
                    {
                        if (!depenFile.IndirectBundled.ContainsKey(Name))
                        {
                            depenFile.IndirectBundled.Add(Name, 0);
                            MemorySize += depenFile.MemorySize;
                        }
                        depenFile.IndirectBundled[Name] = depenFile.IndirectBundled[Name] + 1;
                        if (!depenFile.IndirectBundledRelation.ContainsKey(filePath))
                        {
                            depenFile.IndirectBundledRelation.Add(filePath, Name);
                        }
                        depenFile.UpdateRedundantState();
                    }
                }
                MemorySizeFormat = EditorUtility.FormatBytes(MemorySize);
            }

            AssetImporter import = AssetImporter.GetAtPath(filePath);
            import.assetBundleName = Name;
        }

        /// <summary>
        /// 从AB包中移除资源
        /// </summary>
        public void RemoveAsset(string filePath)
        {
            AssetFileInfo file = AssetBundleEditorUtility.GetFileInfoByPath(filePath);
            if (file.Bundled == Name)
            {
                file.Bundled = "";
                MemorySize -= file.MemorySize;
                file.ReadDependenciesFile();
                file.UpdateRedundantState();
                _filePaths.Remove(filePath);

                for (int i = 0; i < file.Dependencies.Count; i++)
                {
                    AssetFileInfo depenFile = AssetBundleEditorUtility.GetFileInfoByPath(file.Dependencies[i]);
                    if (depenFile.IndirectBundled.ContainsKey(Name))
                    {
                        depenFile.IndirectBundled[Name] = depenFile.IndirectBundled[Name] - 1;
                        if (depenFile.IndirectBundled[Name] <= 0)
                        {
                            depenFile.IndirectBundled.Remove(Name);
                            MemorySize -= depenFile.MemorySize;
                        }
                    }
                    if (depenFile.IndirectBundledRelation.ContainsKey(filePath))
                    {
                        depenFile.IndirectBundledRelation.Remove(filePath);
                    }
                    depenFile.UpdateRedundantState();
                }
                MemorySizeFormat = EditorUtility.FormatBytes(MemorySize);
            }

            AssetImporter import = AssetImporter.GetAtPath(filePath);
            import.assetBundleName = "";
        }

        /// <summary>
        /// 清空AB包中的资源
        /// </summary>
        public void ClearAsset()
        {
            while (_filePaths.Count > 0)
            {
                RemoveAsset(_filePaths[0]);
            }
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
                    if (depenFile.IndirectBundled.ContainsKey(Name))
                    {
                        int number = depenFile.IndirectBundled[Name];
                        depenFile.IndirectBundled.Remove(Name);
                        depenFile.IndirectBundled.Add(name, number);
                    }
                    if (depenFile.IndirectBundledRelation.ContainsKey(_filePaths[i]))
                    {
                        depenFile.IndirectBundledRelation[_filePaths[i]] = name;
                    }
                }
            }

            AssetDatabase.RemoveAssetBundleName(Name, true);
            Name = name;
        }

        /// <summary>
        /// 改变排序方式
        /// </summary>
        public void ChangeSortMode()
        {
            if (_sortMode == BundleAssetSortMode.FromBig)
            {
                _sortMode = BundleAssetSortMode.FromSmall;
                _filePaths.Sort((a, b) =>
                {
                    AssetFileInfo afile = AssetBundleEditorUtility.GetFileInfoByPath(a);
                    AssetFileInfo bfile = AssetBundleEditorUtility.GetFileInfoByPath(b);
                    if (afile.MemorySize > bfile.MemorySize)
                        return 1;
                    else if (afile.MemorySize == bfile.MemorySize)
                        return 0;
                    else
                        return -1;
                });
            }
            else
            {
                _sortMode = BundleAssetSortMode.FromBig;
                _filePaths.Sort((a, b) =>
                {
                    AssetFileInfo afile = AssetBundleEditorUtility.GetFileInfoByPath(a);
                    AssetFileInfo bfile = AssetBundleEditorUtility.GetFileInfoByPath(b);
                    if (afile.MemorySize > bfile.MemorySize)
                        return -1;
                    else if (afile.MemorySize == bfile.MemorySize)
                        return 0;
                    else
                        return 1;
                });
            }
        }

        public override string ToString()
        {
            return string.Format("{0} [Asset {1}]", Name, _filePaths.Count);
        }

        public enum BundleAssetSortMode
        {
            /// <summary>
            /// 从大到小
            /// </summary>
            FromBig,
            /// <summary>
            /// 从小到大
            /// </summary>
            FromSmall
        }
    }
}
