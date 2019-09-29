using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 主要参数
    /// </summary>
    [Serializable]
    public sealed class MainParameter
    {
        public ParameterType Type = ParameterType.String;
        public string Name = "参数名称";
        public string StringValue;
        public int IntegerValue;
        public float FloatValue;
        public bool BooleanValue;
        public Vector2 Vector2Value;
        public Vector3 Vector3Value;
        public Color ColorValue;
        public GameObject PrefabValue = null;
        public Texture TextureValue = null;
        public AudioClip AudioClipValue = null;
        public Material MaterialValue = null;

        /// <summary>
        /// 参数类型
        /// </summary>
        public enum ParameterType
        {
            /// <summary>
            /// 字符串
            /// </summary>
            String,
            /// <summary>
            /// 整型
            /// </summary>
            Integer,
            /// <summary>
            /// 小数
            /// </summary>
            Float,
            /// <summary>
            /// bool
            /// </summary>
            Boolean,
            /// <summary>
            /// 二维向量
            /// </summary>
            Vector2,
            /// <summary>
            /// 三维向量
            /// </summary>
            Vector3,
            /// <summary>
            /// 颜色
            /// </summary>
            Color,
            /// <summary>
            /// 预制体
            /// </summary>
            Prefab,
            /// <summary>
            /// 图片
            /// </summary>
            Texture,
            /// <summary>
            /// 音频
            /// </summary>
            AudioClip,
            /// <summary>
            /// 材质
            /// </summary>
            Material
        }
    }
}