using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 对象池
    /// </summary>
    public sealed class ObjectSpawnPool
    {
        private GameObject _spawnTem;
        private int _limit = 100;
        private Queue<GameObject> _objectQueue = new Queue<GameObject>();
        private HTFAction<GameObject> _onSpawn;
        private HTFAction<GameObject> _onDespawn;

        internal ObjectSpawnPool(GameObject spawnTem, int limit, HTFAction<GameObject> onSpawn, HTFAction<GameObject> onDespawn)
        {
            _spawnTem = spawnTem;
            _limit = limit;
            _onSpawn = onSpawn;
            _onDespawn = onDespawn;
        }

        /// <summary>
        /// 对象数量
        /// </summary>
        public int Count
        {
            get
            {
                return _objectQueue.Count;
            }
        }

        /// <summary>
        /// 生成对象
        /// </summary>
        /// <returns>对象</returns>
        public GameObject Spawn()
        {
            GameObject obj;
            if (_objectQueue.Count > 0)
            {
                obj = _objectQueue.Dequeue();
            }
            else
            {
                obj = Main.CloneGameObject(_spawnTem);
            }

            obj.SetActive(true);

            _onSpawn?.Invoke(obj);

            return obj;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj">对象</param>
        public void Despawn(GameObject obj)
        {
            if (_objectQueue.Count >= _limit)
            {
                Main.Kill(obj);
            }
            else
            {
                obj.SetActive(false);

                _onDespawn?.Invoke(obj);

                _objectQueue.Enqueue(obj);
            }
        }

        /// <summary>
        /// 清空所有对象
        /// </summary>
        public void Clear()
        {
            while (_objectQueue.Count > 0)
            {
                GameObject obj = _objectQueue.Dequeue();
                if (obj)
                {
                    Main.Kill(obj);
                }
            }
        }
    }
}