using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// ¿ò¼Ü½Å±¾Ö´ÐÐË³Ðò²é¿´Æ÷
    /// </summary>
    internal sealed class ExecutionOrder : HTFEditorWindow
    {
        private Texture _classIcon;
        private Dictionary<string, Class> _classes = new Dictionary<string, Class>();
        private List<Function> _functions = new List<Function>();
        private Vector2 _scrollClass;
        private Vector2 _scrollFunction;

        protected override bool IsEnableTitleGUI => false;

        protected override void OnEnable()
        {
            base.OnEnable();

            _classIcon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/HTScriptIcon.png");

            _classes.Clear();
            _classes.Add("LicenserBase", new Class("LicenserBase", true, Color.white));
            _classes.Add("MainDataBase", new Class("MainDataBase", true, Color.white));
            _classes.Add("InternalModuleBase", new Class("InternalModuleBase", true, Color.white));
            _classes.Add("IInternalModuleHelper", new Class("IInternalModuleHelper", true, Color.white));
            _classes.Add("CustomModuleBase", new Class("CustomModuleBase", true, Color.white));
            _classes.Add("FSMDataBase", new Class("FSMDataBase", true, Color.white));
            _classes.Add("FiniteStateBase", new Class("FiniteStateBase", true, Color.white));
            _classes.Add("ProcedureBase", new Class("ProcedureBase", true, Color.white));
            _classes.Add("MonoBehaviour", new Class("MonoBehaviour", true, Color.white));

            _functions.Clear();
            _functions.Add(new Function("LicenserBase", "OnInitialization"));
            _functions.Add(new Function("MainDataBase", "OnInitialization"));
            _functions.Add(new Function("InternalModuleBase", "OnInitialization"));
            _functions.Add(new Function("IInternalModuleHelper", "OnInitialization"));
            _functions.Add(new Function("CustomModuleBase", "OnInitialization"));
            _functions.Add(new Function("MonoBehaviour", "Awake"));
            _functions.Add(new Function("LicenserBase", "Checking"));
            _functions.Add(new Function("MainDataBase", "OnPreparatory"));
            _functions.Add(new Function("InternalModuleBase", "OnPreparatory"));
            _functions.Add(new Function("IInternalModuleHelper", "OnPreparatory"));
            _functions.Add(new Function("CustomModuleBase", "OnPreparatory"));
            _functions.Add(new Function("ProcedureBase", "OnInit"));
            _functions.Add(new Function("FSMDataBase", "OnInit"));
            _functions.Add(new Function("FiniteStateBase", "OnInit"));
            _functions.Add(new Function("MonoBehaviour", "Start"));
            _functions.Add(new Function("InternalModuleBase", "OnRefresh"));
            _functions.Add(new Function("IInternalModuleHelper", "OnRefresh"));
            _functions.Add(new Function("CustomModuleBase", "OnRefresh"));
            _functions.Add(new Function("ProcedureBase", "OnUpdate"));
            _functions.Add(new Function("ProcedureBase", "OnUpdateSecond"));
            _functions.Add(new Function("MonoBehaviour", "Update"));
            _functions.Add(new Function("InternalModuleBase", "OnTermination"));
            _functions.Add(new Function("IInternalModuleHelper", "OnTermination"));
            _functions.Add(new Function("CustomModuleBase", "OnTermination"));
            _functions.Add(new Function("FiniteStateBase", "OnTermination"));
            _functions.Add(new Function("FSMDataBase", "OnTermination"));
            _functions.Add(new Function("MonoBehaviour", "OnDestroy"));
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
                GUIContent gUIContent = new GUIContent(cla.Value.ClassName);
                gUIContent.image = _classIcon;

                GUILayout.BeginHorizontal();
                GUI.color = cla.Value.IsDisplay ? Color.white : Color.gray;
                cla.Value.IsDisplay = GUILayout.Toggle(cla.Value.IsDisplay, gUIContent, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(200));
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
            public string ClassName;
            public bool IsDisplay;
            public Color DisplayColor;

            public Class(string className, bool isDisplay, Color displayColor)
            {
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