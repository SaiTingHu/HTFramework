using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class CoroutinerTrackerWindow : HTFEditorWindow
    {
        private CoroutinerManager _coroutiner;
        private ICoroutinerHelper _helper;
        private Dictionary<Delegate, bool> _enumerators = new Dictionary<Delegate, bool>();
        private CoroutinerManager.CoroutineEnumerator _currentEnumerator;
        private string _currentStackTrace;
        private int _firstLineBlank = 40;
        private int _IDWidth = 300;
        private int _stateWidth = 100;
        private int _creationTimeWidth = 100;
        private int _stoppingTimeWidth = 100;
        private int _elapsedTimeWidth = 100;
        private int _rerunNumberWidth = 100;
        private int _stackTraceHeight = 200;
        private Color _bgColor = new Color(0.7f, 0.7f, 0.7f, 1);
        private Vector2 _scrollContent = Vector2.zero;
        private Vector2 _scrollStackTrace = Vector2.zero;

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/91492838";

        public void Init(CoroutinerManager coroutiner)
        {
            _coroutiner = coroutiner;
            _helper = _coroutiner.GetType().GetProperty("_helper", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_coroutiner) as ICoroutinerHelper;
        }
        private void Update()
        {
            if (_coroutiner == null || _helper == null)
            {
                Close();
            }
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            GUILayout.Label("No.", EditorStyles.toolbarButton, GUILayout.Width(_firstLineBlank));
            GUILayout.Label("ID", EditorStyles.toolbarButton, GUILayout.Width(_IDWidth));
            GUILayout.Label("State", EditorStyles.toolbarButton, GUILayout.Width(_stateWidth));
            GUILayout.Label("Creation Time", EditorStyles.toolbarButton, GUILayout.Width(_creationTimeWidth));
            GUILayout.Label("Stopping Time", EditorStyles.toolbarButton, GUILayout.Width(_stoppingTimeWidth));
            GUILayout.Label("Elapsed Time", EditorStyles.toolbarButton, GUILayout.Width(_elapsedTimeWidth));
            GUILayout.Label("Rerun Number", EditorStyles.toolbarButton, GUILayout.Width(_rerunNumberWidth));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear Not Running", EditorStyles.toolbarButton, GUILayout.Width(120)))
            {
                _coroutiner.ClearNotRunning();
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUI.color = _bgColor;

            ContentGUI();
            StackTraceGUI();
        }
        private void ContentGUI()
        {
            int index1 = 1;
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);
            _scrollContent = GUILayout.BeginScrollView(_scrollContent);
            foreach (var executor in _helper.Warehouse)
            {
                if (!_enumerators.ContainsKey(executor.Key))
                {
                    _enumerators.Add(executor.Key, false);
                }

                GUI.color = Color.white;
                GUILayout.BeginHorizontal("IN BigTitle");
                _enumerators[executor.Key] = EditorGUILayout.Foldout(_enumerators[executor.Key], $"{index1}.{executor.Key.Target} -> {executor.Key.Method}  [{executor.Value.Count}]", true);
                GUILayout.EndHorizontal();
                GUI.color = _bgColor;
                index1 += 1;

                if (_enumerators[executor.Key])
                {
                    int index2 = 1;
                    foreach (var enumerator in executor.Value)
                    {
                        GUI.color = _currentEnumerator == enumerator ? Color.cyan : Color.white;
                        GUILayout.BeginHorizontal("Badge");
                        GUILayout.Label(index2.ToString(), GUILayout.Width(_firstLineBlank));
                        GUILayout.Label(enumerator.ID, GUILayout.Width(_IDWidth));
                        GUILayout.Label(enumerator.State.ToString(), GUILayout.Width(_stateWidth));
                        GUILayout.Label(enumerator.CreationTime.ToString("mm:ss:fff"), GUILayout.Width(_creationTimeWidth));
                        if (enumerator.State == CoroutinerManager.CoroutineState.Running)
                        {
                            GUILayout.Label("--:--:--", GUILayout.Width(_stoppingTimeWidth));
                            GUILayout.Label("-:---", GUILayout.Width(_elapsedTimeWidth));
                        }
                        else
                        {
                            GUILayout.Label(enumerator.StoppingTime.ToString("mm:ss:fff"), GUILayout.Width(_stoppingTimeWidth));
                            GUILayout.Label(enumerator.ElapsedTime.ToString("F3"), GUILayout.Width(_elapsedTimeWidth));
                        }
                        GUILayout.Label(enumerator.RerunNumber.ToString(), GUILayout.Width(_rerunNumberWidth));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Rerun", EditorStyles.miniButtonLeft, GUILayout.Width(50)))
                        {
                            enumerator.RerunInEditor();
                        }
                        GUI.enabled = enumerator.State == CoroutinerManager.CoroutineState.Running;
                        if (GUILayout.Button("Stop", EditorStyles.miniButtonRight, GUILayout.Width(50)))
                        {
                            enumerator.Stop();
                        }
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();
                        
                        if (Event.current != null && Event.current.rawType == EventType.MouseDown)
                        {
                            Rect rect = GUILayoutUtility.GetLastRect();
                            if (rect.Contains(Event.current.mousePosition))
                            {
                                MouseDownEnumerator(enumerator);
                                Event.current.Use();
                            }
                        }
                        index2 += 1;

                        GUILayout.Space(2);
                    }
                    GUI.color = _bgColor;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        private void StackTraceGUI()
        {
            if (_currentEnumerator != null)
            {
                GUILayout.BeginVertical(EditorGlobalTools.Styles.Box, GUILayout.Height(_stackTraceHeight));
                _scrollStackTrace = GUILayout.BeginScrollView(_scrollStackTrace);
                
                EditorGUILayout.TextArea(_currentStackTrace);
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }

        private void MouseDownEnumerator(CoroutinerManager.CoroutineEnumerator enumerator)
        {
            _currentEnumerator = enumerator;
            _currentStackTrace = $"{_currentEnumerator.ID}\r\n\r\n{GetFullStackTraceInfo(_currentEnumerator.StackTraceInfo)}";
            Repaint();
        }
        private string GetFullStackTraceInfo(StackTrace trace)
        {
            string assetsPath = Application.dataPath.Replace("/", "\\");
            StringToolkit.BeginConcat();
            StackFrame[] frames = trace.GetFrames();
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i].GetMethod().Name == "Run" || frames[i].GetMethod().Name == "Rerun")
                {
                    continue;
                }
                StringToolkit.Concat(frames[i].GetMethod().DeclaringType.FullName);
                StringToolkit.Concat(".");
                StringToolkit.Concat(frames[i].GetMethod().Name);
                StringToolkit.Concat(" (");
                StringToolkit.Concat("at Assets");
                StringToolkit.Concat(frames[i].GetFileName().Replace(assetsPath, ""));
                StringToolkit.Concat(":");
                StringToolkit.Concat(frames[i].GetFileLineNumber().ToString());
                StringToolkit.Concat(")");
                StringToolkit.Concat("\r\n");
            }
            return StringToolkit.EndConcat();
        }
    }
}