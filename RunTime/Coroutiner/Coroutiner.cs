using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 协程调度者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Coroutiner : ModuleManager
    {
        public Dictionary<string, CoroutineEnumerator> CoroutineEnumerators { get; } = new Dictionary<string, CoroutineEnumerator>();
        public Dictionary<Delegate, List<CoroutineEnumerator>> Warehouse { get; } = new Dictionary<Delegate, List<CoroutineEnumerator>>();

        /// <summary>
        /// 运行协程
        /// </summary>
        /// <param name="action">协程方法</param>
        /// <returns>协程迭代器ID</returns>
        public string Run(CoroutineAction action)
        {
            CoroutineEnumerator coroutineEnumerator = Main.m_ReferencePool.Spawn<CoroutineEnumerator>().Fill(action, null);
            coroutineEnumerator.Run();
            CoroutineEnumerators.Add(coroutineEnumerator.ID, coroutineEnumerator);
            DepositWarehouse(coroutineEnumerator);
            return coroutineEnumerator.ID;
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
            CoroutineEnumerator coroutineEnumerator = Main.m_ReferencePool.Spawn<CoroutineEnumerator>().Fill(action, new object[] { arg });
            coroutineEnumerator.Run();
            CoroutineEnumerators.Add(coroutineEnumerator.ID, coroutineEnumerator);
            DepositWarehouse(coroutineEnumerator);
            return coroutineEnumerator.ID;
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
            CoroutineEnumerator coroutineEnumerator = Main.m_ReferencePool.Spawn<CoroutineEnumerator>().Fill(action, new object[] { arg1, arg2 });
            coroutineEnumerator.Run();
            CoroutineEnumerators.Add(coroutineEnumerator.ID, coroutineEnumerator);
            DepositWarehouse(coroutineEnumerator);
            return coroutineEnumerator.ID;
        }
        /// <summary>
        /// 重启协程
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        public void Rerun(string id)
        {
            if (!CoroutineEnumerators.ContainsKey(id))
            {
                GlobalTools.LogError("重启协程失败：不存在ID为 " + id + " 的协程！");
                return;
            }
            CoroutineEnumerators[id].Rerun();
        }
        /// <summary>
        /// 终止指定ID的协程
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        public void Stop(string id)
        {
            if (!CoroutineEnumerators.ContainsKey(id))
            {
                GlobalTools.LogError("终止协程失败：不存在ID为 " + id + " 的协程！");
                return;
            }
            CoroutineEnumerators[id].Stop();
        }
        /// <summary>
        /// 终止指定类型的所有协程
        /// </summary>
        /// <param name="action">协程方法</param>
        public void Stop(Delegate action)
        {
            if (!Warehouse.ContainsKey(action))
            {
                GlobalTools.LogError("终止协程失败：不存在 " + action.Method.Name + " 类型的协程！");
                return;
            }
            for (int i = 0; i < Warehouse[action].Count; i++)
            {
                Warehouse[action][i].Stop();
            }
        }
        /// <summary>
        /// 是否运行中
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        /// <returns>是否运行中</returns>
        public bool IsRunning(string id)
        {
            if (!CoroutineEnumerators.ContainsKey(id))
            {
                return false;
            }
            return CoroutineEnumerators[id].IsRunning();
        }
        /// <summary>
        /// 清理所有未运行的协程
        /// </summary>
        public void ClearNotRunning()
        {
            Dictionary<string, CoroutineEnumerator> notRunnings = new Dictionary<string, CoroutineEnumerator>();
            foreach (KeyValuePair<string, CoroutineEnumerator> enumerator in CoroutineEnumerators)
            {
                if (!enumerator.Value.IsRunning())
                {
                    notRunnings.Add(enumerator.Key, enumerator.Value);
                }
            }
            foreach (KeyValuePair<string, CoroutineEnumerator> enumerator in notRunnings)
            {
                CoroutineEnumerators.Remove(enumerator.Key);
                RemoveWarehouse(enumerator.Value);
                Main.m_ReferencePool.Despawn(enumerator.Value);
            }
            notRunnings = null;
        }

        private void DepositWarehouse(CoroutineEnumerator enumerator)
        {
            if (!Warehouse.ContainsKey(enumerator.TargetAction))
            {
                Warehouse.Add(enumerator.TargetAction, new List<CoroutineEnumerator>());
            }
            Warehouse[enumerator.TargetAction].Add(enumerator);
        }
        private void RemoveWarehouse(CoroutineEnumerator enumerator)
        {
            if (Warehouse.ContainsKey(enumerator.TargetAction))
            {
                Warehouse[enumerator.TargetAction].Remove(enumerator);
                if (Warehouse[enumerator.TargetAction].Count <= 0)
                {
                    Warehouse.Remove(enumerator.TargetAction);
                }
            }
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

            public DateTime CreationTime { get; private set; }
            public DateTime StoppingTime { get; private set; }
            public double ElapsedTime { get; private set; }
            public int RerunNumber { get; private set; }

            private IEnumerator _enumerator;
            private Coroutine _coroutine;
            private StackTrace _stackTrace;
            private CoroutineState _state;

            public CoroutineState State
            {
                get
                {
                    return _state;
                }
                private set
                {
                    _state = value;
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
                }
            }
            public string StackTraceInfo
            {
                get
                {
                    return _stackTrace.ToString();
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
                    _stackTrace = new StackTrace();
                    State = CoroutineState.Running;
                    RerunNumber = 1;
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
                    _stackTrace = new StackTrace();
                    State = CoroutineState.Running;
                    RerunNumber += 1;
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
                _stackTrace = null;
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
