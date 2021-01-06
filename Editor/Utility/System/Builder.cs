using AssetBundleBrowser;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace HT.Framework
{
    internal sealed class Builder : HTFEditorWindow
    {
        /// <summary>
        /// 检查项目构建的前置条件，如果至少有一个条件返回false，将禁止打包，全部返回true，才启用打包
        /// </summary>
        public static List<HTFFunc<bool>> CheckBuildPreconditions = new List<HTFFunc<bool>>();
        /// <summary>
        /// 项目发布完成事件
        /// </summary>
        public static event HTFAction<BuildTarget,string> PostProcessBuildEvent;

        [InitializeOnLoadMethod]
        private static void RegisterUpdate()
        {
            EditorApplication.update += CheckBuildPlayerWindow;
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
        
        [PostProcessBuild(0)]
        private static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
        {
            Log.Info("项目发布成功！发布平台：" + target.ToString() + "！发布路径：" + pathToBuildProject + "！");

            PostProcessBuildEvent?.Invoke(target, pathToBuildProject);
        }

        private BuildPlayerWindow _buildPlayerWindow;
        private MethodInfo _onDisableMethod;
        private MethodInfo _onGUIMethod;
        private MethodInfo _updateMethod;
        private MethodInfo _calculateSelectedBuildTarget;
        private PropertyInfo _activeBuildTargetGroup;
        private bool _isCanBuild = false;
        private bool _isShowBuildABButton = false;

        protected override bool IsEnableTitleGUI
        {
            get
            {
                return false;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            BuildPlayerWindow[] buildPlayerWindows = Resources.FindObjectsOfTypeAll<BuildPlayerWindow>();
            _buildPlayerWindow = buildPlayerWindows.Length > 0 ? buildPlayerWindows[0] : CreateInstance<BuildPlayerWindow>();
            _onDisableMethod = _buildPlayerWindow.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);
            _onGUIMethod = _buildPlayerWindow.GetType().GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.NonPublic);
            _updateMethod = _buildPlayerWindow.GetType().GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
            _calculateSelectedBuildTarget = EditorReflectionToolkit.GetTypeInEditorAssemblies("UnityEditor.EditorUserBuildSettingsUtils").GetMethod("CalculateSelectedBuildTarget", BindingFlags.Static | BindingFlags.Public);
            _activeBuildTargetGroup = typeof(EditorUserBuildSettings).GetProperty("activeBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic);

            CheckResourceMode();
            Check();
        }
        private void OnDisable()
        {
            _onDisableMethod.Invoke(_buildPlayerWindow, null);
        }
        protected override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (EditorBuildSettings.scenes != null && EditorBuildSettings.scenes.Length > 0)
            {
                EditorBuildSettings.scenes = null;
                Log.Warning("只允许构建包含框架主体的场景！如有多场景切换的需求，请将其他场景打入AB包！");
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
            for (int i = 0; i < CheckBuildPreconditions.Count; i++)
            {
                if (!CheckBuildPreconditions[i]())
                {
                    _isCanBuild = false;
                    Log.Error("当前无法构建项目：未满足允许项目构建的前置条件！");
                    return;
                }
            }

            Main main = FindObjectOfType<Main>();
            if (main == null)
            {
                _isCanBuild = false;
                Log.Error("当前无法构建项目：请先打开包含框架主体的场景！");
                return;
            }

            ProcedureManager procedure = main.GetComponentByChild<ProcedureManager>("Procedure");
            if (procedure != null && procedure.ActivatedProcedures.Count <= 0)
            {
                _isCanBuild = false;
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
                        _isCanBuild = false;
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
                        _isCanBuild = false;
                        Log.Error("当前无法构建项目：UI管理器丢失了至少一个预定义对象！");
                        return;
                    }
                }
            }

            _isCanBuild = true;
        }
    }
}