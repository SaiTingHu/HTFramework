using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 滚动数据列表
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public sealed class ScrollList : ScrollRect
    {
        /// <summary>
        /// 数据元素模板
        /// </summary>
        public ScrollListElement ElementTemplate;
        /// <summary>
        /// 数据滚动方向
        /// </summary>
        public Direction ScrollDirection = Direction.Vertical;
        /// <summary>
        /// 数据变化时列表停留位置
        /// </summary>
        public StayPosition StayPos = StayPosition.Zero;
        /// <summary>
        /// 第一个元素的位置
        /// </summary>
        public Vector2 FirstPosition;
        /// <summary>
        /// 一行的元素数量，超出此数量后换行
        /// </summary>
        public int RowNumber = 1;
        /// <summary>
        /// 元素之间的间隔
        /// </summary>
        public Vector2 Spacing;
        /// <summary>
        /// 自定义列表布局
        /// </summary>
        public HTFAction<ScrollList> OnCustomLayoutHandler;

        private List<ScrollListData> _datas = new List<ScrollListData>();
        private HashSet<ScrollListData> _dataIndexs = new HashSet<ScrollListData>();
        private Dictionary<ScrollListData, ScrollListElement> _elements = new Dictionary<ScrollListData, ScrollListElement>();
        private Queue<ScrollListElement> _elementsPool = new Queue<ScrollListElement>();
        private bool _isdirty = false;

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="index">数据索引</param>
        public ScrollListData this[int index]
        {
            get
            {
                return (index >= 0 && index < _datas.Count) ? _datas[index] : null;
            }
        }
        /// <summary>
        /// 获取数据对应的数据元素
        /// </summary>
        /// <param name="data">数据</param>
        public ScrollListElement this[ScrollListData data]
        {
            get
            {
                return _elements.ContainsKey(data) ? _elements[data] : null;
            }
        }
        /// <summary>
        /// 当前数据数量
        /// </summary>
        public int Count
        {
            get
            {
                return _datas.Count;
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            if (_isdirty)
            {
                _isdirty = false;
                RefreshScrollView();
            }
        }

        /// <summary>
        /// 添加一条新的数据到列表尾部
        /// </summary>
        /// <param name="data">数据</param>
        public void AddData(ScrollListData data)
        {
            if (_dataIndexs.Contains(data))
            {
                Log.Warning("滚动数据列表：添加数据失败，列表中已存在该数据 " + data.ToString());
                return;
            }

            _datas.Add(data);
            _dataIndexs.Add(data);

            ScrollListElement element = ExtractIdleElement();
            element.Data = data;
            element.List = this;
            _elements.Add(data, element);

            _isdirty = true;
        }
        /// <summary>
        /// 添加多条新的数据到列表尾部
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="datas">数据</param>
        public void AddDatas<T>(T[] datas) where T : ScrollListData
        {
            for (int i = 0; i < datas.Length; i++)
            {
                AddData(datas[i]);
            }
        }
        /// <summary>
        /// 添加多条新的数据到列表尾部
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="datas">数据</param>
        public void AddDatas<T>(List<T> datas) where T : ScrollListData
        {
            for (int i = 0; i < datas.Count; i++)
            {
                AddData(datas[i]);
            }
        }
        /// <summary>
        /// 查找一条数据
        /// </summary>
        /// <param name="match">匹配方式</param>
        public ScrollListData FindData(Predicate<ScrollListData> match)
        {
            return _datas.Find(match);
        }
        /// <summary>
        /// 移除一条数据
        /// </summary>
        /// <param name="data">数据</param>
        public void RemoveData(ScrollListData data)
        {
            if (_dataIndexs.Contains(data))
            {
                _datas.Remove(data);
                _dataIndexs.Remove(data);

                if (_elements.ContainsKey(data))
                {
                    RecycleElement(_elements[data]);
                    _elements.Remove(data);
                }

                _isdirty = true;
            }
            else
            {
                Log.Warning("滚动数据列表：移除数据失败，列表中不存在该数据 " + data.ToString());
            }
        }
        /// <summary>
        /// 清除所有的数据
        /// </summary>
        public void ClearData()
        {
            _datas.Clear();
            _dataIndexs.Clear();

            foreach (var element in _elements)
            {
                RecycleElement(element.Value);
            }
            _elements.Clear();

            _isdirty = true;
        }
        /// <summary>
        /// 更新所有数据
        /// </summary>
        public void UpdateAllData()
        {
            foreach (var item in _elements)
            {
                item.Value.UpdateData();
            }
            _isdirty = true;
        }
        /// <summary>
        /// 更新列表布局
        /// </summary>
        public void UpdateLayout()
        {
            _isdirty = true;
        }
        /// <summary>
        /// 使列表滚动到指定元素的位置
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="duration">滚动动画时长</param>
        /// <param name="ease">滚动动画类型</param>
        public void ScrollToElement(ScrollListElement element, float duration = 0.5f, Ease ease = Ease.Linear)
        {
            if (ScrollDirection == Direction.Vertical)
            {
                content.DOAnchorPos(new Vector2(0, Mathf.Abs(element.rectTransform.anchoredPosition.y)), duration).SetEase(ease);
            }
            else
            {
                content.DOAnchorPos(new Vector2(Mathf.Abs(element.rectTransform.anchoredPosition.x), 0), duration).SetEase(ease);
            }
        }

        /// <summary>
        /// 更新滚动视图
        /// </summary>
        private void RefreshScrollView()
        {
            if (OnCustomLayoutHandler != null)
            {
                OnCustomLayoutHandler(this);
                return;
            }

            if (ScrollDirection == Direction.Vertical)
            {
                float x = FirstPosition.x;
                float y = FirstPosition.y;
                float height = 0;
                int row = 0;
                for (int i = 0; i < _datas.Count; i++)
                {
                    ScrollListElement element = _elements[_datas[i]];
                    element.rectTransform.anchoredPosition = new Vector2(x, y);
                    height = element.rectTransform.sizeDelta.y;
                    row++;

                    if (row < RowNumber)
                    {
                        x += (element.rectTransform.sizeDelta.x + Spacing.x);
                    }
                    else
                    {
                        row = 0;
                        x = FirstPosition.x;
                        y -= (element.rectTransform.sizeDelta.y + Spacing.y);
                    }
                }
                if (row != 0)
                {
                    y -= (height + Spacing.y);
                }

                content.sizeDelta = new Vector2(0, Mathf.Abs(y));
                if (StayPos == StayPosition.Zero) normalizedPosition = Vector2.zero;
                else if (StayPos == StayPosition.One) normalizedPosition = Vector2.one;
            }
            else
            {
                float x = FirstPosition.x;
                float y = FirstPosition.y;
                float width = 0;
                int row = 0;
                for (int i = 0; i < _datas.Count; i++)
                {
                    ScrollListElement element = _elements[_datas[i]];
                    element.rectTransform.anchoredPosition = new Vector2(x, y);
                    width = element.rectTransform.sizeDelta.x;
                    row++;

                    if (row < RowNumber)
                    {
                        y -= (element.rectTransform.sizeDelta.y + Spacing.y);
                    }
                    else
                    {
                        row = 0;
                        x += (element.rectTransform.sizeDelta.x + Spacing.x);
                        y = FirstPosition.y;
                    }
                }
                if (row != 0)
                {
                    x += (width + Spacing.x);
                }

                content.sizeDelta = new Vector2(Mathf.Abs(x), 0);
                if (StayPos == StayPosition.Zero) normalizedPosition = Vector2.zero;
                else if (StayPos == StayPosition.One) normalizedPosition = Vector2.one;
            }
        }
        /// <summary>
        /// 提取一个空闲的列表元素
        /// </summary>
        private ScrollListElement ExtractIdleElement()
        {
            if (_elementsPool.Count > 0)
            {
                ScrollListElement element = _elementsPool.Dequeue();
                element.gameObject.SetActive(true);
                return element;
            }
            else
            {
                GameObject element = Instantiate(ElementTemplate.gameObject, content);
                element.SetActive(true);
                return element.GetComponent<ScrollListElement>();
            }
        }
        /// <summary>
        /// 回收一个无用的列表元素
        /// </summary>
        private void RecycleElement(ScrollListElement element)
        {
            element.Data = null;
            element.List = null;
            element.gameObject.SetActive(false);
            _elementsPool.Enqueue(element);
        }

        /// <summary>
        /// 数据滚动方向
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// 水平
            /// </summary>
            Horizontal = 0,
            /// <summary>
            /// 垂直
            /// </summary>
            Vertical = 1
        }
        /// <summary>
        /// 列表停留位置
        /// </summary>
        public enum StayPosition
        {
            /// <summary>
            /// 停留到滚动视图Zero位置
            /// </summary>
            Zero = 0,
            /// <summary>
            /// 停留到滚动视图One位置
            /// </summary>
            One = 1,
            /// <summary>
            /// 保持位置不动
            /// </summary>
            None = 2
        }
    }
}