using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 材质资产
    /// </summary>
    internal sealed class MaterialContent : AssetContent
    {
        public Material Mat { get; private set; }
        public string ShaderName { get; private set; }
        public int RenderQueue { get; private set; }

        public MaterialContent(Material mat)
        {
            Mat = mat;
            ShaderName = Mat.shader != null ? Mat.shader.name : "<None>";
            RenderQueue = Mat.renderQueue;
        }
    }
}