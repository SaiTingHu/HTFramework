using AssetBundleBrowser;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 框架构建器
    /// </summary>
    public sealed class Builder : HTFEditorWindow
    {
        /// <summary>
        /// 检查项目构建的前置条件，如果至少有一个条件返回false，将禁止打包，全部返回true，才启用打包
        /// </summary>
        public static List<HTFFunc<bool>> CheckBuildPreconditions = new List<HTFFunc<bool>>();
        /// <summary>
        /// 项目发布前事件
        /// </summary>
        public static event HTFAction<BuildPlayerOptions> PreProcessBuildEvent;
        /// <summary>
        /// 项目发布后事件
        /// </summary>
        public static event HTFAction<BuildTarget, string> PostProcessBuildEvent;

        [InitializeOnLoadMethod]
        private static void RegisterUpdate()
        {
            EditorApplication.update += CheckBuildPlayerWindow;
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnRegisterBuildPlayerHandler);
        }
        private static void CheckBuildPlayerWindow()
        {
            if (focusedWindow is BuildPlayerWindow)
            {
                focusedWindow.Close();

                Builder builder = GetWindow<Builder>(true, "HTFramework Builder", true);
                builder.minSize = new Vector2(640, 580);
                builder.Show();
            }
        }
        
        /// <summary>
        /// 项目发布后处理
        /// </summary>
        /// <param name="target">发布平台</param>
        /// <param name="pathToBuildProject">发布路径</param>
        [PostProcessBuild(0)]
        private static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
        {
            Log.Info($"项目发布成功！发布平台：{target}！发布路径：{pathToBuildProject}！");

            PostProcessBuildEvent?.Invoke(target, pathToBuildProject);
        }
        /// <summary>
        /// 项目发布前处理
        /// </summary>
        /// <param name="options">发布参数</param>
        private static void OnRegisterBuildPlayerHandler(BuildPlayerOptions options)
        {
            if (options.options.HasFlag(BuildOptions.AutoRunPlayer))
            {
                Log.Error("项目发布失败：由于某些特殊原因，框架不支持 Build And Run 形式发布项目！");
                return;
            }

            //蚀刻当前版本号到运行时程序
            VersionInfo versionInfo = AssetDatabase.LoadAssetAtPath<VersionInfo>("Assets/HTFramework/Editor/Utility/Version/Version.asset");
            Main main = FindObjectOfType<Main>();
            if (main != null && versionInfo != null)
            {
                main.Version = versionInfo.CurrentVersion.GetFullNumber();
                EditorUtility.SetDirty(main);
            }

            PreProcessBuildEvent?.Invoke(options);

            EditorApplication.ExecuteMenuItem("File/Save");
            EditorApplication.ExecuteMenuItem("File/Save Project");

            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
        }

        private BuildPlayerWindow _buildPlayerWindow;
        private MethodInfo _onGUIMethod;
        private MethodInfo _updateMethod;
        private MethodInfo _calculateSelectedBuildTarget;
        private PropertyInfo _activeBuildTargetGroup;
        private bool _isCanAddScene = false;
        private bool _isCanBuild = false;
        private bool _isShowBuildABButton = false;

        protected override bool IsEnableTitleGUI => false;

        protected override void OnEnable()
        {
            base.OnEnable();

            _buildPlayerWindow = CreateInstance<BuildPlayerWindow>();
            _onGUIMethod = _buildPlayerWindow.GetType().GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.NonPublic);
            _updateMethod = _buildPlayerWindow.GetType().GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
            _calculateSelectedBuildTarget = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.EditorUserBuildSettingsUtils").GetMethod("CalculateSelectedBuildTarget", BindingFlags.Static | BindingFlags.Public);
            _activeBuildTargetGroup = typeof(EditorUserBuildSettings).GetProperty("activeBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic);

            CheckResourceMode();
            Check();
        }
        private void OnDisable()
        {
            _onGUIMethod = null;
            _updateMethod = null;
            _calculateSelectedBuildTarget = null;
            _activeBuildTargetGroup = null;
            if (_buildPlayerWindow != null)
            {
                DestroyImmediate(_buildPlayerWindow);
                _buildPlayerWindow = null;
            }
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (!_isCanAddScene)
            {
                if (EditorBuildSettings.scenes != null && EditorBuildSettings.scenes.Length > 0)
                {
                    EditorBuildSettings.scenes = null;
                    Log.Warning("只允许构建包含框架主体的场景！如有多场景切换的需求，请将其他场景打入AB包！");
                }
            }

            if (!_isCanBuild)
            {
                BuildButtonMaskGUI();
            }

            _onGUIMethod.Invoke(_buildPlayerWindow, null);

            if (!_isCanBuild)
            {
                BuildButtonMaskGUI();
            }

            GUI.enabled = true;

            if (_isShowBuildABButton)
            {
                if (GUI.Button(new Rect(position.width - 422, position.height - 31, 123, 18), "Build AssetBundles"))
                {
                    AssetBundleBrowserMain.ShowWindow();
                }
            }
            if (GUI.Button(new Rect(position.width - 294, position.height - 31, 52, 18), "Check"))
            {
                CheckResourceMode();
                Check();
            }
            if (GUI.Button(new Rect(position.width - 170, 3, 160, 18), "HTFramework Settings..."))
            {
                Setter setter = GetWindow<Setter>();
                setter.titleContent.image = EditorGUIUtility.IconContent("SettingsIcon").image;
                setter.titleContent.text = "HTFramework Setter";
                setter.minSize = new Vector2(640, 580);
                setter.Show();
            }
        }
        private void Update()
        {
            if (!_buildPlayerWindow)
            {
                Close();
                return;
            }

            if (_updateMethod != null)
            {
                _updateMethod.Invoke(_buildPlayerWindow, null);
            }
        }
        private void BuildButtonMaskGUI()
        {
            if (IsSelectedCurrentBuildTarget())
            {
                GUI.color = Color.gray;
                GUI.Button(new Rect(position.width - 237, position.height - 31, 110, 18), "Build");
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
            _isCanAddScene = false;
            _isCanBuild = false;

            Main main = FindObjectOfType<Main>();
            if (main == null)
            {
                Log.Error("当前无法构建项目：请先打开包含框架主体的场景！");
                return;
            }

            _isCanAddScene = main.IsAllowSceneAddBuild;

            for (int i = 0; i < CheckBuildPreconditions.Count; i++)
            {
                if (!CheckBuildPreconditions[i]())
                {
                    Log.Error("当前无法构建项目：未满足允许项目构建的前置条件！");
                    return;
                }
            }

            ProcedureManager procedure = main.GetComponentByChild<ProcedureManager>("Procedure");
            if (procedure != null && procedure.ActivatedProcedures.Count <= 0)
            {
                Log.Error("当前无法构建项目：请添加至少一个流程！");
                return;
            }

            EntityManager entity = main.GetComponentByChild<EntityManager>("Entity");
            if (entity != null)
            {
                for (int i = 0; i < entity.DefineEntityTargets.Count; i++)
                {
                    if (entity.DefineEntityTargets[i] == null)
                    {
                        Log.Error("当前无法构建项目：实体管理器丢失了至少一个预定义对象！");
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
                        Log.Error("当前无法构建项目：UI管理器丢失了至少一个预定义对象！");
                        return;
                    }
                }
            }

            _isCanBuild = true;
        }
    }
}