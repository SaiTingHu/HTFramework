using System;
using System.Collections.Generic;
using System.Reflection;
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
        /// 当前激活的所有状态类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<string> States = new List<string>();
        /// <summary>
        /// 当前初始状态类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string DefaultState = null;
        /// <summary>
        /// 当前最终状态类名【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string FinalState = null;
        /// <summary>
        /// 有限状态机数据类型【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal string Data = "<None>";
        /// <summary>
        /// 有限状态机参数【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal FSMArgsBase Args;
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

        private Dictionary<Type, FiniteStateBase> _stateInstances = new Dictionary<Type, FiniteStateBase>();
        private Type _defaultState;
        private Type _finalState;
        private bool _isAutomate;
        private bool _isSupportedDataDriver;

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
                        CurrentData = Activator.CreateInstance(type) as FSMDataBase;
                        CurrentData.StateMachine = this;
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
            if (string.IsNullOrEmpty(DefaultState) || string.IsNullOrEmpty(FinalState) || _stateInstances.Count <= 0)
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
            _isAutomate = CurrentData != null ? CurrentData.IsAutomate : false;
            _isSupportedDataDriver = CurrentData != null ? CurrentData.IsSupportedDataDriver : false;

            if (Args != null)
            {
                Args.StateMachine = this;
            }

            DoAutomaticTask();
        }
        private void Start()
        {
            if (CurrentData != null)
            {
                CurrentData.OnInit();
            }
            foreach(var state in _stateInstances)
            {
                state.Value.OnInit();
            }
            if (_defaultState != null)
            {
                SwitchState(_defaultState);
            }
        }
        private void Update()
        {
            if (Main.Current.Pause)
            {
                return;
            }
            
            if (CurrentState != null)
            {
                CurrentState.OnUpdate();
                CurrentState.OnReason();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var state in _stateInstances)
            {
                state.Value.OnTermination();
            }
            _stateInstances.Clear();

            if (CurrentData != null)
            {
                CurrentData.OnTermination();
            }

            if (Main.m_FSM.IsExistFSM(Name))
            {
                Main.m_FSM.UnRegisterFSM(this);
            }
        }

        /// <summary>
        /// 当前状态
        /// </summary>
        public FiniteStateBase CurrentState { get; private set; }
        /// <summary>
        /// 当前数据
        /// </summary>
        public FSMDataBase CurrentData { get; private set; }
        /// <summary>
        /// 当前参数
        /// </summary>
        public FSMArgsBase CurrentArgs
        {
            get
            {
                return Args;
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
                return null;
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
                if (CurrentState == _stateInstances[type])
                {
                    return;
                }

                FiniteStateBase lastState = CurrentState;
                FiniteStateBase nextState = _stateInstances[type];
                if (lastState != null)
                {
                    lastState.OnLeave(nextState);
                }
                nextState.OnEnter(lastState);
                CurrentState = nextState;

                AnyStateSwitchEvent?.Invoke(lastState, nextState);
            }
            else
            {
                Log.Warning("切换状态失败：有限状态机 " + Name + " 不存在状态 " + type.Name + "！");
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
                Log.Warning("终止状态失败：有限状态机 " + Name + " 无法终止状态 " + type.Name + "！因为该状态为初始状态或最终状态！");
                return;
            }

            if (_stateInstances.ContainsKey(type))
            {
                if (CurrentState == _stateInstances[type])
                {
                    Log.Warning("终止状态失败：有限状态机 " + Name + " 无法终止状态 " + type.Name + "！因为当前正处于该状态！");
                    return;
                }

                _stateInstances[type].OnTermination();
                _stateInstances.Remove(type);
            }
            else
            {
                Log.Warning("终止状态失败：有限状态机 " + Name + " 不存在状态 " + type.Name + "！");
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
                DoAutomaticTaskOfState(type, state);
                state.OnInit();
                _stateInstances.Add(type, state);
            }
            else
            {
                Log.Warning("附加状态失败：有限状态机 " + Name + " 已存在状态 " + type.Name + "！");
            }
        }

        /// <summary>
        /// 重生，状态机恢复为初始状态
        /// </summary>
        public void Renewal()
        {
            if (CurrentData != null)
            {
                CurrentData.OnRenewal();
            }

            SwitchState(_defaultState);
        }
        /// <summary>
        /// 完结，状态机进入最终状态
        /// </summary>
        public void Final()
        {
            if (CurrentData != null)
            {
                CurrentData.OnFinal();
            }

            SwitchState(_finalState);
        }

        /// <summary>
        /// 进行自动化任务
        /// </summary>
        private void DoAutomaticTask()
        {
            if (_isAutomate)
            {
                DoAutomaticTaskOfData(CurrentData);

                if (Args != null)
                {
                    DoAutomaticTaskOfArgs(Args);
                }

                foreach (var state in _stateInstances)
                {
                    DoAutomaticTaskOfState(state.Key, state.Value);
                }
            }
        }
        /// <summary>
        /// 进行自动化任务（数据）
        /// </summary>
        private void DoAutomaticTaskOfData(FSMDataBase fsmData)
        {
            if (_isAutomate)
            {
                FieldInfo[] fieldInfos = AutomaticTask.GetAutomaticFields(fsmData.GetType());
                AutomaticTask.ApplyObjectPath(fsmData, fieldInfos);

                if (_isSupportedDataDriver)
                {
                    AutomaticTask.ApplyDataBinding(fsmData, fieldInfos);
                }
            }
        }
        /// <summary>
        /// 进行自动化任务（参数）
        /// </summary>
        private void DoAutomaticTaskOfArgs(FSMArgsBase fsmArgs)
        {
            if (_isAutomate)
            {
                FieldInfo[] fieldInfos = AutomaticTask.GetAutomaticFields(fsmArgs.GetType());
                AutomaticTask.ApplyObjectPath(fsmArgs, fieldInfos);

                if (_isSupportedDataDriver)
                {
                    AutomaticTask.ApplyDataBinding(fsmArgs, fieldInfos);
                }
            }
        }
        /// <summary>
        /// 进行自动化任务（状态）
        /// </summary>
        private void DoAutomaticTaskOfState(Type type, FiniteStateBase finiteState)
        {
            if (_isAutomate)
            {
                FieldInfo[] fieldInfos = AutomaticTask.GetAutomaticFields(type);
                AutomaticTask.ApplyObjectPath(finiteState, fieldInfos);

                if (_isSupportedDataDriver)
                {
                    AutomaticTask.ApplyDataBinding(finiteState, fieldInfos);
                }
            }
        }
    }
}