using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 项目资源文件夹锁定器
    /// </summary>
    public static class ProjectFolderLocker
    {
        private static HashSet<string> FolderPaths = new HashSet<string>() { "Assets/HTFramework", "Assets/HTFrameworkAI", "Assets/HTFrameworkDeployment", "Assets/HTFrameworkGameComponent", "Assets/HTModuleManager", "Assets/Plugins" };
        private static object AssetTree;
        private static Dictionary<string, FolderItem> FolderItems = new Dictionary<string, FolderItem>();
        private static Action<TreeViewItem, bool, bool> ChangeExpandedState;
        private static GUIContent LockedGC;

        [InitializeOnLoadMethod]
        private static void InitLocker()
        {
            EditorApplication.delayCall += InitLockerAsync;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }
        internal static async void InitLockerAsync()
        {
            AssetTree = null;
            FolderItems.Clear();
            ChangeExpandedState = null;

            Type projectBrowserType = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.ProjectBrowser");
            FieldInfo m_AssetTree = projectBrowserType.GetField("m_AssetTree", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo m_ViewMode = projectBrowserType.GetField("m_ViewMode", BindingFlags.Instance | BindingFlags.NonPublic);
            EditorWindow projectBrowser = EditorWindow.GetWindow(projectBrowserType);

            bool isOneColumn = (int)m_ViewMode.GetValue(projectBrowser) == 0;
            if (isOneColumn)
            {
                TaskCompletionSource<object> task = new TaskCompletionSource<object>();
                using Timer timer = new Timer(100);
                timer.Elapsed += (sender, args) =>
                {
                    AssetTree = m_AssetTree.GetValue(projectBrowser);
                    if (AssetTree != null)
                    {
                        task.SetResult(null);
                        timer.Stop();
                    }
                };
                timer.Start();
                await task.Task;

                if (AssetTree != null)
                {
                    Type treeViewControllerType = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.IMGUI.Controls.TreeViewController");
                    MethodInfo FindItem = treeViewControllerType.GetMethod("FindItem", BindingFlags.Instance | BindingFlags.Public);

                    object[] para = new object[1];
                    foreach (var folderPath in FolderPaths)
                    {
                        if (AssetDatabase.IsValidFolder(folderPath))
                        {
                            DefaultAsset folder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);
                            if (folder != null)
                            {
                                int id = folder.GetInstanceID();
                                para[0] = id;
                                TreeViewItem treeViewItem = FindItem.Invoke(AssetTree, para) as TreeViewItem;
                                if (treeViewItem != null)
                                {
                                    string guid = AssetDatabase.AssetPathToGUID(folderPath);
                                    FolderItems.Add(guid, new FolderItem()
                                    {
                                        Item = treeViewItem,
                                        IsLock = true
                                    });
                                }
                            }
                        }
                    }

                    MethodInfo changeExpandedState = treeViewControllerType.GetMethod("ChangeExpandedState", BindingFlags.Instance | BindingFlags.NonPublic);
                    ChangeExpandedState = Delegate.CreateDelegate(typeof(Action<TreeViewItem, bool, bool>), AssetTree, changeExpandedState) as Action<TreeViewItem, bool, bool>;

                    PropertyInfo expandedStateChangedProperty = treeViewControllerType.GetProperty("expandedStateChanged", BindingFlags.Instance | BindingFlags.Public);
                    Action expandedStateChanged = expandedStateChangedProperty.GetValue(AssetTree) as Action;
                    expandedStateChanged += OnExpandedStateChanged;
                    expandedStateChangedProperty.SetValue(AssetTree, expandedStateChanged);

                    LockedGC = new GUIContent();
                    LockedGC.image = EditorGUIUtility.IconContent("d_AssemblyLock").image;

                    OnExpandedStateChanged();

                    projectBrowser.Repaint();
                }
            }
        }
        private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (IsSmallIcon(selectionRect) && FolderItems.ContainsKey(guid))
            {
                FolderItem folderItem = FolderItems[guid];

                LockedGC.tooltip = folderItem.IsLock ? "Locked" : "UnLocked";
                GUI.color = folderItem.IsLock ? Color.white : Color.gray;
                if (GUI.Button(new Rect(0, selectionRect.y, 20, 20), LockedGC, EditorStyles.iconButton))
                {
                    folderItem.IsLock = !folderItem.IsLock;
                    if (folderItem.IsLock)
                    {
                        OnExpandedStateChanged();
                    }
                }
                GUI.color = Color.white;

                if (folderItem.IsLock)
                {
                    selectionRect.width += (selectionRect.x - 18);
                    selectionRect.x = 18;
                    GUI.Button(selectionRect, "", "Box");
                }
            }
        }
        private static void OnExpandedStateChanged()
        {
            foreach (var item in FolderItems)
            {
                if (item.Value.IsLock)
                {
                    ChangeExpandedState(item.Value.Item, false, false);
                }
            }
        }
        private static bool IsSmallIcon(Rect rect)
        {
            return rect.width > rect.height;
        }

        /// <summary>
        /// 添加需要锁定的文件夹（建议调用时机：标记了[InitializeOnLoad]类的【静态构造方法】中，标记了[InitializeOnLoadMethod]的静态方法中）
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        public static void AddFolder(string folderPath)
        {
            FolderPaths.Add(folderPath);
        }

        private class FolderItem
        {
            public TreeViewItem Item;
            public bool IsLock;
        }
    }
}