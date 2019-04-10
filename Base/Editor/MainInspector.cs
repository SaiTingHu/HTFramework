using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Main))]
    public class MainInspector : Editor
    {
        private Main _target;

        private void OnEnable()
        {
            _target = target as Main;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("HTFramework Main Module!", MessageType.Info);
            GUILayout.EndHorizontal();

            #region License
            GUILayout.BeginVertical("Box");
            
            GUILayout.BeginHorizontal();
            GUI.color = _target.IsPermanentLicense ? Color.white : Color.gray;
            _target.IsPermanentLicense = GUILayout.Toggle(_target.IsPermanentLicense, "Permanent License");
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            if (!_target.IsPermanentLicense)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Prompt:", "BoldLabel");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _target.EndingPrompt = EditorGUILayout.TextField(_target.EndingPrompt);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Ending Time:", "BoldLabel");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Year:", GUILayout.Width(45));
                int year = EditorGUILayout.IntField(_target.Year, GUILayout.Width(50));
                if (year != _target.Year)
                {
                    _target.Year = year;
                    CorrectDateTime();
                }
                if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                {
                    _target.Year += 1;
                    CorrectDateTime();
                }
                if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                {
                    _target.Year -= 1;
                    CorrectDateTime();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Month:", GUILayout.Width(45));
                int month = EditorGUILayout.IntField(_target.Month, GUILayout.Width(50));
                if (month != _target.Month)
                {
                    _target.Month = month;
                    CorrectDateTime();
                }
                if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                {
                    _target.Month += 1;
                    CorrectDateTime();
                }
                if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                {
                    _target.Month -= 1;
                    CorrectDateTime();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Day:", GUILayout.Width(45));
                int day = EditorGUILayout.IntField(_target.Day, GUILayout.Width(50));
                if (day != _target.Day)
                {
                    _target.Day = day;
                    CorrectDateTime();
                }
                if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                {
                    _target.Day += 1;
                    CorrectDateTime();
                }
                if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                {
                    _target.Day -= 1;
                    CorrectDateTime();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Now", "MiniButton"))
                {
                    _target.Year = DateTime.Now.Year;
                    _target.Month = DateTime.Now.Month;
                    _target.Day = DateTime.Now.Day;
                    CorrectDateTime();
                }
                if (GUILayout.Button("2 Months Later", "MiniButton"))
                {
                    _target.Month += 2;
                    CorrectDateTime();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            #endregion

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }

        private void CorrectDateTime()
        {
            if (_target.Year < DateTime.Now.Year)
            {
                _target.Year = DateTime.Now.Year;
            }
            else if (_target.Year > 9999)
            {
                _target.Year = 9999;
            }

            if (_target.Month < 1)
            {
                _target.Month = 1;
            }
            else if (_target.Month > 12)
            {
                _target.Month = 12;
            }

            if (_target.Month == 2)
            {
                if (_target.Day < 1)
                {
                    _target.Day = 1;
                }
                else if (_target.Day > 28)
                {
                    _target.Day = 28;
                }
            }
            else
            {
                if (_target.Day < 1)
                {
                    _target.Day = 1;
                }
                else if (_target.Day > 30)
                {
                    _target.Day = 30;
                }
            }
        }
    }
}
