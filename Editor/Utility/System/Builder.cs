using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    public sealed class Builder : EditorWindow
    {
        /// <summary>
        /// 检查项目构建的前置条件，如果返回false，将禁止打包，返回true，才启用打包
        /// </summary>
        public static event HTFFunc<bool> CheckBuildPrecondition;

        [InitializeOnLoadMethod]
        private static void RegisterUpdate()
        {
            EditorApplication.update += CheckBuildPlayerWindow;
        }
        private static void CheckBuildPlayerWindow()
        {
            if (focusedWindow == OpendBuildPlayerWindow)
            {
                Builder builder = GetWindow<Builder>(true, "HTFramework Builder", true);
                builder.minSize = new Vector2(640, 580);
                builder.Show();

                OpendBuildPlayerWindow.Close();
            }
        }
        private static BuildPlayerWindow OpendBuildPlayerWindow
        {
            get
            {
                if (_opendBuildPlayerWindow == null)
                {
                    BuildPlayerWindow[] array = Resources.FindObjectsOfTypeAll<BuildPlayerWindow>();
                    _opendBuildPlayerWindow = (array.Length <= 0) ? CreateInstance<BuildPlayerWindow>() : array[0];
                }
                return _opendBuildPlayerWindow;
            }
        }
        private static BuildPlayerWindow _opendBuildPlayerWindow;

        private MethodInfo _onDisableMethod;
        private MethodInfo _onGUIMethod;
        private MethodInfo _updateMethod;
        private MethodInfo _calculateSelectedBuildTarget;
        private PropertyInfo _activeBuildTargetGroup;
        private bool _isCanBuild = false;
        private bool _isShowBuildABButton = false;

        private void OnEnable()
        {
            _onDisableMethod = OpendBuildPlayerWindow.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);
            _onGUIMethod = OpendBuildPlayerWindow.GetType().GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.NonPublic);
            _updateMethod = OpendBuildPlayerWindow.GetType().GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
            _calculateSelectedBuildTarget = EditorGlobalTools.GetTypeInEditorAssemblies("UnityEditor.EditorUserBuildSettingsUtils").GetMethod("CalculateSelectedBuildTarget", BindingFlags.Static | BindingFlags.Public);
            _activeBuildTargetGroup = typeof(EditorUserBuildSettings).GetProperty("activeBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic);

            RemoveAllScene();
            CheckResourceMode();
            Check();
        }

        private void OnDisable()
        {
            _onDisableMethod.Invoke(OpendBuildPlayerWindow, null);
        }

        private void OnGUI()
        {
            if (!_isCanBuild)
            {
                BuildButtonMaskGUI();
            }

            _onGUIMethod.Invoke(OpendBuildPlayerWindow, null);
            
            if (!_isCanBuild)
            {
                BuildButtonMaskGUI();
            }

            GUI.enabled = true;

            if (_isShowBuildABButton)
            {
                if (GUI.Button(new Rect(position.width - 422, position.height - 31, 123, 18), "Build AssetBundles"))
                {
                    AssetBundleEditor.AssetBundleEditorUtility.BuildAssetBundles();
                }
            }
            if (GUI.Button(new Rect(position.width - 294, position.height - 31, 52, 18), "Check"))
            {
                CheckResourceMode();
                Check();
            }
        }

        private void Update()
        {
            _updateMethod.Invoke(OpendBuildPlayerWindow, null);
        }
        
        private void BuildButtonMaskGUI()
        {
            if (IsSelectedCurrentBuildTarget())
            {
                GUI.color = Color.gray;
                GUI.Button(new Rect(position.width - 238, position.height - 31, 110, 18), "Build");
                GUI.Button(new Rect(position.width - 124, position.height - 31, 110, 18), "Build And Run");
                GUI.color = Color.white;
            }
        }

        private bool IsSelectedCurrentBuildTarget()
        {
            BuildTarget selectedBuildTarget = (BuildTarget)_calculateSelectedBuildTarget.Invoke(null, null);
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            BuildTargetGroup activeBuildTargetGroup = (BuildTargetGroup)_activeBuildTargetGroup.GetValue(null);
            return EditorUserBuildSettings.activeBuildTarget == selectedBuildTarget && activeBuildTargetGroup == selectedBuildTargetGroup;
        }

        private void RemoveAllScene()
        {
            EditorBuildSettings.scenes = null;
        }

        private void CheckResourceMode()
        {
            _isShowBuildABButton = false;

            Main main = FindObjectOfType<Main>();
            if (main != null)
            {
                ResourceManager resource = main.GetComponentByChild<ResourceManager>("Resource");
                if (resource != null && resource.Mode == ResourceLoadMode.AssetBundle)
                {
                    _isShowBuildABButton = true;
                }
            }
        }

        private void Check()
        {
            if (CheckBuildPrecondition != null)
            {
                if (!CheckBuildPrecondition())
                {
                    _isCanBuild = false;
                    GlobalTools.LogError("当前无法构建项目：未满足允许项目构建的前置条件！");
                    return;
                }
            }

            Main main = FindObjectOfType<Main>();
            if (main == null)
            {
                _isCanBuild = false;
                GlobalTools.LogError("当前无法构建项目：请先打开包含框架主体的场景！");
                return;
            }

            ProcedureManager procedure = main.GetComponentByChild<ProcedureManager>("Procedure");
            if (procedure != null && procedure.ActivatedProcedures.Count <= 0)
            {
                _isCanBuild = false;
                GlobalTools.LogError("当前无法构建项目：请添加至少一个流程！");
                return;
            }

            EntityManager entity = main.GetComponentByChild<EntityManager>("Entity");
            if (entity != null)
            {
                for (int i = 0; i < entity.DefineEntityTargets.Count; i++)
                {
                    if (entity.DefineEntityTargets[i] == null)
                    {
                        _isCanBuild = false;
                        GlobalTools.LogError("当前无法构建项目：实体管理器丢失了至少一个预定义对象！");
                        return;
                    }
                }
            }

            UIManager ui = main.GetComponentByChild<UIManager>("UI");
            if (ui != null)
            {
                for (int i = 0; i < ui.DefineUIEntitys.Count; i++)
                {
                    if (ui.DefineUIEntitys[i] == null)
                    {
                        _isCanBuild = false;
                        GlobalTools.LogError("当前无法构建项目：UI管理器丢失了至少一个预定义对象！");
                        return;
                    }
                }
            }

            _isCanBuild = true;
        }
    }
}