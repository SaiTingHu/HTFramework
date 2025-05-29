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
        private static HashSet<string> ExcludedProperty = new HashSet<string>() {
            "IsSupportedDataDriver",
            "IsAutomate",
            "destroyCancellationToken",
            "useGUILayout",
            "runInEditMode",
            "allowPrefabModeInPlayMode",
            "enabled",
            "isActiveAndEnabled",
            "transform",
            "gameObject",
            "tag",
            "name",
            "hideFlags"};
        private Behaviour _behaviour;
        private List<DisplayableField> _displayableFields = new List<DisplayableField>();
        private List<DisplayableProperty> _displayableProperties = new List<DisplayableProperty>();

        public override void OnEnable()
        {
            _behaviour = Target as Behaviour;
            FieldInfo[] fields = Target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                if (IsBackingField(fields[i]))
                    continue;

                DisplayableField field = Main.m_ReferencePool.Spawn<DisplayableField>();
                field.SetFieldInfo(fields[i], Target);
                _displayableFields.Add(field);
            }
            PropertyInfo[] properties = Target.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < properties.Length; i++)
            {
                if (!properties[i].CanRead || ExcludedProperty.Contains(properties[i].Name) || properties[i].IsDefined(typeof(ObsoleteAttribute), true))
                    continue;

                DisplayableProperty property = Main.m_ReferencePool.Spawn<DisplayableProperty>();
                property.SetPropertyInfo(properties[i], Target);
                _displayableProperties.Add(property);
            }
        }
        public override void OnDebuggerGUI()
        {
            if (_behaviour != null)
            {
                _behaviour.enabled = BoolField("Enabled", _behaviour.enabled);
            }

            if (_displayableFields.Count == 0 && _displayableProperties.Count == 0)
            {
                GUILayout.Label("No any displayable Field or Property!");
            }
            else
            {
                for (int i = 0; i < _displayableFields.Count; i++)
                {
                    DrawDisplayableField(_displayableFields[i]);
                }
                for (int i = 0; i < _displayableProperties.Count; i++)
                {
                    DrawDisplayableProperty(_displayableProperties[i]);
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
            for (int i = 0; i < _displayableProperties.Count; i++)
            {
                Main.m_ReferencePool.Despawn(_displayableProperties[i]);
            }
            _displayableProperties.Clear();
        }

        /// <summary>
        /// 绘制一个可显示字段
        /// </summary>
        private void DrawDisplayableField(DisplayableField displayableField)
        {
            switch (displayableField.FieldType)
            {
                case DisplayableType.Array:
                    IntFieldReadOnly(displayableField.Name + "(Length)", displayableField.ArrayLength);
                    break;
                case DisplayableType.Integer:
                    int intValue = IntField(displayableField.Name, displayableField.IntegerValue);
                    if (intValue != displayableField.IntegerValue)
                    {
                        displayableField.IntegerValue = intValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Boolean:
                    bool boolValue = BoolField(displayableField.Name, displayableField.BooleanValue);
                    if (boolValue != displayableField.BooleanValue)
                    {
                        displayableField.BooleanValue = boolValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Float:
                    float floatValue = FloatField(displayableField.Name, displayableField.FloatValue);
                    if (floatValue != displayableField.FloatValue)
                    {
                        displayableField.FloatValue = floatValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.String:
                    string stringValue = StringField(displayableField.Name, displayableField.StringValue);
                    if (stringValue != displayableField.StringValue)
                    {
                        displayableField.StringValue = stringValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Color:
                    Color colorValue = ColorField(displayableField.Name, displayableField.ColorValue);
                    if (colorValue != displayableField.ColorValue)
                    {
                        displayableField.ColorValue = colorValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Enum:
                    Enum enumValue = EnumField(displayableField.Name, displayableField.EnumValue);
                    if (enumValue != displayableField.EnumValue)
                    {
                        displayableField.EnumValue = enumValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Vector2:
                    Vector2 v2Value = Vector2Field(displayableField.Name, displayableField.Vector2Value);
                    if (v2Value != displayableField.Vector2Value)
                    {
                        displayableField.Vector2Value = v2Value;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Vector3:
                    Vector3 v3Value = Vector3Field(displayableField.Name, displayableField.Vector3Value);
                    if (v3Value != displayableField.Vector3Value)
                    {
                        displayableField.Vector3Value = v3Value;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Vector4:
                    Vector4 v4Value = Vector4Field(displayableField.Name, displayableField.Vector4Value);
                    if (v4Value != displayableField.Vector4Value)
                    {
                        displayableField.Vector4Value = v4Value;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Rect:
                    Rect rectValue = RectField(displayableField.Name, displayableField.RectValue);
                    if (rectValue != displayableField.RectValue)
                    {
                        displayableField.RectValue = rectValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Bounds:
                    Bounds boundsValue = BoundsField(displayableField.Name, displayableField.BoundsValue);
                    if (boundsValue != displayableField.BoundsValue)
                    {
                        displayableField.BoundsValue = boundsValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Quaternion:
                    QuaternionFieldReadOnly(displayableField.Name, displayableField.QuaternionValue);
                    break;
                case DisplayableType.Vector2Int:
                    Vector2Int v2iValue = Vector2IntField(displayableField.Name, displayableField.Vector2IntValue);
                    if (v2iValue != displayableField.Vector2IntValue)
                    {
                        displayableField.Vector2IntValue = v2iValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.Vector3Int:
                    Vector3Int v3iValue = Vector3IntField(displayableField.Name, displayableField.Vector3IntValue);
                    if (v3iValue != displayableField.Vector3IntValue)
                    {
                        displayableField.Vector3IntValue = v3iValue;
                        displayableField.SaveFieldValue();
                    }
                    break;
                case DisplayableType.ObjectReference:
                    ObjectFieldReadOnly(displayableField.Name, displayableField.ObjectValue);
                    break;
                case DisplayableType.Other:
                    StringFieldReadOnly(displayableField.Name, displayableField.OtherValue);
                    break;
            }
        }
        /// <summary>
        /// 绘制一个可显示属性
        /// </summary>
        private void DrawDisplayableProperty(DisplayableProperty displayableProperty)
        {
            GUI.enabled = displayableProperty.IsCanWrite;
            switch (displayableProperty.PropertyType)
            {
                case DisplayableType.Array:
                    IntFieldReadOnly(displayableProperty.Name + "(Length)", displayableProperty.ArrayLength);
                    break;
                case DisplayableType.Integer:
                    int intValue = IntField(displayableProperty.Name, displayableProperty.IntegerValue);
                    if (intValue != displayableProperty.IntegerValue)
                    {
                        displayableProperty.IntegerValue = intValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Boolean:
                    bool boolValue = BoolField(displayableProperty.Name, displayableProperty.BooleanValue);
                    if (boolValue != displayableProperty.BooleanValue)
                    {
                        displayableProperty.BooleanValue = boolValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Float:
                    float floatValue = FloatField(displayableProperty.Name, displayableProperty.FloatValue);
                    if (floatValue != displayableProperty.FloatValue)
                    {
                        displayableProperty.FloatValue = floatValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.String:
                    string stringValue = StringField(displayableProperty.Name, displayableProperty.StringValue);
                    if (stringValue != displayableProperty.StringValue)
                    {
                        displayableProperty.StringValue = stringValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Color:
                    Color colorValue = ColorField(displayableProperty.Name, displayableProperty.ColorValue);
                    if (colorValue != displayableProperty.ColorValue)
                    {
                        displayableProperty.ColorValue = colorValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Enum:
                    Enum enumValue = EnumField(displayableProperty.Name, displayableProperty.EnumValue);
                    if (enumValue != displayableProperty.EnumValue)
                    {
                        displayableProperty.EnumValue = enumValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Vector2:
                    Vector2 v2Value = Vector2Field(displayableProperty.Name, displayableProperty.Vector2Value);
                    if (v2Value != displayableProperty.Vector2Value)
                    {
                        displayableProperty.Vector2Value = v2Value;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Vector3:
                    Vector3 v3Value = Vector3Field(displayableProperty.Name, displayableProperty.Vector3Value);
                    if (v3Value != displayableProperty.Vector3Value)
                    {
                        displayableProperty.Vector3Value = v3Value;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Vector4:
                    Vector4 v4Value = Vector4Field(displayableProperty.Name, displayableProperty.Vector4Value);
                    if (v4Value != displayableProperty.Vector4Value)
                    {
                        displayableProperty.Vector4Value = v4Value;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Rect:
                    Rect rectValue = RectField(displayableProperty.Name, displayableProperty.RectValue);
                    if (rectValue != displayableProperty.RectValue)
                    {
                        displayableProperty.RectValue = rectValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Bounds:
                    Bounds boundsValue = BoundsField(displayableProperty.Name, displayableProperty.BoundsValue);
                    if (boundsValue != displayableProperty.BoundsValue)
                    {
                        displayableProperty.BoundsValue = boundsValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Quaternion:
                    QuaternionFieldReadOnly(displayableProperty.Name, displayableProperty.QuaternionValue);
                    break;
                case DisplayableType.Vector2Int:
                    Vector2Int v2iValue = Vector2IntField(displayableProperty.Name, displayableProperty.Vector2IntValue);
                    if (v2iValue != displayableProperty.Vector2IntValue)
                    {
                        displayableProperty.Vector2IntValue = v2iValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.Vector3Int:
                    Vector3Int v3iValue = Vector3IntField(displayableProperty.Name, displayableProperty.Vector3IntValue);
                    if (v3iValue != displayableProperty.Vector3IntValue)
                    {
                        displayableProperty.Vector3IntValue = v3iValue;
                        displayableProperty.SavePropertyValue();
                    }
                    break;
                case DisplayableType.ObjectReference:
                    ObjectFieldReadOnly(displayableProperty.Name, displayableProperty.ObjectValue);
                    break;
                case DisplayableType.Other:
                    StringFieldReadOnly(displayableProperty.Name, displayableProperty.OtherValue);
                    break;
            }
            GUI.enabled = true;
        }
        /// <summary>
        /// 是否为自动属性的后端字段
        /// </summary>
        private bool IsBackingField(FieldInfo fieldInfo)
        {
            return fieldInfo.Name.StartsWith("<") && fieldInfo.Name.EndsWith("k__BackingField");
        }

        /// <summary>
        /// 可显示字段
        /// </summary>
        public class DisplayableField : IReference
        {
            public Component Instance;
            public FieldInfo Target;
            public string Name;
            public DisplayableType FieldType;
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
                    FieldType = DisplayableType.Array;
                    Array array = value as Array;
                    ArrayLength = array != null ? array.Length : 0;
                }
                else if (Target.FieldType == typeof(int))
                {
                    FieldType = DisplayableType.Integer;
                    IntegerValue = (int)value;
                }
                else if (Target.FieldType == typeof(bool))
                {
                    FieldType = DisplayableType.Boolean;
                    BooleanValue = (bool)value;
                }
                else if (Target.FieldType == typeof(float))
                {
                    FieldType = DisplayableType.Float;
                    FloatValue = (float)value;
                }
                else if (Target.FieldType == typeof(string))
                {
                    FieldType = DisplayableType.String;
                    StringValue = (string)value;
                }
                else if (Target.FieldType == typeof(Color))
                {
                    FieldType = DisplayableType.Color;
                    ColorValue = (Color)value;
                }
                else if (Target.FieldType.IsEnum)
                {
                    FieldType = DisplayableType.Enum;
                    EnumValue = (Enum)value;
                }
                else if (Target.FieldType == typeof(Vector2))
                {
                    FieldType = DisplayableType.Vector2;
                    Vector2Value = (Vector2)value;
                }
                else if (Target.FieldType == typeof(Vector3))
                {
                    FieldType = DisplayableType.Vector3;
                    Vector3Value = (Vector3)value;
                }
                else if (Target.FieldType == typeof(Vector4))
                {
                    FieldType = DisplayableType.Vector4;
                    Vector4Value = (Vector4)value;
                }
                else if (Target.FieldType == typeof(Rect))
                {
                    FieldType = DisplayableType.Rect;
                    RectValue = (Rect)value;
                }
                else if (Target.FieldType == typeof(Bounds))
                {
                    FieldType = DisplayableType.Bounds;
                    BoundsValue = (Bounds)value;
                }
                else if (Target.FieldType == typeof(Quaternion))
                {
                    FieldType = DisplayableType.Quaternion;
                    QuaternionValue = (Quaternion)value;
                }
                else if (Target.FieldType == typeof(Vector2Int))
                {
                    FieldType = DisplayableType.Vector2Int;
                    Vector2IntValue = (Vector2Int)value;
                }
                else if (Target.FieldType == typeof(Vector3Int))
                {
                    FieldType = DisplayableType.Vector3Int;
                    Vector3IntValue = (Vector3Int)value;
                }
                else if (Target.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    FieldType = DisplayableType.ObjectReference;
                    ObjectValue = (UnityEngine.Object)value;
                }
                else
                {
                    FieldType = DisplayableType.Other;
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
                    case DisplayableType.Integer:
                        Target.SetValue(Instance, IntegerValue);
                        break;
                    case DisplayableType.Boolean:
                        Target.SetValue(Instance, BooleanValue);
                        break;
                    case DisplayableType.Float:
                        Target.SetValue(Instance, FloatValue);
                        break;
                    case DisplayableType.String:
                        Target.SetValue(Instance, StringValue);
                        break;
                    case DisplayableType.Color:
                        Target.SetValue(Instance, ColorValue);
                        break;
                    case DisplayableType.Enum:
                        Target.SetValue(Instance, EnumValue);
                        break;
                    case DisplayableType.Vector2:
                        Target.SetValue(Instance, Vector2Value);
                        break;
                    case DisplayableType.Vector3:
                        Target.SetValue(Instance, Vector3Value);
                        break;
                    case DisplayableType.Vector4:
                        Target.SetValue(Instance, Vector4Value);
                        break;
                    case DisplayableType.Rect:
                        Target.SetValue(Instance, RectValue);
                        break;
                    case DisplayableType.Bounds:
                        Target.SetValue(Instance, BoundsValue);
                        break;
                    case DisplayableType.Vector2Int:
                        Target.SetValue(Instance, Vector2IntValue);
                        break;
                    case DisplayableType.Vector3Int:
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
        }
        /// <summary>
        /// 可显示属性
        /// </summary>
        public class DisplayableProperty : IReference
        {
            public Component Instance;
            public PropertyInfo Target;
            public bool IsCanWrite;
            public string Name;
            public DisplayableType PropertyType;
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
            public UnityEngine.Object ObjectValue;
            public string OtherValue;

            /// <summary>
            /// 设置属性内容
            /// </summary>
            public void SetPropertyInfo(PropertyInfo propertyInfo, Component instance)
            {
                Instance = instance;
                Target = propertyInfo;
                IsCanWrite = Target.CanWrite;
                Name = Target.Name;
                object value = Target.GetValue(Instance);
                if (Target.PropertyType.IsArray)
                {
                    PropertyType = DisplayableType.Array;
                    Array array = value as Array;
                    ArrayLength = array != null ? array.Length : 0;
                }
                else if (Target.PropertyType == typeof(int))
                {
                    PropertyType = DisplayableType.Integer;
                    IntegerValue = (int)value;
                }
                else if (Target.PropertyType == typeof(bool))
                {
                    PropertyType = DisplayableType.Boolean;
                    BooleanValue = (bool)value;
                }
                else if (Target.PropertyType == typeof(float))
                {
                    PropertyType = DisplayableType.Float;
                    FloatValue = (float)value;
                }
                else if (Target.PropertyType == typeof(string))
                {
                    PropertyType = DisplayableType.String;
                    StringValue = (string)value;
                }
                else if (Target.PropertyType == typeof(Color))
                {
                    PropertyType = DisplayableType.Color;
                    ColorValue = (Color)value;
                }
                else if (Target.PropertyType.IsEnum)
                {
                    PropertyType = DisplayableType.Enum;
                    EnumValue = (Enum)value;
                }
                else if (Target.PropertyType == typeof(Vector2))
                {
                    PropertyType = DisplayableType.Vector2;
                    Vector2Value = (Vector2)value;
                }
                else if (Target.PropertyType == typeof(Vector3))
                {
                    PropertyType = DisplayableType.Vector3;
                    Vector3Value = (Vector3)value;
                }
                else if (Target.PropertyType == typeof(Vector4))
                {
                    PropertyType = DisplayableType.Vector4;
                    Vector4Value = (Vector4)value;
                }
                else if (Target.PropertyType == typeof(Rect))
                {
                    PropertyType = DisplayableType.Rect;
                    RectValue = (Rect)value;
                }
                else if (Target.PropertyType == typeof(Bounds))
                {
                    PropertyType = DisplayableType.Bounds;
                    BoundsValue = (Bounds)value;
                }
                else if (Target.PropertyType == typeof(Quaternion))
                {
                    PropertyType = DisplayableType.Quaternion;
                    QuaternionValue = (Quaternion)value;
                }
                else if (Target.PropertyType == typeof(Vector2Int))
                {
                    PropertyType = DisplayableType.Vector2Int;
                    Vector2IntValue = (Vector2Int)value;
                }
                else if (Target.PropertyType == typeof(Vector3Int))
                {
                    PropertyType = DisplayableType.Vector3Int;
                    Vector3IntValue = (Vector3Int)value;
                }
                else if (Target.PropertyType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    PropertyType = DisplayableType.ObjectReference;
                    ObjectValue = (UnityEngine.Object)value;
                }
                else
                {
                    PropertyType = DisplayableType.Other;
                    OtherValue = value.ToString();
                }
            }
            /// <summary>
            /// 保存属性值
            /// </summary>
            public void SavePropertyValue()
            {
                if (!IsCanWrite)
                    return;

                switch (PropertyType)
                {
                    case DisplayableType.Integer:
                        Target.SetValue(Instance, IntegerValue);
                        break;
                    case DisplayableType.Boolean:
                        Target.SetValue(Instance, BooleanValue);
                        break;
                    case DisplayableType.Float:
                        Target.SetValue(Instance, FloatValue);
                        break;
                    case DisplayableType.String:
                        Target.SetValue(Instance, StringValue);
                        break;
                    case DisplayableType.Color:
                        Target.SetValue(Instance, ColorValue);
                        break;
                    case DisplayableType.Enum:
                        Target.SetValue(Instance, EnumValue);
                        break;
                    case DisplayableType.Vector2:
                        Target.SetValue(Instance, Vector2Value);
                        break;
                    case DisplayableType.Vector3:
                        Target.SetValue(Instance, Vector3Value);
                        break;
                    case DisplayableType.Vector4:
                        Target.SetValue(Instance, Vector4Value);
                        break;
                    case DisplayableType.Rect:
                        Target.SetValue(Instance, RectValue);
                        break;
                    case DisplayableType.Bounds:
                        Target.SetValue(Instance, BoundsValue);
                        break;
                    case DisplayableType.Vector2Int:
                        Target.SetValue(Instance, Vector2IntValue);
                        break;
                    case DisplayableType.Vector3Int:
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
        }
        /// <summary>
        /// 可显示字段或属性的类型
        /// </summary>
        public enum DisplayableType
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