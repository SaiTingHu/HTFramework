using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 实体管理器
    /// </summary>
    [InternalModule(HTFrameworkModule.Entity)]
    public sealed class EntityManager : InternalModuleBase<IEntityHelper>
    {
        /// <summary>
        /// 当前定义的实体名称【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<string> DefineEntityNames = new List<string>();
        /// <summary>
        /// 当前定义的实体对象【请勿在代码中修改】
        /// </summary>
        [SerializeField] internal List<GameObject> DefineEntityTargets = new List<GameObject>();
        
        /// <summary>
        /// 是否隐藏所有实体
        /// </summary>
        public bool IsHideAll
        {
            set
            {
                _helper.IsHideAll = value;
            }
            get
            {
                return _helper.IsHideAll;
            }
        }

        public override void OnInit()
        {
            base.OnInit();

            _helper.SetDefine(DefineEntityNames, DefineEntityTargets);
        }
        
        /// <summary>
        /// 创建实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        /// <param name="entityName">实体指定名称（为 <None> 时默认使用实体逻辑类名称）</param>
        /// <param name="onLoading">创建实体过程进度回调</param>
        /// <param name="onLoadDone">创建实体完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine CreateEntity<T>(string entityName = "<None>", HTFAction<float> onLoading = null, HTFAction<EntityLogicBase> onLoadDone = null) where T : EntityLogicBase
        {
            return _helper.CreateEntity(typeof(T), entityName, onLoading, onLoadDone);
        }
        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <param name="entityName">实体指定名称（为 <None> 时默认使用实体逻辑类名称）</param>
        /// <param name="onLoading">创建实体过程进度回调</param>
        /// <param name="onLoadDone">创建实体完成回调</param>
        /// <returns>加载协程</returns>
        public Coroutine CreateEntity(Type type, string entityName = "<None>", HTFAction<float> onLoading = null, HTFAction<EntityLogicBase> onLoadDone = null)
        {
            return _helper.CreateEntity(type, entityName, onLoading, onLoadDone);
        }
        /// <summary>
        /// 销毁实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        public void DestroyEntity(EntityLogicBase entityLogic)
        {
            _helper.DestroyEntity(entityLogic);
        }
        /// <summary>
        /// 销毁指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void DestroyEntities<T>()
        {
            _helper.DestroyEntities(typeof(T));
        }
        /// <summary>
        /// 销毁指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        public void DestroyEntities(Type type)
        {
            _helper.DestroyEntities(type);
        }

        /// <summary>
        /// 根据名称获取实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        /// <param name="entityName">实体名称</param>
        /// <returns>实体</returns>
        public T GetEntity<T>(string entityName) where T : EntityLogicBase
        {
            return _helper.GetEntity(typeof(T), entityName) as T;
        }
        /// <summary>
        /// 根据名称获取实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <param name="entityName">实体名称</param>
        /// <returns>实体</returns>
        public EntityLogicBase GetEntity(Type type, string entityName)
        {
            return _helper.GetEntity(type, entityName);
        }
        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        /// <returns>实体组</returns>
        public List<T> GetEntities<T>() where T : EntityLogicBase
        {
            return _helper.GetEntities(typeof(T)).ConvertAllAS<T, EntityLogicBase>();
        }
        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        /// <returns>实体组</returns>
        public List<EntityLogicBase> GetEntities(Type type)
        {
            return _helper.GetEntities(type);
        }
        
        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        public void ShowEntity(EntityLogicBase entityLogic)
        {
            _helper.ShowEntity(entityLogic);
        }
        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityLogic">实体逻辑对象</param>
        public void HideEntity(EntityLogicBase entityLogic)
        {
            _helper.HideEntity(entityLogic);
        }
        /// <summary>
        /// 显示指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void ShowEntities<T>() where T : EntityLogicBase
        {
            _helper.ShowEntities(typeof(T));
        }
        /// <summary>
        /// 显示指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        public void ShowEntities(Type type)
        {
            _helper.ShowEntities(type);
        }
        /// <summary>
        /// 隐藏指定类型的所有实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类</typeparam>
        public void HideEntities<T>() where T : EntityLogicBase
        {
            _helper.HideEntities(typeof(T));
        }
        /// <summary>
        /// 隐藏指定类型的所有实体
        /// </summary>
        /// <param name="type">实体逻辑类</param>
        public void HideEntities(Type type)
        {
            _helper.HideEntities(type);
        }
    }
}