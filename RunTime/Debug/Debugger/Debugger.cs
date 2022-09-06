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
    public sealed class Debugger
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
            private set
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
        /// <summary>
        /// 当前的帧率
        /// </summary>
        public int FPS { get; private set; } = 0;
        /// <summary>
        /// 当前的最小帧率
        /// </summary>
        public int MinFPS { get; private set; } = 60;
        /// <summary>
        /// 当前的最大帧率
        /// </summary>
        public int MaxFPS { get; private set; } = 0;
        #endregion

        #region Private Field
        private GUISkin _skin;
        private bool _isChinese;
        private Rect _dragWindowRect;
        private Rect _minWindowRect;
        private Rect _maxWindowRect;
        private bool _isExpand = false;
        private DebuggerModule _module = DebuggerModule.Console;
        private Color _fpsColor = Color.white;
        private float _lastTime = 0f;
        private int _fpsCount = 0;
        private Dictionary<string, string> _words = new Dictionary<string, string>();
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
        public void OnInit(GUISkin skin, bool isChinese)
        {
            Application.logMessageReceived += OnLogMessageReceived;

            _skin = skin;
            _isChinese = isChinese;
            _dragWindowRect = new Rect(0, 0, 10000, 20);
            _minWindowRect = new Rect(0, 0, 100, 60);
            _maxWindowRect = new Rect(0, 0, 700, 400);

            _debuggerScene = new DebuggerScene();
            _expandTexture = Resources.Load<Texture>("Texture/Debug/Expand");
            _retractTexture = Resources.Load<Texture>("Texture/Debug/Retract");

            GenerateWords();
        }
        public void OnTerminate()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
        public void OnDebuggerGUI()
        {
            GUI.skin = _skin;

            if (_isExpand)
            {
                _maxWindowRect = GUI.Window(0, _maxWindowRect, OnExpandGUIWindow, GetWord("DEBUGGER"));
                if (_maxWindowRect.x < 0)
                {
                    _maxWindowRect.x = 0;
                }
                else if ((_maxWindowRect.x + _maxWindowRect.width) > Screen.width)
                {
                    _maxWindowRect.x = Screen.width - _maxWindowRect.width;
                }
                if (_maxWindowRect.y < 0)
                {
                    _maxWindowRect.y = 0;
                }
                else if ((_maxWindowRect.y + _maxWindowRect.height) > Screen.height)
                {
                    _maxWindowRect.y = Screen.height - _maxWindowRect.height;
                }
            }
            else
            {
                _minWindowRect = GUI.Window(0, _minWindowRect, OnRetractGUIWindow, GetWord("DEBUGGER"));
                if (_minWindowRect.x < 0)
                {
                    _minWindowRect.x = 0;
                }
                else if ((_minWindowRect.x + _minWindowRect.width) > Screen.width)
                {
                    _minWindowRect.x = Screen.width - _minWindowRect.width;
                }
                if (_minWindowRect.y < 0)
                {
                    _minWindowRect.y = 0;
                }
                else if ((_minWindowRect.y + _minWindowRect.height) > Screen.height)
                {
                    _minWindowRect.y = Screen.height - _minWindowRect.height;
                }
            }
        }
        public void RefreshMaskState()
        {
            Main.m_UI.IsDisplayMask = Main.m_Debug.IsEnableDebugger && _isExpand;
        }
        public void RefreshFPS()
        {
            float time = Time.realtimeSinceStartup - _lastTime;
            _fpsCount += 1;
            if (time > 1)
            {
                FPS = (int)(_fpsCount / time);
                _fpsCount = 0;
                _lastTime = Time.realtimeSinceStartup;

                if (FPS > MaxFPS) MaxFPS = FPS;
                if (FPS < MinFPS) MinFPS = FPS;
            }
        }
        #endregion

        #region Additional Function
        /// <summary>
        /// 展开的窗口
        /// </summary>
        private void OnExpandGUIWindow(int windowId)
        {
            GUI.DragWindow(_dragWindowRect);

            #region Title
            GUILayout.BeginHorizontal();
            GUI.contentColor = _fpsColor;
            if (GUILayout.Button(string.Format("{0}: {1}", GetWord("FPS"), FPS), GUILayout.Height(40)))
            {
                _isExpand = false;
                RefreshMaskState();
            }
            GUI.contentColor = (Module == DebuggerModule.Console ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("Console"), GUILayout.Height(40)))
            {
                Module = DebuggerModule.Console;
            }
            GUI.contentColor = (Module == DebuggerModule.Scene ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("Scene"), GUILayout.Height(40)))
            {
                Module = DebuggerModule.Scene;
            }
            GUI.contentColor = (Module == DebuggerModule.Memory ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("Memory"), GUILayout.Height(40)))
            {
                Module = DebuggerModule.Memory;
            }
            GUI.contentColor = (Module == DebuggerModule.DrawCall ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("DrawCall"), GUILayout.Height(40)))
            {
                Module = DebuggerModule.DrawCall;
            }
            GUI.contentColor = (Module == DebuggerModule.System ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("System"), GUILayout.Height(40)))
            {
                Module = DebuggerModule.System;
            }
            GUI.contentColor = (Module == DebuggerModule.Screen ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("Screen"), GUILayout.Height(40)))
            {
                Module = DebuggerModule.Screen;
            }
            GUI.contentColor = (Module == DebuggerModule.Quality ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("Quality"), GUILayout.Height(40)))
            {
                Module = DebuggerModule.Quality;
            }
            GUI.contentColor = (Module == DebuggerModule.Time ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("Time"), GUILayout.Height(40)))
            {
                Module = DebuggerModule.Time;
            }
            GUI.contentColor = (Module == DebuggerModule.Environment ? Color.white : Color.gray);
            if (GUILayout.Button(GetWord("Environment"), GUILayout.Height(40)))
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
                    if (GUILayout.Button(GetWord("Clear"), GUILayout.Width(60), GUILayout.Height(20)))
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
                    _showInfoLog = GUILayout.Toggle(_showInfoLog, string.Format("{0} [{1}]", GetWord("Info"), _infoLogCount), GUILayout.Height(20));
                    GUI.contentColor = (_showWarningLog ? Color.white : Color.gray);
                    _showWarningLog = GUILayout.Toggle(_showWarningLog, string.Format("{0} [{1}]", GetWord("Warning"), _warningLogCount), GUILayout.Height(20));
                    GUI.contentColor = (_showErrorLog ? Color.white : Color.gray);
                    _showErrorLog = GUILayout.Toggle(_showErrorLog, string.Format("{0} [{1}]", GetWord("Error"), _errorLogCount), GUILayout.Height(20));
                    GUI.contentColor = (_showFatalLog ? Color.white : Color.gray);
                    _showFatalLog = GUILayout.Toggle(_showFatalLog, string.Format("{0} [{1}]", GetWord("Fatal"), _fatalLogCount), GUILayout.Height(20));
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
                    if (GUILayout.Button(GetWord("Refresh"), GUILayout.Width(80), GUILayout.Height(20)))
                    {
                        _debuggerScene.Refresh();
                        return;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    {
                        GUILayout.BeginVertical("Box", GUILayout.Width(335));
                        GUI.contentColor = Color.yellow;
                        GUILayout.Label(GetWord("Hierarchy"), GUILayout.Height(20));
                        GUI.contentColor = Color.white;

                        GUILayout.BeginHorizontal();
                        GUILayout.Label(GetWord("Search") + ":", GUILayout.Width(60));
                        _debuggerScene.GameObjectFiltrate = GUILayout.TextField(_debuggerScene.GameObjectFiltrate);
                        if (GUILayout.Button(GetWord("Search"), GUILayout.Width(60)))
                        {
                            _debuggerScene.ExecuteGameObjectFiltrate(_debuggerGameObjects);
                        }
                        GUILayout.EndHorizontal();

                        _scrollSceneView = GUILayout.BeginScrollView(_scrollSceneView);
                        if (_debuggerScene.IsShowGameObjectFiltrate)
                        {
                            for (int i = 0; i < _debuggerGameObjects.Count; i++)
                            {
                                OnGameObjectGUI(_debuggerGameObjects[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < _debuggerScene.GameObjectRoots.Count; i++)
                            {
                                OnGameObjectGUILevel(_debuggerScene.GameObjectRoots[i], 0);
                            }
                        }
                        GUILayout.EndScrollView();

                        GUILayout.EndVertical();
                    }

                    {
                        GUILayout.BeginVertical("Box", GUILayout.Width(335));
                        GUI.contentColor = Color.yellow;
                        GUILayout.Label(GetWord("Inspector"), GUILayout.Height(20));
                        GUI.contentColor = Color.white;

                        if (_debuggerScene.CurrentGameObject != null && _debuggerScene.CurrentGameObject.Target)
                        {
                            GUILayout.BeginHorizontal();
                            _debuggerScene.IsReadyAddComponent = GUILayout.Toggle(_debuggerScene.IsReadyAddComponent, GetWord("Add Component"), "Button");
                            if (_debuggerScene.CurrentComponent != null)
                            {
                                if (GUILayout.Button(GetWord("Delete Component")))
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
                                GUILayout.Label(GetWord("Search") + ":", GUILayout.Width(60));
                                _debuggerScene.ComponentFiltrate = GUILayout.TextField(_debuggerScene.ComponentFiltrate);
                                if (GUILayout.Button(GetWord("Search"), GUILayout.Width(60)))
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
                                                GUILayout.Label(GetWord("No Debugger GUI!"));
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
                    GUILayout.Label(GetWord("Memory") + " " + GetWord("Information"), GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(275));

                    long memory = Profiler.GetTotalReservedMemoryLong() / 1000000;
                    if (memory > _maxTotalReservedMemory) _maxTotalReservedMemory = memory;
                    if (memory < _minTotalReservedMemory) _minTotalReservedMemory = memory;
                    string text = string.Format("{0}: {1}MB        [{2}: {3}  {4}: {5}]", GetWord("Total Memory"), memory, GetWord("Min"), _minTotalReservedMemory, GetWord("Max"), _maxTotalReservedMemory);
                    GUILayout.Label(text);

                    memory = Profiler.GetTotalAllocatedMemoryLong() / 1000000;
                    if (memory > _maxTotalAllocatedMemory) _maxTotalAllocatedMemory = memory;
                    if (memory < _minTotalAllocatedMemory) _minTotalAllocatedMemory = memory;
                    text = string.Format("{0}: {1}MB        [{2}: {3}  {4}: {5}]", GetWord("Used Memory"), memory, GetWord("Min"), _minTotalAllocatedMemory, GetWord("Max"), _maxTotalAllocatedMemory);
                    GUILayout.Label(text);

                    memory = Profiler.GetTotalUnusedReservedMemoryLong() / 1000000;
                    if (memory > _maxTotalUnusedReservedMemory) _maxTotalUnusedReservedMemory = memory;
                    if (memory < _minTotalUnusedReservedMemory) _minTotalUnusedReservedMemory = memory;
                    text = string.Format("{0}: {1}MB        [{2}: {3}  {4}: {5}]", GetWord("Free Memory"), memory, GetWord("Min"), _minTotalUnusedReservedMemory, GetWord("Max"), _maxTotalUnusedReservedMemory);
                    GUILayout.Label(text);

                    memory = Profiler.GetMonoHeapSizeLong() / 1000000;
                    if (memory > _maxMonoHeapSize) _maxMonoHeapSize = memory;
                    if (memory < _minMonoHeapSize) _minMonoHeapSize = memory;
                    text = string.Format("{0}: {1}MB        [{2}: {3}  {4}: {5}]", GetWord("Total Mono Memory"), memory, GetWord("Min"), _minMonoHeapSize, GetWord("Max"), _maxMonoHeapSize);
                    GUILayout.Label(text);

                    memory = Profiler.GetMonoUsedSizeLong() / 1000000;
                    if (memory > _maxMonoUsedSize) _maxMonoUsedSize = memory;
                    if (memory < _minMonoUsedSize) _minMonoUsedSize = memory;
                    text = string.Format("{0}: {1}MB        [{2}: {3}  {4}: {5}]", GetWord("Used Mono Memory"), memory, GetWord("Min"), _minMonoUsedSize, GetWord("Max"), _maxMonoUsedSize);
                    GUILayout.Label(text);

                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(GetWord("Unload unused resources"), GUILayout.Height(20)))
                    {
                        Resources.UnloadUnusedAssets();
                    }
                    if (GUILayout.Button(GetWord("Garbage Collection"), GUILayout.Height(20)))
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    GUILayout.EndHorizontal();
                    #endregion
                    break;
                case DebuggerModule.DrawCall:
                    #region DrawCall
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label(GetWord("DrawCall") + " " + GetWord("Information"), GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    _scrollDrawCallView = GUILayout.BeginScrollView(_scrollDrawCallView, "Box");
#if UNITY_EDITOR
                    GUILayout.Label(GetWord("DrawCalls") + ": " + UnityEditor.UnityStats.drawCalls);
                    GUILayout.Label(GetWord("Batches") + ": " + UnityEditor.UnityStats.batches);
                    GUILayout.Label(GetWord("Static Batched DrawCalls") + ": " + UnityEditor.UnityStats.staticBatchedDrawCalls);
                    GUILayout.Label(GetWord("Static Batches") + ": " + UnityEditor.UnityStats.staticBatches);
                    GUILayout.Label(GetWord("Dynamic Batched DrawCalls") + ": " + UnityEditor.UnityStats.dynamicBatchedDrawCalls);
                    GUILayout.Label(GetWord("Dynamic Batches") + ": " + UnityEditor.UnityStats.dynamicBatches);
                    if (UnityEditor.UnityStats.triangles > 10000)
                    {
                        GUILayout.Label(GetWord("Triangles") + ": " + UnityEditor.UnityStats.triangles / 10000 + "W");
                    }
                    else
                    {
                        GUILayout.Label(GetWord("Triangles") + ": " + UnityEditor.UnityStats.triangles);
                    }
                    if (UnityEditor.UnityStats.vertices > 10000)
                    {
                        GUILayout.Label(GetWord("Vertices") + ": " + UnityEditor.UnityStats.vertices / 10000 + "W");
                    }
                    else
                    {
                        GUILayout.Label(GetWord("Vertices") + ": " + UnityEditor.UnityStats.vertices);
                    }
#else
                    if (_isChinese)
                    {
                        GUILayout.Label("绘制信息只在编辑器模式下可见！");
                    }
                    else
                    {
                        GUILayout.Label("DrawCall can only be displayed in Editor Mode!");
                    }
#endif
                    GUILayout.EndScrollView();
                    #endregion
                    break;
                case DebuggerModule.System:
                    #region System
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label(GetWord("System") + " " + GetWord("Information"), GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    _scrollSystemView = GUILayout.BeginScrollView(_scrollSystemView, "Box");
                    GUILayout.Label(GetWord("Operating System") + ": " + SystemInfo.operatingSystem);
                    GUILayout.Label(GetWord("System Memory") + ": " + SystemInfo.systemMemorySize.ToString() + "MB");
                    GUILayout.Label(GetWord("Processor") + ": " + SystemInfo.processorType);
                    GUILayout.Label(GetWord("Number Of Processor") + ": " + SystemInfo.processorCount.ToString());
                    GUILayout.Label(GetWord("Graphics Device Name") + ": " + SystemInfo.graphicsDeviceName);
                    GUILayout.Label(GetWord("Graphics Device Type") + ": " + SystemInfo.graphicsDeviceType.ToString());
                    GUILayout.Label(GetWord("Graphics Memory") + ": " + SystemInfo.graphicsMemorySize.ToString() + "MB");
                    GUILayout.Label(GetWord("Graphics DeviceID") + ": " + SystemInfo.graphicsDeviceID.ToString());
                    GUILayout.Label(GetWord("Graphics Device Vendor") + ": " + SystemInfo.graphicsDeviceVendor);
                    GUILayout.Label(GetWord("Graphics Device Vendor ID") + ": " + SystemInfo.graphicsDeviceVendorID.ToString());
                    GUILayout.Label(GetWord("Device Model") + ": " + SystemInfo.deviceModel);
                    GUILayout.Label(GetWord("Device Name") + ": " + SystemInfo.deviceName);
                    GUILayout.Label(GetWord("Device Type") + ": " + SystemInfo.deviceType.ToString());
                    GUILayout.Label(GetWord("Device Unique Identifier") + ": " + SystemInfo.deviceUniqueIdentifier);
                    GUILayout.EndScrollView();
                    #endregion
                    break;
                case DebuggerModule.Screen:
                    #region Screen
                    GUILayout.BeginHorizontal();
                    GUI.contentColor = Color.white;
                    GUILayout.Label(GetWord("Screen") + " " + GetWord("Information"), GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(250), GUILayout.Height(275));
                    GUILayout.Label(GetWord("DPI") + ": " + Screen.dpi.ToString());
                    GUILayout.Label(GetWord("Resolution") + ": " + Screen.width.ToString() + " x " + Screen.height.ToString());
                    GUILayout.Label(GetWord("Device Resolution") + ": " + Screen.currentResolution.ToString());
                    GUILayout.Label(GetWord("Device Sleep") + ": " + (Screen.sleepTimeout == SleepTimeout.NeverSleep ? GetWord("Never Sleep") : GetWord("System Setting")));
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(GetWord("Device Sleep"), GUILayout.Height(20)))
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
                    if (GUILayout.Button(GetWord("Screen Capture"), GUILayout.Height(20)))
                    {
                        Main.Current.StartCoroutine(ScreenShot());
                    }
                    if (GUILayout.Button(GetWord("Full Screen"), GUILayout.Height(20)))
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
                    GUILayout.Label(GetWord("Quality") + " " + GetWord("Information"), GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(275));
                    GUILayout.Label(GetWord("Graphics Quality") + ": " + GetWord(QualitySettings.names[QualitySettings.GetQualityLevel()]));
                    GUILayout.Label(GetWord("Min FPS") + ": " + MinFPS.ToString());
                    GUILayout.Label(GetWord("Max FPS") + ": " + MaxFPS.ToString());
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(GetWord("Lower"), GUILayout.Height(20)))
                    {
                        QualitySettings.DecreaseLevel();
                    }
                    if (GUILayout.Button(GetWord("Upgrade"), GUILayout.Height(20)))
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
                    GUILayout.Label(GetWord("Time") + " " + GetWord("Information"), GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(275));
                    GUILayout.Label(GetWord("Current Time") + ": " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                    GUILayout.Label(GetWord("Elapse Time") + ": " + ((int)Time.realtimeSinceStartup).ToString());
                    GUILayout.Label(GetWord("Time Scale") + ": " + Time.timeScale.ToString());
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("0.1 " + GetWord("Multiple"), GUILayout.Height(20)))
                    {
                        Time.timeScale = 0.1f;
                    }
                    if (GUILayout.Button("0.2 " + GetWord("Multiple"), GUILayout.Height(20)))
                    {
                        Time.timeScale = 0.2f;
                    }
                    if (GUILayout.Button("0.5 " + GetWord("Multiple"), GUILayout.Height(20)))
                    {
                        Time.timeScale = 0.5f;
                    }
                    if (GUILayout.Button("1 " + GetWord("Multiple"), GUILayout.Height(20)))
                    {
                        Time.timeScale = 1;
                    }
                    if (GUILayout.Button("2 " + GetWord("Multiple"), GUILayout.Height(20)))
                    {
                        Time.timeScale = 2;
                    }
                    if (GUILayout.Button("5 " + GetWord("Multiple"), GUILayout.Height(20)))
                    {
                        Time.timeScale = 5;
                    }
                    if (GUILayout.Button("10 " + GetWord("Multiple"), GUILayout.Height(20)))
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
                    GUILayout.Label(GetWord("Environment") + " " + GetWord("Information"), GUILayout.Height(20));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box", GUILayout.Height(275));
                    GUILayout.Label(GetWord("Product Name") + ": " + Application.productName);
                    GUILayout.Label(GetWord("Product Identifier") + ": " + Application.identifier);
                    GUILayout.Label(GetWord("Product Version") + ": " + Application.version);
                    GUILayout.Label(GetWord("Product DataPath") + ": " + Application.dataPath);
                    GUILayout.Label(GetWord("Company Name") + ": " + Application.companyName);
                    GUILayout.Label(GetWord("Unity Version") + ": " + Application.unityVersion);
                    GUILayout.Label(GetWord("Has Pro License") + ": " + GetWord(Application.HasProLicense().ToString()));
                    string internetState = GetWord("NotReachable");
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                        internetState = GetWord("No Network");
                    else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                        internetState = GetWord("WiFi");
                    else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                        internetState = GetWord("Data Network");
                    GUILayout.Label(GetWord("Internet State") + ": " + internetState);
                    GUILayout.EndVertical();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(GetWord("Quit"), GUILayout.Height(20)))
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
        /// <summary>
        /// 收起的窗口
        /// </summary>
        private void OnRetractGUIWindow(int windowId)
        {
            GUI.DragWindow(_dragWindowRect);

            GUI.contentColor = _fpsColor;
            if (GUILayout.Button(string.Format("{0}: {1}", GetWord("FPS"), FPS), GUILayout.Width(80), GUILayout.Height(30)))
            {
                _isExpand = true;
                RefreshMaskState();
            }
            GUI.contentColor = Color.white;
        }
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
            log.Name = string.Format("[{0}] [{1}] {2}", GetWord(log.Type), log.Time, log.Message);
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
        /// 生成词汇
        /// </summary>
        private void GenerateWords()
        {
            #region Title
            _words.Add("DEBUGGER", "调试器");
            _words.Add("FPS", "帧率");
            _words.Add("Console", "控制台");
            _words.Add("Scene", "场景");
            _words.Add("Memory", "内存");
            _words.Add("DrawCall", "绘制");
            _words.Add("System", "系统");
            _words.Add("Screen", "屏幕");
            _words.Add("Quality", "质量");
            _words.Add("Time", "时间");
            _words.Add("Environment", "环境");
            _words.Add("Information", "信息");
            #endregion

            #region Console
            _words.Add("Clear", "清除");
            _words.Add("Info", "常规信息");
            _words.Add("Warning", "警告日志");
            _words.Add("Error", "错误日志");
            _words.Add("Fatal", "致命错误");
            #endregion

            #region Scene
            _words.Add("Refresh", "刷新");
            _words.Add("Hierarchy", "场景内的所有物体");
            _words.Add("Search", "查找");
            _words.Add("Active", "激活");
            _words.Add("Look at", "看向他");
            _words.Add("Delete", "删除");
            _words.Add("Tag", "标签");
            _words.Add("Layer", "层级");
            _words.Add("Inspector", "当前选中物体的组件");
            _words.Add("Add Component", "添加组件");
            _words.Add("Delete Component", "删除组件");
            _words.Add("No Debugger GUI!", "此组件没有可用的调试器界面！");
            #endregion

            #region Memory
            _words.Add("Total Memory", "总内存");
            _words.Add("Used Memory", "已使用的内存");
            _words.Add("Free Memory", "空闲的内存");
            _words.Add("Total Mono Memory", "总的托管堆内存");
            _words.Add("Used Mono Memory", "已使用的托管堆内存");
            _words.Add("Min", "最小值");
            _words.Add("Max", "最大值");
            _words.Add("Unload unused resources", "卸载未使用的资源");
            _words.Add("Garbage Collection", "进行一次垃圾回收");
            #endregion

            #region DrawCall
            _words.Add("DrawCalls", "绘制次数");
            _words.Add("Batches", "批处理次数");
            _words.Add("Static Batched DrawCalls", "静态批处理绘制次数");
            _words.Add("Static Batches", "静态批处理次数");
            _words.Add("Dynamic Batched DrawCalls", "动态批处理绘制次数");
            _words.Add("Dynamic Batches", "动态批处理次数");
            _words.Add("Triangles", "三角面总数");
            _words.Add("Vertices", "顶点总数");
            #endregion

            #region System
            _words.Add("Operating System", "操作系统");
            _words.Add("System Memory", "系统内存");
            _words.Add("Processor", "处理器");
            _words.Add("Number Of Processor", "处理器数量");
            _words.Add("Graphics Device Name", "显卡名称");
            _words.Add("Graphics Device Type", "显卡类型");
            _words.Add("Graphics Memory", "显存");
            _words.Add("Graphics DeviceID", "显卡设备ID");
            _words.Add("Graphics Device Vendor", "显卡设备厂商");
            _words.Add("Graphics Device Vendor ID", "显卡设备厂商ID");
            _words.Add("Device Model", "设备型号");
            _words.Add("Device Name", "设备名称");
            _words.Add("Device Type", "设备类型");
            _words.Add("Device Unique Identifier", "设备唯一标识符");
            #endregion

            #region Screen
            _words.Add("DPI", "显示器像素密度");
            _words.Add("Resolution", "程序分辨率");
            _words.Add("Device Resolution", "设备分辨率");
            _words.Add("Device Sleep", "设备休眠");
            _words.Add("Never Sleep", "永不休眠");
            _words.Add("System Setting", "使用系统设置");
            _words.Add("Screen Capture", "截屏");
            _words.Add("Full Screen", "全屏");
            #endregion

            #region Quality
            _words.Add("Graphics Quality", "图形质量");
            _words.Add("Min FPS", "最低帧率");
            _words.Add("Max FPS", "最高帧率");
            _words.Add("Lower", "降低质量");
            _words.Add("Upgrade", "升高质量");
            _words.Add("Very Low", "极低");
            _words.Add("Low", "低");
            _words.Add("Medium", "中等");
            _words.Add("High", "高");
            _words.Add("Very High", "极高");
            _words.Add("Ultra", "超高");
            #endregion

            #region Time
            _words.Add("Current Time", "当前系统时间");
            _words.Add("Elapse Time", "已消逝的时间");
            _words.Add("Time Scale", "时间缩放倍数");
            _words.Add("Multiple", "倍");
            #endregion

            #region Environment
            _words.Add("Product Name", "项目名称");
            _words.Add("Product Identifier", "项目ID");
            _words.Add("Product Version", "项目版本");
            _words.Add("Product DataPath", "项目数据路径");
            _words.Add("Company Name", "公司名称");
            _words.Add("Unity Version", "Unity版本");
            _words.Add("Has Pro License", "Unity专业版");
            _words.Add("True", "是");
            _words.Add("False", "否");
            _words.Add("NotReachable", "不可到达");
            _words.Add("No Network", "没有网络");
            _words.Add("WiFi", "移动热点");
            _words.Add("Data Network", "数据流量");
            _words.Add("Internet State", "网络状态");
            _words.Add("Quit", "退出程序");
            #endregion
        }
        /// <summary>
        /// 获取本地化词汇
        /// </summary>
        /// <param name="key">词汇key</param>
        /// <returns>词汇</returns>
        private string GetWord(string key)
        {
            if (_isChinese && _words.ContainsKey(key))
            {
                return _words[key];
            }
            else
            {
                return key;
            }
        }
        /// <summary>
        /// 屏幕截图
        /// </summary>
        private IEnumerator ScreenShot()
        {
            string path = null;
#if UNITY_ANDROID
            path = "/sdcard/DCIM/ScreenShots/";
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            path = Application.dataPath + "/ScreenShots/";
#endif

            if (!string.IsNullOrEmpty(path))
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
                Main.Kill(texture);
                Main.m_Debug.IsEnableDebugger = true;
            }
            else
            {
                Log.Warning("当前平台不支持截屏！");
                yield return null;
            }
        }
        #endregion

        #region GUI Function
        /// <summary>
        /// 调试器游戏对象UI
        /// </summary>
        private void OnGameObjectGUI(DebuggerGameObject gameObject)
        {
            GUILayout.BeginHorizontal();
            GUI.contentColor = gameObject.Target ? (gameObject.Target.activeSelf ? Color.cyan : Color.gray) : Color.red;
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
                GUI.enabled = gameObject.Name != "HTFramework";
                GUI.contentColor = gameObject.Target.activeSelf ? Color.white : Color.gray;
                bool active = GUILayout.Toggle(gameObject.Target.activeSelf, GetWord("Active"));
                if (active != gameObject.Target.activeSelf)
                {
                    gameObject.Target.SetActive(active);
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(GetWord("Delete")))
                {
                    Main.Kill(gameObject.Target);
                    _debuggerScene.Refresh();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(GetWord("Tag") + ": " + gameObject.Target.tag);
                GUILayout.Label(GetWord("Layer") + ": " + gameObject.Layer);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
        }
        /// <summary>
        /// 调试器游戏对象UI（层级模式）
        /// </summary>
        private void OnGameObjectGUILevel(DebuggerGameObject gameObject, int level)
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
                GUI.enabled = !gameObject.IsMain;
                GUI.contentColor = gameObject.Target.activeSelf ? Color.white : Color.gray;
                bool active = GUILayout.Toggle(gameObject.Target.activeSelf, GetWord("Active"));
                if (active != gameObject.Target.activeSelf)
                {
                    gameObject.Target.SetActive(active);
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(GetWord("Look at")))
                {
                    Main.m_Controller.Mode = ControlMode.FreeControl;
                    Main.m_Controller.SetLookPoint(gameObject.Target.transform.position);
                }
                if (GUILayout.Button(GetWord("Delete")))
                {
                    Main.Kill(gameObject.Target);
                    _debuggerScene.Refresh();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(GetWord("Tag") + ": " + gameObject.Target.tag);
                GUILayout.Label(GetWord("Layer") + ": " + gameObject.Layer);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            if (gameObject.IsExpand)
            {
                for (int i = 0; i < gameObject.Childrens.Count; i++)
                {
                    OnGameObjectGUILevel(gameObject.Childrens[i], level + 1);
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