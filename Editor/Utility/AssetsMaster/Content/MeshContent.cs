using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 网格资产
    /// </summary>
    internal sealed class MeshContent : AssetContent
    {
        public Mesh Me { get; private set; }
        public int VertexCount { get; private set; }
        public HashSet<GameObject> InStaticBatching { get; private set; } = new HashSet<GameObject>();
        public HashSet<GameObject> InSkinned { get; private set; } = new HashSet<GameObject>();
        
        public MeshContent(Mesh me)
        {
            Me = me;
            VertexCount = me.vertexCount;
        }
    }
}