using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

namespace HT.Framework
{
    /// <summary>
    /// 调试器
    /// </summary>
    public class Debugger
    {
        #region Public Property
        /// <summary>
        /// 当前模块
        /// </summary>
        public DebuggerModule Module
        {
            get
            {
                return _module;
            }
            protected set
            {
                if (_module != value)
                {
                    _module = value;
                    switch (_module)
                    {
                        case DebuggerModule.Scene:
                            _debuggerScene.Refresh();
                            break;
                    }
                }
            }
        }
        #endregion

        #region Private Field
        private GUISkin _skin;
        private Rect _dragWindowRect;
        private Rect _minWindowRect;
        private Rect _maxWindowRect;
        private bool _isExpand = false;
        private DebuggerModule _module = DebuggerModule.Console;
        //FPS
        private int _fps = 0;
        private int _minfps = 60;
        private int _maxfps = 0;
        private Color _fpsColor = Color.white;
        private float _lastShowFPSTime = 0f;
        //Console
        private List<DebuggerConsoleLog> _consoleLogs = new List<DebuggerConsoleLog>();
        private int _currentLogIndex = -1;
        private int _infoLogCount = 0;
        private int _warningLogCount = 0;
        private int _errorLogCount = 0;
        private int _fatalLogCount = 0;
        private bool _showInfoLog = true;
        private bool _showWarningLog = true;
        private bool _showErrorLog = true;
        private bool _showFatalLog = true;
        private Vector2 _scrollLogView = Vector2.zero;
        private Vector2 _scrollCurrentLogView = Vector2.zero;
        //Scene
        private DebuggerScene _debuggerScene;
        private Texture _expandTexture;
        private Texture _retractTexture;
        private List<DebuggerGameObject> _debuggerGameObjects = new List<DebuggerGameObject>();
        private List<Type> _addComponents = new List<Type>();
        private Vector2 _scrollSceneView = Vector2.zero;
        private Vector2 _scrollInspectorView = Vector2.zero;
        //Memory
        private long _minTotalReservedMemory = 10000;
        private long _maxTotalReservedMemory = 0;
        private long _minTotalAllocatedMemory = 10000;
        private long _maxTotalAllocatedMemory = 0;
        private long _minTotalUnusedReservedMemory = 10000;
        private long _maxTotalUnusedReservedMemory = 0;
        private long _minMonoHeapSize = 10000;
        private long _maxMonoHeapSize = 0;
        private long _minMonoUsedSize = 10000;
        private long _maxMonoUsedSize = 0;
        //DrawCall
        private Vector2 _scrollDrawCallView = Vector2.zero;
        //System
        private Vector2 _scrollSystemView = Vector2.zero;
        #endregion

        #region Lifecycle Function
        public virtual void OnInit(GUISkin skin)
        {
            Application.logMessageReceived += OnLogMessageReceived;

            _skin = skin;
            _dragWindowRect = new Rect(0, 0, 10000, 20);
            _minWindowRect = new Rect(0, 0, 100, 60);
            _maxWindowRect = new Rect(0, 0, 700, 400);
            
            _debuggerScene = new DebuggerScene();
            _expandTexture = Resources.Load<Texture>("Texture/Debug/Expand");
            _retractTexture = Resources.Load<Texture>("Texture/Debug/Retract");
        }
        public virtual void OnDestory()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
        public void OnDebuggerGUI()
        {
            GUI.skin = _skin;

            if (_isExpand)
            {
                _maxWindowRect = GUI.Window(0, _maxWindowRect, OnExpandGUIWindow, "DEBUGGER");
                if (_maxWindowRect.x < 0)
                {
                    _maxWindowRect.x = 0;
                }
                if (_maxWindowRect.y < 0)
                {
                    _maxWindowRect.y = 0;
                }
            }
            else
            {
                _minWindowRect = GUI.Window(0, _minWindowRect, OnRetractGUIWindow, "DEBUGGER");
                if (_minWindowRect.x < 0)
                {
                    _minWindowRect.x = 0;
                }
                if (_minWindowRect.y < 0)
                {
                    _minWindowRect.y = 0;
                }
            }

            FPSUpdate();
        }
        protected virtual void OnExpandGUIWindow(int windowId)
        {
            GUI.DragWindow(_dragWindowRect);

            #region Title
            GUILayout.BeginHorizontal();
            GUI.contentColor = _fpsColor;
            if (GUILayout.Button("FPS: " + _fps.ToString(), GUILayout.Height(40)))
            {
                _isExpand = false;
            }
            GUI.contentColor = (Module == DebuggerModule.Console ? Color.white : Color.gray);
            if (GUILayout.Button("Console", GUILayout.Height(40)))
            {
                Module = DebuggerModule.Console;
            }
            GUI.contentColor = (Module == DebuggerModule.Scene ? Color.white : Color.gray);
            if (GUILayout.Button("Scene", GUILayout.Height(40)))
            {
                Module = DebuggerModule.Scene;
            }
            GUI.contentColor = (Module == DebuggerModule.Memory ? Color.white : Color.gray);
            if (GUILayout.Button("Memory", GUILayout.Height(40)))
            {
                Module = DebuggerModule.Memory;
            }
            GUI.contentColor = (Module == DebuggerModule.DrawCall ? Color.white : Color.gray);
            if (GUILayout.Button("DrawCall", GUILayout.Height(40)))
            {
                Module = DebuggerModule.DrawCall;
            }
            GUI.contentColor = (Module == DebuggerModule.System ? Color.white : Color.gray);
            if (GUILayout.Button("System", GUILayout.Height(40)))
            {
                Module = DebuggerModule.System;
            }
            GUI.contentColor = (Module == DebuggerModule.Screen ? Color.white : Color.gray);
            if (GUILayout.Button("Screen", GUILayout.Height(40)))
            {
                Module = DebuggerModule.Screen;
            }
            GUI.contentColor = (Module == DebuggerModule.Quality ? Color.white : Color.gray);
            if (GUILayout.Button("Quality", GUILayout.Height(40)))
            {
                Module = DebuggerModule.Quality;
            }
            GUI.contentColor = (Module == DebuggerModule.Time ? Color.white : Color.gray);
            if (GUILayout.Button("Time", GUILayout.Height(40)))
            {
                Module = DebuggerModule.Time;
            }
            GUI.contentColor = (Module == DebuggerModule.Environment ? Color.white : Color.gray);
            if (GUILayout.Button("Environment", GUILayout.Height(40)))
            {
                Module = DebuggerModule.Environment;
            }
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
            #endregion

            switch (Module)
            {
                case DebuggerModule.Console:
                    #region Console
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    if (GUILayout.Button("Clear", GUILayout.Width(60), GUILayout.Height(20)))
                    {
                        Main.m_ReferencePool.Despawns(_consoleLogs);
                        _fatalLogCount = 0;
                        _warningLogCount = 0;
                        _errorLogCount = 0;
                        _infoLogCount = 0;
                        _currentLogIndex = -1;
                        _fpsColor = Color.white;
                    }
                    GUI.contentColor = (_showInfoLog ? Color.white : Color.gray);
                    _showInfoLog = GUILayout.Toggle(_showInfoLog, "Info [" + _infoLogCount.ToString() + "]", GUILayout.Height(20));
                    GUI.contentColor = (_showWarningLog ? Color.white : Color.gray);
                    _showWarningLog = GUILayout.Toggle(_showWarningLog, "Warning [" + _warningLogCount.ToString() + "]", GUILayout.Height(20));
                    GUI.contentColor = (_showErrorLog ? Color.white : Color.gray);
                    _showErrorLog = GUILayout.Toggle(_showErrorLog, "Error [" + _errorLogCount.ToString() + "]", GUILayout.Height(20));
                    GUI.contentColor = (_showFatalLog ? Color.white : Color.gray);
                    _showFatalLog = GUILayout.Toggle(_showFatalLog, "Fatal [" + _fatalLogCount.ToString() + "]", GUILayout.Height(20));
                    GUI.contentColor = Color.white;
                    GUILayout.EndHorizontal();

                    _scrollLogView = GUILayout.BeginScrollView(_scrollLogView, "Box", GUILayout.Height(200));
                    for (int i = 0; i < _consoleLogs.Count; i++)
                    {
                        bool show = false;
                        Color color = Color.white;
                        switch (_consoleLogs[i].Type)
                        {
                            case "Fatal":
                                show = _showFatalLog;
                                color = Color.magenta;
                                break;
                            case "Error":
                                show = _showErrorLog;
                                color = Color.red;
                                break;
                            case "Info":
                                show = _showInfoLog;
                                color = Color.white;
                                break;
                            case "Warning":
                                show = _showWarningLog;
                                color = Color.yellow;
                                break;
                            default:
                                break;
                        }

                        if (show)
                        {
                            GUILayout.BeginHorizontal();
                            GUI.contentColor = color;
                            if (GUILayout.Toggle(_currentLogIndex == i, _consoleLogs[i].Name))
                            {
                                _currentLogIndex = i;
                            }
                            GUILayout.FlexibleSpace();
                            GUI.contentColor = Color.white;
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndScrollView();

                    _scrollCurrentLogView = GUILayout.BeginScrollView(_scrollCurrentLogView, "Box");
                    if (_currentLogIndex != -1)
                    {
                        GUILayout.Label(_consoleLogs[_currentLogIndex].Message + "\r\n\r\n" + _consoleLogs[_currentLogIndex].StackTrace);
                    }
                    GUILayout.EndScrollView();
                    #endregion
                    break;
                case DebuggerModule.Scene:
                    #region Scene
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    if (GUILayout.Button("Refresh", GUILayout.Width(60), GUILayout.Height(20)))
                    {
                        _debuggerScene.Refresh();
                        return;
                    }
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();

                    {
                        GUILayout.BeginVertical("Box", GUILayout.Width(335));
                        GUI.contentColor = Color.yellow;
                        GUILayout.Label("Hierarchy", GUILayout.Height(20));
                        GUI.contentColor = Color.white;

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Search:", GUILayout.Width(60));
                        _debuggerScene.GameObjectFiltrate = GUILayout.TextField(_debuggerScene.GameObjectFiltrate);
                        if (GUILayout.Button("Search", GUILayout.Width(60)))
                        {
                            _debuggerScene.ExecuteGameObjectFiltrate(_debuggerGameObjects);
                        }
                        GUILayout.EndHorizontal();

                        _scrollSceneView = GUILayout.BeginScrollView(_scrollSceneView);
                        if (_debuggerScene.IsShowGameObjectFiltrate)
                        {
                            for (int i = 0; i < _debuggerGameObjects.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                GUI.contentColor = _debuggerGameObjects[i].Target ? (_debuggerGameObjects[i].Target.activeSelf ? Color.cyan : Color.gray) : Color.red;
                                bool value = _debuggerScene.CurrentGameObject == _debuggerGameObjects[i];
                                if (GUILayout.Toggle(value, _debuggerGameObjects[i].Name) != value)
                                {
                                    if (_debuggerScene.CurrentGameObject != _debuggerGameObjects[i])
                                    {
                                        _debuggerScene.CurrentGameObject = _debuggerGameObjects[i];
                                    }
                                    else
                                    {
                                        _debuggerScene.CurrentGameObject = null;
                                    }
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();

                                if (_debuggerScene.CurrentGameObject == _debuggerGameObjects[i] && _debuggerGameObjects[i].Target)
                                {
                                    GUILayout.BeginVertical("Box");

                                    GUILayout.BeginHorizontal();
                                    bool active = GUILayout.Toggle(_debuggerGameObjects[i].Target.activeSelf, "Active");
                                    if (active != _debuggerGameObjects[i].Target.activeSelf)
                                    {
                                        _debuggerGameObjects[i].Target.SetActive(active);
                                    }
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Tag: " + _debuggerGameObjects[i].Target.tag);
                                    GUILayout.Label("Layer: " + _debuggerGameObjects[i].Layer);
                                    GUILayout.FlexibleSpace();
                                    if (GUILayout.Button("-", GUILayout.Width(20)))
                                    {
                                        Main.Kill(_debuggerGameObjects[i].Target);
                                        _debuggerScene.Refresh();
                                    }
                                    GUILayout.EndHorizontal();

                                    GUILayout.EndVertical();
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < _debuggerScene.GameObjectRoots.Count; i++)
                            {
                                OnDebuggerGameObjectGUI(_debuggerScene.GameObjectRoots[i], 0);
                            }
                        }
                        GUILayout.EndScrollView();

                        GUILayout.EndVertical();
                    }

                    {
                        GUILayout.BeginVertical("Box", GUILayout.Width(335));
                        GUI.contentColor = Color.yellow;
                        GUILayout.Label("Inspector", GUILayout.Height(20));
                        GUI.contentColor = Color.white;

                        if (_debuggerScene.CurrentGameObject != null && _debuggerScene.CurrentGameObject.Target)
                        {
                            GUILayout.BeginHorizontal();
                            _debuggerScene.IsReadyAddComponent = GUILayout.Toggle(_debuggerScene.IsReadyAddComponent, "Add Component", "Button");
                            if (_debuggerScene.CurrentComponent != null)
                            {
                                if (GUILayout.Button("Delete Component"))
                                {
                                    Main.Kill(_debuggerScene.CurrentComponent);
                                    _debuggerScene.CurrentGameObject = _debuggerScene.CurrentGameObject;
                                }
                            }
                            GUILayout.EndHorizontal();
                        }

                        _scrollInspectorView = GUILayout.BeginScrollView(_scrollInspectorView);
                        if (_debuggerScene.CurrentGameObject != null && _debuggerScene.CurrentGameObject.Target)
                        {
                            if (_debuggerScene.IsReadyAddComponent)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Search:", GUILayout.Width(60));
                                _debuggerScene.ComponentFiltrate = GUILayout.TextField(_debuggerScene.ComponentFiltrate);
                                if (GUILayout.Button("Search", GUILayout.Width(60)))
                                {
                                    _debuggerScene.ExecuteComponentFiltrate(_addComponents);
                                }
                                GUILayout.EndHorizontal();

                                for (int i = 0; i < _addComponents.Count; i++)
                                {
                                    if (GUILayout.Button(_addComponents[i].FullName, "Label"))
                                    {
                                        _debuggerScene.CurrentGameObject.Target.AddComponent(_addComponents[i]);
                                        _debuggerScene.CurrentGameObject = _debuggerScene.CurrentGameObject;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < _debuggerScene.Components.Count; i++)
                                {
                                    Component component = _debuggerScene.Components[i];
                                    if (component)
                                    {
                                        GUILayout.BeginHorizontal();
                                        GUI.contentColor = Color.cyan;
                                        bool value = _debuggerScene.CurrentComponent == component;
                                        if (GUILayout.Toggle(value, component.GetType().Name) != value)
                                        {
                                            if (_debuggerScene.CurrentComponent != component)
                                            {
                                                _debuggerScene.CurrentComponent = component;
                                            }
                                            else
                                            {
                                                _debuggerScene.CurrentComponent = null;
                                            }
                                        }
                                        GUILayout.FlexibleSpace();
                                        GUILayout.EndHorizontal();

                                        if (_debuggerScene.CurrentComponent == component)
                                        {
                                            GUI.contentColor = Color.white;
                                            GUILayout.BeginVertical("Box");

                                            if (_debuggerScene.CurrentDebuggerComponent != null)
                                            {
                                                _debuggerScene.CurrentDebuggerComponent.OnDebuggerGUI();
                                            }
                                            else
                                            {
                                                GUILayout.Label("No Debugger GUI!");
                                            }

                                            GUILayout.EndVertical();
                                        }
                                    }
                                }
                            }
                        }
                        GUILayout.EndScrollView();

                        GUILayout.EndVertical();
                    }

                    GUILayout.EndHorizontal();
                    #endregion
                    break;
                case DebuggerModule.Memory:
                    #region Memory
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label("Memory Information", GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(275));

                    long memory = Profiler.GetTotalReservedMemoryLong() / 1000000;
                    if (memory > _maxTotalReservedMemory) _maxTotalReservedMemory = memory;
                    if (memory < _minTotalReservedMemory) _minTotalReservedMemory = memory;
                    GUILayout.Label("Total Memory: " + memory.ToString() + "MB        [Min: " + _minTotalReservedMemory.ToString() + "  Max: " + _maxTotalReservedMemory.ToString() + "]");

                    memory = Profiler.GetTotalAllocatedMemoryLong() / 1000000;
                    if (memory > _maxTotalAllocatedMemory) _maxTotalAllocatedMemory = memory;
                    if (memory < _minTotalAllocatedMemory) _minTotalAllocatedMemory = memory;
                    GUILayout.Label("Used Memory: " + memory.ToString() + "MB        [Min: " + _minTotalAllocatedMemory.ToString() + "  Max: " + _maxTotalAllocatedMemory.ToString() + "]");

                    memory = Profiler.GetTotalUnusedReservedMemoryLong() / 1000000;
                    if (memory > _maxTotalUnusedReservedMemory) _maxTotalUnusedReservedMemory = memory;
                    if (memory < _minTotalUnusedReservedMemory) _minTotalUnusedReservedMemory = memory;
                    GUILayout.Label("Free Memory: " + memory.ToString() + "MB        [Min: " + _minTotalUnusedReservedMemory.ToString() + "  Max: " + _maxTotalUnusedReservedMemory.ToString() + "]");

                    memory = Profiler.GetMonoHeapSizeLong() / 1000000;
                    if (memory > _maxMonoHeapSize) _maxMonoHeapSize = memory;
                    if (memory < _minMonoHeapSize) _minMonoHeapSize = memory;
                    GUILayout.Label("Total Mono Memory: " + memory.ToString() + "MB        [Min: " + _minMonoHeapSize.ToString() + "  Max: " + _maxMonoHeapSize.ToString() + "]");

                    memory = Profiler.GetMonoUsedSizeLong() / 1000000;
                    if (memory > _maxMonoUsedSize) _maxMonoUsedSize = memory;
                    if (memory < _minMonoUsedSize) _minMonoUsedSize = memory;
                    GUILayout.Label("Used Mono Memory: " + memory.ToString() + "MB        [Min: " + _minMonoUsedSize.ToString() + "  Max: " + _maxMonoUsedSize.ToString() + "]");

                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Unload unused resources", GUILayout.Height(20)))
                    {
                        Resources.UnloadUnusedAssets();
                    }
                    if (GUILayout.Button("Garbage Collection", GUILayout.Height(20)))
                    {
                        GC.Collect();
                    }
                    GUILayout.EndHorizontal();
                    #endregion
                    break;
                case DebuggerModule.DrawCall:
                    #region DrawCall
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label("DrawCall Information", GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    _scrollDrawCallView = GUILayout.BeginScrollView(_scrollDrawCallView, "Box");
#if UNITY_EDITOR
                    GUILayout.Label("DrawCalls: " + UnityEditor.UnityStats.drawCalls);
                    GUILayout.Label("Batches: " + UnityEditor.UnityStats.batches);
                    GUILayout.Label("Static Batched DrawCalls: " + UnityEditor.UnityStats.staticBatchedDrawCalls);
                    GUILayout.Label("Static Batches: " + UnityEditor.UnityStats.staticBatches);
                    GUILayout.Label("Dynamic Batched DrawCalls: " + UnityEditor.UnityStats.dynamicBatchedDrawCalls);
                    GUILayout.Label("Dynamic Batches: " + UnityEditor.UnityStats.dynamicBatches);
                    if (UnityEditor.UnityStats.triangles > 10000)
                    {
                        GUILayout.Label("Triangles: " + UnityEditor.UnityStats.triangles / 10000 + "W");
                    }
                    else
                    {
                        GUILayout.Label("Triangles: " + UnityEditor.UnityStats.triangles);
                    }
                    if (UnityEditor.UnityStats.vertices > 10000)
                    {
                        GUILayout.Label("Vertices: " + UnityEditor.UnityStats.vertices / 10000 + "W");
                    }
                    else
                    {
                        GUILayout.Label("Vertices: " + UnityEditor.UnityStats.vertices);
                    }
#else
                    GUILayout.Label("DrawCall can only be displayed in Editor Mode!");
#endif
                    GUILayout.EndScrollView();
                    #endregion
                    break;
                case DebuggerModule.System:
                    #region System
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label("System Information", GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    _scrollSystemView = GUILayout.BeginScrollView(_scrollSystemView, "Box");
                    GUILayout.Label("Operating System: " + SystemInfo.operatingSystem);
                    GUILayout.Label("System Memory: " + SystemInfo.systemMemorySize.ToString() + "MB");
                    GUILayout.Label("Processor: " + SystemInfo.processorType);
                    GUILayout.Label("Number Of Processor: " + SystemInfo.processorCount.ToString());
                    GUILayout.Label("Graphics Device Name: " + SystemInfo.graphicsDeviceName);
                    GUILayout.Label("Graphics Device Type: " + SystemInfo.graphicsDeviceType.ToString());
                    GUILayout.Label("Graphics Memory: " + SystemInfo.graphicsMemorySize.ToString() + "MB");
                    GUILayout.Label("Graphics DeviceID: " + SystemInfo.graphicsDeviceID.ToString());
                    GUILayout.Label("Graphics Device Vendor: " + SystemInfo.graphicsDeviceVendor);
                    GUILayout.Label("Graphics Device Vendor ID: " + SystemInfo.graphicsDeviceVendorID.ToString());
                    GUILayout.Label("Device Model: " + SystemInfo.deviceModel);
                    GUILayout.Label("Device Name: " + SystemInfo.deviceName);
                    GUILayout.Label("Device Type: " + SystemInfo.deviceType.ToString());
                    GUILayout.Label("Device Unique Identifier: " + SystemInfo.deviceUniqueIdentifier);
                    GUILayout.EndScrollView();
                    #endregion
                    break;
                case DebuggerModule.Screen:
                    #region Screen
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label("Screen Information", GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(250), GUILayout.Height(275));
                    GUILayout.Label("DPI: " + Screen.dpi.ToString());
                    GUILayout.Label("Resolution: " + Screen.width.ToString() + " x " + Screen.height.ToString());
                    GUILayout.Label("Device Resolution: " + Screen.currentResolution.ToString());
                    GUILayout.Label("Device Sleep: " + (Screen.sleepTimeout == SleepTimeout.NeverSleep ? "Never Sleep" : "System Setting"));
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Device Sleep", GUILayout.Height(20)))
                    {
                        if (Screen.sleepTimeout == SleepTimeout.NeverSleep)
                        {
                            Screen.sleepTimeout = SleepTimeout.SystemSetting;
                        }
                        else
                        {
                            Screen.sleepTimeout = SleepTimeout.NeverSleep;
                        }
                    }
                    if (GUILayout.Button("Screen Capture", GUILayout.Height(20)))
                    {
                        Main.Current.StartCoroutine(ScreenShot());
                    }
                    if (GUILayout.Button("Full Screen", GUILayout.Height(20)))
                    {
                        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, !Screen.fullScreen);
                    }
                    GUILayout.EndHorizontal();
                    #endregion
                    break;
                case DebuggerModule.Quality:
                    #region Quality
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label("Quality Information", GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(275));

                    GUILayout.Label("Graphics Quality: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Lower", GUILayout.Height(20)))
                    {
                        QualitySettings.DecreaseLevel();
                    }
                    if (GUILayout.Button("Upgrade", GUILayout.Height(20)))
                    {
                        QualitySettings.IncreaseLevel();
                    }
                    GUILayout.EndHorizontal();
                    #endregion
                    break;
                case DebuggerModule.Time:
                    #region Time
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label("Time Information", GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(275));
                    GUILayout.Label("Current Time: " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    GUILayout.Label("Elapse Time: " + ((int)Time.realtimeSinceStartup).ToString());
                    GUILayout.Label("Time Scale: " + Time.timeScale.ToString());
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("0.1 Multiple", GUILayout.Height(20)))
                    {
                        Time.timeScale = 0.1f;
                    }
                    if (GUILayout.Button("0.2 Multiple", GUILayout.Height(20)))
                    {
                        Time.timeScale = 0.2f;
                    }
                    if (GUILayout.Button("0.5 Multiple", GUILayout.Height(20)))
                    {
                        Time.timeScale = 0.5f;
                    }
                    if (GUILayout.Button("1 Multiple", GUILayout.Height(20)))
                    {
                        Time.timeScale = 1;
                    }
                    if (GUILayout.Button("2 Multiple", GUILayout.Height(20)))
                    {
                        Time.timeScale = 2;
                    }
                    if (GUILayout.Button("5 Multiple", GUILayout.Height(20)))
                    {
                        Time.timeScale = 5;
                    }
                    if (GUILayout.Button("10 Multiple", GUILayout.Height(20)))
                    {
                        Time.timeScale = 10;
                    }
                    GUILayout.EndHorizontal();
                    #endregion
                    break;
                case DebuggerModule.Environment:
                    #region Environment
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label("Environment Information", GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(275));
                    GUILayout.Label("Product Name: " + Application.productName);
                    GUILayout.Label("Product Identifier: " + Application.identifier);
                    GUILayout.Label("Product Version: " + Application.version);
                    GUILayout.Label("Product DataPath: " + Application.dataPath);
                    GUILayout.Label("Company Name: " + Application.companyName);
                    GUILayout.Label("Unity Version: " + Application.unityVersion);
                    GUILayout.Label("Has Pro License: " + Application.HasProLicense());
                    string internetState = "NotReachable";
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                        internetState = "No Network";
                    else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                        internetState = "WiFi";
                    else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                        internetState = "Data Network";
                    GUILayout.Label("Internet State: " + internetState);
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Quit", GUILayout.Height(20)))
                    {
                        Application.Quit();
                    }
                    GUILayout.EndHorizontal();
                    #endregion
                    break;
                default:
                    break;
            }
        }
        protected virtual void OnRetractGUIWindow(int windowId)
        {
            GUI.DragWindow(_dragWindowRect);

            GUI.contentColor = _fpsColor;
            if (GUILayout.Button("FPS: " + _fps.ToString(), GUILayout.Width(80), GUILayout.Height(30)))
            {
                _isExpand = true;
            }
            GUI.contentColor = Color.white;
        }
        #endregion

        #region Additional Function
        /// <summary>
        /// 接收系统日志
        /// </summary>
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            DebuggerConsoleLog log = Main.m_ReferencePool.Spawn<DebuggerConsoleLog>();
            log.Time = DateTime.Now.ToString("HH:mm:ss");
            log.Message = condition;
            log.StackTrace = stackTrace;
            if (type == LogType.Assert)
            {
                log.Type = "Fatal";
                _fatalLogCount += 1;
            }
            else if (type == LogType.Exception || type == LogType.Error)
            {
                log.Type = "Error";
                _errorLogCount += 1;
            }
            else if (type == LogType.Warning)
            {
                log.Type = "Warning";
                _warningLogCount += 1;
            }
            else if (type == LogType.Log)
            {
                log.Type = "Info";
                _infoLogCount += 1;
            }
            log.Name = "[" + log.Type + "] [" + log.Time + "] " + log.Message;
            _consoleLogs.Add(log);

            if (_warningLogCount > 0)
            {
                _fpsColor = Color.yellow;
            }
            if (_errorLogCount > 0)
            {
                _fpsColor = Color.red;
            }
        }
        /// <summary>
        /// 更新FPS
        /// </summary>
        private void FPSUpdate()
        {
            float time = Time.realtimeSinceStartup - _lastShowFPSTime;
            if (time >= 1)
            {
                _fps = (int)(1.0f / Time.deltaTime);
                _lastShowFPSTime = Time.realtimeSinceStartup;

                if (_fps > _maxfps) _maxfps = _fps;
                if (_fps < _minfps) _minfps = _fps;
            }
        }
        /// <summary>
        /// 屏幕截图
        /// </summary>
        private IEnumerator ScreenShot()
        {
            string path = "";
#if UNITY_ANDROID
            path = "/mnt/sdcard/DCIM/ScreenShot/";
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            path = Application.dataPath + "/ScreenShot/";
#endif

            if (path != "")
            {
                Main.m_Debug.IsEnableDebugger = false;
                yield return YieldInstructioner.GetWaitForEndOfFrame();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                texture.Apply();
                string name = "ScreenShotImage_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".png";
                byte[] bytes = texture.EncodeToPNG();
                File.WriteAllBytes(path + name, bytes);
                Main.m_Debug.IsEnableDebugger = true;
            }
            else
            {
                GlobalTools.LogWarning("当前平台不支持截屏！");
                yield return 0;
            }
        }
        #endregion

        #region GUI Function
        /// <summary>
        /// 调试器游戏对象UI
        /// </summary>
        private void OnDebuggerGameObjectGUI(DebuggerGameObject gameObject, int level)
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = gameObject.Target ? (gameObject.Target.activeSelf ? Color.cyan : Color.gray) : Color.red;
            if (gameObject.Childrens.Count > 0)
            {
                GUILayout.Space(20 * level);
                if (GUILayout.Button(gameObject.IsExpand ? _expandTexture : _retractTexture, "Label", GUILayout.Width(16), GUILayout.Height(16)))
                {
                    gameObject.IsExpand = !gameObject.IsExpand;
                }
            }
            else
            {
                GUILayout.Space(20 * level + 20);
            }
            bool value = _debuggerScene.CurrentGameObject == gameObject;
            if (GUILayout.Toggle(value, gameObject.Name) != value)
            {
                if (_debuggerScene.CurrentGameObject != gameObject)
                {
                    _debuggerScene.CurrentGameObject = gameObject;
                }
                else
                {
                    _debuggerScene.CurrentGameObject = null;
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (_debuggerScene.CurrentGameObject == gameObject && gameObject.Target)
            {
                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal();
                bool active = GUILayout.Toggle(gameObject.Target.activeSelf, "Active");
                if (active != gameObject.Target.activeSelf)
                {
                    gameObject.Target.SetActive(active);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Tag: " + gameObject.Target.tag);
                GUILayout.Label("Layer: " + gameObject.Layer);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    Main.Kill(gameObject.Target);
                    _debuggerScene.Refresh();
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            if (gameObject.IsExpand)
            {
                for (int i = 0; i < gameObject.Childrens.Count; i++)
                {
                    OnDebuggerGameObjectGUI(gameObject.Childrens[i], level + 1);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 调试器模块
    /// </summary>
    public enum DebuggerModule
    {
        Console,
        Scene,
        Memory,
        DrawCall,
        System,
        Screen,
        Quality,
        Time,
        Environment
    }
}