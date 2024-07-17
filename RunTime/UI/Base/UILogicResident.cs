using System;
using System.Collections.Generic;
using System.Reflection;

namespace HT.Framework
{
    /// <summary>
    /// 常驻UI
    /// </summary>
    public abstract class UILogicResident : UILogicBase
    {
        private Dictionary<Type, UIRegion> _regions = new Dictionary<Type, UIRegion>();

        /// <summary>
        /// 初始化
        /// </summary>
        public override void OnInit()
        {
            //base.OnInit();

            //重写父类的自动化任务
            int injectCount = 0;
            int bindCount = 0;

            if (IsAutomate)
            {
                FieldInfo[] fieldInfos = AutomaticTask.GetAutomaticFields(GetType());
                injectCount = AutomaticTask.ApplyInject(this, fieldInfos);

                if (IsSupportedDataDriver)
                {
                    bindCount = AutomaticTask.ApplyDataBinding(this, fieldInfos);
                }
            }

            Type[] types = DefineRegions();
            if (types != null)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    if (type != null && type.IsSubclassOf(typeof(UIRegion)))
                    {
                        if (!_regions.ContainsKey(type))
                        {
                            UIRegion region = Activator.CreateInstance(type) as UIRegion;
                            region.Host = this;
                            _regions.Add(type, region);

                            if (IsAutomate)
                            {
                                FieldInfo[] fieldInfos = AutomaticTask.GetAutomaticFields(type);
                                injectCount += AutomaticTask.ApplyInject(region, fieldInfos);

                                if (IsSupportedDataDriver)
                                {
                                    bindCount += AutomaticTask.ApplyDataBinding(region, fieldInfos);
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Error($"UI逻辑类 {GetType().FullName} 定义区域失败：区域类型 {type.FullName} 必须继承至基类 UILogicResident.UIRegion！");
                    }
                }
            }

            if (_regions.Count > 0)
            {
                foreach (var region in _regions)
                {
                    region.Value.OnInit();
                }
            }

#if UNITY_EDITOR
            SafetyChecker.DoSafetyCheck(this, injectCount, bindCount);
#endif
        }
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="args">可选参数</param>
        public override void OnOpen(params object[] args)
        {
            base.OnOpen(args);

            if (_regions.Count > 0)
            {
                foreach (var region in _regions)
                {
                    region.Value.OnOpen(args);
                }
            }
        }
        /// <summary>
        /// 置顶UI
        /// </summary>
        public virtual void OnPlaceTop()
        {
            if (_regions.Count > 0)
            {
                foreach (var region in _regions)
                {
                    region.Value.OnPlaceTop();
                }
            }
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        public override void OnClose()
        {
            base.OnClose();

            if (_regions.Count > 0)
            {
                foreach (var region in _regions)
                {
                    region.Value.OnClose();
                }
            }
        }
        /// <summary>
        /// 销毁UI
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();

            if (_regions.Count > 0)
            {
                foreach (var region in _regions)
                {
                    region.Value.OnDestroy();
                }
                _regions.Clear();
            }
        }
        /// <summary>
        /// UI逻辑更新
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_regions.Count > 0)
            {
                foreach (var region in _regions)
                {
                    region.Value.OnUpdate();
                }
            }
        }

        /// <summary>
        /// 定义所有区域
        /// </summary>
        /// <returns>所有区域类型</returns>
        protected virtual Type[] DefineRegions()
        {
            return null;
        }
        /// <summary>
        /// 获取一个区域
        /// </summary>
        /// <typeparam name="T">区域类型</typeparam>
        /// <returns>区域</returns>
        public T GetRegion<T>() where T : UIRegion
        {
            return GetRegion(typeof(T)) as T;
        }
        /// <summary>
        /// 获取一个区域
        /// </summary>
        /// <param name="type">区域类型</param>
        /// <returns>区域</returns>
        public UIRegion GetRegion(Type type)
        {
            if (_regions.ContainsKey(type))
            {
                return _regions[type];
            }
            return null;
        }

        /// <summary>
        /// UI区域基类
        /// </summary>
        public abstract class UIRegion
        {
            /// <summary>
            /// 区域的宿主
            /// </summary>
            public UILogicResident Host;

            /// <summary>
            /// 初始化
            /// </summary>
            public virtual void OnInit()
            {
                
            }
            /// <summary>
            /// 宿主UI打开时
            /// </summary>
            public virtual void OnOpen(params object[] args)
            {
                
            }
            /// <summary>
            /// 宿主UI置顶时
            /// </summary>
            public virtual void OnPlaceTop()
            {
                
            }
            /// <summary>
            /// 宿主UI关闭时
            /// </summary>
            public virtual void OnClose()
            {
                
            }
            /// <summary>
            /// 宿主UI销毁时
            /// </summary>
            public virtual void OnDestroy()
            {
                
            }
            /// <summary>
            /// 宿主UI逻辑帧更新
            /// </summary>
            public virtual void OnUpdate()
            {
                
            }
        }
    }
}