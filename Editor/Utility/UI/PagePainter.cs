using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace HT.Framework
{
    /// <summary>
    /// 分页绘制器
    /// </summary>
    public sealed class PagePainter
    {
        /// <summary>
        /// 当前分页
        /// </summary>
        private string _currentPage;
        /// <summary>
        /// 所有分页
        /// </summary>
        private Dictionary<string, Page> _pages = new Dictionary<string, Page>();
        /// <summary>
        /// 所有分页的顺序
        /// </summary>
        private List<string> _pagesOrder = new List<string>();

        /// <summary>
        /// 当前的宿主
        /// </summary>
        public Editor CurrentHost { get; private set; }
        /// <summary>
        /// 当前分页
        /// </summary>
        public string CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                EditorPrefs.SetString(CurrentHost.GetType().FullName, _currentPage);
            }
        }
        /// <summary>
        /// 分页选中时背景风格
        /// </summary>
        public string CheckStyle { get; set; } = "SelectionRect";
        /// <summary>
        /// 分页未选中时背景风格
        /// </summary>
        public string UncheckStyle { get; set; } = "Box";

        public PagePainter(Editor host)
        {
            CurrentHost = host ?? throw new HTFrameworkException(HTFrameworkModule.Utility, "初始化分页绘制器失败：宿主不能为空！");
            CurrentPage = EditorPrefs.GetString(CurrentHost.GetType().FullName, null);
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public void Painting()
        {
            for (int i = 0; i < _pagesOrder.Count; i++)
            {
                string pageName = _pagesOrder[i];
                Page page = _pages[pageName];

                GUILayout.BeginVertical(pageName == CurrentPage ? CheckStyle : UncheckStyle);

                GUILayout.BeginHorizontal();
                GUILayout.Space(12);
                bool oldValue = CurrentPage == pageName;
                page.Expanded.target = EditorGUILayout.Foldout(oldValue, page.Content, true);
                if (page.Expanded.target != oldValue)
                {
                    if (page.Expanded.target) CurrentPage = pageName;
                    else CurrentPage = null;
                }
                GUILayout.EndHorizontal();

                if (EditorGUILayout.BeginFadeGroup(page.Expanded.faded))
                {
                    page.OnPaint();
                }
                EditorGUILayout.EndFadeGroup();

                GUILayout.EndVertical();
            }
        }
        /// <summary>
        /// 添加一个分页
        /// </summary>
        /// <param name="pageName">分页名称</param>
        /// <param name="pageIcon">分页图标</param>
        /// <param name="onPaint">绘制方法</param>
        public void AddPage(string pageName, Texture pageIcon, HTFAction onPaint)
        {
            if (_pages.ContainsKey(pageName))
                return;

            _pages.Add(pageName, new Page(CurrentHost, pageName, pageIcon, onPaint));
            _pagesOrder.Add(pageName);
        }
        /// <summary>
        /// 移除一个分页
        /// </summary>
        /// <param name="pageName">分页名称</param>
        public void RemovePage(string pageName)
        {
            if (!_pages.ContainsKey(pageName))
                return;

            _pages.Remove(pageName);
            _pagesOrder.Remove(pageName);
        }
        /// <summary>
        /// 清空所有分页
        /// </summary>
        public void ClearPage()
        {
            _pages.Clear();
            _pagesOrder.Clear();
        }

        /// <summary>
        /// 分页
        /// </summary>
        private class Page
        {
            /// <summary>
            /// 宿主
            /// </summary>
            public Editor Host { get; private set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// GUI绘制内容
            /// </summary>
            public GUIContent Content { get; private set; }
            /// <summary>
            /// 是否展开分页
            /// </summary>
            public AnimBool Expanded { get; private set; }
            /// <summary>
            /// 绘制方法
            /// </summary>
            public HTFAction OnPaint { get; private set; }
            
            public Page(Editor host, string name, Texture icon, HTFAction onPaint)
            {
                Host = host;
                Name = name;
                Content = new GUIContent();
                Content.image = icon;
                Content.text = name;
                Expanded = new AnimBool(false, new UnityAction(Host.Repaint));
                OnPaint = onPaint;
            }
        }
    }
}