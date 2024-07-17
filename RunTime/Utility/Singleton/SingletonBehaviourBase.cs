using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 单例模式 Behaviour 基类
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class SingletonBehaviourBase<T> : HTBehaviour, IUpdateFrame where T : HTBehaviour
    {
        private static T _current;
        /// <summary>
        /// 当前实例
        /// </summary>
        public static T Current
        {
            get
            {
                return _current;
            }
        }

        private Dictionary<Type, ModuleBase> _modules = new Dictionary<Type, ModuleBase>();

        protected override void Awake()
        {
            base.Awake();

            if (_current == null)
            {
                _current = GetComponent<T>();
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.Utility, $"单例类 {typeof(T).FullName} 发现两个或以上实例，这是不被允许的！");
            }
        }
        public virtual void OnUpdateFrame()
        {
            if (_modules.Count > 0)
            {
                foreach (var module in _modules)
                {
                    module.Value.OnUpdate();
                }
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            _current = null;

            if (_modules.Count > 0)
            {
                foreach (var module in _modules)
                {
                    module.Value.OnDestroy();
                }
                _modules.Clear();
            }
        }

        /// <summary>
        /// 添加一个模块
        /// </summary>
        /// <typeparam name="M">模块类型</typeparam>
        protected void AddModule<M>() where M : ModuleBase
        {
            AddModule(typeof(M));
        }
        /// <summary>
        /// 添加一个模块
        /// </summary>
        /// <param name="type">模块类型</param>
        protected void AddModule(Type type)
        {
            if (type != null && type.IsSubclassOf(typeof(ModuleBase)))
            {
                if (!_modules.ContainsKey(type))
                {
                    ModuleBase module = Activator.CreateInstance(type) as ModuleBase;
                    module.Host = Current;
                    module.OnInit();
                    _modules.Add(type, module);
                }
            }
            else
            {
                Log.Error($"单例类 {GetType().FullName} 添加模块失败：模块类型 {type.FullName} 必须继承至基类 SingletonBehaviourBase<T>.ModuleBase！");
            }
        }
        /// <summary>
        /// 移除一个模块
        /// </summary>
        /// <typeparam name="M">模块类型</typeparam>
        protected void RemoveModule<M>() where M : ModuleBase
        {
            RemoveModule(typeof(M));
        }
        /// <summary>
        /// 移除一个模块
        /// </summary>
        /// <param name="type">模块类型</param>
        protected void RemoveModule(Type type)
        {
            if (type != null && type.IsSubclassOf(typeof(ModuleBase)))
            {
                if (_modules.ContainsKey(type))
                {
                    _modules[type].OnDestroy();
                    _modules.Remove(type);
                }
            }
            else
            {
                Log.Error($"单例类 {GetType().FullName} 移除模块失败：模块类型 {type.FullName} 必须继承至基类 SingletonBehaviourBase<T>.ModuleBase！");
            }
        }
        /// <summary>
        /// 获取一个模块
        /// </summary>
        /// <typeparam name="M">模块类型</typeparam>
        /// <returns>模块</returns>
        public M GetModule<M>() where M : ModuleBase
        {
            return GetModule(typeof(M)) as M;
        }
        /// <summary>
        /// 获取一个模块
        /// </summary>
        /// <param name="type">模块类型</param>
        /// <returns>模块</returns>
        public ModuleBase GetModule(Type type)
        {
            if (_modules.ContainsKey(type))
            {
                return _modules[type];
            }
            return null;
        }

        /// <summary>
        /// 模块基类
        /// </summary>
        public abstract class ModuleBase
        {
            /// <summary>
            /// 模块的宿主
            /// </summary>
            public T Host;

            /// <summary>
            /// 初始化
            /// </summary>
            public virtual void OnInit()
            { 
            
            }
            /// <summary>
            /// 帧更新
            /// </summary>
            public virtual void OnUpdate()
            {

            }
            /// <summary>
            /// 销毁
            /// </summary>
            public virtual void OnDestroy()
            {

            }
        }
    }
}