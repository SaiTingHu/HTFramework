using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public sealed class CoroutinerTrackerWindow : EditorWindow
    {
        private Coroutiner _coroutiner;
        private Dictionary<Delegate, bool> _enumerators = new Dictionary<Delegate, bool>();
        private Coroutiner.CoroutineEnumerator _currentEnumerator;
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

        public void Init(Coroutiner coroutiner)
        {
            _coroutiner = coroutiner;
        }
        private void OnGUI()
        {
            GUI.color = _bgColor;

            TitleGUI();
            ContentGUI();
            StackTraceGUI();
        }
        private void Update()
        {
            if (!_coroutiner)
            {
                Close();
            }
        }
        private void TitleGUI()
        {
            GUILayout.BeginHorizontal("Toolbar");
            GUILayout.Label("No.", "toolbarbutton", GUILayout.Width(_firstLineBlank));
            GUILayout.Label("ID", "toolbarbutton", GUILayout.Width(_IDWidth));
            GUILayout.Label("State", "toolbarbutton", GUILayout.Width(_stateWidth));
            GUILayout.Label("Creation Time", "toolbarbutton", GUILayout.Width(_creationTimeWidth));
            GUILayout.Label("Stopping Time", "toolbarbutton", GUILayout.Width(_stoppingTimeWidth));
            GUILayout.Label("Elapsed Time", "toolbarbutton", GUILayout.Width(_elapsedTimeWidth));
            GUILayout.Label("Rerun Number", "toolbarbutton", GUILayout.Width(_rerunNumberWidth));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear Not Running", "toolbarbutton", GUILayout.Width(120)))
            {
                _coroutiner.ClearNotRunning();
            }
            GUILayout.EndHorizontal();
        }
        private void ContentGUI()
        {
            int index1 = 1;
            GUILayout.BeginVertical("Box");
            _scrollContent = GUILayout.BeginScrollView(_scrollContent);
            foreach (KeyValuePair<Delegate, List<Coroutiner.CoroutineEnumerator>> executor in _coroutiner.Warehouse)
            {
                if (!_enumerators.ContainsKey(executor.Key))
                {
                    _enumerators.Add(executor.Key, false);
                }

                GUI.color = Color.white;
                GUILayout.BeginHorizontal("IN BigTitle");
                _enumerators[executor.Key] = EditorGUILayout.Foldout(_enumerators[executor.Key], string.Format("{0}.{1} -> {2}  [{3}]", index1, executor.Key.Target, executor.Key.Method, executor.Value.Count), true);
                GUILayout.EndHorizontal();
                GUI.color = _bgColor;
                index1 += 1;

                if (_enumerators[executor.Key])
                {
                    int index2 = 1;
                    foreach (Coroutiner.CoroutineEnumerator enumerator in executor.Value)
                    {
                        GUI.color = _currentEnumerator == enumerator ? Color.cyan : Color.white;
                        GUILayout.BeginHorizontal("Badge");
                        GUILayout.Label(index2.ToString(), GUILayout.Width(_firstLineBlank));
                        GUILayout.Label(enumerator.ID, GUILayout.Width(_IDWidth));
                        GUILayout.Label(enumerator.State.ToString(), GUILayout.Width(_stateWidth));
                        GUILayout.Label(enumerator.CreationTime.ToString("mm:ss:fff"), GUILayout.Width(_creationTimeWidth));
                        if (enumerator.State == Coroutiner.CoroutineState.Running)
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
                        if (GUILayout.Button("Rerun", "Minibuttonleft", GUILayout.Width(50)))
                        {
                            enumerator.RerunInEditor();
                        }
                        GUI.enabled = enumerator.State == Coroutiner.CoroutineState.Running;
                        if (GUILayout.Button("Stop", "Minibuttonright", GUILayout.Width(50)))
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
                GUILayout.BeginVertical("Box", GUILayout.Height(_stackTraceHeight));
                _scrollStackTrace = GUILayout.BeginScrollView(_scrollStackTrace);
                
                EditorGUILayout.TextArea(_currentStackTrace);
                
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }

        private void MouseDownEnumerator(Coroutiner.CoroutineEnumerator enumerator)
        {
            _currentEnumerator = enumerator;
            _currentStackTrace = _currentEnumerator.ID + "\r\n\r\n" + GetFullStackTraceInfo(_currentEnumerator.StackTraceInfo);
            Repaint();
        }
        private string GetFullStackTraceInfo(StackTrace trace)
        {
            string assetsPath = Application.dataPath.Replace("/", "\\");
            StringBuilder info = new StringBuilder();
            StackFrame[] frames = trace.GetFrames();
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i].GetMethod().Name == "Run" || frames[i].GetMethod().Name == "Rerun")
                {
                    continue;
                }
                info.Append(frames[i].GetMethod().DeclaringType.FullName);
                info.Append(".");
                info.Append(frames[i].GetMethod().Name);
                info.Append(" (");
                info.Append("at Assets");
                info.Append(frames[i].GetFileName().Replace(assetsPath, ""));
                info.Append(":");
                info.Append(frames[i].GetFileLineNumber());
                info.Append(")");
                info.Append("\r\n");
            }
            return info.ToString();
        }
    }
}