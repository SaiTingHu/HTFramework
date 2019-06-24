using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    [AddComponentMenu("HTFramework/FSM")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-900)]
    public sealed class FSM : MonoBehaviour
    {
        public string Name = "New Finite State Machine";
        public string Data = "<None>";
        public List<string> States = new List<string>();
        public List<string> StateNames = new List<string>();
        public string DefaultState = "";
        public string DefaultStateName = "";

        private FSMData _data;
        private Dictionary<Type, FiniteState> _stateInstances = new Dictionary<Type, FiniteState>();
        private FiniteState _currentState;

        private void Awake()
        {
            Main.m_FSM.RegisterFSM(this);
            //加载数据类
            if (Data != "<None>")
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(Data);
                if (type != null)
                {
                    if (type.BaseType == typeof(FSMData))
                    {
                        _data = Activator.CreateInstance(type) as FSMData;
                        _data.StateMachine = this;
                        _data.OnInit();
                    }
                    else
                    {
                        GlobalTools.LogError("创建数据类失败：数据类 " + Data + " 必须继承至有限状态机数据基类：FSMData！");
                    }
                }
                else
                {
                    GlobalTools.LogError("创建数据类失败：丢失数据类 " + Data + "！");
                }
            }
            //加载所有状态
            for (int i = 0; i < States.Count; i++)
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(States[i]);
                if (type != null)
                {
                    if (type.BaseType == typeof(FiniteState))
                    {
                        if (!_stateInstances.ContainsKey(type))
                        {
                            FiniteState state = Activator.CreateInstance(type) as FiniteState;
                            state.StateMachine = this;
                            state.OnInit();
                            _stateInstances.Add(type, state);
                        }
                    }
                    else
                    {
                        GlobalTools.LogError("加载有限状态失败：有限状态 " + States[i] + " 必须继承至有限状态基类：FiniteState！");
                    }
                }
                else
                {
                    GlobalTools.LogError("加载有限状态失败：丢失有限状态 " + States[i] + "！");
                }
            }
            //进入默认状态
            if (DefaultState == "" || _stateInstances.Count <= 0)
            {
                GlobalTools.LogError("有限状态机 " + Name + " 的状态为空！或未指定默认状态！");
                return;
            }
            Type dtype = GlobalTools.GetTypeInRunTimeAssemblies(DefaultState);
            if (dtype != null)
            {
                if (_stateInstances.ContainsKey(dtype))
                {
                    _currentState = _stateInstances[dtype];
                    _currentState.OnEnter();
                }
                else
                {
                    GlobalTools.LogError("切换状态失败：有限状态机 " + Name + " 不存在状态 " + dtype.Name + "！");
                }
            }
            else
            {
                GlobalTools.LogError("切换状态失败：丢失有限状态 " + DefaultState + "！");
            }
        }

        private void Update()
        {
            if (_currentState != null)
            {
                _currentState.OnUpdate();
                _currentState.OnReason();
            }
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<Type, FiniteState> state in _stateInstances)
            {
                state.Value.OnTermination();
            }

            Main.m_FSM.UnRegisterFSM(this);
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public FiniteState CurrentState
        {
            get
            {
                return _currentState;
            }
        }

        /// <summary>
        /// 当前数据
        /// </summary>
        public FSMData CurrentData
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        public T GetState<T>() where T : FiniteState
        {
            if (_stateInstances.ContainsKey(typeof(T)))
            {
                return _stateInstances[typeof(T)] as T;
            }
            else
            {
                GlobalTools.LogError("获取状态失败：有限状态机 " + Name + " 不存在状态 " + typeof(T).Name + "！");
                return null;
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void SwitchState<T>() where T : FiniteState
        {
            if (_stateInstances.ContainsKey(typeof(T)))
            {
                if (_currentState == _stateInstances[typeof(T)])
                {
                    return;
                }

                if (_currentState != null)
                {
                    _currentState.OnLeave();
                }

                _currentState = _stateInstances[typeof(T)];
                _currentState.OnEnter();
            }
            else
            {
                GlobalTools.LogError("切换状态失败：有限状态机 " + Name + " 不存在状态 " + typeof(T).Name + "！");
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void SwitchState(Type type)
        {
            if (_stateInstances.ContainsKey(type))
            {
                if (_currentState == _stateInstances[type])
                {
                    return;
                }

                if (_currentState != null)
                {
                    _currentState.OnLeave();
                }

                _currentState = _stateInstances[type];
                _currentState.OnEnter();
            }
            else
            {
                GlobalTools.LogError("切换状态失败：有限状态机 " + Name + " 不存在状态 " + type.Name + "！");
            }
        }

        /// <summary>
        /// 终止状态
        /// </summary>
        public void TerminationState<T>() where T : FiniteState
        {
            if (_stateInstances.ContainsKey(typeof(T)))
            {
                if (_currentState == _stateInstances[typeof(T)])
                {
                    GlobalTools.LogError("终止状态失败：有限状态机 " + Name + " 无法终止状态 " + typeof(T).Name + "！因为当前正处于该状态！");
                    return;
                }

                _stateInstances[typeof(T)].OnTermination();
                _stateInstances.Remove(typeof(T));
            }
            else
            {
                GlobalTools.LogError("终止状态失败：有限状态机 " + Name + " 不存在状态 " + typeof(T).Name + "！");
            }
        }

        /// <summary>
        /// 附加状态
        /// </summary>
        public void AppendState<T>() where T : FiniteState
        {
            if (!_stateInstances.ContainsKey(typeof(T)))
            {
                FiniteState state = Activator.CreateInstance(typeof(T)) as FiniteState;
                state.StateMachine = this;
                state.OnInit();
                _stateInstances.Add(typeof(T), state);
            }
            else
            {
                GlobalTools.LogError("附加状态失败：有限状态机 " + Name + " 已存在状态 " + typeof(T).Name + "！");
            }
        }
    }
}
