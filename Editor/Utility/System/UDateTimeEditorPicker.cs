using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UDateTime 编辑器日期拾取器
    /// </summary>
    internal sealed class UDateTimeEditorPicker : HTFEditorWindow
    {
        public static void OpenWindow(UDateTimeDrawer drawer)
        {
            UDateTimeEditorPicker window = GetWindow<UDateTimeEditorPicker>(true, "DateTime Picker", true);
            window._drawer = drawer;
            window._select = drawer.GetData();
            window._showYear = window._select.Year;
            window._showMonth = window._select.Month;
            window.UpdateShowDay();
            window.minSize = new Vector2(250, 330);
            window.maxSize = new Vector2(250, 330);
            window.Show();
        }

        protected override bool IsEnableTitleGUI => false;

        private string[] _formats = new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy年MM月dd日 HH:mm:ss", "yyyy年MM月dd日" };
        private GUIContent _lastYearGC;
        private GUIContent _nextYearGC;
        private GUIContent _lastMonthGC;
        private GUIContent _nextMonthGC;
        private GUIContent _resetGC;
        private UDateTimeDrawer _drawer;
        private UDateTime _select;
        private int _showYear;
        private int _showMonth;
        private int _showDays;
        private DayOfWeek _firstDayWeek;

        protected override void OnEnable()
        {
            base.OnEnable();

            _lastYearGC = new GUIContent();
            _lastYearGC.image = EditorGUIUtility.IconContent("Animation.FirstKey").image;
            _lastYearGC.tooltip = "上一个年份";
            _nextYearGC = new GUIContent();
            _nextYearGC.image = EditorGUIUtility.IconContent("Animation.LastKey").image;
            _nextYearGC.tooltip = "下一个年份";
            _lastMonthGC = new GUIContent();
            _lastMonthGC.image = EditorGUIUtility.IconContent("Animation.PrevKey").image;
            _lastMonthGC.tooltip = "上一个月份";
            _nextMonthGC = new GUIContent();
            _nextMonthGC.image = EditorGUIUtility.IconContent("Animation.NextKey").image;
            _nextMonthGC.tooltip = "下一个月份";
            _resetGC = new GUIContent();
            _resetGC.image = EditorGUIUtility.IconContent("Preset.Context").image;
            _resetGC.tooltip = "重置格式";

            Selection.selectionChanged += Close;
        }
        private void OnDisable()
        {
            Selection.selectionChanged -= Close;
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUILayout.BeginHorizontal("AC BoldHeader");
            if (GUILayout.Button(_lastYearGC, EditorStyles.iconButton))
            {
                _showYear -= 1;
                if (_showYear < 1) _showYear = 1;
                UpdateShowDay();
            }
            GUILayout.Space(10);
            if (GUILayout.Button(_lastMonthGC, EditorStyles.iconButton))
            {
                _showMonth -= 1;
                if (_showMonth < 1) _showMonth = 12;
                UpdateShowDay();
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{_showYear} 年 {_showMonth} 月", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(_nextMonthGC, EditorStyles.iconButton))
            {
                _showMonth += 1;
                if (_showMonth > 12) _showMonth = 1;
                UpdateShowDay();
            }
            GUILayout.Space(10);
            if (GUILayout.Button(_nextYearGC, EditorStyles.iconButton))
            {
                _showYear += 1;
                if (_showYear > 9999) _showYear = 9999;
                UpdateShowDay();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("DD HeaderStyle");
            GUILayout.Label("日", "CenteredLabel");
            GUILayout.Label("一", "CenteredLabel");
            GUILayout.Label("二", "CenteredLabel");
            GUILayout.Label("三", "CenteredLabel");
            GUILayout.Label("四", "CenteredLabel");
            GUILayout.Label("五", "CenteredLabel");
            GUILayout.Label("六", "CenteredLabel");
            GUILayout.EndHorizontal();

            int day = 1;
            for (int i = 0; i < 6; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < 7; j++)
                {
                    if ((i == 0 && j < (int)_firstDayWeek) || day > _showDays)
                    {
                        GUI.enabled = false;
                        GUILayout.Button("", GUILayout.Width(32), GUILayout.Height(30));
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.backgroundColor = (_showYear == _select.Year && _showMonth == _select.Month && day == _select.Day) ? Color.green : Color.white;
                        if (GUILayout.Button(day.ToString(), GUILayout.Width(32), GUILayout.Height(30)))
                        {
                            _select.Year = _showYear;
                            _select.Month = _showMonth;
                            _select.Day = day;
                        }
                        GUI.backgroundColor = Color.white;
                        day++;
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Time", GUILayout.Width(50));
            _select.Hour = EditorGUILayout.IntField("", _select.Hour, GUILayout.Width(30));
            _select.Hour = Mathf.Clamp(_select.Hour, 0, 23);
            GUILayout.Label(":", GUILayout.Width(10));
            _select.Minute = EditorGUILayout.IntField("", _select.Minute, GUILayout.Width(30));
            _select.Minute = Mathf.Clamp(_select.Minute, 0, 59);
            GUILayout.Label(":", GUILayout.Width(10));
            _select.Second = EditorGUILayout.IntField("", _select.Second, GUILayout.Width(30));
            _select.Second = Mathf.Clamp(_select.Second, 0, 59);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Format", GUILayout.Width(50));
            _select.Format = EditorGUILayout.TextField("", _select.Format, GUILayout.Width(170));
            if (GUILayout.Button(_resetGC, EditorStyles.iconButton))
            {
                GenericMenu gm = new GenericMenu();
                for (int i = 0; i < _formats.Length; i++)
                {
                    string format = _formats[i];
                    gm.AddItem(new GUIContent(format), format == _select.Format, () =>
                    {
                        _select.Format = format;
                    });
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Now"))
            {
                _showYear = DateTime.Now.Year;
                _showMonth = DateTime.Now.Month;
                _select.FromDateTime(DateTime.Now);
                UpdateShowDay();
            }
            if (GUILayout.Button("Save"))
            {
                _drawer.SaveData(_select);
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private void UpdateShowDay()
        {
            _showDays = DateTime.DaysInMonth(_showYear, _showMonth);
            _firstDayWeek = new DateTime(_showYear, _showMonth, 1).DayOfWeek;
        }
    }
}