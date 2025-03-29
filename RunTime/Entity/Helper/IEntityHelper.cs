using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 实体管理器的助手接口
    /// </summary>
    public interface IEntityHelper : IInternalModuleHelper
    {
        /// <summary>
        /// 所有实体列表
        /// </summary>
        Dictionary<Type, List<EntityLogicBase>> Entities { get; }
        /// <summary>
        /// 所有实体对象池
        /// </summary>
        Dictionary<Type, Queue<GameObject>> ObjectPools { get; }
        /// <summary>
        /// 是否隐藏所有实体
        /// </summary>
        bool IsHideAll { get; set; }

        /// <summary>
        /// 设置预定义
        /// </summary>
        /// <param name="defineEntityNames">预定义的实体名称</param>
        /// <param name="defineEntityTargets">预定义的实体对象</param>
        void SetDefine(List<string> defineEntityNames, List<GameObject> defineEntityTargets);
        /// <summary>
        /// 添加预定义（如果已存在则覆盖，已创建的实体不受影响，销毁后再次创建生效）
        /// </summary>
        /// <param name="defineEntityName">预定义的实体名称</param>
        /// <param name="defineEntityTarget">预定义的实体对象</param>
        void AddDefine(string defineEntityName, GameObject defineEntityTarget);

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <param name="entityName">实体指定名称（为 <None> 时默认使用实体逻辑类名称）</param>
        /// <param name="onLoading">创建实体过程进度回调</param>
        /// <param name="onLoadDone">创建实体完成回调</param>
        /// <returns>加载协程</returns>
        Coroutine CreateEntity(Type type, string entityName, HTFAction<float> onLoading, HTFAction<EntityLogicBase> onLoadDone);
        /// <summary>
        /// 销毁实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        void DestroyEntity(EntityLogicBase entityLogic);
        /// <summary>
        /// 销毁指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        void DestroyEntities(Type type);

        /// <summary>
        /// 根据名称获取实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <param name="entityName">实体名称</param>
        /// <returns>实体</returns>
        EntityLogicBase GetEntity(Type type, string entityName);
        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <returns>实体组</returns>
        List<EntityLogicBase> GetEntities(Type type);
        /// <summary>
        /// 获取 transform 的最顶层父级所属的实体
        /// </summary>
        /// <param name="transform">目标 transform</param>
        /// <returns>实体</returns>
        EntityLogicBase GetEntityByTransform(Transform transform);

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        void ShowEntity(EntityLogicBase entityLogic);
        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        void HideEntity(EntityLogicBase entityLogic);
        /// <summary>
        /// 显示指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        void ShowEntities(Type type);
        /// <summary>
        /// 隐藏指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        void HideEntities(Type type);
    }
}