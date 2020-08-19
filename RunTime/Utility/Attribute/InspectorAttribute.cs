using System;

namespace HT.Framework
{
    /// <summary>
    /// 类成员检视器特性
    /// </summary>
    public abstract class InspectorAttribute : Attribute
    {
        
    }

    /// <summary>
    /// 按钮检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ButtonAttribute : InspectorAttribute
    {
        public string Text { get; private set; }
        public EnableMode Mode { get; private set; }
        public string Style { get; private set; }
        public int Order { get; private set; }

        public ButtonAttribute(string text = null, EnableMode mode = EnableMode.Always, string style = "Button", int order = 0)
        {
            Text = text;
            Mode = mode;
            Style = style;
            Order = order;
        }

        /// <summary>
        /// 按钮激活模式
        /// </summary>
        public enum EnableMode
        {
            /// <summary>
            /// 总是激活
            /// </summary>
            Always,
            /// <summary>
            /// 只在编辑模式激活
            /// </summary>
            Editor,
            /// <summary>
            /// 只在运行模式激活
            /// </summary>
            Playmode
        }
    }

    /// <summary>
    /// 下拉框检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DropdownAttribute : InspectorAttribute
    {
#if UNITY_EDITOR
        public Type ValueType { get; private set; }
        public object[] Values { get; private set; }
        public string[] DisplayOptions { get; private set; }
#endif

        public DropdownAttribute(params string[] values)
        {
#if UNITY_EDITOR
            ValueType = typeof(string);
            Values = values;
            DisplayOptions = values;
#endif
        }
        public DropdownAttribute(params int[] values)
        {
#if UNITY_EDITOR
            ValueType = typeof(int);
            Values = new object[values.Length];
            DisplayOptions = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                Values[i] = values[i];
                DisplayOptions[i] = values[i].ToString();
            }
#endif
        }
        public DropdownAttribute(params float[] values)
        {
#if UNITY_EDITOR
            ValueType = typeof(float);
            Values = new object[values.Length];
            DisplayOptions = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                Values[i] = values[i];
                DisplayOptions[i] = values[i].ToString();
            }
#endif
        }
    }

    /// <summary>
    /// 可排序列表检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ReorderableListAttribute : InspectorAttribute
    {

    }

    /// <summary>
    /// 激活状态检视器 - 参数condition为激活条件判断方法的名称，返回值必须为bool
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class EnableAttribute : InspectorAttribute
    {
        public string Condition { get; private set; }

        public EnableAttribute(string condition)
        {
            Condition = condition;
        }
    }

    /// <summary>
    /// 显示状态检视器 - 参数condition为显示条件判断方法的名称，返回值必须为bool
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class DisplayAttribute : InspectorAttribute
    {
        public string Condition { get; private set; }

        public DisplayAttribute(string condition)
        {
            Condition = condition;
        }
    }

    /// <summary>
    /// 标签检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class LabelAttribute : InspectorAttribute
    {
        public string Name { get; private set; }

        public LabelAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 颜色检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ColorAttribute : InspectorAttribute
    {
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }
        public float A { get; private set; }

        public ColorAttribute(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    /// <summary>
    /// 只读检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ReadOnlyAttribute : InspectorAttribute
    {
        
    }
}