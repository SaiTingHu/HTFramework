using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤参数
    /// </summary>
    [Serializable]
    public sealed class StepParameter
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
        public GameObject GameObjectValue = null;
        public string GameObjectGUID = "<None>";
        public string GameObjectPath = "<None>";

        /// <summary>
        /// 克隆
        /// </summary>
        public StepParameter Clone()
        {
            StepParameter parameter = new StepParameter();
            parameter.Type = Type;
            parameter.Name = Name;
            parameter.StringValue = StringValue;
            parameter.IntegerValue = IntegerValue;
            parameter.FloatValue = FloatValue;
            parameter.BooleanValue = BooleanValue;
            parameter.Vector2Value = Vector2Value;
            parameter.Vector3Value = Vector3Value;
            parameter.ColorValue = ColorValue;
            parameter.GameObjectValue = GameObjectValue;
            parameter.GameObjectGUID = GameObjectGUID;
            parameter.GameObjectPath = GameObjectPath;

            return parameter;
        }
        
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
            /// 游戏物体
            /// </summary>
            GameObject
        }
    }
}