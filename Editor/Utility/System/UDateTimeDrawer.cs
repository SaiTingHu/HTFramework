using System;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomPropertyDrawer(typeof(UDateTime))]
    internal sealed class UDateTimeDrawer : PropertyDrawer
    {
        private GUIStyle _dateTimeBtnGS;
        private GUIContent _dateTimeIconGC;
        private SerializedProperty _year;
        private SerializedProperty _month;
        private SerializedProperty _day;
        private SerializedProperty _hour;
        private SerializedProperty _minute;
        private SerializedProperty _second;
        private SerializedProperty _format;
        private int _yearCache;
        private int _monthCache;
        private int _dayCache;
        private int _hourCache;
        private int _minuteCache;
        private int _secondCache;
        private string _formatCache;
        private string _dateTimeStr;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.targetObjects.Length > 1)
            {
                EditorGUI.HelpBox(position, "UDateTime cannot be multi-edited.", MessageType.None);
                return;
            }

            InitDrawer(property);
            UpdateDateTimeStr();

            float width = EditorGUIUtility.labelWidth + 2;

            Rect rect = position;
            rect.Set(position.x, position.y, width, position.height);
            EditorGUI.LabelField(rect, label);

            rect.Set(position.x + width, position.y, position.width - width, position.height);
            if (GUI.Button(rect, _dateTimeStr, _dateTimeBtnGS))
            {
                UDateTimeEditorPicker.OpenWindow(this);
            }

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            rect.Set(position.x + position.width - 20, position.y, 20, position.height);
            EditorGUI.LabelField(rect, _dateTimeIconGC);

            EditorGUI.indentLevel = indent;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18;
        }

        public UDateTime GetData()
        {
            UDateTime dateTime = new UDateTime();
            dateTime.Year = _year.intValue;
            dateTime.Month = _month.intValue;
            dateTime.Day = _day.intValue;
            dateTime.Hour = _hour.intValue;
            dateTime.Minute = _minute.intValue;
            dateTime.Second = _second.intValue;
            dateTime.Format = _format.stringValue;
            return dateTime;
        }
        public void SaveData(UDateTime dateTime)
        {
            _year.intValue = dateTime.Year;
            _month.intValue = dateTime.Month;
            _day.intValue = dateTime.Day;
            _hour.intValue = dateTime.Hour;
            _minute.intValue = dateTime.Minute;
            _second.intValue = dateTime.Second;
            _format.stringValue = dateTime.Format;

            _year.serializedObject.ApplyModifiedProperties();

            if (_year.serializedObject.targetObject is UDateTimeField)
            {
                UDateTimeField field = _year.serializedObject.targetObject as UDateTimeField;
                field.UpdateField();
                EditorUtility.SetDirty(field.CaptionText);
            }
        }

        private void InitDrawer(SerializedProperty property)
        {
            if (_dateTimeBtnGS != null)
                return;

            _dateTimeBtnGS = new GUIStyle(EditorStyles.miniButton);
            _dateTimeBtnGS.alignment = TextAnchor.MiddleLeft;
            _dateTimeIconGC = new GUIContent();
            _dateTimeIconGC.image = EditorGUIUtility.IconContent("Profiler.UIDetails").image;
            _year = property.FindPropertyRelative("Year");
            _month = property.FindPropertyRelative("Month");
            _day = property.FindPropertyRelative("Day");
            _hour = property.FindPropertyRelative("Hour");
            _minute = property.FindPropertyRelative("Minute");
            _second = property.FindPropertyRelative("Second");
            _format = property.FindPropertyRelative("Format");
        }
        private void UpdateDateTimeStr()
        {
            if (_yearCache != _year.intValue ||
                _monthCache != _month.intValue ||
                _dayCache != _day.intValue ||
                _hourCache != _hour.intValue ||
                _minuteCache != _minute.intValue ||
                _secondCache != _second.intValue ||
                _formatCache != _format.stringValue)
            {
                _yearCache = _year.intValue;
                _monthCache = _month.intValue;
                _dayCache = _day.intValue;
                _hourCache = _hour.intValue;
                _minuteCache = _minute.intValue;
                _secondCache = _second.intValue;
                _formatCache = _format.stringValue;
                _dateTimeStr = new DateTime(_year.intValue, _month.intValue, _day.intValue, _hour.intValue, _minute.intValue, _second.intValue).ToString(_format.stringValue);
            }
        }
    }
}