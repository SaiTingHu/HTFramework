using System;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 程序集查看器
    /// </summary>
    internal sealed class AssemblyViewer : HTFEditorWindow
    {
        private Assembly[] _assemblies;
        private Assembly _currentAssembly;
        private Vector2 _assemblyScroll = Vector2.zero;
        private string _assemblyFilter = "";

        private Type[] _types;
        private Type _currentType;
        private Vector2 _typeScroll = Vector2.zero;
        private string _typeFilter = "";
        private string _typeFormatName = "";
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
        private string _memberFormatName = "";
        private bool _onlyShowStaticMember = false;
        private bool _showField = true;
        private bool _showMethod = true;
        private bool _showProperty = true;

        private StringBuilder _builder = new StringBuilder();

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/102971712";
        private Assembly CurrentAssembly
        {
            get
            {
                return _currentAssembly;
            }
            set
            {
                if (_currentAssembly == value)
                    return;

                _currentAssembly = value;
                if (_currentAssembly != null)
                {
                    _types = _currentAssembly.GetTypes();
                }
                else
                {
                    _types = null;
                }
                CurrentType = null;
            }
        }
        private Type CurrentType
        {
            get
            {
                return _currentType;
            }
            set
            {
                if (_currentType == value)
                    return;

                _currentType = value;
                if (_currentType != null)
                {
                    _typeFormatName = FormatTypeInfo(_currentType);
                    _fields = _currentType.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _methods = _currentType.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _propertys = _currentType.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }
                else
                {
                    _fields = null;
                    _methods = null;
                    _propertys = null;
                }
                CurrentMethod = null;
                CurrentField = null;
                CurrentProperty = null;
            }
        }
        private FieldInfo CurrentField
        {
            get
            {
                return _currentField;
            }
            set
            {
                if (_currentField == value)
                    return;

                _currentField = value;
                if (_currentField != null)
                {
                    _memberFormatName = FormatFieldInfo(_currentField);
                }
            }
        }
        private MethodInfo CurrentMethod
        {
            get
            {
                return _currentMethod;
            }
            set
            {
                if (_currentMethod == value)
                    return;

                _currentMethod = value;
                if (_currentMethod != null)
                {
                    _parameterInfos = _currentMethod.GetParameters();
                    _memberFormatName = FormatMethodInfo(_currentMethod);
                }
            }
        }
        private PropertyInfo CurrentProperty
        {
            get
            {
                return _currentProperty;
            }
            set
            {
                if (_currentProperty == value)
                    return;

                _currentProperty = value;
                if (_currentProperty != null)
                {
                    _memberFormatName = FormatPropertyInfo(_currentProperty);
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _assemblies = AppDomain.CurrentDomain.GetAssemblies();
            CurrentAssembly = null;
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            GUILayout.FlexibleSpace();
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUILayout.BeginHorizontal();

            OnAssemblyGUI();
            OnTypeGUI();
            OnMemberGUI();

            GUILayout.EndHorizontal();
        }
        private void OnAssemblyGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width / 3 - 5));

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Assembly", "PreLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tag:");
            if (GUILayout.Button("UnityEngine", EditorStyles.miniButton))
            {
                _assemblyFilter = "UnityEngine";
            }
            if (GUILayout.Button("UnityEditor", EditorStyles.miniButton))
            {
                _assemblyFilter = "UnityEditor";
            }
            if (GUILayout.Button("Assembly-CSharp", EditorStyles.miniButton))
            {
                _assemblyFilter = "Assembly-CSharp";
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _assemblyFilter = EditorGUILayout.TextField("", _assemblyFilter, EditorGlobalTools.Styles.SearchTextField);
            if (GUILayout.Button("", string.IsNullOrEmpty(_assemblyFilter) ? EditorGlobalTools.Styles.SearchCancelButtonEmpty : EditorGlobalTools.Styles.SearchCancelButton))
            {
                _assemblyFilter = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            _assemblyScroll = GUILayout.BeginScrollView(_assemblyScroll);

            for (int i = 0; i < _assemblies.Length; i++)
            {
                AssemblyName an = _assemblies[i].GetName();
                if (an.Name.ToLower().Contains(_assemblyFilter.ToLower()))
                {
                    GUI.backgroundColor = CurrentAssembly == _assemblies[i] ? Color.cyan : Color.white;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(an.Name))
                    {
                        CurrentAssembly = _assemblies[i];
                        GUI.FocusControl(null);
                    }
                    GUILayout.EndHorizontal();

                    if (CurrentAssembly == _assemblies[i])
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Version:", GUILayout.Width(80));
                        GUILayout.Label(an.Version.ToString());
                        GUILayout.FlexibleSpace();
                        GUI.enabled = !CurrentAssembly.IsDynamic;
                        if (GUILayout.Button("Open in Explorer", EditorStyles.miniButton, GUILayout.Width(110)))
                        {
                            string args = $"/Select, {CurrentAssembly.Location}";
                            ExecutableToolkit.ExecuteExplorer(args);
                        }
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUI.enabled = !CurrentAssembly.IsDynamic;
                        if (GUILayout.Button("Open in ILSpy", EditorStyles.miniButton, GUILayout.Width(110)))
                        {
                            string ilspyPath = EditorPrefs.GetString(EditorPrefsTable.ILSpyPath, null);
                            bool succeed = ExecutableToolkit.Execute(ilspyPath, $"\"{CurrentAssembly.Location}\"");
                            if (!succeed)
                            {
                                EditorApplication.ExecuteMenuItem("HTFramework/HTFramework Settings...");
                                Log.Error("请在 Setter 面板设置 ILSpy 的启动路径，如未安装 ILSpy，请进入官网下载：http://www.ilspy.net/ 中文版官网：http://www.fishlee.net/soft/ilspy_chs/");
                            }
                        }
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();
                    }
                    GUI.backgroundColor = Color.white;
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        private void OnTypeGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width / 3 - 5));

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Type", "PreLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _onlyShowStaticType = GUILayout.Toggle(_onlyShowStaticType, "Only Show Static");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _typeFilter = EditorGUILayout.TextField("", _typeFilter, EditorGlobalTools.Styles.SearchTextField);
            if (GUILayout.Button("", string.IsNullOrEmpty(_typeFilter) ? EditorGlobalTools.Styles.SearchCancelButtonEmpty : EditorGlobalTools.Styles.SearchCancelButton))
            {
                _typeFilter = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            _typeScroll = GUILayout.BeginScrollView(_typeScroll);

            if (_types != null)
            {
                for (int i = 0; i < _types.Length; i++)
                {
                    if (!IsRejectType(_types[i]) && _types[i].Name.ToLower().Contains(_typeFilter.ToLower()))
                    {
                        if (_onlyShowStaticType && (!_types[i].IsAbstract || !_types[i].IsSealed))
                        {
                            continue;
                        }

                        GUI.backgroundColor = CurrentType == _types[i] ? Color.cyan : Color.white;
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(_types[i].Name))
                        {
                            CurrentType = _types[i];
                            GUI.FocusControl(null);
                        }
                        GUILayout.EndHorizontal();

                        if (CurrentType == _types[i])
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.TextField($"namespace {CurrentType.Namespace}");
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.TextField(_typeFormatName);
                            GUILayout.EndHorizontal();
                        }
                        GUI.backgroundColor = Color.white;
                    }
                }
            }
            
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        private void OnMemberGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(position.width / 3 - 5));

            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

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
            _memberFilter = EditorGUILayout.TextField("", _memberFilter, EditorGlobalTools.Styles.SearchTextField);
            if (GUILayout.Button("", string.IsNullOrEmpty(_memberFilter) ? EditorGlobalTools.Styles.SearchCancelButtonEmpty : EditorGlobalTools.Styles.SearchCancelButton))
            {
                _memberFilter = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            _memberScroll = GUILayout.BeginScrollView(_memberScroll);

            if (_showField && _fields != null)
            {
                for (int i = 0; i < _fields.Length; i++)
                {
                    if (!IsRejectField(_fields[i]) && _fields[i].Name.ToLower().Contains(_memberFilter.ToLower()))
                    {
                        if (_onlyShowStaticMember && !_fields[i].IsStatic)
                        {
                            continue;
                        }

                        GUI.backgroundColor = CurrentField == _fields[i] ? Color.cyan : Color.white;
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button($"[Field]  {_fields[i].Name}"))
                        {
                            CurrentField = _fields[i];
                            CurrentMethod = null;
                            CurrentProperty = null;
                            GUI.FocusControl(null);
                        }
                        GUILayout.EndHorizontal();

                        if (CurrentField == _fields[i])
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.TextField(_memberFormatName);
                            GUILayout.EndHorizontal();
                        }
                        GUI.backgroundColor = Color.white;
                    }
                }
            }
            if (_showMethod && _methods != null)
            {
                for (int i = 0; i < _methods.Length; i++)
                {
                    if (!IsRejectMethod(_methods[i]) && _methods[i].Name.ToLower().Contains(_memberFilter.ToLower()))
                    {
                        if (_onlyShowStaticMember && !_methods[i].IsStatic)
                        {
                            continue;
                        }

                        GUI.backgroundColor = CurrentMethod == _methods[i] ? Color.cyan : Color.white;
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button($"[Method]  {_methods[i].Name}"))
                        {
                            CurrentMethod = _methods[i];
                            CurrentField = null;
                            CurrentProperty = null;
                            GUI.FocusControl(null);
                        }
                        GUILayout.EndHorizontal();

                        if (CurrentMethod == _methods[i])
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.TextField(_memberFormatName);
                            GUILayout.EndHorizontal();
                        }
                        GUI.backgroundColor = Color.white;
                    }
                }
            }
            if (_showProperty && _propertys != null)
            {
                for (int i = 0; i < _propertys.Length; i++)
                {
                    if (!IsRejectProperty(_propertys[i]) && _propertys[i].Name.ToLower().Contains(_memberFilter.ToLower()))
                    {
                        if (_onlyShowStaticMember && !IsStatic(_propertys[i]))
                        {
                            continue;
                        }

                        GUI.backgroundColor = CurrentProperty == _propertys[i] ? Color.cyan : Color.white;
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button($"[Property]  {_propertys[i].Name}"))
                        {
                            CurrentProperty = _propertys[i];
                            CurrentMethod = null;
                            CurrentField = null;
                            GUI.FocusControl(null);
                        }
                        GUILayout.EndHorizontal();

                        if (CurrentProperty == _propertys[i])
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.TextField(_memberFormatName);
                            GUILayout.EndHorizontal();
                        }
                        GUI.backgroundColor = Color.white;
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
                return $"private {property.PropertyType.Name} {property.Name} " + "{}";
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