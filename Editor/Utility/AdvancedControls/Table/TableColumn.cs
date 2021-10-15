using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 表格视图列元素
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public sealed class TableColumn<T> : MultiColumnHeaderState.Column
    {
        /// <summary>
        /// 绘制列元素的方法
        /// </summary>
        public DrawCellMethod<T> DrawCell;
        /// <summary>
        /// 对比列元素的方法
        /// </summary>
        public CompareMethod<T> Compare;
    }

    /// <summary>
    /// 绘制列元素的方法
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="cellRect">绘制区域</param>
    /// <param name="data">绘制数据</param>
    /// <param name="rowIndex">在表格中的行索引</param>
    /// <param name="isSelected">是否选中</param>
    /// <param name="isFocused">是否焦点</param>
    public delegate void DrawCellMethod<T>(Rect cellRect, T data, int rowIndex, bool isSelected, bool isFocused);
    /// <summary>
    /// 对比列元素的方法
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="data1">数据1</param>
    /// <param name="data2">数据2</param>
    /// <returns>排序号</returns>
    public delegate int CompareMethod<T>(T data1, T data2);
}