using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 框架脚本执行顺序查看器
    /// </summary>
    internal sealed class ExecutionOrder : HTFEditorWindow
    {
        private Dictionary<string, Class> _classes = new Dictionary<string, Class>();
        private List<Function> _functions = new List<Function>();
        private GUIContent _moduleGC;
        private GUIContent _csGC;
        private Vector2 _scrollClass;
        private Vector2 _scrollFunction;

        protected override bool IsEnableTitleGUI => false;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            _classes.Clear();
            _classes.Add("LicenserBase", new Class("Main.Licenser", "LicenserBase", true, Color.white));
            _classes.Add("MainDataBase", new Class("Main.MainData", "MainDataBase", true, Color.white));
            _classes.Add("InternalModuleBase", new Class("Internal Module", "InternalModuleBase", true, Color.white));
            _classes.Add("IInternalModuleHelper", new Class("Internal Module Helper", "IInternalModuleHelper", true, Color.white));
            _classes.Add("CustomModuleBase", new Class("Custom Module", "CustomModuleBase", true, Color.white));
            _classes.Add("FSMDataBase", new Class("FSM Data", "FSMDataBase", true, Color.white));
            _classes.Add("FiniteStateBase", new Class("FSM State", "FiniteStateBase", true, Color.white));
            _classes.Add("ProcedureBase", new Class("Procedure", "ProcedureBase", true, Color.white));
            _classes.Add("MonoBehaviour", new Class(null, "MonoBehaviour", true, Color.white));

            _functions.Clear();
            _functions.Add(new Function("LicenserBase", "OnInit"));
            _functions.Add(new Function("MainDataBase", "OnInit"));
            _functions.Add(new Function("InternalModuleBase", "OnInit"));
            _functions.Add(new Function("IInternalModuleHelper", "OnInit"));
            _functions.Add(new Function("CustomModuleBase", "OnInit"));
            _functions.Add(new Function("MonoBehaviour", "Awake"));
            _functions.Add(new Function("LicenserBase", "Checking"));
            _functions.Add(new Function("MainDataBase", "OnReady"));
            _functions.Add(new Function("InternalModuleBase", "OnReady"));
            _functions.Add(new Function("IInternalModuleHelper", "OnReady"));
            _functions.Add(new Function("CustomModuleBase", "OnReady"));
            _functions.Add(new Function("ProcedureBase", "OnInit"));
            _functions.Add(new Function("FSMDataBase", "OnInit"));
            _functions.Add(new Function("FiniteStateBase", "OnInit"));
            _functions.Add(new Function("MonoBehaviour", "Start"));
            _functions.Add(new Function("InternalModuleBase", "OnUpdate"));
            _functions.Add(new Function("IInternalModuleHelper", "OnUpdate"));
            _functions.Add(new Function("CustomModuleBase", "OnUpdate"));
            _functions.Add(new Function("ProcedureBase", "OnUpdate"));
            _functions.Add(new Function("ProcedureBase", "OnUpdateSecond"));
            _functions.Add(new Function("MonoBehaviour", "Update"));
            _functions.Add(new Function("InternalModuleBase", "OnTerminate"));
            _functions.Add(new Function("IInternalModuleHelper", "OnTerminate"));
            _functions.Add(new Function("CustomModuleBase", "OnTerminate"));
            _functions.Add(new Function("FiniteStateBase", "OnTerminate"));
            _functions.Add(new Function("FSMDataBase", "OnTerminate"));
            _functions.Add(new Function("MonoBehaviour", "OnDestroy"));

            _moduleGC = EditorGUIUtility.IconContent("Prefab Icon");
            _csGC = EditorGUIUtility.IconContent("cs Script Icon");
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUILayout.BeginHorizontal();

            ClassGUI();

            GUILayout.Space(5);

            GUILayout.Box("", "DopesheetBackground", GUILayout.Width(5), GUILayout.ExpandHeight(true));

            GUILayout.Space(5);

            FunctionGUI();

            GUILayout.EndHorizontal();
        }
        private void ClassGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(320));
            _scrollClass = GUILayout.BeginScrollView(_scrollClass);

            foreach (var cla in _classes)
            {
                GUIContent gc = string.IsNullOrEmpty(cla.Value.ModuleName) ? _csGC : _moduleGC;
                gc.text = string.IsNullOrEmpty(cla.Value.ModuleName) ? cla.Value.ClassName : cla.Value.ModuleName;

                GUILayout.BeginHorizontal();
                GUI.color = cla.Value.IsDisplay ? Color.white : Color.gray;
                cla.Value.IsDisplay = GUILayout.Toggle(cla.Value.IsDisplay, gc, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(200));
                cla.Value.DisplayColor = EditorGUILayout.ColorField(cla.Value.DisplayColor, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(100));
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void FunctionGUI()
        {
            GUILayout.BeginVertical();
            _scrollFunction = GUILayout.BeginScrollView(_scrollFunction);

            for (int i = 0; i < _functions.Count; i++)
            {
                if (_classes[_functions[i].ClassName].IsDisplay)
                {
                    GUILayout.BeginHorizontal();
                    GUI.backgroundColor = _classes[_functions[i].ClassName].DisplayColor;
                    GUILayout.Label(_functions[i].ClassName + "." + _functions[i].FunctionName, "wordwrapminibutton", GUILayout.Height(20));
                    GUI.backgroundColor = Color.white;
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private class Class
        {
            public string ModuleName;
            public string ClassName;
            public bool IsDisplay;
            public Color DisplayColor;

            public Class(string moduleName, string className, bool isDisplay, Color displayColor)
            {
                ModuleName = moduleName;
                ClassName = className;
                IsDisplay = isDisplay;
                DisplayColor = displayColor;
            }
        }
        private class Function
        {
            public string ClassName;
            public string FunctionName;

            public Function(string className, string functionName)
            {
                ClassName = className;
                FunctionName = functionName;
            }
        }
    }
}