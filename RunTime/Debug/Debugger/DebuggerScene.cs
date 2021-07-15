using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试器场景
    /// </summary>
    internal sealed class DebuggerScene
    {
        private Dictionary<Type, Type> _debuggerComponents = new Dictionary<Type, Type>();
        private List<Type> _componentTypes = new List<Type>();
        private List<DebuggerGameObject> _gameObjects = new List<DebuggerGameObject>();
        private DebuggerGameObject _currentGameObject;
        private Component _currentComponent;
        private List<GameObject> _gameObjectCaches = new List<GameObject>();

        /// <summary>
        /// 所有游戏对象根物体
        /// </summary>
        public List<DebuggerGameObject> GameObjectRoots { get; private set; } = new List<DebuggerGameObject>();
        /// <summary>
        /// 当前选中的游戏对象
        /// </summary>
        public DebuggerGameObject CurrentGameObject
        {
            get
            {
                return _currentGameObject;
            }
            set
            {
                _currentGameObject = value;
                CollectDebuggerComponents();
            }
        }
        /// <summary>
        /// 当前选中的游戏对象的所有组件
        /// </summary>
        public List<Component> Components { get; private set; } = new List<Component>();
        /// <summary>
        /// 当前选中的组件
        /// </summary>
        public Component CurrentComponent
        {
            get
            {
                return _currentComponent;
            }
            set
            {
                if (_currentComponent != value)
                {
                    _currentComponent = value;
                    if (CurrentDebuggerComponent != null)
                    {
                        Main.m_ReferencePool.Despawn(CurrentDebuggerComponent);
                        CurrentDebuggerComponent = null;
                    }
                    if (_currentComponent != null)
                    {
                        Type type = _currentComponent.GetType();
                        if (_debuggerComponents.ContainsKey(type))
                        {
                            CurrentDebuggerComponent = Main.m_ReferencePool.Spawn(_debuggerComponents[type]) as DebuggerComponentBase;
                            CurrentDebuggerComponent.Target = _currentComponent;
                            CurrentDebuggerComponent.OnEnable();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 当前选中的调试器组件
        /// </summary>
        public DebuggerComponentBase CurrentDebuggerComponent { get; private set; }
        /// <summary>
        /// 当前是否准备添加组件
        /// </summary>
        public bool IsReadyAddComponent { get; set; } = false;
        /// <summary>
        /// 当前是否显示筛选的游戏物体
        /// </summary>
        public bool IsShowGameObjectFiltrate { get; set; } = false;
        /// <summary>
        /// 游戏对象名称筛选
        /// </summary>
        public string GameObjectFiltrate { get; set; } = "";
        /// <summary>
        /// 组件类型筛选
        /// </summary>
        public string ComponentFiltrate { get; set; } = "";

        public DebuggerScene()
        {
            Type baseType = typeof(DebuggerComponentBase);
            List<Type> types = ReflectionToolkit.GetTypesInRunTimeAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(baseType))
                {
                    CustomDebuggerAttribute attr = types[i].GetCustomAttribute<CustomDebuggerAttribute>();
                    if (attr != null)
                    {
                        _debuggerComponents.Add(attr.InspectedType, types[i]);
                    }
                }
            }

            baseType = typeof(Component);
            for (int i = 0; i < types.Count; i++)
            {
                if (types[i].IsSubclassOf(baseType))
                {
                    _componentTypes.Add(types[i]);
                }
            }
        }

        /// <summary>
        /// 刷新场景
        /// </summary>
        public void Refresh()
        {
            Main.Current.StartCoroutine(CollectDebuggerGameObjects());
        }
        /// <summary>
        /// 游戏对象筛选
        /// </summary>
        /// <param name="gameObjects">游戏对象列表</param>
        public void ExecuteGameObjectFiltrate(List<DebuggerGameObject> gameObjects)
        {
            if (gameObjects == null) gameObjects = new List<DebuggerGameObject>();
            else gameObjects.Clear();

            if (GameObjectFiltrate == "")
            {
                IsShowGameObjectFiltrate = false;
            }
            else
            {
                IsShowGameObjectFiltrate = true;
                string filtrate = GameObjectFiltrate.ToLower();
                for (int i = 0; i < _gameObjects.Count; i++)
                {
                    if (_gameObjects[i].Name.ToLower().Contains(filtrate))
                    {
                        gameObjects.Add(_gameObjects[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 组件类型筛选
        /// </summary>
        /// <param name="types">组件类型列表</param>
        public void ExecuteComponentFiltrate(List<Type> types)
        {
            if (types == null) types = new List<Type>();
            else types.Clear();

            string filtrate = ComponentFiltrate.ToLower();
            for (int i = 0; i < _componentTypes.Count; i++)
            {
                if (_componentTypes[i].Name.ToLower().Contains(filtrate))
                {
                    types.Add(_componentTypes[i]);
                }
            }
        }

        /// <summary>
        /// 重新收集调试器游戏对象
        /// </summary>
        private IEnumerator CollectDebuggerGameObjects()
        {
            yield return YieldInstructioner.GetWaitForEndOfFrame();

            Main.m_ReferencePool.Despawns(_gameObjects);
            GameObjectRoots.Clear();

            _gameObjectCaches.Clear();
            GlobalTools.GetRootGameObjectsInAllScene(_gameObjectCaches);
            for (int i = 0; i < _gameObjectCaches.Count; i++)
            {
                CollectDebuggerGameObject(_gameObjectCaches[i].transform, null);
            }
            CurrentGameObject = null;
            IsShowGameObjectFiltrate = false;
        }
        /// <summary>
        /// 收集调试器游戏对象及子物体
        /// </summary>
        private void CollectDebuggerGameObject(Transform transform, DebuggerGameObject parent)
        {
            DebuggerGameObject gameObject = Main.m_ReferencePool.Spawn<DebuggerGameObject>();
            gameObject.Target = transform.gameObject;
            gameObject.Name = transform.gameObject.name;
            gameObject.Layer = LayerMask.LayerToName(transform.gameObject.layer);
            gameObject.IsMain = gameObject.Target.GetComponent<Main>();
            gameObject.Parent = parent;
            _gameObjects.Add(gameObject);

            if (gameObject.Parent != null)
            {
                gameObject.Parent.Childrens.Add(gameObject);
            }
            else
            {
                GameObjectRoots.Add(gameObject);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                CollectDebuggerGameObject(transform.GetChild(i), gameObject);
            }
        }
        /// <summary>
        /// 收集调试器组件
        /// </summary>
        private void CollectDebuggerComponents()
        {
            Components.Clear();
            if (CurrentGameObject != null && CurrentGameObject.Target)
            {
                CurrentGameObject.Target.GetComponents(Components);
            }
            CurrentComponent = null;
            IsReadyAddComponent = false;
        }
    }
}