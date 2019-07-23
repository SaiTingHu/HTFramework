using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UIManager : ModuleManager
    {
        /// <summary>
        /// 是否启用Overlay类型的UI【只在Inspector面板设置有效，代码中设置无效】
        /// </summary>
        public bool IsEnableOverlayUI = true;
        /// <summary>
        /// 是否启用Camera类型的UI【只在Inspector面板设置有效，代码中设置无效】
        /// </summary>
        public bool IsEnableCameraUI = false;
        /// <summary>
        /// 是否启用World类型的UI【只在Inspector面板设置有效，代码中设置无效】
        /// </summary>
        public bool IsEnableWorldUI = false;

        //当前打开的Overlay类型的非常驻UI（非常驻UI同时只能打开一个）
        private UILogicTemporary _currentOverlayTemporaryUI;
        //所有Overlay类型的UI
        private Dictionary<Type, UILogic> _overlayUIs = new Dictionary<Type, UILogic>();
        //当前打开的Camera类型的非常驻UI（非常驻UI同时只能打开一个）
        private UILogicTemporary _currentCameraTemporaryUI;
        //所有Camera类型的UI
        private Dictionary<Type, UILogic> _cameraUIs = new Dictionary<Type, UILogic>();
        //所有World类型的UI
        private Dictionary<string, WorldUIDomain> _worldUIs = new Dictionary<string, WorldUIDomain>();

        private GameObject _UIEntity;
        private Transform _overlayUIRoot;
        private RectTransform _overlayUIRootRect;
        private Transform _overlayResidentPanel;
        private Transform _overlayTemporaryPanel;
        private Transform _cameraUIRoot;
        private RectTransform _cameraUIRootRect;
        private Transform _cameraResidentPanel;
        private Transform _cameraTemporaryPanel;
        private Transform _worldUIRoot;

        public override void OnInitialization()
        {
            base.OnInitialization();

            _UIEntity = transform.FindChildren("UIEntity");
            _overlayUIRoot = _UIEntity.transform.Find("OverlayUIRoot");
            _overlayUIRootRect = _overlayUIRoot.rectTransform();
            _overlayResidentPanel = _overlayUIRoot.Find("ResidentPanel");
            _overlayTemporaryPanel = _overlayUIRoot.Find("TemporaryPanel");
            _cameraUIRoot = _UIEntity.transform.Find("CameraUIRoot");
            _cameraUIRootRect = _cameraUIRoot.rectTransform();
            _cameraResidentPanel = _cameraUIRoot.Find("ResidentPanel");
            _cameraTemporaryPanel = _cameraUIRoot.Find("TemporaryPanel");
            _worldUIRoot = _UIEntity.transform.Find("WorldUIRoot");
            UICamera = _UIEntity.GetComponentByChild<Camera>("UICamera");

            _overlayUIRoot.gameObject.SetActive(IsEnableOverlayUI);
            _cameraUIRoot.gameObject.SetActive(IsEnableCameraUI);
            UICamera.gameObject.SetActive(IsEnableCameraUI);
            _worldUIRoot.gameObject.SetActive(IsEnableWorldUI);

            //创建所有UI的逻辑对象
            List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(typeof(UILogicResident)) || types[i].IsSubclassOf(typeof(UILogicTemporary)))
                {
                    UIResourceAttribute attribute = types[i].GetCustomAttribute<UIResourceAttribute>();
                    if (attribute != null)
                    {
                        switch (attribute.EntityType)
                        {
                            case UIType.Overlay:
                                if (IsEnableOverlayUI)
                                {
                                    _overlayUIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogic);
                                }
                                break;
                            case UIType.Camera:
                                if (IsEnableCameraUI)
                                {
                                    _cameraUIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogic);
                                }
                                break;
                            case UIType.World:
                                if (IsEnableWorldUI)
                                {
                                    if (!_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                                    {
                                        _worldUIs.Add(attribute.WorldUIDomainName, new WorldUIDomain(attribute.WorldUIDomainName, _worldUIRoot.FindChildren("CanvasTem")));
                                    }
                                    _worldUIs[attribute.WorldUIDomainName].Injection(types[i]);
                                }
                                break;
                        }
                    }
                    else
                    {
                        GlobalTools.LogError(string.Format("创建UI逻辑对象失败：UI逻辑类 {0} 丢失 UIResourceAttribute 标记！", types[i].Name));
                    }
                }
            }
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            if (IsEnableOverlayUI)
            {
                foreach (KeyValuePair<Type, UILogic> ui in _overlayUIs)
                {
                    if (ui.Value.IsOpened)
                    {
                        ui.Value.OnUpdate();
                    }
                }
            }

            if (IsEnableCameraUI)
            {
                foreach (KeyValuePair<Type, UILogic> ui in _cameraUIs)
                {
                    if (ui.Value.IsOpened)
                    {
                        ui.Value.OnUpdate();
                    }
                }
            }

            if (IsEnableWorldUI)
            {
                foreach (KeyValuePair<string, WorldUIDomain> ui in _worldUIs)
                {
                    ui.Value.Refresh();
                }
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            foreach (KeyValuePair<Type, UILogic> ui in _overlayUIs)
            {
                UILogic uiLogic = ui.Value;

                if (!uiLogic.IsCreated)
                {
                    return;
                }

                uiLogic.OnDestroy();
                Destroy(uiLogic.UIEntity);
                uiLogic.UIEntity = null;
            }
            _overlayUIs.Clear();

            foreach (KeyValuePair<Type, UILogic> ui in _cameraUIs)
            {
                UILogic uiLogic = ui.Value;

                if (!uiLogic.IsCreated)
                {
                    return;
                }

                uiLogic.OnDestroy();
                Destroy(uiLogic.UIEntity);
                uiLogic.UIEntity = null;
            }
            _cameraUIs.Clear();

            foreach (KeyValuePair<string, WorldUIDomain> ui in _worldUIs)
            {
                ui.Value.Termination();
            }
            _worldUIs.Clear();
        }

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
            if (_worldUIs.ContainsKey(domainName))
            {
                return _worldUIs[domainName].WorldUIRoot;
            }
            else
            {
                GlobalTools.LogError(string.Format("获取世界UI域的根节点失败：不存在名为 {0} 的世界UI域！", domainName));
                return null;
            }
        }

        /// <summary>
        /// 是否隐藏所有UI实体
        /// </summary>
        public bool HideAll
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
        /// 预加载常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        public void PreloadingResidentUI<T>() where T : UILogicResident
        {
            PreloadingResidentUI(typeof(T));
        }

        /// <summary>
        /// 预加载常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        public void PreloadingResidentUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogic ui = _overlayUIs[type];

                            if (!ui.IsCreated)
                            {
                                Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _overlayResidentPanel, null, (obj) =>
                                {
                                    ui.UIEntity = obj;
                                    ui.OnInit();
                                }, true);
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogic ui = _cameraUIs[type];

                            if (!ui.IsCreated)
                            {
                                Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _cameraResidentPanel, null, (obj) =>
                                {
                                    ui.UIEntity = obj;
                                    ui.OnInit();
                                }, true);
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].PreloadingResidentUI(type);
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("预加载UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 预加载非常驻UI
        /// </summary>
        /// <typeparam name="T">非常驻UI逻辑类</typeparam>
        public void PreloadingTemporaryUI<T>() where T : UILogicTemporary
        {
            PreloadingTemporaryUI(typeof(T));
        }

        /// <summary>
        /// 预加载非常驻UI
        /// </summary>
        /// <param name="type">非常驻UI逻辑类</param>
        public void PreloadingTemporaryUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogic ui = _overlayUIs[type];

                            if (!ui.IsCreated)
                            {
                                Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _overlayTemporaryPanel, null, (obj) =>
                                {
                                    ui.UIEntity = obj;
                                    ui.OnInit();
                                }, true);
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogic ui = _cameraUIs[type];

                            if (!ui.IsCreated)
                            {
                                Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _cameraTemporaryPanel, null, (obj) =>
                                {
                                    ui.UIEntity = obj;
                                    ui.OnInit();
                                }, true);
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].PreloadingTemporaryUI(type);
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("预加载UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 打开常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        /// <param name="args">可选参数</param>
        public void OpenResidentUI<T>(params object[] args) where T : UILogicResident
        {
            OpenResidentUI(typeof(T), args);
        }

        /// <summary>
        /// 打开常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <param name="args">可选参数</param>
        public void OpenResidentUI(Type type, params object[] args)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicResident ui = _overlayUIs[type] as UILogicResident;

                            if (ui.IsOpened)
                            {
                                return;
                            }

                            if (!ui.IsCreated)
                            {
                                Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _overlayResidentPanel, null, (obj) =>
                                {
                                    ui.UIEntity = obj;
                                    ui.UIEntity.transform.SetAsLastSibling();
                                    ui.UIEntity.SetActive(true);
                                    ui.OnInit();
                                    ui.OnOpen(args);
                                    ui.OnPlaceTop();
                                }, true);
                            }
                            else
                            {
                                ui.UIEntity.transform.SetAsLastSibling();
                                ui.UIEntity.SetActive(true);
                                ui.OnOpen(args);
                                ui.OnPlaceTop();
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicResident ui = _cameraUIs[type] as UILogicResident;

                            if (ui.IsOpened)
                            {
                                return;
                            }

                            if (!ui.IsCreated)
                            {
                                Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _cameraResidentPanel, null, (obj) =>
                                {
                                    ui.UIEntity = obj;
                                    ui.UIEntity.transform.SetAsLastSibling();
                                    ui.UIEntity.SetActive(true);
                                    ui.OnInit();
                                    ui.OnOpen(args);
                                    ui.OnPlaceTop();
                                }, true);
                            }
                            else
                            {
                                ui.UIEntity.transform.SetAsLastSibling();
                                ui.UIEntity.SetActive(true);
                                ui.OnOpen(args);
                                ui.OnPlaceTop();
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].OpenResidentUI(type);
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("打开UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 打开非常驻UI
        /// </summary>
        /// <typeparam name="T">非常驻UI逻辑类</typeparam>
        /// <param name="args">可选参数</param>
        public void OpenTemporaryUI<T>(params object[] args) where T : UILogicTemporary
        {
            OpenTemporaryUI(typeof(T), args);
        }

        /// <summary>
        /// 打开非常驻UI
        /// </summary>
        /// <param name="type">非常驻UI逻辑类</param>
        /// <param name="args">可选参数</param>
        public void OpenTemporaryUI(Type type, params object[] args)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicTemporary ui = _overlayUIs[type] as UILogicTemporary;

                            if (ui.IsOpened)
                            {
                                return;
                            }

                            if (_currentOverlayTemporaryUI != null && _currentOverlayTemporaryUI.IsOpened)
                            {
                                _currentOverlayTemporaryUI.UIEntity.SetActive(false);
                                _currentOverlayTemporaryUI.OnClose();
                                _currentOverlayTemporaryUI = null;
                            }

                            if (!ui.IsCreated)
                            {
                                Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _overlayTemporaryPanel, null, (obj) =>
                                {
                                    ui.UIEntity = obj;
                                    ui.UIEntity.transform.SetAsLastSibling();
                                    ui.UIEntity.SetActive(true);
                                    ui.OnInit();
                                    ui.OnOpen(args);
                                    _currentOverlayTemporaryUI = ui;
                                }, true);
                            }
                            else
                            {
                                ui.UIEntity.transform.SetAsLastSibling();
                                ui.UIEntity.SetActive(true);
                                ui.OnOpen(args);
                                _currentOverlayTemporaryUI = ui;
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicTemporary ui = _cameraUIs[type] as UILogicTemporary;

                            if (ui.IsOpened)
                            {
                                return;
                            }

                            if (_currentCameraTemporaryUI != null && _currentCameraTemporaryUI.IsOpened)
                            {
                                _currentCameraTemporaryUI.UIEntity.SetActive(false);
                                _currentCameraTemporaryUI.OnClose();
                                _currentCameraTemporaryUI = null;
                            }

                            if (!ui.IsCreated)
                            {
                                Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _cameraTemporaryPanel, null, (obj) =>
                                {
                                    ui.UIEntity = obj;
                                    ui.UIEntity.transform.SetAsLastSibling();
                                    ui.UIEntity.SetActive(true);
                                    ui.OnInit();
                                    ui.OnOpen(args);
                                    _currentCameraTemporaryUI = ui;
                                }, true);
                            }
                            else
                            {
                                ui.UIEntity.transform.SetAsLastSibling();
                                ui.UIEntity.SetActive(true);
                                ui.OnOpen(args);
                                _currentCameraTemporaryUI = ui;
                            }
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].OpenTemporaryUI(type);
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("打开UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        /// <returns>UI逻辑对象</returns>
        public T GetOpenedUI<T>() where T : UILogic
        {
            return GetOpenedUI(typeof(T)) as T;
        }

        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        public UILogic GetOpenedUI(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogic ui = _overlayUIs[type];

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
                            GlobalTools.LogError(string.Format("获取UI失败：UI对象 {0} 并未存在，或并未打开！", type.Name));
                            return null;
                        }
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogic ui = _cameraUIs[type];

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
                            GlobalTools.LogError(string.Format("获取UI失败：UI对象 {0} 并未存在，或并未打开！", type.Name));
                            return null;
                        }
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return _worldUIs[attribute.WorldUIDomainName].GetOpenedUI(type);
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("获取UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                            return null;
                        }
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        public void PlaceTop<T>() where T : UILogicResident
        {
            PlaceTop(typeof(T));
        }

        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        public void PlaceTop(Type type)
        {
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicResident ui = _overlayUIs[type] as UILogicResident;

                            if (!ui.IsOpened)
                            {
                                return;
                            }

                            ui.UIEntity.transform.SetAsLastSibling();
                            ui.OnPlaceTop();
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("置顶UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicResident ui = _cameraUIs[type] as UILogicResident;

                            if (!ui.IsOpened)
                            {
                                return;
                            }

                            ui.UIEntity.transform.SetAsLastSibling();
                            ui.OnPlaceTop();
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("置顶UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].PlaceTop(type);
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("置顶UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        public void CloseUI<T>() where T : UILogic
        {
            CloseUI(typeof(T));
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
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogic ui = _overlayUIs[type];

                            if (!ui.IsCreated)
                            {
                                return;
                            }

                            if (!ui.IsOpened)
                            {
                                return;
                            }

                            ui.UIEntity.SetActive(false);
                            ui.OnClose();
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("关闭UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogic ui = _cameraUIs[type];

                            if (!ui.IsCreated)
                            {
                                return;
                            }

                            if (!ui.IsOpened)
                            {
                                return;
                            }

                            ui.UIEntity.SetActive(false);
                            ui.OnClose();
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("关闭UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].CloseUI(type);
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("关闭UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <typeparam name="T">UI逻辑类</typeparam>
        public void DestroyUI<T>() where T : UILogic
        {
            DestroyUI(typeof(T));
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
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogic ui = _overlayUIs[type];

                            if (!ui.IsCreated)
                            {
                                return;
                            }

                            if (ui.IsOpened)
                            {
                                return;
                            }

                            ui.OnDestroy();
                            Destroy(ui.UIEntity);
                            ui.UIEntity = null;
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("销毁UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogic ui = _cameraUIs[type];

                            if (!ui.IsCreated)
                            {
                                return;
                            }

                            if (ui.IsOpened)
                            {
                                return;
                            }

                            ui.OnDestroy();
                            Destroy(ui.UIEntity);
                            ui.UIEntity = null;
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("销毁UI失败：UI对象 {0} 并未存在！", type.Name));
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].DestroyUI(type);
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("销毁UI失败：UI对象 {0} 的域 {1} 并未存在！", type.Name, attribute.WorldUIDomainName));
                        }
                        break;
                }
            }
        }  

        /// <summary>
        /// 世界UI的域
        /// </summary>
        private sealed class WorldUIDomain
        {
            //域的名称
            public string Name { get; private set; }

            //当前打开的World类型的非常驻UI（非常驻UI同时只能打开一个）
            private UILogicTemporary _currentWorldTemporaryUI;
            //所有World类型的UI
            private Dictionary<Type, UILogic> _worldUIs = new Dictionary<Type, UILogic>();

            private Transform _worldUIRoot;
            private RectTransform _worldUIRootRect;
            private Transform _worldResidentPanel;
            private Transform _worldTemporaryPanel;

            public WorldUIDomain(string name, GameObject canvasTem)
            {
                Name = name;

                _worldUIRoot = Main.CloneGameObject(canvasTem, true).transform;
                _worldUIRootRect = _worldUIRoot.rectTransform();
                _worldResidentPanel = _worldUIRoot.Find("ResidentPanel");
                _worldTemporaryPanel = _worldUIRoot.Find("TemporaryPanel");
                _worldUIRoot.gameObject.SetActive(true);
            }

            /// <summary>
            /// 刷新域
            /// </summary>
            public void Refresh()
            {
                foreach (KeyValuePair<Type, UILogic> ui in _worldUIs)
                {
                    if (ui.Value.IsOpened)
                    {
                        ui.Value.OnUpdate();
                    }
                }
            }

            /// <summary>
            /// 销毁域
            /// </summary>
            public void Termination()
            {
                foreach (KeyValuePair<Type, UILogic> ui in _worldUIs)
                {
                    UILogic uiLogic = ui.Value;

                    if (!uiLogic.IsCreated)
                    {
                        return;
                    }

                    uiLogic.OnDestroy();
                    Destroy(uiLogic.UIEntity);
                    uiLogic.UIEntity = null;
                }
                _worldUIs.Clear();

                Destroy(_worldUIRoot.gameObject);
            }

            /// <summary>
            /// 域的UI根节点
            /// </summary>
            public RectTransform WorldUIRoot
            {
                get
                {
                    return _worldUIRootRect;
                }
            }

            /// <summary>
            /// 注入UI逻辑类到域
            /// </summary>
            /// <param name="uiLogicType">UI逻辑类型</param>
            public void Injection(Type uiLogicType)
            {
                if (!_worldUIs.ContainsKey(uiLogicType))
                {
                    _worldUIs.Add(uiLogicType, Activator.CreateInstance(uiLogicType) as UILogic);
                }
            }

            /// <summary>
            /// 预加载常驻UI
            /// </summary>
            /// <param name="type">常驻UI逻辑类</param>
            public void PreloadingResidentUI(Type type)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogic ui = _worldUIs[type];

                    if (!ui.IsCreated)
                    {
                        Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldResidentPanel, null, (obj) =>
                        {
                            ui.UIEntity = obj;
                            ui.OnInit();
                        }, true);
                    }
                }
                else
                {
                    GlobalTools.LogError(string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                }
            }

            /// <summary>
            /// 预加载非常驻UI
            /// </summary>
            /// <param name="type">非常驻UI逻辑类</param>
            public void PreloadingTemporaryUI(Type type)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogic ui = _worldUIs[type];

                    if (!ui.IsCreated)
                    {
                        Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldTemporaryPanel, null, (obj) =>
                        {
                            ui.UIEntity = obj;
                            ui.OnInit();
                        }, true);
                    }
                }
                else
                {
                    GlobalTools.LogError(string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
                }
            }

            /// <summary>
            /// 打开常驻UI
            /// </summary>
            /// <param name="type">常驻UI逻辑类</param>
            /// <param name="args">可选参数</param>
            public void OpenResidentUI(Type type, params object[] args)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogicResident ui = _worldUIs[type] as UILogicResident;

                    if (ui.IsOpened)
                    {
                        return;
                    }

                    if (!ui.IsCreated)
                    {
                        Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldResidentPanel, null, (obj) =>
                        {
                            ui.UIEntity = obj;
                            ui.UIEntity.transform.SetAsLastSibling();
                            ui.UIEntity.SetActive(true);
                            ui.OnInit();
                            ui.OnOpen(args);
                            ui.OnPlaceTop();
                        }, true);
                    }
                    else
                    {
                        ui.UIEntity.transform.SetAsLastSibling();
                        ui.UIEntity.SetActive(true);
                        ui.OnOpen(args);
                        ui.OnPlaceTop();
                    }
                }
                else
                {
                    GlobalTools.LogError(string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                }
            }

            /// <summary>
            /// 打开非常驻UI
            /// </summary>
            /// <param name="type">非常驻UI逻辑类</param>
            /// <param name="args">可选参数</param>
            public void OpenTemporaryUI(Type type, params object[] args)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogicTemporary ui = _worldUIs[type] as UILogicTemporary;

                    if (ui.IsOpened)
                    {
                        return;
                    }

                    if (_currentWorldTemporaryUI != null && _currentWorldTemporaryUI.IsOpened)
                    {
                        _currentWorldTemporaryUI.UIEntity.SetActive(false);
                        _currentWorldTemporaryUI.OnClose();
                        _currentWorldTemporaryUI = null;
                    }

                    if (!ui.IsCreated)
                    {
                        Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldTemporaryPanel, null, (obj) =>
                        {
                            ui.UIEntity = obj;
                            ui.UIEntity.transform.SetAsLastSibling();
                            ui.UIEntity.SetActive(true);
                            ui.OnInit();
                            ui.OnOpen(args);
                            _currentWorldTemporaryUI = ui;
                        }, true);
                    }
                    else
                    {
                        ui.UIEntity.transform.SetAsLastSibling();
                        ui.UIEntity.SetActive(true);
                        ui.OnOpen(args);
                        _currentWorldTemporaryUI = ui;
                    }
                }
                else
                {
                    GlobalTools.LogError(string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
                }
            }

            /// <summary>
            /// 获取已经打开的UI
            /// </summary>
            /// <param name="type">UI逻辑类</param>
            /// <returns>UI逻辑对象</returns>
            public UILogic GetOpenedUI(Type type)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogic ui = _worldUIs[type];

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
                    GlobalTools.LogError(string.Format("获取UI失败：UI对象 {0} 并未存在，或并未打开！", type.Name));
                    return null;
                }
            }

            /// <summary>
            /// 置顶常驻UI
            /// </summary>
            /// <param name="type">常驻UI逻辑类</param>
            public void PlaceTop(Type type)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogicResident ui = _worldUIs[type] as UILogicResident;

                    if (!ui.IsOpened)
                    {
                        return;
                    }

                    ui.UIEntity.transform.SetAsLastSibling();
                    ui.OnPlaceTop();
                }
                else
                {
                    GlobalTools.LogError(string.Format("置顶UI失败：UI对象 {0} 并未存在！", type.Name));
                }
            }

            /// <summary>
            /// 关闭UI
            /// </summary>
            /// <param name="type">UI逻辑类</param>
            public void CloseUI(Type type)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogic ui = _worldUIs[type];

                    if (!ui.IsCreated)
                    {
                        return;
                    }

                    if (!ui.IsOpened)
                    {
                        return;
                    }

                    ui.UIEntity.SetActive(false);
                    ui.OnClose();
                }
                else
                {
                    GlobalTools.LogError(string.Format("关闭UI失败：UI对象 {0} 并未存在！", type.Name));
                }
            }

            /// <summary>
            /// 销毁UI
            /// </summary>
            /// <param name="type">UI逻辑类</param>
            public void DestroyUI(Type type)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogic ui = _worldUIs[type];

                    if (!ui.IsCreated)
                    {
                        return;
                    }

                    if (ui.IsOpened)
                    {
                        return;
                    }

                    ui.OnDestroy();
                    Destroy(ui.UIEntity);
                    ui.UIEntity = null;
                }
                else
                {
                    GlobalTools.LogError(string.Format("销毁UI失败：UI对象 {0} 并未存在！", type.Name));
                }
            }
        }
    }
}
