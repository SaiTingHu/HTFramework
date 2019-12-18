using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 对象池管理者
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.ObjectPool)]
    public sealed class ObjectPoolManager : InternalModuleBase
    {
        /// <summary>
        /// 单个对象池上限【请勿在代码中修改】
        /// </summary>
        public int Limit = 100;

        private Dictionary<string, ObjectSpawnPool> _spawnPools = new Dictionary<string, ObjectSpawnPool>();

        public override void OnTermination()
        {
            base.OnTermination();

            ClearAll();
        }

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
        /// <param name="name">对象池名称</param>
        /// <param name="spawnTem">对象模板</param>
        /// <param name="onSpawn">对象生成时初始化委托</param>
        /// <param name="onDespawn">对象回收时处理委托</param>
        public void RegisterSpawnPool(string name, GameObject spawnTem, HTFAction<GameObject> onSpawn, HTFAction<GameObject> onDespawn)
        {
            if (!_spawnPools.ContainsKey(name))
            {
                _spawnPools.Add(name, new ObjectSpawnPool(spawnTem, Limit, onSpawn, onDespawn));
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.ObjectPool, "注册对象池失败：已存在对象池 " + name + " ！");
            }
        }
        /// <summary>
        /// 移除已注册的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        public void UnRegisterSpawnPool(string name)
        {
            if (_spawnPools.ContainsKey(name))
            {
                _spawnPools[name].Clear();
                _spawnPools.Remove(name);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.ObjectPool, "移除对象池失败：不存在对象池 " + name + " ！");
            }
        }

        /// <summary>
        /// 获取对象池中对象数量
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>对象数量</returns>
        public int GetPoolCount(string name)
        {
            if (_spawnPools.ContainsKey(name))
            {
                return _spawnPools[name].Count;
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.ObjectPool, "获取对象数量失败：不存在对象池 " + name + " ！");
            }
        }
        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>对象</returns>
        public GameObject Spawn(string name)
        {
            if (_spawnPools.ContainsKey(name))
            {
                return _spawnPools[name].Spawn();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.ObjectPool, "生成对象失败：不存在对象池 " + name + " ！");
            }
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="target">对象</param>
        public void Despawn(string name, GameObject target)
        {
            if (_spawnPools.ContainsKey(name))
            {
                _spawnPools[name].Despawn(target);
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.ObjectPool, "回收对象失败：不存在对象池 " + name + " ！");
            }
        }
        /// <summary>
        /// 批量回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="targets">对象数组</param>
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
                throw new HTFrameworkException(HTFrameworkModule.ObjectPool, "回收对象失败：不存在对象池 " + name + " ！");
            }
        }
        /// <summary>
        /// 批量回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="targets">对象集合</param>
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
                throw new HTFrameworkException(HTFrameworkModule.ObjectPool, "回收对象失败：不存在对象池 " + name + " ！");
            }
        }
        /// <summary>
        /// 清空指定的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        public void Clear(string name)
        {
            if (_spawnPools.ContainsKey(name))
            {
                _spawnPools[name].Clear();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.ObjectPool, "清空对象池失败：不存在对象池 " + name + " ！");
            }
        }
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void ClearAll()
        {
            foreach (var spawnPool in _spawnPools)
            {
                spawnPool.Value.Clear();
            }
        }
    }
}