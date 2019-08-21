using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 流程管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ProcedureManager : ModuleManager
    {
        [SerializeField]
        public List<string> ActivatedProcedures = new List<string>();
        public string DefaultProcedure = "";

        private Dictionary<Type, Procedure> _procedureInstances = new Dictionary<Type, Procedure>();
        private List<Type> _procedureTypes = new List<Type>();
        private Procedure _currentProcedure;
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
                    if (type.IsSubclassOf(typeof(Procedure)))
                    {
                        if (!_procedureInstances.ContainsKey(type))
                        {
                            _procedureInstances.Add(type, Activator.CreateInstance(type) as Procedure);
                            _procedureTypes.Add(type);
                        }
                    }
                    else
                    {
                        GlobalTools.LogError("创建流程失败：流程 " + ActivatedProcedures[i] + " 必须继承至流程基类：Procedure！");
                    }
                }
                else
                {
                    GlobalTools.LogError("创建流程失败：丢失流程 " + ActivatedProcedures[i] + "！");
                }
            }
        }

        public override void OnPreparatory()
        {
            base.OnPreparatory();

            //流程初始化
            foreach (KeyValuePair<Type, Procedure> procedureInstance in _procedureInstances)
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
                        _currentProcedure.OnEnter();
                    }
                    else
                    {
                        GlobalTools.LogError("进入流程失败：不存在流程 " + type.Name + " 或者流程未激活！");
                    }
                }
                else
                {
                    GlobalTools.LogError("进入流程失败：丢失流程 " + DefaultProcedure + "！");
                }
            }
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            //流程帧刷新
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
        }

        /// <summary>
        /// 当前流程
        /// </summary>
        public Procedure CurrentProcedure
        {
            get
            {
                return _currentProcedure;
            }
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        public T GetProcedure<T>() where T : Procedure
        {
            if (_procedureInstances.ContainsKey(typeof(T)))
            {
                return _procedureInstances[typeof(T)] as T;
            }
            else
            {
                GlobalTools.LogError("获取流程失败：不存在流程 " + typeof(T).Name + " 或者流程未激活！");
                return null;
            }
        }

        /// <summary>
        /// 切换流程
        /// </summary>
        /// <typeparam name="T">目标流程</typeparam>
        public void SwitchProcedure<T>() where T : Procedure
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

                if (_currentProcedure != null)
                {
                    _currentProcedure.OnLeave();
                }

                _currentProcedure = _procedureInstances[type];
                _currentProcedure.OnEnter();
            }
            else
            {
                GlobalTools.LogError("切换流程失败：不存在流程 " + type.Name + " 或者流程未激活！");
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
            if (index - 1 >= 0 && index - 1 < _procedureTypes.Count)
            {
                SwitchProcedure(_procedureTypes[index - 1]);
            }
            else
            {
                GlobalTools.LogError("切换流程失败：不存在序号为 " + index + " 的流程或者流程未激活！");
            }
        }
    }
}