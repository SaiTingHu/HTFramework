using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// ECS的实体
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ECS_Entity : HTBehaviour
    {
        /// <summary>
        /// 创建新的实体
        /// </summary>
        /// <param name="target">实体依附的目标</param>
        /// <param name="entityName">实体名称</param>
        /// <returns>创建成功的实体</returns>
        public static ECS_Entity CreateEntity(GameObject target, string entityName = "Default")
        {
            if (target == null)
            {
                return null;
            }

            ECS_Entity entity = target.GetComponent<ECS_Entity>();
            if (entity == null) entity = target.AddComponent<ECS_Entity>();
            entity.Name = entityName;
            if (string.IsNullOrEmpty(entity.ID)) entity.GenerateID();
            return entity;
        }

        [SerializeField] private string _name = "";
        [SerializeField] private string _id = "";
        private Dictionary<Type, ECS_Component> _components = new Dictionary<Type, ECS_Component>();
        private Dictionary<int, ECS_Order> _orders = new Dictionary<int, ECS_Order>();

        /// <summary>
        /// 实体名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        /// <summary>
        /// 实体ID
        /// </summary>
        public string ID
        {
            get
            {
                return _id;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            Main.m_ECS.AddEntity(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Main.m_ECS.RemoveEntity(this);
        }

        /// <summary>
        /// 附加组件
        /// </summary>
        /// <param name="component">组件</param>
        internal void AppendComponent(ECS_Component component)
        {
            Type type = component.GetType();
            if (!_components.ContainsKey(type))
            {
                _components.Add(type, component);
                Main.m_ECS.SetDirty();
            }
        }
        /// <summary>
        /// 移除组件
        /// </summary>
        /// <param name="component">组件</param>
        internal void RemoveComponent(ECS_Component component)
        {
            Type type = component.GetType();
            if (_components.ContainsKey(type))
            {
                _components.Remove(type);
                Main.m_ECS.SetDirty();
            }
        }
        /// <summary>
        /// 是否存在指定组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>是否存在</returns>
        internal bool IsExistComponent<T>() where T : ECS_Component
        {
            return IsExistComponent(typeof(T));
        }
        /// <summary>
        /// 是否存在指定组件
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <returns>是否存在</returns>
        internal bool IsExistComponent(Type type)
        {
            return _components.ContainsKey(type);
        }
        /// <summary>
        /// 是否存在指定的多个组件
        /// </summary>
        /// <param name="types">组件类型数组</param>
        /// <returns>是否存在</returns>
        internal bool IsExistComponents(Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < types.Length; i++)
            {
                if (!_components.ContainsKey(types[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 生成ID
        /// </summary>
        internal void GenerateID()
        {
            _id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件</returns>
        public T Component<T>() where T : ECS_Component
        {
            return Component(typeof(T)) as T;
        }
        /// <summary>
        /// 获取组件
        /// </summary>
        /// <param name="type">组件类型</param>
        /// <returns>组件</returns>
        public ECS_Component Component(Type type)
        {
            if (_components.ContainsKey(type))
            {
                return _components[type];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 发出指令
        /// </summary>
        /// <param name="id">指令ID</param>
        /// <param name="order">指令信息</param>
        public void GiveOrder(int id, ECS_Order order = null)
        {
            if (!_orders.ContainsKey(id))
            {
                _orders.Add(id, order);
            }
        }
        /// <summary>
        /// 撤销指令
        /// </summary>
        /// <param name="id">指令ID</param>
        public void RecedeOrder(int id)
        {
            if (_orders.ContainsKey(id))
            {
                _orders.Remove(id);
            }
        }
        /// <summary>
        /// 是否存在指定ID的指令
        /// </summary>
        /// <param name="id">指令ID</param>
        /// <returns>是否存在该指令</returns>
        public bool IsExistOrder(int id)
        {
            return _orders.ContainsKey(id);
        }
        /// <summary>
        /// 获取指令
        /// </summary>
        /// <param name="id">指令ID</param>
        /// <returns>指令信息</returns>
        public ECS_Order GetOrder(int id)
        {
            if (_orders.ContainsKey(id))
            {
                return _orders[id];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 清空所有指令
        /// </summary>
        public void ClearOrder()
        {
            _orders.Clear();
        }
    }
}