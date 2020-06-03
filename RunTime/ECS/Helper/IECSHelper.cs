using System;
using System.Collections.Generic;

namespace HT.Framework
{
    /// <summary>
    /// ECS管理器的助手接口
    /// </summary>
    public interface IECSHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有系统【系统类型，系统对象】
        /// </summary>
        Dictionary<Type, ECS_System> Systems { get; set; }
        /// <summary>
        /// 所有实体【实体ID，实体对象】
        /// </summary>
        Dictionary<string, ECS_Entity> Entities { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        void OnInitialization();
        /// <summary>
        /// 准备工作
        /// </summary>
        void OnPreparatory();
        /// <summary>
        /// 刷新
        /// </summary>
        void OnRefresh();
        /// <summary>
        /// 终结
        /// </summary>
        void OnTermination();

        /// <summary>
        /// 设置ECS环境为脏的，触发ECS环境重新刷新
        /// </summary>
        void SetDirty();
        /// <summary>
        /// 设置系统激活
        /// </summary>
        /// <param name="type">系统类型</param>
        /// <param name="isEnable">是否激活</param>
        void SetSystemEnable(Type type, bool isEnable);
        /// <summary>
        /// 获取系统
        /// </summary>
        /// <param name="type">系统类型</param>
        /// <returns>系统对象</returns>
        ECS_System GetSystem(Type type);
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        void AddEntity(ECS_Entity entity);
        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity">实体</param>
        void RemoveEntity(ECS_Entity entity);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>实体对象</returns>
        ECS_Entity GetEntity(string id);
    }
}