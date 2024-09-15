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

        protected List<CellAlignment> _cellAlignments = new List<CellAlignment>();
        protected float _rowY;
        protected float _cellX;
        protected bool _isDirty = false;

        private RectTransform _rectTransform;
        private CanvasGroup _group;
        private bool _isShow = true;
        private float _width;

        public RectTransform Rect
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
        public CanvasGroup Group
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
        public virtual bool IsShow
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
        /// 表格宽度
        /// </summary>
        public virtual float Width
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
        /// 表格边框厚度
        /// </summary>
        protected virtual float Border { get; private set; } = 2;

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

            float tabelWidth = tableMark.Width;
            float tableHeight = (tableMark.RowHeight + RowHeightOffset) * tableMark.Rows.Count;
            Rect.sizeDelta = new Vector2(tabelWidth, tableHeight);

            GenerateCellAlignment(tableMark.Signs);

            _rowY = -Border;
            float rowHeight = (tableHeight - Border * (tableMark.Rows.Count + 1)) / tableMark.Rows.Count;
            RectTransform rowTemp = transform.Find("RowTemp").rectTransform();
            for (int i = 0; i < tableMark.Rows.Count; i++)
            {
                GenerateRow(rowTemp, rowHeight, tableMark.Rows[i]);
                _rowY -= rowHeight;
                _rowY -= Border;
            }

            Width = tabelWidth;
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

                UpdateWidth();
            }
        }
        /// <summary>
        /// 更新表格宽度
        /// </summary>
        protected virtual void UpdateWidth()
        {
            SetWidth(Rect, Width);

            float cellWidth = (Width - Border * (_cellAlignments.Count + 1)) / _cellAlignments.Count;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform row = transform.GetChild(i);
                if (row.name == "BG" || !row.gameObject.activeSelf)
                    continue;

                _cellX = Border;
                for (int j = 0; j < row.childCount; j++)
                {
                    Transform cell = row.GetChild(j);
                    if (!cell.gameObject.activeSelf)
                        continue;

                    RectTransform cellRect = cell.rectTransform();
                    cellRect.anchoredPosition = new Vector2(_cellX, 0);
                    SetWidth(cellRect, cellWidth);
                    _cellX += cellWidth;
                    _cellX += Border;
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
        private void GenerateRow(RectTransform rowTemp, float height, string[] rowData)
        {
            RectTransform row = Main.CloneGameObject(rowTemp.gameObject, true).rectTransform();
            row.transform.localRotation = Quaternion.identity;
            row.transform.localScale = Vector3.one;
            row.anchoredPosition = new Vector2(0, _rowY);
            SetHeight(row, height);

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
            cell.transform.localRotation = Quaternion.identity;
            cell.transform.localScale = Vector3.one;
            cell.anchoredPosition = Vector2.zero;
            SetWidth(cell, 100);

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