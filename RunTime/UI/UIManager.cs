using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.UI)]
    public sealed class UIManager : InternalModuleBase
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
        
        private IUIHelper _helper;

        private UIManager()
        {

        }
        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IUIHelper;
        }

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
        /// 预加载常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingResidentUI<T>() where T : UILogicResident
        {
            return _helper.PreloadingResidentUI(typeof(T));
        }
        /// <summary>
        /// 预加载常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingResidentUI(Type type)
        {
            return _helper.PreloadingResidentUI(type);
        }
        /// <summary>
        /// 预加载非常驻UI
        /// </summary>
        /// <typeparam name="T">非常驻UI逻辑类</typeparam>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingTemporaryUI<T>() where T : UILogicTemporary
        {
            return _helper.PreloadingTemporaryUI(typeof(T));
        }
        /// <summary>
        /// 预加载非常驻UI
        /// </summary>
        /// <param name="type">非常驻UI逻辑类</param>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingTemporaryUI(Type type)
        {
            return _helper.PreloadingTemporaryUI(type);
        }
        /// <summary>
        /// 打开常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenResidentUI<T>(params object[] args) where T : UILogicResident
        {
            return _helper.OpenResidentUI(typeof(T), args);
        }
        /// <summary>
        /// 打开常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenResidentUI(Type type, params object[] args)
        {
            return _helper.OpenResidentUI(type, args);
        }
        /// <summary>
        /// 打开非常驻UI
        /// </summary>
        /// <typeparam name="T">非常驻UI逻辑类</typeparam>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenTemporaryUI<T>(params object[] args) where T : UILogicTemporary
        {
            return _helper.OpenTemporaryUI(typeof(T), args);
        }
        /// <summary>
        /// 打开非常驻UI
        /// </summary>
        /// <param name="type">非常驻UI逻辑类</param>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenTemporaryUI(Type type, params object[] args)
        {
            return _helper.OpenTemporaryUI(type, args);
        }
        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        /// <returns>UI逻辑对象</returns>
        public T GetOpenedUI<T>() where T : UILogicBase
        {
            return _helper.GetOpenedUI(typeof(T)) as T;
        }
        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        public UILogicBase GetOpenedUI(Type type)
        {
            return _helper.GetOpenedUI(type);
        }
        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        public void PlaceTopUI<T>() where T : UILogicResident
        {
            _helper.PlaceTopUI(typeof(T));
        }
        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        public void PlaceTopUI(Type type)
        {
            _helper.PlaceTopUI(type);
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        public void CloseUI<T>() where T : UILogicBase
        {
            _helper.CloseUI(typeof(T));
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        public void CloseUI(Type type)
        {
            _helper.CloseUI(type);
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        public void DestroyUI<T>() where T : UILogicBase
        {
            _helper.DestroyUI(typeof(T));
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        public void DestroyUI(Type type)
        {
            _helper.DestroyUI(type);
        }
    }
}