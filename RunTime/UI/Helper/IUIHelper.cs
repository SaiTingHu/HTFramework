using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI管理器的助手接口
    /// </summary>
    public interface IUIHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有Overlay类型的UI
        /// </summary>
        Dictionary<Type, UILogicBase> OverlayUIs { get; }
        /// <summary>
        /// 所有Camera类型的UI
        /// </summary>
        Dictionary<Type, UILogicBase> CameraUIs { get; }
        /// <summary>
        /// Overlay类型的UI根节点
        /// </summary>
        RectTransform OverlayUIRoot { get; }
        /// <summary>
        /// Camera类型的UI根节点
        /// </summary>
        RectTransform CameraUIRoot { get; }
        /// <summary>
        /// Camera类型UI的摄像机
        /// </summary>
        Camera UICamera { get; }
        /// <summary>
        /// 是否锁住当前打开的临时UI，锁住后打开其他临时UI将无法顶掉当前打开的UI，使其显示于绝对顶端
        /// </summary>
        bool IsLockTemporaryUI { get; set; }
        /// <summary>
        /// 是否隐藏所有UI实体
        /// </summary>
        bool IsHideAll { get; set; }
        /// <summary>
        /// 是否显示全屏遮罩
        /// </summary>
        bool IsDisplayMask { get; set; }

        /// <summary>
        /// 设置预定义
        /// </summary>
        /// <param name="defineUINames">预定义的UI名称</param>
        /// <param name="defineUIEntitys">预定义的UI实体</param>
        void SetDefine(List<string> defineUINames, List<GameObject> defineUIEntitys);
        /// <summary>
        /// 添加预定义（如果已存在则覆盖，已打开的UI不受影响，销毁后再次打开生效）
        /// </summary>
        /// <param name="defineUIName">预定义的UI名称</param>
        /// <param name="defineUIEntity">预定义的UI实体</param>
        void AddDefine(string defineUIName, GameObject defineUIEntity);

        /// <summary>
        /// 预加载常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <returns>加载协程</returns>
        Coroutine PreloadingResidentUI(Type type);
        /// <summary>
        /// 预加载临时UI
        /// </summary>
        /// <param name="type">临时UI逻辑类</param>
        /// <returns>加载协程</returns>
        Coroutine PreloadingTemporaryUI(Type type);
        /// <summary>
        /// 打开常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        Coroutine OpenResidentUI(Type type, params object[] args);
        /// <summary>
        /// 打开临时UI
        /// </summary>
        /// <param name="type">临时UI逻辑类</param>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        Coroutine OpenTemporaryUI(Type type, params object[] args);
        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        UILogicBase GetOpenedUI(Type type);
        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        UILogicBase GetUI(Type type);
        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        void PlaceTopUI(Type type);
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        void CloseUI(Type type);
        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        void DestroyUI(Type type);

        /// <summary>
        /// 显示同属于一个深度的所有UI界面
        /// </summary>
        /// <param name="depth">深度</param>
        void ShowAllUIOfDepth(int depth);
        /// <summary>
        /// 隐藏同属于一个深度的所有UI界面
        /// </summary>
        /// <param name="depth">深度</param>
        void HideAllUIOfDepth(int depth);
    }
}