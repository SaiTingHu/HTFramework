using System.Collections.Generic;
using System.Linq;
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
        /// AB包中的所有资源
        /// </summary>
        private List<FileInfo> _fileInfos;

        public BundleInfo(string name)
        {
            Name = name;
            _fileInfos = new List<FileInfo>();
        }

        /// <summary>
        /// AB包中的资源
        /// </summary>
        public FileInfo this[int i]
        {
            get
            {
                if (i >= 0 && i < _fileInfos.Count)
                {
                    return _fileInfos[i];
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
                return _fileInfos.Count;
            }
        }

        /// <summary>
        /// 添加资源到AB包中
        /// </summary>
        public void AddAsset(FileInfo asset)
        {
            asset.Bundled = this;
            _fileInfos.Add(asset);

            AssetImporter import = AssetImporter.GetAtPath(asset.Path);
            import.assetBundleName = Name;
        }

        /// <summary>
        /// 从AB包中删除资源
        /// </summary>
        public void RemoveAsset(FileInfo asset)
        {
            if (asset.Bundled == this)
            {
                asset.Bundled = null;
                _fileInfos.Remove(asset);
            }

            AssetImporter import = AssetImporter.GetAtPath(asset.Path);
            import.assetBundleName = "";
        }

        /// <summary>
        /// 清空AB包中的资源
        /// </summary>
        public void ClearAsset()
        {
            for (int i = 0; i < _fileInfos.Count; i++)
            {
                _fileInfos[i].Bundled = null;

                AssetImporter import = AssetImporter.GetAtPath(_fileInfos[i].Path);
                import.assetBundleName = "";
            }
            _fileInfos.Clear();
        }

        /// <summary>
        /// 重命名AB包
        /// </summary>
        public void RenameBundle(string name)
        {
            AssetDatabase.RemoveAssetBundleName(Name, true);

            Name = name;
            for (int i = 0; i < _fileInfos.Count; i++)
            {
                AssetImporter import = AssetImporter.GetAtPath(_fileInfos[i].Path);
                import.assetBundleName = Name;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
