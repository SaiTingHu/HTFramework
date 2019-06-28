using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 对象池管理者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ObjectPoolManager : ModuleManager
    {
        /// <summary>
        /// 单个对象池上限
        /// </summary>
        public int Limit = 100;

        private Dictionary<string, ObjectSpawnPool> _spawnPools = new Dictionary<string, ObjectSpawnPool>();

        /// <summary>
        /// 所有对象池
        /// </summary>
        public Dictionary<string, ObjectSpawnPool> SpawnPools
        {
            get
            {
                return _spawnPools;
            }
        }

        /// <summary>
        /// 注册对象池
        /// </summary>
        public void RegisterSpawnPool(string name, GameObject spawnTem, HTFAction<GameObject> onSpawn, HTFAction<GameObject> onDespawn)
        {
            if (!_spawnPools.ContainsKey(name))
            {
                _spawnPools.Add(name, new ObjectSpawnPool(spawnTem, Limit, onSpawn, onDespawn));
            }
            else
            {
                GlobalTools.LogError("注册对象池失败：已存在对象池 " + name + " ！");
            }
        }

        /// <summary>
        /// 移除已注册的对象池
        /// </summary>
        public void UnRegisterSpawnPool(string name)
        {
            if (_spawnPools.ContainsKey(name))
            {
                _spawnPools[name].Clear(true);
                _spawnPools.Remove(name);
            }
            else
            {
                GlobalTools.LogWarning("移除对象池失败：不存在对象池 " + name + " ！");
            }
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        public GameObject Spawn(string name)
        {
            if (_spawnPools.ContainsKey(name))
            {
                return _spawnPools[name].Spawn();
            }
            else
            {
                GlobalTools.LogError("生成对象失败：不存在对象池 " + name + " ！");
                return null;
            }
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Despawn(string name, GameObject target)
        {
            if (_spawnPools.ContainsKey(name))
            {
                _spawnPools[name].Despawn(target);
            }
            else
            {
                GlobalTools.LogError("回收对象失败：不存在对象池 " + name + " ！");
            }
        }

        /// <summary>
        /// 批量回收对象
        /// </summary>
        public void Despawns(string name, GameObject[] targets)
        {
            if (_spawnPools.ContainsKey(name))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    _spawnPools[name].Despawn(targets[i]);
                }
            }
            else
            {
                GlobalTools.LogError("回收对象失败：不存在对象池 " + name + " ！");
            }
        }

        /// <summary>
        /// 批量回收对象
        /// </summary>
        public void Despawns(string name, List<GameObject> targets)
        {
            if (_spawnPools.ContainsKey(name))
            {
                for (int i = 0; i < targets.Count; i++)
                {
                    _spawnPools[name].Despawn(targets[i]);
                }
                targets.Clear();
            }
            else
            {
                GlobalTools.LogError("回收对象失败：不存在对象池 " + name + " ！");
            }
        }

        /// <summary>
        /// 销毁所有对象池
        /// </summary>
        public void Clear()
        {
            foreach (KeyValuePair<string, ObjectSpawnPool> spawnPool in _spawnPools)
            {
                spawnPool.Value.Clear(true);
            }
            _spawnPools.Clear();
        }

        public override void Termination()
        {
            base.Termination();
        }
    }
}