using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的UI管理器助手
    /// </summary>
    internal sealed class DefaultUIHelper : IUIHelper
    {
        //UI管理器
        private UIManager _module;
        //当前定义的UI与实体对应关系
        private Dictionary<string, GameObject> _defineUIAndEntitys = new Dictionary<string, GameObject>();
        //当前定义的UI的所有资源标记
        private Dictionary<Type, UIResourceAttribute> _allUIResource = new Dictionary<Type, UIResourceAttribute>();
        //当前定义的Overlay类型的UI的所有深度界面
        private Dictionary<int, RectTransform> _allOverlayDepth = new Dictionary<int, RectTransform>();
        //当前定义的Camera类型的UI的所有深度界面
        private Dictionary<int, RectTransform> _allCameraDepth = new Dictionary<int, RectTransform>();
        //当前打开的Overlay类型的临时UI（临时UI同时只能打开一个）
        private UILogicTemporary _currentOverlayTemporaryUI;
        //当前打开的Camera类型的临时UI（临时UI同时只能打开一个）
        private UILogicTemporary _currentCameraTemporaryUI;
        //所有UI根节点
        private GameObject _uiRoot;
        //Overlay类型的UI根节点
        private RectTransform _overlayUIRoot;
        private RectTransform _overlayResidentPanel;
        private RectTransform _overlayTemporaryPanel;
        //Camera类型的UI根节点
        private RectTransform _cameraUIRoot;
        private RectTransform _cameraResidentPanel;
        private RectTransform _cameraTemporaryPanel;
        //遮罩面板
        private GameObject _maskPanel;

        /// <summary>
        /// 所属的内置模块
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
        /// Overlay类型的UI根节点
        /// </summary>
        public RectTransform OverlayUIRoot
        {
            get
            {
                return _overlayUIRoot;
            }
        }
        /// <summary>
        /// Camera类型的UI根节点
        /// </summary>
        public RectTransform CameraUIRoot
        {
            get
            {
                return _cameraUIRoot;
            }
        }
        /// <summary>
        /// Camera类型UI的摄像机
        /// </summary>
        public Camera UICamera { get; private set; }
        /// <summary>
        /// 是否锁住当前打开的临时UI，锁住后打开其他临时UI将无法顶掉当前打开的UI，使其显示于绝对顶端
        /// </summary>
        public bool IsLockTemporaryUI { get; set; } = false;
        /// <summary>
        /// 是否隐藏所有UI实体
        /// </summary>
        public bool IsHideAll
        {
            set
            {
                _uiRoot.SetActive(!value);
            }
            get
            {
                return !_uiRoot.activeSelf;
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
            _uiRoot = _module.transform.FindChildren("UIRoot");
            _overlayUIRoot = _uiRoot.transform.Find("OverlayUIRoot").rectTransform();
            _overlayResidentPanel = _overlayUIRoot.Find("ResidentPanel").rectTransform();
            _overlayTemporaryPanel = _overlayUIRoot.Find("TemporaryPanel").rectTransform();
            _cameraUIRoot = _uiRoot.transform.Find("CameraUIRoot").rectTransform();
            _cameraResidentPanel = _cameraUIRoot.Find("ResidentPanel").rectTransform();
            _cameraTemporaryPanel = _cameraUIRoot.Find("TemporaryPanel").rectTransform();
            _maskPanel = _overlayUIRoot.FindChildren("MaskPanel");
            UICamera = _uiRoot.transform.Find("UICamera").GetComponent<Camera>();

            _overlayUIRoot.gameObject.SetActive(_module.IsEnableOverlayUI);
            _cameraUIRoot.gameObject.SetActive(_module.IsEnableCameraUI);
            UICamera.gameObject.SetActive(_module.IsEnableCameraUI);

            //创建所有UI的逻辑对象
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
            {
                return (type.IsSubclassOf(typeof(UILogicResident)) || type.IsSubclassOf(typeof(UILogicTemporary))) && !type.IsAbstract;
            }, false);
            for (int i = 0; i < types.Count; i++)
            {
                UIResourceAttribute attribute = types[i].GetCustomAttribute<UIResourceAttribute>();
                if (attribute != null)
                {
                    _allUIResource.Add(types[i], attribute);
                    switch (attribute.EntityType)
                    {
                        case UIType.Overlay:
                            if (_module.IsEnableOverlayUI)
                            {
                                OverlayUIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogicBase);
                                CreateDepthPanel(types[i], attribute.Depth, _overlayResidentPanel.Find("DepthPanel"), _allOverlayDepth);
                            }
                            break;
                        case UIType.Camera:
                            if (_module.IsEnableCameraUI)
                            {
                                CameraUIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogicBase);
                                CreateDepthPanel(types[i], attribute.Depth, _cameraResidentPanel.Find("DepthPanel"), _allCameraDepth);
                            }
                            break;
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.UI, $"创建UI逻辑对象失败：UI逻辑类 {types[i].Name} 丢失 UIResourceAttribute 标记！");
                }
            }

            SortDepthPanel(_allOverlayDepth);
            SortDepthPanel(_allCameraDepth);
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
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {
            _defineUIAndEntitys.Clear();
            _allUIResource.Clear();

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

            DestroyDepthPanel(_allOverlayDepth);
            DestroyDepthPanel(_allCameraDepth);
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
        /// 设置预定义
        /// </summary>
        /// <param name="defineUINames">预定义的UI名称</param>
        /// <param name="defineUIEntitys">预定义的UI实体</param>
        public void SetDefine(List<string> defineUINames, List<GameObject> defineUIEntitys)
        {
            _defineUIAndEntitys.Clear();
            for (int i = 0; i < defineUINames.Count; i++)
            {
                if (!_defineUIAndEntitys.ContainsKey(defineUINames[i]))
                {
                    _defineUIAndEntitys.Add(defineUINames[i], defineUIEntitys[i]);
                }
            }
        }
        /// <summary>
        /// 添加预定义（如果已存在则覆盖，已打开的UI不受影响，销毁后再次打开生效）
        /// </summary>
        /// <param name="defineUIName">预定义的UI名称</param>
        /// <param name="defineUIEntity">预定义的UI实体</param>
        public void AddDefine(string defineUIName, GameObject defineUIEntity)
        {
            if (_defineUIAndEntitys.ContainsKey(defineUIName))
            {
                _defineUIAndEntitys[defineUIName] = defineUIEntity;
            }
            else
            {
                _defineUIAndEntitys.Add(defineUIName, defineUIEntity);
            }
        }

        /// <summary>
        /// 预加载常驻UI
        /// </summary>
        /// <param name="type">常驻UI逻辑类</param>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingResidentUI(Type type)
        {
            UIResourceAttribute attribute = GetUIResource(type);
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            RectTransform depthPanel = GetDepthPanel(type, attribute.Depth, _allOverlayDepth);
                            return CreateUIEntity(attribute, type.FullName, OverlayUIs[type], depthPanel != null ? depthPanel : _overlayResidentPanel);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, $"预加载UI失败：UI对象 {type.Name} 并未存在！");
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            RectTransform depthPanel = GetDepthPanel(type, attribute.Depth, _allCameraDepth);
                            return CreateUIEntity(attribute, type.FullName, CameraUIs[type], depthPanel != null ? depthPanel : _cameraResidentPanel);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, $"预加载UI失败：UI对象 {type.Name} 并未存在！");
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 预加载临时UI
        /// </summary>
        /// <param name="type">临时UI逻辑类</param>
        /// <returns>加载协程</returns>
        public Coroutine PreloadingTemporaryUI(Type type)
        {
            UIResourceAttribute attribute = GetUIResource(type);
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, $"预加载UI失败：UI对象 {type.Name} 并未存在！");
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            return CreateUIEntity(attribute, type.FullName, CameraUIs[type], _cameraTemporaryPanel);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, $"预加载UI失败：UI对象 {type.Name} 并未存在！");
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
            UIResourceAttribute attribute = GetUIResource(type);
            if (attribute != null)
            {
                switch (attribute.EntityType)
                {
                    case UIType.Overlay:
                        if (OverlayUIs.ContainsKey(type))
                        {
                            RectTransform depthPanel = GetDepthPanel(type, attribute.Depth, _allOverlayDepth);
                            return CreateOpenUIEntity(attribute, type.FullName, OverlayUIs[type], depthPanel != null ? depthPanel : _overlayResidentPanel, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, $"打开UI失败：UI对象 {type.Name} 并未存在！");
                        }
                    case UIType.Camera:
                        if (CameraUIs.ContainsKey(type))
                        {
                            RectTransform depthPanel = GetDepthPanel(type, attribute.Depth, _allCameraDepth);
                            return CreateOpenUIEntity(attribute, type.FullName, CameraUIs[type], depthPanel != null ? depthPanel : _cameraResidentPanel, args);
                        }
                        else
                        {
                            throw new HTFrameworkException(HTFrameworkModule.UI, $"打开UI失败：UI对象 {type.Name} 并未存在！");
                        }
                }
            }
            return null;
        }
        /// <summary>
        /// 打开临时UI
        /// </summary>
        /// <param name="type">临时UI逻辑类</param>
        /// <param name="args">可选参数</param>
        /// <returns>加载协程</returns>
        public Coroutine OpenTemporaryUI(Type type, params object[] args)
        {
            UIResourceAttribute attribute = GetUIResource(type);
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, $"打开UI失败：UI对象 {type.Name} 并未存在！");
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
                            throw new HTFrameworkException(HTFrameworkModule.UI, $"打开UI失败：UI对象 {type.Name} 并未存在！");
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
            UIResourceAttribute attribute = GetUIResource(type);
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
                }
            }
            return null;
        }
        /// <summary>
        /// 获取 transform 的最顶层父级所属的UI
        /// </summary>
        /// <param name="transform">目标 transform</param>
        /// <returns>UI逻辑对象</returns>
        public UILogicBase GetUIByTransform(Transform transform)
        {
            foreach (var item in OverlayUIs)
            {
                if (transform.IsChildOf(item.Value.UIEntity.transform))
                {
                    return item.Value;
                }
            }
            foreach (var item in CameraUIs)
            {
                if (transform.IsChildOf(item.Value.UIEntity.transform))
                {
                    return item.Value;
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
            UIResourceAttribute attribute = GetUIResource(type);
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
            UIResourceAttribute attribute = GetUIResource(type);
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
                }
            }
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        public void CloseUI(Type type)
        {
            UIResourceAttribute attribute = GetUIResource(type);
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
                }
            }
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        public void DestroyUI(Type type)
        {
            UIResourceAttribute attribute = GetUIResource(type);
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
                }
            }
        }

        /// <summary>
        /// 显示同属于一个深度的所有UI界面
        /// </summary>
        /// <param name="depth">深度</param>
        public void ShowAllUIOfDepth(int depth)
        {
            foreach (var item in _allOverlayDepth)
            {
                if (item.Key == depth)
                {
                    item.Value.gameObject.SetActive(true);
                }
            }
            foreach (var item in _allCameraDepth)
            {
                if (item.Key == depth)
                {
                    item.Value.gameObject.SetActive(true);
                }
            }
        }
        /// <summary>
        /// 隐藏同属于一个深度的所有UI界面
        /// </summary>
        /// <param name="depth">深度</param>
        public void HideAllUIOfDepth(int depth)
        {
            foreach (var item in _allOverlayDepth)
            {
                if (item.Key == depth)
                {
                    item.Value.gameObject.SetActive(false);
                }
            }
            foreach (var item in _allCameraDepth)
            {
                if (item.Key == depth)
                {
                    item.Value.gameObject.SetActive(false);
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

            void Create(GameObject entity)
            {
                uILogic.UIEntity = entity;
                uILogic.UIEntity.SetActive(false);
                uILogic.OnInit();
            }

            if (_defineUIAndEntitys.ContainsKey(uITypeName) && _defineUIAndEntitys[uITypeName] != null)
            {
                Create(Main.Clone(_defineUIAndEntitys[uITypeName], uIParent));
                return null;
            }
            else
            {
                return Main.m_Resource.LoadPrefab(new PrefabInfo(uIResource), uIParent, null, Create, true);
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

            void CreateAndOpen(GameObject entity)
            {
                uILogic.UIEntity = entity;
                uILogic.UIEntity.SetActive(true);
                uILogic.OnInit();
                uILogic.OnOpen(args);
                PlaceTopUIEntity(uILogic as UILogicResident);
            }

            void Open()
            {
                uILogic.UIEntity.SetActive(true);
                uILogic.OnOpen(args);
                PlaceTopUIEntity(uILogic as UILogicResident);
            }

            if (!uILogic.IsCreated)
            {
                if (_defineUIAndEntitys.ContainsKey(uITypeName) && _defineUIAndEntitys[uITypeName] != null)
                {
                    CreateAndOpen(Main.Clone(_defineUIAndEntitys[uITypeName], uIParent));
                    return null;
                }
                else
                {
                    return Main.m_Resource.LoadPrefab(new PrefabInfo(uIResource), uIParent, null, CreateAndOpen, true);
                }
            }
            else
            {
                Open();
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

        /// <summary>
        /// 获取UI资源标记
        /// </summary>
        /// <param name="type">UI逻辑类型</param>
        /// <returns>UI资源标记</returns>
        private UIResourceAttribute GetUIResource(Type type)
        {
            if (_allUIResource.ContainsKey(type))
            {
                return _allUIResource[type];
            }
            return null;
        }
        /// <summary>
        /// 创建深度界面
        /// </summary>
        /// <param name="type">UI逻辑类型</param>
        /// <param name="depth">深度</param>
        /// <param name="parent">深度界面父级</param>
        /// <param name="allDepth">深度界面集合</param>
        private void CreateDepthPanel(Type type, int depth, Transform parent, Dictionary<int, RectTransform> allDepth)
        {
            if (depth < 0)
                return;

            if (!type.IsSubclassOf(typeof(UILogicResident)))
                return;

            if (!allDepth.ContainsKey(depth))
            {
                GameObject panel = new GameObject(depth.ToString());
                panel.layer = parent.gameObject.layer;
                panel.transform.SetParent(parent);
                RectTransform rectTransform = panel.AddComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector3.one;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                allDepth.Add(depth, rectTransform);
            }
        }
        /// <summary>
        /// 排序深度界面
        /// </summary>
        /// <param name="allDepth">深度界面集合</param>
        private void SortDepthPanel(Dictionary<int, RectTransform> allDepth)
        {
            if (allDepth.Count <= 1)
                return;

            List<RectTransform> depths = new List<RectTransform>();
            foreach (var item in allDepth)
            {
                depths.Add(item.Value);
            }
            depths.Sort((a, b) =>
            {
                int aIndex = int.Parse(a.name);
                int bIndex = int.Parse(b.name);
                return aIndex - bIndex;
            });
            for (int i = 0; i < depths.Count; i++)
            {
                depths[i].SetSiblingIndex(i);
            }
        }
        /// <summary>
        /// 获取深度界面
        /// </summary>
        /// <param name="type">UI逻辑类型</param>
        /// <param name="depth">深度</param>
        /// <param name="allDepth">深度界面集合</param>
        /// <returns>深度界面</returns>
        private RectTransform GetDepthPanel(Type type, int depth, Dictionary<int, RectTransform> allDepth)
        {
            if (depth < 0)
                return null;

            if (!type.IsSubclassOf(typeof(UILogicResident)))
                return null;

            if (allDepth.ContainsKey(depth))
            {
                return allDepth[depth];
            }
            return null;
        }
        /// <summary>
        /// 销毁深度界面
        /// </summary>
        /// <param name="allDepth">深度界面集合</param>
        private void DestroyDepthPanel(Dictionary<int, RectTransform> allDepth)
        {
            foreach (var item in allDepth)
            {
                Main.Kill(item.Value.gameObject);
            }
            allDepth.Clear();
        }
    }
}