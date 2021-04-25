using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 对象池管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.ObjectPool)]
    public sealed class ObjectPoolManager : InternalModuleBase<IObjectPoolHelper>
    {
        /// <summary>
        /// 单个对象池上限【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int Limit = 100;
        
        /// <summary>
        /// 注册对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="spawnTem">对象模板</param>
        /// <param name="onSpawn">对象生成时初始化委托</param>
        /// <param name="onDespawn">对象回收时处理委托</param>
        /// <param name="limit">对象池上限，等于0时，表示使用默认值</param>
        public void RegisterSpawnPool(string name, GameObject spawnTem, HTFAction<GameObject> onSpawn = null, HTFAction<GameObject> onDespawn = null, int limit = 0)
        {
            _helper.RegisterSpawnPool(name, spawnTem, onSpawn, onDespawn, limit);
        }
        /// <summary>
        /// 是否存在指定名称的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistSpawnPool(string name)
        {
            return _helper.IsExistSpawnPool(name);
        }
        /// <summary>
        /// 移除已注册的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        public void UnRegisterSpawnPool(string name)
        {
            _helper.UnRegisterSpawnPool(name);
        }

        /// <summary>
        /// 获取对象池中对象数量
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>对象数量</returns>
        public int GetPoolCount(string name)
        {
            return _helper.GetPoolCount(name);
        }
        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>对象</returns>
        public GameObject Spawn(string name)
        {
            return _helper.Spawn(name);
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="target">对象</param>
        public void Despawn(string name, GameObject target)
        {
            _helper.Despawn(name, target);
        }
        /// <summary>
        /// 批量回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="targets">对象数组</param>
        public void Despawns(string name, GameObject[] targets)
        {
            _helper.Despawns(name, targets);
        }
        /// <summary>
        /// 批量回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="targets">对象集合</param>
        public void Despawns(string name, List<GameObject> targets)
        {
            _helper.Despawns(name, targets);
        }
        /// <summary>
        /// 清空指定的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        public void Clear(string name)
        {
            _helper.Clear(name);
        }
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        public void ClearAll()
        {
            _helper.ClearAll();
        }
    }
}