using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// UI管理器
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class UIManager : ModuleManager
    {
        [SerializeField]
        public List<string> UIs = new List<string>();
        [SerializeField]
        public List<GameObject> UIEntitys = new List<GameObject>();

        //当前打开的非常驻UI（非常驻UI同时只能打开一个）
        private UILogicTemporary _currentTemporaryUI;
        //所有已激活UI
        private Dictionary<Type, UILogic> _UIs = new Dictionary<Type, UILogic>();
        //所有已激活UI的实体
        private Dictionary<Type, GameObject> _UIEntitys = new Dictionary<Type, GameObject>();

        private Transform _uiRoot;
        private Transform _residentPanel;
        private Transform _temporaryPanel;

        public override void Initialization()
        {
            _uiRoot = transform.Find("UIRoot");
            _residentPanel = _uiRoot.Find("ResidentPanel");
            _temporaryPanel = _uiRoot.Find("TemporaryPanel");
            //创建所有已激活的UI
            for (int i = 0; i < UIs.Count; i++)
            {
                Type type = Type.GetType(UIs[i]);
                if (type != null)
                {
                    if (type.BaseType == typeof(UILogicResident) || type.BaseType == typeof(UILogicTemporary))
                    {
                        if (!_UIs.ContainsKey(type))
                        {
                            _UIs.Add(type, Activator.CreateInstance(type) as UILogic);
                            _UIEntitys.Add(type, UIEntitys[i]);
                        }
                    }
                    else
                    {
                        GlobalTools.LogError("创建UI对象失败：UI对象 " + UIs[i] + " 必须继承至UI基类：UILogicResident 或 UILogicTemporary！");
                    }
                }
                else
                {
                    GlobalTools.LogError("创建UI对象失败：丢失UI对象 " + UIs[i] + "！");
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

        /// <summary>
        /// 打开常驻UI
        /// </summary>
        public void OpenResidentUI<T>() where T : UILogicResident
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogic ui = _UIs[typeof(T)];

                if (ui.IsOpened)
                {
                    return;
                }

                if (!ui.IsCreated)
                {
                    ui.UIEntity = Instantiate(_UIEntitys[typeof(T)], _residentPanel);
                    ui.UIEntity.rectTransform().anchoredPosition = _UIEntitys[typeof(T)].rectTransform().anchoredPosition;
                    ui.UIEntity.transform.localRotation = Quaternion.identity;
                    ui.UIEntity.transform.localScale = Vector3.one;
                    ui.OnInit();
                }

                ui.UIEntity.transform.SetAsLastSibling();
                ui.UIEntity.SetActive(true);
                ui.OnOpen();
            }
            else
            {
                GlobalTools.LogError("打开UI失败：UI对象 " + typeof(T).Name + " 并未存在于激活列表！");
            }
        }

        /// <summary>
        /// 打开非常驻UI
        /// </summary>
        public void OpenTemporaryUI<T>() where T : UILogicTemporary
        {
            if (_UIs.ContainsKey(typeof(T)))
            {
                UILogic ui = _UIs[typeof(T)];

                if (ui.IsOpened)
                {
                    return;
                }

                if (!ui.IsCreated)
                {
                    ui.UIEntity = Instantiate(_UIEntitys[typeof(T)], _temporaryPanel);
                    ui.UIEntity.rectTransform().anchoredPosition = _UIEntitys[typeof(T)].rectTransform().anchoredPosition;
                    ui.UIEntity.transform.localRotation = Quaternion.identity;
                    ui.UIEntity.transform.localScale = Vector3.one;
                    ui.OnInit();
                }

                //关闭当前打开的非常驻UI
                if (_currentTemporaryUI != null && _currentTemporaryUI.IsOpened)
                {
                    _currentTemporaryUI.UIEntity.SetActive(false);
                    _currentTemporaryUI.OnClose();
                }

                _currentTemporaryUI = ui as UILogicTemporary;
                _currentTemporaryUI.UIEntity.transform.SetAsLastSibling();
                _currentTemporaryUI.UIEntity.SetActive(true);
                _currentTemporaryUI.OnOpen();
            }
            else
            {
                GlobalTools.LogError("打开UI失败：UI对象 " + typeof(T).Name + " 并未存在于激活列表！");
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
                GlobalTools.LogError("关闭UI失败：UI对象 " + typeof(T).Name + " 并未存在于激活列表！");
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
                GlobalTools.LogError("销毁UI失败：UI对象 " + typeof(T).Name + " 并未存在于激活列表！");
            }
        }
    }
}
