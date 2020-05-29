using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 对象池管理器的助手接口
    /// </summary>
    public interface IObjectPoolHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有对象池
        /// </summary>
        Dictionary<string, ObjectSpawnPool> SpawnPools { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 终结
        /// </summary>
        void OnTermination();

        /// <summary>
        /// 注册对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="spawnTem">对象模板</param>
        /// <param name="onSpawn">对象生成时初始化委托</param>
        /// <param name="onDespawn">对象回收时处理委托</param>
        /// <param name="limit">对象池上限，等于0时，表示使用默认值</param>
        void RegisterSpawnPool(string name, GameObject spawnTem, HTFAction<GameObject> onSpawn, HTFAction<GameObject> onDespawn, int limit);
        /// <summary>
        /// 是否存在指定名称的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>是否存在</returns>
        bool IsExistSpawnPool(string name);
        /// <summary>
        /// 移除已注册的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        void UnRegisterSpawnPool(string name);

        /// <summary>
        /// 获取对象池中对象数量
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>对象数量</returns>
        int GetPoolCount(string name);
        /// <summary>
        /// 生成对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <returns>对象</returns>
        GameObject Spawn(string name);
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="target">对象</param>
        void Despawn(string name, GameObject target);
        /// <summary>
        /// 批量回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="targets">对象数组</param>
        void Despawns(string name, GameObject[] targets);
        /// <summary>
        /// 批量回收对象
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="targets">对象集合</param>
        void Despawns(string name, List<GameObject> targets);
        /// <summary>
        /// 清空指定的对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        void Clear(string name);
        /// <summary>
        /// 清空所有对象池
        /// </summary>
        void ClearAll();
    }
}