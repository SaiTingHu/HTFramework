using System.Collections.Generic;
using UnityEditor;

namespace HT.Framework.AssetBundleEditor
{
    /// <summary>
    /// 所有的AB包
    /// </summary>
    public sealed class AssetBundleInfo
    {
        private List<BundleInfo> _bundleInfos;

        public AssetBundleInfo()
        {
            _bundleInfos = new List<BundleInfo>();
        }

        /// <summary>
        /// AB包
        /// </summary>
        public BundleInfo this[int i]
        {
            get
            {
                if (i >= 0 && i < _bundleInfos.Count)
                {
                    return _bundleInfos[i];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// AB包的数量
        /// </summary>
        public int Count
        {
            get
            {
                return _bundleInfos.Count;
            }
        }

        /// <summary>
        /// 添加新的AB包
        /// </summary>
        public void AddBundle(BundleInfo info)
        {
            _bundleInfos.Add(info);
        }

        /// <summary>
        /// 删除AB包
        /// </summary>
        public void DeleteBundle(BundleInfo info)
        {
            AssetDatabase.RemoveAssetBundleName(info.Name, true);
            info.ClearAsset();
            _bundleInfos.Remove(info);
        }

        /// <summary>
        /// 删除AB包
        /// </summary>
        public void DeleteBundle(int index)
        {
            AssetDatabase.RemoveAssetBundleName(_bundleInfos[index].Name, true);
            _bundleInfos[index].ClearAsset();
            _bundleInfos.RemoveAt(index);
        }

        /// <summary>
        /// 移除所有AB包
        /// </summary>
        public void ClearBundle()
        {
            for (int i = 0; i < _bundleInfos.Count; i++)
            {
                DeleteBundle(i);
            }
            _bundleInfos.Clear();
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }

        /// <summary>
        /// 是否已存在指定的AB包
        /// </summary>
        public bool IsExistBundle(string name)
        {
            for (int i = 0; i < _bundleInfos.Count; i++)
            {
                if (_bundleInfos[i].Name == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
