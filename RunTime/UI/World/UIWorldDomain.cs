using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 世界UI的域
    /// </summary>
    public class UIWorldDomain
    {
        //域的名称
        protected string _name;
        //当前打开的World类型的非常驻UI（非常驻UI同时只能打开一个）
        protected UILogicTemporary _currentWorldTemporaryUI;
        //所有World类型的UI
        protected Dictionary<Type, UILogicBase> _worldUIs = new Dictionary<Type, UILogicBase>();

        protected Transform _worldUIRoot;
        protected RectTransform _worldUIRootRect;
        protected Transform _worldResidentPanel;
        protected Transform _worldTemporaryPanel;

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

        public UIWorldDomain(string name, GameObject canvasTem)
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
        public void OnUpdate()
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
                    continue;

                uiLogic.OnDestroy();
                Main.Kill(uiLogic.UIEntity);
                uiLogic.UIEntity = null;
            }
            _worldUIs.Clear();

            Main.Kill(_worldUIRoot.gameObject);
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

                if (ui.IsCreated)
                    return null;

                if (entity != null)
                {
                    ui.UIEntity = Main.Clone(entity, _worldResidentPanel);
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
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
            }
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

                if (ui.IsCreated)
                    return null;

                if (entity != null)
                {
                    ui.UIEntity = Main.Clone(entity, _worldTemporaryPanel);
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
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("预加载UI失败：UI对象 {0} 并未存在！", type.Name));
            }
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
                    return null;

                if (!ui.IsCreated)
                {
                    if (entity != null)
                    {
                        ui.UIEntity = Main.Clone(entity, _worldResidentPanel);
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
                throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
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
                    return null;

                if (_currentWorldTemporaryUI != null && _currentWorldTemporaryUI.IsOpened)
                {
                    _currentWorldTemporaryUI.UIEntity.SetActive(false);
                    _currentWorldTemporaryUI.OnClose();
                    _currentWorldTemporaryUI = null;
                }
                _currentWorldTemporaryUI = ui;

                if (!ui.IsCreated)
                {
                    if (entity != null)
                    {
                        ui.UIEntity = Main.Clone(entity, _worldTemporaryPanel);
                        ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                        ui.UIEntity.SetActive(true);
                        ui.OnInit();
                        ui.OnOpen(args);
                        return null;
                    }
                    else
                    {
                        return Main.m_Resource.LoadPrefab(new PrefabInfo(type.GetCustomAttribute<UIResourceAttribute>()), _worldTemporaryPanel, null, (obj) =>
                        {
                            ui.UIEntity = obj;
                            ui.UIEntity.SetLayerIncludeChildren(_worldUIRoot.gameObject.layer);
                            ui.UIEntity.SetActive(true);
                            ui.OnInit();
                            ui.OnOpen(args);
                        }, true);
                    }
                }
                else
                {
                    ui.UIEntity.SetActive(true);
                    ui.OnOpen(args);
                }
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.UI, string.Format("打开UI失败：UI对象 {0} 并未存在！", type.Name));
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
                return null;
            }
        }
        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="type">UI逻辑类</param>
        /// <returns>UI逻辑对象</returns>
        public UILogicBase GetUI(Type type)
        {
            if (_worldUIs.ContainsKey(type))
            {
                UILogicBase ui = _worldUIs[type];
                return ui;
            }
            else
            {
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
                    return;

                ui.UIEntity.transform.SetAsLastSibling();
                ui.OnPlaceTop();
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
                    return;

                if (!ui.IsOpened)
                    return;

                ui.UIEntity.SetActive(false);
                ui.OnClose();
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
                    return;
                
                ui.OnDestroy();
                Main.Kill(ui.UIEntity);
                ui.UIEntity = null;
            }
        }
    }
}