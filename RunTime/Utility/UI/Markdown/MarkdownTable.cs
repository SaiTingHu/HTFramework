using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HT.Framework.MarkdownText;

namespace HT.Framework
{
    /// <summary>
    /// 显示Markdown的Table
    /// </summary>
    [DisallowMultipleComponent]
    public class MarkdownTable : HTBehaviour, IUpdateFrame
    {
        /// <summary>
        /// 行高度偏移值
        /// </summary>
        public float RowHeightOffset = 5;
        /// <summary>
        /// 表格的可点击按钮
        /// </summary>
        public Button TableButton;

        protected List<CellAlignment> _cellAlignments = new List<CellAlignment>();
        protected bool _isDirty = false;

        private RectTransform _rectTransform;
        private CanvasGroup _group;
        private bool _isShow = true;
        private float _width = 0;
        private float _height = 0;
        private float _border = 0;

        protected RectTransform Rect
        {
            get
            {
                if (_rectTransform == null)
                {
                    _rectTransform = transform.rectTransform();
                }
                return _rectTransform;
            }
        }
        protected CanvasGroup Group
        {
            get
            {
                if (_group == null)
                {
                    _group = transform.GetComponent<CanvasGroup>();
                }
                return _group;
            }
        }
        /// <summary>
        /// 是否显示表格
        /// </summary>
        public bool IsShow
        {
            get
            {
                return _isShow;
            }
            set
            {
                _isShow = value;
                _isDirty = true;
            }
        }
        /// <summary>
        /// 表格位置
        /// </summary>
        public Vector2 Pos
        {
            get
            {
                return Rect.anchoredPosition;
            }
            set
            {
                Rect.anchoredPosition = value;
            }
        }
        /// <summary>
        /// 表格宽度
        /// </summary>
        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                _isDirty = true;
            }
        }
        /// <summary>
        /// 表格高度
        /// </summary>
        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                _isDirty = true;
            }
        }
        /// <summary>
        /// 表格边框厚度
        /// </summary>
        public float Border
        {
            get
            {
                return _border;
            }
            set
            {
                _border = value;
                _isDirty = true;
            }
        }
        /// <summary>
        /// 表格行数
        /// </summary>
        public int RowNumber { get; protected set; }
        /// <summary>
        /// 表格列数
        /// </summary>
        public int ColumnNumber { get; protected set; }
        /// <summary>
        /// 初始的高度
        /// </summary>
        public float InitialHeight { get; protected set; }

        /// <summary>
        /// 生成表格
        /// </summary>
        public virtual void Generate(TableMark tableMark)
        {
            if (tableMark.Rows.Count <= 0)
            {
                gameObject.SetActive(false);
                return;
            }

            GenerateCellAlignment(tableMark.Signs);

            RectTransform rowTemp = transform.Find("RowTemp").rectTransform();
            for (int i = 0; i < tableMark.Rows.Count; i++)
            {
                GenerateRow(rowTemp, tableMark.Rows[i]);
            }

            Width = tableMark.Width;
            Height = (tableMark.RowHeight + RowHeightOffset) * tableMark.Rows.Count;
            Border = 2;
            RowNumber = tableMark.Rows.Count;
            ColumnNumber = _cellAlignments.Count;
            InitialHeight = Height;
        }
        /// <summary>
        /// 帧更新
        /// </summary>
        public virtual void OnUpdateFrame()
        {
            if (_isDirty)
            {
                _isDirty = false;

                Group.alpha = IsShow ? 1 : 0;
                Group.blocksRaycasts = IsShow;

                if (IsShow) UpdateTableSize();
            }
        }
        /// <summary>
        /// 更新表格大小
        /// </summary>
        protected virtual void UpdateTableSize()
        {
            Rect.sizeDelta = new Vector2(Width, Height);

            float rowHeight = (Height - Border * (RowNumber + 1)) / RowNumber;
            float cellWidth = (Width - Border * (ColumnNumber + 1)) / ColumnNumber;

            float rowY = -Border;
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform row = transform.GetChild(i) as RectTransform;
                if (row.name == "BG" || !row.gameObject.activeSelf)
                    continue;

                row.anchoredPosition = new Vector2(0, rowY);
                SetHeight(row, rowHeight);
                rowY -= rowHeight;
                rowY -= Border;

                float cellX = Border;
                for (int j = 0; j < row.childCount; j++)
                {
                    RectTransform cell = row.GetChild(j) as RectTransform;
                    if (!cell.gameObject.activeSelf)
                        continue;

                    cell.anchoredPosition = new Vector2(cellX, 0);
                    SetWidth(cell, cellWidth);
                    cellX += cellWidth;
                    cellX += Border;
                }
            }
        }

        /// <summary>
        /// 生成表格元素对齐方式
        /// </summary>
        private void GenerateCellAlignment(List<string> signs)
        {
            _cellAlignments.Clear();
            for (int i = 0; i < signs.Count; i++)
            {
                string sign = signs[i].Trim();
                CellAlignment cellAlignment;
                if (sign.StartsWith(":") && !sign.EndsWith(":"))
                {
                    cellAlignment = CellAlignment.Left;
                }
                else if (!sign.StartsWith(":") && sign.EndsWith(":"))
                {
                    cellAlignment = CellAlignment.Right;
                }
                else
                {
                    cellAlignment = CellAlignment.Center;
                }
                _cellAlignments.Add(cellAlignment);
            }

            if (_cellAlignments.Count <= 0)
            {
                _cellAlignments.Add(CellAlignment.Center);
            }
        }
        /// <summary>
        /// 生成行
        /// </summary>
        private void GenerateRow(RectTransform rowTemp, string[] rowData)
        {
            RectTransform row = Main.CloneGameObject(rowTemp.gameObject, true).rectTransform();
            row.localRotation = Quaternion.identity;
            row.localScale = Vector3.one;
            row.anchoredPosition = Vector2.zero;

            RectTransform cellTemp = row.Find("CellTemp").rectTransform();
            for (int i = 0; i < _cellAlignments.Count; i++)
            {
                GenerateCell(cellTemp, _cellAlignments[i], (rowData != null && i < rowData.Length) ? rowData[i] : null);
            }
        }
        /// <summary>
        /// 生成元素
        /// </summary>
        private void GenerateCell(RectTransform cellTemp, CellAlignment cellAlignment, string cellData)
        {
            RectTransform cell = Main.CloneGameObject(cellTemp.gameObject, true).rectTransform();
            cell.localRotation = Quaternion.identity;
            cell.localScale = Vector3.one;
            cell.anchoredPosition = Vector2.zero;

            Text text = cell.Find("Text").GetComponent<Text>();
            text.text = cellData;
            if (cellAlignment == CellAlignment.Left) text.alignment = TextAnchor.MiddleLeft;
            else if (cellAlignment == CellAlignment.Center) text.alignment = TextAnchor.MiddleCenter;
            else if (cellAlignment == CellAlignment.Right) text.alignment = TextAnchor.MiddleRight;
        }
        /// <summary>
        /// 设置宽度
        /// </summary>
        private void SetWidth(RectTransform rect, float width)
        {
            Vector2 sizeDelta = rect.sizeDelta;
            sizeDelta.x = width;
            rect.sizeDelta = sizeDelta;
        }
        /// <summary>
        /// 设置高度
        /// </summary>
        private void SetHeight(RectTransform rect, float height)
        {
            Vector2 sizeDelta = rect.sizeDelta;
            sizeDelta.y = height;
            rect.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// 表格元素对齐方式
        /// </summary>
        public enum CellAlignment
        {
            /// <summary>
            /// 左对齐
            /// </summary>
            Left,
            /// <summary>
            /// 居中
            /// </summary>
            Center,
            /// <summary>
            /// 右对齐
            /// </summary>
            Right
        }
    }
}