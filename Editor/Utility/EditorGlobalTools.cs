using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 编辑器全局工具
    /// </summary>
    [InitializeOnLoad]
    public static class EditorGlobalTools
    {
        #region 构造函数
        static EditorGlobalTools()
        {
            OnInitHierarchy();
            OnInitProject();
        }
        #endregion

        #region 框架相关目录
        /// <summary>
        /// 框架相关目录
        /// </summary>
        public static readonly HashSet<string> HTFrameworkFolder = new HashSet<string>() { "HTFramework", "HTFrameworkAI", "HTFrameworkAuxiliary", "HTFrameworkILHotfix" };
        #endregion

        #region About 【优先级0】
        /// <summary>
        /// About
        /// </summary>
        [@MenuItem("HTFramework/About", false, 0)]
        private static void About()
        {
            About about = EditorWindow.GetWindow<About>(true, "HTFramework About", true);
            about.position = new Rect(200, 200, 600, 350);
            about.minSize = new Vector2(600, 350);
            about.maxSize = new Vector2(600, 350);
            about.Show();
        }
        #endregion

        #region Batch 【优先级100-101】
        /// <summary>
        /// 打开ComponentBatch窗口
        /// </summary>
        [@MenuItem("HTFramework/Batch/Component Batch", false, 100)]
        private static void OpenComponentBatch()
        {
            ComponentBatch cb = EditorWindow.GetWindow<ComponentBatch>();
            cb.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            cb.titleContent.text = "Component Batch";
            cb.position = new Rect(200, 200, 300, 140);
            cb.Show();
        }

        /// <summary>
        /// 打开ProjectBatch窗口
        /// </summary>
        [@MenuItem("HTFramework/Batch/Project Batch", false, 101)]
        private static void OpenProjectBatch()
        {
            ProjectBatch pb = EditorWindow.GetWindow<ProjectBatch>();
            pb.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            pb.titleContent.text = "Project Batch";
            pb.position = new Rect(200, 200, 300, 120);
            pb.Show();
        }

        /// <summary>
        /// 【验证函数】设置鼠标射线可捕获物体目标
        /// </summary>
        [@MenuItem("HTFramework/Batch/Set Mouse Ray Target", true)]
        private static bool SetMouseRayTargetValidate()
        {
            return Selection.gameObjects.Length > 0;
        }
        /// <summary>
        /// 设置鼠标射线可捕获物体目标
        /// </summary>
        [@MenuItem("HTFramework/Batch/Set Mouse Ray Target", false, 120)]
        private static void SetMouseRayTarget()
        {
            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                if (!objs[i].GetComponent<Collider>())
                {
                    objs[i].AddComponent<BoxCollider>();
                }
                if (!objs[i].GetComponent<MouseRayTarget>())
                {
                    objs[i].AddComponent<MouseRayTarget>();
                }
                objs[i].GetComponent<MouseRayTarget>().Name = objs[i].name;
            }
        }

        /// <summary>
        /// 【验证函数】设置鼠标射线可捕获UI目标
        /// </summary>
        [@MenuItem("HTFramework/Batch/Set Mouse Ray UI Target", true)]
        private static bool SetMouseRayUITargetValidate()
        {
            return Selection.gameObjects.Length > 0;
        }
        /// <summary>
        /// 设置鼠标射线可捕获UI目标
        /// </summary>
        [@MenuItem("HTFramework/Batch/Set Mouse Ray UI Target", false, 121)]
        private static void SetMouseRayUITarget()
        {
            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                if (!objs[i].GetComponent<Graphic>())
                {
                    GlobalTools.LogWarning("对象 " + objs[i].name + " 没有Graphic组件，无法做为可捕获UI目标！");
                    continue;
                }
                objs[i].GetComponent<Graphic>().raycastTarget = true;
                if (!objs[i].GetComponent<MouseRayUITarget>())
                {
                    objs[i].AddComponent<MouseRayUITarget>();
                }
                objs[i].GetComponent<MouseRayUITarget>().Name = objs[i].name;
            }
        }
        #endregion

        #region Console 【优先级102-105】
        /// <summary>
        /// 清理控制台
        /// </summary>
        [@MenuItem("HTFramework/Console/Clear &1", false, 102)]
        private static void ClearConsole()
        {
            Type logEntries = GetTypeInEditorAssemblies("UnityEditor.LogEntries");
            MethodInfo clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        /// <summary>
        /// 打印普通日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug Log", false, 103)]
        private static void ConsoleDebugLog()
        {
            GlobalTools.LogInfo("Debug.Log!");
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug LogWarning", false, 104)]
        private static void ConsoleDebugLogWarning()
        {
            GlobalTools.LogWarning("Debug.LogWarning!");
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug LogError", false, 105)]
        private static void ConsoleDebugLogError()
        {
            GlobalTools.LogError("Debug.LogError!");
        }
        #endregion

        #region Editor 【优先级106-108】
        /// <summary>
        /// 运行场景
        /// </summary>
        [@MenuItem("HTFramework/Editor/Run &2", false, 106)]
        private static void RunScene()
        {
            EditorApplication.isPlaying = !EditorApplication.isPlaying;
        }

        /// <summary>
        /// 【验证函数】看向指定目标
        /// </summary>
        [@MenuItem("HTFramework/Editor/Look At", true)]
        private static bool LookAtValidate()
        {
            return EditorApplication.isPlaying && Selection.activeGameObject != null;
        }
        /// <summary>
        /// 看向指定目标
        /// </summary>
        [@MenuItem("HTFramework/Editor/Look At", false, 107)]
        private static void LookAt()
        {
            Main.m_Controller.SetLookPoint(Selection.activeGameObject.transform.position);
        }

        /// <summary>
        /// 打开编辑器安装路径
        /// </summary>
        [@MenuItem("HTFramework/Editor/Open Installation Path", false, 108)]
        private static void OpenInstallationPath()
        {
            string path = EditorApplication.applicationPath.Substring(0, EditorApplication.applicationPath.LastIndexOf("/"));
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(path);
            System.Diagnostics.Process.Start(psi);
        }
        #endregion

        #region Tools 【优先级109-113】
        /// <summary>
        /// 合并多个模型网格
        /// </summary>
        [@MenuItem("HTFramework/Tools/Mesh/Mesh Combines", false, 109)]
        private static void MeshCombines()
        {
            if (Selection.gameObjects.Length <= 1)
            {
                GlobalTools.LogWarning("请先选中至少2个以上的待合并网格！");
                return;
            }

            GameObject[] objs = Selection.gameObjects;
            CombineInstance[] combines = new CombineInstance[objs.Length];
            List<Material> materials = new List<Material>();
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("合并网格", "正在合并网格及纹理......（" + i + "/" + objs.Length + "）", ((float)i) / objs.Length);

                if (!objs[i].GetComponent<MeshRenderer>() || !objs[i].GetComponent<MeshFilter>())
                    continue;

                Material[] mats = objs[i].GetComponent<MeshRenderer>().sharedMaterials;
                for (int j = 0; j < mats.Length; j++)
                {
                    materials.Add(mats[j]);
                }

                combines[i].mesh = objs[i].GetComponent<MeshFilter>().sharedMesh;
                combines[i].transform = objs[i].transform.localToWorldMatrix;

                objs[i].SetActive(false);
            }

            EditorUtility.DisplayProgressBar("合并网格", "合并完成！", 1.0f);

            GameObject newMesh = new GameObject("CombineMesh");
            newMesh.AddComponent<MeshFilter>();
            newMesh.AddComponent<MeshRenderer>();
            newMesh.GetComponent<MeshFilter>().sharedMesh = new Mesh();
            //false表示合并为子网格列表，多个材质混用时必须这样设置
            newMesh.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combines, false);
            newMesh.GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();

            AssetDatabase.CreateAsset(newMesh.GetComponent<MeshFilter>().sharedMesh, "Assets/CombineMesh.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.ClearProgressBar();
        }
        
        /// <summary>
        /// 展示模型信息
        /// </summary>
        [@MenuItem("HTFramework/Tools/Mesh/Mesh Info", false, 110)]
        private static void ShowMeshInfo()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                MeshFilter filter = Selection.gameObjects[i].GetComponent<MeshFilter>();
                if (filter)
                {
                    GlobalTools.LogInfo("Mesh [" + filter.name + "] : vertices " + filter.sharedMesh.vertexCount + ", triangles " + filter.sharedMesh.triangles.Length);
                }
            }
        }

        /// <summary>
        /// 打开 Assembly Viewer
        /// </summary>
        [@MenuItem("HTFramework/Tools/Assembly Viewer", false, 111)]
        private static void OpenAssemblyViewer()
        {
            AssemblyViewer viewer = EditorWindow.GetWindow<AssemblyViewer>();
            viewer.titleContent.image = EditorGUIUtility.IconContent("Assembly Icon").image;
            viewer.titleContent.text = "Assembly Viewer";
            viewer.Show();
        }

        /// <summary>
        /// 打开 Custom Executer
        /// </summary>
        [@MenuItem("HTFramework/Tools/Custom Executer", false, 112)]
        private static void OpenCustomExecuter()
        {
            CustomExecuter tools = EditorWindow.GetWindow<CustomExecuter>();
            tools.titleContent.image = EditorGUIUtility.IconContent("LightProbeProxyVolume Icon").image;
            tools.titleContent.text = "Custom Executer";
            tools.minSize = new Vector2(500, 600);
            tools.Initialization();
            tools.Show();
        }

        private static List<MethodInfo> _customTools = new List<MethodInfo>();

        /// <summary>
        /// 执行 Custom Tool
        /// </summary>
        [@MenuItem("HTFramework/Tools/Custom Tool", false, 113)]
        private static void ExecuteCustomTool()
        {
            _customTools.Clear();
            List<Type> types = GetTypesInEditorAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < methods.Length; j++)
                {
                    if (methods[j].IsDefined(typeof(CustomToolAttribute), false))
                    {
                        _customTools.Add(methods[j]);
                    }
                }
            }
            if (_customTools.Count <= 0)
            {
                GlobalTools.LogWarning("当前不存在至少一个自定义工具！为任何处于 Editor 文件夹中的类的无参静态函数添加 CustomTool 特性，可将该函数附加至自定义工具菜单！");
            }
            else
            {
                for (int i = 0; i < _customTools.Count; i++)
                {
                    _customTools[i].Invoke(null, null);
                }
                GlobalTools.LogInfo("已执行 " + _customTools.Count + " 个自定义工具！");
                _customTools.Clear();
            }
        }
        #endregion

        #region HTFramework Setting... 【优先级1000】
        /// <summary>
        /// HTFramework Setting...
        /// </summary>
        [@MenuItem("HTFramework/HTFramework Settings...", false, 1000)]
        private static void OpenHTFrameworkSettings()
        {
            Setter setter = EditorWindow.GetWindow<Setter>();
            setter.titleContent.image = EditorGUIUtility.IconContent("SettingsIcon").image;
            setter.titleContent.text = "HTFramework Setter";
            setter.minSize = new Vector2(640, 580);
            setter.Show();
        }
        #endregion

        #region 工程视图新建菜单
        /// <summary>
        /// 新建AspectProxy类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# AspectProxy Script", false, 11)]
        private static void CreateAspectProxy()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_AspectProxy_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 AspectProxy 类", directory, "NewAspectProxy", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/AspectProxyTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                        EditorPrefs.SetString(EditorPrefsTable.Script_AspectProxy_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建AspectProxy失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建CustomModule类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# CustomModule Script", false, 12)]
        private static void CreateCustomModule()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_CustomModule_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 CustomModule 类", directory, "NewCustomModule", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/CustomModuleTemplate.txt", typeof(TextAsset)) as TextAsset;
                    if (asset)
                    {
                        string code = asset.text;
                        code = code.Replace("#SCRIPTNAME#", className);
                        code = code.Replace("#MODULENAME#", className);
                        File.AppendAllText(path, code);
                        asset = null;
                        AssetDatabase.Refresh();

                        string assetPath = path.Substring(path.LastIndexOf("Assets"));
                        TextAsset cs = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                        EditorGUIUtility.PingObject(cs);
                        Selection.activeObject = cs;
                        AssetDatabase.OpenAsset(cs);
                        EditorPrefs.SetString(EditorPrefsTable.Script_CustomModule_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建CustomModule失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建DataSet类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# DataSet Script", false, 13)]
        private static void CreateDataSet()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_DataSet_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 DataSet 类", directory, "NewDataSet", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/DataSetTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                        EditorPrefs.SetString(EditorPrefsTable.Script_DataSet_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建DataSet失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建EntityLogic类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# EntityLogic Script", false, 14)]
        private static void CreateEntityLogic()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_EntityLogic_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 EntityLogic 类", directory, "NewEntityLogic", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/EntityLogicTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                        EditorPrefs.SetString(EditorPrefsTable.Script_EntityLogic_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建EntityLogic失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建EventHandler类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# EventHandler Script", false, 15)]
        private static void CreateEventHandler()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_EventHandler_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 EventHandler 类", directory, "NewEventHandler", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/EventHandlerTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                        EditorPrefs.SetString(EditorPrefsTable.Script_EventHandler_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建EventHandler失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建FiniteState类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# FiniteState Script", false, 16)]
        private static void CreateFiniteState()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_FiniteState_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 FiniteState 类", directory, "NewFiniteState", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/FiniteStateTemplate.txt", typeof(TextAsset)) as TextAsset;
                    if (asset)
                    {
                        string code = asset.text;
                        code = code.Replace("#SCRIPTNAME#", className);
                        code = code.Replace("#STATENAME#", className);
                        File.AppendAllText(path, code);
                        asset = null;
                        AssetDatabase.Refresh();

                        string assetPath = path.Substring(path.LastIndexOf("Assets"));
                        TextAsset cs = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                        EditorGUIUtility.PingObject(cs);
                        Selection.activeObject = cs;
                        AssetDatabase.OpenAsset(cs);
                        EditorPrefs.SetString(EditorPrefsTable.Script_FiniteState_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建FiniteState失败，已存在类型 " + className);
                }
            }
        }
        
        /// <summary>
        /// 新建Procedure类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# Procedure Script", false, 17)]
        private static void CreateProcedure()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_Procedure_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 Procedure 类", directory, "NewProcedure", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/ProcedureTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                        EditorPrefs.SetString(EditorPrefsTable.Script_Procedure_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建Procedure失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建ProtocolChannel类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# Protocol Channel Script", false, 18)]
        private static void CreateProtocolChannel()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_ProtocolChannel_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 ProtocolChannel 类", directory, "NewProtocolChannel", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/ProtocolChannelTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                        EditorPrefs.SetString(EditorPrefsTable.Script_ProtocolChannel_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建ProtocolChannel失败，已存在类型 " + className);
                }
            }
        }
        
        /// <summary>
        /// 新建UILogicResident类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# UILogicResident Script", false, 19)]
        private static void CreateUILogicResident()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_UILogicResident_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 UILogicResident 类", directory, "NewUILogicResident", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/UILogicResidentTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                        EditorPrefs.SetString(EditorPrefsTable.Script_UILogicResident_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建UILogicResident失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建UILogicTemporary类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# UILogicTemporary Script", false, 20)]
        private static void CreateUILogicTemporary()
        {
            string directory = EditorPrefs.GetString(EditorPrefsTable.Script_UILogicTemporary_Directory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 UILogicTemporary 类", directory, "NewUILogicTemporary", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/UILogicTemporaryTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                        EditorPrefs.SetString(EditorPrefsTable.Script_UILogicTemporary_Directory, path.Substring(0, path.LastIndexOf("/")));
                    }
                }
                else
                {
                    GlobalTools.LogError("新建UILogicTemporary失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 【验证函数】新建HotfixProcedure类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixProcedure Script", true)]
        private static bool CreateHotfixProcedureValidate()
        {
            return AssetDatabase.IsValidFolder("Assets/Hotfix");
        }

        /// <summary>
        /// 新建HotfixProcedure类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixProcedure Script", false, 100)]
        private static void CreateHotfixProcedure()
        {
            string path = EditorUtility.SaveFilePanel("新建 HotfixProcedure 类", Application.dataPath + "/Hotfix", "NewHotfixProcedure", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/HotfixProcedureTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                    }
                }
                else
                {
                    GlobalTools.LogError("新建HotfixProcedure失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 【验证函数】新建HotfixObject类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixObject Script", true)]
        private static bool CreateHotfixObjectValidate()
        {
            return AssetDatabase.IsValidFolder("Assets/Hotfix");
        }

        /// <summary>
        /// 新建HotfixObject类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixObject Script", false, 101)]
        private static void CreateHotfixObject()
        {
            string path = EditorUtility.SaveFilePanel("新建 HotfixObject 类", Application.dataPath + "/Hotfix", "NewHotfixObject", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/HotfixObjectTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                    }
                }
                else
                {
                    GlobalTools.LogError("新建HotfixObject失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 【验证函数】新建WebGL插件
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/WebGL Plugin", true)]
        private static bool CreateWebGLPluginValidate()
        {
#if UNITY_WEBGL
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// 新建WebGL插件
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/WebGL Plugin", false, 200)]
        private static void CreateWebGLPlugin()
        {
            string pluginsDirectory = Application.dataPath + "/Plugins";
            string pluginPath = pluginsDirectory + "/WebGLPlugin.jslib";
            string callerPath = pluginsDirectory + "/WebGLCaller.cs";
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }
            if (!File.Exists(pluginPath))
            {
                TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/WebGLPluginTemplate.txt", typeof(TextAsset)) as TextAsset;
                if (asset)
                {
                    File.AppendAllText(pluginPath, asset.text);
                    asset = null;
                }
            }
            if (!File.Exists(callerPath))
            {
                TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Editor/Utility/Template/WebGLCallerTemplate.txt", typeof(TextAsset)) as TextAsset;
                if (asset)
                {
                    File.AppendAllText(callerPath, asset.text);
                    asset = null;
                }
            }

            AssetDatabase.Refresh();
            TextAsset plugin = AssetDatabase.LoadAssetAtPath("Assets/Plugins/WebGLCaller.cs", typeof(TextAsset)) as TextAsset;
            EditorGUIUtility.PingObject(plugin);
            Selection.activeObject = plugin;
        }
        #endregion

        #region 反射工具
        /// <summary>
        /// 当前的热更新程序集
        /// </summary>
        private static readonly HashSet<string> HotfixAssemblies = new HashSet<string>() { "Hotfix", "HTFramework.ILHotfix.RunTime " };
        /// <summary>
        /// 当前的编辑器程序集
        /// </summary>
        private static readonly HashSet<string> EditorAssemblies = new HashSet<string>() { "Assembly-CSharp-Editor", "UnityEditor" };
        
        /// <summary>
        /// 从当前程序域的热更新程序集中获取所有类型
        /// </summary>
        public static List<Type> GetTypesInHotfixAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if (HotfixAssemblies.Contains(assemblys[i].GetName().Name))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前程序域的热更新程序集中获取指定类型
        /// </summary>
        public static Type GetTypeInHotfixAssemblies(string typeName)
        {
            Type type = null;
            foreach (string assembly in HotfixAssemblies)
            {
                type = Type.GetType(typeName + "," + assembly);
                if (type != null)
                {
                    return type;
                }
            }
            GlobalTools.LogError("获取类型 " + typeName + " 失败！当前热更新程序集中不存在此类型！");
            return null;
        }
        
        /// <summary>
        /// 从当前程序域的编辑器程序集中获取所有类型
        /// </summary>
        public static List<Type> GetTypesInEditorAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if (EditorAssemblies.Contains(assemblys[i].GetName().Name))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前程序域的编辑器程序集中获取指定类型
        /// </summary>
        public static Type GetTypeInEditorAssemblies(string typeName)
        {
            Type type = null;
            foreach (string assembly in EditorAssemblies)
            {
                type = Type.GetType(typeName + "," + assembly);
                if (type != null)
                {
                    return type;
                }
            }
            GlobalTools.LogError("获取类型 " + typeName + " 失败！当前编辑器程序集中不存在此类型！");
            return null;
        }

        /// <summary>
        /// 从当前程序域的所有程序集中获取所有类型
        /// </summary>
        public static List<Type> GetTypesInAllAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                types.AddRange(assemblys[i].GetTypes());
            }
            return types;
        }
        #endregion

        #region 序列化工具
        /// <summary>
        /// 将对象序列化为字节数组
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>序列化后的字节数组</returns>
        public static byte[] Serialize(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 将字节数组反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="byteArray">字节数组</param>
        /// <returns>反序列化后的对象</returns>
        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(byteArray, 0, byteArray.Length);
                ms.Seek(0, SeekOrigin.Begin);
                T obj = bf.Deserialize(ms) as T;
                return obj;
            }
        }
        #endregion

        #region 数学工具
        /// <summary>
        /// Vector2转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Vector2值</param>
        /// <param name="format">格式</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Vector2 value, string format)
        {
            return GlobalTools.StringConcat(value.x.ToString(format), "f,", value.y.ToString(format), "f");
        }
        /// <summary>
        /// 标准Paste字符串转换为Vector2
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <returns>Vector2值</returns>
        public static Vector2 ToPasteVector2(this string value)
        {
            string[] vector2 = value.Split(',');
            if (vector2.Length == 2)
            {
                float x, y;
                vector2[0] = vector2[0].Replace("f", "");
                vector2[1] = vector2[1].Replace("f", "");
                if (float.TryParse(vector2[0], out x) && float.TryParse(vector2[1], out y))
                {
                    return new Vector2(x, y);
                }
            }
            return Vector2.zero;
        }
        /// <summary>
        /// Vector3转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Vector3值</param>
        /// <param name="format">格式</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Vector3 value, string format)
        {
            return GlobalTools.StringConcat(value.x.ToString(format), "f,", value.y.ToString(format), "f,", value.z.ToString(format), "f");
        }
        /// <summary>
        /// 标准Paste字符串转换为Vector3
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <returns>Vector3值</returns>
        public static Vector3 ToPasteVector3(this string value)
        {
            string[] vector3 = value.Split(',');
            if (vector3.Length == 3)
            {
                float x, y, z;
                vector3[0] = vector3[0].Replace("f", "");
                vector3[1] = vector3[1].Replace("f", "");
                vector3[2] = vector3[2].Replace("f", "");
                if (float.TryParse(vector3[0], out x) && float.TryParse(vector3[1], out y) && float.TryParse(vector3[2], out z))
                {
                    return new Vector3(x, y, z);
                }
            }
            return Vector3.zero;
        }
        /// <summary>
        /// Quaternion转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Quaternion值</param>
        /// <param name="format">格式</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Quaternion value, string format)
        {
            return GlobalTools.StringConcat(value.x.ToString(format), "f,", value.y.ToString(format), "f,", value.z.ToString(format), "f,", value.w.ToString(format), "f");
        }
        /// <summary>
        /// 标准Paste字符串转换为Quaternion
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <returns>Quaternion值</returns>
        public static Quaternion ToPasteQuaternion(this string value)
        {
            string[] quaternion = value.Split(',');
            if (quaternion.Length == 4)
            {
                float x, y, z, w;
                quaternion[0] = quaternion[0].Replace("f", "");
                quaternion[1] = quaternion[1].Replace("f", "");
                quaternion[2] = quaternion[2].Replace("f", "");
                quaternion[3] = quaternion[3].Replace("f", "");
                if (float.TryParse(quaternion[0], out x) && float.TryParse(quaternion[1], out y) && float.TryParse(quaternion[2], out z) && float.TryParse(quaternion[3], out w))
                {
                    return new Quaternion(x, y, z, w);
                }
            }
            return Quaternion.identity;
        }
        #endregion

        #region IO工具
        /// <summary>
        /// 删除文件夹及以下的所有文件夹、文件
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        public static void DeleteFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                DirectoryInfo directory = Directory.CreateDirectory(folderPath);

                FileInfo[] files = directory.GetFiles();
                foreach (var file in files)
                {
                    file.Delete();
                }

                DirectoryInfo[] folders = directory.GetDirectories();
                foreach (var folder in folders)
                {
                    DeleteFolder(folder.FullName);
                }

                directory.Delete();
            }
        }
        #endregion

        #region GUI工具
        private static HashSet<string> _noRepeatNames = new HashSet<string>();

        /// <summary>
        /// 开始不重复命名
        /// </summary>
        public static void BeginNoRepeatNaming()
        {
            _noRepeatNames.Clear();
        }
        /// <summary>
        /// 获取不重复命名（自动加工原名，以防止重复）
        /// </summary>
        /// <param name="rawName">原名</param>
        /// <returns>不重复命名</returns>
        public static string GetNoRepeatName(string rawName)
        {
            if (_noRepeatNames.Contains(rawName))
            {
                int index = 0;
                string noRepeatName = rawName + " " + index.ToString();
                while (_noRepeatNames.Contains(noRepeatName))
                {
                    index += 1;
                    noRepeatName = rawName + " " + index.ToString();
                }

                _noRepeatNames.Add(noRepeatName);
                return noRepeatName;
            }
            else
            {
                _noRepeatNames.Add(rawName);
                return rawName;
            }
        }
        #endregion

        #region Hierarchy窗口扩展
        private static GUIStyle HierarchyItemStyle;
        private static Texture HTFrameworkLOGO;

        /// <summary>
        /// 编辑器初始化
        /// </summary>
        private static void OnInitHierarchy()
        {
            HierarchyItemStyle = new GUIStyle();
            HierarchyItemStyle.alignment = TextAnchor.MiddleRight;
            HierarchyItemStyle.normal.textColor = Color.cyan;
            HTFrameworkLOGO = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/HTFrameworkLOGO.png");

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }
        /// <summary>
        /// Hierarchy窗口元素GUI
        /// </summary>
        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject main = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (main)
            {
                if (main.GetComponent<Main>())
                {
                    GUI.Box(selectionRect, HTFrameworkLOGO, HierarchyItemStyle);
                }
            }
        }
        #endregion

        #region Project窗口扩展
        private static GUIStyle ProjectItemStyle;
        private static Texture HTFrameworkLOGOTitle;

        /// <summary>
        /// 编辑器初始化
        /// </summary>
        private static void OnInitProject()
        {
            ProjectItemStyle = new GUIStyle();
            ProjectItemStyle.alignment = TextAnchor.MiddleRight;
            ProjectItemStyle.normal.textColor = Color.cyan;
            HTFrameworkLOGOTitle = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/HTFrameworkLOGOTitle.png");

            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }
        /// <summary>
        /// Project窗口元素GUI
        /// </summary>
        private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            string mainFolder = AssetDatabase.GUIDToAssetPath(guid);
            if (string.Equals(mainFolder, "Assets/HTFramework"))
            {
                GUI.Box(selectionRect, HTFrameworkLOGOTitle, ProjectItemStyle);
            }
        }
        #endregion

        #region Styles
        public static class Styles
        {
            public static readonly string Box = "Box";
            public static readonly string IconButton = "IconButton";
            public static readonly string LargeButton = "LargeButton";
            public static readonly string LargeButtonLeft = "LargeButtonLeft";
            public static readonly string LargeButtonRight = "LargeButtonRight";
            public static readonly string ButtonLeft = "ButtonLeft";
            public static readonly string ButtonMid = "ButtonMid";
            public static readonly string ButtonRight = "ButtonRight";
            public static readonly string MiniPopup = "MiniPopup";
            public static readonly string Wordwrapminibutton = "Wordwrapminibutton";
            public static readonly string OLPlus = "OL Plus";
            public static readonly string OLMinus = "OL Minus";
            public static readonly string Label = "Label";
            public static readonly string SearchTextField = "SearchTextField";
            public static readonly string SearchCancelButton = "SearchCancelButton";
            public static readonly string SearchCancelButtonEmpty = "SearchCancelButtonEmpty";
            public static readonly string ToolbarSeachTextField = "ToolbarSeachTextField";
            public static readonly string ToolbarSeachCancelButton = "ToolbarSeachCancelButton";
            public static readonly string ToolbarSeachCancelButtonEmpty = "ToolbarSeachCancelButtonEmpty";
        }
        #endregion
    }
}