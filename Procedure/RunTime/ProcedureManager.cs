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
        private Procedure _currentProcedure;
        private float _timer = 0;

        public override void Initialization()
        {
            base.Initialization();

            //创建所有已激活的流程对象
            for (int i = 0; i < ActivatedProcedures.Count; i++)
            {
                Type type = Type.GetType(ActivatedProcedures[i]);
                if (type != null)
                {
                    if (type.BaseType == typeof(Procedure))
                    {
                        if (!_procedureInstances.ContainsKey(type))
                        {
                            _procedureInstances.Add(type, Activator.CreateInstance(type) as Procedure);
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

        public override void Preparatory()
        {
            base.Preparatory();

            //流程初始化
            foreach (KeyValuePair<Type, Procedure> procedureInstance in _procedureInstances)
            {
                procedureInstance.Value.OnInit();
            }

            //进入默认流程
            if (DefaultProcedure != "")
            {
                Type type = Type.GetType(DefaultProcedure);
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

        public override void Refresh()
        {
            base.Refresh();

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

        public override void Termination()
        {
            base.Termination();

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
        public void SwitchProcedure<T>() where T : Procedure
        {
            if (_procedureInstances.ContainsKey(typeof(T)))
            {
                if (_currentProcedure == _procedureInstances[typeof(T)])
                {
                    return;
                }

                if (_currentProcedure != null)
                {
                    _currentProcedure.OnLeave();
                }

                _currentProcedure = _procedureInstances[typeof(T)];
                _currentProcedure.OnEnter();
            }
            else
            {
                GlobalTools.LogError("切换流程失败：不存在流程 " + typeof(T).Name + " 或者流程未激活！");
            }
        }
    }
}