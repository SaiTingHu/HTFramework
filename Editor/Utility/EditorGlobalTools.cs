using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
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
        }
        #endregion

        #region About 【优先级0】
        /// <summary>
        /// About
        /// </summary>
        [MenuItem("HTFramework/About", false, 0)]
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
        [MenuItem("HTFramework/Batch/Component Batch", false, 100)]
        private static void OpenComponentBatch()
        {
            ComponentBatch cb = EditorWindow.GetWindow<ComponentBatch>();
            cb.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            cb.titleContent.text = "Component Batch";
            cb.position = new Rect(200, 200, 300, 165);
            cb.Show();
        }

        /// <summary>
        /// 打开ProjectBatch窗口
        /// </summary>
        [MenuItem("HTFramework/Batch/Project Batch", false, 101)]
        private static void OpenProjectBatch()
        {
            ProjectBatch pb = EditorWindow.GetWindow<ProjectBatch>();
            pb.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            pb.titleContent.text = "Project Batch";
            pb.position = new Rect(200, 200, 300, 145);
            pb.Show();
        }

        /// <summary>
        /// 【验证函数】添加边界框碰撞器
        /// </summary>
        [MenuItem("HTFramework/Batch/Add Bounds Box Collider", true)]
        private static bool AddBoundsBoxColliderValidate()
        {
            return Selection.gameObjects.Length > 0;
        }
        /// <summary>
        /// 添加边界框碰撞器
        /// </summary>
        [MenuItem("HTFramework/Batch/Add Bounds Box Collider", false, 120)]
        private static void AddBoundsBoxCollider()
        {
            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i].GetComponent<Renderer>())
                {
                    if (!objs[i].GetComponent<Collider>())
                    {
                        Undo.AddComponent<BoxCollider>(objs[i]);
                        EditorUtility.SetDirty(objs[i]);
                    }
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
                    Undo.DestroyObjectImmediate(collider);
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

                BoxCollider boxCollider = Undo.AddComponent<BoxCollider>(trans.gameObject);
                boxCollider.center = bounds.center - trans.position;
                boxCollider.size = bounds.size;

                trans.position = postion;
                trans.rotation = rotation;
                trans.localScale = scale;

                EditorUtility.SetDirty(trans.gameObject);
            }
        }

        /// <summary>
        /// 【验证函数】设置鼠标射线可捕获物体目标
        /// </summary>
        [MenuItem("HTFramework/Batch/Set Mouse Ray Target", true)]
        private static bool SetMouseRayTargetValidate()
        {
            return Selection.gameObjects.Length > 0;
        }
        /// <summary>
        /// 设置鼠标射线可捕获物体目标
        /// </summary>
        [MenuItem("HTFramework/Batch/Set Mouse Ray Target", false, 121)]
        private static void SetMouseRayTarget()
        {
            AddBoundsBoxCollider();

            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                Collider collider = objs[i].GetComponent<Collider>();
                if (collider)
                {
                    collider.isTrigger = true;

                    MouseRayTarget rayTarget = objs[i].GetComponent<MouseRayTarget>();
                    if (!rayTarget) rayTarget = Undo.AddComponent<MouseRayTarget>(objs[i]);
                    rayTarget.Name = objs[i].name;

                    EditorUtility.SetDirty(objs[i]);
                }
            }
        }

        /// <summary>
        /// 【验证函数】设置鼠标射线可捕获UI目标
        /// </summary>
        [MenuItem("HTFramework/Batch/Set Mouse Ray UI Target", true)]
        private static bool SetMouseRayUITargetValidate()
        {
            return Selection.gameObjects.Length > 0;
        }
        /// <summary>
        /// 设置鼠标射线可捕获UI目标
        /// </summary>
        [MenuItem("HTFramework/Batch/Set Mouse Ray UI Target", false, 122)]
        private static void SetMouseRayUITarget()
        {
            GameObject[] objs = Selection.gameObjects;
            for (int i = 0; i < objs.Length; i++)
            {
                Graphic graphic = objs[i].GetComponent<Graphic>();
                if (graphic)
                {
                    graphic.raycastTarget = true;

                    MouseRayUITarget rayUITarget = objs[i].GetComponent<MouseRayUITarget>();
                    if (!rayUITarget) rayUITarget = Undo.AddComponent<MouseRayUITarget>(objs[i]);
                    rayUITarget.Name = objs[i].name;

                    EditorUtility.SetDirty(objs[i]);
                }
                else
                {
                    Log.Warning($"对象 {objs[i].name} 没有Graphic组件，无法做为可捕获UI目标！");
                }
            }
        }
        #endregion

        #region Console 【优先级101】
        /// <summary>
        /// 清理控制台
        /// </summary>
        [MenuItem("HTFramework/Console/Clear &1", false, 101)]
        private static void ClearConsole()
        {
            Type logEntries = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.LogEntries");
            MethodInfo clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        /// <summary>
        /// 打印普通日志
        /// </summary>
        [MenuItem("HTFramework/Console/Debug Log", false, 102)]
        private static void ConsoleDebugLog()
        {
            Log.Info("Debug.Log!");
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        [MenuItem("HTFramework/Console/Debug LogWarning", false, 103)]
        private static void ConsoleDebugLogWarning()
        {
            Log.Warning("Debug.LogWarning!");
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        [MenuItem("HTFramework/Console/Debug LogError", false, 104)]
        private static void ConsoleDebugLogError()
        {
            Log.Error("Debug.LogError!");
        }
        #endregion

        #region Editor 【优先级102】
        /// <summary>
        /// 打开编辑器安装路径
        /// </summary>
        [MenuItem("HTFramework/Editor/Open Installation Path", false, 102)]
        private static void OpenInstallationPath()
        {
            string path = EditorApplication.applicationPath.Substring(0, EditorApplication.applicationPath.LastIndexOf("/"));
            ProcessStartInfo psi = new ProcessStartInfo(path);
            Process.Start(psi);
        }

        /// <summary>
        /// 打开DataPath文件夹
        /// </summary>
        [MenuItem("HTFramework/Editor/Open DataPath Folder", false, 103)]
        private static void OpenDataPathFolder()
        {
            string path = Application.dataPath;
            ProcessStartInfo psi = new ProcessStartInfo(path);
            Process.Start(psi);
        }

        /// <summary>
        /// 打开StreamingAssets文件夹
        /// </summary>
        [MenuItem("HTFramework/Editor/Open StreamingAssets Folder", false, 104)]
        private static void OpenStreamingAssetsFolder()
        {
            string path = Application.streamingAssetsPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
            ProcessStartInfo psi = new ProcessStartInfo(path);
            Process.Start(psi);
        }

        /// <summary>
        /// 打开PersistentData文件夹
        /// </summary>
        [MenuItem("HTFramework/Editor/Open PersistentData Folder", false, 105)]
        private static void OpenPersistentDataFolder()
        {
            string path = Application.persistentDataPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            ProcessStartInfo psi = new ProcessStartInfo(path);
            Process.Start(psi);
        }
        #endregion

        #region ECS 【优先级103】
        /// <summary>
        /// 标记目标为ECS系统的实体
        /// </summary>
        [MenuItem("HTFramework/ECS/Mark As To Entity", false, 103)]
        private static void MarkAsToEntity()
        {
            int index = 0;
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                ECS_Entity.CreateEntity(Selection.gameObjects[i]);
                EditorUtility.SetDirty(Selection.gameObjects[i]);
                index += 1;
            }
            Log.Info($"已完成ECS实体标记 {index} 个！");
        }

        /// <summary>
        /// 打开ECS系统检视器
        /// </summary>
        [MenuItem("HTFramework/ECS/Inspector", false, 104)]
        private static void OpenECSInspector()
        {
            ECS_Inspector inspector = EditorWindow.GetWindow<ECS_Inspector>();
            inspector.titleContent.image = EditorGUIUtility.IconContent("Grid.BoxTool").image;
            inspector.titleContent.text = "ECS Inspector";
            inspector.Show();
        }
        #endregion

        #region Naming 【优先级104】
        /// <summary>
        /// 标准化命名
        /// </summary>
        [MenuItem("HTFramework/Naming/Standardizing Naming &2", false, 104)]
        private static void StandardizingNaming()
        {
            if (Selection.objects.Length <= 0)
                return;

            string path = EditorPrefs.GetString(EditorPrefsTable.StandardizingNaming_Config, null);
            if (string.IsNullOrEmpty(path))
            {
                Log.Error("请先设置标准化命名配置数据集，才能使用标准化命名！");
                OpenStandardizingNamingWindow();
                return;
            }
            StandardizingNamingData data = AssetDatabase.LoadAssetAtPath<StandardizingNamingData>(path);
            if (data == null)
            {
                Log.Error("请先设置标准化命名配置数据集，才能使用标准化命名！");
                OpenStandardizingNamingWindow();
                return;
            }

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                if (Selection.objects[i] is GameObject)
                {
                    GameObject obj = Selection.objects[i] as GameObject;
                    string objPath = AssetDatabase.GetAssetPath(obj);
                    if (string.IsNullOrEmpty(objPath))
                    {
                        HierarchyElementNaming(data, obj);
                    }
                }
                else
                {
                    ProjectElementNaming(data, Selection.objects[i]);
                }
            }
        }
        private static void HierarchyElementNaming(StandardizingNamingData data, GameObject obj)
        {
            for (int i = 0; i < data.HierarchyNamingSigns.Count; i++)
            {
                NamingSign namingSign = data.HierarchyNamingSigns[i];
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(namingSign.FullName);
                if (type != null && obj.GetComponent(type))
                {
                    string newName = namingSign.Sign.Replace(data.NameMatch, obj.name);
                    obj.name = newName;
                    EditorUtility.SetDirty(obj);
                    break;
                }
            }
        }
        private static void ProjectElementNaming(StandardizingNamingData data, UnityEngine.Object obj)
        {
            for (int i = 0; i < data.ProjectNamingSigns.Count; i++)
            {
                NamingSign namingSign = data.ProjectNamingSigns[i];
                if (obj.GetType().FullName == namingSign.FullName)
                {
                    string newName = namingSign.Sign.Replace(data.NameMatch, obj.name);
                    string oldPath = AssetDatabase.GetAssetPath(obj);
                    AssetDatabase.RenameAsset(oldPath, newName);
                    EditorUtility.SetDirty(obj);
                    break;
                }
            }
        }
        /// <summary>
        /// 打开标准化命名配置窗口
        /// </summary>
        [MenuItem("HTFramework/Naming/Standardizing Naming Config", false, 105)]
        private static void OpenStandardizingNamingWindow()
        {
            StandardizingNamingWindow window = EditorWindow.GetWindow<StandardizingNamingWindow>();
            window.titleContent.image = EditorGUIUtility.IconContent("d_editicon.sml").image;
            window.titleContent.text = "Standardizing Naming";
            window.minSize = new Vector2(300, 80);
            window.maxSize = new Vector2(300, 80);
            window.Show();
        }
        #endregion

        #region Tools 【优先级105】
        /// <summary>
        /// 打开 Assets Master
        /// </summary>
        [MenuItem("HTFramework/Tools/Assets Master", false, 105)]
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
        [MenuItem("HTFramework/Tools/Assembly Viewer", false, 106)]
        private static void OpenAssemblyViewer()
        {
            AssemblyViewer viewer = EditorWindow.GetWindow<AssemblyViewer>();
            viewer.titleContent.image = EditorGUIUtility.IconContent("Assembly Icon").image;
            viewer.titleContent.text = "Assembly Viewer";
            viewer.Show();
        }

        /// <summary>
        /// 打开 Code Snippet Executer
        /// </summary>
        [MenuItem("HTFramework/Tools/Code Snippet Executer", false, 107)]
        private static void OpenCodeSnippetExecuter()
        {
            CodeSnippetExecuter executer = EditorWindow.GetWindow<CodeSnippetExecuter>();
            executer.titleContent.image = EditorGUIUtility.IconContent("LightProbeProxyVolume Icon").image;
            executer.titleContent.text = "Code Snippet Executer";
            executer.minSize = new Vector2(500, 600);
            executer.Initialization();
            executer.Show();
        }

        /// <summary>
        /// 合并多个静态网格
        /// </summary>
        [MenuItem("HTFramework/Tools/Mesh Combines", false, 108)]
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
                EditorUtility.DisplayProgressBar("合并网格", $"正在合并网格及纹理......（{i}/{objs.Length}）", ((float)i) / objs.Length);

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
        /// 截取蒙皮网格当前帧，保存为静态网格
        /// </summary>
        [MenuItem("HTFramework/Tools/SkinnedMesh Bake", false, 109)]
        private static void SkinnedMeshBake()
        {
            if (Selection.gameObjects.Length != 1)
            {
                Log.Warning("请先选中1个待截取静态网格的蒙皮网格！");
                return;
            }

            SkinnedMeshRenderer skinnedMeshRenderer = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer == null)
            {
                Log.Warning("请先选中1个待截取静态网格的蒙皮网格！");
                return;
            }

            Mesh mesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(mesh);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            AssetDatabase.CreateAsset(mesh, $"Assets/{skinnedMeshRenderer.sharedMesh.name}.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = mesh;
            EditorGUIUtility.PingObject(mesh);
        }

        /// <summary>
        /// 通过 Tag 搜索物体
        /// </summary>
        [MenuItem("HTFramework/Tools/Search GameObject By Tag", false, priority = 121)]
        private static void SearchByTag()
        {
            SearchByTagToolkit sw = EditorWindow.GetWindow<SearchByTagToolkit>();
            sw.titleContent.image = EditorGUIUtility.IconContent("Search On Icon").image;
            sw.titleContent.text = "Search By Tag";
            sw.Show();
        }

        /// <summary>
        /// 通过 Layer 搜索物体
        /// </summary>
        [MenuItem("HTFramework/Tools/Search GameObject By Layer", false, priority = 122)]
        private static void SearchByLayer()
        {
            SearchByLayerToolkit sw = EditorWindow.GetWindow<SearchByLayerToolkit>();
            sw.titleContent.image = EditorGUIUtility.IconContent("Search On Icon").image;
            sw.titleContent.text = "Search By Layer";
            sw.Show();
        }
        #endregion

        #region ProjectWizard 【优先级10000】
        /// <summary>
        /// ProjectWizard
        /// </summary>
        [MenuItem("HTFramework/Project Wizard", false, 10000)]
        private static void OpenProjectWizard()
        {
            ProjectWizard wizard = EditorWindow.GetWindow<ProjectWizard>();
            wizard.titleContent.image = EditorGUIUtility.IconContent("SocialNetworks.UDNLogo").image;
            wizard.titleContent.text = "Project Wizard";
            wizard.position = new Rect(200, 200, 600, 600);
            wizard.Show();
        }
        #endregion

        #region Execution Order 【优先级10001】
        /// <summary>
        /// Execution Order
        /// </summary>
        [MenuItem("HTFramework/Execution Order", false, 10001)]
        private static void OpenExecutionOrder()
        {
            ExecutionOrder window = EditorWindow.GetWindow<ExecutionOrder>();
            window.titleContent.image = EditorGUIUtility.IconContent("SortingGroup Icon").image;
            window.titleContent.text = "Execution Order";
            window.Show();
        }
        #endregion

        #region HTFramework Setting... 【优先级10002】
        /// <summary>
        /// HTFramework Setting...
        /// </summary>
        [MenuItem("HTFramework/HTFramework Settings...", false, 10002)]
        private static void OpenHTFrameworkSettings()
        {
            Setter setter = EditorWindow.GetWindow<Setter>();
            setter.titleContent.image = EditorGUIUtility.IconContent("SettingsIcon").image;
            setter.titleContent.text = "HTFramework Setter";
            setter.minSize = new Vector2(640, 580);
            setter.Show();
        }
        #endregion

        #region 层级视图新建菜单 【优先级100】
        /// <summary>
        /// 【验证函数】新建框架主环境
        /// </summary>
        [MenuItem("GameObject/HTFramework/Main Environment", true)]
        private static bool CreateMainValidate()
        {
            return UnityEngine.Object.FindObjectOfType<Main>() == null;
        }
        /// <summary>
        /// 新建框架主环境
        /// </summary>
        [MenuItem("GameObject/HTFramework/Main Environment", false, 100)]
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
                SceneVisibilityManager.instance.Hide(main, true);
                SceneVisibilityManager.instance.DisablePicking(main, true);
            }
            else
            {
                Log.Error("新建框架主环境失败，丢失主预制体：Assets/HTFramework/HTFramework.prefab");
            }
        }

        /// <summary>
        /// 新建FSM
        /// </summary>
        [MenuItem("GameObject/HTFramework/FSM", false, 101)]
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

        #region 工程视图新建菜单 【优先级100】
        /// <summary>
        /// 新建AspectProxy类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# AspectProxy Script", false, 100)]
        private static void CreateAspectProxy()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_AspectProxy_Folder, "AspectProxy", "AspectProxyTemplate");
        }

        /// <summary>
        /// 新建CustomModule类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# CustomModule Script", false, 101)]
        private static void CreateCustomModule()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_CustomModule_Folder, "CustomModule", "CustomModuleTemplate", "#MODULENAME#");
        }

        /// <summary>
        /// 新建DataSet类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# DataSet Script", false, 102)]
        private static void CreateDataSet()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_DataSet_Folder, "DataSet", "DataSetTemplate");
        }

        /// <summary>
        /// 新建EntityLogic类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# EntityLogic Script", false, 103)]
        private static void CreateEntityLogic()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_EntityLogic_Folder, "EntityLogic", "EntityLogicTemplate");
        }

        /// <summary>
        /// 新建EventHandler类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# EventHandler Script", false, 104)]
        private static void CreateEventHandler()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_EventHandler_Folder, "EventHandler", "EventHandlerTemplate");
        }

        /// <summary>
        /// 新建FiniteState类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# FiniteState Script", false, 105)]
        private static void CreateFiniteState()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_FiniteState_Folder, "FiniteState", "FiniteStateTemplate", "#STATENAME#");
        }
        
        /// <summary>
        /// 新建Procedure类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# Procedure Script", false, 106)]
        private static void CreateProcedure()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_Procedure_Folder, "Procedure", "ProcedureTemplate");
        }

        /// <summary>
        /// 新建ProtocolChannel类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# Protocol Channel Script", false, 107)]
        private static void CreateProtocolChannel()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_ProtocolChannel_Folder, "ProtocolChannel", "ProtocolChannelTemplate");
        }
        
        /// <summary>
        /// 新建UILogicResident类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# UILogicResident Script", false, 108)]
        private static void CreateUILogicResident()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_UILogicResident_Folder, "UILogicResident", "UILogicResidentTemplate");
        }

        /// <summary>
        /// 新建UILogicTemporary类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/C# UILogicTemporary Script", false, 109)]
        private static void CreateUILogicTemporary()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_UILogicTemporary_Folder, "UILogicTemporary", "UILogicTemporaryTemplate");
        }

        /// <summary>
        /// 新建ECS的组件类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/[ECS] C# Component Script", false, 120)]
        private static void CreateECSComponent()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_ECSComponent_Folder, "ECSComponent", "ECSComponentTemplate");
        }

        /// <summary>
        /// 新建ECS的系统类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/[ECS] C# System Script", false, 121)]
        private static void CreateECSSystem()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_ECSSystem_Folder, "ECSSystem", "ECSSystemTemplate");
        }

        /// <summary>
        /// 新建ECS的指令类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/[ECS] C# Order Script", false, 122)]
        private static void CreateECSOrder()
        {
            CreateScriptFormTemplate(EditorPrefsTable.Script_ECSOrder_Folder, "ECSOrder", "ECSOrderTemplate");
        }

        /// <summary>
        /// 【验证函数】新建HotfixProcedure类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixProcedure Script", true)]
        private static bool CreateHotfixProcedureValidate()
        {
            return AssetDatabase.IsValidFolder("Assets/Hotfix");
        }
        /// <summary>
        /// 新建HotfixProcedure类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixProcedure Script", false, 140)]
        private static void CreateHotfixProcedure()
        {
            EditorPrefs.SetString(EditorPrefsTable.Script_HotfixProcedure_Folder, "/Hotfix");
            CreateScriptFormTemplate(EditorPrefsTable.Script_HotfixProcedure_Folder, "HotfixProcedure", "HotfixProcedureTemplate");
        }

        /// <summary>
        /// 【验证函数】新建HotfixObject类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixObject Script", true)]
        private static bool CreateHotfixObjectValidate()
        {
            return AssetDatabase.IsValidFolder("Assets/Hotfix");
        }
        /// <summary>
        /// 新建HotfixObject类
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/[Hotfix] C# HotfixObject Script", false, 141)]
        private static void CreateHotfixObject()
        {
            EditorPrefs.SetString(EditorPrefsTable.Script_HotfixObject_Folder, "/Hotfix");
            CreateScriptFormTemplate(EditorPrefsTable.Script_HotfixObject_Folder, "HotfixObject", "HotfixObjectTemplate");
        }

        /// <summary>
        /// 【验证函数】新建WebGL插件
        /// </summary>
        [MenuItem("Assets/Create/HTFramework/WebGL Plugin", true)]
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
        [MenuItem("Assets/Create/HTFramework/WebGL Plugin", false, 160)]
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
                    File.AppendAllText(pluginPath, asset.text, Encoding.UTF8);
                    asset = null;
                }
            }
            if (!File.Exists(callerPath))
            {
                TextAsset asset = AssetDatabase.LoadAssetAtPath(EditorPrefsTable.ScriptTemplateFolder + "WebGLCallerTemplate.txt", typeof(TextAsset)) as TextAsset;
                if (asset)
                {
                    File.AppendAllText(callerPath, asset.text, Encoding.UTF8);
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
        public static string CreateScriptFormTemplate(string prefsKey, string scriptType, string templateName, params string[] replace)
        {
            string directory = EditorPrefs.GetString(prefsKey, "");
            string fullPath = Application.dataPath + directory;
            if (!Directory.Exists(fullPath)) fullPath = Application.dataPath;

            string path = EditorUtility.SaveFilePanel($"Create {scriptType} Class", fullPath, $"New{scriptType}", "cs");
            if (!string.IsNullOrEmpty(path))
            {
                if (!path.Contains(Application.dataPath))
                {
                    Log.Error($"新建 {scriptType} 失败：创建路径必须在当前项目的 Assets 路径下！");
                    return "<None>";
                }

                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath($"{EditorPrefsTable.ScriptTemplateFolder}{templateName}.txt", typeof(TextAsset)) as TextAsset;
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
                        File.AppendAllText(path, code, Encoding.UTF8);
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
                        Log.Error($"新建 {scriptType} 失败：丢失脚本模板文件！");
                    }
                }
                else
                {
                    Log.Error($"新建 {scriptType} 失败：已存在类文件 {className}！");
                }
            }
            return "<None>";
        }
        /// <summary>
        /// 从模板创建脚本
        /// </summary>
        /// <param name="prefsKey">脚本配置路径Key</param>
        /// <param name="scriptType">脚本类型</param>
        /// <param name="templateName">脚本模板名称</param>
        /// <param name="handler">自定义处理者</param>
        /// <param name="replace">脚本替换字段</param>
        /// <returns>脚本名称</returns>
        public static string CreateScriptFormTemplate(string prefsKey, string scriptType, string templateName, HTFFunc<string, string> handler, params string[] replace)
        {
            string directory = EditorPrefs.GetString(prefsKey, "");
            string fullPath = Application.dataPath + directory;
            if (!Directory.Exists(fullPath)) fullPath = Application.dataPath;

            string path = EditorUtility.SaveFilePanel($"Create {scriptType} Class", fullPath, $"New{scriptType}", "cs");
            if (!string.IsNullOrEmpty(path))
            {
                if (!path.Contains(Application.dataPath))
                {
                    Log.Error($"新建 {scriptType} 失败：创建路径必须在当前项目的 Assets 路径下！");
                    return "<None>";
                }

                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath($"{EditorPrefsTable.ScriptTemplateFolder}{templateName}.txt", typeof(TextAsset)) as TextAsset;
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
                        if (handler != null)
                        {
                            code = handler(code);
                        }
                        File.AppendAllText(path, code, Encoding.UTF8);
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
                        Log.Error($"新建 {scriptType} 失败：丢失脚本模板文件！");
                    }
                }
                else
                {
                    Log.Error($"新建 {scriptType} 失败：已存在类文件 {className}！");
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
            return $"Vector2({value.x.ToString(format)}f,{value.y.ToString(format)}f)";
        }
        /// <summary>
        /// Vector3转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Vector3值</param>
        /// <param name="format">格式</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Vector3 value, string format)
        {
            return $"Vector3({value.x.ToString(format)}f,{value.y.ToString(format)}f,{value.z.ToString(format)}f)";
        }
        /// <summary>
        /// Vector4转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Vector4值</param>
        /// <param name="format">格式</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Vector4 value, string format)
        {
            return $"Vector4({value.x.ToString(format)}f,{value.y.ToString(format)}f,{value.z.ToString(format)}f,{value.w.ToString(format)}f)";
        }
        /// <summary>
        /// Vector2Int转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Vector2Int值</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Vector2Int value)
        {
            return $"Vector2Int({value.x},{value.y})";
        }
        /// <summary>
        /// Vector3Int转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Vector3Int值</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Vector3Int value)
        {
            return $"Vector3Int({value.x},{ value.y},{value.z})";
        }
        /// <summary>
        /// Quaternion转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Quaternion值</param>
        /// <param name="format">格式</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Quaternion value, string format)
        {
            return $"Quaternion({value.x.ToString(format)}f,{value.y.ToString(format)}f,{value.z.ToString(format)}f,{value.w.ToString(format)}f)";
        }
        /// <summary>
        /// Bounds转换为标准Copy字符串
        /// </summary>
        /// <param name="value">Bounds值</param>
        /// <param name="format">格式</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this Bounds value, string format)
        {
            return string.Format("Bounds({0}f,{1}f,{2}f,{3}f,{4}f,{5}f)"
                , value.center.x.ToString(format), value.center.y.ToString(format), value.center.z.ToString(format)
                , value.size.x.ToString(format), value.size.y.ToString(format), value.size.z.ToString(format));
        }
        /// <summary>
        /// BoundsInt转换为标准Copy字符串
        /// </summary>
        /// <param name="value">BoundsInt值</param>
        /// <returns>Copy字符串</returns>
        public static string ToCopyString(this BoundsInt value)
        {
            return string.Format("BoundsInt({0},{1},{2},{3},{4},{5})"
                , value.position.x, value.position.y, value.position.z
                , value.size.x, value.size.y, value.size.z);
        }
        /// <summary>
        /// 标准Paste字符串转换为Vector2
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Vector2值</returns>
        public static Vector2 ToPasteVector2(this string value, Vector2 defaultValue = default)
        {
            if (value.StartsWith("Vector2("))
            {
                value = value.Replace("Vector2(", "");
                value = value.Replace(")", "");
                value = value.Replace("f", "");

                string[] vector2 = value.Split(',');
                if (vector2.Length == 2)
                {
                    float x, y;
                    if (float.TryParse(vector2[0], out x) && float.TryParse(vector2[1], out y))
                    {
                        return new Vector2(x, y);
                    }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 标准Paste字符串转换为Vector3
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Vector3值</returns>
        public static Vector3 ToPasteVector3(this string value, Vector3 defaultValue = default)
        {
            if (value.StartsWith("Vector3("))
            {
                value = value.Replace("Vector3(", "");
                value = value.Replace(")", "");
                value = value.Replace("f", "");

                string[] vector3 = value.Split(',');
                if (vector3.Length == 3)
                {
                    float x, y, z;
                    if (float.TryParse(vector3[0], out x) && float.TryParse(vector3[1], out y) && float.TryParse(vector3[2], out z))
                    {
                        return new Vector3(x, y, z);
                    }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 标准Paste字符串转换为Vector4
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Vector4值</returns>
        public static Vector4 ToPasteVector4(this string value, Vector4 defaultValue = default)
        {
            if (value.StartsWith("Vector4("))
            {
                value = value.Replace("Vector4(", "");
                value = value.Replace(")", "");
                value = value.Replace("f", "");

                string[] vector4 = value.Split(',');
                if (vector4.Length == 4)
                {
                    float x, y, z, w;
                    if (float.TryParse(vector4[0], out x) && float.TryParse(vector4[1], out y) && float.TryParse(vector4[2], out z) && float.TryParse(vector4[3], out w))
                    {
                        return new Vector4(x, y, z, w);
                    }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 标准Paste字符串转换为Vector2Int
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Vector2Int值</returns>
        public static Vector2Int ToPasteVector2Int(this string value, Vector2Int defaultValue = default)
        {
            if (value.StartsWith("Vector2Int("))
            {
                value = value.Replace("Vector2Int(", "");
                value = value.Replace(")", "");

                string[] vector2 = value.Split(',');
                if (vector2.Length == 2)
                {
                    int x, y;
                    if (int.TryParse(vector2[0], out x) && int.TryParse(vector2[1], out y))
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 标准Paste字符串转换为Vector3Int
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Vector3Int值</returns>
        public static Vector3Int ToPasteVector3Int(this string value, Vector3Int defaultValue = default)
        {
            if (value.StartsWith("Vector3Int("))
            {
                value = value.Replace("Vector3Int(", "");
                value = value.Replace(")", "");

                string[] vector3 = value.Split(',');
                if (vector3.Length == 3)
                {
                    int x, y, z;
                    if (int.TryParse(vector3[0], out x) && int.TryParse(vector3[1], out y) && int.TryParse(vector3[2], out z))
                    {
                        return new Vector3Int(x, y, z);
                    }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 标准Paste字符串转换为Quaternion
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Quaternion值</returns>
        public static Quaternion ToPasteQuaternion(this string value, Quaternion defaultValue = default)
        {
            if (value.StartsWith("Quaternion("))
            {
                value = value.Replace("Quaternion(", "");
                value = value.Replace(")", "");
                value = value.Replace("f", "");

                string[] quaternion = value.Split(',');
                if (quaternion.Length == 4)
                {
                    float x, y, z, w;
                    if (float.TryParse(quaternion[0], out x) && float.TryParse(quaternion[1], out y) && float.TryParse(quaternion[2], out z) && float.TryParse(quaternion[3], out w))
                    {
                        return new Quaternion(x, y, z, w);
                    }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 标准Paste字符串转换为Bounds
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>Bounds值</returns>
        public static Bounds ToPasteBounds(this string value, Bounds defaultValue = default)
        {
            if (value.StartsWith("Bounds("))
            {
                value = value.Replace("Bounds(", "");
                value = value.Replace(")", "");
                value = value.Replace("f", "");

                string[] bounds = value.Split(',');
                if (bounds.Length == 6)
                {
                    float centerX, centerY, centerZ;
                    float sizeX, sizeY, sizeZ;
                    if (float.TryParse(bounds[0], out centerX) && float.TryParse(bounds[1], out centerY) && float.TryParse(bounds[2], out centerZ)
                        && float.TryParse(bounds[3], out sizeX) && float.TryParse(bounds[4], out sizeY) && float.TryParse(bounds[5], out sizeZ))
                    {
                        return new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
                    }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 标准Paste字符串转换为BoundsInt
        /// </summary>
        /// <param name="value">Paste字符串</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns>BoundsInt值</returns>
        public static BoundsInt ToPasteBoundsInt(this string value, BoundsInt defaultValue = default)
        {
            if (value.StartsWith("BoundsInt("))
            {
                value = value.Replace("BoundsInt(", "");
                value = value.Replace(")", "");

                string[] bounds = value.Split(',');
                if (bounds.Length == 6)
                {
                    int centerX, centerY, centerZ;
                    int sizeX, sizeY, sizeZ;
                    if (int.TryParse(bounds[0], out centerX) && int.TryParse(bounds[1], out centerY) && int.TryParse(bounds[2], out centerZ)
                        && int.TryParse(bounds[3], out sizeX) && int.TryParse(bounds[4], out sizeY) && int.TryParse(bounds[5], out sizeZ))
                    {
                        return new BoundsInt(new Vector3Int(centerX, centerY, centerZ), new Vector3Int(sizeX, sizeY, sizeZ));
                    }
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 将Location转换为Json字符串
        /// </summary>
        /// <param name="location">Location对象</param>
        /// <returns>Json字符串</returns>
        public static string LocationToJson(this Location location)
        {
            if (location != null)
            {
                JsonData jsonData = new JsonData();
                jsonData["type"] = "Location";
                jsonData["Position"] = location.Position.ToCopyString("F4");
                jsonData["Rotation"] = location.Rotation.ToCopyString("F4");
                jsonData["Scale"] = location.Scale.ToCopyString("F4");
                return JsonToolkit.JsonToString(jsonData);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 将Json字符串转换为Location
        /// </summary>
        /// <param name="json">Json字符串</param>
        /// <returns>Location对象</returns>
        public static Location JsonToLocation(this string json)
        {
            JsonData jsonData = JsonToolkit.StringToJson(json);
            if (jsonData != null && jsonData.GetValueInSafe("type") == "Location")
            {
                Location location = new Location();
                location.Position = jsonData["Position"].ToString().ToPasteVector3(Vector3.zero);
                location.Rotation = jsonData["Rotation"].ToString().ToPasteVector3(Vector3.zero);
                location.Scale = jsonData["Scale"].ToString().ToPasteVector3(Vector3.zero);
                return location;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Hierarchy窗口扩展
        private static GUIStyle HierarchyIconStyle;
        private static Texture HTFrameworkLOGO;

        /// <summary>
        /// 编辑器初始化
        /// </summary>
        private static void OnInitHierarchy()
        {
            HierarchyIconStyle = new GUIStyle();
            HierarchyIconStyle.alignment = TextAnchor.MiddleRight;
            HierarchyIconStyle.normal.textColor = Color.cyan;
            HTFrameworkLOGO = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/HTFrameworkLOGO.png");

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }
        /// <summary>
        /// Hierarchy窗口元素GUI
        /// </summary>
        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            GameObject instance = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (instance)
            {
                if (instance.GetComponent<Main>())
                {
                    GUI.Box(selectionRect, HTFrameworkLOGO, HierarchyIconStyle);
                }
            }
        }
        #endregion

        #region Project窗口扩展
        private static GUIStyle ProjectIconStyle;
        private static GUIStyle ProjectFolderStyle;
        private static Texture HTFrameworkLOGOTitle;
        private static Texture HTFolderLarge;
        private static Texture HTFolderSmall;
        private static string MainFolderGUID;

        /// <summary>
        /// 编辑器初始化
        /// </summary>
        private static void OnInitProject()
        {
            ProjectIconStyle = new GUIStyle();
            ProjectIconStyle.alignment = TextAnchor.MiddleRight;
            ProjectIconStyle.normal.textColor = Color.cyan;
            ProjectFolderStyle = new GUIStyle();
            ProjectFolderStyle.alignment = TextAnchor.MiddleLeft;
            ProjectFolderStyle.normal.textColor = Color.cyan;
            HTFrameworkLOGOTitle = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/HTFrameworkLOGOTitle.png");
            HTFolderLarge = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/HTFolderLarge.png");
            HTFolderSmall = AssetDatabase.LoadAssetAtPath<Texture>("Assets/HTFramework/Editor/Main/Texture/HTFolderSmall.png");
            MainFolderGUID = AssetDatabase.AssetPathToGUID("Assets/HTFramework");

            Editor.finishedDefaultHeaderGUI += OnFinishedDefaultHeaderGUI;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }
        /// <summary>
        /// Default头部GUI
        /// </summary>
        /// <param name="editor">编辑器对象</param>
        private static void OnFinishedDefaultHeaderGUI(Editor editor)
        {
            if (editor.targets != null && editor.targets.Length > 1)
                return;

            string path = AssetDatabase.GetAssetPath(editor.target);
            if (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/")
                || editor.target is MonoScript
                || editor.target is Shader
                || editor.target is GameObject
                || editor.target is TextAsset)
                return;

            if (editor.target is DefaultAsset)
            {
                if (AssetDatabase.IsValidFolder(path))
                {
                    if (string.Equals(path, "Assets/HTFramework"))
                    {
                        GUI.DrawTexture(new Rect(6, 6, 32, 32), HTFolderLarge);

                        EditorGUILayout.HelpBox("Unity HTFramework, a rapid development framework of client to the unity.", MessageType.Info);
                    }
                }
                else
                {
                    DrawVSCodeButton(editor);
                }
            }
            else
            {
                DrawVSCodeButton(editor);
            }
        }
        /// <summary>
        /// Project窗口元素GUI
        /// </summary>
        private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (IsSmallIcon(selectionRect) && guid == MainFolderGUID)
            {
                GUI.Box(selectionRect, HTFrameworkLOGOTitle, ProjectIconStyle);

                if (selectionRect.x < 16) selectionRect.x += 3;
                GUI.Box(selectionRect, HTFolderSmall, ProjectFolderStyle);
            }
        }
        /// <summary>
        /// 是否为显示小图标模式
        /// </summary>
        /// <param name="rect">显示区域</param>
        /// <returns>是否为显示小图标模式</returns>
        private static bool IsSmallIcon(Rect rect)
        {
            return rect.width > rect.height;
        }
        /// <summary>
        /// 绘制 Edit with VSCode 按钮
        /// </summary>
        private static void DrawVSCodeButton(Editor editor)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Edit With VSCode"))
            {
                EditWithVSCode(PathToolkit.ProjectPath + AssetDatabase.GetAssetPath(editor.target));
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Edit .meta With VSCode"))
            {
                EditWithVSCode(PathToolkit.ProjectPath + AssetDatabase.GetAssetPath(editor.target) + ".meta");
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }
        /// <summary>
        /// 打开 VSCode 编辑
        /// </summary>
        private static void EditWithVSCode(string filePath)
        {
            string vscodePath = EditorPrefs.GetString(EditorPrefsTable.VSCodePath, null);
            bool succeed = ExecutableToolkit.Execute(vscodePath, $"\"{filePath}\"");
            if (!succeed)
            {
                EditorApplication.ExecuteMenuItem("HTFramework/HTFramework Settings...");
                Log.Error("请在 Setter 面板设置 VSCode 的启动路径，如未安装 VSCode，请进入官网下载：https://code.visualstudio.com/");
            }
        }
        #endregion

        #region Styles
        public static class Styles
        {
            public static readonly string Box = "Box";
            public static readonly string IconButton = "IconButton";
            public static readonly string InvisibleButton = "InvisibleButton";
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
            public static readonly string ToolbarSearchTextField = "ToolbarSearchTextField";
            public static readonly string ToolbarSearchCancelButton = "ToolbarSearchCancelButton";
            public static readonly string ToolbarSearchCancelButtonEmpty = "ToolbarSearchCancelButtonEmpty";
        }
        #endregion
    }
}