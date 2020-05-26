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
        
        private IUIHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IUIHelper;
            _helper.OnInitialization(DefineUINames, DefineUIEntitys);
        }
        internal override void OnRefresh()
        {
            base.OnRefresh();

            _helper.OnRefresh();
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            _helper.OnTermination();
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

        /// <summary>
        /// 世界UI的域
        /// </summary>
        public sealed class WorldUIDomain
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
            public void OnRefresh()
            {
                foreach (var ui in _worldUIs)
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
            public void OnTermination()
            {
                foreach (var ui in _worldUIs)
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