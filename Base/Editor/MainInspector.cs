using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(Main))]
    public sealed class MainInspector : Editor
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

            #region MainData
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("MainData");
            if (GUILayout.Button(_target.MainDataType, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                Assembly assembly = Assembly.GetAssembly(typeof(MainData));
                Type[] types = assembly.GetTypes();
                gm.AddItem(new GUIContent("<None>"), _target.MainDataType == "<None>", () =>
                {
                    Undo.RecordObject(target, "Set Main Data");
                    _target.MainDataType = "<None>";
                    this.HasChanged();
                });
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].BaseType == typeof(MainData))
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), _target.MainDataType == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set Main Data");
                            _target.MainDataType = types[j].FullName;
                            this.HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            #endregion

            #region License
            GUILayout.BeginVertical("Box");
            
            GUILayout.BeginHorizontal();
            this.Toggle(_target.IsPermanentLicense, out _target.IsPermanentLicense, "Permanent License");
            GUILayout.EndHorizontal();

            if (!_target.IsPermanentLicense)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Prompt:", "BoldLabel");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                this.TextField(_target.EndingPrompt, out _target.EndingPrompt);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Ending Time:", "BoldLabel");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Year:", GUILayout.Width(45));
                int year = EditorGUILayout.IntField(_target.Year, GUILayout.Width(50));
                if (year != _target.Year)
                {
                    Undo.RecordObject(target, "Set Year");
                    _target.Year = year;
                    CorrectDateTime();
                    this.HasChanged();
                }
                if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                {
                    Undo.RecordObject(target, "Set Year");
                    _target.Year += 1;
                    CorrectDateTime();
                    this.HasChanged();
                }
                if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                {
                    Undo.RecordObject(target, "Set Year");
                    _target.Year -= 1;
                    CorrectDateTime();
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Month:", GUILayout.Width(45));
                int month = EditorGUILayout.IntField(_target.Month, GUILayout.Width(50));
                if (month != _target.Month)
                {
                    Undo.RecordObject(target, "Set Month");
                    _target.Month = month;
                    CorrectDateTime();
                    this.HasChanged();
                }
                if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                {
                    Undo.RecordObject(target, "Set Month");
                    _target.Month += 1;
                    CorrectDateTime();
                    this.HasChanged();
                }
                if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                {
                    Undo.RecordObject(target, "Set Month");
                    _target.Month -= 1;
                    CorrectDateTime();
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Day:", GUILayout.Width(45));
                int day = EditorGUILayout.IntField(_target.Day, GUILayout.Width(50));
                if (day != _target.Day)
                {
                    Undo.RecordObject(target, "Set Day");
                    _target.Day = day;
                    CorrectDateTime();
                    this.HasChanged();
                }
                if (GUILayout.Button("", "OL Plus", GUILayout.Width(15)))
                {
                    Undo.RecordObject(target, "Set Day");
                    _target.Day += 1;
                    CorrectDateTime();
                    this.HasChanged();
                }
                if (GUILayout.Button("", "OL Minus", GUILayout.Width(15)))
                {
                    Undo.RecordObject(target, "Set Day");
                    _target.Day -= 1;
                    CorrectDateTime();
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Now", "MiniButton"))
                {
                    Undo.RecordObject(target, "Set Now");
                    _target.Year = DateTime.Now.Year;
                    _target.Month = DateTime.Now.Month;
                    _target.Day = DateTime.Now.Day;
                    CorrectDateTime();
                    this.HasChanged();
                }
                if (GUILayout.Button("2 Months Later", "MiniButton"))
                {
                    Undo.RecordObject(target, "Set 2 Months Later");
                    _target.Month += 2;
                    CorrectDateTime();
                    this.HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            #endregion
        }

        /// <summary>
        /// 纠正时间
        /// </summary>
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
