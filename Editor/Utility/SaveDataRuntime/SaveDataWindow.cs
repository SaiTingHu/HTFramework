using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class SaveDataWindow : HTFEditorWindow
    {
        public static void OpenWindow(Dictionary<Component, SaveDataRuntime.SavedComponent> savedComponents)
        {
            SaveDataWindow window = GetWindow<SaveDataWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("SaveAs").image;
            window.titleContent.text = "Save Component Data";
            window.SaveData(savedComponents);
            window.Show();
        }

        private Vector2 _scroll;
        private GUIContent _componentGC;
        private GUIStyle _componentStyle;
        private List<SavedComponent> _savedComponents = new List<SavedComponent>();
        private List<GameObject> _prefabs = new List<GameObject>();

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_componentGC == null)
            {
                _componentGC = new GUIContent();
            }
            if (_componentStyle == null)
            {
                _componentStyle = new GUIStyle(EditorStyles.linkLabel);
                _componentStyle.hover.textColor = Color.yellow;
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            _scroll = GUILayout.BeginScrollView(_scroll);

            for (int i = 0; i < _prefabs.Count; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GameObject prefab = _prefabs[i];

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Prefab Asset", GUILayout.Width(200));
                EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
                GUI.enabled = prefab != null;
                if (GUILayout.Button("Open", GUILayout.Width(40)))
                {
                    AssetDatabase.OpenAsset(prefab);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                bool enabled = false;
                if (prefab != null)
                {
                    for (int j = 0; j < _savedComponents.Count; j++)
                    {
                        if (_savedComponents[j].Prefab == prefab)
                        {
                            enabled = _savedComponents[j].IsOpenedPrefab;
                            GUI.enabled = enabled;
                            OnSavedComponentGUI(_savedComponents[j]);
                            GUI.enabled = true;
                        }
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUI.enabled = enabled;
                    if (GUILayout.Button("Apply All", GUILayout.Width(70)))
                    {
                        if (EditorUtility.DisplayDialog("Prompt", "Are you sure you want to apply all changed component?", "Yes", "No"))
                        {
                            for (int j = 0; j < _savedComponents.Count; j++)
                            {
                                if (_savedComponents[j].Prefab == prefab)
                                {
                                    LoadComponentDataToOther(_savedComponents[j]);
                                }
                            }
                        }
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

                GUILayout.Space(5);
            }

            GUILayout.EndScrollView();
        }
        private void OnSavedComponentGUI(SavedComponent savedObject)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Changed Component", GUILayout.Width(180));
            _componentGC.image = EditorGUIUtility.ObjectContent(null, savedObject.ComponentType).image;
            _componentGC.text = $"{savedObject.Name} [{savedObject.ComponentName}]";
            _componentGC.tooltip = savedObject.ComponentPath;
            if (GUILayout.Button(_componentGC, _componentStyle, GUILayout.Height(18)))
            {
                if (savedObject.TargetGameObject != null)
                {
                    Selection.activeGameObject = savedObject.TargetGameObject;
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove",EditorStyles.miniButtonLeft, GUILayout.Width(60)))
            {
                _savedComponents.Remove(savedObject);
            }
            if (GUILayout.Button("Apply", EditorStyles.miniButtonRight, GUILayout.Width(50)))
            {
                LoadComponentDataToOther(savedObject);
            }
            GUILayout.EndHorizontal();
        }
        private void Update()
        {
            if (_savedComponents.Count <= 0)
            {
                Close();
            }
        }
        private void OnDestroy()
        {
            ClearFolder();
        }

        private string CreateFolder()
        {
            string rootPath = PathToolkit.ProjectPath + "Library/HTFramework/";
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            rootPath += "SaveDataRuntime/";
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            return rootPath;
        }
        private void ClearFolder()
        {
            string rootPath = PathToolkit.ProjectPath + "Library/HTFramework/SaveDataRuntime/";
            if (Directory.Exists(rootPath))
            {
                FileUtil.DeleteFileOrDirectory(rootPath);
            }
        }
        private void SaveData(Dictionary<Component, SaveDataRuntime.SavedComponent> savedComponents)
        {
            _prefabs.Clear();
            _savedComponents.Clear();

            string rootPath = CreateFolder();

            foreach (var item in savedComponents)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item.Value.PrefabPath);
                if (prefab != null && item.Key != null)
                {
                    string id = Guid.NewGuid().ToString("N").ToUpper();
                    string filePath = $"{rootPath}{id}.htsc";
                    SaveComponentDataToFile(item.Key, filePath);
                    if (!_prefabs.Contains(prefab)) _prefabs.Add(prefab);
                    _savedComponents.Add(new SavedComponent(prefab, item.Value.PrefabPath, item.Key.gameObject.name, item.Key.GetType().FullName, item.Value.ComponentPath, filePath));
                }
            }
        }

        private void SaveComponentDataToFile(Component component, string path)
        {
            Type type = component.GetType();
            GameObject gameObject = new GameObject();
            if (type == typeof(Transform))
            {
                Transform transform = gameObject.transform;
                EditorUtility.CopySerialized(component, transform);
            }
            else if (type == typeof(RectTransform))
            {
                RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
                EditorUtility.CopySerialized(component, rectTransform);
            }
            else
            {
                Component dest = gameObject.AddComponent(type);
                EditorUtility.CopySerialized(component, dest);
            }

            ScriptableToolkit.SaveGameObjectToFile(gameObject, path);
            DestroyImmediate(gameObject);
        }
        private void LoadComponentDataToOther(SavedComponent savedComponent)
        {
            if (savedComponent.TargetGameObject != null)
            {
                Selection.activeGameObject = savedComponent.TargetGameObject;
                if (savedComponent.TargetComponent == null)
                {
                    Undo.AddComponent(savedComponent.TargetGameObject, savedComponent.ComponentType);
                    HasChanged(savedComponent.TargetGameObject);
                }
                if (savedComponent.TargetComponent != null)
                {
                    GameObject gameObject = ScriptableToolkit.LoadGameObjectFromFile(savedComponent.FilePath);
                    if (gameObject)
                    {
                        Undo.RecordObject(savedComponent.TargetComponent, "Apply Runtime Data");
                        CopySerializedIgnoreObjectReference(gameObject.GetComponent(savedComponent.ComponentType), savedComponent.TargetComponent);
                        HasChanged(savedComponent.TargetComponent);
                        DestroyImmediate(gameObject);
                    }
                }
            }
        }
        private void CopySerializedIgnoreObjectReference(Component source, Component dest)
        {
            Editor sourceEditor = Editor.CreateEditor(source);
            Editor destEditor = Editor.CreateEditor(dest);

            using (SerializedProperty iterator = sourceEditor.serializedObject.GetIterator())
            {
                while (iterator.NextVisible(true))
                {
                    if (iterator.propertyType == SerializedPropertyType.Generic
                        || iterator.propertyType == SerializedPropertyType.ObjectReference
                        || iterator.propertyType == SerializedPropertyType.ExposedReference
                        || iterator.propertyType == SerializedPropertyType.ManagedReference)
                        continue;

                    destEditor.serializedObject.CopyFromSerializedProperty(iterator);
                }
            }
            destEditor.serializedObject.ApplyModifiedProperties();

            DestroyImmediate(sourceEditor);
            DestroyImmediate(destEditor);
        }

        [Serializable]
        public class SavedComponent
        {
            public GameObject Prefab;
            public string PrefabPath;
            public string Name;
            public string ComponentName;
            public string ComponentPath;
            public string FilePath;

            private Type _type;
            private GameObject _targetGameObject;
            private Component _targetComponent;

            public Type ComponentType
            {
                get
                {
                    if (_type == null)
                    {
                        _type = ReflectionToolkit.GetTypeInAllAssemblies(ComponentName);
                    }
                    return _type;
                }
            }
            public GameObject TargetGameObject
            {
                get
                {
                    if (_targetGameObject == null)
                    {
                        PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (prefabStage != null && prefabStage.assetPath == PrefabPath)
                        {
                            _targetGameObject = prefabStage.prefabContentsRoot.FindChildren(ComponentPath);
                        }
                    }
                    return _targetGameObject;
                }
            }
            public Component TargetComponent
            {
                get
                {
                    if (_targetComponent == null)
                    {
                        if (TargetGameObject != null)
                        {
                            _targetComponent = TargetGameObject.GetComponent(ComponentType);
                        }
                    }
                    return _targetComponent;
                }
            }
            public bool IsOpenedPrefab
            {
                get
                {
                    PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                    return prefabStage != null && prefabStage.assetPath == PrefabPath;
                }
            }

            public SavedComponent(GameObject prefab, string prefabPath, string name, string componentName, string componentPath, string filePath)
            {
                Prefab = prefab;
                PrefabPath = prefabPath;
                Name = name;
                ComponentName = componentName;
                ComponentPath = componentPath;
                FilePath = filePath;
            }
        }
    }
}