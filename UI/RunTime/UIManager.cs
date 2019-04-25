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
        //当前打开的非常驻UI（非常驻UI同时只能打开一个）
        private UILogicTemporary _currentTemporaryUI;
        //所有UI
        private Dictionary<Type, UILogic> _UIs = new Dictionary<Type, UILogic>();

        private Transform _uiRoot;
        private RectTransform _uiRootRect;
        private Transform _residentPanel;
        private Transform _temporaryPanel;

        public override void Initialization()
        {
            _uiRoot = transform.Find("UIRoot");
            _uiRootRect = _uiRoot.rectTransform();
            _residentPanel = _uiRoot.Find("ResidentPanel");
            _temporaryPanel = _uiRoot.Find("TemporaryPanel");
            //创建所有UI的逻辑对象
            Assembly assembly = Assembly.GetAssembly(typeof(UILogic));
            Type[] types = assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].BaseType == typeof(UILogicResident) || types[i].BaseType == typeof(UILogicTemporary))
                {
                    _UIs.Add(types[i], Activator.CreateInstance(types[i]) as UILogic);
                }
            }
        }

        public override void Refresh()
        {
            foreach (KeyValuePair<Type, UILogic> ui in _UIs)
            {
                if (ui.Value.IsOpened)
                {
                    ui.Value.OnUpdate();
                }
            }
        }

        public override void Termination()
        {
            foreach (KeyValuePair<Type, UILogic> ui in _UIs)
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
            _UIs.Clear();
        }

        /// <summary>
        /// UI根节点
        /// </summary>
        public RectTransform UIRoot
        {
            get
            {
                return _uiRootRect;
            }
        }

        /// <summary>
        /// 是否隐藏所有UI实体
        /// </summary>
        public bool HideAll
        {
            set
            {
                _uiRoot.gameObject.SetActive(!value);
            }
            get
            {
                return !_uiRoot.gameObject.activeSelf;
            }
        }

        /// <summary>
        /// 预加载常驻UI
        /// </summary>
        public void PreloadingResidentUI<T>() where T : UILogicResident
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogic ui = _UIs[typeof(T)];
                
                if (!ui.IsCreated)
                {
                    object[] atts = typeof(T).GetCustomAttributes(typeof(UIResourceAttribute), false);
                    if (atts.Length != 1)
                    {
                        GlobalTools.LogError("预加载UI失败：UI对象 " + typeof(T).Name + " 并未标记UIResourceAttribute特性！");
                        return;
                    }
                    Main.m_Resource.LoadPrefab(new PrefabInfo(atts[0] as UIResourceAttribute), _residentPanel, null, (obj) =>
                    {
                        ui.UIEntity = obj;
                        ui.OnInit();
                    }, true);
                }
            }
            else
            {
                GlobalTools.LogError("预加载UI失败：UI对象 " + typeof(T).Name + " 并未存在！");
            }
        }

        /// <summary>
        /// 预加载非常驻UI
        /// </summary>
        public void PreloadingTemporaryUI<T>() where T : UILogicTemporary
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogic ui = _UIs[typeof(T)];

                if (!ui.IsCreated)
                {
                    object[] atts = typeof(T).GetCustomAttributes(typeof(UIResourceAttribute), false);
                    if (atts.Length != 1)
                    {
                        GlobalTools.LogError("预加载UI失败：UI对象 " + typeof(T).Name + " 并未标记UIResourceAttribute特性！");
                        return;
                    }
                    Main.m_Resource.LoadPrefab(new PrefabInfo(atts[0] as UIResourceAttribute), _temporaryPanel, null, (obj) =>
                    {
                        ui.UIEntity = obj;
                        ui.OnInit();
                    }, true);
                }
            }
            else
            {
                GlobalTools.LogError("预加载UI失败：UI对象 " + typeof(T).Name + " 并未存在！");
            }
        }

        /// <summary>
        /// 打开常驻UI
        /// </summary>
        public void OpenResidentUI<T>(params object[] args) where T : UILogicResident
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogicResident ui = _UIs[typeof(T)] as UILogicResident;

                if (ui.IsOpened)
                {
                    return;
                }

                if (!ui.IsCreated)
                {
                    object[] atts = typeof(T).GetCustomAttributes(typeof(UIResourceAttribute), false);
                    if (atts.Length != 1)
                    {
                        GlobalTools.LogError("打开UI失败：UI对象 " + typeof(T).Name + " 并未标记UIResourceAttribute特性！");
                        return;
                    }
                    Main.m_Resource.LoadPrefab(new PrefabInfo(atts[0] as UIResourceAttribute), _residentPanel, null, (obj) =>
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
                GlobalTools.LogError("打开UI失败：UI对象 " + typeof(T).Name + " 并未存在！");
            }
        }

        /// <summary>
        /// 打开非常驻UI
        /// </summary>
        public void OpenTemporaryUI<T>(params object[] args) where T : UILogicTemporary
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogicTemporary ui = _UIs[typeof(T)] as UILogicTemporary;

                if (ui.IsOpened)
                {
                    return;
                }

                //关闭当前打开的非常驻UI
                if (_currentTemporaryUI != null && _currentTemporaryUI.IsOpened)
                {
                    _currentTemporaryUI.UIEntity.SetActive(false);
                    _currentTemporaryUI.OnClose();
                    _currentTemporaryUI = null;
                }

                if (!ui.IsCreated)
                {
                    object[] atts = typeof(T).GetCustomAttributes(typeof(UIResourceAttribute), false);
                    if (atts.Length != 1)
                    {
                        GlobalTools.LogError("打开UI失败：UI对象 " + typeof(T).Name + " 并未标记UIResourceAttribute特性！");
                        return;
                    }
                    Main.m_Resource.LoadPrefab(new PrefabInfo(atts[0] as UIResourceAttribute), _temporaryPanel, null, (obj) =>
                    {
                        ui.UIEntity = obj;
                        ui.UIEntity.transform.SetAsLastSibling();
                        ui.UIEntity.SetActive(true);
                        ui.OnInit();
                        ui.OnOpen(args);
                        _currentTemporaryUI = ui;
                    }, true);
                }
                else
                {
                    ui.UIEntity.transform.SetAsLastSibling();
                    ui.UIEntity.SetActive(true);
                    ui.OnOpen(args);
                    _currentTemporaryUI = ui;
                }
            }
            else
            {
                GlobalTools.LogError("打开UI失败：UI对象 " + typeof(T).Name + " 并未存在！");
            }
        }

        /// <summary>
        /// 获取已经打开的UI
        /// </summary>
        public T GetOpenedUI<T>() where T : UILogic
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogic ui = _UIs[typeof(T)];

                if (ui.IsOpened)
                {
                    return ui as T;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                GlobalTools.LogError("获取UI失败：UI对象 " + typeof(T).Name + " 并未存在！");
                return null;
            }
        }

        /// <summary>
        /// 置顶常驻UI
        /// </summary>
        public void PlaceTop<T>() where T : UILogicResident
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogicResident ui = _UIs[typeof(T)] as UILogicResident;

                if (!ui.IsOpened)
                {
                    return;
                }

                ui.UIEntity.transform.SetAsLastSibling();
                ui.OnPlaceTop();
            }
            else
            {
                GlobalTools.LogError("置顶UI失败：UI对象 " + typeof(T).Name + " 并未存在！");
            }
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        public void CloseUI<T>() where T : UILogic
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogic ui = _UIs[typeof(T)];

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
                GlobalTools.LogError("关闭UI失败：UI对象 " + typeof(T).Name + " 并未存在！");
            }
        }

        /// <summary>
        /// 销毁UI
        /// </summary>
        public void DestroyUI<T>() where T : UILogic
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogic ui = _UIs[typeof(T)];

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
                GlobalTools.LogError("销毁UI失败：UI对象 " + typeof(T).Name + " 并未存在！");
            }
        }
    }
}
