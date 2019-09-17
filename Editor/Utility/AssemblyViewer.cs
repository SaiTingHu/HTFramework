using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 程序集查看器
    /// </summary>
    public sealed class AssemblyViewer : EditorWindow
    {
        [@MenuItem("HTFramework/Assembly Viewer")]
        private static void ShowWindow()
        {
            AssemblyViewer viewer = GetWindow<AssemblyViewer>();
            viewer.titleContent.text = "AssemblyViewer";
            viewer.Show();
        }

        private Assembly[] _assemblies;
        private Assembly _currentAssembly;
        private Vector2 _assemblyScroll = Vector2.zero;
        private string _assemblyFilter = "";

        private Type[] _types;
        private Type _currentType;
        private Vector2 _typeScroll = Vector2.zero;
        private string _typeFilter = "";
        private bool _onlyShowStaticType = false;

        private FieldInfo[] _fields;
        private FieldInfo _currentField;
        private MethodInfo[] _methods;
        private MethodInfo _currentMethod;
        private PropertyInfo[] _propertys;
        private PropertyInfo _currentProperty;
        private ParameterInfo[] _parameterInfos;
        private Vector2 _memberScroll = Vector2.zero;
        private string _memberFilter = "";
        private bool _onlyShowStaticMember = false;
        private bool _showField = true;
        private bool _showMethod = true;
        private bool _showProperty = true;

        private StringBuilder _builder = new StringBuilder();

        private void OnEnable()
        {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _currentAssembly = null;
            _currentType = null;
            _currentMethod = null;
            _currentField = null;
            _currentProperty = null;
        }
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            OnAssemblyGUI();
            OnTypeGUI();
            OnMemberGUI();

            GUILayout.EndHorizontal();
        }

        private void OnAssemblyGUI()
        {
            GUILayout.BeginVertical("Helpbox", GUILayout.Width(position.width / 3 - 5));

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Assembly", "PreLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tag:");
            if (GUILayout.Button("UnityEngine", "Minibutton"))
            {
                _assemblyFilter = "UnityEngine";
            }
            if (GUILayout.Button("UnityEditor", "Minibutton"))
            {
                _assemblyFilter = "UnityEditor";
            }
            if (GUILayout.Button("Assembly-CSharp", "Minibutton"))
            {
                _assemblyFilter = "Assembly-CSharp";
            }
            if (GUILayout.Button("Assembly-CSharp-Editor", "Minibutton"))
            {
                _assemblyFilter = "Assembly-CSharp-Editor";
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _assemblyFilter = EditorGUILayout.TextField("", _assemblyFilter, "SearchTextField");
            if (GUILayout.Button("", _assemblyFilter != "" ? "SearchCancelButton" : "SearchCancelButtonEmpty"))
            {
                _assemblyFilter = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            _assemblyScroll = GUILayout.BeginScrollView(_assemblyScroll, "Box");

            for (int i = 0; i < _assemblies.Length; i++)
            {
                AssemblyName an = _assemblies[i].GetName();
                if (an.Name.ToLower().Contains(_assemblyFilter.ToLower()))
                {
                    GUI.color = _currentAssembly == _assemblies[i] ? Color.cyan : Color.white;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(an.Name, "Toolbarbutton"))
                    {
                        _currentAssembly = _assemblies[i];
                        _types = _currentAssembly.GetTypes();
                        _currentType = null;
                        _currentMethod = null;
                        _currentField = null;
                        _currentProperty = null;
                        GUI.FocusControl(null);
                    }
                    GUILayout.EndHorizontal();

                    if (_currentAssembly == _assemblies[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Version:" + an.Version);
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Open", "Minibutton"))
                        {
                            string args = "/Select, " + _currentAssembly.Location;
                            ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe", args);
                            Process.Start(psi);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUI.color = Color.white;
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        private void OnTypeGUI()
        {
            GUILayout.BeginVertical("Helpbox", GUILayout.Width(position.width / 3 - 5));

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Type", "PreLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _onlyShowStaticType = GUILayout.Toggle(_onlyShowStaticType, "Only Show Static");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _typeFilter = EditorGUILayout.TextField("", _typeFilter, "SearchTextField");
            if (GUILayout.Button("", _typeFilter != "" ? "SearchCancelButton" : "SearchCancelButtonEmpty"))
            {
                _typeFilter = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            _typeScroll = GUILayout.BeginScrollView(_typeScroll, "Box");

            if (_currentAssembly != null)
            {
                for (int i = 0; i < _types.Length; i++)
                {
                    if (!IsRejectType(_types[i]) && _types[i].Name.ToLower().Contains(_typeFilter.ToLower()))
                    {
                        if (_onlyShowStaticType && (!_types[i].IsAbstract || !_types[i].IsSealed))
                        {
                            continue;
                        }

                        GUI.color = _currentType == _types[i] ? Color.cyan : Color.white;
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(_types[i].Name, "Toolbarbutton"))
                        {
                            _currentType = _types[i];
                            _fields = _currentType.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            _methods = _currentType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            _propertys = _currentType.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            _currentMethod = null;
                            _currentField = null;
                            _currentProperty = null;
                            GUI.FocusControl(null);
                        }
                        GUILayout.EndHorizontal();

                        if (_currentType == _types[i])
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.TextField("namespace " + _currentType.Namespace);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.TextField(FormatTypeInfo(_currentType));
                            GUILayout.EndHorizontal();
                        }
                        GUI.color = Color.white;
                    }
                }
            }
            
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        private void OnMemberGUI()
        {
            GUILayout.BeginVertical("Helpbox", GUILayout.Width(position.width / 3 - 5));

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Member", "PreLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _onlyShowStaticMember = GUILayout.Toggle(_onlyShowStaticMember, "Only Show Static");
            _showField = GUILayout.Toggle(_showField, "Show Field");
            _showMethod = GUILayout.Toggle(_showMethod, "Show Method");
            _showProperty = GUILayout.Toggle(_showProperty, "Show Property");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _memberFilter = EditorGUILayout.TextField("", _memberFilter, "SearchTextField");
            if (GUILayout.Button("", _memberFilter != "" ? "SearchCancelButton" : "SearchCancelButtonEmpty"))
            {
                _memberFilter = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            _memberScroll = GUILayout.BeginScrollView(_memberScroll, "Box");

            if (_currentType != null)
            {
                if (_showField)
                {
                    for (int i = 0; i < _fields.Length; i++)
                    {
                        if (!IsRejectField(_fields[i]) && _fields[i].Name.ToLower().Contains(_memberFilter.ToLower()))
                        {
                            if (_onlyShowStaticMember && !_fields[i].IsStatic)
                            {
                                continue;
                            }

                            GUI.color = _currentField == _fields[i] ? Color.cyan : Color.white;
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("[Field]  " + _fields[i].Name, "Toolbarbutton"))
                            {
                                _currentField = _fields[i];
                                _currentMethod = null;
                                _currentProperty = null;
                                GUI.FocusControl(null);
                            }
                            GUILayout.EndHorizontal();

                            if (_currentField == _fields[i])
                            {
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.TextField(FormatFieldInfo(_currentField));
                                GUILayout.EndHorizontal();
                            }
                            GUI.color = Color.white;
                        }
                    }
                }
                if (_showMethod)
                {
                    for (int i = 0; i < _methods.Length; i++)
                    {
                        if (!IsRejectMethod(_methods[i]) && _methods[i].Name.ToLower().Contains(_memberFilter.ToLower()))
                        {
                            if (_onlyShowStaticMember && !_methods[i].IsStatic)
                            {
                                continue;
                            }

                            GUI.color = _currentMethod == _methods[i] ? Color.cyan : Color.white;
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("[Method]  " + _methods[i].Name, "Toolbarbutton"))
                            {
                                _currentMethod = _methods[i];
                                _parameterInfos = _currentMethod.GetParameters();
                                _currentField = null;
                                _currentProperty = null;
                                GUI.FocusControl(null);
                            }
                            GUILayout.EndHorizontal();

                            if (_currentMethod == _methods[i])
                            {
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.TextField(FormatMethodInfo(_currentMethod));
                                GUILayout.EndHorizontal();
                            }
                            GUI.color = Color.white;
                        }
                    }
                }
                if (_showProperty)
                {
                    for (int i = 0; i < _propertys.Length; i++)
                    {
                        if (!IsRejectProperty(_propertys[i]) && _propertys[i].Name.ToLower().Contains(_memberFilter.ToLower()))
                        {
                            if (_onlyShowStaticMember && !IsStatic(_propertys[i]))
                            {
                                continue;
                            }

                            GUI.color = _currentProperty == _propertys[i] ? Color.cyan : Color.white;
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("[Property]  " + _propertys[i].Name, "Toolbarbutton"))
                            {
                                _currentProperty = _propertys[i];
                                _currentMethod = null;
                                _currentField = null;
                                GUI.FocusControl(null);
                            }
                            GUILayout.EndHorizontal();

                            if (_currentProperty == _propertys[i])
                            {
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.TextField(FormatPropertyInfo(_currentProperty));
                                GUILayout.EndHorizontal();
                            }
                            GUI.color = Color.white;
                        }
                    }
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private string FormatTypeInfo(Type type)
        {
            _builder.Clear();
            _builder.Append(type.IsVisible ? (type.IsPublic ? "public " : "private ") : "internal ");
            if (type.IsAbstract && type.IsSealed)
            {
                _builder.Append("static ");
            }
            else
            {
                if (type.IsAbstract) _builder.Append("abstract ");
                if (type.IsSealed) _builder.Append("sealed ");
            }
            if (type.IsClass) _builder.Append("class ");
            if (type.IsValueType && !type.IsEnum) _builder.Append("struct ");
            if (type.IsInterface) _builder.Append("interface ");
            if (type.IsEnum) _builder.Append("enum ");
            _builder.Append(type.Name);
            if (type.IsGenericType) _builder.Append("<T>");
            _builder.Append(" : ");
            _builder.Append(type.BaseType != null ? type.BaseType.Name : "<None>");
            
            return _builder.ToString();
        }
        private string FormatFieldInfo(FieldInfo field)
        {
            _builder.Clear();
            _builder.Append(field.IsFamily ? "protected " : (field.IsPublic ? "public " : "private "));
            if (field.IsStatic) _builder.Append("static ");
            if (field.IsAssembly) _builder.Append("internal ");
            _builder.Append(field.FieldType.Name);
            _builder.Append(" ");
            _builder.Append(field.Name);
            
            return _builder.ToString();
        }
        private string FormatMethodInfo(MethodInfo method)
        {
            _builder.Clear();
            _builder.Append(method.IsFamily ? "protected " : (method.IsPublic ? "public " : "private "));
            if (method.IsStatic) _builder.Append("static ");
            if (method.IsAssembly) _builder.Append("internal ");
            if (method.IsAbstract) _builder.Append("abstract ");
            if (method.IsVirtual) _builder.Append("virtual ");
            _builder.Append(method.ReturnType.Name);
            _builder.Append(" ");
            _builder.Append(method.Name);
            _builder.Append("(");
            for (int i = 0; i < _parameterInfos.Length; i++)
            {
                if (i != 0)
                {
                    _builder.Append(", ");
                }
                if (_parameterInfos[i].IsIn) _builder.Append("in ");
                if (_parameterInfos[i].IsOut) _builder.Append("out ");
                _builder.Append(_parameterInfos[i].ParameterType.Name);
                _builder.Append(" ");
                _builder.Append(_parameterInfos[i].Name);
            }
            _builder.Append(")");

            return _builder.ToString();
        }
        private string FormatPropertyInfo(PropertyInfo property)
        {
            MethodInfo get = property.GetGetMethod();
            MethodInfo set = property.GetSetMethod();
            MethodInfo method = (get != null ? get : set);
            if (method == null)
            {
                return "private " + property.PropertyType.Name + " " + property.Name + " {}";
            }

            _builder.Clear();
            _builder.Append(method.IsFamily ? "protected " : (method.IsPublic ? "public " : "private "));
            if (method.IsStatic) _builder.Append("static ");
            if (method.IsAssembly) _builder.Append("internal ");
            if (method.IsAbstract) _builder.Append("abstract ");
            if (method.IsVirtual) _builder.Append("virtual ");
            _builder.Append(property.PropertyType.Name);
            _builder.Append(" ");
            _builder.Append(property.Name);
            _builder.Append(" { ");
            if (get != null) _builder.Append("get;");
            if (set != null) _builder.Append("set;");
            _builder.Append(" }");

            return _builder.ToString();
        }
        private bool IsStatic(PropertyInfo property)
        {
            MethodInfo getMethod = property.GetGetMethod();
            MethodInfo setMethod = property.GetSetMethod();
            if (getMethod != null)
            {
                return getMethod.IsStatic;
            }
            else if (setMethod != null)
            {
                return setMethod.IsStatic;
            }
            else
            {
                return false;
            }
        }
        private bool IsRejectType(Type type)
        {
            if (type.Name.Contains("__") || type.Name.Contains("<") || type.Name.Contains(">"))
            {
                return true;
            }
            return false;
        }
        private bool IsRejectField(FieldInfo field)
        {
            if (field.Name.Contains("<") || field.Name.Contains(">"))
            {
                return true;
            }
            return false;
        }
        private bool IsRejectMethod(MethodInfo method)
        {
            if (method.Name.Contains("get_") || method.Name.Contains("set_"))
            {
                return true;
            }
            return false;
        }
        private bool IsRejectProperty(PropertyInfo property)
        {
            return false;
        }
    }
}