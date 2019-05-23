using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 对象池
    /// </summary>
    public sealed class ObjectSpawnPool : UnityEngine.Object
    {
        private GameObject _spawnTem;
        private int _limit = 100;
        private Queue<GameObject> _objectQueue = new Queue<GameObject>();
        
        private Action<GameObject> _onSpawn;
        private Action<GameObject> _onDespawn;

        public ObjectSpawnPool(GameObject spawnTem, int limit, Action<GameObject> onSpawn, Action<GameObject> onDespawn)
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
        public GameObject Spawn()
        {
            GameObject obj;
            if (_objectQueue.Count > 0)
            {
                obj = _objectQueue.Dequeue();
            }
            else
            {
                obj = Instantiate(_spawnTem);
            }

            obj.SetActive(true);

            if (_onSpawn != null)
                _onSpawn(obj);

            return obj;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Despawn(GameObject obj)
        {
            if (_objectQueue.Count >= _limit)
            {
                Destroy(obj);
            }
            else
            {
                obj.SetActive(false);

                if (_onDespawn != null)
                    _onDespawn(obj);

                _objectQueue.Enqueue(obj);
            }
        }

        /// <summary>
        /// 销毁所有对象
        /// </summary>
        public void Clear(bool destruct)
        {
            while (_objectQueue.Count > 0)
            {
                GameObject obj = _objectQueue.Dequeue();
                if (obj)
                {
                    Destroy(obj);
                }
            }
            if (destruct)
            {
                Destroy(this);
            }
        }
    }
}
