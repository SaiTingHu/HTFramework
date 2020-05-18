using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 调试管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Debug)]
    public sealed class DebugManager : InternalModuleBase
    {
        /// <summary>
        /// 调试器类型【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string DebuggerType = "";
        /// <summary>
        /// 调试器皮肤【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal GUISkin DebuggerSkin;
        /// <summary>
        /// 是否启用调试器
        /// </summary>
        public bool IsEnableDebugger = false;

        private Debugger _debugger;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            if (IsEnableDebugger)
            {
                //创建调试器
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(DebuggerType);
                if (type != null)
                {
                    if (type == typeof(Debugger) || type.IsSubclassOf(typeof(Debugger)))
                    {
                        _debugger = Activator.CreateInstance(type) as Debugger;
                        _debugger.OnInit(DebuggerSkin);
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Debug, "创建调试器失败：调试器类 " + DebuggerType + " 必须继承至：Debugger！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Debug, "创建调试器失败：丢失调试器类 " + DebuggerType + "！");
                }
            }
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            if (_debugger != null)
            {
                _debugger.OnDestory();
                _debugger = null;
            }
        }

        private void OnGUI()
        {
            if (IsEnableDebugger)
            {
                if (_debugger != null)
                {
                    _debugger.OnDebuggerGUI();
                }
            }
        }
    }
}