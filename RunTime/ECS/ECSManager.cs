using System;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// ECS管理器
    /// </summary>
    [DisallowMultipleComponent]
    [InternalModule(HTFrameworkModule.ECS)]
    public sealed class ECSManager : InternalModuleBase
    {
        private IECSHelper _helper;

        /// <summary>
        /// 初始化模块
        /// </summary>
        internal override void OnInitialization()
        {
            base.OnInitialization();

            _helper = Helper as IECSHelper;
            _helper.OnInitialization();
        }
        /// <summary>
        /// 模块准备工作
        /// </summary>
        internal override void OnPreparatory()
        {
            base.OnPreparatory();

            _helper.OnPreparatory();
        }
        /// <summary>
        /// 刷新模块
        /// </summary>
        internal override void OnRefresh()
        {
            base.OnRefresh();

            _helper.OnRefresh();
        }
        /// <summary>
        /// 终结模块
        /// </summary>
        internal override void OnTermination()
        {
            base.OnTermination();

            _helper.OnTermination();
        }

        /// <summary>
        /// 设置ECS环境为脏的，触发ECS环境重新刷新
        /// </summary>
        public void SetDirty()
        {
            _helper.SetDirty();
        }
        /// <summary>
        /// 设置系统激活
        /// </summary>
        /// <typeparam name="T">系统类型</typeparam>
        /// <param name="isEnable">是否激活</param>
        public void SetSystemEnable<T>(bool isEnable) where T : ECS_System
        {
            _helper.SetSystemEnable(typeof(T), isEnable);
        }
        /// <summary>
        /// 设置系统激活
        /// </summary>
        /// <param name="type">系统类型</param>
        /// <param name="isEnable">是否激活</param>
        public void SetSystemEnable(Type type, bool isEnable)
        {
            _helper.SetSystemEnable(type, isEnable);
        }
        /// <summary>
        /// 获取系统
        /// </summary>
        /// <typeparam name="T">系统类型</typeparam>
        /// <returns>系统对象</returns>
        public T GetSystem<T>() where T : ECS_System
        {
            return _helper.GetSystem(typeof(T)) as T;
        }
        /// <summary>
        /// 获取系统
        /// </summary>
        /// <param name="type">系统类型</param>
        /// <returns>系统对象</returns>
        public ECS_System GetSystem(Type type)
        {
            return _helper.GetSystem(type);
        }
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        internal void AddEntity(ECS_Entity entity)
        {
            _helper.AddEntity(entity);
        }
        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity">实体</param>
        internal void RemoveEntity(ECS_Entity entity)
        {
            _helper.RemoveEntity(entity);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>实体对象</returns>
        public ECS_Entity GetEntity(string id)
        {
            return _helper.GetEntity(id);
        }
    }
}