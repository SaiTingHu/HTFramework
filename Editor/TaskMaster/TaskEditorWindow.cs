using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 任务编辑器窗口
    /// </summary>
    public sealed class TaskEditorWindow : HTFEditorWindow, ILocalizeWindow
    {
        private static HTFFunc<string, string> NewTaskPointScriptHandler;

        /// <summary>
        /// 注册【新建任务点脚本】时的自定义处理者
        /// </summary>
        /// <param name="handler">自定义处理者</param>
        public static void RegisterNewTaskPointScriptHandler(HTFFunc<string, string> handler)
        {
            NewTaskPointScriptHandler = handler;
        }
        /// <summary>
        /// 打开窗口
        /// </summary>
        /// <param name="contentAsset">任务资源</param>
        public static void ShowWindow(TaskContentAsset contentAsset)
        {
            TaskEditorWindow window = GetWindow<TaskEditorWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("AnimatorStateMachine Icon").image;
            window.titleContent.text = "Task Editor";
            window._contentAsset = contentAsset;
            window._currentContent = null;
            window._taskContentList = null;
            window._isMinimize = false;
            if (!EditorApplication.isPlaying)
            {
                window.ReSet();
            }
            window.minSize = new Vector2(800, 600);
            window.maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            window.Show();
        }

        private TaskContentAsset _contentAsset;
        private TaskContentBase _currentContent;
        private Texture _background;
        private ReorderableList _taskContentList;
        private bool _isMinimize = false;
        private Rect _recordedPosition;

        private bool _isShowContent = true;
        private bool _isShowProperty = true;
        private bool _isShowPoint = true;
        private bool _isLockID = true;
        private bool _isExpandOnlySelected = false;
        private bool _isShowPointFullName = false;
        private Vector2 _contentScroll;
        private int _contentGUIWidth = 250;
        private bool _isBreakDepend = false;
        private GUIContent _addGC;
        private GUIContent _editGC;
        private GUIContent _deleteGC;
        private HTFFunc<string, string> _getWord;

        protected override bool IsEnableTitleGUI
        {
            get
            {
                return !_isMinimize;
            }
        }
        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/104317219";

        protected override void OnEnable()
        {
            base.OnEnable();

            _background = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/Grid.png");

            _isExpandOnlySelected = EditorPrefs.GetBool(EditorPrefsTable.TaskEditorWindow_ExpandOnlySelected, false);
            _isShowPointFullName = EditorPrefs.GetBool(EditorPrefsTable.TaskEditorWindow_ShowPointFullName, false);
            _addGC = new GUIContent();
            _addGC.image = EditorGUIUtility.IconContent("d_Toolbar Plus More").image;
            _addGC.tooltip = "Add Task Content";
            _editGC = new GUIContent();
            _editGC.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            _editGC.tooltip = "Edit Content Script";
            _deleteGC = new GUIContent();
            _deleteGC.image = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
            _deleteGC.tooltip = "Delete";
            _getWord = GetWord;

            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }
        private void Update()
        {
            if (_contentAsset == null)
            {
                Close();
            }
        }
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        }
        protected override void OnGUIReady()
        {
            base.OnGUIReady();

            if (_isMinimize)
                return;

            GUI.DrawTextureWithTexCoords(new Rect(0, 0, position.width, position.height), _background, new Rect(0, 0, position.width / 50, position.height / 50));

            OnPointGUI();
            OnDependGUI();
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            if (GUILayout.Button(_contentAsset.name, EditorStyles.toolbarButton))
            {
                Selection.activeObject = _contentAsset;
                EditorGUIUtility.PingObject(_contentAsset);
            }
            if (GUILayout.Button(GetWord("Regen Task ID"), EditorStyles.toolbarPopup))
            {
                TaskRegenIDWindow.ShowWindow(this, _contentAsset, CurrentLanguage);
            }
            _isShowContent = GUILayout.Toggle(_isShowContent, GetWord("Task Content"), EditorStyles.toolbarButton);
            _isShowProperty = GUILayout.Toggle(_isShowProperty, GetWord("Task Content Properties"), EditorStyles.toolbarButton);
            _isShowPoint = GUILayout.Toggle(_isShowPoint, GetWord("Task Point"), EditorStyles.toolbarButton);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GetWord("ReSet State"), EditorStyles.toolbarButton))
            {
                if (EditorApplication.isPlaying)
                {
                    if (EditorUtility.DisplayDialog("Prompt", "Currently is playing mode. Are you sure you want to reset status?", "Yes", "No"))
                    {
                        ReSet();
                    }
                }
                else
                {
                    ReSet();
                }
            }
            if (GUILayout.Button(GetWord("Setting"), EditorStyles.toolbarPopup))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent(GetWord("Lock ID")), _isLockID, () =>
                {
                    _isLockID = !_isLockID;
                });
                gm.AddItem(new GUIContent(GetWord("Expand Point Only Selected")), _isExpandOnlySelected, () =>
                {
                    _isExpandOnlySelected = !_isExpandOnlySelected;
                    EditorPrefs.SetBool(EditorPrefsTable.TaskEditorWindow_ExpandOnlySelected, _isExpandOnlySelected);
                });
                gm.AddItem(new GUIContent(GetWord("Show Point Full Name")), _isShowPointFullName, () =>
                {
                    _isShowPointFullName = !_isShowPointFullName;
                    EditorPrefs.SetBool(EditorPrefsTable.TaskEditorWindow_ShowPointFullName, _isShowPointFullName);
                });
                gm.ShowAsContext();
            }
            if (GUILayout.Button(GetWord("Minimize"), EditorStyles.toolbarPopup))
            {
                MinimizeWindow();
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (_isMinimize)
            {
                MinimizeGUI();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                OnContentGUI();
                GUILayout.Space(5);
                OnPropertyGUI();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                OnPointEventHandle();
            }

            if (GUI.changed)
            {
                HasChanged(_contentAsset);
            }
        }
        protected override void GenerateWords()
        {
            base.GenerateWords();

            AddWord("重新生成任务身份号", "Regen Task ID");
            AddWord("任务内容", "Task Content");
            AddWord("任务内容的属性", "Task Content Properties");
            AddWord("任务点", "Task Point");
            AddWord("重置状态", "ReSet State");
            AddWord("任务内容列表", "Task Content List");
            AddWord("<新建任务内容脚本>", "<New Task Content Script>");
            AddWord("<新建任务点脚本>", "<New Task Point Script>");
            AddWord("添加任务内容", "Add Task Content");
            AddWord("添加任务点", "Add Task Point");
            AddWord("查找任务点", "Find Task Point");
            AddWord("任务内容属性", "Task Content Property");
            AddWord("设置", "Setting");
            AddWord("最小化", "Minimize");
            AddWord("最大化", "Maximize");
            AddWord("锁定身份号", "Lock ID");
            AddWord("仅在选中时展开任务点", "Expand Point Only Selected");
            AddWord("显示任务点全名", "Show Point Full Name");
            AddWord("身份号", "ID");
            AddWord("名称", "Name");
            AddWord("细节", "Details");
            AddWord("是否显示细节提示", "Is Display Details");
            AddWord("任务点", "Points");
            AddWord("目标", "Target");
            AddWord("依赖的任务点", "Dependent task point");
            AddWord("被依赖的任务点", "Be dependent task point");
            AddWord("编辑任务点脚本", "Edit Point Script");
            AddWord("删除", "Delete");
            AddWord("删除任务点", "Delete Point");
            AddWord("复制", "Copy");
            AddWord("粘贴", "Paste");
            AddWord("克隆", "Clone");
            AddWord("全部折叠", "Collapse All");
            AddWord("全部展开", "Expand All");
            AddWord("全部启用", "Enable All");
            AddWord("全部禁用", "Disable All");
            AddWord("已启用", "ENABLE");
            AddWord("已禁用", "DISABLED");
        }

        /// <summary>
        /// 最小化后的GUI
        /// </summary>
        private void MinimizeGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(GetWord("Maximize"), EditorStyles.toolbarButton))
            {
                MaximizeWindow();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label($"{GetWord("Task Content Count")}:{_contentAsset.Content.Count}");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
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
        /// 任务内容属性GUI
        /// </summary>
        private void OnPropertyGUI()
        {
            if (_isShowProperty)
            {
                if (_currentContent != null)
                {
                    _currentContent.OnEditorGUI(_contentAsset, _getWord, _isLockID);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    string prompt = CurrentLanguage == Language.English ? "Please select a Task Content!" : "请选择一个任务内容！";
                    GUILayout.Label(prompt, EditorStyles.boldLabel);
                    GUILayout.EndHorizontal();
                }
            }
        }
        /// <summary>
        /// 任务点GUI
        /// </summary>
        private void OnPointGUI()
        {
            if (_isShowPoint)
            {
                if (_currentContent != null)
                {
                    for (int i = 0; i < _currentContent.Points.Count; i++)
                    {
                        _currentContent.Points[i].OnEditorGUI(_contentAsset, _currentContent, _getWord, _isLockID, _isExpandOnlySelected, _isShowPointFullName);
                    }
                }
            }
        }
        /// <summary>
        /// 任务点依赖GUI
        /// </summary>
        private void OnDependGUI()
        {
            if (_isShowPoint)
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
                            Vector2 center = (_currentContent.Points[depend.OriginalPoint].LeftPosition + _currentContent.Points[depend.DependPoint].RightPosition) * 0.5f;
                            Rect centerRect = new Rect(center.x - 10, center.y - 10, 20, 20);
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
        }
        /// <summary>
        /// 任务点GUI事件处理
        /// </summary>
        private void OnPointEventHandle()
        {
            if (_currentContent != null && Event.current != null)
            {
                Vector2 pos = Event.current.mousePosition;

                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if (Event.current.button == 1)
                        {
                            GenericMenu gm = new GenericMenu();
                            gm.AddItem(new GUIContent(GetWord("Add Task Point") + "/默认"), false, () =>
                            {
                                AddPoint(_currentContent, typeof(TaskPointDefault), pos);
                            });
                            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                            {
                                return type.IsSubclassOf(typeof(TaskPointBase)) && !type.IsAbstract && type != typeof(TaskPointDefault);
                            }, false);
                            for (int i = 0; i < types.Count; i++)
                            {
                                Type type = types[i];
                                string pointName = type.FullName;
                                TaskPointAttribute attribute = type.GetCustomAttribute<TaskPointAttribute>();
                                if (attribute != null)
                                {
                                    pointName = attribute.Name;
                                }
                                gm.AddItem(new GUIContent($"{GetWord("Add Task Point")}/{pointName}"), false, () =>
                                {
                                    AddPoint(_currentContent, type, pos);
                                });
                            }
                            StringToolkit.BeginNoRepeatNaming();
                            for (int i = 0; i < _currentContent.Points.Count; i++)
                            {
                                TaskPointBase point = _currentContent.Points[i];
                                gm.AddItem(new GUIContent(StringToolkit.GetNoRepeatName($"{GetWord("Find Task Point")}/{point.Name}")), false, () =>
                                {
                                    FindPoint(point);
                                });
                            }
                            gm.AddSeparator("");
                            CopyTaskPoint(gm);
                            PasteTaskPoint(gm, pos);
                            gm.AddSeparator("");
                            if (_isExpandOnlySelected)
                            {
                                gm.AddDisabledItem(new GUIContent(GetWord("Collapse All")));
                                gm.AddDisabledItem(new GUIContent(GetWord("Expand All")));
                            }
                            else
                            {
                                gm.AddItem(new GUIContent(GetWord("Collapse All")), false, () =>
                                {
                                    for (int i = 0; i < _currentContent.Points.Count; i++)
                                    {
                                        TaskPointBase point = _currentContent.Points[i];
                                        point.IsExpand = false;
                                    }
                                    GUI.changed = true;
                                });
                                gm.AddItem(new GUIContent(GetWord("Expand All")), false, () =>
                                {
                                    for (int i = 0; i < _currentContent.Points.Count; i++)
                                    {
                                        TaskPointBase point = _currentContent.Points[i];
                                        point.IsExpand = true;
                                    }
                                    GUI.changed = true;
                                });
                            }
                            gm.AddSeparator("");
                            gm.AddItem(new GUIContent(GetWord("Enable All")), false, () =>
                            {
                                for (int i = 0; i < _currentContent.Points.Count; i++)
                                {
                                    TaskPointBase point = _currentContent.Points[i];
                                    point.IsEnable = true;
                                }
                                GUI.changed = true;
                            });
                            gm.AddItem(new GUIContent(GetWord("Disable All")), false, () =>
                            {
                                for (int i = 0; i < _currentContent.Points.Count; i++)
                                {
                                    TaskPointBase point = _currentContent.Points[i];
                                    point.IsEnable = false;
                                }
                                GUI.changed = true;
                            });
                            gm.AddSeparator("");
                            gm.AddItem(new GUIContent(GetWord("<New Task Point Script>")), false, () =>
                            {
                                NewTaskPointScript();
                            });
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
                    _currentContent.Points[i].OnPointEventHandle(Event.current, _contentAsset, _currentContent, _getWord);
                }
            }
        }
        
        /// <summary>
        /// 新增任务内容
        /// </summary>
        private void AddContent(Type type)
        {
            TaskContentAttribute attribute = type.GetCustomAttribute<TaskContentAttribute>();
            TaskContentBase taskContent = CreateInstance(type) as TaskContentBase;
            taskContent.GUID = _contentAsset.TaskIDName + _contentAsset.TaskIDSign.ToString();
            taskContent.Name = (attribute != null ? attribute.GetLastName() : "New Task  ") + _contentAsset.TaskIDSign.ToString();
            _contentAsset.TaskIDSign += 1;
            _contentAsset.Content.Add(taskContent);
            _taskContentList.index = _contentAsset.Content.Count - 1;
            _currentContent = taskContent;

            TaskContentAsset.GenerateSerializeSubObject(taskContent, _contentAsset);
        }
        /// <summary>
        /// 克隆任务内容
        /// </summary>
        private void CloneContent(TaskContentBase content)
        {
            TaskContentBase taskContent = content.Clone();
            taskContent.Points.Clear();
            for (int i = 0; i < content.Points.Count; i++)
            {
                ClonePoint(taskContent, content.Points[i], content.Points[i].Anchor.position);
            }
            taskContent.GUID = _contentAsset.TaskIDName + _contentAsset.TaskIDSign.ToString();
            _contentAsset.TaskIDSign += 1;
            _contentAsset.Content.Add(taskContent);
            _taskContentList.index = _contentAsset.Content.Count - 1;
            _currentContent = taskContent;

            TaskContentAsset.GenerateSerializeSubObject(taskContent, _contentAsset);
        }
        /// <summary>
        /// 删除任务内容
        /// </summary>
        private void DeleteContent(int taskIndex)
        {
            for (int i = 0; i < _contentAsset.Content[taskIndex].Points.Count; i++)
            {
                TaskContentAsset.DestroySerializeSubObject(_contentAsset.Content[taskIndex].Points[i], _contentAsset);
            }
            TaskContentAsset.DestroySerializeSubObject(_contentAsset.Content[taskIndex], _contentAsset);

            _contentAsset.Content[taskIndex].Depends.Clear();
            _contentAsset.Content[taskIndex].Points.Clear();
            _contentAsset.Content.RemoveAt(taskIndex);
            _taskContentList.index = -1;
            _currentContent = null;
            HasChanged(_contentAsset);
        }
        /// <summary>
        /// 复制任务内容
        /// </summary>
        private void CopyTaskContent(GenericMenu gm)
        {
            if (_currentContent == null)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Copy")));
            }
            else
            {
                string assetPath = AssetDatabase.GetAssetPath(_contentAsset);
                gm.AddItem(new GUIContent($"{GetWord("Copy")} {_currentContent.Name}"), false, () =>
                {
                    GUIUtility.systemCopyBuffer = $"TaskContent|{assetPath}|{_currentContent.GUID}";
                });
            }
        }
        /// <summary>
        /// 粘贴、克隆任务内容
        /// </summary>
        private void PasteCloneTaskContent(GenericMenu gm)
        {
            string assetPath = AssetDatabase.GetAssetPath(_contentAsset);

            TaskContentBase content = null;
            string[] buffers = GUIUtility.systemCopyBuffer.Split('|');
            if (buffers.Length == 3 && buffers[0] == "TaskContent")
            {
                if (buffers[1] == assetPath)
                {
                    content = _contentAsset.Content.Find((s) => { return s.GUID == buffers[2]; });
                }
                else
                {
                    TaskContentAsset taskContentAsset = AssetDatabase.LoadAssetAtPath<TaskContentAsset>(buffers[1]);
                    if (taskContentAsset)
                    {
                        content = taskContentAsset.Content.Find((s) => { return s.GUID == buffers[2]; });
                    }
                }
            }

            if (content == null)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Paste")));
            }
            else
            {
                gm.AddItem(new GUIContent($"{GetWord("Paste")} {content.Name}"), false, () =>
                {
                    CloneContent(content);
                });
            }

            if (_currentContent == null)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Clone")));
            }
            else
            {
                gm.AddItem(new GUIContent(GetWord("Clone")), false, () =>
                {
                    CloneContent(_currentContent);
                });
            }
        }
        /// <summary>
        /// 新建任务内容脚本
        /// </summary>
        private void NewTaskContentScript()
        {
            EditorGlobalTools.CreateScriptFormTemplate(EditorPrefsTable.Script_TaskContent_Folder, "TaskContent", "TaskContentTemplate");
        }
        /// <summary>
        /// 生成任务内容列表
        /// </summary>
        private void GenerateTaskList()
        {
            if (_taskContentList == null)
            {
                _taskContentList = new ReorderableList(_contentAsset.Content, typeof(TaskContentBase), true, true, false, false);
                _taskContentList.drawHeaderCallback = (Rect rect) =>
                {
                    Rect sub = rect;
                    sub.Set(rect.x, rect.y, 200, rect.height);
                    GUI.Label(sub, GetWord("Task Content List") + ":");

                    sub.Set(rect.x + rect.width - 20, rect.y - 2, 20, 20);
                    if (GUI.Button(sub, _addGC, "InvisibleButton"))
                    {
                        GenericMenu gm = new GenericMenu();
                        gm.AddItem(new GUIContent(GetWord("Add Task Content") + "/默认"), false, () =>
                        {
                            AddContent(typeof(TaskContentDefault));
                        });
                        List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies(type =>
                        {
                            return type.IsSubclassOf(typeof(TaskContentBase)) && !type.IsAbstract && type != typeof(TaskContentDefault);
                        }, false);
                        for (int i = 0; i < types.Count; i++)
                        {
                            Type type = types[i];
                            string contentName = type.FullName;
                            TaskContentAttribute attribute = type.GetCustomAttribute<TaskContentAttribute>();
                            if (attribute != null)
                            {
                                contentName = attribute.Name;
                            }
                            gm.AddItem(new GUIContent($"{GetWord("Add Task Content")}/{contentName}"), false, () =>
                            {
                                AddContent(type);
                            });
                        }
                        gm.AddSeparator("");
                        CopyTaskContent(gm);
                        PasteCloneTaskContent(gm);
                        gm.AddSeparator("");
                        gm.AddItem(new GUIContent(GetWord("<New Task Content Script>")), false, () =>
                        {
                            NewTaskContentScript();
                        });
                        gm.ShowAsContext();
                    }
                };
                _taskContentList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0 && index < _contentAsset.Content.Count)
                    {
                        Rect sub = rect;
                        GUI.Label(sub, $"{index + 1}.{_contentAsset.Content[index].Name}");

                        if (isActive)
                        {
                            sub.Set(rect.x + rect.width - 40, rect.y, 20, 20);
                            if (GUI.Button(sub, _editGC, "InvisibleButton"))
                            {
                                MonoScript monoScript = MonoScript.FromScriptableObject(_contentAsset.Content[index]);
                                AssetDatabase.OpenAsset(monoScript);
                            }
                            sub.Set(rect.x + rect.width - 20, rect.y, 20, 20);
                            if (GUI.Button(sub, _deleteGC, "InvisibleButton"))
                            {
                                if (EditorUtility.DisplayDialog("Prompt", $"Are you sure delete task [{_contentAsset.Content[index].Name}]?", "Yes", "No"))
                                {
                                    DeleteContent(index);
                                }
                            }
                        }
                    }
                };
                _taskContentList.onSelectCallback = (ReorderableList list) =>
                {
                    if (list.index >= 0 && list.index < _contentAsset.Content.Count)
                    {
                        _currentContent = _contentAsset.Content[list.index];
                    }
                    else
                    {
                        _currentContent = null;
                    }
                };
                _taskContentList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= 0 && index < _contentAsset.Content.Count)
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            GUI.backgroundColor = _contentAsset.Content[index].IsComplete ? Color.green : Color.white;
                            GUIStyle gUIStyle = (index % 2 != 0) ? "CN EntryBackEven" : "Box";
                            gUIStyle = (!isActive && !isFocused) ? gUIStyle : "RL Element";
                            rect.x += 2;
                            rect.width -= 6;
                            gUIStyle.Draw(rect, false, isActive, isActive, isFocused);
                            GUI.backgroundColor = Color.white;
                        }
                    }
                };
                _taskContentList.onReorderCallback = (ReorderableList list) =>
                {
                    HasChanged(_contentAsset);
                };
            }
        }
        /// <summary>
        /// 最小化窗口
        /// </summary>
        private void MinimizeWindow()
        {
            _recordedPosition = position;
            minSize = new Vector2(200, 100);
            maxSize = new Vector2(200, 100);
            _isMinimize = true;
        }
        /// <summary>
        /// 最大化窗口
        /// </summary>
        private void MaximizeWindow()
        {
            minSize = new Vector2(800, 600);
            maxSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            position = _recordedPosition;
            _isMinimize = false;
        }

        /// <summary>
        /// 新增任务点
        /// </summary>
        private void AddPoint(TaskContentBase content, Type type, Vector2 pos)
        {
            TaskPointAttribute attribute = type.GetCustomAttribute<TaskPointAttribute>();
            TaskPointBase taskPoint = CreateInstance(type) as TaskPointBase;
            taskPoint.Anchor = new Rect(pos.x, pos.y, 0, 0);
            taskPoint.GUID = _contentAsset.TaskPointIDName + _contentAsset.TaskPointIDSign.ToString();
            taskPoint.Name = (attribute != null ? attribute.GetLastName() : "New Task Point ") + _contentAsset.TaskPointIDSign.ToString();
            _contentAsset.TaskPointIDSign += 1;
            content.Points.Add(taskPoint);

            TaskContentAsset.GenerateSerializeSubObject(taskPoint, _contentAsset);
        }
        /// <summary>
        /// 克隆任务点
        /// </summary>
        private void ClonePoint(TaskContentBase content, TaskPointBase point, Vector2 pos)
        {
            TaskPointBase taskPoint = point.Clone();
            taskPoint.Anchor = new Rect(pos.x, pos.y, 0, 0);
            taskPoint.GUID = _contentAsset.TaskPointIDName + _contentAsset.TaskPointIDSign.ToString();
            _contentAsset.TaskPointIDSign += 1;
            content.Points.Add(taskPoint);

            TaskContentAsset.GenerateSerializeSubObject(taskPoint, _contentAsset);
        }
        /// <summary>
        /// 查找任务点
        /// </summary>
        private void FindPoint(TaskPointBase taskPoint)
        {
            Vector2 target = new Vector2(position.width * 0.5f - taskPoint.Anchor.width * 0.5f, position.height * 0.5f - taskPoint.Anchor.height * 0.5f);
            Vector2 detail = target - taskPoint.Anchor.position;
            for (int i = 0; i < _currentContent.Points.Count; i++)
            {
                _currentContent.Points[i].Anchor.position += detail;
            }
        }
        /// <summary>
        /// 复制任务点
        /// </summary>
        private void CopyTaskPoint(GenericMenu gm)
        {
            string assetPath = AssetDatabase.GetAssetPath(_contentAsset);
            int number = 0;
            for (int i = 0; i < _currentContent.Points.Count; i++)
            {
                TaskPointBase taskPoint = _currentContent.Points[i];
                if (taskPoint.IsSelected)
                {
                    number += 1;
                    gm.AddItem(new GUIContent($"{GetWord("Copy")} {taskPoint.Name}"), false, () =>
                    {
                        GUIUtility.systemCopyBuffer = $"TaskPoint|{assetPath}|{_currentContent.GUID}|{taskPoint.GUID}";
                    });
                }
            }

            if (number == 0)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Copy")));
            }
        }
        /// <summary>
        /// 粘贴任务点
        /// </summary>
        private void PasteTaskPoint(GenericMenu gm, Vector2 pos)
        {
            string assetPath = AssetDatabase.GetAssetPath(_contentAsset);
            
            TaskContentBase content = null;
            TaskPointBase point = null;
            string[] buffers = GUIUtility.systemCopyBuffer.Split('|');
            if (buffers.Length == 4 && buffers[0] == "TaskPoint")
            {
                if (buffers[1] == assetPath)
                {
                    content = _contentAsset.Content.Find((s) => { return s.GUID == buffers[2]; });
                    if (content)
                    {
                        point = content.Points.Find((s) => { return s.GUID == buffers[3]; });
                    }
                }
                else
                {
                    TaskContentAsset taskContentAsset = AssetDatabase.LoadAssetAtPath<TaskContentAsset>(buffers[1]);
                    if (taskContentAsset)
                    {
                        content = taskContentAsset.Content.Find((s) => { return s.GUID == buffers[2]; });
                        if (content)
                        {
                            point = content.Points.Find((s) => { return s.GUID == buffers[3]; });
                        }
                    }
                }
            }

            if (point == null)
            {
                gm.AddDisabledItem(new GUIContent(GetWord("Paste")));
            }
            else
            {
                gm.AddItem(new GUIContent($"{GetWord("Paste")} {point.Name}"), false, () =>
                {
                    ClonePoint(_currentContent, point, pos);
                });
            }
        }
        /// <summary>
        /// 新建任务点脚本
        /// </summary>
        private void NewTaskPointScript()
        {
            EditorGlobalTools.CreateScriptFormTemplate(EditorPrefsTable.Script_TaskPoint_Folder, "TaskPoint", "TaskPointTemplate", NewTaskPointScriptHandler);
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
            for (int i = 0; i < _contentAsset.Content.Count; i++)
            {
                TaskContentBase taskContent = _contentAsset.Content[i];
                taskContent.ReSet();
                for (int j = 0; j < taskContent.Points.Count; j++)
                {
                    taskContent.Points[j].ReSet();
                }
            }
        }
    }
}