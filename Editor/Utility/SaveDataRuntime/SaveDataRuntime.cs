using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HT.Framework
{
    internal static class SaveDataRuntime
    {
        [InitializeOnLoadMethod]
        private static void InitSaver()
        {
            if (EditorPrefs.GetBool(EditorPrefsTable.SaveDataRuntime_Enable, true))
            {
                _inspectorWindowType = ReflectionToolkit.GetTypeInAllAssemblies("UnityEditor.InspectorWindow");
                _editorElementType = ReflectionToolkit.GetTypeInAllAssemblies("UnityEditor.UIElements.EditorElement");
                _trackerProperty = _inspectorWindowType.GetProperty("tracker", BindingFlags.Instance | BindingFlags.Public);
                _editorElementTargetProperty = _editorElementType.GetProperty("editor", BindingFlags.Instance | BindingFlags.Public);
                _saveIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/HTFramework/Editor/Main/Texture/SaveIcon.png");

                Selection.selectionChanged += OnSelectionChanged;
                EditorApplication.update += OnUpdateGUI;
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }
        }

        private static Type _inspectorWindowType;
        private static Type _editorElementType;
        private static PropertyInfo _trackerProperty;
        private static PropertyInfo _editorElementTargetProperty;
        private static Texture2D _saveIcon;

        private static ActiveEditorTracker _tracker;
        private static int _currentTrackerCount;
        private static List<RuntimeComponent> _runtimeComponents = new List<RuntimeComponent>();
        private static Dictionary<Component, SavedComponent> _savedComponents = new Dictionary<Component, SavedComponent>();

        private static void OnSelectionChanged()
        {
            if (!EditorApplication.isPlaying)
            {
                ClearRuntimeComponents();
                return;
            }

            if (Selection.gameObjects != null && Selection.gameObjects.Length > 1)
            {
                ClearRuntimeComponents();
                return;
            }

            if (Selection.activeGameObject == null)
            {
                ClearRuntimeComponents();
                return;
            }

            ClearRuntimeComponents();

            EditorWindow window = EditorWindow.GetWindow(_inspectorWindowType);
            _tracker = _trackerProperty.GetValue(window) as ActiveEditorTracker;
            _currentTrackerCount = _tracker.activeEditors.Length;
            CollectRuntimeComponents(window.rootVisualElement);
        }
        private static void OnUpdateGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            for (int i = 0; i < _runtimeComponents.Count; i++)
            {
                _runtimeComponents[i].OnUpdateGUI();
            }

            if (_tracker != null && _tracker.activeEditors != null)
            {
                if (_tracker.activeEditors.Length != _currentTrackerCount)
                {
                    OnSelectionChanged();
                }
            }
        }
        private static void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            ClearRuntimeComponents();

            if (stateChange == PlayModeStateChange.EnteredPlayMode)
            {
                _savedComponents.Clear();
            }
            else if (stateChange == PlayModeStateChange.ExitingPlayMode)
            {
                if (_savedComponents.Count > 0)
                {
                    SaveDataWindow.OpenWindow(_savedComponents);
                    _savedComponents.Clear();
                }
            }
        }

        private static void CollectRuntimeComponents(VisualElement element)
        {
            for (int i = 0; i < element.childCount; i++)
            {
                if (element[i].GetType() == _editorElementType)
                {
                    IMGUIContainer headerElement = FindHeaderElement(element[i]);
                    Editor editor = _editorElementTargetProperty.GetValue(element[i]) as Editor;
                    string path = AssetDatabase.GetAssetPath(editor.target);
                    if (editor != null && editor.target is Component && string.IsNullOrEmpty(path) && headerElement != null)
                    {
                        _runtimeComponents.Add(new RuntimeComponent(editor, headerElement));
                    }
                }

                CollectRuntimeComponents(element[i]);
            }
        }
        private static void ClearRuntimeComponents()
        {
            for (int i = 0; i < _runtimeComponents.Count; i++)
            {
                _runtimeComponents[i].Destroy();
            }
            _runtimeComponents.Clear();
            _tracker = null;
            _currentTrackerCount = 0;
        }
        private static IMGUIContainer FindHeaderElement(VisualElement element)
        {
            for (int i = 0; i < element.childCount; i++)
            {
                if (element[i].GetType() == typeof(IMGUIContainer) && element[i].name.EndsWith("Header"))
                {
                    return element[i] as IMGUIContainer;
                }
            }
            return null;
        }

        private static bool IsExistSavedComponent(Component component)
        {
            if (component)
            {
                return _savedComponents.ContainsKey(component);
            }
            return false;
        }
        private static void AddSavedComponent(Component component)
        {
            if (component)
            {
                if (!_savedComponents.ContainsKey(component))
                {
                    SetPrefabPath(component, out string prefabPath, out string componentPath);
                    if (!string.IsNullOrEmpty(prefabPath) && !string.IsNullOrEmpty(componentPath))
                    {
                        _savedComponents.Add(component, new SavedComponent(prefabPath, componentPath));
                    }
                    else
                    {
                        Log.Error("SaveDataRuntime：此组件不支持运行时保存数据，只有隶属于 Entity 模块和 UI 模块的预制体（其子级物体上的组件）支持运行时保存数据。");
                    }
                }
            }
        }
        private static void RemoveSavedComponent(Component component)
        {
            if (component)
            {
                if (_savedComponents.ContainsKey(component))
                {
                    _savedComponents.Remove(component);
                }
            }
        }
        private static void SetPrefabPath(Component component, out string prefabPath, out string componentPath)
        {
            if (Main.m_Entity)
            {
                EntityLogicBase entityLogic = Main.m_Entity.GetEntityByTransform(component.transform);
                if (entityLogic != null)
                {
                    Type type = entityLogic.GetType();
                    int index = Main.m_Entity.DefineEntityNames.IndexOf(type.FullName);
                    if (index != -1)
                    {
                        GameObject prefab = Main.m_Entity.DefineEntityTargets[index];
                        prefabPath = AssetDatabase.GetAssetPath(prefab);
                    }
                    else
                    {
                        EntityResourceAttribute attribute = type.GetCustomAttribute<EntityResourceAttribute>();
                        prefabPath = attribute.AssetPath;
                    }
                    componentPath = component.transform.ChildPathOf(entityLogic.Entity.transform);
                    return;
                }
            }

            if (Main.m_UI)
            {
                UILogicBase uiLogic = Main.m_UI.GetUIByTransform(component.transform);
                if (uiLogic != null)
                {
                    Type type = uiLogic.GetType();
                    int index = Main.m_UI.DefineUINames.IndexOf(type.FullName);
                    if (index != -1)
                    {
                        GameObject prefab = Main.m_UI.DefineUIEntitys[index];
                        prefabPath = AssetDatabase.GetAssetPath(prefab);
                    }
                    else
                    {
                        UIResourceAttribute attribute = type.GetCustomAttribute<UIResourceAttribute>();
                        prefabPath = attribute.AssetPath;
                    }
                    componentPath = component.transform.ChildPathOf(uiLogic.UIEntity.transform);
                    return;
                }
            }

            prefabPath = null;
            componentPath = null;
        }

        public class RuntimeComponent
        {
            public Editor Target;
            public IMGUIContainer HeaderElement;

            private Button _saveButton;

            public bool IsSaved
            {
                set
                {
                    _saveButton.style.unityBackgroundImageTintColor = value ? Color.green : Color.white;
                }
            }

            public RuntimeComponent(Editor target, IMGUIContainer headerElement)
            {
                Target = target;
                HeaderElement = headerElement;

                _saveButton = new Button(() =>
                {
                    Component component = Target.target as Component;
                    if (IsExistSavedComponent(component))
                    {
                        RemoveSavedComponent(component);
                    }
                    else
                    {
                        AddSavedComponent(component);
                    }
                    IsSaved = IsExistSavedComponent(component);
                });
                _saveButton.style.borderBottomWidth = 0;
                _saveButton.style.borderTopWidth = 0;
                _saveButton.style.borderLeftWidth = 0;
                _saveButton.style.borderRightWidth = 0;
                _saveButton.style.left = HeaderElement.layout.width - 90;
                _saveButton.style.marginTop = 3;
                _saveButton.style.marginBottom = 3;
                _saveButton.style.width = 16;
                _saveButton.style.height = 16;
                _saveButton.style.backgroundImage = _saveIcon;
                _saveButton.style.backgroundColor = Color.clear;

                IsSaved = IsExistSavedComponent(Target.target as Component);

                HeaderElement.Add(_saveButton);
            }
            public void OnUpdateGUI()
            {
                if (HeaderElement != null && _saveButton != null)
                {
                    _saveButton.style.left = HeaderElement.layout.width - 90;
                }
            }
            public void Destroy()
            {
                if (HeaderElement != null && _saveButton != null)
                {
                    HeaderElement.Remove(_saveButton);
                }
            }
        }
        public class SavedComponent
        {
            public string PrefabPath;
            public string ComponentPath;

            public SavedComponent(string prefabPath, string componentPath)
            {
                PrefabPath = prefabPath;
                ComponentPath = componentPath;
            }
        }
    }
}