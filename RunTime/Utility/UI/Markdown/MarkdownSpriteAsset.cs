using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// Markdown图像资源
    /// </summary>
    [CreateAssetMenu(menuName = "HTFramework/Markdown Sprite Asset", order = 20)]
    [Serializable]
    public sealed class MarkdownSpriteAsset : DataSetBase
    {
        public List<SpriteTarget> SpriteTargets = new List<SpriteTarget>();

        [Serializable]
        public class SpriteTarget
        {
            public string ID;
            public Sprite Target;
        }
    }
}