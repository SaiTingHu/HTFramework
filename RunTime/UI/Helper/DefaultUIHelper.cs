using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的UI管理器助手
    /// </summary>
    public sealed class DefaultUIHelper : IUIHelper
    {
        //UI管理器
        private UIManager _module;
        //当前定义的UI与实体对应关系
        private Dictionary<string, GameObject> _defineUIAndEntitys = new Dictionary<string, GameObject>();
        //当前打开的Overlay类型的非常驻UI（非常驻UI同时只能打开一个）
        private UILogicTemporary _currentOverlayTemporaryUI;
        //当前打开的Camera类型的非常驻UI（非常驻UI同时只能打开一个）
        private UILogicTemporary _currentCameraTemporaryUI;
        //所有UI根节点
        private GameObject _UIEntity;
        //Overlay类型的UI根节点
        private Transform _overlayUIRoot;
        private RectTransform _overlayUIRootRect;
        private Transform _overlayResidentPanel;
        private Transform _overlayTemporaryPanel;
        //Camera类型的UI根节点
        private Transform _cameraUIRoot;
        private RectTransform _cameraUIRootRect;
        private Transform _cameraResidentPanel;
        private Transform _cameraTemporaryPanel;
        //World类型的UI根节点
        private Transform _worldUIRoot;
        //遮罩面板
        private GameObject _maskPanel;

        /// <summary>
        /// UI管理器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 所有Overlay类型的UI
        /// </summary>
        public Dictionary<Type, UILogicBase> OverlayUIs { get; private set; } = new Dictionary<Type, UILogicBase>();
        /// <summary>
        /// 所有Camera类型的UI
        /// </summary>
        public Dictionary<Type, UILogicBase> CameraUIs { get; private set; } = new Dictionary<Type, UILogicBase>();
        /// <summary>
        /// 所有World类型的UI
        /// </summary>
        public Dictionary<string, UIWorldDomain> WorldUIs { get; private set; } = new Dictionary<string, UIWorldDomain>();
        /// <summary>
        /// Camera类型UI的摄像机
        /// </summary>
        public Camera UICamera { get; private set; }
        /// <summary>
        /// Overlay类型的UI根节点
        /// </summary>
        public RectTransform OverlayUIRoot
        {
            get
            {
                return _overlayUIRootRect;
            }
        }
        /// <summary>
        /// Camera类型的UI根节点
        /// </summary>
        public RectTransform CameraUIRoot
        {
            get
            {
                return _cameraUIRootRect;
            }
        }
        /// <summary>
        /// World类型的UI域根节点
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <returns>域根节点</returns>
        public RectTransform WorldUIDomainRoot(string domainName)
        {
            if (WorldUIs.ContainsKey(domainName))
            {
                return WorldUIs[domainName].WorldUIRoot;
            }
            else
            {
                Log.Warning("获取世界UI域的根节点失败：不存在名为 " + domainName + " 的世界UI域！");
                return null;
            }
        }
        /// <summary>
        /// 是否锁住当前打开的非常驻UI（World类型UI无效），锁住后打开其他非常驻UI将无法顶掉当前打开的UI，使其显示于绝对顶端
        /// </summary>
        public bool IsLockTemporaryUI { get; set; } = false;
        /// <summary>
        /// 是否隐藏所有UI实体
        /// </summary>
        public bool IsHideAll
        {
            set
            {
                _UIEntity.SetActive(!value);
            }
            get
            {
                return !_UIEntity.activeSelf;
            }
        }
        /// <summary>
        /// 是否显示全屏遮罩
        /// </summary>
        public bool IsDisplayMask
        {
            set
            {
                _maskPanel.SetActive(value);
            }
            get
            {
                return _maskPanel.activeSelf;
            }
        }

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            _module = Module as UIManager;

            for (int i = 0; i < _module.DefineUINames.Count; i++)
            {
                if (!_defineUIAndEntitys.ContainsKey(_module.DefineUINames[i]))
                {
                    _defineUIAndEntitys.Add(_module.DefineUINames[i], _module.DefineUIEntitys[i]);
                }
            }

            _UIEntity = _module.transform.FindChildren("UIEntity");
            _overlayUIRoot = _UIEntity.transform.Find("OverlayUIRoot");
            _overlayUIRootRect = _overlayUIRoot.rectTransform();
            _overlayResidentPanel = _overlayUIRoot.Find("ResidentPanel");
            _overlayTemporaryPanel = _overlayUIRoot.Find("TemporaryPanel");
            _cameraUIRoot = _UIEntity.transform.Find("CameraUIRoot");
            _cameraUIRootRect = _cameraUIRoot.rectTransform();
            _cameraResidentPanel = _cameraUIRoot.Find("ResidentPanel");
            _cameraTemporaryPanel = _cameraUIRoot.Find("TemporaryPanel");
            _worldUIRoot = _UIEntity.transform.Find("WorldUIRoot");
            _maskPanel = _overlayUIRoot.FindChildren("MaskPanel");
            UICamera = _UIEntity.GetComponentByChild<Camera>("UICamera");

            _overlayUIRoot.gameObject.SetActive(_module.IsEnableOverlayUI);
            _cameraUIRoot.gameObject.SetActive(_module.IsEnableCameraUI);
            UICamera.gameObject.SetActive(_module.IsEnableCameraUI);
            _worldUIRoot.gameObject.SetActive(_module.IsEnableWorldUI);

            //创建所有UI的逻辑对象
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return (type.IsSubclassOf(typeof(UILogicResident)) || type.IsSubclassOf(typeof(UILogicTemporary))) && !type.IsAbstract;
            });
            for (int i = 0; i < types.Count; i++)
            {
                UIResourceAttribute attribute = types[i].GetCustomAttribute<UIResourceAttribute>();
                if (attribute != null)
                {
                    switch (attribute.EntityType)
                    {
                        case UIType.Overlay:
                            if (_module.IsEnableOverlayUI)
                            {
                                OverlayUIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogicBase);
                            }
                            break;
                        case UIType.Camera:
                            if (_module.IsEnableCameraUI)
                            {
                                CameraUIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogicBase);
                            }
                            break;
                        case UIType.World:
                            if (_module.IsEnableWorldUI)
                            {
                                if (!WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                                {
                                    WorldUIs.Add(attribute.WorldUIDomainName, new UIWorldDomain(attribute.WorldUIDomainName, _worldUIRoot.FindChildren("CanvasTem")));
                                }
                                WorldUIs[attribute.WorldUIDomainName].Injection(types[i]);
                            }
                            break;
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.UI, "创建UI逻辑对象失败：UI逻辑类 " + types[i].Name + " 丢失 UIResourceAttribute 标记！");
                }
            }
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
            if (_module.IsEnableOverlayUI)
            {
                foreach (var ui in OverlayUIs)
                {
                    if (ui.Value.IsOpened)
                    {
                        ui.Value.OnUpdate();
                    }
                }
            }

            if (_module.IsEnableCameraUI)
            {
                foreach (var ui in CameraUIs)
                {
                    if (ui.Value.IsOpened)
                    {
                        ui.Value.OnUpdate();
                    }
                }
            }

            if (_module.IsEnableWorldUI)
            {
                foreach (var ui in WorldUIs)
                {
                    ui.Value.OnUpdate();
                }
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            _defineUIAndEntitys.Clear();

            foreach (var ui in OverlayUIs)
            {
                DestroyUIEntity(ui.Value);
            }
            OverlayUIs.Clear();

            foreach (var ui in CameraUIs)
            {
                DestroyUIEntity(ui.Value);
            }
            CameraUIs.Clear();

            foreach (var ui in WorldUIs)
            {
                ui.Value.OnTerminate();
            }
            WorldUIs.Clear();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }
        
        /// <summary>
        /// 预加载常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingResidentUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            return CreateUIEntity(attribute, type.FullName, OverlayUIs[type], _overlayResidentPanel);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            return CreateUIEntity(attribute, type.FullName, CameraUIs[type], _cameraResidentPanel);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return WorldUIs[attribute.WorldUIDomainName].PreloadingResidentUI(type, _defineUIAndEntitys.ContainsKey(type.FullName) ? _defineUIAndEntitys[type.FullName] : null);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("预加载UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 预加载非常驻UI
        /// </summary>
        /// <param name="type">非常驻UI逻辑类</param>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingTemporaryUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            return CreateUIEntity(attribute, type.FullName, OverlayUIs[type], _overlayTemporaryPanel);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            return CreateUIEntity(attribute, type.FullName, CameraUIs[type], _cameraTemporaryPanel);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return WorldUIs[attribute.WorldUIDomainName].PreloadingTemporaryUI(type, _defineUIAndEntitys.ContainsKey(type.FullName) ? _defineUIAndEntitys[type.FullName] : null);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("预加载UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 打开常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenResidentUI(Type type, params object[] args)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            return CreateOpenUIEntity(attribute, type.FullName, OverlayUIs[type], _overlayResidentPanel, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            return CreateOpenUIEntity(attribute, type.FullName, CameraUIs[type], _cameraResidentPanel, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return WorldUIs[attribute.WorldUIDomainName].OpenResidentUI(type, _defineUIAndEntitys.ContainsKey(type.FullName) ? _defineUIAndEntitys[type.FullName] : null, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("打开UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 打开非常驻UI
        /// </summary>
        /// <param name="type">非常驻UI逻辑类</param>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenTemporaryUI(Type type, params object[] args)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            if (_currentOverlayTemporaryUI != null && _currentOverlayTemporaryUI.IsOpened)
                            {
                                if (IsLockTemporaryUI)
                                    return null;

                                CloseUIEntity(_currentOverlayTemporaryUI);
                                _currentOverlayTemporaryUI = null;
                            }
                            _currentOverlayTemporaryUI = OverlayUIs[type] as UILogicTemporary;

                            return CreateOpenUIEntity(attribute, type.FullName, OverlayUIs[type], _overlayTemporaryPanel, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            if (_currentCameraTemporaryUI != null && _currentCameraTemporaryUI.IsOpened)
                            {
                                if (IsLockTemporaryUI)
                                    return null;

                                CloseUIEntity(_currentCameraTemporaryUI);
                                _currentCameraTemporaryUI = null;
                            }
                            _currentCameraTemporaryUI = CameraUIs[type] as UILogicTemporary;

                            return CreateOpenUIEntity(attribute, type.FullName, CameraUIs[type], _cameraTemporaryPanel, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return WorldUIs[attribute.WorldUIDomainName].OpenTemporaryUI(type, _defineUIAndEntitys.ContainsKey(type.FullName) ? _defineUIAndEntitys[type.FullName] : null, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("打开UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        public UILogicBase GetOpenedUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            UILogicBase ui = OverlayUIs[type];

                            if (ui.IsOpened)
                            {
                                return ui;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            UILogicBase ui = CameraUIs[type];

                            if (ui.IsOpened)
                            {
                                return ui;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return WorldUIs[attribute.WorldUIDomainName].GetOpenedUI(type);
                        }
                        else
                        {
                            return null;
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        public UILogicBase GetUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            UILogicBase ui = OverlayUIs[type];
                            return ui;
                        }
                        else
                        {
                            return null;
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            UILogicBase ui = CameraUIs[type];
                            return ui;
                        }
                        else
                        {
                            return null;
                        }
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return WorldUIs[attribute.WorldUIDomainName].GetUI(type);
                        }
                        else
                        {
                            return null;
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        public void PlaceTopUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            PlaceTopUIEntity(OverlayUIs[type] as UILogicResident);
                        }
                        break;
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            PlaceTopUIEntity(CameraUIs[type] as UILogicResident);
                        }
                        break;
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            WorldUIs[attribute.WorldUIDomainName].PlaceTop(type);
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        public void CloseUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            CloseUIEntity(OverlayUIs[type]);
                        }
                        break;
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            CloseUIEntity(CameraUIs[type]);
                        }
                        break;
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            WorldUIs[attribute.WorldUIDomainName].CloseUI(type);
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        public void DestroyUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            DestroyUIEntity(OverlayUIs[type]);
                        }
                        break;
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            DestroyUIEntity(CameraUIs[type]);
                        }
                        break;
                    case UIType.World:
                        if (WorldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            WorldUIs[attribute.WorldUIDomainName].DestroyUI(type);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 创建UI实体
        /// </summary>
        /// <param name="uIResource">UI资源标记</param>
        /// <param name="uITypeName">UI逻辑类型</param>
        /// <param name="uILogic">UI逻辑类对象</param>
        /// <param name="uIParent">UI的父级</param>
        /// <returns>加载协程</returns>
        private Coroutine CreateUIEntity(UIResourceAttribute uIResource, string uITypeName, UILogicBase uILogic, Transform uIParent)
        {
            if (uILogic.IsCreated)
                return null;

            if (_defineUIAndEntitys.ContainsKey(uITypeName) && _defineUIAndEntitys[uITypeName] != null)
            {
                uILogic.UIEntity = Main.Clone(_defineUIAndEntitys[uITypeName], uIParent);
                uILogic.UIEntity.SetActive(false);
                uILogic.OnInit();
                return null;
            }
            else
            {
                return Main.m_Resource.LoadPrefab(new PrefabInfo(uIResource), uIParent, null, (obj) =>
                {
                    uILogic.UIEntity = obj;
                    uILogic.OnInit();
                }, true);
            }
        }
        /// <summary>
        /// 创建并打开UI实体
        /// </summary>
        /// <param name="uIResource">UI资源标记</param>
        /// <param name="uITypeName">UI逻辑类型</param>
        /// <param name="uILogic">UI逻辑类对象</param>
        /// <param name="uIParent">UI的父级</param>
        /// <param name="args">UI打开的参数</param>
        /// <returns>加载协程</returns>
        private Coroutine CreateOpenUIEntity(UIResourceAttribute uIResource, string uITypeName, UILogicBase uILogic, Transform uIParent, params object[] args)
        {
            if (uILogic.IsOpened)
                return null;

            if (!uILogic.IsCreated)
            {
                if (_defineUIAndEntitys.ContainsKey(uITypeName) && _defineUIAndEntitys[uITypeName] != null)
                {
                    uILogic.UIEntity = Main.Clone(_defineUIAndEntitys[uITypeName], uIParent);
                    uILogic.UIEntity.SetActive(true);
                    uILogic.OnInit();
                    uILogic.OnOpen(args);
                    PlaceTopUIEntity(uILogic as UILogicResident);
                    return null;
                }
                else
                {
                    return Main.m_Resource.LoadPrefab(new PrefabInfo(uIResource), uIParent, null, (obj) =>
                    {
                        uILogic.UIEntity = obj;
                        uILogic.UIEntity.SetActive(true);
                        uILogic.OnInit();
                        uILogic.OnOpen(args);
                        PlaceTopUIEntity(uILogic as UILogicResident);
                    }, true);
                }
            }
            else
            {
                uILogic.UIEntity.SetActive(true);
                uILogic.OnOpen(args);
                PlaceTopUIEntity(uILogic as UILogicResident);
                return null;
            }
        }
        /// <summary>
        /// 置顶常驻UI实体
        /// </summary>
        /// <param name="uILogic">UI逻辑类对象</param>
        private void PlaceTopUIEntity(UILogicResident uILogic)
        {
            if (uILogic == null)
                return;

            if (!uILogic.IsOpened)
                return;

            uILogic.UIEntity.transform.SetAsLastSibling();
            uILogic.OnPlaceTop();
        }
        /// <summary>
        /// 关闭UI实体
        /// </summary>
        /// <param name="uILogic">UI逻辑类对象</param>
        private void CloseUIEntity(UILogicBase uILogic)
        {
            if (!uILogic.IsCreated)
                return;

            if (!uILogic.IsOpened)
                return;

            uILogic.UIEntity.SetActive(false);
            uILogic.OnClose();
        }
        /// <summary>
        /// 销毁UI实体
        /// </summary>
        /// <param name="uILogic">UI逻辑类对象</param>
        private void DestroyUIEntity(UILogicBase uILogic)
        {
            if (!uILogic.IsCreated)
                return;

            uILogic.OnDestroy();
            Main.Kill(uILogic.UIEntity);
            uILogic.UIEntity = null;
        }
    }
}