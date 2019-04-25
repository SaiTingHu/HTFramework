using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.IO;
using UnityEditor.SceneManagement;
using UnityObject = UnityEngine.Object;

namespace HT.Framework
{
    /// <summary>
    /// 编辑器全局工具
    /// </summary>
    public static class EditorGlobalTools
    {
        #region 批处理工具
        /// <summary>
        /// 设置鼠标射线可捕获目标
        /// </summary>
        [@MenuItem("HTFramework/Batch/Set Mouse Ray Target", false, 0)]
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
        /// 打开ReplaceFontBatch窗口
        /// </summary>
        [@MenuItem("HTFramework/Batch/Replace Font Batch", false, 11)]
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
        [@MenuItem("HTFramework/Batch/Replace Font Color Batch", false, 12)]
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
        [@MenuItem("HTFramework/Batch/Set Raycast Target Batch", false, 13)]
        private static void OpenSetRaycastTargetBatch()
        {
            SetRaycastTargetBatch srtb = EditorWindow.GetWindow<SetRaycastTargetBatch>();
            srtb.titleContent.text = "SetRaycastTarget";
            srtb.position = new Rect(200, 200, 300, 250);
            srtb.Show();
        }
        
        /// <summary>
        /// 设置网格渲染器组件
        /// </summary>
        [@MenuItem("HTFramework/Batch/MeshRenderer Batch", false, 30)]
        private static void SetMeshRendererBatch()
        {
            MeshRendererBatch mrb = EditorWindow.GetWindow<MeshRendererBatch>();
            mrb.titleContent.text = "MeshRendererBatch";
            mrb.position = new Rect(200, 200, 300, 250);
            mrb.Show();
        }

        /// <summary>
        /// 设置Texture导入参数
        /// </summary>
        [@MenuItem("HTFramework/Batch/Texture ImportSettings Batch", false, 50)]
        private static void SetTextureImportSettingsBatch()
        {
            TextureImportSettingsBatch tisb = EditorWindow.GetWindow<TextureImportSettingsBatch>();
            tisb.titleContent.text = "TextureBatch";
            tisb.position = new Rect(200, 200, 300, 250);
            tisb.Show();
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
        [@MenuItem("HTFramework/Console/Clear &1")]
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
        [@MenuItem("HTFramework/Editor/Run &2")]
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
        private static readonly string HelperScriptDirectory = ".Script.Helper";
        private static readonly string FiniteStateScriptDirectory = ".Script.FiniteState";
        private static readonly string ProcedureScriptDirectory = ".Script.Procedure";
        private static readonly string EventHandlerScriptDirectory = ".Script.EventHandler";
        private static readonly string AspectProxyScriptDirectory = ".Script.AspectProxy";
        private static readonly string UILogicResidentScriptDirectory = ".Script.UILogicResident";
        private static readonly string UILogicTemporaryScriptDirectory = ".Script.UILogicTemporary";
        private static readonly string DataSetScriptDirectory = ".Script.DataSet";

        /// <summary>
        /// 新建Helper类
        /// </summary>
        [@MenuItem("Assets/Create/HTFramework/C# Helper Script", false, 11)]
        private static void CreateHelper()
        {
            string directory = EditorPrefs.GetString(Application.productName + HelperScriptDirectory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 Helper 类", directory, "NewHelper", "cs");
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
                        EditorPrefs.SetString(Application.productName + HelperScriptDirectory, path);
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
            string directory = EditorPrefs.GetString(Application.productName + FiniteStateScriptDirectory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 FiniteState 类", directory, "NewFiniteState", "cs");
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
                        EditorPrefs.SetString(Application.productName + FiniteStateScriptDirectory, path);
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
            string directory = EditorPrefs.GetString(Application.productName + ProcedureScriptDirectory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 Procedure 类", directory, "NewProcedure", "cs");
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
                        EditorPrefs.SetString(Application.productName + ProcedureScriptDirectory, path);
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
            string directory = EditorPrefs.GetString(Application.productName + EventHandlerScriptDirectory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 EventHandler 类", directory, "NewEventHandler", "cs");
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
                        EditorPrefs.SetString(Application.productName + EventHandlerScriptDirectory, path);
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
            string directory = EditorPrefs.GetString(Application.productName + AspectProxyScriptDirectory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 AspectProxy 类", directory, "NewAspectProxy", "cs");
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
                        EditorPrefs.SetString(Application.productName + AspectProxyScriptDirectory, path);
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
            string directory = EditorPrefs.GetString(Application.productName + UILogicResidentScriptDirectory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 UILogicResident 类", directory, "NewUILogicResident", "cs");
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
                        EditorPrefs.SetString(Application.productName + UILogicResidentScriptDirectory, path);
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
            string directory = EditorPrefs.GetString(Application.productName + UILogicTemporaryScriptDirectory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 UILogicTemporary 类", directory, "NewUILogicTemporary", "cs");
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
                        EditorPrefs.SetString(Application.productName + UILogicTemporaryScriptDirectory, path);
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
            string directory = EditorPrefs.GetString(Application.productName + DataSetScriptDirectory, Application.dataPath);
            string path = EditorUtility.SaveFilePanel("新建 DataSet 类", directory, "NewDataSet", "cs");
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
                        EditorPrefs.SetString(Application.productName + DataSetScriptDirectory, path);
                    }
                }
                else
                {
                    GlobalTools.LogError("新建DataSet失败，已存在类型 " + className);
                }
            }
        }
        #endregion

        #region Editor类的扩展GUI控件
        /// <summary>
        /// 设置Editor指向的target被改变
        /// </summary>
        public static void HasChanged(this Editor editor)
        {
            EditorUtility.SetDirty(editor.target);
            Component component = editor.target as Component;
            if (component)
            {
                EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
            }
        }

        /// <summary>
        /// 制作一个Toggle
        /// </summary>
        public static void Toggle(this Editor editor, bool value, out bool outValue, string name)
        {
            GUI.color = value ? Color.white : Color.gray;
            bool newValue = GUILayout.Toggle(value, name);
            if (value != newValue)
            {
                Undo.RecordObject(editor.target, "Set " + name);
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
            GUI.color = Color.white;
        }

        /// <summary>
        /// 制作一个IntSlider
        /// </summary>
        public static void IntSlider(this Editor editor, int value, out int outValue, int leftValue, int rightValue, string name)
        {
            int newValue = EditorGUILayout.IntSlider(name, value, leftValue, rightValue);
            if (value != newValue)
            {
                Undo.RecordObject(editor.target, "Set " + name);
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个FloatSlider
        /// </summary>
        public static void FloatSlider(this Editor editor, float value, out float outValue, float leftValue, float rightValue, string name)
        {
            float newValue = EditorGUILayout.Slider(name, value, leftValue, rightValue);
            if (!Mathf.Approximately(value, newValue))
            {
                Undo.RecordObject(editor.target, "Set " + name);
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
        }

        /// <summary>
        /// 制作一个FloatField
        /// </summary>
        public static void FloatField(this Editor editor, float value, out float outValue, string name)
        {
            float newValue = EditorGUILayout.FloatField(name, value);
            if (!Mathf.Approximately(value, newValue))
            {
                Undo.RecordObject(editor.target, "Set " + name);
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个FloatField
        /// </summary>
        public static void FloatField(this Editor editor, float value, out float outValue)
        {
            float newValue = EditorGUILayout.FloatField(value);
            if (!Mathf.Approximately(value, newValue))
            {
                Undo.RecordObject(editor.target, "Set Float");
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个TextField
        /// </summary>
        public static void TextField(this Editor editor, string value, out string outValue)
        {
            string newValue = EditorGUILayout.TextField(value);
            if (value != newValue)
            {
                Undo.RecordObject(editor.target, "Set String");
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个IntField
        /// </summary>
        public static void IntField(this Editor editor, int value, out int outValue)
        {
            int newValue = EditorGUILayout.IntField(value);
            if (value != newValue)
            {
                Undo.RecordObject(editor.target, "Set Int");
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个ObjectField
        /// </summary>
        public static void ObjectField<T>(this Editor editor, T value, out T outValue, bool allowSceneObjects, string name) where T : UnityObject
        {
            T newValue = EditorGUILayout.ObjectField(name, value, typeof(T), allowSceneObjects) as T;
            if (value != newValue)
            {
                Undo.RecordObject(editor.target, "Set" + name);
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        /// <summary>
        /// 制作一个Vector3Field
        /// </summary>
        public static void Vector3Field(this Editor editor, Vector3 value, out Vector3 outValue,string name)
        {
            Vector3 newValue = EditorGUILayout.Vector3Field(name, value);
            if (value != newValue)
            {
                Undo.RecordObject(editor.target, "Set Vector3");
                outValue = newValue;
                editor.HasChanged();
            }
            else
            {
                outValue = value;
            }
        }
        #endregion
    }
}