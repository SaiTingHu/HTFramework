using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class Setter : HTFEditorWindow
    {
        private List<SettingItemBase> _settingItems = new List<SettingItemBase>();
        private List<bool> _settingItemSigns = new List<bool>();
        private GUIContent _resetGUIContent;
        private GUIContent _editGUIContent;
        private int _currentItem;
        private string _itemFilter;
        private Vector2 _scrollItemNameGUI;
        private Vector2 _scrollItemSettingGUI;

        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/104610857";

        protected override void OnEnable()
        {
            base.OnEnable();

            _settingItems.Clear();
            _settingItems.Add(new SettingItemMain());
            _settingItems.Add(new SettingItemWebRequest());

            _settingItemSigns.Clear();
            _settingItemSigns.Add(true);
            _settingItemSigns.Add(true);

            List<Type> types = EditorReflectionToolkit.GetTypesInEditorAssemblies(type =>
            {
                return type.IsSubclassOf(typeof(SettingItemBase)) && type.GetCustomAttribute<InternalSettingItemAttribute>() == null;
            }, false);
            for (int i = 0; i < types.Count; i++)
            {
                _settingItems.Add(Activator.CreateInstance(types[i]) as SettingItemBase);
                _settingItemSigns.Add(false);
            }

            for (int i = 0; i < _settingItems.Count; i++)
            {
                _settingItems[i].OnBeginSetting();
            }

            _resetGUIContent = new GUIContent();
            _resetGUIContent.image = EditorGUIUtility.IconContent("_Popup").image;
            _resetGUIContent.tooltip = "Menu";

            _editGUIContent = new GUIContent();
            _editGUIContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            _editGUIContent.tooltip = "Edit Module";

            _currentItem = 0;

            _itemFilter = "";
        }
        private void OnDisable()
        {
            for (int i = 0; i < _settingItems.Count; i++)
            {
                _settingItems[i].OnEndSetting();
            }
        }
        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                Close();
            }
        }

        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            if (GUILayout.Button("New Setting Item", EditorStyles.toolbarPopup))
            {
                NewSettingItemScript();
            }

            GUILayout.FlexibleSpace();

            _itemFilter = EditorGUILayout.TextField("", _itemFilter, EditorGlobalTools.Styles.ToolbarSearchTextField);
            if (GUILayout.Button("", string.IsNullOrEmpty(_itemFilter) ? EditorGlobalTools.Styles.ToolbarSearchCancelButtonEmpty : EditorGlobalTools.Styles.ToolbarSearchCancelButton))
            {
                _itemFilter = "";
                GUI.FocusControl(null);
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            GUILayout.BeginHorizontal();
            
            OnItemNameGUI();

            GUILayout.Space(5);

            GUILayout.Box("", "DopesheetBackground", GUILayout.Width(5), GUILayout.ExpandHeight(true));

            GUILayout.Space(5);

            OnItemSettingGUI();

            GUILayout.EndHorizontal();
        }
        private void OnItemNameGUI()
        {
            GUILayout.BeginVertical(GUILayout.Width(250));

            _scrollItemNameGUI = GUILayout.BeginScrollView(_scrollItemNameGUI);

            for (int i = 0; i < _settingItems.Count; i++)
            {
                if (IsDisplay(_settingItems[i].Name))
                {
                    GUIContent gUIContent = new GUIContent(_settingItems[i].Name);
                    if (_settingItemSigns[i]) gUIContent.image = EditorGUIUtility.IconContent("Preset Icon").image;

                    if (_currentItem == i) GUILayout.BeginHorizontal("InsertionMarker");
                    else GUILayout.BeginHorizontal();
                    if (GUILayout.Button(gUIContent, EditorStyles.label, GUILayout.Height(20)))
                    {
                        _currentItem = i;
                        GUI.FocusControl(null);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        private void OnItemSettingGUI()
        {
            if (_currentItem != -1)
            {
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label(_settingItems[_currentItem].Name, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (_settingItemSigns[_currentItem])
                {
                    if (GUILayout.Button(_editGUIContent, "InvisibleButton", GUILayout.Height(20), GUILayout.Width(20)))
                    {
                        EditModule();
                    }
                }
                if (GUILayout.Button(_resetGUIContent, "InvisibleButton", GUILayout.Height(20), GUILayout.Width(20)))
                {
                    GenericMenu gm = new GenericMenu();
                    gm.AddItem(new GUIContent("Reset"), false, ResetCurrentItem);
                    gm.ShowAsContext();
                }
                GUILayout.Space(5);
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                _scrollItemSettingGUI = GUILayout.BeginScrollView(_scrollItemSettingGUI);
                
                _settingItems[_currentItem].OnSettingGUI();

                GUILayout.EndScrollView();

                GUILayout.EndVertical();
            }
        }

        private bool IsDisplay(string name)
        {
            if (string.IsNullOrEmpty(_itemFilter))
            {
                return true;
            }
            else
            {
                return name.ToLower().Contains(_itemFilter.ToLower());
            }
        }
        private void ResetCurrentItem()
        {
            _settingItems[_currentItem].OnReset();
            GUI.changed = true;
        }
        private void NewSettingItemScript()
        {
            EditorGlobalTools.CreateScriptFormTemplate(EditorPrefsTable.Script_SettingItem_Folder, "SettingItem (EditorOnly)", "SettingItemTemplate");
        }
        private void EditModule()
        {
            HTFrameworkModule module = _settingItems[_currentItem].GetType().GetCustomAttribute<InternalSettingItemAttribute>().Module;
            GameObject moduleEntity = null;
            switch (module)
            {
                case HTFrameworkModule.Main:
                    moduleEntity = GameObject.Find("HTFramework");
                    break;
                case HTFrameworkModule.WebRequest:
                    moduleEntity = GameObject.Find("HTFramework/WebRequest");
                    break;
            }

            if (moduleEntity)
            {
                Selection.activeGameObject = moduleEntity;
                EditorGUIUtility.PingObject(moduleEntity);
            }
            else
            {
                Log.Warning("未找到该设置项相关联的模块！");
            }
        }
    }
}