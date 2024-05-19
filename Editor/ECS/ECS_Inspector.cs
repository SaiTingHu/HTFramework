using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// ECS检视器
    /// </summary>
    internal sealed class ECS_Inspector : HTFEditorWindow
    {
        private List<Type> _components = new List<Type>();
        private Dictionary<Type, ECS_ComponentInfo> _componentInfos = new Dictionary<Type, ECS_ComponentInfo>();
        private List<Type> _systems = new List<Type>();
        private Dictionary<Type, ECS_SystemInfo> _systemInfos = new Dictionary<Type, ECS_SystemInfo>();
        private ECS_Entity _entity;
        private bool _isDirty = false;
        private HashSet<Type> _temporary = new HashSet<Type>();
        private Vector2 _scrollComponent;
        private Vector2 _scrollSystem;

        public ECS_Entity Entity
        {
            get
            {
                return _entity;
            }
            set
            {
                if (_entity != value)
                {
                    _entity = value;
                    SetDirty();
                }
            }
        }
        protected override string HelpUrl => "https://wanderer.blog.csdn.net/article/details/106619485";

        protected override void OnEnable()
        {
            base.OnEnable();

            _components.Clear();
            _componentInfos.Clear();
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies((type) =>
            {
                return type.IsSubclassOf(typeof(ECS_Component)) && !type.IsAbstract;
            }, false);
            for (int i = 0; i < types.Count; i++)
            {
                _components.Add(types[i]);
                _componentInfos.Add(types[i], new ECS_ComponentInfo(types[i]));
            }

            _systems.Clear();
            _systemInfos.Clear();
            types = ReflectionToolkit.GetTypesInRunTimeAssemblies((type) =>
            {
                return type.IsSubclassOf(typeof(ECS_System)) && !type.IsAbstract;
            }, false);
            for (int i = 0; i < types.Count; i++)
            {
                _systems.Add(types[i]);
                _systemInfos.Add(types[i], new ECS_SystemInfo(types[i], _componentInfos));
            }

            Entity = null;
        }
        protected override void OnTitleGUI()
        {
            base.OnTitleGUI();

            GUILayout.FlexibleSpace();
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            EntityGUI();
            CorrectionDirty();
            ComponentGUI();
            SystemGUI();
        }
        private void EntityGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Entity:", GUILayout.Width(60));
            Entity = EditorGUILayout.ObjectField(Entity, typeof(ECS_Entity), true) as ECS_Entity;
            GUILayout.EndHorizontal();
        }
        private void ComponentGUI()
        {
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Components", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            _scrollComponent = GUILayout.BeginScrollView(_scrollComponent);

            for (int i = 0; i < _components.Count; i++)
            {
                ECS_ComponentInfo info = _componentInfos[_components[i]];

                GUILayout.BeginHorizontal();
                GUI.color = info.IsExist ? Color.white : Color.gray;
                GUILayout.Space(20);
                GUILayout.Label($"{i + 1}.", GUILayout.Width(30));
                GUILayout.Toggle(info.IsExist, "", GUILayout.Width(20));
                GUILayout.Label(info.Name);
                GUI.color = Color.white;
                GUILayout.FlexibleSpace();
                GUI.enabled = Entity && !info.IsExist;
                if (GUILayout.Button("Append", EditorStyles.miniButtonLeft))
                {
                    Entity.gameObject.AddComponent(_components[i]);
                    SetDirty();
                    HasChanged(Entity);
                }
                GUI.enabled = Entity && info.IsExist;
                if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
                {
                    Component component = Entity.GetComponent(_components[i]);
                    if (component)
                    {
                        DestroyImmediate(component);
                        SetDirty();
                        HasChanged(Entity);
                    }
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
        private void SystemGUI()
        {
            GUILayout.BeginVertical(EditorGlobalTools.Styles.Box);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Systems", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            _scrollSystem = GUILayout.BeginScrollView(_scrollSystem);

            for (int i = 0; i < _systems.Count; i++)
            {
                ECS_SystemInfo info = _systemInfos[_systems[i]];

                GUILayout.BeginHorizontal();
                GUI.color = info.IsStar ? Color.white : Color.gray;
                GUILayout.Space(20);
                GUILayout.Label($"{i + 1}.", GUILayout.Width(30));
                GUILayout.Toggle(info.IsStar, "", GUILayout.Width(20));
                GUILayout.Label(info.Name);
                GUI.color = Color.white;
                GUILayout.FlexibleSpace();
                GUI.enabled = Entity && !info.IsStar;
                if (GUILayout.Button("Apply To Star", EditorStyles.miniButton))
                {
                    for (int j = 0; j < info.StarComponents.Count; j++)
                    {
                        if (!Entity.GetComponent(info.StarComponents[j].ComponentType))
                        {
                            Entity.gameObject.AddComponent(info.StarComponents[j].ComponentType);
                        }
                    }
                    SetDirty();
                    HasChanged(Entity);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        private new void SetDirty()
        {
            _isDirty = true;
        }
        private void CorrectionDirty()
        {
            if (_isDirty)
            {
                _isDirty = false;
                
                if (Entity != null)
                {
                    ECS_Component[] components = Entity.GetComponents<ECS_Component>();
                    _temporary.Clear();
                    for (int i = 0; i < components.Length; i++)
                    {
                        _temporary.Add(components[i].GetType());
                    }

                    foreach (var item in _componentInfos)
                    {
                        item.Value.IsExist = _temporary.Contains(item.Key);
                    }

                    foreach (var item in _systemInfos)
                    {
                        item.Value.IsStar = ContainsAll(_temporary, item.Value.StarComponents);
                    }

                    _components.Sort((x, y) =>
                    {
                        if (_componentInfos[x].IsExist)
                        {
                            return -1;
                        }
                        else if (_componentInfos[y].IsExist)
                        {
                            return 1;
                        }
                        return 0;
                    });

                    _systems.Sort((x, y) =>
                    {
                        if (_systemInfos[x].IsStar)
                        {
                            return -1;
                        }
                        else if (_systemInfos[y].IsStar)
                        {
                            return 1;
                        }
                        return 0;
                    });
                }
                else
                {
                    foreach (var item in _componentInfos)
                    {
                        item.Value.IsExist = false;
                    }

                    foreach (var item in _systemInfos)
                    {
                        item.Value.IsStar = false;
                    }
                }
            }
        }
        private bool ContainsAll(HashSet<Type> source, List<ECS_ComponentInfo> target)
        {
            for (int i = 0; i < target.Count; i++)
            {
                if (!source.Contains(target[i].ComponentType))
                {
                    return false;
                }
            }
            return true;
        }

        private class ECS_ComponentInfo
        {
            public string Name;
            public Type ComponentType;
            public bool IsExist;

            public ECS_ComponentInfo(Type type)
            {
                ComponentNameAttribute cna = type.GetCustomAttribute<ComponentNameAttribute>();
                string name = cna != null ? cna.Name : "未命名";
                Name = $"{name} ({type.FullName})";
                ComponentType = type;
                IsExist = false;
            }
        }
        private class ECS_SystemInfo
        {
            public string Name;
            public Type SystemType;
            public List<ECS_ComponentInfo> StarComponents;
            public bool IsStar;

            public ECS_SystemInfo(Type type, Dictionary<Type, ECS_ComponentInfo> components)
            {
                SystemNameAttribute mna = type.GetCustomAttribute<SystemNameAttribute>();
                string name = mna != null ? mna.Name : "未命名";
                Name = $"{name} ({type.FullName})";
                SystemType = type;
                StarComponents = new List<ECS_ComponentInfo>();
                StarComponentAttribute sca = type.GetCustomAttribute<StarComponentAttribute>();
                if (sca != null && sca.StarComponents != null)
                {
                    for (int i = 0; i < sca.StarComponents.Length; i++)
                    {
                        StarComponents.Add(components[sca.StarComponents[i]]);
                    }
                }
                IsStar = false;
            }
        }
    }
}