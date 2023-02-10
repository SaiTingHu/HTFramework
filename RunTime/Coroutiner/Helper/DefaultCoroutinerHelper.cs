using System;
using System.Collections.Generic;
using static HT.Framework.CoroutinerManager;

namespace HT.Framework
{
    /// <summary>
    /// 默认的协程调度器助手
    /// </summary>
    public sealed class DefaultCoroutinerHelper : ICoroutinerHelper
    {
        /// <summary>
        /// 协程调度器
        /// </summary>
        public IModuleManager Module { get; set; }
        /// <summary>
        /// 所有协程迭代器
        /// </summary>
        public Dictionary<string, CoroutineEnumerator> CoroutineEnumerators { get; } = new Dictionary<string, CoroutineEnumerator>();
        /// <summary>
        /// 迭代器仓库
        /// </summary>
        public Dictionary<Delegate, List<CoroutineEnumerator>> Warehouse { get; } = new Dictionary<Delegate, List<CoroutineEnumerator>>();

        /// <summary>
        /// 初始化助手
        /// </summary>
        public void OnInit()
        {
            
        }
        /// <summary>
        /// 助手准备工作
        /// </summary>
        public void OnReady()
        {

        }
        /// <summary>
        /// 刷新助手
        /// </summary>
        public void OnUpdate()
        {
           
        }
        /// <summary>
        /// 终结助手
        /// </summary>
        public void OnTerminate()
        {

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
                Log.Warning($"重启协程失败：不存在ID为 {id} 的协程！");
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
                Log.Warning($"终止协程失败：不存在ID为 {id} 的协程！");
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
            if (Warehouse.ContainsKey(action))
            {
                for (int i = 0; i < Warehouse[action].Count; i++)
                {
                    Warehouse[action][i].Stop();
                }
            }
        }
        /// <summary>
        /// 是否存在指定ID的协程
        /// </summary>
        /// <param name="id">协程迭代器ID</param>
        /// <returns>是否存在</returns>
        public bool IsExist(string id)
        {
            return CoroutineEnumerators.ContainsKey(id);
        }
        /// <summary>
        /// 指定ID的协程是否运行中
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
            foreach (var enumerator in CoroutineEnumerators)
            {
                if (!enumerator.Value.IsRunning())
                {
                    notRunnings.Add(enumerator.Key, enumerator.Value);
                }
            }
            foreach (var enumerator in notRunnings)
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
    }
}