using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
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
    public sealed class ObjectInspector : Editor
    {
        private List<FieldInspector> _fields = new List<FieldInspector>();
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
            for (int i = 0; i < _fields.Count; i++)
            {
                _fields[i].Draw(this);
            }
        }
        /// <summary>
        /// 绘制事件
        /// </summary>
        private void EventGUI()
        {
            for (int i = 0; i < _events.Count; i++)
            {
                _events[i].Draw(this);
            }
        }
        /// <summary>
        /// 绘制方法
        /// </summary>
        private void MethodGUI()
        {
            for (int i = 0; i < _methods.Count; i++)
            {
                _methods[i].Draw(this);
            }
        }
        /// <summary>
        /// 场景中处理字段
        /// </summary>
        private void FieldSceneHandle()
        {
            for (int i = 0; i < _fields.Count; i++)
            {
                _fields[i].SceneHandle(this);
            }
        }
        /// <summary>
        /// 标记目标已改变
        /// </summary>
        private void HasChanged()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorUtility.SetDirty(target);
                Component component = target as Component;
                if (component != null && component.gameObject.scene != null)
                {
                    EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
                }
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
            public List<FieldDrawer> Drawers = new List<FieldDrawer>();
            public List<FieldSceneHandler> SceneHandlers = new List<FieldSceneHandler>();
            public MethodInfo EnableCondition;
            public MethodInfo DisplayCondition;
            public string Label;
            public Color UseColor = Color.white;
            public bool IsReadOnly = false;

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
                            Drawers.Add(new DropdownDrawer(iattributes[i]));
                        }
                        else if (iattributes[i] is LayerAttribute)
                        {
                            Drawers.Add(new LayerDrawer(iattributes[i]));
                        }
                        else if (iattributes[i] is ReorderableListAttribute)
                        {
                            Drawers.Add(new ReorderableList(iattributes[i]));
                        }
                        else if (iattributes[i] is PasswordAttribute)
                        {
                            Drawers.Add(new PasswordDrawer(iattributes[i]));
                        }
                        else if (iattributes[i] is HyperlinkAttribute)
                        {
                            Drawers.Add(new HyperlinkDrawer(iattributes[i]));
                        }
                        else if (iattributes[i] is FilePathAttribute)
                        {
                            Drawers.Add(new FilePathDrawer(iattributes[i]));
                        }
                        else if (iattributes[i] is FolderPathAttribute)
                        {
                            Drawers.Add(new FolderPathDrawer(iattributes[i]));
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
                    }
                }
            }

            public void Draw(ObjectInspector inspector)
            {
                if (IsDisplay)
                {
                    GUI.color = UseColor;
                    if (Drawers.Count > 0)
                    {
                        GUI.enabled = IsEnable;
                        for (int i = 0; i < Drawers.Count; i++)
                        {
                            Drawers[i].Draw(inspector, this);
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
        private abstract class FieldDrawer
        {
            public InspectorAttribute IAttribute;

            public FieldDrawer(InspectorAttribute attribute)
            {
                IAttribute = attribute;
            }

            public abstract void Draw(ObjectInspector inspector, FieldInspector fieldInspector);
        }
        /// <summary>
        /// 字段绘制器 - 下拉菜单
        /// </summary>
        private sealed class DropdownDrawer : FieldDrawer
        {
            public DropdownAttribute DAttribute;

            public DropdownDrawer(InspectorAttribute attribute) : base(attribute)
            {
                DAttribute = attribute as DropdownAttribute;
            }

            public override void Draw(ObjectInspector inspector, FieldInspector fieldInspector)
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
        private sealed class LayerDrawer : FieldDrawer
        {
            public LayerAttribute LAttribute;

            public LayerDrawer(InspectorAttribute attribute) : base(attribute)
            {
                LAttribute = attribute as LayerAttribute;
            }

            public override void Draw(ObjectInspector inspector, FieldInspector fieldInspector)
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
        private sealed class ReorderableList : FieldDrawer
        {
            public ReorderableListAttribute RAttribute;
            public UReorderableList List;

            public ReorderableList(InspectorAttribute attribute) : base(attribute)
            {
                RAttribute = attribute as ReorderableListAttribute;
            }

            public override void Draw(ObjectInspector inspector, FieldInspector fieldInspector)
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
        private sealed class PasswordDrawer : FieldDrawer
        {
            public PasswordAttribute PAttribute;

            public PasswordDrawer(InspectorAttribute attribute) : base(attribute)
            {
                PAttribute = attribute as PasswordAttribute;
            }

            public override void Draw(ObjectInspector inspector, FieldInspector fieldInspector)
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
        private sealed class HyperlinkDrawer : FieldDrawer
        {
            public HyperlinkAttribute HAttribute;
            public MethodInfo LinkLabel;
            public object[] Parameter;

            public HyperlinkDrawer(InspectorAttribute attribute) : base(attribute)
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

            public override void Draw(ObjectInspector inspector, FieldInspector fieldInspector)
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
        private sealed class FilePathDrawer : FieldDrawer
        {
            public FilePathAttribute FAttribute;
            public GUIContent OpenGC;

            public FilePathDrawer(InspectorAttribute attribute) : base(attribute)
            {
                FAttribute = attribute as FilePathAttribute;
                OpenGC = EditorGUIUtility.IconContent("Folder Icon");
            }

            public override void Draw(ObjectInspector inspector, FieldInspector fieldInspector)
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
        private sealed class FolderPathDrawer : FieldDrawer
        {
            public FolderPathAttribute FAttribute;
            public GUIContent OpenGC;

            public FolderPathDrawer(InspectorAttribute attribute) : base(attribute)
            {
                FAttribute = attribute as FolderPathAttribute;
                OpenGC = EditorGUIUtility.IconContent("Folder Icon");
            }

            public override void Draw(ObjectInspector inspector, FieldInspector fieldInspector)
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
            public Transform Target;

            public BoundsHandler(SceneHandlerAttribute attribute) : base(attribute)
            {
                BAttribute = attribute as BoundsHandlerAttribute;
            }

            public override void SceneHandle(ObjectInspector inspector, FieldInspector fieldInspector)
            {
                if (fieldInspector.Field.FieldType == typeof(Bounds))
                {
                    Bounds value = (Bounds)fieldInspector.Field.GetValue(inspector.target);
                    
                    using (new Handles.DrawingScope(fieldInspector.UseColor))
                    {
                        Handles.DrawWireCube(value.center, value.size);
                        if (BAttribute.IsCanEdit)
                        {
                            Vector3 center = value.center;
                            Vector3 min = value.min;
                            Vector3 max = value.max;
                            Vector3 left = new Vector3(min.x, center.y, center.z);
                            Vector3 right = new Vector3(max.x, center.y, center.z);
                            Vector3 up = new Vector3(center.x, max.y, center.z);
                            Vector3 down = new Vector3(center.x, min.y, center.z);
                            Vector3 forward = new Vector3(center.x, center.y, max.z);
                            Vector3 back = new Vector3(center.x, center.y, min.z);

                            EditorGUI.BeginChangeCheck();
                            Vector3 newLeft = Handles.Slider(left, Vector3.left, GetSize(inspector.target, left), Handles.DotHandleCap, 0);
                            Vector3 newRight = Handles.Slider(right, Vector3.right, GetSize(inspector.target, right), Handles.DotHandleCap, 0);
                            Vector3 newUp = Handles.Slider(up, Vector3.up, GetSize(inspector.target, up), Handles.DotHandleCap, 0);
                            Vector3 newDown = Handles.Slider(down, Vector3.down, GetSize(inspector.target, down), Handles.DotHandleCap, 0);
                            Vector3 newForward = Handles.Slider(forward, Vector3.forward, GetSize(inspector.target, forward), Handles.DotHandleCap, 0);
                            Vector3 newBack = Handles.Slider(back, Vector3.back, GetSize(inspector.target, back), Handles.DotHandleCap, 0);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(inspector.target, "Bounds Handler");
                                value.SetMinMax(new Vector3(newLeft.x, newDown.y, newBack.z), new Vector3(newRight.x, newUp.y, newForward.z));
                                fieldInspector.Field.SetValue(inspector.target, value);
                                inspector.HasChanged();
                            }
                        }
                        if (BAttribute.Display != null)
                        {
                            Handles.Label(value.center, BAttribute.Display);
                        }
                    }
                }
            }

            public float GetSize(UObject target, Vector3 point)
            {
                if (Target == null)
                {
                    Component component = target as Component;
                    if (component != null)
                    {
                        Target = component.transform;
                    }
                }

                return Target != null ? (HandleUtility.GetHandleSize(Target.TransformPoint(point)) * 0.03f) : 1;
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

            public void Draw(ObjectInspector inspector)
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

            public void Draw(ObjectInspector inspector)
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