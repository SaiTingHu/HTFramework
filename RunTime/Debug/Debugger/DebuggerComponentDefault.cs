using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 缺省的组件检视器
    /// </summary>
    internal sealed class DebuggerComponentDefault : DebuggerComponentBase
    {
        private Behaviour _behaviour;
        private List<DisplayableField> _displayableFields = new List<DisplayableField>();

        public override void OnEnable()
        {
            _behaviour = Target as Behaviour;
            FieldInfo[] fields = Target.GetType().GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                DisplayableField field = Main.m_ReferencePool.Spawn<DisplayableField>();
                field.SetFieldInfo(fields[i], Target);
                _displayableFields.Add(field);
            }
        }
        public override void OnDebuggerGUI()
        {
            if (_behaviour != null)
            {
                _behaviour.enabled = BoolField("Enabled", _behaviour.enabled);
            }

            if (_displayableFields.Count == 0)
            {
                GUILayout.Label("No any displayable field!");
            }
            else
            {
                for (int i = 0; i < _displayableFields.Count; i++)
                {
                    DrawDisplayableField(_displayableFields[i]);
                }
            }
        }
        public override void Reset()
        {
            base.Reset();

            for (int i = 0; i < _displayableFields.Count; i++)
            {
                Main.m_ReferencePool.Despawn(_displayableFields[i]);
            }
            _displayableFields.Clear();
        }

        /// <summary>
        /// 绘制一个可显示字段
        /// </summary>
        private void DrawDisplayableField(DisplayableField displayableField)
        {
            switch (displayableField.FieldType)
            {
                case DisplayableField.DisplayableFieldType.Array:
                    IntFieldReadOnly(displayableField.Name + "(Length)", displayableField.ArrayLength);
                    break;
                case DisplayableField.DisplayableFieldType.Integer:
                    int intValue = IntField(displayableField.Name, displayableField.IntegerValue);
                    if (intValue != displayableField.IntegerValue)
                    {
                        displayableField.IntegerValue = intValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Boolean:
                    bool boolValue = BoolField(displayableField.Name, displayableField.BooleanValue);
                    if (boolValue != displayableField.BooleanValue)
                    {
                        displayableField.BooleanValue = boolValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Float:
                    float floatValue = FloatField(displayableField.Name, displayableField.FloatValue);
                    if (floatValue != displayableField.FloatValue)
                    {
                        displayableField.FloatValue = floatValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.String:
                    string stringValue = StringField(displayableField.Name, displayableField.StringValue);
                    if (stringValue != displayableField.StringValue)
                    {
                        displayableField.StringValue = stringValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Color:
                    Color colorValue = ColorField(displayableField.Name, displayableField.ColorValue);
                    if (colorValue != displayableField.ColorValue)
                    {
                        displayableField.ColorValue = colorValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Enum:
                    Enum enumValue = EnumField(displayableField.Name, displayableField.EnumValue);
                    if (enumValue != displayableField.EnumValue)
                    {
                        displayableField.EnumValue = enumValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Vector2:
                    Vector2 v2Value = Vector2Field(displayableField.Name, displayableField.Vector2Value);
                    if (v2Value != displayableField.Vector2Value)
                    {
                        displayableField.Vector2Value = v2Value;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Vector3:
                    Vector3 v3Value = Vector3Field(displayableField.Name, displayableField.Vector3Value);
                    if (v3Value != displayableField.Vector3Value)
                    {
                        displayableField.Vector3Value = v3Value;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Vector4:
                    Vector4 v4Value = Vector4Field(displayableField.Name, displayableField.Vector4Value);
                    if (v4Value != displayableField.Vector4Value)
                    {
                        displayableField.Vector4Value = v4Value;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Rect:
                    Rect rectValue = RectField(displayableField.Name, displayableField.RectValue);
                    if (rectValue != displayableField.RectValue)
                    {
                        displayableField.RectValue = rectValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Bounds:
                    Bounds boundsValue = BoundsField(displayableField.Name, displayableField.BoundsValue);
                    if (boundsValue != displayableField.BoundsValue)
                    {
                        displayableField.BoundsValue = boundsValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Quaternion:
                    QuaternionFieldReadOnly(displayableField.Name, displayableField.QuaternionValue);
                    break;
                case DisplayableField.DisplayableFieldType.Vector2Int:
                    Vector2Int v2iValue = Vector2IntField(displayableField.Name, displayableField.Vector2IntValue);
                    if (v2iValue != displayableField.Vector2IntValue)
                    {
                        displayableField.Vector2IntValue = v2iValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.Vector3Int:
                    Vector3Int v3iValue = Vector3IntField(displayableField.Name, displayableField.Vector3IntValue);
                    if (v3iValue != displayableField.Vector3IntValue)
                    {
                        displayableField.Vector3IntValue = v3iValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableField.DisplayableFieldType.ObjectReference:
                    ObjectFieldReadOnly(displayableField.Name, displayableField.ObjectValue);
                    break;
                case DisplayableField.DisplayableFieldType.Other:
                    StringFieldReadOnly(displayableField.Name, displayableField.OtherValue);
                    break;
            }
        }

        /// <summary>
        /// 可显示字段
        /// </summary>
        public class DisplayableField : IReference
        {
            public Component Instance;
            public FieldInfo Target;
            public string Name;
            public DisplayableFieldType FieldType;
            public int ArrayLength;
            public int IntegerValue;
            public bool BooleanValue;
            public float FloatValue;
            public string StringValue;
            public Color ColorValue;
            public Enum EnumValue;
            public Vector2 Vector2Value;
            public Vector3 Vector3Value;
            public Vector4 Vector4Value;
            public Rect RectValue;
            public Bounds BoundsValue;
            public Quaternion QuaternionValue;
            public Vector2Int Vector2IntValue;
            public Vector3Int Vector3IntValue;
            public RectInt RectIntValue;
            public BoundsInt BoundsIntValue;
            public UnityEngine.Object ObjectValue;
            public string OtherValue;

            /// <summary>
            /// 设置字段内容
            /// </summary>
            public void SetFieldInfo(FieldInfo fieldInfo, Component instance)
            {
                Instance = instance;
                Target = fieldInfo;
                Name = Target.Name;
                object value = Target.GetValue(Instance);
                if (Target.FieldType.IsArray)
                {
                    FieldType = DisplayableFieldType.Array;
                    Array array = value as Array;
                    ArrayLength = array != null ? array.Length : 0;
                }
                else if (Target.FieldType == typeof(int))
                {
                    FieldType = DisplayableFieldType.Integer;
                    IntegerValue = (int)value;
                }
                else if (Target.FieldType == typeof(bool))
                {
                    FieldType = DisplayableFieldType.Boolean;
                    BooleanValue = (bool)value;
                }
                else if (Target.FieldType == typeof(float))
                {
                    FieldType = DisplayableFieldType.Float;
                    FloatValue = (float)value;
                }
                else if (Target.FieldType == typeof(string))
                {
                    FieldType = DisplayableFieldType.String;
                    StringValue = (string)value;
                }
                else if (Target.FieldType == typeof(Color))
                {
                    FieldType = DisplayableFieldType.Color;
                    ColorValue = (Color)value;
                }
                else if (Target.FieldType.IsEnum)
                {
                    FieldType = DisplayableFieldType.Enum;
                    EnumValue = (Enum)value;
                }
                else if (Target.FieldType == typeof(Vector2))
                {
                    FieldType = DisplayableFieldType.Vector2;
                    Vector2Value = (Vector2)value;
                }
                else if (Target.FieldType == typeof(Vector3))
                {
                    FieldType = DisplayableFieldType.Vector3;
                    Vector3Value = (Vector3)value;
                }
                else if (Target.FieldType == typeof(Vector4))
                {
                    FieldType = DisplayableFieldType.Vector4;
                    Vector4Value = (Vector4)value;
                }
                else if (Target.FieldType == typeof(Rect))
                {
                    FieldType = DisplayableFieldType.Rect;
                    RectValue = (Rect)value;
                }
                else if (Target.FieldType == typeof(Bounds))
                {
                    FieldType = DisplayableFieldType.Bounds;
                    BoundsValue = (Bounds)value;
                }
                else if (Target.FieldType == typeof(Quaternion))
                {
                    FieldType = DisplayableFieldType.Quaternion;
                    QuaternionValue = (Quaternion)value;
                }
                else if (Target.FieldType == typeof(Vector2Int))
                {
                    FieldType = DisplayableFieldType.Vector2Int;
                    Vector2IntValue = (Vector2Int)value;
                }
                else if (Target.FieldType == typeof(Vector3Int))
                {
                    FieldType = DisplayableFieldType.Vector3Int;
                    Vector3IntValue = (Vector3Int)value;
                }
                else if (Target.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    FieldType = DisplayableFieldType.ObjectReference;
                    ObjectValue = (UnityEngine.Object)value;
                }
                else
                {
                    FieldType = DisplayableFieldType.Other;
                    OtherValue = value.ToString();
                }
            }
            /// <summary>
            /// 保存字段值
            /// </summary>
            public void SaveFieldValue()
            {
                switch (FieldType)
                {
                    case DisplayableFieldType.Integer:
                        Target.SetValue(Instance, IntegerValue);
                        break;
                    case DisplayableFieldType.Boolean:
                        Target.SetValue(Instance, BooleanValue);
                        break;
                    case DisplayableFieldType.Float:
                        Target.SetValue(Instance, FloatValue);
                        break;
                    case DisplayableFieldType.String:
                        Target.SetValue(Instance, StringValue);
                        break;
                    case DisplayableFieldType.Color:
                        Target.SetValue(Instance, ColorValue);
                        break;
                    case DisplayableFieldType.Enum:
                        Target.SetValue(Instance, EnumValue);
                        break;
                    case DisplayableFieldType.Vector2:
                        Target.SetValue(Instance, Vector2Value);
                        break;
                    case DisplayableFieldType.Vector3:
                        Target.SetValue(Instance, Vector3Value);
                        break;
                    case DisplayableFieldType.Vector4:
                        Target.SetValue(Instance, Vector4Value);
                        break;
                    case DisplayableFieldType.Rect:
                        Target.SetValue(Instance, RectValue);
                        break;
                    case DisplayableFieldType.Bounds:
                        Target.SetValue(Instance, BoundsValue);
                        break;
                    case DisplayableFieldType.Vector2Int:
                        Target.SetValue(Instance, Vector2IntValue);
                        break;
                    case DisplayableFieldType.Vector3Int:
                        Target.SetValue(Instance, Vector3IntValue);
                        break;
                }
            }
            public void Reset()
            {
                Instance = null;
                Target = null;
                ObjectValue = null;
            }

            public enum DisplayableFieldType
            {
                /// <summary>
                /// 数组
                /// </summary>
                Array,
                /// <summary>
                /// 整型
                /// </summary>
                Integer,
                /// <summary>
                /// 布尔型
                /// </summary>
                Boolean,
                /// <summary>
                /// 浮点型
                /// </summary>
                Float,
                /// <summary>
                /// 字符串
                /// </summary>
                String,
                /// <summary>
                /// 颜色
                /// </summary>
                Color,
                /// <summary>
                /// 枚举
                /// </summary>
                Enum,
                /// <summary>
                /// 二维向量
                /// </summary>
                Vector2,
                /// <summary>
                /// 三维向量
                /// </summary>
                Vector3,
                /// <summary>
                /// 四维向量
                /// </summary>
                Vector4,
                /// <summary>
                /// 矩形区域
                /// </summary>
                Rect,
                /// <summary>
                /// 包围盒
                /// </summary>
                Bounds,
                /// <summary>
                /// 四元素
                /// </summary>
                Quaternion,
                /// <summary>
                /// 二维向量（整型）
                /// </summary>
                Vector2Int,
                /// <summary>
                /// 三维向量（整型）
                /// </summary>
                Vector3Int,
                /// <summary>
                /// 继承至 UnityEngine.Object 的对象
                /// </summary>
                ObjectReference,
                /// <summary>
                /// 其他
                /// </summary>
                Other
            }
        }
    }
}