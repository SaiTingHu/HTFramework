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

        //当前定义的UI与实体对应关系
        private Dictionary<string, GameObject> _defineUIAndEntitys = new Dictionary<string, GameObject>();

        //当前打开的Overlay类型的非常驻UI（非常驻UI同时只能打开一个）
        private UILogicTemporary _currentOverlayTemporaryUI;
        //所有Overlay类型的UI
        private Dictionary<Type, UILogicBase> _overlayUIs = new Dictionary<Type, UILogicBase>();
        //当前打开的Camera类型的非常驻UI（非常驻UI同时只能打开一个）
        private UILogicTemporary _currentCameraTemporaryUI;
        //所有Camera类型的UI
        private Dictionary<Type, UILogicBase> _cameraUIs = new Dictionary<Type, UILogicBase>();
        //所有World类型的UI
        private Dictionary<string, WorldUIDomain> _worldUIs = new Dictionary<string, WorldUIDomain>();

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

        public override void OnInitialization()
        {
            base.OnInitialization();

            for (int i = 0; i < DefineUINames.Count; i++)
            {
                if (!_defineUIAndEntitys.ContainsKey(DefineUINames[i]))
                {
                    _defineUIAndEntitys.Add(DefineUINames[i], DefineUIEntitys[i]);
                }
            }

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
                                    _overlayUIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogicBase);
                                }
                                break;
                            case UIType.Camera:
                                if (IsEnableCameraUI)
                                {
                                    _cameraUIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogicBase);
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
                        throw new HTFrameworkException(HTFrameworkModule.UI, "创建UI逻辑对象失败：UI逻辑类 " + types[i].Name + " 丢失 UIResourceAttribute 标记！");
                    }
                }
            }
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            if (IsEnableOverlayUI)
            {
                foreach (KeyValuePair<Type, UILogicBase> ui in _overlayUIs)
                {
                    if (ui.Value.IsOpened)
                    {
                        ui.Value.OnUpdate();
                    }
                }
            }

            if (IsEnableCameraUI)
            {
                foreach (KeyValuePair<Type, UILogicBase> ui in _cameraUIs)
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

            _defineUIAndEntitys.Clear();

            foreach (KeyValuePair<Type, UILogicBase> ui in _overlayUIs)
            {
                UILogicBase uiLogic = ui.Value;

                if (!uiLogic.IsCreated)
                {
                    continue;
                }

                uiLogic.OnDestroy();
                Destroy(uiLogic.UIEntity);
                uiLogic.UIEntity = null;
            }
            _overlayUIs.Clear();

            foreach (KeyValuePair<Type, UILogicBase> ui in _cameraUIs)
            {
                UILogicBase uiLogic = ui.Value;

                if (!uiLogic.IsCreated)
                {
                    continue;
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
                throw new HTFrameworkException(HTFrameworkModule.UI, "获取世界UI域的根节点失败：不存在名为 " + domainName + " 的世界UI域！");
            }
        }
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
        /// 预加载常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingResidentUI<T>() where T : UILogicResident
        {
            return PreloadingResidentUI(typeof(T));
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
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _overlayUIs[type];

                            if (!ui.IsCreated)
                            {
                                if (_defineUIAndEntitys.ContainsKey(type.FullName) && _defineUIAndEntitys[type.FullName] != null)
                                {
                                    ui.UIEntity = Instantiate(_defineUIAndEntitys[type.FullName], _overlayResidentPanel);
                                    ui.UIEntity.SetActive(false);
                                    ui.OnInit();
                                    return null;
                                }
                                else
                                {
                                    return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _overlayResidentPanel, null, (obj) =>
                                    {
                                        ui.UIEntity = obj;
                                        ui.OnInit();
                                    }, true);
                                }
                            }
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "预加载UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _cameraUIs[type];

                            if (!ui.IsCreated)
                            {
                                if (_defineUIAndEntitys.ContainsKey(type.FullName) && _defineUIAndEntitys[type.FullName] != null)
                                {
                                    ui.UIEntity = Instantiate(_defineUIAndEntitys[type.FullName], _cameraResidentPanel);
                                    ui.UIEntity.SetActive(false);
                                    ui.OnInit();
                                    return null;
                                }
                                else
                                {
                                    return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _cameraResidentPanel, null, (obj) =>
                                    {
                                        ui.UIEntity = obj;
                                        ui.OnInit();
                                    }, true);
                                }
                            }
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "预加载UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return _worldUIs[attribute.WorldUIDomainName].PreloadingResidentUI(type, _defineUIAndEntitys.ContainsKey(type.FullName) ? _defineUIAndEntitys[type.FullName] : null);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "预加载UI失败：UI对象 " + type.Name + " 的域 " + attribute.WorldUIDomainName + " 并未存在！");
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 预加载非常驻UI
        /// </summary>
        /// <typeparam name="T">非常驻UI逻辑类</typeparam>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingTemporaryUI<T>() where T : UILogicTemporary
        {
            return PreloadingTemporaryUI(typeof(T));
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
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _overlayUIs[type];

                            if (!ui.IsCreated)
                            {
                                if (_defineUIAndEntitys.ContainsKey(type.FullName) && _defineUIAndEntitys[type.FullName] != null)
                                {
                                    ui.UIEntity = Instantiate(_defineUIAndEntitys[type.FullName], _overlayTemporaryPanel);
                                    ui.UIEntity.SetActive(false);
                                    ui.OnInit();
                                    return null;
                                }
                                else
                                {
                                    return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _overlayTemporaryPanel, null, (obj) =>
                                    {
                                        ui.UIEntity = obj;
                                        ui.OnInit();
                                    }, true);
                                }
                            }
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "预加载UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _cameraUIs[type];

                            if (!ui.IsCreated)
                            {
                                if (_defineUIAndEntitys.ContainsKey(type.FullName) && _defineUIAndEntitys[type.FullName] != null)
                                {
                                    ui.UIEntity = Instantiate(_defineUIAndEntitys[type.FullName], _cameraTemporaryPanel);
                                    ui.UIEntity.SetActive(false);
                                    ui.OnInit();
                                    return null;
                                }
                                else
                                {
                                    return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _cameraTemporaryPanel, null, (obj) =>
                                    {
                                        ui.UIEntity = obj;
                                        ui.OnInit();
                                    }, true);
                                }
                            }
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "预加载UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return _worldUIs[attribute.WorldUIDomainName].PreloadingTemporaryUI(type, _defineUIAndEntitys.ContainsKey(type.FullName) ? _defineUIAndEntitys[type.FullName] : null);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "预加载UI失败：UI对象 " + type.Name + " 的域 " + attribute.WorldUIDomainName + " 并未存在！");
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 打开常驻UI
        /// </summary>
        /// <typeparam name="T">常驻UI逻辑类</typeparam>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenResidentUI<T>(params object[] args) where T : UILogicResident
        {
            return OpenResidentUI(typeof(T), args);
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
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicResident ui = _overlayUIs[type] as UILogicResident;

                            if (ui.IsOpened)
                            {
                                return null;
                            }

                            if (!ui.IsCreated)
                            {
                                if (_defineUIAndEntitys.ContainsKey(type.FullName) && _defineUIAndEntitys[type.FullName] != null)
                                {
                                    ui.UIEntity = Instantiate(_defineUIAndEntitys[type.FullName], _overlayResidentPanel);
                                    ui.UIEntity.transform.SetAsLastSibling();
                                    ui.UIEntity.SetActive(true);
                                    ui.OnInit();
                                    ui.OnOpen(args);
                                    ui.OnPlaceTop();
                                    return null;
                                }
                                else
                                {
                                    return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _overlayResidentPanel, null, (obj) =>
                                    {
                                        ui.UIEntity = obj;
                                        ui.UIEntity.transform.SetAsLastSibling();
                                        ui.UIEntity.SetActive(true);
                                        ui.OnInit();
                                        ui.OnOpen(args);
                                        ui.OnPlaceTop();
                                    }, true);
                                }
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "打开UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicResident ui = _cameraUIs[type] as UILogicResident;

                            if (ui.IsOpened)
                            {
                                return null;
                            }

                            if (!ui.IsCreated)
                            {
                                if (_defineUIAndEntitys.ContainsKey(type.FullName) && _defineUIAndEntitys[type.FullName] != null)
                                {
                                    ui.UIEntity = Instantiate(_defineUIAndEntitys[type.FullName], _cameraResidentPanel);
                                    ui.UIEntity.transform.SetAsLastSibling();
                                    ui.UIEntity.SetActive(true);
                                    ui.OnInit();
                                    ui.OnOpen(args);
                                    ui.OnPlaceTop();
                                    return null;
                                }
                                else
                                {
                                    return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _cameraResidentPanel, null, (obj) =>
                                    {
                                        ui.UIEntity = obj;
                                        ui.UIEntity.transform.SetAsLastSibling();
                                        ui.UIEntity.SetActive(true);
                                        ui.OnInit();
                                        ui.OnOpen(args);
                                        ui.OnPlaceTop();
                                    }, true);
                                }
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "打开UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return _worldUIs[attribute.WorldUIDomainName].OpenResidentUI(type, _defineUIAndEntitys.ContainsKey(type.FullName) ? _defineUIAndEntitys[type.FullName] : null, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "打开UI失败：UI对象 " + type.Name + " 的域 " + attribute.WorldUIDomainName + " 并未存在！");
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 打开非常驻UI
        /// </summary>
        /// <typeparam name="T">非常驻UI逻辑类</typeparam>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenTemporaryUI<T>(params object[] args) where T : UILogicTemporary
        {
            return OpenTemporaryUI(typeof(T), args);
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
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicTemporary ui = _overlayUIs[type] as UILogicTemporary;

                            if (ui.IsOpened)
                            {
                                return null;
                            }

                            if (_currentOverlayTemporaryUI != null && _currentOverlayTemporaryUI.IsOpened)
                            {
                                _currentOverlayTemporaryUI.UIEntity.SetActive(false);
                                _currentOverlayTemporaryUI.OnClose();
                                _currentOverlayTemporaryUI = null;
                            }

                            if (!ui.IsCreated)
                            {
                                if (_defineUIAndEntitys.ContainsKey(type.FullName) && _defineUIAndEntitys[type.FullName] != null)
                                {
                                    ui.UIEntity = Instantiate(_defineUIAndEntitys[type.FullName], _overlayTemporaryPanel);
                                    ui.UIEntity.transform.SetAsLastSibling();
                                    ui.UIEntity.SetActive(true);
                                    ui.OnInit();
                                    ui.OnOpen(args);
                                    _currentOverlayTemporaryUI = ui;
                                    return null;
                                }
                                else
                                {
                                    return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _overlayTemporaryPanel, null, (obj) =>
                                    {
                                        ui.UIEntity = obj;
                                        ui.UIEntity.transform.SetAsLastSibling();
                                        ui.UIEntity.SetActive(true);
                                        ui.OnInit();
                                        ui.OnOpen(args);
                                        _currentOverlayTemporaryUI = ui;
                                    }, true);
                                }
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "打开UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicTemporary ui = _cameraUIs[type] as UILogicTemporary;

                            if (ui.IsOpened)
                            {
                                return null;
                            }

                            if (_currentCameraTemporaryUI != null && _currentCameraTemporaryUI.IsOpened)
                            {
                                _currentCameraTemporaryUI.UIEntity.SetActive(false);
                                _currentCameraTemporaryUI.OnClose();
                                _currentCameraTemporaryUI = null;
                            }

                            if (!ui.IsCreated)
                            {
                                if (_defineUIAndEntitys.ContainsKey(type.FullName) && _defineUIAndEntitys[type.FullName] != null)
                                {
                                    ui.UIEntity = Instantiate(_defineUIAndEntitys[type.FullName], _cameraTemporaryPanel);
                                    ui.UIEntity.transform.SetAsLastSibling();
                                    ui.UIEntity.SetActive(true);
                                    ui.OnInit();
                                    ui.OnOpen(args);
                                    _currentCameraTemporaryUI = ui;
                                    return null;
                                }
                                else
                                {
                                    return Main.m_Resource.LoadPrefab(new PrefabInfo(attribute), _cameraTemporaryPanel, null, (obj) =>
                                    {
                                        ui.UIEntity = obj;
                                        ui.UIEntity.transform.SetAsLastSibling();
                                        ui.UIEntity.SetActive(true);
                                        ui.OnInit();
                                        ui.OnOpen(args);
                                        _currentCameraTemporaryUI = ui;
                                    }, true);
                                }
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "打开UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return _worldUIs[attribute.WorldUIDomainName].OpenTemporaryUI(type, _defineUIAndEntitys.ContainsKey(type.FullName) ? _defineUIAndEntitys[type.FullName] : null, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "打开UI失败：UI对象 " + type.Name + " 的域 " + attribute.WorldUIDomainName + " 并未存在！");
                        }
                }
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
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _overlayUIs[type];

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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "获取UI失败：UI对象 " + type.Name + " 并未存在，或并未打开！");
                        }
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _cameraUIs[type];

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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "获取UI失败：UI对象 " + type.Name + " 并未存在，或并未打开！");
                        }
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            return _worldUIs[attribute.WorldUIDomainName].GetOpenedUI(type);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "获取UI失败：UI对象 " + type.Name + " 的域 " + attribute.WorldUIDomainName + " 并未存在！");
                        }
                }
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "置顶UI失败：UI对象 " + type.Name + " 并未存在！");
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "置顶UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].PlaceTop(type);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "置顶UI失败：UI对象 " + type.Name + " 的域 " + attribute.WorldUIDomainName + " 并未存在！");
                        }
                        break;
                }
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
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _overlayUIs[type];

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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "关闭UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _cameraUIs[type];

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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "关闭UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].CloseUI(type);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "关闭UI失败：UI对象 " + type.Name + " 的域 " + attribute.WorldUIDomainName + " 并未存在！");
                        }
                        break;
                }
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
            UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (_overlayUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _overlayUIs[type];

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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "销毁UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.Camera:
                        if (_cameraUIs.ContainsKey(type))
                        {
                            UILogicBase ui = _cameraUIs[type];

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
                            throw new HTFrameworkException(HTFrameworkModule.UI, "销毁UI失败：UI对象 " + type.Name + " 并未存在！");
                        }
                        break;
                    case UIType.World:
                        if (_worldUIs.ContainsKey(attribute.WorldUIDomainName))
                        {
                            _worldUIs[attribute.WorldUIDomainName].DestroyUI(type);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, "销毁UI失败：UI对象 " + type.Name + " 的域 " + attribute.WorldUIDomainName + " 并未存在！");
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
            private string _name;
            //当前打开的World类型的非常驻UI（非常驻UI同时只能打开一个）
            private UILogicTemporary _currentWorldTemporaryUI;
            //所有World类型的UI
            private Dictionary<Type, UILogicBase> _worldUIs = new Dictionary<Type, UILogicBase>();

            private Transform _worldUIRoot;
            private RectTransform _worldUIRootRect;
            private Transform _worldResidentPanel;
            private Transform _worldTemporaryPanel;

            public WorldUIDomain(string name, GameObject canvasTem)
            {
                _name = name;
                _worldUIRoot = Main.CloneGameObject(canvasTem, true).transform;
                _worldUIRoot.name = _name;
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
                foreach (KeyValuePair<Type, UILogicBase> ui in _worldUIs)
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
                foreach (KeyValuePair<Type, UILogicBase> ui in _worldUIs)
                {
                    UILogicBase uiLogic = ui.Value;

                    if (!uiLogic.IsCreated)
                    {
                        continue;
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
                    _worldUIs.Add(uiLogicType, Activator.CreateInstance(uiLogicType) as UILogicBase);
                }
            }
            /// <summary>
            /// 预加载常驻UI
            /// </summary>
            /// <param name="type">常驻UI逻辑类</param>
            /// <param name="entity">UI实体</param>
            /// <returns>加载协程</returns>
            public Coroutine PreloadingResidentUI(Type type, GameObject entity)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogicBase ui = _worldUIs[type];

                    if (!ui.IsCreated)
                    {
                        if (entity != null)
                        {
                            ui.UIEntity = Instantiate(entity, _worldResidentPanel);
                            ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                            ui.OnInit();
                            return null;
                        }
                        else
                        {
                            return Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldResidentPanel, null, (obj) =>
                            {
                                ui.UIEntity = obj;
                                ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                                ui.OnInit();
                            }, true);
                        }
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.UI, "预加载UI失败：UI对象 " + type.Name + " 并未存在！");
                }
                return null;
            }
            /// <summary>
            /// 预加载非常驻UI
            /// </summary>
            /// <param name="type">非常驻UI逻辑类</param>
            /// <param name="entity">UI实体</param>
            /// <returns>加载协程</returns>
            public Coroutine PreloadingTemporaryUI(Type type, GameObject entity)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogicBase ui = _worldUIs[type];

                    if (!ui.IsCreated)
                    {
                        if (entity != null)
                        {
                            ui.UIEntity = Instantiate(entity, _worldTemporaryPanel);
                            ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                            ui.OnInit();
                            return null;
                        }
                        else
                        {
                            return Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldTemporaryPanel, null, (obj) =>
                            {
                                ui.UIEntity = obj;
                                ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                                ui.OnInit();
                            }, true);
                        }
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.UI, "预加载UI失败：UI对象 " + type.Name + " 并未存在！");
                }
                return null;
            }
            /// <summary>
            /// 打开常驻UI
            /// </summary>
            /// <param name="type">常驻UI逻辑类</param>
            /// <param name="entity">UI实体</param>
            /// <param name="args">可选参数</param>
            /// <returns>加载协程</returns>
            public Coroutine OpenResidentUI(Type type, GameObject entity, params object[] args)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogicResident ui = _worldUIs[type] as UILogicResident;

                    if (ui.IsOpened)
                    {
                        return null;
                    }

                    if (!ui.IsCreated)
                    {
                        if (entity != null)
                        {
                            ui.UIEntity = Instantiate(entity, _worldResidentPanel);
                            ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                            ui.UIEntity.transform.SetAsLastSibling();
                            ui.UIEntity.SetActive(true);
                            ui.OnInit();
                            ui.OnOpen(args);
                            ui.OnPlaceTop();
                            return null;
                        }
                        else
                        {
                            return Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldResidentPanel, null, (obj) =>
                            {
                                ui.UIEntity = obj;
                                ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                                ui.UIEntity.transform.SetAsLastSibling();
                                ui.UIEntity.SetActive(true);
                                ui.OnInit();
                                ui.OnOpen(args);
                                ui.OnPlaceTop();
                            }, true);
                        }
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
                    throw new HTFrameworkException(HTFrameworkModule.UI, "打开UI失败：UI对象 " + type.Name + " 并未存在！");
                }
                return null;
            }
            /// <summary>
            /// 打开非常驻UI
            /// </summary>
            /// <param name="type">非常驻UI逻辑类</param>
            /// <param name="entity">UI实体</param>
            /// <param name="args">可选参数</param>
            /// <returns>加载协程</returns>
            public Coroutine OpenTemporaryUI(Type type, GameObject entity, params object[] args)
            {
                if (_worldUIs.ContainsKey(type))
                {
                    UILogicTemporary ui = _worldUIs[type] as UILogicTemporary;

                    if (ui.IsOpened)
                    {
                        return null;
                    }

                    if (_currentWorldTemporaryUI != null && _currentWorldTemporaryUI.IsOpened)
                    {
                        _currentWorldTemporaryUI.UIEntity.SetActive(false);
                        _currentWorldTemporaryUI.OnClose();
                        _currentWorldTemporaryUI = null;
                    }

                    if (!ui.IsCreated)
                    {
                        if (entity != null)
                        {
                            ui.UIEntity = Instantiate(entity, _worldTemporaryPanel);
                            ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                            ui.UIEntity.transform.SetAsLastSibling();
                            ui.UIEntity.SetActive(true);
                            ui.OnInit();
                            ui.OnOpen(args);
                            _currentWorldTemporaryUI = ui;
                            return null;
                        }
                        else
                        {
                            return Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldTemporaryPanel, null, (obj) =>
                            {
                                ui.UIEntity = obj;
                                ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                                ui.UIEntity.transform.SetAsLastSibling();
                                ui.UIEntity.SetActive(true);
                                ui.OnInit();
                                ui.OnOpen(args);
                                _currentWorldTemporaryUI = ui;
                            }, true);
                        }
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
                    throw new HTFrameworkException(HTFrameworkModule.UI, "打开UI失败：UI对象 " + type.Name + " 并未存在！");
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
                if (_worldUIs.ContainsKey(type))
                {
                    UILogicBase ui = _worldUIs[type];

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
                    throw new HTFrameworkException(HTFrameworkModule.UI, "获取UI失败：UI对象 " + type.Name + " 并未存在，或并未打开！");
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
                    throw new HTFrameworkException(HTFrameworkModule.UI, "置顶UI失败：UI对象 " + type.Name + " 并未存在！");
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
                    UILogicBase ui = _worldUIs[type];

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
                    throw new HTFrameworkException(HTFrameworkModule.UI, "关闭UI失败：UI对象 " + type.Name + " 并未存在！");
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
                    UILogicBase ui = _worldUIs[type];

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
                    throw new HTFrameworkException(HTFrameworkModule.UI, "销毁UI失败：UI对象 " + type.Name + " 并未存在！");
                }
            }
        }
    }
}