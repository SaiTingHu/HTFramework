using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 丢失的资产
    /// </summary>
    internal sealed class MissingContent : AssetContent
    {
        public GameObject Target { get; private set; }
        public HashSet<string> MissingInfos { get; private set; } = new HashSet<string>();

        public MissingContent(GameObject target)
        {
            Target = target;
        }
    }
}