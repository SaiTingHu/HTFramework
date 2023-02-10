using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 事件工具箱
    /// </summary>
    public static class EventToolkit
    {
        /// <summary>
        /// UGUI 控件添加公共事件监听
        /// </summary>
        /// <param name="target">事件监听目标</param>
        /// <param name="type">事件类型</param>
        /// <param name="callback">回调函数</param>
        public static void AddCommonEventListener(this RectTransform target, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            if (target == null)
                return;

            EventTrigger trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.gameObject.AddComponent<EventTrigger>();
            }
            if (trigger.triggers == null)
            {
                trigger.triggers = new List<EventTrigger.Entry>();
            }

            //定义一个事件入口
            EventTrigger.Entry entry = new EventTrigger.Entry();
            //设置事件类型
            entry.eventID = type;
            //设置事件回调函数
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(callback);
            //添加事件到事件组
            trigger.triggers.Add(entry);
        }
        /// <summary>
        /// UGUI 控件移除所有公共事件监听
        /// </summary>
        /// <param name="target">事件监听目标</param>
        public static void RemoveAllCommonEventListener(this RectTransform target)
        {
            if (target == null)
                return;

            EventTrigger trigger = target.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                if (trigger.triggers != null)
                {
                    trigger.triggers.Clear();
                }
            }
        }
        /// <summary>
        /// UGUI Button添加点击事件监听
        /// </summary>
        /// <param name="target">事件监听目标</param>
        /// <param name="callback">回调函数</param>
        public static void AddEventListener(this RectTransform target, UnityAction callback)
        {
            if (target == null)
                return;

            Button button = target.GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(callback);
            }
            else
            {
                Log.Info($"{target.name} 丢失了组件 Button！");
            }
        }
        /// <summary>
        /// UGUI Button移除所有点击事件监听
        /// </summary>
        /// <param name="target">事件监听目标</param>
        public static void RemoveAllEventListener(this RectTransform target)
        {
            if (target == null)
                return;

            Button button = target.GetComponent<Button>();
            if (button)
            {
                button.onClick.RemoveAllListeners();
            }
            else
            {
                Log.Info($"{target.name} 丢失了组件 Button！");
            }
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标添加鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="callback">点击事件回调</param>
        public static void AddClickListener(this GameObject target, UnityAction callback)
        {
            Main.m_Controller.AddClickListener(target, callback);
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标移除鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="callback">点击事件回调</param>
        public static void RemoveClickListener(this GameObject target, UnityAction callback)
        {
            Main.m_Controller.RemoveClickListener(target, callback);
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标移除所有的鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        public static void RemoveAllClickListener(this GameObject target)
        {
            Main.m_Controller.RemoveAllClickListener(target);
        }
    }
}