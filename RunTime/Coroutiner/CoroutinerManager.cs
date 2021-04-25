using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 协程调度器
    /// </summary>
    [InternalModule(HTFrameworkModule.Coroutiner)]
    public sealed class CoroutinerManager : InternalModuleBase<ICoroutinerHelper>
    {
        /// <summary>
        /// 运行协程
        /// </summary>
        /// <param name="action">协程方法</param>
        /// <returns>协程迭代器ID</returns>
        public string Run(CoroutineAction action)
        {
            return _helper.Run(action);
        }
        /// <summary>
        /// 运行协程
        /// </summary>
        /// <typeparam name="T">协程方法的参数类型</typeparam>
        /// <param name="action">协程方法</param>
        /// <param name="arg">协程方法的参数</param>
        /// <returns>协程迭代器ID</returns>
        public string Run<T>(CoroutineAction<T> action, T arg)
        {
            return _helper.Run(action, arg);
        }
        /// <summary>
        /// 运行协程
        /// </summary>
        /// <typeparam name="T1">协程方法的参数类型</typeparam>
        /// <typeparam name="T2">协程方法的参数类型</typeparam>
        /// <param name="action">协程方法</param>
        /// <param name="arg1">协程方法的参数</param>
        /// <param name="arg2">协程方法的参数</param>
        /// <returns>协程迭代器ID</returns>
        public string Run<T1, T2>(CoroutineAction<T1, T2> action, T1 arg1, T2 arg2)
        {
            return _helper.Run(action, arg1, arg2);
        }
        /// <summary>
        /// 重启协程
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        public void Rerun(string id)
        {
            _helper.Rerun(id);
        }
        /// <summary>
        /// 终止指定ID的协程
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        public void Stop(string id)
        {
            _helper.Stop(id);
        }
        /// <summary>
        /// 终止指定类型的所有协程
        /// </summary>
        /// <param name="action">协程方法</param>
        public void Stop(Delegate action)
        {
            _helper.Stop(action);
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        /// <returns>是否存在</returns>
        public bool IsExist(string id)
        {
            return _helper.IsExist(id);
        }
        /// <summary>
        /// 是否运行中
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        /// <returns>是否运行中</returns>
        public bool IsRunning(string id)
        {
            return _helper.IsRunning(id);
        }
        /// <summary>
        /// 清理所有未运行的协程
        /// </summary>
        public void ClearNotRunning()
        {
            _helper.ClearNotRunning();
        }

        /// <summary>
        /// 协程迭代器
        /// </summary>
        public sealed class CoroutineEnumerator : IEnumerator, IReference
        {
            public string ID { get; private set; }
            public object TargetObject { get; private set; }
            public Delegate TargetAction { get; private set; }
            public object[] Args { get; private set; }
            
            private IEnumerator _enumerator;
            private Coroutine _coroutine;
            private CoroutineState _state;

#if UNITY_EDITOR
            public StackTrace StackTraceInfo { get; private set; }
            public DateTime CreationTime { get; private set; }
            public DateTime StoppingTime { get; private set; }
            public double ElapsedTime { get; private set; }
            public int RerunNumber { get; private set; }

            public void RerunInEditor()
            {
                if (State == CoroutineState.Running)
                {
                    Main.m_Coroutiner.StopCoroutine(_coroutine);
                    State = CoroutineState.Stoped;
                }

                _enumerator = TargetAction.Method.Invoke(TargetObject, Args) as IEnumerator;
                if (_enumerator != null)
                {
                    _coroutine = Main.m_Coroutiner.StartCoroutine(this);
                    State = CoroutineState.Running;
                    RerunNumber += 1;
                }
            }
#endif
            public CoroutineState State
            {
                get
                {
                    return _state;
                }
                private set
                {
                    _state = value;
#if UNITY_EDITOR
                    switch (_state)
                    {
                        case CoroutineState.Running:
                            CreationTime = DateTime.Now;
                            break;
                        case CoroutineState.Stoped:
                            StoppingTime = DateTime.Now;
                            ElapsedTime = (StoppingTime - CreationTime).TotalSeconds;
                            break;
                        case CoroutineState.Finish:
                            StoppingTime = DateTime.Now;
                            ElapsedTime = (StoppingTime - CreationTime).TotalSeconds;
                            break;
                    }
#endif
                }
            }
            
            public CoroutineEnumerator Fill(Delegate targetAction, object[] args)
            {
                ID = Guid.NewGuid().ToString();
                TargetObject = targetAction.Target;
                TargetAction = targetAction;
                Args = args;
                return this;
            }
            public void Run()
            {
                _enumerator = TargetAction.Method.Invoke(TargetObject, Args) as IEnumerator;
                if (_enumerator != null)
                {
                    _coroutine = Main.m_Coroutiner.StartCoroutine(this);
                    State = CoroutineState.Running;
#if UNITY_EDITOR
                    StackTraceInfo = new StackTrace(true);
                    RerunNumber = 1;
#endif
                }
            }
            public void Rerun()
            {
                if (State == CoroutineState.Running)
                {
                    Main.m_Coroutiner.StopCoroutine(_coroutine);
                    State = CoroutineState.Stoped;
                }

                _enumerator = TargetAction.Method.Invoke(TargetObject, Args) as IEnumerator;
                if (_enumerator != null)
                {
                    _coroutine = Main.m_Coroutiner.StartCoroutine(this);
                    State = CoroutineState.Running;
#if UNITY_EDITOR
                    StackTraceInfo = new StackTrace(true);
                    RerunNumber += 1;
#endif
                }
            }
            public void Stop()
            {
                if (State == CoroutineState.Running)
                {
                    Main.m_Coroutiner.StopCoroutine(_coroutine);
                    State = CoroutineState.Stoped;
                }
            }
            public bool IsRunning()
            {
                return State == CoroutineState.Running;
            }
            public object Current
            {
                get
                {
                    return _enumerator.Current;
                }
            }
            public bool MoveNext()
            {
                bool isNext = _enumerator.MoveNext();

                if (!isNext)
                {
                    State = CoroutineState.Finish;
                }

                return isNext;
            }
            public void Reset()
            {
                TargetObject = null;
                TargetAction = null;
                Args = null;
                _enumerator = null;
                _coroutine = null;
#if UNITY_EDITOR
                StackTraceInfo = null;
#endif
            }
        }

        /// <summary>
        /// 协程状态
        /// </summary>
        public enum CoroutineState
        {
            /// <summary>
            /// 运行中
            /// </summary>
            Running,
            /// <summary>
            /// 外部原因终止
            /// </summary>
            Stoped,
            /// <summary>
            /// 运行完毕
            /// </summary>
            Finish
        }
    }
}