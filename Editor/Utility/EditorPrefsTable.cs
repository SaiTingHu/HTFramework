using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// HT.Framework编辑器配置表
    /// </summary>
    internal static class EditorPrefsTable
    {
        #region EditorGlobalTools
        /// <summary>
        /// 新建StepHelper脚本的文件夹
        /// </summary>
        public static string Script_StepHelper_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.StepHelper";
            }
        }
        /// <summary>
        /// 新建FiniteState脚本的文件夹
        /// </summary>
        public static string Script_FiniteState_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.FiniteState";
            }
        }
        /// <summary>
        /// 新建Procedure脚本的文件夹
        /// </summary>
        public static string Script_Procedure_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.Procedure";
            }
        }
        /// <summary>
        /// 新建EventHandler脚本的文件夹
        /// </summary>
        public static string Script_EventHandler_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.EventHandler";
            }
        }
        /// <summary>
        /// 新建AspectProxy脚本的文件夹
        /// </summary>
        public static string Script_AspectProxy_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.AspectProxy";
            }
        }
        /// <summary>
        /// 新建UILogicResident脚本的文件夹
        /// </summary>
        public static string Script_UILogicResident_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.UILogicResident";
            }
        }
        /// <summary>
        /// 新建UILogicTemporary脚本的文件夹
        /// </summary>
        public static string Script_UILogicTemporary_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.UILogicTemporary";
            }
        }
        /// <summary>
        /// 新建DataSet脚本的文件夹
        /// </summary>
        public static string Script_DataSet_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.DataSet";
            }
        }
        /// <summary>
        /// 新建EntityLogic脚本的文件夹
        /// </summary>
        public static string Script_EntityLogic_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.EntityLogic";
            }
        }
        /// <summary>
        /// 新建CustomModule脚本的文件夹
        /// </summary>
        public static string Script_CustomModule_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.CustomModule";
            }
        }
        /// <summary>
        /// 新建ProtocolChannel脚本的文件夹
        /// </summary>
        public static string Script_ProtocolChannel_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.ProtocolChannel";
            }
        }
        /// <summary>
        /// 新建TaskContent脚本的文件夹
        /// </summary>
        public static string Script_TaskContent_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.TaskContent";
            }
        }
        /// <summary>
        /// 新建TaskPoint脚本的文件夹
        /// </summary>
        public static string Script_TaskPoint_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.TaskPoint";
            }
        }
        /// <summary>
        /// 新建SettingItem脚本的文件夹
        /// </summary>
        public static string Script_SettingItem_Directory
        {
            get
            {
                return Application.productName + ".HT.Framework.Script.SettingItem";
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Transform组件Inspector面板配置：是否展开Property
        /// </summary>
        public static string Transform_Property
        {
            get
            {
                return Application.productName + ".HT.Framework.Transform.Property";
            }
        }
        /// <summary>
        /// Transform组件Inspector面板配置：是否展开Hierarchy
        /// </summary>
        public static string Transform_Hierarchy
        {
            get
            {
                return Application.productName + ".HT.Framework.Transform.Hierarchy";
            }
        }
        /// <summary>
        /// Transform组件Inspector面板配置：是否展开Copy
        /// </summary>
        public static string Transform_Copy
        {
            get
            {
                return Application.productName + ".HT.Framework.Transform.Copy";
            }
        }
        /// <summary>
        /// RectTransform组件Inspector面板配置：是否展开Property
        /// </summary>
        public static string RectTransform_Property
        {
            get
            {
                return Application.productName + ".HT.Framework.RectTransform.Property";
            }
        }
        /// <summary>
        /// RectTransform组件Inspector面板配置：是否展开Hierarchy
        /// </summary>
        public static string RectTransform_Hierarchy
        {
            get
            {
                return Application.productName + ".HT.Framework.RectTransform.Hierarchy";
            }
        }
        /// <summary>
        /// RectTransform组件Inspector面板配置：是否展开Copy
        /// </summary>
        public static string RectTransform_Copy
        {
            get
            {
                return Application.productName + ".HT.Framework.RectTransform.Copy";
            }
        }
        #endregion

        #region Main
        /// <summary>
        /// ScriptingDefine历史记录
        /// </summary>
        public static string ScriptingDefine_Record
        {
            get
            {
                return Application.productName + ".HT.Framework.ScriptingDefineRecord";
            }
        }
        #endregion

        #region StepEditor
        /// <summary>
        /// 步骤编辑器窗口样式-步骤列表背景
        /// </summary>
        public static string Style_StepEditor_StepListBG
        {
            get
            {
                return Application.productName + ".HT.Framework.Style.StepEditor.StepListBG";
            }
        }
        #endregion
    }
}