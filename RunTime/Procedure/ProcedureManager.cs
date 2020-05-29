using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 流程管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Procedure)]
    public sealed class ProcedureManager : InternalModuleBase
    {
        /// <summary>
        /// 当前激活的流程类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<string> ActivatedProcedures = new List<string>();
        /// <summary>
        /// 当前的默认流程类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string DefaultProcedure = "";

        /// <summary>
        /// 任意流程切换事件（上一个离开的流程、下一个进入的流程）
        /// </summary>
        public event HTFAction<ProcedureBase, ProcedureBase> AnyProcedureSwitchEvent;
        
        private IProcedureHelper _helper;

        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IProcedureHelper;
            _helper.OnInitialization(ActivatedProcedures, DefaultProcedure);
            _helper.AnyProcedureSwitchEvent += (last, next) =>
            {
                AnyProcedureSwitchEvent?.Invoke(last, next);
            };
        }
        internal override void OnPreparatory()
        {
            base.OnPreparatory();

            _helper.OnPreparatory();
        }
        internal override void OnRefresh()
        {
            base.OnRefresh();

            _helper.OnRefresh();
        }
        internal override void OnTermination()
        {
            base.OnTermination();

            _helper.OnTermination();
        }

        /// <summary>
        /// 当前流程
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                return _helper.CurrentProcedure;
            }
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <typeparam name="T">流程类</typeparam>
        /// <returns>流程对象</returns>
        public T GetProcedure<T>() where T : ProcedureBase
        {
            return _helper.GetProcedure(typeof(T)) as T;
        }
        /// <summary>
        /// 获取流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>流程对象</returns>
        public ProcedureBase GetProcedure(Type type)
        {
            return _helper.GetProcedure(type);
        }

        /// <summary>
        /// 是否存在流程
        /// </summary>
        /// <typeparam name="T">流程类</typeparam>
        /// <returns>是否存在</returns>
        public bool IsExistProcedure<T>() where T : ProcedureBase
        {
            return _helper.IsExistProcedure(typeof(T));
        }
        /// <summary>
        /// 是否存在流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>是否存在</returns>
        public bool IsExistProcedure(Type type)
        {
            return _helper.IsExistProcedure(type);
        }

        /// <summary>
        /// 切换流程
        /// </summary>
        /// <typeparam name="T">目标流程</typeparam>
        public void SwitchProcedure<T>() where T : ProcedureBase
        {
            _helper.SwitchProcedure(typeof(T));
        }
        /// <summary>
        /// 切换流程
        /// </summary>
        /// <param name="type">目标流程</param>
        public void SwitchProcedure(Type type)
        {
            _helper.SwitchProcedure(type);
        }
        /// <summary>
        /// 切换至下一流程
        /// </summary>
        public void SwitchNextProcedure()
        {
            _helper.SwitchNextProcedure();
        }
        /// <summary>
        /// 切换至上一流程
        /// </summary>
        public void SwitchLastProcedure()
        {
            _helper.SwitchLastProcedure();
        }
        /// <summary>
        /// 切换至指定序号的流程（依据编辑器面板的序号）
        /// </summary>
        /// <param name="index">流程序号</param>
        public void SwitchTargetProcedure(int index)
        {
            _helper.SwitchTargetProcedure(index);
        }
    }
}