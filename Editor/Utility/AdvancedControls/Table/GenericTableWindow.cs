using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace HT.Framework
{
    /// <summary>
    /// 通用表格绘制器
    /// </summary>
    internal sealed class GenericTableWindow : HTFEditorWindow
    {
        /// <summary>
        /// 打开通用表格绘制器
        /// </summary>
        /// <param name="target">表格数据目标实例</param>
        /// <param name="fieldName">表格数据的字段名称</param>
        public static void OpenWindow(UObject target, string fieldName)
        {
            GenericTableWindow window = GetWindow<GenericTableWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("ScriptableObject Icon").image;
            window.titleContent.text = "Generic Table";
            window.Initialization(target, fieldName);
        }

        private const int Border = 10;
        private const int TitleHeight = 20;
        private Dictionary<string, FieldInfo> _fieldInfos = new Dictionary<string, FieldInfo>();
        private TableView<object> _tableView;
        private UObject _target;
        private string _targetName;

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/120796924";

        private void Initialization(UObject target, string fieldName)
        {
            FieldInfo fieldInfo = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo == null)
            {
                Log.Warning(string.Format("通用表格绘制器：未从 {0} 中找到字段 {1}！", target.GetType().FullName, fieldName));
                Close();
                return;
            }

            List<object> datas = GetDatas(fieldInfo.GetValue(target));
            if (datas.Count <= 0)
            {
                Log.Warning(string.Format("通用表格绘制器：{0} 的字段 {1} 长度为0，或不是数组、集合类型！", target.GetType().FullName, fieldName));
                Close();
                return;
            }

            List<TableColumn<object>> columns = GetColumns(datas[0].GetType());
            if (columns.Count <= 0)
            {
                Log.Warning(string.Format("通用表格绘制器：{0} 的字段 {1} 不是复杂类型，或类型中不含有可序列化字段！", target.GetType().FullName, fieldName));
                Close();
                return;
            }

            _tableView = new TableView<object>(datas, columns);
            _tableView.IsEnableContextClick = false;
            _target = target;
            _targetName = string.Format("{1}.{2} ({0})", _target.name, _target.GetType().FullName, fieldName);
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            if (GUILayout.Button(_targetName, EditorStyles.toolbarButton))
            {
                Selection.activeObject = _target;
                EditorGUIUtility.PingObject(_target);
            }
            GUILayout.FlexibleSpace();
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();
            
            Rect rect = new Rect(0, 0, position.width, position.height);
            rect.x += Border;
            rect.y += Border + TitleHeight;
            rect.width -= Border * 2;
            rect.height -= Border * 2 + TitleHeight;
            _tableView.OnGUI(rect);
        }
        private void Update()
        {
            if (EditorApplication.isCompiling || _tableView == null || _target == null)
            {
                Close();
            }
        }

        private List<object> GetDatas(object field)
        {
            List<object> datas = new List<object>();
            Array array = field as Array;
            IEnumerable<object> list = field as IEnumerable<object>;
            if (array != null)
            {
                foreach (var item in array)
                {
                    datas.Add(item);
                }
            }
            else if (list != null)
            {
                foreach (var item in list)
                {
                    datas.Add(item);
                }
            }
            return datas;
        }
        private List<TableColumn<object>> GetColumns(Type type)
        {
            _fieldInfos.Clear();
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                if (fieldInfos[i].IsPublic || fieldInfos[i].IsDefined(typeof(SerializeField), true))
                {
                    if (!_fieldInfos.ContainsKey(fieldInfos[i].Name))
                    {
                        _fieldInfos.Add(fieldInfos[i].Name, fieldInfos[i]);
                    }
                }
            }

            List<TableColumn<object>> columns = new List<TableColumn<object>>();
            foreach (var item in _fieldInfos)
            {
                TableColumn<object> column = null;
                FieldInfo field = item.Value;
                if (field.FieldType.IsEnum)
                {
                    column = GetEnumColumn(field);
                }
                else if (field.FieldType == typeof(string))
                {
                    column = GetStringColumn(field);
                }
                else if (field.FieldType == typeof(int))
                {
                    column = GetIntColumn(field);
                }
                else if (field.FieldType == typeof(float))
                {
                    column = GetFloatColumn(field);
                }
                else if (field.FieldType == typeof(bool))
                {
                    column = GetBoolColumn(field);
                }
                else if (field.FieldType == typeof(Vector2))
                {
                    column = GetVector2Column(field);
                }
                else if (field.FieldType == typeof(Vector3))
                {
                    column = GetVector3Column(field);
                }
                else if (field.FieldType == typeof(Color))
                {
                    column = GetColorColumn(field);
                }
                else if (field.FieldType.IsSubclassOf(typeof(UObject)))
                {
                    column = GetObjectColumn(field);
                }
                if (column != null)
                {
                    column.autoResize = false;
                    column.headerContent = new GUIContent(field.Name);
                    columns.Add(column);
                }
            }
            return columns;
        }
        private TableColumn<object> GetEnumColumn(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 100;
            column.canSort = false;
            column.Compare = null;
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                Enum value = EditorGUI.EnumPopup(rect, (Enum)field.GetValue(data));
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
        private TableColumn<object> GetStringColumn(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 100;
            column.canSort = true;
            column.Compare = (a, b) =>
            {
                string x = (string)field.GetValue(a);
                string y = (string)field.GetValue(b);
                return x.CompareTo(y);
            };
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                string value = EditorGUI.TextField(rect, (string)field.GetValue(data));
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
        private TableColumn<object> GetIntColumn(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 100;
            column.canSort = true;
            column.Compare = (a, b) =>
            {
                int x = (int)field.GetValue(a);
                int y = (int)field.GetValue(b);
                return x.CompareTo(y);
            };
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                int value = EditorGUI.IntField(rect, (int)field.GetValue(data));
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
        private TableColumn<object> GetFloatColumn(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 100;
            column.canSort = true;
            column.Compare = (a, b) =>
            {
                float x = (float)field.GetValue(a);
                float y = (float)field.GetValue(b);
                return x.CompareTo(y);
            };
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                float value = EditorGUI.FloatField(rect, (float)field.GetValue(data));
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
        private TableColumn<object> GetBoolColumn(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 40;
            column.canSort = false;
            column.Compare = null;
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                bool value = EditorGUI.Toggle(rect, (bool)field.GetValue(data));
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
        private TableColumn<object> GetVector2Column(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 100;
            column.canSort = false;
            column.Compare = null;
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                Vector2 value = EditorGUI.Vector2Field(rect, "", (Vector2)field.GetValue(data));
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
        private TableColumn<object> GetVector3Column(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 150;
            column.canSort = false;
            column.Compare = null;
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                Vector3 value = EditorGUI.Vector3Field(rect, "", (Vector3)field.GetValue(data));
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
        private TableColumn<object> GetColorColumn(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 100;
            column.canSort = false;
            column.Compare = null;
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                Color value = EditorGUI.ColorField(rect, (Color)field.GetValue(data));
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
        private TableColumn<object> GetObjectColumn(FieldInfo field)
        {
            TableColumn<object> column = new TableColumn<object>();
            column.width = 150;
            column.canSort = false;
            column.Compare = null;
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.BeginChangeCheck();
                UObject value = EditorGUI.ObjectField(rect, field.GetValue(data) as UObject, field.FieldType, true);
                if (EditorGUI.EndChangeCheck())
                {
                    field.SetValue(data, value);
                    HasChanged(_target);
                }
            };
            return column;
        }
    }
}