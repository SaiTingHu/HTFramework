using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 流程管理者
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.Procedure)]
    public sealed class ProcedureManager : InternalModuleBase
    {
        /// <summary>
        /// 当前激活的流程类名【请勿在代码中修改】
        /// </summary>
        public List<string> ActivatedProcedures = new List<string>();
        /// <summary>
        /// 当前的默认流程类名【请勿在代码中修改】
        /// </summary>
        public string DefaultProcedure = "";

        private Dictionary<Type, ProcedureBase> _procedureInstances = new Dictionary<Type, ProcedureBase>();
        private List<Type> _procedureTypes = new List<Type>();
        private ProcedureBase _currentProcedure;
        private float _timer = 0;

        public override void OnInitialization()
        {
            base.OnInitialization();

            //创建所有已激活的流程对象
            for (int i = 0; i < ActivatedProcedures.Count; i++)
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(ActivatedProcedures[i]);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(ProcedureBase)))
                    {
                        if (!_procedureInstances.ContainsKey(type))
                        {
                            _procedureInstances.Add(type, Activator.CreateInstance(type) as ProcedureBase);
                            _procedureTypes.Add(type);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Procedure, "创建流程失败：流程 " + ActivatedProcedures[i] + " 必须继承至流程基类：ProcedureBase！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Procedure, "创建流程失败：丢失流程 " + ActivatedProcedures[i] + "！");
                }
            }
        }

        public override void OnPreparatory()
        {
            base.OnPreparatory();

            //流程初始化
            foreach (var procedureInstance in _procedureInstances)
            {
                procedureInstance.Value.OnInit();
            }

            //进入默认流程
            if (DefaultProcedure != "")
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(DefaultProcedure);
                if (type != null)
                {
                    if (_procedureInstances.ContainsKey(type))
                    {
                        _currentProcedure = _procedureInstances[type];
                        _currentProcedure.OnEnter(null);
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.Procedure, "进入流程失败：不存在流程 " + type.Name + " 或者流程未激活！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.Procedure, "进入流程失败：丢失流程 " + DefaultProcedure + " ！");
                }
            }
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            if (_currentProcedure != null)
            {
                _currentProcedure.OnUpdate();

                if (_timer < 1)
                {
                    _timer += Time.deltaTime;
                }
                else
                {
                    _timer = 0;
                    _currentProcedure.OnUpdateSecond();
                }
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            _procedureInstances.Clear();
            _procedureTypes.Clear();
        }

        /// <summary>
        /// 当前流程
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                return _currentProcedure;
            }
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <typeparam name="T">流程类</typeparam>
        /// <returns>流程对象</returns>
        public T GetProcedure<T>() where T : ProcedureBase
        {
            return GetProcedure(typeof(T)) as T;
        }
        /// <summary>
        /// 获取流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>流程对象</returns>
        public ProcedureBase GetProcedure(Type type)
        {
            if (_procedureInstances.ContainsKey(type))
            {
                return _procedureInstances[type];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Procedure, "获取流程失败：不存在流程 " + type.Name + " 或者流程未激活！");
            }
        }

        /// <summary>
        /// 是否存在流程
        /// </summary>
        /// <typeparam name="T">流程类</typeparam>
        /// <returns>是否存在</returns>
        public bool IsExistProcedure<T>() where T : ProcedureBase
        {
            return IsExistProcedure(typeof(T));
        }
        /// <summary>
        /// 是否存在流程
        /// </summary>
        /// <param name="type">流程类</param>
        /// <returns>是否存在</returns>
        public bool IsExistProcedure(Type type)
        {
            return _procedureInstances.ContainsKey(type);
        }

        /// <summary>
        /// 切换流程
        /// </summary>
        /// <typeparam name="T">目标流程</typeparam>
        public void SwitchProcedure<T>() where T : ProcedureBase
        {
            SwitchProcedure(typeof(T));
        }
        /// <summary>
        /// 切换流程
        /// </summary>
        /// <param name="type">目标流程</param>
        public void SwitchProcedure(Type type)
        {
            if (_procedureInstances.ContainsKey(type))
            {
                if (_currentProcedure == _procedureInstances[type])
                {
                    return;
                }

                ProcedureBase lastProcedure = _currentProcedure;
                ProcedureBase nextProcedure = _procedureInstances[type];
                if (lastProcedure != null)
                {
                    lastProcedure.OnLeave(nextProcedure);
                }
                nextProcedure.OnEnter(lastProcedure);
                _currentProcedure = nextProcedure;
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
            int index = _procedureTypes.IndexOf(_currentProcedure.GetType());
            if (index >= _procedureTypes.Count - 1)
            {
                SwitchProcedure(_procedureTypes[0]);
            }
            else
            {
                SwitchProcedure(_procedureTypes[index + 1]);
            }
        }
        /// <summary>
        /// 切换至上一流程
        /// </summary>
        public void SwitchLastProcedure()
        {
            int index = _procedureTypes.IndexOf(_currentProcedure.GetType());
            if (index <= 0)
            {
                SwitchProcedure(_procedureTypes[_procedureTypes.Count - 1]);
            }
            else
            {
                SwitchProcedure(_procedureTypes[index - 1]);
            }
        }

        /// <summary>
        /// 切换至指定序号的流程（依据编辑器面板的序号）
        /// </summary>
        /// <param name="index">流程序号</param>
        public void SwitchTargetProcedure(int index)
        {
            index = index - 1;
            if (index >= 0 && index < _procedureTypes.Count)
            {
                SwitchProcedure(_procedureTypes[index]);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Procedure, "切换流程失败：不存在序号为 " + (index + 1).ToString() + " 的流程或者流程未激活！");
            }
        }
    }
}