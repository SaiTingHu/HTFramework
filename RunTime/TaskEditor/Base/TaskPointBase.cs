using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 任务点基类
    /// </summary>
    public abstract class TaskPointBase : ScriptableObject
    {
        /// <summary>
        /// 任务点名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 任务点详细介绍
        /// </summary>
        public string Details;
        /// <summary>
        /// 任务点面板的锚点
        /// </summary>
        public Rect Anchor;

        public TaskPointBase()
        {
            Name = "New Task Point";
            Details = "New Task Point";
            Anchor = Rect.zero;
        }

#if UNITY_EDITOR
        private bool _isDraging = false;
        private bool _isSelected = false;
        private bool _isEditName = false;
        private bool _isEditDetails = false;
        private int _height = 0;

        public Vector2 LeftPosition
        {
            get
            {
                return new Vector2(Anchor.x + 15, Anchor.y + 30);
            }
        }

        public Vector2 RightPosition
        {
            get
            {
                return new Vector2(Anchor.x + Anchor.width - 15, Anchor.y + 30);
            }
        }

        public Vector2 LeftTangent
        {
            get
            {
                return new Vector2(Anchor.x - 200, Anchor.y + 30);
            }
        }

        public Vector2 RightTangent
        {
            get
            {
                return new Vector2(Anchor.x + Anchor.width + 200, Anchor.y + 30);
            }
        }

        internal void OnEditorGUI(TaskContentBase taskContent)
        {
            GUI.backgroundColor = _isSelected ? Color.yellow : Color.white;

            GUILayout.BeginArea(Anchor, Name, "Window");

            GUI.backgroundColor = Color.white;

            _height = 25;

            _height += OnDependGUI(taskContent);

            _height += OnPropertyGUI();
            
            Anchor.height = _height;

            GUILayout.EndArea();
        }

        internal void OnDrag(Vector2 delta)
        {
            Anchor.position += delta;
        }

        internal void OnPointEventHandle(Event e, TaskContentBase content)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (Anchor.Contains(e.mousePosition))
                        {
                            _isDraging = true;
                            _isSelected = true;
                            GUI.changed = true;
                            GUI.FocusControl(null);
                        }
                        else
                        {
                            _isSelected = false;
                            _isEditName = false;
                            _isEditDetails = false;
                            GUI.changed = true;
                        }
                    }
                    else if (e.button == 1)
                    {
                        if (_isSelected && Anchor.Contains(e.mousePosition))
                        {
                            RightClickMenu(content);
                            e.Use();
                        }
                    }
                    break;
                case EventType.MouseUp:
                    _isDraging = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && _isDraging)
                    {
                        OnDrag(e.delta);
                        e.Use();
                        GUI.changed = true;
                    }
                    break;
            }
        }

        internal int OnDependGUI(TaskContentBase taskContent)
        {
            int height = 0;

            GUILayout.BeginHorizontal();
            GUIContent gUIContent = EditorGUIUtility.IconContent("DotFrameDotted");
            gUIContent.tooltip = "Dependent task point";
            if (GUILayout.Button(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                int index = taskContent.Points.IndexOf(this);
                for (int i = 0; i < taskContent.Points.Count; i++)
                {
                    if (i != index)
                    {
                        int m = i;
                        bool isExist = taskContent.IsExistDepend(index, m);
                        gm.AddItem(new GUIContent(taskContent.Points[m].Name), isExist, () =>
                        {
                            if (isExist)
                            {
                                taskContent.DisconnectDepend(index, m);
                            }
                            else
                            {
                                taskContent.ConnectDepend(index, m);
                            }
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.FlexibleSpace();
            gUIContent.tooltip = "Be dependent task point";
            if (GUILayout.Button(gUIContent, "InvisibleButton", GUILayout.Width(20), GUILayout.Height(20)))
            {
                GenericMenu gm = new GenericMenu();
                int index = taskContent.Points.IndexOf(this);
                for (int i = 0; i < taskContent.Points.Count; i++)
                {
                    if (i != index)
                    {
                        int m = i;
                        bool isExist = taskContent.IsExistDepend(m, index);
                        gm.AddItem(new GUIContent(taskContent.Points[m].Name), isExist, () =>
                        {
                            if (isExist)
                            {
                                taskContent.DisconnectDepend(m, index);
                            }
                            else
                            {
                                taskContent.ConnectDepend(m, index);
                            }
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            height += 20;

            return height;
        }

        public virtual int OnPropertyGUI()
        {
            int height = 0;

            #region Name
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(40));
            if (_isEditName)
            {
                Name = EditorGUILayout.TextField(Name, GUILayout.Width(120));
            }
            else
            {
                GUILayout.Label(Name, GUILayout.Width(120));
            }
            GUILayout.FlexibleSpace();
            if (_isSelected)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("editicon.sml"), "IconButton"))
                {
                    _isEditName = !_isEditName;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();
            #endregion

            height += 20;

            #region Details
            GUILayout.BeginHorizontal();
            GUILayout.Label("Details:", GUILayout.Width(40));
            if (_isEditDetails)
            {
                Details = EditorGUILayout.TextField(Details, GUILayout.Width(120));
            }
            else
            {
                GUILayout.Label(Details, GUILayout.Width(120));
            }
            GUILayout.FlexibleSpace();
            if (_isSelected)
            {
                if (GUILayout.Button(EditorGUIUtility.IconContent("editicon.sml"), "IconButton"))
                {
                    _isEditDetails = !_isEditDetails;
                    GUI.FocusControl(null);
                }
            }
            GUILayout.EndHorizontal();
            #endregion

            height += 20;

            return height;
        }
        
        private void RightClickMenu(TaskContentBase content)
        {
            GenericMenu gm = new GenericMenu();
            gm.AddItem(new GUIContent("Delete " + Name), false, () =>
            {
                if (EditorUtility.DisplayDialog("Delete Task Point", "Are you sure you want to delete task point [" + Name + "]?", "Yes", "No"))
                {
                    DeletePoint(content);
                }
            });
            gm.AddItem(new GUIContent("Edit Point Script"), false, () =>
            {
                MonoScript monoScript = MonoScript.FromScriptableObject(this);
                AssetDatabase.OpenAsset(monoScript);
            });
            gm.ShowAsContext();
        }

        private void DeletePoint(TaskContentBase content)
        {
            int index = content.Points.IndexOf(this);
            for (int i = 0; i < content.Depends.Count; i++)
            {
                if (content.Depends[i].OriginalPoint == index || content.Depends[i].DependPoint == index)
                {
                    content.Depends.RemoveAt(i);
                    i -= 1;
                }
                else
                {
                    if (content.Depends[i].OriginalPoint > index)
                    {
                        content.Depends[i].OriginalPoint -= 1;
                    }
                    if (content.Depends[i].DependPoint > index)
                    {
                        content.Depends[i].DependPoint -= 1;
                    }
                }
            }

            TaskContentBase.DestroySerializeSubObject(this, content);
            content.Points.Remove(this);
        }
#endif
    }
}