using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// HT.Framework编辑器配置表
    /// </summary>
    internal static class EditorPrefsTable
    {
        #region Editor Config
        /// <summary>
        /// 框架根路径
        /// </summary>
        public static readonly string HTFrameworkRootPath = "Assets/HTFramework";
        /// <summary>
        /// 快捷创建Script的模板文件夹
        /// </summary>
        public static readonly string ScriptTemplateFolder = "Assets/HTFramework/Editor/Utility/Template/";
        /// <summary>
        /// 框架相关目录
        /// </summary>
        public static readonly HashSet<string> HTFrameworkFolder = new HashSet<string>() { "HTFramework", "HTFrameworkAI", "HTFrameworkGameComponent" };
        #endregion

        #region Editor PrefsKey
        /// <summary>
        /// 新建StepHelper脚本的文件夹
        /// </summary>
        public static readonly string Script_StepHelper_Folder = "HT.Framework.Script.StepHelper";
        /// <summary>
        /// 新建FiniteState脚本的文件夹
        /// </summary>
        public static readonly string Script_FiniteState_Folder = "HT.Framework.Script.FiniteState";
        /// <summary>
        /// 新建FSMArgs脚本的文件夹
        /// </summary>
        public static readonly string Script_FSMArgs_Folder = "HT.Framework.Script.FSMArgs";
        /// <summary>
        /// 新建FSMData脚本的文件夹
        /// </summary>
        public static readonly string Script_FSMData_Folder = "HT.Framework.Script.FSMData";
        /// <summary>
        /// 新建Procedure脚本的文件夹
        /// </summary>
        public static readonly string Script_Procedure_Folder = "HT.Framework.Script.Procedure";
        /// <summary>
        /// 新建EventHandler脚本的文件夹
        /// </summary>
        public static readonly string Script_EventHandler_Folder = "HT.Framework.Script.EventHandler";
        /// <summary>
        /// 新建AspectProxy脚本的文件夹
        /// </summary>
        public static readonly string Script_AspectProxy_Folder = "HT.Framework.Script.AspectProxy";
        /// <summary>
        /// 新建UILogicResident脚本的文件夹
        /// </summary>
        public static readonly string Script_UILogicResident_Folder = "HT.Framework.Script.UILogicResident";
        /// <summary>
        /// 新建UILogicTemporary脚本的文件夹
        /// </summary>
        public static readonly string Script_UILogicTemporary_Folder = "HT.Framework.Script.UILogicTemporary";
        /// <summary>
        /// 新建DataSet脚本的文件夹
        /// </summary>
        public static readonly string Script_DataSet_Folder = "HT.Framework.Script.DataSet";
        /// <summary>
        /// 新建EntityLogic脚本的文件夹
        /// </summary>
        public static readonly string Script_EntityLogic_Folder = "HT.Framework.Script.EntityLogic";
        /// <summary>
        /// 新建CustomModule脚本的文件夹
        /// </summary>
        public static readonly string Script_CustomModule_Folder = "HT.Framework.Script.CustomModule";
        /// <summary>
        /// 新建ProtocolChannel脚本的文件夹
        /// </summary>
        public static readonly string Script_ProtocolChannel_Folder = "HT.Framework.Script.ProtocolChannel";
        /// <summary>
        /// 新建TaskContent脚本的文件夹
        /// </summary>
        public static readonly string Script_TaskContent_Folder = "HT.Framework.Script.TaskContent";
        /// <summary>
        /// 新建TaskPoint脚本的文件夹
        /// </summary>
        public static readonly string Script_TaskPoint_Folder = "HT.Framework.Script.TaskPoint";
        /// <summary>
        /// 新建SettingItem脚本的文件夹
        /// </summary>
        public static readonly string Script_SettingItem_Folder = "HT.Framework.Script.SettingItem";
        /// <summary>
        /// 新建ECS组件的文件夹
        /// </summary>
        public static readonly string Script_ECSComponent_Folder = "HT.Framework.Script.ECSComponent";
        /// <summary>
        /// 新建ECS系统的文件夹
        /// </summary>
        public static readonly string Script_ECSSystem_Folder = "HT.Framework.Script.ECSSystem";
        /// <summary>
        /// 新建ECS指令的文件夹
        /// </summary>
        public static readonly string Script_ECSOrder_Folder = "HT.Framework.Script.ECSOrder";
        /// <summary>
        /// 新建HotfixProcedure脚本的文件夹
        /// </summary>
        public static readonly string Script_HotfixProcedure_Folder = "HT.Framework.Script.HotfixProcedure";
        /// <summary>
        /// 新建HotfixObject脚本的文件夹
        /// </summary>
        public static readonly string Script_HotfixObject_Folder = "HT.Framework.Script.HotfixObject";

        /// <summary>
        /// Transform组件Inspector面板配置：是否仅显示局部坐标
        /// </summary>
        public static readonly string Transform_OnlyShowLocal = "HT.Framework.Transform.OnlyShowLocal";
        /// <summary>
        /// 快捷工具是否启用
        /// </summary>
        public static readonly string LnkTools_Enable = "HT.Framework.LnkTools.Enable";
        /// <summary>
        /// VSCode的启动路径
        /// </summary>
        public static readonly string VSCodePath = "HT.Framework.VSCodePath";
        /// <summary>
        /// ILSpy的启动路径
        /// </summary>
        public static readonly string ILSpyPath = "HT.Framework.ILSpyPath";
        /// <summary>
        /// About窗口：是否在启动时自动打开
        /// </summary>
        public static readonly string About_IsShowOnStart = "HT.Framework.About.IsShowOnStart";
        /// <summary>
        /// ScriptingDefine历史记录
        /// </summary>
        public static readonly string ScriptingDefine_Record = "HT.Framework.ScriptingDefineRecord";
        /// <summary>
        /// StandardizingNaming的配置数据集路径
        /// </summary>
        public static readonly string StandardizingNaming_Config = "HT.Framework.StandardizingNaming.Config";

        /// <summary>
        /// TaskEditorWindow配置：是否仅在选中时展开任务点
        /// </summary>
        public static readonly string TaskEditorWindow_ExpandOnlySelected = "HT.Framework.TaskEditorWindow.ExpandOnlySelected";
        /// <summary>
        /// TaskEditorWindow配置：是否显示任务点全名
        /// </summary>
        public static readonly string TaskEditorWindow_ShowPointFullName = "HT.Framework.TaskEditorWindow.ShowPointFullName";
        #endregion
    }
}