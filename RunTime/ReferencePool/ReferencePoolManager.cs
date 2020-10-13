using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 引用池管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.ReferencePool)]
    public sealed class ReferencePoolManager : InternalModuleBase
    {
        /// <summary>
        /// 单个引用池上限【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal int Limit = 100;
        
        private IReferencePoolHelper _helper;

        private ReferencePoolManager()
        {

        }
        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IReferencePoolHelper;
        }

        /// <summary>
        /// 获取引用池中引用数量
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>引用数量</returns>
        public int GetPoolCount<T>() where T : class, IReference, new()
        {
            return _helper.GetPoolCount(typeof(T));
        }
        /// <summary>
        /// 生成引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>对象</returns>
        public T Spawn<T>() where T : class, IReference, new()
        {
            return _helper.Spawn<T>();
        }
        /// <summary>
        /// 生成引用
        /// </summary>
        /// <param name="type">引用类型</param>
        /// <returns>对象</returns>
        public IReference Spawn(Type type)
        {
            return _helper.Spawn(type);
        }
        /// <summary>
        /// 回收引用
        /// </summary>
        /// <param name="refe">对象</param>
        public void Despawn(IReference refe)
        {
            _helper.Despawn(refe);
        }
        /// <summary>
        /// 批量回收引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="refes">对象集合</param>
        public void Despawns<T>(List<T> refes) where T : class, IReference, new()
        {
            _helper.Despawns(refes);
        }
        /// <summary>
        /// 批量回收引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="refes">对象数组</param>
        public void Despawns<T>(T[] refes) where T : class, IReference, new()
        {
            _helper.Despawns(refes);
        }
        /// <summary>
        /// 清空指定的引用池
        /// </summary>
        /// <param name="type">引用类型</param>
        public void Clear(Type type)
        {
            _helper.Clear(type);
        }
        /// <summary>
        /// 清空所有引用池
        /// </summary>
        public void ClearAll()
        {
            _helper.ClearAll();
        }
    }
}