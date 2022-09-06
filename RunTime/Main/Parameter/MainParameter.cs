using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 全局主参数
    /// </summary>
    [Serializable]
    public sealed class MainParameter
    {
        public ParameterType Type = ParameterType.String;
        public string Name = "参数名称";
        public string StringValue = "";
        public int IntegerValue = 0;
        public float FloatValue = 0;
        public bool BooleanValue = false;
        public Vector2 Vector2Value = Vector2.zero;
        public Vector3 Vector3Value = Vector3.zero;
        public Color ColorValue = Color.white;
        public DataSetBase DataSet = null;
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
            /// 数据集
            /// </summary>
            DataSet,
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