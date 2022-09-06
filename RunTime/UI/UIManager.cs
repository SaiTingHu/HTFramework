using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.UI)]
    public sealed class UIManager : InternalModuleBase<IUIHelper>
    {
        /// <summary>
        /// 是否启用Overlay类型的UI【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableOverlayUI = true;
        /// <summary>
        /// 是否启用Camera类型的UI【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableCameraUI = false;
        /// <summary>
        /// 是否启用World类型的UI【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsEnableWorldUI = false;
        /// <summary>
        /// 当前定义的UI名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<string> DefineUINames = new List<string>();
        /// <summary>
        /// 当前定义的UI实体【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<GameObject> DefineUIEntitys = new List<GameObject>();
        
        /// <summary>
        /// Camera类型UI的摄像机
        /// </summary>
        public Camera UICamera
        {
            get
            {
                return _helper.UICamera;
            }
        }
        /// <summary>
        /// Overlay类型的UI根节点
        /// </summary>
        public RectTransform OverlayUIRoot
        {
            get
            {
                return _helper.OverlayUIRoot;
            }
        }
        /// <summary>
        /// Camera类型的UI根节点
        /// </summary>
        public RectTransform CameraUIRoot
        {
            get
            {
                return _helper.CameraUIRoot;
            }
        }
        /// <summary>
        /// World类型的UI域根节点
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <returns>域根节点</returns>
        public RectTransform WorldUIDomainRoot(string domainName)
        {
            return _helper.WorldUIDomainRoot(domainName);
        }
        /// <summary>
        /// 是否锁住当前打开的非常驻UI（World类型UI无效），锁住后打开其他非常驻UI将无法顶掉当前打开的UI，使其显示于绝对顶端
        /// </summary>
        public bool IsLockTemporaryUI
        {
            get
            {
                return _helper.IsLockTemporaryUI;
            }
            set
            {
                _helper.IsLockTemporaryUI = value;
            }
        }
        /// <summary>
        /// 是否隐藏所有UI实体
        /// </summary>
        public bool IsHideAll
        {
            set
            {
                _helper.IsHideAll = value;
            }
            get
            {
                return _helper.IsHideAll;
            }
        }
        /// <summary>
        /// 是否显示全屏遮罩
        /// </summary>
        public bool IsDisplayMask
        {
            set
            {
                _helper.IsDisplayMask = value;
            }
            get
            {
                return _helper.IsDisplayMask;
            }
        }
        
        /// <summary>
        /// 预加载UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingUI<T>() where T : UILogicBase
        {
            return PreloadingUI(typeof(T));
        }
        /// <summary>
        /// 预加载UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingUI(Type type)
        {
            if (type.IsAbstract)
                return null;

            if (type.IsSubclassOf(typeof(UILogicResident)))
            {
                return _helper.PreloadingResidentUI(type);
            }
            else if (type.IsSubclassOf(typeof(UILogicTemporary)))
            {
                return _helper.PreloadingTemporaryUI(type);
            }
            return null;
        }
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenUI<T>(params object[] args) where T : UILogicBase
        {
            return OpenUI(typeof(T), args);
        }
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenUI(Type type, params object[] args)
        {
            if (type.IsAbstract)
                return null;

            if (type.IsSubclassOf(typeof(UILogicResident)))
            {
                return _helper.OpenResidentUI(type, args);
            }
            else if (type.IsSubclassOf(typeof(UILogicTemporary)))
            {
                return _helper.OpenTemporaryUI(type, args);
            }
            return null;
        }
        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        /// <returns>UI逻辑对象</returns>
        public T GetOpenedUI<T>() where T : UILogicBase
        {
            return GetOpenedUI(typeof(T)) as T;
        }
        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        public UILogicBase GetOpenedUI(Type type)
        {
            if (type.IsAbstract)
                return null;

            if (type.IsSubclassOf(typeof(UILogicBase)))
            {
                return _helper.GetOpenedUI(type);
            }
            return null;
        }
        /// <summary>
        /// 获取UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        /// <returns>UI逻辑对象</returns>
        internal T GetUI<T>() where T : UILogicBase
        {
            return GetUI(typeof(T)) as T;
        }
        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        internal UILogicBase GetUI(Type type)
        {
            if (type.IsAbstract)
                return null;

            if (type.IsSubclassOf(typeof(UILogicBase)))
            {
                return _helper.GetUI(type);
            }
            return null;
        }
        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        public void PlaceTopUI<T>() where T : UILogicResident
        {
            PlaceTopUI(typeof(T));
        }
        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        public void PlaceTopUI(Type type)
        {
            if (type.IsAbstract)
                return;

            if (type.IsSubclassOf(typeof(UILogicResident)))
            {
                _helper.PlaceTopUI(type);
            }
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        public void CloseUI<T>() where T : UILogicBase
        {
            CloseUI(typeof(T));
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        public void CloseUI(Type type)
        {
            if (type.IsAbstract)
                return;

            if (type.IsSubclassOf(typeof(UILogicBase)))
            {
                _helper.CloseUI(type);
            }
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        public void DestroyUI<T>() where T : UILogicBase
        {
            DestroyUI(typeof(T));
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        public void DestroyUI(Type type)
        {
            if (type.IsAbstract)
                return;

            if (type.IsSubclassOf(typeof(UILogicBase)))
            {
                _helper.DestroyUI(type);
            }
        }
    }
}