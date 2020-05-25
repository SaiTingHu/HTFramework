using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class TaskEditorWindow : HTFEditorWindow
    {
        public static void ShowWindow(TaskContentAsset contentAsset)
        {
            TaskEditorWindow window = GetWindow<TaskEditorWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("AnimatorStateMachine Icon").image;
            window.titleContent.text = "Task Editor";
            window._asset = contentAsset;
            window.minSize = new Vector2(800, 600);
            window.maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            window.Show();
        }

        private TaskContentAsset _asset;
        private TaskContentBase _currentContent;
        private Texture _background;
        private ReorderableList _taskContentList;

        private bool _isShowContent = true;
        private bool _isShowProperty = true;
        private Vector2 _contentScroll;
        private int _contentGUIWidth = 250;
        private bool _isBreakDepend = false;
        private GUIContent _addGUIContent;
        private GUIContent _editGUIContent;
        private GUIContent _deleteGUIContent;
        
        private string MainAssetPath
        {
            get
            {
                return AssetDatabase.GetAssetPath(_asset);
            }
        }
        protected override bool IsEnableTitleGUI
        {
            get
            {
                return false;
            }
        }
        private void OnEnable()
        {
            _background = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/StepEditor/Texture/background.png");

            _addGUIContent = new GUIContent();
            _addGUIContent.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGUIContent.tooltip = "Add a task";
            _editGUIContent = new GUIContent();
            _editGUIContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            _editGUIContent.tooltip = "Edit Content Script";
            _deleteGUIContent = new GUIContent();
            _deleteGUIContent.image = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
            _deleteGUIContent.tooltip = "Delete";

            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }
        private void Update()
        {
            if (_asset == null)
            {
                Close();
            }
        }
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();
            
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), _background, new Rect(0, 0, position.width / 50, position.height / 50));
            
            OnPointGUI();
            OnDependGUI();
            OnTitleGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            OnContentGUI();
            GUILayout.Space(5);
            OnPropertyGUI();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            OnPointEventHandle();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_asset);
            }
        }
        /// <summary>
        /// 标题GUI
        /// </summary>
        private new void OnTitleGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button(_asset.name, EditorStyles.toolbarButton))
            {
                Selection.activeObject = _asset;
                EditorGUIUtility.PingObject(_asset);
            }
            if (GUILayout.Button("Regen Task ID", EditorStyles.toolbarPopup))
            {
                TaskRegenIDWindow.ShowWindow(this, _asset);
            }
            _isShowContent = GUILayout.Toggle(_isShowContent, "Task Content", EditorStyles.toolbarButton);
            _isShowProperty = GUILayout.Toggle(_isShowProperty, "Property", EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("ReSet", EditorStyles.toolbarButton))
            {
                ReSet();
            }
            if (GUILayout.Button("About", EditorStyles.toolbarButton))
            {
                Application.OpenURL(@"https://wanderer.blog.csdn.net/article/details/104317219");
            }
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// 任务内容GUI
        /// </summary>
        private void OnContentGUI()
        {
            if (_isShowContent)
            {
                GUILayout.BeginVertical(GUILayout.Width(_contentGUIWidth));

                GenerateTaskList();

                _contentScroll = GUILayout.BeginScrollView(_contentScroll);

                _taskContentList.DoLayoutList();

                GUILayout.EndScrollView();
                
                GUILayout.EndVertical();
            }
        }
        /// <summary>
        /// 任务属性GUI
        /// </summary>
        private void OnPropertyGUI()
        {
            if (_isShowProperty)
            {
                if (_currentContent != null)
                {
                    _currentContent.OnEditorGUI();
                }
                else
                {
                    GUILayout.Label("Please select a Task Content!", EditorStyles.boldLabel);
                }
            }
        }
        /// <summary>
        /// 任务点GUI
        /// </summary>
        private void OnPointGUI()
        {
            if (_currentContent != null)
            {
                for (int i = 0; i < _currentContent.Points.Count; i++)
                {
                    _currentContent.Points[i].OnEditorGUI(_currentContent);
                }
            }
        }
        /// <summary>
        /// 任务点依赖GUI
        /// </summary>
        private void OnDependGUI()
        {
            if (_currentContent != null)
            {
                for (int i = 0; i < _currentContent.Depends.Count; i++)
                {
                    TaskDepend depend = _currentContent.Depends[i];
                    Handles.DrawBezier(_currentContent.Points[depend.OriginalPoint].LeftPosition, _currentContent.Points[depend.DependPoint].RightPosition
                        , _currentContent.Points[depend.OriginalPoint].LeftTangent, _currentContent.Points[depend.DependPoint].RightTangent, Color.white, null, 3);

                    if (_isBreakDepend)
                    {
                        Vector2 center = (_currentContent.Points[depend.OriginalPoint].LeftPosition + _currentContent.Points[depend.DependPoint].RightPosition) / 2;
                        Rect centerRect = new Rect(center.x - 8, center.y - 8, 20, 20);
                        if (GUI.Button(centerRect, "", EditorGlobalTools.Styles.OLMinus))
                        {
                            _currentContent.Depends.RemoveAt(i);
                            break;
                        }
                        EditorGUIUtility.AddCursorRect(centerRect, MouseCursor.ArrowMinus);
                    }
                }
            }
        }
        /// <summary>
        /// 任务点GUI事件处理
        /// </summary>
        private void OnPointEventHandle()
        {
            if (_currentContent != null && Event.current != null)
            {
                Vector2 mousePosition = Event.current.mousePosition;

                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if (Event.current.button == 1)
                        {
                            GenericMenu gm = new GenericMenu();
                            gm.AddItem(new GUIContent("<New Task Point Script>"), false, () =>
                            {
                                NewTaskPointScript();
                            });
                            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                            {
                                return type.IsSubclassOf(typeof(TaskPointBase));
                            });
                            for (int i = 0; i < types.Count; i++)
                            {
                                Type type = types[i];
                                string contentName = type.FullName;
                                TaskPointAttribute attri = type.GetCustomAttribute<TaskPointAttribute>();
                                if (attri != null)
                                {
                                    contentName = attri.Name;
                                }
                                gm.AddItem(new GUIContent("Add Task Point/" + contentName), false, () =>
                                {
                                    AddPoint(type, mousePosition);
                                });
                            }
                            StringToolkit.BeginNoRepeatNaming();
                            for (int i = 0; i < _currentContent.Points.Count; i++)
                            {
                                TaskPointBase point = _currentContent.Points[i];
                                gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName("Find Task Point/" + point.Name)), false, () =>
                                {
                                    FindPoint(point);
                                });
                            }
                            gm.ShowAsContext();
                        }
                        break;
                    case EventType.MouseDrag:
                        if (Event.current.button == 2)
                        {
                            for (int i = 0; i < _currentContent.Points.Count; i++)
                            {
                                _currentContent.Points[i].OnDrag(Event.current.delta);
                            }
                            GUI.changed = true;
                        }
                        break;
                    case EventType.KeyDown:
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.LeftAlt:
                            case KeyCode.RightAlt:
                                _isBreakDepend = true;
                                GUI.changed = true;
                                break;
                        }
                        break;
                    case EventType.KeyUp:
                        switch (Event.current.keyCode)
                        {
                            case KeyCode.LeftAlt:
                            case KeyCode.RightAlt:
                                _isBreakDepend = false;
                                GUI.changed = true;
                                break;
                        }
                        break;
                }

                for (int i = 0; i < _currentContent.Points.Count; i++)
                {
                    _currentContent.Points[i].OnPointEventHandle(Event.current, _currentContent);
                }
            }
        }
        
        /// <summary>
        /// 新增任务内容
        /// </summary>
        private void AddContent(Type type)
        {
            TaskContentBase taskContent = CreateInstance(type) as TaskContentBase;
            taskContent.GUID = _asset.TaskIDName + _asset.TaskIDSign.ToString();
            taskContent.Details = taskContent.Name = "New Task " + _asset.TaskIDSign.ToString();
            _asset.TaskIDSign += 1;
            _asset.Content.Add(taskContent);
            _taskContentList.index = _asset.Content.Count - 1;
            _currentContent = taskContent;

            TaskContentBase.GenerateSerializeSubObject(taskContent, _asset);
        }
        /// <summary>
        /// 删除任务内容
        /// </summary>
        private void DeleteContent(int taskIndex)
        {
            for (int i = 0; i < _asset.Content[taskIndex].Points.Count; i++)
            {
                TaskContentBase.DestroySerializeSubObject(_asset.Content[taskIndex].Points[i], _asset);
            }
            TaskContentBase.DestroySerializeSubObject(_asset.Content[taskIndex], _asset);

            _asset.Content[taskIndex].Depends.Clear();
            _asset.Content[taskIndex].Points.Clear();
            _asset.Content.RemoveAt(taskIndex);
            _taskContentList.index = -1;
            _currentContent = null;
        }
        /// <summary>
        /// 新建任务内容脚本
        /// </summary>
        private void NewTaskContentScript()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_TaskContent_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 TaskContent 类", directory, "NewTaskContent", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/TaskContentTemplate.txt", typeof(TextAsset)) as TextAsset;
                    if (asset)
                    {
                        string code = asset.text;
                        code = code.Replace("#SCRIPTNAME#", className);
                        File.AppendAllText(path, code);
                        asset = null;
                        AssetDatabase.Refresh();

                        string assetPath = path.Substring(path.LastIndexOf("Assets"));
                        TextAsset cs = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                        EditorGUIUtility.PingObject(cs);
                        Selection.activeObject = cs;
                        AssetDatabase.OpenAsset(cs);
                        EditorPrefs.SetString(EditorPrefsTable.Script_TaskContent_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    Log.Error("新建TaskContent失败，已存在类型 " + className);
                }
            }
        }
        /// <summary>
        /// 生成任务内容列表
        /// </summary>
        private void GenerateTaskList()
        {
            if (_taskContentList == null)
            {
                _taskContentList = new ReorderableList(_asset.Content, typeof(TaskContentBase), true, true, false, false);
                _taskContentList.drawHeaderCallback = (Rect rect) =>
                {
                    Rect sub = rect;
                    sub.Set(rect.x, rect.y, 200, rect.height);
                    GUI.Label(sub, "Task Content List:");

                    sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                    if (GUI.Button(sub, _addGUIContent, "InvisibleButton"))
                    {
                        GenericMenu gm = new GenericMenu();
                        gm.AddItem(new GUIContent("<New Task Content Script>"), false, () =>
                        {
                            NewTaskContentScript();
                        });
                        List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                        {
                            return type.IsSubclassOf(typeof(TaskContentBase));
                        });
                        for (int i = 0; i < types.Count; i++)
                        {
                            Type type = types[i];
                            string contentName = type.FullName;
                            TaskContentAttribute attri = type.GetCustomAttribute<TaskContentAttribute>();
                            if (attri != null)
                            {
                                contentName = attri.Name;
                            }
                            gm.AddItem(new GUIContent(contentName), false, () =>
                            {
                                AddContent(type);
                            });
                        }
                        gm.ShowAsContext();
                    }
                };
                _taskContentList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0 && index < _asset.Content.Count)
                    {
                        Rect sub = rect;
                        GUI.Label(sub, (index + 1).ToString() + "." + _asset.Content[index].Name);

                        if (isActive)
                        {
                            sub.Set(rect.x + rect.width - 40, rect.y, 20, 20);
                            if (GUI.Button(sub, _editGUIContent, "InvisibleButton"))
                            {
                                MonoScript monoScript = MonoScript.FromScriptableObject(_asset.Content[index]);
                                AssetDatabase.OpenAsset(monoScript);
                            }
                            sub.Set(rect.x + rect.width - 20, rect.y, 20, 20);
                            if (GUI.Button(sub, _deleteGUIContent, "InvisibleButton"))
                            {
                                if (EditorUtility.DisplayDialog("Prompt", "Are you sure delete task [" + _asset.Content[index].Name + "]?", "Yes", "No"))
                                {
                                    DeleteContent(index);
                                }
                            }
                        }
                    }
                };
                _taskContentList.onSelectCallback = (ReorderableList list) =>
                {
                    if (list.index >= 0 && list.index < _asset.Content.Count)
                    {
                        _currentContent = _asset.Content[list.index];
                    }
                    else
                    {
                        _currentContent = null;
                    }
                };
                _taskContentList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0 && index < _asset.Content.Count)
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "CN EntryBackodd";
                            gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                            gUIStyle.Draw(rect, false, isActive, isActive, isFocused);

                            if (_asset.Content[index].IsDone)
                            {
                                GUI.backgroundColor = Color.green;
                                GUI.Box(rect, "");
                                GUI.backgroundColor = Color.white;
                            }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// 新增任务点
        /// </summary>
        private void AddPoint(Type type, Vector2 position)
        {
            TaskPointBase taskPoint = CreateInstance(type) as TaskPointBase;
            taskPoint.Anchor = new Rect(position.x, position.y, 200, 85);
            taskPoint.GUID = _asset.TaskPointIDName + _asset.TaskPointIDSign.ToString();
            taskPoint.Details = taskPoint.Name = "New Task Point " + _asset.TaskPointIDSign.ToString();
            _asset.TaskPointIDSign += 1;
            _currentContent.Points.Add(taskPoint);

            TaskContentBase.GenerateSerializeSubObject(taskPoint, _asset);
        }
        /// <summary>
        /// 查找任务点
        /// </summary>
        private void FindPoint(TaskPointBase taskPoint)
        {
            taskPoint.Anchor.position = new Vector2(position.width / 2 - 150, position.height / 2 - 150);
        }
        /// <summary>
        /// 新建任务点脚本
        /// </summary>
        private void NewTaskPointScript()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_TaskPoint_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 TaskPoint 类", directory, "NewTaskPoint", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/TaskPointTemplate.txt", typeof(TextAsset)) as TextAsset;
                    if (asset)
                    {
                        string code = asset.text;
                        code = code.Replace("#SCRIPTNAME#", className);
                        File.AppendAllText(path, code);
                        asset = null;
                        AssetDatabase.Refresh();

                        string assetPath = path.Substring(path.LastIndexOf("Assets"));
                        TextAsset cs = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                        EditorGUIUtility.PingObject(cs);
                        Selection.activeObject = cs;
                        AssetDatabase.OpenAsset(cs);
                        EditorPrefs.SetString(EditorPrefsTable.Script_TaskPoint_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    Log.Error("新建TaskPoint失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 当编辑器播放状态改变
        /// </summary>
        private void OnPlayModeStateChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ReSet();
            }
        }
        /// <summary>
        /// 重置状态
        /// </summary>
        private void ReSet()
        {
            for (int i = 0; i < _asset.Content.Count; i++)
            {
                TaskContentBase taskContent = _asset.Content[i];
                taskContent.ReSet();
                for (int j = 0; j < taskContent.Points.Count; j++)
                {
                    taskContent.Points[j].ReSet();
                }
            }
        }
    }
}