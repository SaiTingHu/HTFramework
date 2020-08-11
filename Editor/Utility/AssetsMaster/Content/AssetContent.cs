using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 资产基类
    /// </summary>
    internal abstract class AssetContent
    {
        public HashSet<GameObject> InGameObjects { get; private set; } = new HashSet<GameObject>();
    }
}