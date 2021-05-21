using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using UObject = UnityEngine.Object;
using UReorderableList = UnityEditorInternal.ReorderableList;

namespace HT.Framework
{
    /// <summary>
    /// 对象检视器
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UObject), true)]
    internal sealed class ObjectInspector : Editor
    {
        private List<FieldInspector> _fields = new List<FieldInspector>();
        private List<PropertyInspector> _properties = new List<PropertyInspector>();
        private List<EventInspector> _events = new List<EventInspector>();
        private List<MethodInspector> _methods = new List<MethodInspector>();

        private void OnEnable()
        {
            try
            {
                using (SerializedProperty iterator = serializedObject.GetIterator())
                {
                    while (iterator.NextVisible(true))
                    {
                        SerializedProperty property = serializedObject.FindProperty(iterator.name);
                        if (property != null)
                        {
                            _fields.Add(new FieldInspector(property));
                        }
                    }
                }

                List<PropertyInfo> properties = target.GetType().GetProperties((property) =>
                {
                    return property.IsDefined(typeof(PropertyDisplayAttribute), true);
                });
                for (int i = 0; i < properties.Count; i++)
                {
                    _properties.Add(new PropertyInspector(properties[i]));
                }

                List<FieldInfo> events = target.GetType().GetFields((field) =>
                {
                    return field.FieldType.IsSubclassOf(typeof(MulticastDelegate)) && field.IsDefined(typeof(EventAttribute), true);
                });
                for (int i = 0; i < events.Count; i++)
                {
                    _events.Add(new EventInspector(events[i]));
                }

                List<MethodInfo> methods = target.GetType().GetMethods((method) =>
                {
                    return method.IsDefined(typeof(ButtonAttribute), true);
                });
                for (int i = 0; i < methods.Count; i++)
                {
                    _methods.Add(new MethodInspector(methods[i]));
                }
                _methods.Sort((a, b) => { return a.Attribute.Order - b.Attribute.Order; });
            }
            catch { }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            FieldGUI();
            PropertyGUI();
            EventGUI();
            MethodGUI();

            serializedObject.ApplyModifiedProperties();
        }
        private void OnSceneGUI()
        {
            FieldSceneHandle();
        }
        /// <summary>
        /// 绘制字段
        /// </summary>
        private void FieldGUI()
        {
            bool drawerValue = true;
            int indentLevel = 0;
            for (int i = 0; i < _fields.Count; i++)
            {
                if (_fields[i].Drawer != null)
                {
                    EditorGUI.indentLevel = 0;
                    indentLevel = 1;

                    if (string.IsNullOrEmpty(_fields[i].Drawer.Style))
                    {
                        GUILayout.BeginHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal(_fields[i].Drawer.Style);
                        GUILayout.Space(10);
                    }
                    _fields[i].DrawerValue = EditorGUILayout.Foldout(_fields[i].DrawerValue, _fields[i].Drawer.Name, _fields[i].Drawer.ToggleOnLabelClick);
                    drawerValue = _fields[i].DrawerValue;
                    GUILayout.EndHorizontal();
                }

                if (drawerValue)
                {
                    EditorGUI.indentLevel = indentLevel;

                    _fields[i].Painting(this);
                }
            }
        }
        /// <summary>
        /// 绘制属性
        /// </summary>
        private void PropertyGUI()
        {
            for (int i = 0; i < _properties.Count; i++)
            {
                _properties[i].Painting(this);
            }
        }
        /// <summary>
        /// 绘制事件
        /// </summary>
        private void EventGUI()
        {
            for (int i = 0; i < _events.Count; i++)
            {
                _events[i].Painting(this);
            }
        }
        /// <summary>
        /// 绘制方法
        /// </summary>
        private void MethodGUI()
        {
            for (int i = 0; i < _methods.Count; i++)
            {
                _methods[i].Painting(this);
            }
        }
        /// <summary>
        /// 场景中处理字段
        /// </summary>
        private void FieldSceneHandle()
        {
            bool drawerValue = true;
            for (int i = 0; i < _fields.Count; i++)
            {
                if (_fields[i].Drawer != null)
                {
                    drawerValue = _fields[i].DrawerValue;
                }

                if (drawerValue)
                {
                    _fields[i].SceneHandle(this);
                }
            }
        }
        /// <summary>
        /// 标记目标已改变
        /// </summary>
        private void HasChanged()
        {
            EditorUtility.SetDirty(target);

            if (EditorApplication.isPlaying)
                return;

            Component component = target as Component;
            if (component != null && component.gameObject.scene != null)
            {
                EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
            }
        }

        #region Field
        /// <summary>
        /// 字段检视器
        /// </summary>
        private sealed class FieldInspector
        {
            public FieldInfo Field;
            public SerializedProperty Property;
            public List<FieldPainter> Painters = new List<FieldPainter>();
            public List<FieldSceneHandler> SceneHandlers = new List<FieldSceneHandler>();
            public MethodInfo EnableCondition;
            public MethodInfo DisplayCondition;
            public string Label;
            public Color UseColor = Color.white;
            public bool IsReadOnly = false;
            public DrawerAttribute Drawer;
            public bool DrawerValue = true;

            public bool IsEnable
            {
                get
                {
                    bool condition = true;
                    if (EnableCondition != null)
                    {
                        if (EnableCondition.IsStatic)
                        {
                            condition = (bool)EnableCondition.Invoke(null, null);
                        }
                        else
                        {
                            condition = (bool)EnableCondition.Invoke(Property.serializedObject.targetObject, null);
                        }
                    }
                    return !IsReadOnly && condition;
                }
            }
            public bool IsDisplay
            {
                get
                {
                    bool condition = true;
                    if (DisplayCondition != null)
                    {
                        if (DisplayCondition.IsStatic)
                        {
                            condition = (bool)DisplayCondition.Invoke(null, null);
                        }
                        else
                        {
                            condition = (bool)DisplayCondition.Invoke(Property.serializedObject.targetObject, null);
                        }
                    }
                    return condition;
                }
            }

            public FieldInspector(SerializedProperty property)
            {
                BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                Field = property.serializedObject.targetObject.GetType().GetField(property.name, flags);
                Property = property;
                Label = property.displayName;

                if (Field != null)
                {
                    InspectorAttribute[] iattributes = (InspectorAttribute[])Field.GetCustomAttributes(typeof(InspectorAttribute), true);
                    for (int i = 0; i < iattributes.Length; i++)
                    {
                        if (iattributes[i] is DropdownAttribute)
                        {
                            Painters.Add(new DropdownPainter(iattributes[i]));
                        }
                        else if (iattributes[i] is LayerAttribute)
                        {
                            Painters.Add(new LayerPainter(iattributes[i]));
                        }
                        else if (iattributes[i] is ReorderableListAttribute)
                        {
                            Painters.Add(new ReorderableListPainter(iattributes[i]));
                        }
                        else if (iattributes[i] is PasswordAttribute)
                        {
                            Painters.Add(new PasswordPainter(iattributes[i]));
                        }
                        else if (iattributes[i] is HyperlinkAttribute)
                        {
                            Painters.Add(new HyperlinkPainter(iattributes[i]));
                        }
                        else if (iattributes[i] is FilePathAttribute)
                        {
                            Painters.Add(new FilePathPainter(iattributes[i]));
                        }
                        else if (iattributes[i] is FolderPathAttribute)
                        {
                            Painters.Add(new FolderPathPainter(iattributes[i]));
                        }
                        else if (iattributes[i] is EnableAttribute)
                        {
                            EnableCondition = property.serializedObject.targetObject.GetType().GetMethod(iattributes[i].Cast<EnableAttribute>().Condition, flags);
                            if (EnableCondition != null && EnableCondition.ReturnType != typeof(bool))
                            {
                                EnableCondition = null;
                            }
                        }
                        else if (iattributes[i] is DisplayAttribute)
                        {
                            DisplayCondition = property.serializedObject.targetObject.GetType().GetMethod(iattributes[i].Cast<DisplayAttribute>().Condition, flags);
                            if (DisplayCondition != null && DisplayCondition.ReturnType != typeof(bool))
                            {
                                DisplayCondition = null;
                            }
                        }
                        else if (iattributes[i] is LabelAttribute)
                        {
                            Label = iattributes[i].Cast<LabelAttribute>().Name;
                        }
                        else if (iattributes[i] is ColorAttribute)
                        {
                            ColorAttribute attribute = iattributes[i] as ColorAttribute;
                            UseColor = new Color(attribute.R, attribute.G, attribute.B, attribute.A);
                        }
                        else if (iattributes[i] is ReadOnlyAttribute)
                        {
                            IsReadOnly = true;
                        }
                        else if (iattributes[i] is GenericMenuAttribute)
                        {
                            Painters.Add(new GenericMenuPainter(iattributes[i]));
                        }
                        else if (iattributes[i] is DrawerAttribute)
                        {
                            Drawer = iattributes[i] as DrawerAttribute;
                            DrawerValue = Drawer.DefaultOpened;
                        }
                    }

                    SceneHandlerAttribute[] sattributes = (SceneHandlerAttribute[])Field.GetCustomAttributes(typeof(SceneHandlerAttribute), true);
                    for (int i = 0; i < sattributes.Length; i++)
                    {
                        if (sattributes[i] is MoveHandlerAttribute)
                        {
                            SceneHandlers.Add(new MoveHandler(sattributes[i]));
                        }
                        else if (sattributes[i] is RadiusHandlerAttribute)
                        {
                            SceneHandlers.Add(new RadiusHandler(sattributes[i]));
                        }
                        else if (sattributes[i] is BoundsHandlerAttribute)
                        {
                            SceneHandlers.Add(new BoundsHandler(sattributes[i]));
                        }
                        else if (sattributes[i] is DirectionHandlerAttribute)
                        {
                            SceneHandlers.Add(new DirectionHandler(sattributes[i]));
                        }
                        else if (sattributes[i] is CircleAreaHandlerAttribute)
                        {
                            SceneHandlers.Add(new CircleAreaHandler(sattributes[i]));
                        }
                    }
                }
            }

            public void Painting(ObjectInspector inspector)
            {
                if (IsDisplay)
                {
                    GUI.color = UseColor;
                    if (Painters.Count > 0)
                    {
                        GUI.enabled = IsEnable;
                        for (int i = 0; i < Painters.Count; i++)
                        {
                            Painters[i].Painting(inspector, this);
                        }
                        GUI.enabled = true;
                    }
                    else
                    {
                        if (Property.name == "m_Script")
                        {
                            GUI.enabled = false;
                            EditorGUILayout.PropertyField(Property);
                            GUI.enabled = true;
                        }
                        else
                        {
                            GUI.enabled = IsEnable;
                            EditorGUILayout.PropertyField(Property, new GUIContent(Label), true);
                            GUI.enabled = true;
                        }
                    }
                    GUI.color = Color.white;
                }
            }

            public void SceneHandle(ObjectInspector inspector)
            {
                if (IsDisplay)
                {
                    if (SceneHandlers.Count > 0)
                    {
                        for (int i = 0; i < SceneHandlers.Count; i++)
                        {
                            SceneHandlers[i].SceneHandle(inspector, this);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 字段绘制器
        /// </summary>
        private abstract class FieldPainter
        {
            public InspectorAttribute IAttribute;

            public FieldPainter(InspectorAttribute attribute)
            {
                IAttribute = attribute;
            }

            public abstract void Painting(ObjectInspector inspector, FieldInspector fieldInspector);
        }
        /// <summary>
        /// 字段绘制器 - 下拉菜单
        /// </summary>
        private sealed class DropdownPainter : FieldPainter
        {
            public DropdownAttribute DAttribute;

            public DropdownPainter(InspectorAttribute attribute) : base(attribute)
            {
                DAttribute = attribute as DropdownAttribute;
            }

            public override void Painting(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (DAttribute.ValueType == fieldInspector.Field.FieldType)
                {
                    object value = fieldInspector.Field.GetValue(inspector.target);
                    int selectIndex = Array.IndexOf(DAttribute.Values, value);
                    if (selectIndex < 0) selectIndex = 0;
                    
                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    int newIndex = EditorGUILayout.Popup(fieldInspector.Label, selectIndex, DAttribute.DisplayOptions);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(inspector.target, "Dropdown");
                        fieldInspector.Field.SetValue(inspector.target, DAttribute.Values[newIndex]);
                        inspector.HasChanged();
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("[" + fieldInspector.Field.Name + "] used a mismatched Dropdown!", MessageType.Error);
                    GUILayout.EndHorizontal();
                }
            }
        }
        /// <summary>
        /// 字段绘制器 - 层级检视
        /// </summary>
        private sealed class LayerPainter : FieldPainter
        {
            public LayerAttribute LAttribute;

            public LayerPainter(InspectorAttribute attribute) : base(attribute)
            {
                LAttribute = attribute as LayerAttribute;
            }

            public override void Painting(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(string))
                {
                    string value = (string)fieldInspector.Field.GetValue(inspector.target);
                    int layer = LayerMask.NameToLayer(value);
                    if (layer < 0) layer = 0;
                    if (layer > 31) layer = 31;

                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    int newLayer = EditorGUILayout.LayerField(fieldInspector.Label, layer);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(inspector.target, "Layer");
                        fieldInspector.Field.SetValue(inspector.target, LayerMask.LayerToName(newLayer));
                        inspector.HasChanged();
                    }
                    GUILayout.EndHorizontal();
                }
                else if (fieldInspector.Field.FieldType == typeof(int))
                {
                    int layer = (int)fieldInspector.Field.GetValue(inspector.target);
                    if (layer < 0) layer = 0;
                    if (layer > 31) layer = 31;

                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    int newLayer = EditorGUILayout.LayerField(fieldInspector.Label, layer);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(inspector.target, "Layer");
                        fieldInspector.Field.SetValue(inspector.target, newLayer);
                        inspector.HasChanged();
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("[" + fieldInspector.Field.Name + "] can't used Layer! because the types don't match!", MessageType.Error);
                    GUILayout.EndHorizontal();
                }
            }
        }
        /// <summary>
        /// 字段绘制器 - 可排序列表
        /// </summary>
        private sealed class ReorderableListPainter : FieldPainter
        {
            public ReorderableListAttribute RAttribute;
            public UReorderableList List;

            public ReorderableListPainter(InspectorAttribute attribute) : base(attribute)
            {
                RAttribute = attribute as ReorderableListAttribute;
            }

            public override void Painting(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Property.isArray)
                {
                    if (List == null)
                    {
                        GenerateList(fieldInspector);
                    }

                    List.DoLayoutList();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("[" + fieldInspector.Field.Name + "] can't use the ReorderableList!", MessageType.Error);
                    GUILayout.EndHorizontal();
                }
            }

            public void GenerateList(FieldInspector fieldInspector)
            {
                List = new UReorderableList(fieldInspector.Property.serializedObject, fieldInspector.Property, true, true, true, true);
                List.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, string.Format("{0}: {1}", fieldInspector.Label, fieldInspector.Property.arraySize), EditorStyles.boldLabel);
                };
                List.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element = fieldInspector.Property.GetArrayElementAtIndex(index);
                    rect.x += 10;
                    rect.y += 2;
                    rect.width -= 10;
                    EditorGUI.PropertyField(rect, element, true);
                };
                List.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "CN EntryBackodd";
                        gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                        rect.x += 2;
                        rect.width -= 6;
                        gUIStyle.Draw(rect, false, isActive, isActive, isFocused);
                    }
                };
                List.elementHeightCallback = (int index) =>
                {
                    return EditorGUI.GetPropertyHeight(fieldInspector.Property.GetArrayElementAtIndex(index)) + 6;
                };
            }
        }
        /// <summary>
        /// 字段绘制器 - 密码
        /// </summary>
        private sealed class PasswordPainter : FieldPainter
        {
            public PasswordAttribute PAttribute;

            public PasswordPainter(InspectorAttribute attribute) : base(attribute)
            {
                PAttribute = attribute as PasswordAttribute;
            }

            public override void Painting(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(string))
                {
                    string value = (string)fieldInspector.Field.GetValue(inspector.target);
                    if (value == null) value = "";

                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    string newValue = EditorGUILayout.PasswordField(fieldInspector.Label, value);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(inspector.target, "Password");
                        fieldInspector.Field.SetValue(inspector.target, newValue);
                        inspector.HasChanged();
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("[" + fieldInspector.Field.Name + "] can't used Password! because the types don't match!", MessageType.Error);
                    GUILayout.EndHorizontal();
                }
            }
        }
        /// <summary>
        /// 字段绘制器 - 超链接
        /// </summary>
        private sealed class HyperlinkPainter : FieldPainter
        {
            public HyperlinkAttribute HAttribute;
            public MethodInfo LinkLabel;
            public object[] Parameter;

            public HyperlinkPainter(InspectorAttribute attribute) : base(attribute)
            {
                HAttribute = attribute as HyperlinkAttribute;
                MethodInfo[] methods = typeof(EditorGUILayout).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
                foreach (var method in methods)
                {
                    if (method.Name == "LinkLabel")
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters != null && parameters.Length > 0 && parameters[0].ParameterType == typeof(string))
                        {
                            LinkLabel = method;
                            break;
                        }
                    }
                }
                Parameter = new object[] { HAttribute.Name, new GUILayoutOption[0] };
            }

            public override void Painting(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(string))
                {
                    GUILayout.BeginHorizontal();
                    bool isClick = (bool)LinkLabel.Invoke(null, Parameter);
                    if (isClick)
                    {
                        Application.OpenURL((string)fieldInspector.Field.GetValue(inspector.target));
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("[" + fieldInspector.Field.Name + "] can't used Hyperlink! because the types don't match!", MessageType.Error);
                    GUILayout.EndHorizontal();
                }
            }
        }
        /// <summary>
        /// 字段绘制器 - 文件路径
        /// </summary>
        private sealed class FilePathPainter : FieldPainter
        {
            public FilePathAttribute FAttribute;
            public GUIContent OpenGC;

            public FilePathPainter(InspectorAttribute attribute) : base(attribute)
            {
                FAttribute = attribute as FilePathAttribute;
                OpenGC = EditorGUIUtility.IconContent("Folder Icon");
            }

            public override void Painting(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(string))
                {
                    string value = (string)fieldInspector.Field.GetValue(inspector.target);
                    if (value == null) value = "";

                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    string newValue = EditorGUILayout.TextField(fieldInspector.Label, value);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(inspector.target, "FilePath");
                        fieldInspector.Field.SetValue(inspector.target, newValue);
                        inspector.HasChanged();
                    }
                    if (GUILayout.Button(OpenGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        string path = EditorUtility.OpenFilePanel("Select File", Application.dataPath, FAttribute.Extension);
                        if (path.Length != 0)
                        {
                            Undo.RecordObject(inspector.target, "FilePath");
                            fieldInspector.Field.SetValue(inspector.target, "Assets" + path.Replace(Application.dataPath, ""));
                            inspector.HasChanged();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("[" + fieldInspector.Field.Name + "] can't used FilePath! because the types don't match!", MessageType.Error);
                    GUILayout.EndHorizontal();
                }
            }
        }
        /// <summary>
        /// 字段绘制器 - 文件夹路径
        /// </summary>
        private sealed class FolderPathPainter : FieldPainter
        {
            public FolderPathAttribute FAttribute;
            public GUIContent OpenGC;

            public FolderPathPainter(InspectorAttribute attribute) : base(attribute)
            {
                FAttribute = attribute as FolderPathAttribute;
                OpenGC = EditorGUIUtility.IconContent("Folder Icon");
            }

            public override void Painting(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(string))
                {
                    string value = (string)fieldInspector.Field.GetValue(inspector.target);
                    if (value == null) value = "";

                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    string newValue = EditorGUILayout.TextField(fieldInspector.Label, value);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(inspector.target, "FolderPath");
                        fieldInspector.Field.SetValue(inspector.target, newValue);
                        inspector.HasChanged();
                    }
                    if (GUILayout.Button(OpenGC, EditorGlobalTools.Styles.IconButton, GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                    {
                        string path = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
                        if (path.Length != 0)
                        {
                            Undo.RecordObject(inspector.target, "FolderPath");
                            fieldInspector.Field.SetValue(inspector.target, "Assets" + path.Replace(Application.dataPath, ""));
                            inspector.HasChanged();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("[" + fieldInspector.Field.Name + "] can't used FolderPath! because the types don't match!", MessageType.Error);
                    GUILayout.EndHorizontal();
                }
            }
        }
        /// <summary>
        /// 字段绘制器 - 通用菜单
        /// </summary>
        private sealed class GenericMenuPainter : FieldPainter
        {
            public GenericMenuAttribute GAttribute;
            public MethodInfo GenerateMenu;
            public MethodInfo ChooseMenu;
            public bool IsReady = false;

            public GenericMenuPainter(InspectorAttribute attribute) : base(attribute)
            {
                GAttribute = attribute as GenericMenuAttribute;
            }

            public override void Painting(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(string))
                {
                    if (!IsReady)
                    {
                        Ready(fieldInspector);
                    }

                    string value = (string)fieldInspector.Field.GetValue(inspector.target);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(fieldInspector.Label, GUILayout.Width(EditorGUIUtility.labelWidth - 5));
                    if (GUILayout.Button(value, EditorStyles.popup))
                    {
                        if (GenerateMenu != null)
                        {
                            string[] menus = CallGenerateMenu(fieldInspector);
                            if (menus != null && menus.Length > 0)
                            {
                                GenericMenu gm = new GenericMenu();
                                for (int i = 0; i < menus.Length; i++)
                                {
                                    int j = i;
                                    gm.AddItem(new GUIContent(menus[j]), value == menus[j], () =>
                                    {
                                        Undo.RecordObject(inspector.target, "GenericMenu");
                                        value = menus[j];
                                        fieldInspector.Field.SetValue(inspector.target, value);
                                        inspector.HasChanged();

                                        if (ChooseMenu != null)
                                        {
                                            CallChooseMenu(fieldInspector, value);
                                        }
                                    });
                                }
                                gm.ShowAsContext();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("[" + fieldInspector.Field.Name + "] can't used GenericMenu! because the types don't match!", MessageType.Error);
                    GUILayout.EndHorizontal();
                }
            }

            public void Ready(FieldInspector fieldInspector)
            {
                IsReady = true;
                BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                if (!string.IsNullOrEmpty(GAttribute.GenerateMenu))
                {
                    GenerateMenu = fieldInspector.Property.serializedObject.targetObject.GetType().GetMethod(GAttribute.GenerateMenu, flags);
                    if (GenerateMenu != null && GenerateMenu.ReturnType != typeof(string[]))
                    {
                        GenerateMenu = null;
                    }
                }
                if (!string.IsNullOrEmpty(GAttribute.ChooseMenu))
                {
                    ChooseMenu = fieldInspector.Property.serializedObject.targetObject.GetType().GetMethod(GAttribute.ChooseMenu, flags);
                    if (ChooseMenu != null)
                    {
                        ParameterInfo[] parameters = ChooseMenu.GetParameters();
                        if (parameters.Length != 1)
                        {
                            GenerateMenu = null;
                        }
                        else if (parameters[0].ParameterType != typeof(string))
                        {
                            GenerateMenu = null;
                        }
                    }
                }
            }

            public string[] CallGenerateMenu(FieldInspector fieldInspector)
            {
                if (GenerateMenu.IsStatic)
                {
                    return GenerateMenu.Invoke(null, null) as string[];
                }
                else
                {
                    return GenerateMenu.Invoke(fieldInspector.Property.serializedObject.targetObject, null) as string[];
                }
            }

            public void CallChooseMenu(FieldInspector fieldInspector, string value)
            {
                if (ChooseMenu.IsStatic)
                {
                    ChooseMenu.Invoke(null, new object[] { value });
                }
                else
                {
                    ChooseMenu.Invoke(fieldInspector.Property.serializedObject.targetObject, new object[] { value });
                }
            }
        }
        /// <summary>
        /// 字段场景处理器
        /// </summary>
        private abstract class FieldSceneHandler
        {
            public SceneHandlerAttribute SAttribute;

            public FieldSceneHandler(SceneHandlerAttribute attribute)
            {
                SAttribute = attribute;
            }

            public abstract void SceneHandle(ObjectInspector inspector, FieldInspector fieldInspector);
        }
        /// <summary>
        /// 字段场景处理器 - 移动手柄
        /// </summary>
        private sealed class MoveHandler : FieldSceneHandler
        {
            public MoveHandlerAttribute MAttribute;

            public MoveHandler(SceneHandlerAttribute attribute) : base(attribute)
            {
                MAttribute = attribute as MoveHandlerAttribute;
            }

            public override void SceneHandle(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(Vector3))
                {
                    Vector3 value = (Vector3)fieldInspector.Field.GetValue(inspector.target);
                    
                    using (new Handles.DrawingScope(fieldInspector.UseColor))
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector3 newValue = Handles.PositionHandle(value, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(inspector.target, "Move Handler");
                            fieldInspector.Field.SetValue(inspector.target, newValue);
                            inspector.HasChanged();
                        }
                        if (MAttribute.Display != null)
                        {
                            Handles.Label(newValue, MAttribute.Display);
                        }
                    }
                }
                else if (fieldInspector.Field.FieldType == typeof(Vector2))
                {
                    Vector2 value = (Vector2)fieldInspector.Field.GetValue(inspector.target);

                    using (new Handles.DrawingScope(fieldInspector.UseColor))
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector2 newValue = Handles.PositionHandle(value, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(inspector.target, "Move Handler");
                            fieldInspector.Field.SetValue(inspector.target, newValue);
                            inspector.HasChanged();
                        }
                        if (MAttribute.Display != null)
                        {
                            Handles.Label(newValue, MAttribute.Display);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 字段场景处理器 - 半径手柄
        /// </summary>
        private sealed class RadiusHandler : FieldSceneHandler
        {
            public RadiusHandlerAttribute RAttribute;

            public RadiusHandler(SceneHandlerAttribute attribute) : base(attribute)
            {
                RAttribute = attribute as RadiusHandlerAttribute;
            }

            public override void SceneHandle(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(float))
                {
                    Component component = inspector.target as Component;
                    Vector3 center = component != null ? component.transform.position : Vector3.zero;
                    float value = (float)fieldInspector.Field.GetValue(inspector.target);

                    using (new Handles.DrawingScope(fieldInspector.UseColor))
                    {
                        EditorGUI.BeginChangeCheck();
                        float newValue = Handles.RadiusHandle(Quaternion.identity, center, value);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(inspector.target, "Radius Handler");
                            fieldInspector.Field.SetValue(inspector.target, newValue);
                            inspector.HasChanged();
                        }
                        if (RAttribute.Display != null)
                        {
                            Handles.Label(center, RAttribute.Display);
                        }
                    }
                }
                else if (fieldInspector.Field.FieldType == typeof(int))
                {
                    Component component = inspector.target as Component;
                    Vector3 center = component != null ? component.transform.position : Vector3.zero;
                    int value = (int)fieldInspector.Field.GetValue(inspector.target);

                    using (new Handles.DrawingScope(fieldInspector.UseColor))
                    {
                        EditorGUI.BeginChangeCheck();
                        int newValue = (int)Handles.RadiusHandle(Quaternion.identity, center, value);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(inspector.target, "Radius Handler");
                            fieldInspector.Field.SetValue(inspector.target, newValue);
                            inspector.HasChanged();
                        }
                        if (RAttribute.Display != null)
                        {
                            Handles.Label(center, RAttribute.Display);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 字段场景处理器 - 包围盒
        /// </summary>
        private sealed class BoundsHandler : FieldSceneHandler
        {
            public BoundsHandlerAttribute BAttribute;
            public BoxBoundsHandle BoundsHandle;

            public BoundsHandler(SceneHandlerAttribute attribute) : base(attribute)
            {
                BAttribute = attribute as BoundsHandlerAttribute;
                BoundsHandle = new BoxBoundsHandle();
            }

            public override void SceneHandle(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(Bounds))
                {
                    Bounds value = (Bounds)fieldInspector.Field.GetValue(inspector.target);
                    BoundsHandle.center = value.center;
                    BoundsHandle.size = value.size;

                    using (new Handles.DrawingScope(fieldInspector.UseColor))
                    {
                        EditorGUI.BeginChangeCheck();
                        BoundsHandle.DrawHandle();
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(inspector.target, "Bounds Handler");
                            value.center = BoundsHandle.center;
                            value.size = BoundsHandle.size;
                            fieldInspector.Field.SetValue(inspector.target, value);
                            inspector.HasChanged();
                        }
                        if (BAttribute.Display != null)
                        {
                            Handles.Label(BoundsHandle.center, BAttribute.Display);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 字段场景处理器 - 方向
        /// </summary>
        private sealed class DirectionHandler : FieldSceneHandler
        {
            public DirectionHandlerAttribute DAttribute;
            public Transform Target;
            public Vector3 Position;
            public float ExternalSize;
            public float InternalSize;
            public float DynamicMultiple;

            public DirectionHandler(SceneHandlerAttribute attribute) : base(attribute)
            {
                DAttribute = attribute as DirectionHandlerAttribute;
                DynamicMultiple = 1;
            }

            public override void SceneHandle(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(Vector3))
                {
                    Vector3 value = (Vector3)fieldInspector.Field.GetValue(inspector.target);

                    if (value != Vector3.zero)
                    {
                        using (new Handles.DrawingScope(fieldInspector.UseColor))
                        {
                            ExternalSize = GetExternalSize(inspector.target);
                            InternalSize = GetInternalSize(inspector.target);
                            Handles.CircleHandleCap(0, Position, Quaternion.FromToRotation(Vector3.forward, value), ExternalSize, EventType.Repaint);
                            Handles.CircleHandleCap(0, Position, Quaternion.FromToRotation(Vector3.forward, value), InternalSize, EventType.Repaint);
                            Handles.Slider(Position, value);
                        }
                    }
                }
                else if (fieldInspector.Field.FieldType == typeof(Vector2))
                {
                    Vector2 value = (Vector2)fieldInspector.Field.GetValue(inspector.target);

                    if (value != Vector2.zero)
                    {
                        using (new Handles.DrawingScope(fieldInspector.UseColor))
                        {
                            ExternalSize = GetExternalSize(inspector.target);
                            InternalSize = GetInternalSize(inspector.target);
                            Handles.CircleHandleCap(0, Position, Quaternion.FromToRotation(Vector3.forward, value), ExternalSize, EventType.Repaint);
                            Handles.CircleHandleCap(0, Position, Quaternion.FromToRotation(Vector3.forward, value), InternalSize, EventType.Repaint);
                            Handles.Slider(Position, value);
                        }
                    }
                }
            }

            public float GetExternalSize(UObject target)
            {
                if (Target == null)
                {
                    Component component = target as Component;
                    if (component)
                    {
                        Target = component.transform;
                    }
                }

                if (Target != null)
                {
                    Position = Target.position;
                    return HandleUtility.GetHandleSize(Target.TransformPoint(Target.position)) * 1;
                }
                else
                {
                    return 1;
                }
            }

            public float GetInternalSize(UObject target)
            {
                if (Target == null)
                {
                    Component component = target as Component;
                    if (component)
                    {
                        Target = component.transform;
                    }
                }

                if (DAttribute.IsDynamic)
                {
                    if (DynamicMultiple < 2)
                    {
                        DynamicMultiple += 0.005f;
                    }
                    else
                    {
                        DynamicMultiple = 0;
                    }
                    GUI.changed = true;
                }

                if (Target != null)
                {
                    Position = Target.position;
                    return HandleUtility.GetHandleSize(Target.TransformPoint(Target.position)) * 0.5f * DynamicMultiple;
                }
                else
                {
                    return 0.5f * DynamicMultiple;
                }
            }
        }
        /// <summary>
        /// 字段场景处理器 - 圆形区域
        /// </summary>
        private sealed class CircleAreaHandler : FieldSceneHandler
        {
            public CircleAreaHandlerAttribute CAttribute;
            public Transform Target;
            public Vector3 Position;
            public Quaternion Rotation;
            public float Size;
            public float DynamicMultiple;

            public CircleAreaHandler(SceneHandlerAttribute attribute) : base(attribute)
            {
                CAttribute = attribute as CircleAreaHandlerAttribute;
                Rotation = GetRotation();
                DynamicMultiple = 1;
            }

            public override void SceneHandle(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(float))
                {
                    float value = (float)fieldInspector.Field.GetValue(inspector.target);

                    using (new Handles.DrawingScope(fieldInspector.UseColor))
                    {
                        Position = GetPosition(inspector.target);
                        Size = GetSize(inspector.target, value);
                        Handles.CircleHandleCap(0, Position, Rotation, Size, EventType.Repaint);
                        if (Target)
                        {
                            Handles.Slider(Position, Target.forward);
                        }
                    }
                }
            }

            public Vector3 GetPosition(UObject target)
            {
                if (Target == null)
                {
                    Component component = target as Component;
                    if (component)
                    {
                        Target = component.transform;
                    }
                }

                return Target != null ? Target.position : Vector3.zero;
            }

            public Quaternion GetRotation()
            {
                if (CAttribute.Direction == CircleAreaHandlerAttribute.Axis.X)
                {
                    return Quaternion.FromToRotation(Vector3.forward, Vector3.right);
                }
                else if (CAttribute.Direction == CircleAreaHandlerAttribute.Axis.Y)
                {
                    return Quaternion.FromToRotation(Vector3.forward, Vector3.up);
                }
                else
                {
                    return Quaternion.identity;
                }
            }

            public float GetSize(UObject target, float value)
            {
                if (CAttribute.IsDynamic)
                {
                    if (DynamicMultiple < 1)
                    {
                        DynamicMultiple += 0.0025f;
                    }
                    else
                    {
                        DynamicMultiple = 0;
                    }
                    GUI.changed = true;
                }
                
                return value * DynamicMultiple;
            }
        }
        #endregion

        #region Property
        /// <summary>
        /// 属性检视器
        /// </summary>
        private sealed class PropertyInspector
        {
            public PropertyInfo Property;
            public PropertyDisplayAttribute Attribute;
            public string Name;

            public PropertyInspector(PropertyInfo property)
            {
                Property = property;
                Attribute = property.GetCustomAttribute<PropertyDisplayAttribute>(true);
                Name = string.IsNullOrEmpty(Attribute.Text) ? property.Name : Attribute.Text;
            }

            public void Painting(ObjectInspector inspector)
            {
                if (!Property.CanRead)
                    return;

                if (Attribute.DisplayOnlyRuntime && !EditorApplication.isPlaying)
                    return;

                if (Property.CanWrite)
                {
                    CanWritePainting(inspector);
                }
                else
                {
                    ReadOnlyPainting(inspector);
                }
            }

            private void CanWritePainting(ObjectInspector inspector)
            {
                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                object value = Property.GetValue(inspector.target);
                object newValue = value;
                if (Property.PropertyType == typeof(string))
                {
                    string realValue = EditorGUILayout.TextField(Name, (string)value);
                    if (EditorGUI.EndChangeCheck()) newValue = realValue;
                }
                else if (Property.PropertyType == typeof(int))
                {
                    int realValue = EditorGUILayout.IntField(Name, (int)value);
                    if (EditorGUI.EndChangeCheck()) newValue = realValue;
                }
                else if (Property.PropertyType == typeof(float))
                {
                    float realValue = EditorGUILayout.FloatField(Name, (float)value);
                    if (EditorGUI.EndChangeCheck()) newValue = realValue;
                }
                else if (Property.PropertyType == typeof(bool))
                {
                    bool realValue = EditorGUILayout.Toggle(Name, (bool)value);
                    if (EditorGUI.EndChangeCheck()) newValue = realValue;
                }
                else if (Property.PropertyType == typeof(Vector2))
                {
                    Vector2 realValue = EditorGUILayout.Vector2Field(Name, (Vector2)value);
                    if (EditorGUI.EndChangeCheck()) newValue = realValue;
                }
                else if (Property.PropertyType == typeof(Vector3))
                {
                    Vector3 realValue = EditorGUILayout.Vector3Field(Name, (Vector3)value);
                    if (EditorGUI.EndChangeCheck()) newValue = realValue;
                }
                else if (Property.PropertyType == typeof(Color))
                {
                    Color realValue = EditorGUILayout.ColorField(Name, (Color)value);
                    if (EditorGUI.EndChangeCheck()) newValue = realValue;
                }
                else if (Property.PropertyType.IsSubclassOf(typeof(UObject)))
                {
                    UObject realValue = EditorGUILayout.ObjectField(Name, value as UObject, Property.PropertyType, true);
                    if (EditorGUI.EndChangeCheck()) newValue = realValue;
                }
                else
                {
                    EditorGUILayout.HelpBox("[" + Name + "] can't used PropertyDisplay! because the types don't match!", MessageType.Error);
                }
                GUILayout.EndHorizontal();

                if (value != newValue)
                {
                    Undo.RecordObject(inspector.target, "Property Changed");
                    Property.SetValue(inspector.target, newValue);
                    inspector.HasChanged();
                }
            }
            
            private void ReadOnlyPainting(ObjectInspector inspector)
            {
                GUI.enabled = false;

                GUILayout.BeginHorizontal();
                object value = Property.GetValue(inspector.target);
                if (Property.PropertyType == typeof(string))
                {
                    EditorGUILayout.TextField(Name, (string)value);
                }
                else if (Property.PropertyType == typeof(int))
                {
                    EditorGUILayout.IntField(Name, (int)value);
                }
                else if (Property.PropertyType == typeof(float))
                {
                    EditorGUILayout.FloatField(Name, (float)value);
                }
                else if (Property.PropertyType == typeof(bool))
                {
                    EditorGUILayout.Toggle(Name, (bool)value);
                }
                else if (Property.PropertyType == typeof(Vector2))
                {
                    EditorGUILayout.Vector2Field(Name, (Vector2)value);
                }
                else if (Property.PropertyType == typeof(Vector3))
                {
                    EditorGUILayout.Vector3Field(Name, (Vector3)value);
                }
                else if (Property.PropertyType == typeof(Color))
                {
                    EditorGUILayout.ColorField(Name, (Color)value);
                }
                else if (Property.PropertyType.IsSubclassOf(typeof(UObject)))
                {
                    EditorGUILayout.ObjectField(Name, value as UObject, Property.PropertyType, false);
                }
                else
                {
                    EditorGUILayout.HelpBox("[" + Name + "] can't used PropertyDisplay! because the types don't match!", MessageType.Error);
                }
                GUILayout.EndHorizontal();

                GUI.enabled = true;
            }
        }
        #endregion

        #region Event
        /// <summary>
        /// 事件检视器
        /// </summary>
        private sealed class EventInspector
        {
            public FieldInfo Field;
            public EventAttribute Attribute;
            public string Name;
            public bool IsFoldout;

            public EventInspector(FieldInfo field)
            {
                Field = field;
                Attribute = field.GetCustomAttribute<EventAttribute>(true);
                Name = string.IsNullOrEmpty(Attribute.Text) ? field.Name : Attribute.Text;
                IsFoldout = true;
            }

            public void Painting(ObjectInspector inspector)
            {
                MulticastDelegate multicast = Field.GetValue(inspector.target) as MulticastDelegate;
                Delegate[] delegates = multicast != null ? multicast.GetInvocationList() : null;

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                IsFoldout = EditorGUILayout.Foldout(IsFoldout, string.Format("{0} [{1}]", Name, delegates != null ? delegates.Length : 0));
                GUILayout.EndHorizontal();

                if (IsFoldout && delegates != null)
                {
                    for (int i = 0; i < delegates.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(30);
                        GUILayout.Label(string.Format("{0}->{1}", delegates[i].Target, delegates[i].Method), "Textfield");
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// 函数检视器
        /// </summary>
        private sealed class MethodInspector
        {
            public MethodInfo Method;
            public ButtonAttribute Attribute;
            public string Name;

            public MethodInspector(MethodInfo method)
            {
                Method = method;
                Attribute = method.GetCustomAttribute<ButtonAttribute>(true);
                Name = string.IsNullOrEmpty(Attribute.Text) ? Method.Name : Attribute.Text;
            }

            public void Painting(ObjectInspector inspector)
            {
                GUI.enabled = Attribute.Mode == ButtonAttribute.EnableMode.Always
                || (Attribute.Mode == ButtonAttribute.EnableMode.Editor && !EditorApplication.isPlaying)
                || (Attribute.Mode == ButtonAttribute.EnableMode.Playmode && EditorApplication.isPlaying);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Name, Attribute.Style))
                {
                    inspector.HasChanged();

                    if (Method.ReturnType.Name != "Void")
                    {
                        object result = null;
                        if (Method.IsStatic) result = Method.Invoke(null, null);
                        else result = Method.Invoke(inspector.target, null);
                        Log.Info("点击按钮 " + Name + " 后，存在返回值：" + result);
                    }
                    else
                    {
                        if (Method.IsStatic) Method.Invoke(null, null);
                        else Method.Invoke(inspector.target, null);
                    }
                }
                GUILayout.EndHorizontal();

                GUI.enabled = true;
            }
        }
        #endregion
    }
}