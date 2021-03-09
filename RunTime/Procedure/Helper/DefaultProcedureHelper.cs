using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 默认的流程管理器助手
    /// </summary>
    public sealed class DefaultProcedureHelper : IProcedureHelper
    {
        private float _timer = 0;
        private string _defaultProcedure;

        /// <summary>
        /// 流程管理器
        /// </summary>
        public InternalModuleBase Module { get; set; }
        /// <summary>
        /// 当前流程
        /// </summary>
        public ProcedureBase CurrentProcedure { get; private set; }
        /// <summary>
        /// 所有流程
        /// </summary>
        public Dictionary<Type, ProcedureBase> Procedures { get; private set; } = new Dictionary<Type, ProcedureBase>();
        /// <summary>
        /// 所有流程的类型
        /// </summary>
        public List<Type> ProcedureTypes { get; private set; } = new List<Type>();
        /// <summary>
        /// 任意流程切换事件（上一个离开的流程、下一个进入的流程）
        /// </summary>
        public event HTFAction<ProcedureBase, ProcedureBase> AnyProcedureSwitchEvent;
        
        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInitialization()
        {
            ProcedureManager manager = Module as ProcedureManager;

            //创建所有已激活的流程对象
            for (int i = 0; i < manager.ActivatedProcedures.Count; i++)
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(manager.ActivatedProcedures[i]);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(ProcedureBase)))
                    {
                        if (!Procedures.ContainsKey(type))
                        {
                            Procedures.Add(type, Activator.CreateInstance(type) as ProcedureBase);
                            ProcedureTypes.Add(type);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Procedure, "创建流程失败：流程 " + manager.ActivatedProcedures[i] + " 必须继承至流程基类：ProcedureBase！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Procedure, "创建流程失败：丢失流程 " + manager.ActivatedProcedures[i] + "！");
                }
            }
            _defaultProcedure = manager.DefaultProcedure;
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnPreparatory()
        {
            //流程初始化
            foreach (var procedure in Procedures)
            {
                procedure.Value.OnInit();
            }

            //进入默认流程
            if (_defaultProcedure != "")
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(_defaultProcedure);
                if (type != null)
                {
                    if (Procedures.ContainsKey(type))
                    {
                        CurrentProcedure = Procedures[type];
                        CurrentProcedure.OnEnter(null);
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Procedure, "进入流程失败：不存在流程 " + type.Name + " 或者流程未激活！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Procedure, "进入流程失败：丢失流程 " + _defaultProcedure + " ！");
                }
            }
        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnRefresh()
        {
            if (CurrentProcedure != null)
            {
                CurrentProcedure.OnUpdate();

                if (_timer < 1)
                {
                    _timer += Time.deltaTime;
                }
                else
                {
                    _timer = 0;
                    CurrentProcedure.OnUpdateSecond();
                }
            }
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTermination()
        {
            Procedures.Clear();
            ProcedureTypes.Clear();
        }
        /// <summary>
        /// 暂停助手
        /// </summary>
        public void OnPause()
        {

        }
        /// <summary>
        /// 恢复助手
        /// </summary>
        public void OnResume()
        {

        }

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>流程对象</returns>
        public ProcedureBase GetProcedure(Type type)
        {
            if (Procedures.ContainsKey(type))
            {
                return Procedures[type];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Procedure, "获取流程失败：不存在流程 " + type.Name + " 或者流程未激活！");
            }
        }
        /// <summary>
        /// 是否存在流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>是否存在</returns>
        public bool IsExistProcedure(Type type)
        {
            return Procedures.ContainsKey(type);
        }
        
        /// <summary>
        /// 切换流程
        /// </summary>
        /// <param name="type">目标流程</param>
        public void SwitchProcedure(Type type)
        {
            if (Procedures.ContainsKey(type))
            {
                if (CurrentProcedure == Procedures[type])
                {
                    return;
                }

                ProcedureBase lastProcedure = CurrentProcedure;
                ProcedureBase nextProcedure = Procedures[type];
                if (lastProcedure != null)
                {
                    lastProcedure.OnLeave(nextProcedure);
                }
                nextProcedure.OnEnter(lastProcedure);
                CurrentProcedure = nextProcedure;

                AnyProcedureSwitchEvent?.Invoke(lastProcedure, nextProcedure);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Procedure, "切换流程失败：不存在流程 " + type.Name + " 或者流程未激活！");
            }
        }
        /// <summary>
        /// 切换至下一流程
        /// </summary>
        public void SwitchNextProcedure()
        {
            int index = ProcedureTypes.IndexOf(CurrentProcedure.GetType());
            if (index >= ProcedureTypes.Count - 1)
            {
                SwitchProcedure(ProcedureTypes[0]);
            }
            else
            {
                SwitchProcedure(ProcedureTypes[index + 1]);
            }
        }
        /// <summary>
        /// 切换至上一流程
        /// </summary>
        public void SwitchLastProcedure()
        {
            int index = ProcedureTypes.IndexOf(CurrentProcedure.GetType());
            if (index <= 0)
            {
                SwitchProcedure(ProcedureTypes[ProcedureTypes.Count - 1]);
            }
            else
            {
                SwitchProcedure(ProcedureTypes[index - 1]);
            }
        }
        /// <summary>
        /// 切换至指定序号的流程（依据编辑器面板的序号）
        /// </summary>
        /// <param name="index">流程序号</param>
        public void SwitchTargetProcedure(int index)
        {
            index = index - 1;
            if (index >= 0 && index < ProcedureTypes.Count)
            {
                SwitchProcedure(ProcedureTypes[index]);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Procedure, "切换流程失败：不存在序号为 " + (index + 1).ToString() + " 的流程或者流程未激活！");
            }
        }
    }
}