using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;

namespace HT.Framework
{
    /// <summary>
    /// 编辑器全局工具
    /// </summary>
    public static class EditorGlobalTools
    {
        #region 批处理工具
        /// <summary>
        /// 打开ReplaceFontBatch窗口
        /// </summary>
        [@MenuItem("HTFramework/Batch/Replace Font Batch", false, 0)]
        private static void OpenReplaceFontBatch()
        {
            ReplaceFontBatch rfb = EditorWindow.GetWindow<ReplaceFontBatch>();
            rfb.titleContent.text = "ReplaceFont";
            rfb.position = new Rect(200, 200, 300, 100);
            rfb.Show();
        }

        /// <summary>
        /// 打开ReplaceFontColorBatch窗口
        /// </summary>
        [@MenuItem("HTFramework/Batch/Replace Font Color Batch", false, 1)]
        private static void OpenReplaceFontColorBatch()
        {
            ReplaceFontColorBatch rfb = EditorWindow.GetWindow<ReplaceFontColorBatch>();
            rfb.titleContent.text = "ReplaceFontColor";
            rfb.position = new Rect(200, 200, 300, 100);
            rfb.Show();
        }

        /// <summary>
        /// 打开SetRaycastTargetBatch窗口
        /// </summary>
        [@MenuItem("HTFramework/Batch/Set Raycast Target Batch", false, 2)]
        private static void OpenSetRaycastTargetBatch()
        {
            SetRaycastTargetBatch srtb = EditorWindow.GetWindow<SetRaycastTargetBatch>();
            srtb.titleContent.text = "SetRaycastTarget";
            srtb.position = new Rect(200, 200, 300, 250);
            srtb.Show();
        }

        /// <summary>
        /// 设置鼠标射线可捕获目标
        /// </summary>
        [@MenuItem("HTFramework/Batch/Set Mouse Ray Target", false, 13)]
        private static void SetMouseRayTarget()
        {
            if (Selection.gameObjects.Length <= 0)
            {
                GlobalTools.LogWarning("请先选中场景中的物体！");
                return;
            }

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
        /// 设置网格渲染器组件
        /// </summary>
        [@MenuItem("HTFramework/Batch/MeshRenderer Batch", false, 24)]
        private static void SetMeshRendererBatch()
        {
            MeshRendererBatch mrb = EditorWindow.GetWindow<MeshRendererBatch>();
            mrb.titleContent.text = "MeshRendererBatch";
            mrb.position = new Rect(200, 200, 300, 250);
            mrb.Show();
        }
        #endregion

        #region 网格工具
        /// <summary>
        /// 合并多个模型网格
        /// </summary>
        [@MenuItem("HTFramework/Mesh/Mesh Combines")]
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
        [@MenuItem("HTFramework/Mesh/Mesh Info")]
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
        #endregion

        #region 控制台工具
        /// <summary>
        /// 清理控制台
        /// </summary>
        [@MenuItem("HTFramework/Console/Clear &C")]
        private static void ClearConsole()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Editor));
            Type logEntries = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        /// <summary>
        /// 打印普通日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug Log")]
        private static void ConsoleDebugLog()
        {
            GlobalTools.LogInfo("Debug.Log!");
        }

        /// <summary>
        /// 打印警告日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug LogWarning")]
        private static void ConsoleDebugLogWarning()
        {
            GlobalTools.LogWarning("Debug.LogWarning!");
        }

        /// <summary>
        /// 打印错误日志
        /// </summary>
        [@MenuItem("HTFramework/Console/Debug LogError")]
        private static void ConsoleDebugLogError()
        {
            GlobalTools.LogError("Debug.LogError!");
        }
        #endregion

        #region 编辑器工具
        /// <summary>
        /// 运行场景
        /// </summary>
        [@MenuItem("HTFramework/Editor/Run &R")]
        private static void RunScene()
        {
            EditorApplication.isPlaying = !EditorApplication.isPlaying;
        }
        /// <summary>
        /// 打开编辑器安装路径
        /// </summary>
        [@MenuItem("HTFramework/Editor/Open Installation Path")]
        private static void OpenInstallationPath()
        {
            string path = EditorApplication.applicationPath.Substring(0, EditorApplication.applicationPath.LastIndexOf("/"));
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(path);
            System.Diagnostics.Process.Start(psi);
        }
        #endregion

        #region 工程视图新建菜单
        /// <summary>
        /// 新建Helper类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# Helper Script", false, 11)]
        private static void CreateHelper()
        {
            string path = EditorUtility.SaveFilePanel("新建 Helper 类", Application.dataPath, "NewHelper", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Other/HelperTemplate.txt", typeof(TextAsset)) as TextAsset;
                    if (asset)
                    {
                        string code = asset.text;
                        code = code.Replace("#SCRIPTNAME#", className);
                        code = code.Replace("#HELPERNAME#", className);
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
                    GlobalTools.LogError("新建Helper失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建FiniteState类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# FiniteState Script", false, 12)]
        private static void CreateFiniteState()
        {
            string path = EditorUtility.SaveFilePanel("新建 FiniteState 类", Application.dataPath, "NewFiniteState", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Other/FiniteStateTemplate.txt", typeof(TextAsset)) as TextAsset;
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
        [@MenuItem("Assets/Create/HTFramework/C# Procedure Script", false, 13)]
        private static void CreateProcedure()
        {
            string path = EditorUtility.SaveFilePanel("新建 Procedure 类", Application.dataPath, "NewProcedure", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Other/ProcedureTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                    GlobalTools.LogError("新建Procedure失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建EventHandler类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# EventHandler Script", false, 14)]
        private static void CreateEventHandler()
        {
            string path = EditorUtility.SaveFilePanel("新建 EventHandler 类", Application.dataPath, "NewEventHandler", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Other/EventHandlerTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                    GlobalTools.LogError("新建EventHandler失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建AspectProxy类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# AspectProxy Script", false, 15)]
        private static void CreateAspectProxy()
        {
            string path = EditorUtility.SaveFilePanel("新建 AspectProxy 类", Application.dataPath, "NewAspectProxy", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Other/AspectProxyTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                    GlobalTools.LogError("新建AspectProxy失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建UILogicResident类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# UILogicResident Script", false, 16)]
        private static void CreateUILogicResident()
        {
            string path = EditorUtility.SaveFilePanel("新建 UILogicResident 类", Application.dataPath, "NewUILogicResident", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Other/UILogicResidentTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                    GlobalTools.LogError("新建UILogicResident失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建UILogicTemporary类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# UILogicTemporary Script", false, 17)]
        private static void CreateUILogicTemporary()
        {
            string path = EditorUtility.SaveFilePanel("新建 UILogicTemporary 类", Application.dataPath, "NewUILogicTemporary", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Other/UILogicTemporaryTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                    GlobalTools.LogError("新建UILogicTemporary失败，已存在类型 " + className);
                }
            }
        }

        /// <summary>
        /// 新建DataSet类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# DataSet Script", false, 18)]
        private static void CreateDataSet()
        {
            string path = EditorUtility.SaveFilePanel("新建 DataSet 类", Application.dataPath, "NewDataSet", "cs");
            if (path != "")
            {
                string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
                if (!File.Exists(path))
                {
                    TextAsset asset = AssetDatabase.LoadAssetAtPath("Assets/HTFramework/Other/DataSetTemplate.txt", typeof(TextAsset)) as TextAsset;
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
                    GlobalTools.LogError("新建DataSet失败，已存在类型 " + className);
                }
            }
        }
        #endregion
    }
}