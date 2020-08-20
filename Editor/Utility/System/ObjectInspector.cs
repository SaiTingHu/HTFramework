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
        private List<MethodInspector> _methods = new List<MethodInspector>();

        private void OnEnable()
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
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            FieldGUI();
            MethodGUI();

            serializedObject.ApplyModifiedProperties();
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

        /// <summary>
        /// 字段检视器
        /// </summary>
        private sealed class FieldInspector
        {
            public FieldInfo Field;
            public SerializedProperty Property;
            public List<FieldDrawer> Drawers = new List<FieldDrawer>();
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
                    InspectorAttribute[] attributes = (InspectorAttribute[])Field.GetCustomAttributes(typeof(InspectorAttribute), true);
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        if (attributes[i] is DropdownAttribute)
                        {
                            Drawers.Add(new DropdownDrawer(attributes[i]));
                        }
                        else if (attributes[i] is LayerAttribute)
                        {
                            Drawers.Add(new LayerDrawer(attributes[i]));
                        }
                        else if (attributes[i] is ReorderableListAttribute)
                        {
                            Drawers.Add(new ReorderableList(attributes[i]));
                        }
                        else if (attributes[i] is PasswordAttribute)
                        {
                            Drawers.Add(new PasswordDrawer(attributes[i]));
                        }
                        else if (attributes[i] is EnableAttribute)
                        {
                            EnableCondition = property.serializedObject.targetObject.GetType().GetMethod(attributes[i].Cast<EnableAttribute>().Condition, flags);
                            if (EnableCondition != null && EnableCondition.ReturnType != typeof(bool))
                            {
                                EnableCondition = null;
                            }
                        }
                        else if (attributes[i] is DisplayAttribute)
                        {
                            DisplayCondition = property.serializedObject.targetObject.GetType().GetMethod(attributes[i].Cast<DisplayAttribute>().Condition, flags);
                            if (DisplayCondition != null && DisplayCondition.ReturnType != typeof(bool))
                            {
                                DisplayCondition = null;
                            }
                        }
                        else if (attributes[i] is LabelAttribute)
                        {
                            Label = attributes[i].Cast<LabelAttribute>().Name;
                        }
                        else if (attributes[i] is ColorAttribute)
                        {
                            ColorAttribute attribute = attributes[i] as ColorAttribute;
                            UseColor = new Color(attribute.R, attribute.G, attribute.B, attribute.A);
                        }
                        else if (attributes[i] is ReadOnlyAttribute)
                        {
                            IsReadOnly = true;
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
    }
}