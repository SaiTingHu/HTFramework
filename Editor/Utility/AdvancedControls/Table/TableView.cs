using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 表格视图
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public sealed class TableView<T> : TreeView where T : class, new()
    {
        /// <summary>
        /// 表格数据
        /// </summary>
        private List<T> _datas;
        /// <summary>
        /// 根元素
        /// </summary>
        private TableViewItem<T> _rootItem;
        /// <summary>
        /// 所有的元素
        /// </summary>
        private List<TableViewItem<T>> _items;
        /// <summary>
        /// 所有的元素绘制项
        /// </summary>
        private List<TreeViewItem> _drawItems;
        /// <summary>
        /// 元素ID标记
        /// </summary>
        private int _idSign = 0;

        /// <summary>
        /// 行高度
        /// </summary>
        public float RowHeight
        {
            get
            {
                return rowHeight;
            }
            set
            {
                rowHeight = value;
            }
        }
        /// <summary>
        /// 是否启用上下文右键点击
        /// </summary>
        public bool IsEnableContextClick { get; set; } = true;

        /// <summary>
        /// 表格视图
        /// </summary>
        /// <param name="datas">表格视图数据</param>
        /// <param name="columns">表格视图的所有列</param>
        public TableView(List<T> datas, List<TableColumn<T>> columns) : base(new TreeViewState())
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            rowHeight = EditorGUIUtility.singleLineHeight + 4;
            columns.Insert(0, GetIndexColumn());
            multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(columns.ToArray()));
            multiColumnHeader.sortingChanged += OnSortingChanged;
            multiColumnHeader.visibleColumnsChanged += OnVisibleColumnsChanged;

            _datas = datas;
            _rootItem = new TableViewItem<T>(-1, -1, null);
            _items = new List<TableViewItem<T>>();
            for (var i = 0; i < _datas.Count; i++)
            {
                _items.Add(new TableViewItem<T>(_idSign, 0, _datas[i]));
                _idSign += 1;
            }
            _drawItems = new List<TreeViewItem>();

            Reload();
        }
        /// <summary>
        /// 构造根节点
        /// </summary>
        protected override TreeViewItem BuildRoot()
        {
            return _rootItem;
        }
        /// <summary>
        /// 构造所有行
        /// </summary>
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            _drawItems.Clear();
            for (int i = 0; i < _items.Count; i++)
            {
                _drawItems.Add(_items[i]);
            }
            return _drawItems;
        }
        /// <summary>
        /// 绘制行
        /// </summary>
        protected override void RowGUI(RowGUIArgs args)
        {
            TableViewItem<T> item = args.item as TableViewItem<T>;
            int visibleColumns = args.GetNumVisibleColumns();
            for (var i = 0; i < visibleColumns; i++)
            {
                Rect cellRect = args.GetCellRect(i);
                int index = args.GetColumn(i);
                CenterRectUsingSingleLineHeight(ref cellRect);
                TableColumn<T> column = multiColumnHeader.GetColumn(index) as TableColumn<T>;
                column.DrawCell?.Invoke(cellRect, item.Data, args.row, args.selected, args.focused);
            }
        }
        /// <summary>
        /// 上下文右键点击
        /// </summary>
        protected override void ContextClicked()
        {
            if (!IsEnableContextClick)
                return;

            List<TableViewItem<T>> selectedItems = new List<TableViewItem<T>>();
            foreach (var itemID in GetSelection())
            {
                TableViewItem<T> item = _items.Find((it) => { return it.id == itemID; });
                if (item != null) selectedItems.Add(item);
            }

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add row"), false, () =>
            {
                AddData(new T());
            });
            if (selectedItems.Count > 0)
            {
                menu.AddItem(new GUIContent("Delete selected rows"), false, () =>
                {
                    DeleteDatas(selectedItems);
                });
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Clear rows"), false, () =>
            {
                ClearData();
            });
            menu.ShowAsContext();
        }

        /// <summary>
        /// 获取索引列
        /// </summary>
        private TableColumn<T> GetIndexColumn()
        {
            TableColumn<T> column = new TableColumn<T>();
            column.autoResize = false;
            column.headerContent = new GUIContent("Index");
            column.width = 50;
            column.canSort = false;
            column.Compare = null;
            column.DrawCell = (rect, data, rowIndex, isSelected, isFocused) =>
            {
                EditorGUI.LabelField(rect, rowIndex.ToString());
            };
            return column;
        }
        /// <summary>
        /// 当重新排序
        /// </summary>
        private void OnSortingChanged(MultiColumnHeader columnheader)
        {
            bool isAscending = multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex);
            TableColumn<T> column = multiColumnHeader.GetColumn(multiColumnHeader.sortedColumnIndex) as TableColumn<T>;
            if (column.Compare != null)
            {
                _items.Sort((a, b) =>
                {
                    if (isAscending)
                    {
                        return -column.Compare(a.Data, b.Data);
                    }
                    else
                    {
                        return column.Compare(a.Data, b.Data);
                    }
                });
                Reload();
            }
        }
        /// <summary>
        /// 当列激活状态改变
        /// </summary>
        private void OnVisibleColumnsChanged(MultiColumnHeader columnheader)
        {
            Reload();
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data">数据</param>
        public void AddData(T data)
        {
            if (_datas.Contains(data))
                return;

            _datas.Add(data);
            _items.Add(new TableViewItem<T>(_idSign, 0, data));
            _idSign += 1;
            Reload();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="data">数据</param>
        public void DeleteData(T data)
        {
            if (!_datas.Contains(data))
                return;

            _datas.Remove(data);
            TableViewItem<T> item = _items.Find((i) => { return i.Data == data; });
            if (item != null)
            {
                _items.Remove(item);
            }
            Reload();
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="items">数据元素</param>
        public void DeleteDatas(List<TableViewItem<T>> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                _datas.Remove(items[i].Data);
                _items.Remove(items[i]);
            }
            Reload();
        }
        /// <summary>
        /// 清空所有数据
        /// </summary>
        public void ClearData()
        {
            _datas.Clear();
            _items.Clear();
            Reload();
        }
    }
}