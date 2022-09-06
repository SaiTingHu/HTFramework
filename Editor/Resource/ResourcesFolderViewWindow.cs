﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class ResourcesFolderViewWindow : HTFEditorWindow
    {
        private List<ResourceFolder> _resourcesFolders = new List<ResourceFolder>();
        private Vector2 _scroll;

        protected override bool IsEnableTitleGUI => false;

        public void Init()
        {
            SearchResourcesFolder(Application.dataPath);
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            _scroll = GUILayout.BeginScrollView(_scroll);
            for (int i = 0; i < _resourcesFolders.Count; i++)
            {
                GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

                GUILayout.BeginHorizontal();
                _resourcesFolders[i].IsExpanding = EditorGUILayout.Foldout(_resourcesFolders[i].IsExpanding, string.Format("{0}  [{1}]", _resourcesFolders[i].AssetName, _resourcesFolders[i].Resources.Count), true);
                GUILayout.EndHorizontal();

                if (_resourcesFolders[i].IsExpanding)
                {
                    for (int j = 0; j < _resourcesFolders[i].Resources.Count; j++)
                    {
                        Object resource = _resourcesFolders[i].Resources[j];
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUIContent content = EditorGUIUtility.ObjectContent(resource, resource.GetType());
                        content.text = resource.name;
                        if (GUILayout.Button(content, "PR PrefabLabel", GUILayout.Height(20)))
                        {
                            Selection.activeObject = resource;
                            EditorGUIUtility.PingObject(resource);
                        }
                        if (resource is GameObject)
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Load to Scene", EditorStyles.miniButton))
                            {
                                GameObject obj = PrefabUtility.InstantiatePrefab(resource) as GameObject;
                                Selection.activeGameObject = obj;
                                EditorGUIUtility.PingObject(obj);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }
        private void Update()
        {
            if (EditorApplication.isCompiling)
            {
                Close();
            }
        }
        private void OnDestroy()
        {
            _resourcesFolders.Clear();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }
        private void SearchResourcesFolder(string folderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);
            if (!IsIgnoreFolder(directory.Name))
            {
                FileSystemInfo[] fis = directory.GetFileSystemInfos();
                for (int i = 0; i < fis.Length; i++)
                {
                    DirectoryInfo di = fis[i] as DirectoryInfo;
                    if (di != null)
                    {
                        if (di.Name == "Resources")
                        {
                            _resourcesFolders.Add(new ResourceFolder(di));
                        }
                        else
                        {
                            SearchResourcesFolder(di.FullName);
                        }
                    }
                }
            }
        }
        private bool IsIgnoreFolder(string folderName)
        {
            if (EditorPrefsTable.HTFrameworkFolder.Contains(folderName))
            {
                return true;
            }
            return false;
        }

        private class ResourceFolder
        {
            public string FullName;
            public string AssetName;
            public bool IsExpanding;
            public List<Object> Resources;

            public ResourceFolder(DirectoryInfo directoryInfo)
            {
                FullName = directoryInfo.FullName;
                AssetName = "Assets" + FullName.Replace("\\", "/").Replace(Application.dataPath, "");
                IsExpanding = false;
                Resources = new List<Object>();

                FileSystemInfo[] fis = directoryInfo.GetFileSystemInfos();
                for (int i = 0; i < fis.Length; i++)
                {
                    if (fis[i].Extension != ".meta")
                    {
                        string path = AssetName + "/" + fis[i].Name;
                        System.Type t = AssetDatabase.GetMainAssetTypeAtPath(path);
                        Resources.Add(AssetDatabase.LoadAssetAtPath(path, t));
                    }
                }
            }
        }
    }
}