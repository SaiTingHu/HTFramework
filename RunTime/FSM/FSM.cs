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
    public sealed class FSM : HTBehaviour
    {
        /// <summary>
        /// 是否自动注册到管理器【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal bool IsAutoRegister = true;
        /// <summary>
        /// 有限状态机数据类型【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string Data = "<None>";
        /// <summary>
        /// 当前激活的所有状态类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<string> States = new List<string>();
        /// <summary>
        /// 当前激活的所有状态名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<string> StateNames = new List<string>();
        /// <summary>
        /// 当前初始状态类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string DefaultState = "";
        /// <summary>
        /// 当前初始状态名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string DefaultStateName = "";
        /// <summary>
        /// 当前最终状态类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string FinalState = "";
        /// <summary>
        /// 当前最终状态名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string FinalStateName = "";
        /// <summary>
        /// 有限状态机名称
        /// </summary>
        public string Name = "New Finite State Machine";
        /// <summary>
        /// 有限状态机所属的组名称
        /// </summary>
        public string Group = "Default";
        /// <summary>
        /// 任意状态切换事件（上一个离开的状态、下一个进入的状态）
        /// </summary>
        public event HTFAction<FiniteStateBase, FiniteStateBase> AnyStateSwitchEvent;
        
        private FSMDataBase _data;
        private Dictionary<Type, FiniteStateBase> _stateInstances = new Dictionary<Type, FiniteStateBase>();
        private FiniteStateBase _currentState;
        private Type _defaultState;
        private Type _finalState;

        private FSM()
        {

        }

        protected override void Awake()
        {
            base.Awake();

            if (IsAutoRegister)
            {
                Main.m_FSM.RegisterFSM(this);
            }
            //加载数据类
            if (Data != "<None>")
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(Data);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(FSMDataBase)))
                    {
                        _data = Activator.CreateInstance(type) as FSMDataBase;
                        _data.StateMachine = this;
                        _data.OnInit();
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.FSM, "创建有限状态机数据类失败：数据类 " + Data + " 必须继承至有限状态机数据基类：FSMDataBase！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.FSM, "创建有限状态机数据类失败：丢失数据类 " + Data + " ！");
                }
            }
            //加载所有状态
            for (int i = 0; i < States.Count; i++)
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(States[i]);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(FiniteStateBase)))
                    {
                        if (!_stateInstances.ContainsKey(type))
                        {
                            FiniteStateBase state = Activator.CreateInstance(type) as FiniteStateBase;
                            state.StateMachine = this;
                            state.OnInit();
                            _stateInstances.Add(type, state);
                        }
                    }
                    else
                    {
                        throw new HTFrameworkException(HTFrameworkModule.FSM, "加载有限状态失败：有限状态类 " + States[i] + " 必须继承至有限状态基类：FiniteStateBase！");
                    }
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.FSM, "加载有限状态失败：丢失有限状态类 " + States[i] + " ！");
                }
            }
            //设置默认状态、最终状态
            if (DefaultState == "" || FinalState == "" || _stateInstances.Count <= 0)
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "有限状态机 " + Name + " 的状态为空！或未指定默认状态、最终状态！");
            }
            _defaultState = ReflectionToolkit.GetTypeInRunTimeAssemblies(DefaultState);
            if (_defaultState == null)
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "有限状态机 " + Name + " 丢失了默认状态 " + DefaultState + "！");
            }
            _finalState = ReflectionToolkit.GetTypeInRunTimeAssemblies(FinalState);
            if (_finalState == null)
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "有限状态机 " + Name + " 丢失了最终状态 " + FinalState + "！");
            }
        }
        private void Start()
        {
            //进入默认状态
            if (_defaultState != null)
            {
                if (_stateInstances.ContainsKey(_defaultState))
                {
                    _currentState = _stateInstances[_defaultState];
                    _currentState.OnEnter(null);
                }
                else
                {
                    throw new HTFrameworkException(HTFrameworkModule.FSM, "切换状态失败：有限状态机 " + Name + " 不存在状态 " + _defaultState.Name + "！");
                }
            }
        }
        private void Update()
        {
            if (Main.Current.Pause)
            {
                return;
            }
            
            if (_currentState != null)
            {
                _currentState.OnUpdate();
                _currentState.OnReason();
            }
        }
        private void OnDestroy()
        {
            foreach (var state in _stateInstances)
            {
                state.Value.OnTermination();
            }
            _stateInstances.Clear();

            if (_data != null)
            {
                _data.OnTermination();
            }

            if (Main.m_FSM.IsExistFSM(Name))
            {
                Main.m_FSM.UnRegisterFSM(this);
            }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public FiniteStateBase CurrentState
        {
            get
            {
                return _currentState;
            }
        }
        /// <summary>
        /// 当前数据
        /// </summary>
        public FSMDataBase CurrentData
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>状态实例</returns>
        public T GetState<T>() where T : FiniteStateBase
        {
            return GetState(typeof(T)) as T;
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="type">状态类型</param>
        /// <returns>状态实例</returns>
        public FiniteStateBase GetState(Type type)
        {
            if (_stateInstances.ContainsKey(type))
            {
                return _stateInstances[type];
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "获取状态失败：有限状态机 " + Name + " 不存在状态 " + type.Name + "！");
            }
        }

        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        /// <returns>是否存在</returns>
        public bool IsExistState<T>() where T : FiniteStateBase
        {
            return IsExistState(typeof(T));
        }
        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <param name="type">状态类型</param>
        /// <returns>是否存在</returns>
        public bool IsExistState(Type type)
        {
            return _stateInstances.ContainsKey(type);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        public void SwitchState<T>() where T : FiniteStateBase
        {
            SwitchState(typeof(T));
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="type">状态类型</param>
        public void SwitchState(Type type)
        {
            if (_stateInstances.ContainsKey(type))
            {
                if (_currentState == _stateInstances[type])
                {
                    return;
                }

                FiniteStateBase lastState = _currentState;
                FiniteStateBase nextState = _stateInstances[type];
                if (lastState != null)
                {
                    lastState.OnLeave(nextState);
                }
                nextState.OnEnter(lastState);
                _currentState = nextState;

                AnyStateSwitchEvent?.Invoke(lastState, nextState);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "切换状态失败：有限状态机 " + Name + " 不存在状态 " + type.Name + "！");
            }
        }

        /// <summary>
        /// 终止状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        public void TerminationState<T>() where T : FiniteStateBase
        {
            TerminationState(typeof(T));
        }
        /// <summary>
        /// 终止状态
        /// </summary>
        /// <param name="type">状态类型</param>
        public void TerminationState(Type type)
        {
            if (type == _defaultState || type == _finalState)
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "终止状态失败：有限状态机 " + Name + " 无法终止状态 " + type.Name + "！因为该状态为初始状态或最终状态！");
            }

            if (_stateInstances.ContainsKey(type))
            {
                if (_currentState == _stateInstances[type])
                {
                    throw new HTFrameworkException(HTFrameworkModule.FSM, "终止状态失败：有限状态机 " + Name + " 无法终止状态 " + type.Name + "！因为当前正处于该状态！");
                }

                _stateInstances[type].OnTermination();
                _stateInstances.Remove(type);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "终止状态失败：有限状态机 " + Name + " 不存在状态 " + type.Name + "！");
            }
        }

        /// <summary>
        /// 附加状态
        /// </summary>
        /// <typeparam name="T">状态类型</typeparam>
        public void AppendState<T>() where T : FiniteStateBase
        {
            AppendState(typeof(T));
        }
        /// <summary>
        /// 附加状态
        /// </summary>
        /// <param name="type">状态类型</param>
        public void AppendState(Type type)
        {
            if (!_stateInstances.ContainsKey(type))
            {
                FiniteStateBase state = Activator.CreateInstance(type) as FiniteStateBase;
                state.StateMachine = this;
                state.OnInit();
                _stateInstances.Add(type, state);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.FSM, "附加状态失败：有限状态机 " + Name + " 已存在状态 " + type.Name + "！");
            }
        }

        /// <summary>
        /// 重生，状态机恢复为初始状态
        /// </summary>
        public void Renewal()
        {
            if (_data != null)
            {
                _data.OnRenewal();
            }

            SwitchState(_defaultState);
        }
        /// <summary>
        /// 完结，状态机进入最终状态
        /// </summary>
        public void Final()
        {
            if (_data != null)
            {
                _data.OnFinal();
            }

            SwitchState(_finalState);
        }
    }
}