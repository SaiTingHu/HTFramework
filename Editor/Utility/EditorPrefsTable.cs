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
        public static readonly HashSet<string> HTFrameworkFolder = new HashSet<string>() { "HTFramework", "HTFrameworkAI", "HTFrameworkILHotfix", "HTFrameworkXLua", "HTFrameworkGameComponent" };
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
        /// Transform组件Inspector面板配置：是否展开Property
        /// </summary>
        public static readonly string Transform_Property= "HT.Framework.Transform.Property";
        /// <summary>
        /// Transform组件Inspector面板配置：是否展开Hierarchy
        /// </summary>
        public static readonly string Transform_Hierarchy= "HT.Framework.Transform.Hierarchy";
        /// <summary>
        /// Transform组件Inspector面板配置：是否展开Copy
        /// </summary>
        public static readonly string Transform_Copy= "HT.Framework.Transform.Copy";
        /// <summary>
        /// RectTransform组件Inspector面板配置：是否展开Property
        /// </summary>
        public static readonly string RectTransform_Property= "HT.Framework.RectTransform.Property";
        /// <summary>
        /// RectTransform组件Inspector面板配置：是否展开Hierarchy
        /// </summary>
        public static readonly string RectTransform_Hierarchy= "HT.Framework.RectTransform.Hierarchy";
        /// <summary>
        /// RectTransform组件Inspector面板配置：是否展开Copy
        /// </summary>
        public static readonly string RectTransform_Copy= "HT.Framework.RectTransform.Copy";

        /// <summary>
        /// 快捷工具是否启用
        /// </summary>
        public static readonly string LnkTools_Enable = "HT.Framework.LnkTools.Enable";
        /// <summary>
        /// 快捷工具是否展开
        /// </summary>
        public static readonly string LnkTools_Expansion = "HT.Framework.LnkTools.Expansion";
        /// <summary>
        /// About窗口：是否在启动时自动打开
        /// </summary>
        public static readonly string About_IsShowOnStart = "HT.Framework.About.IsShowOnStart";
        /// <summary>
        /// ScriptingDefine历史记录
        /// </summary>
        public static readonly string ScriptingDefine_Record = "HT.Framework.ScriptingDefineRecord";
        /// <summary>
        /// 步骤编辑器窗口样式-步骤列表背景
        /// </summary>
        public static readonly string StepEditor_Style_StepListBG = "HT.Framework.StepEditor.Style.StepListBG";
        #endregion
    }
}