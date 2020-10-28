using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
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
            OnInitLnkTools();
        }
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

        #region Batch 【优先级100】
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
        /// 【验证函数】添加边界框碰撞器
        /// </summary>
        [@MenuItem("HTFramework/Batch/Add Bounds Box Collider", true)]
        private static bool AddBoundsBoxColliderValidate()
        {
            return Selection.gameObjects.Length > 0;
        }
        /// <summary>
        /// 添加边界框碰撞器
        /// </summary>
        [@MenuItem("HTFramework/Batch/Add Bounds Box Collider", false, 120)]
        private static void AddBoundsBoxCollider()
        {
            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].GetComponent<MeshRenderer>())
                {
                    if (!objs[i].GetComponent<Collider>())
                        objs[i].AddComponent<BoxCollider>();
                    continue;
                }

                Transform trans = objs[i].transform;
                Renderer[] renders = trans.GetComponentsInChildren<Renderer>();
                if (renders.Length == 0)
                    continue;

                Vector3 postion = trans.position;
                Quaternion rotation = trans.rotation;
                Vector3 scale = trans.localScale;
                trans.position = Vector3.zero;
                trans.rotation = Quaternion.identity;
                trans.localScale = Vector3.one;

                Collider[] colliders = trans.GetComponents<Collider>();
                foreach (Collider collider in colliders)
                {
                    UnityEngine.Object.DestroyImmediate(collider);
                }

                Vector3 center = Vector3.zero;
                foreach (Renderer child in renders)
                {
                    center += child.bounds.center;
                }
                center /= renders.Length;

                Bounds bounds = new Bounds(center, Vector3.zero);
                foreach (Renderer child in renders)
                {
                    bounds.Encapsulate(child.bounds);
                }

                BoxCollider boxCollider = trans.gameObject.AddComponent<BoxCollider>();
                boxCollider.center = bounds.center - trans.position;
                boxCollider.size = bounds.size;

                trans.position = postion;
                trans.rotation = rotation;
                trans.localScale = scale;
            }
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
        [@MenuItem("HTFramework/Batch/Set Mouse Ray Target", false, 121)]
        private static void SetMouseRayTarget()
        {
            AddBoundsBoxCollider();

            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                Collider collider = objs[i].GetComponent<Collider>();
                if (!collider) collider = objs[i].AddComponent<BoxCollider>();
                collider.isTrigger = true;

                MouseRayTarget rayTarget = objs[i].GetComponent<MouseRayTarget>();
                if (!rayTarget) rayTarget = objs[i].AddComponent<MouseRayTarget>();
                rayTarget.Name = objs[i].name;
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
        [@MenuItem("HTFramework/Batch/Set Mouse Ray UI Target", false, 122)]
        private static void SetMouseRayUITarget()
        {
            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                Graphic graphic = objs[i].GetComponent<Graphic>();
                if (!graphic)
                {
                    Log.Warning("对象 " + objs[i].name + " 没有Graphic组件，无法做为可捕获UI目标！");
                    continue;
                }
                graphic.raycastTarget = true;

                MouseRayUITarget rayUITarget = objs[i].GetComponent<MouseRayUITarget>();
                if (!rayUITarget) rayUITarget = objs[i].AddComponent<MouseRayUITarget>();
                rayUITarget.Name = objs[i].name;
            }
        }
        #endregion

        #region Console 【优先级101】
        /// <summary>
        /// 清理控制台
        /// </summary>
        [@MenuItem("HTFramework/Console/Clear &1", false, 101)]
        private static void ClearConsole()
        {
            Type logEntries = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.LogEntries");
            MethodInfo clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        /// <summary>
        /// 打印普通日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug Log", false, 102)]
        private static void ConsoleDebugLog()
        {
            Log.Info("Debug.Log!");
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug LogWarning", false, 103)]
        private static void ConsoleDebugLogWarning()
        {
            Log.Warning("Debug.LogWarning!");
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug LogError", false, 104)]
        private static void ConsoleDebugLogError()
        {
            Log.Error("Debug.LogError!");
        }
        #endregion

        #region Editor 【优先级102】
        /// <summary>
        /// 运行场景
        /// </summary>
        [@MenuItem("HTFramework/Editor/Run &2", false, 102)]
        private static void RunScene()
        {
            EditorApplication.isPlaying = !EditorApplication.isPlaying;
        }
        
        /// <summary>
        /// 打开编辑器安装路径
        /// </summary>
        [@MenuItem("HTFramework/Editor/Open Installation Path", false, 103)]
        private static void OpenInstallationPath()
        {
            string path = EditorApplication.applicationPath.Substring(0, EditorApplication.applicationPath.LastIndexOf("/"));
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(path);
            System.Diagnostics.Process.Start(psi);
        }
        #endregion

        #region ECS 【优先级103】
        /// <summary>
        /// 标记目标为ECS系统的实体
        /// </summary>
        [@MenuItem("HTFramework/ECS/Mark As To Entity", false, 103)]
        private static void MarkAsToEntity()
        {
            int index = 0;
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                ECS_Entity.CreateEntity(Selection.gameObjects[i]);
                index += 1;
            }
            Log.Info("已完成ECS实体标记 " + index + " 个！");
        }

        /// <summary>
        /// 打开ECS系统检视器
        /// </summary>
        [@MenuItem("HTFramework/ECS/Inspector", false, 104)]
        private static void OpenECSInspector()
        {
            ECS_Inspector inspector = EditorWindow.GetWindow<ECS_Inspector>();
            inspector.titleContent.image = EditorGUIUtility.IconContent("Grid.BoxTool").image;
            inspector.titleContent.text = "ECS Inspector";
            inspector.Show();
        }
        #endregion

        #region Tools 【优先级104】
        /// <summary>
        /// 合并多个模型网格
        /// </summary>
        [@MenuItem("HTFramework/Tools/Mesh/Mesh Combines", false, 104)]
        private static void MeshCombines()
        {
            if (Selection.gameObjects.Length <= 1)
            {
                Log.Warning("请先选中至少2个以上的待合并网格！");
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
        [@MenuItem("HTFramework/Tools/Mesh/Mesh Info", false, 105)]
        private static void ShowMeshInfo()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                MeshFilter filter = Selection.gameObjects[i].GetComponent<MeshFilter>();
                if (filter)
                {
                    Log.Info("Mesh [" + filter.name + "] : vertices " + filter.sharedMesh.vertexCount + ", triangles " + filter.sharedMesh.triangles.Length);
                }
            }
        }

        /// <summary>
        /// 打开 Assets Master
        /// </summary>
        [@MenuItem("HTFramework/Tools/Assets Master", false, 106)]
        private static void OpenAssetsMaster()
        {
            AssetsMaster master = EditorWindow.GetWindow<AssetsMaster>();
            master.titleContent.image = EditorGUIUtility.IconContent("d_WelcomeScreen.AssetStoreLogo").image;
            master.titleContent.text = "Assets Master";
            master.minSize = new Vector2(1000, 600);
            master.SearchAssetsInOpenedScene();
            master.Show();
        }

        /// <summary>
        /// 打开 Assembly Viewer
        /// </summary>
        [@MenuItem("HTFramework/Tools/Assembly Viewer", false, 107)]
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
        [@MenuItem("HTFramework/Tools/Custom Executer", false, 108)]
        private static void OpenCustomExecuter()
        {
            CustomExecuter tools = EditorWindow.GetWindow<CustomExecuter>();
            tools.titleContent.image = EditorGUIUtility.IconContent("LightProbeProxyVolume Icon").image;
            tools.titleContent.text = "Custom Executer";
            tools.minSize = new Vector2(500, 600);
            tools.Initialization();
            tools.Show();
        }

        private static List<MethodInfo> CustomTools = new List<MethodInfo>();

        /// <summary>
        /// 执行 Custom Tool
        /// </summary>
        [@MenuItem("HTFramework/Tools/Custom Tool", false, 109)]
        private static void ExecuteCustomTool()
        {
            CustomTools.Clear();
            List<Type> types = EditorReflectionToolkit.GetTypesInEditorAssemblies();
            for (int i = 0; i < types.Count; i++)
            {
                MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < methods.Length; j++)
                {
                    if (methods[j].IsDefined(typeof(CustomToolAttribute), false))
                    {
                        CustomTools.Add(methods[j]);
                    }
                }
            }
            if (CustomTools.Count <= 0)
            {
                Log.Warning("当前不存在至少一个自定义工具！为任何处于 Editor 文件夹中的类的无参静态函数添加 CustomTool 特性，可将该函数附加至自定义工具菜单！");
            }
            else
            {
                for (int i = 0; i < CustomTools.Count; i++)
                {
                    CustomTools[i].Invoke(null, null);
                }
                Log.Info("已执行 " + CustomTools.Count + " 个自定义工具！");
                CustomTools.Clear();
            }
        }

        /// <summary>
        /// 打开 Extended Inspector
        /// </summary>
        [@MenuItem("HTFramework/Tools/Extended Inspector", false, 110)]
        private static void OpenExtendedInspector()
        {
            ExtendedInspectorWindow window = EditorWindow.GetWindow<ExtendedInspectorWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("d_UnityEditor.InspectorWindow").image;
            window.titleContent.text = "Extended Inspector";
            window.Show();
        }
        #endregion

        #region ProjectWizard 【优先级1000】
        /// <summary>
        /// ProjectWizard
        /// </summary>
        [@MenuItem("HTFramework/Project Wizard", false, 1000)]
        private static void ProjectWizard()
        {
            ProjectWizard wizard = EditorWindow.GetWindow<ProjectWizard>();
            wizard.titleContent.image = EditorGUIUtility.IconContent("SocialNetworks.UDNLogo").image;
            wizard.titleContent.text = "Project Wizard";
            wizard.position = new Rect(200, 200, 600, 500);
            wizard.Show();
        }
        #endregion

        #region HTFramework Setting... 【优先级1001】
        /// <summary>
        /// HTFramework Setting...
        /// </summary>
        [@MenuItem("HTFramework/HTFramework Settings...", false, 1001)]
        private static void OpenHTFrameworkSettings()
        {
            Setter setter = EditorWindow.GetWindow<Setter>();
            setter.titleContent.image = EditorGUIUtility.IconContent("SettingsIcon").image;
            setter.titleContent.text = "HTFramework Setter";
            setter.minSize = new Vector2(640, 580);
            setter.Show();
        }
        #endregion

        #region 层级视图新建菜单
        /// <summary>
        /// 【验证函数】新建框架主环境
        /// </summary>
        [@MenuItem("GameObject/HTFramework/Main Environment", true)]
        private static bool CreateMainValidate()
        {
            return UnityEngine.Object.FindObjectOfType<Main>() == null;
        }
        /// <summary>
        /// 新建框架主环境
        /// </summary>
        [@MenuItem("GameObject/HTFramework/Main Environment", false, 0)]
        private static void CreateMain()
        {
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/HTFramework/HTFramework.prefab");
            if (asset)
            {
                GameObject main = PrefabUtility.InstantiatePrefab(asset) as GameObject;
                main.name = "HTFramework";
                main.transform.localPosition = Vector3.zero;
                main.transform.localRotation = Quaternion.identity;
                main.transform.localScale = Vector3.one;
                Selection.activeGameObject = main;
                EditorSceneManager.MarkSceneDirty(main.scene);
            }
            else
            {
                Log.Error("新建框架主环境失败，丢失主预制体：Assets/HTFramework/HTFramework.prefab");
            }
        }

        /// <summary>
        /// 新建FSM
        /// </summary>
        [@MenuItem("GameObject/HTFramework/FSM", false, 1)]
        private static void CreateFSM()
        {
            GameObject fsm = new GameObject();
            fsm.name = "New FSM";
            fsm.transform.localPosition = Vector3.zero;
            fsm.transform.localRotation = Quaternion.identity;
            fsm.transform.localScale = Vector3.one;
            fsm.AddComponent<FSM>();
            Selection.activeGameObject = fsm;
            EditorSceneManager.MarkSceneDirty(fsm.scene);
        }
        #endregion

        #region 工程视图新建菜单
        /// <summary>
        /// 新建AspectProxy类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# AspectProxy Script", false, 11)]
        private static void CreateAspectProxy()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_AspectProxy_Folder, "AspectProxy", "AspectProxyTemplate");
        }

        /// <summary>
        /// 新建CustomModule类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# CustomModule Script", false, 12)]
        private static void CreateCustomModule()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_CustomModule_Folder, "CustomModule", "CustomModuleTemplate", "#MODULENAME#");
        }

        /// <summary>
        /// 新建DataSet类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# DataSet Script", false, 13)]
        private static void CreateDataSet()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_DataSet_Folder, "DataSet", "DataSetTemplate");
        }

        /// <summary>
        /// 新建EntityLogic类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# EntityLogic Script", false, 14)]
        private static void CreateEntityLogic()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_EntityLogic_Folder, "EntityLogic", "EntityLogicTemplate");
        }

        /// <summary>
        /// 新建EventHandler类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# EventHandler Script", false, 15)]
        private static void CreateEventHandler()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_EventHandler_Folder, "EventHandler", "EventHandlerTemplate");
        }

        /// <summary>
        /// 新建FiniteState类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# FiniteState Script", false, 16)]
        private static void CreateFiniteState()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_FiniteState_Folder, "FiniteState", "FiniteStateTemplate", "#STATENAME#");
        }
        
        /// <summary>
        /// 新建Procedure类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# Procedure Script", false, 17)]
        private static void CreateProcedure()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_Procedure_Folder, "Procedure", "ProcedureTemplate");
        }

        /// <summary>
        /// 新建ProtocolChannel类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# Protocol Channel Script", false, 18)]
        private static void CreateProtocolChannel()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_ProtocolChannel_Folder, "ProtocolChannel", "ProtocolChannelTemplate");
        }
        
        /// <summary>
        /// 新建UILogicResident类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# UILogicResident Script", false, 19)]
        private static void CreateUILogicResident()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_UILogicResident_Folder, "UILogicResident", "UILogicResidentTemplate");
        }

        /// <summary>
        /// 新建UILogicTemporary类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# UILogicTemporary Script", false, 20)]
        private static void CreateUILogicTemporary()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_UILogicTemporary_Folder, "UILogicTemporary", "UILogicTemporaryTemplate");
        }

        /// <summary>
        /// 新建ECS的组件类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/[ECS] C# Component Script", false, 1000)]
        private static void CreateECSComponent()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_ECSComponent_Folder, "ECSComponent", "ECSComponentTemplate");
        }

        /// <summary>
        /// 新建ECS的系统类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/[ECS] C# System Script", false, 1001)]
        private static void CreateECSSystem()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_ECSSystem_Folder, "ECSSystem", "ECSSystemTemplate");
        }

        /// <summary>
        /// 新建ECS的指令类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/[ECS] C# Order Script", false, 1002)]
        private static void CreateECSOrder()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_ECSOrder_Folder, "ECSOrder", "ECSOrderTemplate");
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
        [@MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixProcedure Script", false, 2000)]
        private static void CreateHotfixProcedure()
        {
            EditorPrefs.SetString(EditorPrefsTable.Script_HotfixProcedure_Folder, "/Hotfix");
            CreateScriptFormTemplate(EditorPrefsTable.Script_HotfixProcedure_Folder, "HotfixProcedure", "HotfixProcedureTemplate");
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
        [@MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixObject Script", false, 2001)]
        private static void CreateHotfixObject()
        {
            EditorPrefs.SetString(EditorPrefsTable.Script_HotfixObject_Folder, "/Hotfix");
            CreateScriptFormTemplate(EditorPrefsTable.Script_HotfixObject_Folder, "HotfixObject", "HotfixObjectTemplate");
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
        [@MenuItem("Assets/Create/HTFramework/WebGL Plugin", false, 3000)]
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
                TextAsset asset = AssetDatabase.LoadAssetAtPath(EditorPrefsTable.ScriptTemplateFolder + "WebGLPluginTemplate.txt", typeof(TextAsset)) as TextAsset;
                if (asset)
                {
                    File.AppendAllText(pluginPath, asset.text);
                    asset = null;
                }
            }
            if (!File.Exists(callerPath))
            {
                TextAsset asset = AssetDatabase.LoadAssetAtPath(EditorPrefsTable.ScriptTemplateFolder + "WebGLCallerTemplate.txt", typeof(TextAsset)) as TextAsset;
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

        /// <summary>
        /// 从模板创建脚本
        /// </summary>
        /// <param name="prefsKey">脚本配置路径Key</param>
        /// <param name="scriptType">脚本类型</param>
        /// <param name="templateName">脚本模板名称</param>
        /// <param name="replace">脚本替换字段</param>
        /// <returns>脚本名称</returns>
        public static string CreateScriptFormTemplate(string prefsKey, string scriptType,string templateName, params string[] replace)
        {
            string directory = EditorPrefs.GetString(prefsKey, "");
            string fullPath = Application.dataPath + directory;
            if (!Directory.Exists(fullPath)) fullPath = Application.dataPath;

            string path = EditorUtility.SaveFilePanel("Create " + scriptType + " Class", fullPath, "New" + scriptType, "cs");
            if (!string.IsNullOrEmpty(path))
            {
                if (!path.Contains(Application.dataPath))
                {
                    Log.Error("新建 " + scriptType + " 失败：创建路径必须在当前项目的 Assets 路径下！");
                    return "<None>";
                }

                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath(EditorPrefsTable.ScriptTemplateFolder + templateName + ".txt", typeof(TextAsset)) as TextAsset;
                    if (asset)
                    {
                        string code = asset.text;
                        code = code.Replace("#SCRIPTNAME#", className);
                        if (replace != null && replace.Length > 0)
                        {
                            for (int i = 0; i < replace.Length; i++)
                            {
                                code = code.Replace(replace[i], className);
                            }
                        }
                        File.AppendAllText(path, code);
                        asset = null;
                        AssetDatabase.Refresh();

                        string assetPath = path.Substring(path.LastIndexOf("Assets"));
                        TextAsset csFile = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                        EditorGUIUtility.PingObject(csFile);
                        Selection.activeObject = csFile;
                        AssetDatabase.OpenAsset(csFile);
                        EditorPrefs.SetString(prefsKey, path.Substring(0, path.LastIndexOf("/")).Replace(Application.dataPath, ""));
                        return className;
                    }
                    else
                    {
                        Log.Error("新建 " + scriptType + " 失败：丢失脚本模板文件！");
                    }
                }
                else
                {
                    Log.Error("新建 " + scriptType + " 失败：已存在类文件 " + className);
                }
            }
            return "<None>";
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
            return StringToolkit.Concat(value.x.ToString(format), "f,", value.y.ToString(format), "f");
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
            return StringToolkit.Concat(value.x.ToString(format), "f,", value.y.ToString(format), "f,", value.z.ToString(format), "f");
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
            return StringToolkit.Concat(value.x.ToString(format), "f,", value.y.ToString(format), "f,", value.z.ToString(format), "f,", value.w.ToString(format), "f");
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

        #region 编辑器工具
        /// <summary>
        /// 强制编辑器进行编译
        /// </summary>
        public static void CoerciveCompile()
        {
            MonoScript monoScript = MonoImporter.GetAllRuntimeMonoScripts()[0];
            int order = MonoImporter.GetExecutionOrder(monoScript);
            MonoImporter.SetExecutionOrder(monoScript, order);
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
                DirectoryInfo directory = new DirectoryInfo(folderPath);

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

        #region LnkTools
        private static bool IsEnableLnkTools = false;
        private static bool IsExpansionLnkTools = false;
        private static List<LnkTools> LnkToolss = new List<LnkTools>();

        /// <summary>
        /// LnkTools初始化
        /// </summary>
        private static void OnInitLnkTools()
        {
            IsEnableLnkTools = EditorPrefs.GetBool(EditorPrefsTable.LnkTools_Enable, false);
            IsExpansionLnkTools = EditorPrefs.GetBool(EditorPrefsTable.LnkTools_Expansion, false);

            if (IsEnableLnkTools)
            {
                LnkToolss.Clear();
                List<Type> types = EditorReflectionToolkit.GetTypesInEditorAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    for (int j = 0; j < methods.Length; j++)
                    {
                        if (methods[j].IsDefined(typeof(LnkToolsAttribute), false))
                        {
                            LnkToolsAttribute attribute = methods[j].GetCustomAttribute<LnkToolsAttribute>();
                            LnkTools lnkTools = new LnkTools(attribute.Tooltip, attribute.Priority, methods[j]);
                            LnkToolss.Add(lnkTools);
                        }
                    }
                }

                LnkToolss.Sort((x, y) =>
                {
                    if (x.Priority < y.Priority) return -1;
                    else if (x.Priority == y.Priority) return 0;
                    else return 1;
                });

                SceneView.onSceneGUIDelegate += OnLnkToolsGUI;
            }
        }
        /// <summary>
        /// LnkTools界面
        /// </summary>
        private static void OnLnkToolsGUI(SceneView sceneView)
        {
            Handles.BeginGUI();

            Rect rect = Rect.zero;
            int h = sceneView.in2DMode ? 5 : 120;

            if (IsExpansionLnkTools)
            {
                rect.Set(sceneView.position.width - 115, h, 110, (LnkToolss.Count + 1) * 22 + 8);
                GUI.Box(rect, "");
            }

            rect.Set(sceneView.position.width - 110, h + 5, 100, 20);
            bool expansion = GUI.Toggle(rect, IsExpansionLnkTools, "LnkTools", "Prebutton");
            if (expansion != IsExpansionLnkTools)
            {
                IsExpansionLnkTools = expansion;
                EditorPrefs.SetBool(EditorPrefsTable.LnkTools_Expansion, IsExpansionLnkTools);
            }
            rect.y += 22;

            if (IsExpansionLnkTools)
            {
                for (int i = 0; i < LnkToolss.Count; i++)
                {
                    if (GUI.Button(rect, LnkToolss[i].Tooltip))
                    {
                        LnkToolss[i].Method.Invoke(null, null);
                    }
                    rect.y += 22;
                }
            }

            Handles.EndGUI();
        }
        
        /// <summary>
        /// LnkTools，看向指定目标
        /// </summary>
        [LnkTools("Look At")]
        private static void LookAt()
        {
            if (EditorApplication.isPlaying && Main.m_Controller != null)
            {
                if (Selection.activeGameObject != null)
                {
                    Main.m_Controller.SetLookPoint(Selection.activeGameObject.transform.position);
                }
                else
                {
                    Log.Warning("请选中一个LookAt的目标！");
                }
            }
            else
            {
                Log.Warning("仅在运行时才能调用框架的Controller模块LookAt至选中目标！");
            }
        }

        private class LnkTools
        {
            public string Tooltip;
            public int Priority;
            public MethodInfo Method;

            public LnkTools(string tooltip, int priority, MethodInfo method)
            {
                Tooltip = tooltip;
                Priority = priority;
                Method = method;
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
            public static readonly string LargeButtonMid = "LargeButtonMid";
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