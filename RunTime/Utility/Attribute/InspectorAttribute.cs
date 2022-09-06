using System;
using System.Diagnostics;

namespace HT.Framework
{
    /// <summary>
    /// 类成员检视器特性
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public abstract class InspectorAttribute : Attribute
    {
        
    }
    
    /// <summary>
    /// 下拉框检视器（支持 string、int、float 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class DropdownAttribute : InspectorAttribute
    {
#if UNITY_EDITOR
        public Type ValueType { get; private set; }
        public object[] Values { get; private set; }
        public string[] DisplayOptions { get; private set; }
#endif
        /// <summary>
        /// 下拉框检视器（string、int、float）
        /// </summary>
        /// <param name="values">下拉框可选值</param>
        public DropdownAttribute(params string[] values)
        {
#if UNITY_EDITOR
            ValueType = typeof(string);
            Values = values;
            DisplayOptions = values;
#endif
        }
        /// <summary>
        /// 下拉框检视器（string、int、float）
        /// </summary>
        /// <param name="values">下拉框可选值</param>
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
        /// <summary>
        /// 下拉框检视器（string、int、float）
        /// </summary>
        /// <param name="values">下拉框可选值</param>
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
    /// 层级检视器（支持 string 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class LayerAttribute : InspectorAttribute
    {

    }

    /// <summary>
    /// 可排序列表检视器（支持数组、List 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ReorderableListAttribute : InspectorAttribute
    {

    }

    /// <summary>
    /// 密码检视器（支持 string 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class PasswordAttribute : InspectorAttribute
    {

    }

    /// <summary>
    /// 超链接检视器（支持 string 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class HyperlinkAttribute : InspectorAttribute
    {
        public string Name { get; private set; }

        /// <summary>
        /// 超链接检视器（支持 string 类型）
        /// </summary>
        /// <param name="name">显示名称</param>
        public HyperlinkAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 文件路径检视器（支持 string 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class FilePathAttribute : InspectorAttribute
    {
        public string Extension { get; private set; }

        /// <summary>
        /// 文件路径检视器（支持 string 类型）
        /// </summary>
        /// <param name="extension">文件扩展名</param>
        public FilePathAttribute(string extension = "*.*")
        {
            Extension = extension;
        }
    }

    /// <summary>
    /// 文件夹路径检视器（支持 string 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class FolderPathAttribute : InspectorAttribute
    {
        
    }

    /// <summary>
    /// 激活状态检视器 - 参数condition为激活条件判断方法的名称，返回值必须为bool
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class EnableAttribute : InspectorAttribute
    {
        public string Condition { get; private set; }

        /// <summary>
        /// 激活状态检视器
        /// </summary>
        /// <param name="condition">激活条件判断方法的名称，返回值必须为bool</param>
        public EnableAttribute(string condition)
        {
            Condition = condition;
        }
    }

    /// <summary>
    /// 显示状态检视器 - 参数condition为显示条件判断方法的名称，返回值必须为bool
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class DisplayAttribute : InspectorAttribute
    {
        public string Condition { get; private set; }

        /// <summary>
        /// 显示状态检视器
        /// </summary>
        /// <param name="condition">显示条件判断方法的名称，返回值必须为bool</param>
        public DisplayAttribute(string condition)
        {
            Condition = condition;
        }
    }

    /// <summary>
    /// 标签检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class LabelAttribute : InspectorAttribute
    {
        public string Name { get; private set; }

        /// <summary>
        /// 标签检视器
        /// </summary>
        /// <param name="name">标签</param>
        public LabelAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 颜色检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ColorAttribute : InspectorAttribute
    {
        public float R { get; private set; }
        public float G { get; private set; }
        public float B { get; private set; }
        public float A { get; private set; }

        /// <summary>
        /// 颜色检视器
        /// </summary>
        /// <param name="r">颜色r值</param>
        /// <param name="g">颜色g值</param>
        /// <param name="b">颜色b值</param>
        /// <param name="a">颜色a值</param>
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
    [Conditional("UNITY_EDITOR")]
    public sealed class ReadOnlyAttribute : InspectorAttribute
    {
        
    }

    /// <summary>
    /// 预览检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class PreviewAttribute : InspectorAttribute
    {
        public float Size { get; private set; }

        /// <summary>
        /// 预览检视器
        /// </summary>
        /// <param name="size">预览框的大小</param>
        public PreviewAttribute(float size = 100)
        {
            Size = size;
        }
    }

    /// <summary>
    /// 通用菜单检视器（支持 string 类型）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class GenericMenuAttribute : InspectorAttribute
    {
        public string GenerateMenu { get; private set; }
        public string ChooseMenu { get; private set; }

        /// <summary>
        /// 通用菜单检视器（支持 string 类型）
        /// </summary>
        /// <param name="generateMenu">生成菜单的所有选项的方法名称，返回值必须为string[]</param>
        /// <param name="chooseMenu">选择菜单选项后调用的方法名称，必须带有一个string参数</param>
        public GenericMenuAttribute(string generateMenu, string chooseMenu = null)
        {
            GenerateMenu = generateMenu;
            ChooseMenu = chooseMenu;
        }
    }

    /// <summary>
    /// 通用表格绘制器（支持自定义复杂类型的数组、集合）
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class GenericTableAttribute : InspectorAttribute
    {

    }

    /// <summary>
    /// 抽屉检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class DrawerAttribute : InspectorAttribute
    {
        public string Name { get; private set; }
        public string Condition { get; private set; }
        public string Style { get; private set; }
        public bool DefaultOpened { get; private set; }
        public bool ToggleOnLabelClick { get; private set; }

        /// <summary>
        /// 抽屉检视器
        /// </summary>
        /// <param name="name">显示名称</param>
        /// <param name="defaultOpened">默认是否打开</param>
        /// <param name="toggleOnLabelClick">抽屉的标签是否也可点击</param>
        public DrawerAttribute(string name, bool defaultOpened = false, bool toggleOnLabelClick = true)
        {
            Name = name;
            Condition = null;
            Style = null;
            DefaultOpened = defaultOpened;
            ToggleOnLabelClick = toggleOnLabelClick;
        }
        /// <summary>
        /// 抽屉检视器
        /// </summary>
        /// <param name="name">显示名称</param>
        /// <param name="condition">显示条件判断方法的名称，返回值必须为bool</param>
        /// <param name="defaultOpened">默认是否打开</param>
        /// <param name="toggleOnLabelClick">抽屉的标签是否也可点击</param>
        public DrawerAttribute(string name, string condition, bool defaultOpened = false, bool toggleOnLabelClick = true)
        {
            Name = name;
            Condition = condition;
            Style = null;
            DefaultOpened = defaultOpened;
            ToggleOnLabelClick = toggleOnLabelClick;
        }
        /// <summary>
        /// 抽屉检视器
        /// </summary>
        /// <param name="name">显示名称</param>
        /// <param name="condition">显示条件判断方法的名称，返回值必须为bool</param>
        /// <param name="style">GUI样式</param>
        /// <param name="defaultOpened">默认是否打开</param>
        /// <param name="toggleOnLabelClick">抽屉的标签是否也可点击</param>
        public DrawerAttribute(string name, string condition, string style, bool defaultOpened = false, bool toggleOnLabelClick = true)
        {
            Name = name;
            Condition = condition;
            Style = style;
            DefaultOpened = defaultOpened;
            ToggleOnLabelClick = toggleOnLabelClick;
        }
    }

    /// <summary>
    /// 公共属性检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class PropertyDisplayAttribute : InspectorAttribute
    {
        public string Text { get; private set; }
        public bool DisplayOnlyRuntime { get; private set; }

        /// <summary>
        /// 公共属性检视器
        /// </summary>
        /// <param name="text">显示名称</param>
        /// <param name="displayOnlyRuntime">是否仅在编辑器运行时显示</param>
        public PropertyDisplayAttribute(string text = null, bool displayOnlyRuntime = true)
        {
            Text = text;
            DisplayOnlyRuntime = displayOnlyRuntime;
        }
    }

    /// <summary>
    /// 事件、委托检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class EventAttribute : InspectorAttribute
    {
        public string Text { get; private set; }

        /// <summary>
        /// 事件、委托检视器
        /// </summary>
        /// <param name="text">显示名称</param>
        public EventAttribute(string text = null)
        {
            Text = text;
        }
    }

    /// <summary>
    /// 按钮检视器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
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
}