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
        /// 所有World类型的UI
        /// </summary>
        Dictionary<string, UIManager.WorldUIDomain> WorldUIs { get; }
        /// <summary>
        /// Camera类型UI的摄像机
        /// </summary>
        Camera UICamera { get; }
        /// <summary>
        /// Overlay类型的UI根节点
        /// </summary>
        RectTransform OverlayUIRoot { get; }
        /// <summary>
        /// Camera类型的UI根节点
        /// </summary>
        RectTransform CameraUIRoot { get; }
        /// <summary>
        /// World类型的UI域根节点
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <returns>域根节点</returns>
        RectTransform WorldUIDomainRoot(string domainName);
        /// <summary>
        /// 是否锁住当前打开的非常驻UI（World类型UI无效），锁住后打开其他非常驻UI将无法顶掉当前打开的UI，使其显示于绝对顶端
        /// </summary>
        bool IsLockTemporaryUI { get; set; }
        /// <summary>
        /// 是否隐藏所有UI实体
        /// </summary>
        bool IsHideAll { get; set; }

        /// <summary>
        /// 预加载常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <returns>加载协程</returns>
        Coroutine PreloadingResidentUI(Type type);
        /// <summary>
        /// 预加载非常驻UI
        /// </summary>
        /// <param name="type">非常驻UI逻辑类</param>
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
        /// 打开非常驻UI
        /// </summary>
        /// <param name="type">非常驻UI逻辑类</param>
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
    }
}