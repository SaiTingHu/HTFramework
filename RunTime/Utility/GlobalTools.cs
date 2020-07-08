using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 全局工具
    /// </summary>
    public static class GlobalTools
    {
        #region 延时工具
        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="behaviour">执行者</param>
        /// <param name="action">执行的代码</param>
        /// <param name="delaySeconds">延时的秒数</param>
        /// <returns>延时的协程</returns>
        public static Coroutine DelayExecute(this MonoBehaviour behaviour, HTFAction action, float delaySeconds)
        {
            Coroutine coroutine = behaviour.StartCoroutine(DelayExecute(action, delaySeconds));
            return coroutine;
        }
        private static IEnumerator DelayExecute(HTFAction action, float delaySeconds)
        {
            yield return YieldInstructioner.GetWaitForSeconds(delaySeconds);
            action();
        }

        /// <summary>
        /// 下一帧执行
        /// </summary>
        /// <param name="behaviour">执行者</param>
        /// <param name="action">执行的代码</param>
        /// <returns>延时的协程</returns>
        public static Coroutine NextFrameExecute(this MonoBehaviour behaviour, HTFAction action)
        {
            Coroutine coroutine = behaviour.StartCoroutine(NextFrameExecute(action));
            return coroutine;
        }
        private static IEnumerator NextFrameExecute(HTFAction action)
        {
            yield return null;
            action();
        }

        /// <summary>
        /// 等待执行
        /// </summary>
        /// <param name="behaviour">执行者</param>
        /// <param name="action">执行的代码</param>
        /// <param name="waitUntil">等待的WaitUntil</param>
        /// <returns>等待的协程</returns>
        public static Coroutine WaitExecute(this MonoBehaviour behaviour, HTFAction action, WaitUntil waitUntil)
        {
            Coroutine coroutine = behaviour.StartCoroutine(WaitExecute(action, waitUntil));
            return coroutine;
        }
        private static IEnumerator WaitExecute(HTFAction action, WaitUntil waitUntil)
        {
            yield return waitUntil;
            action();
        }
        #endregion
        
        #region 事件工具
        /// <summary>
        /// UGUI 控件添加公共事件监听
        /// </summary>
        /// <param name="target">事件监听目标</param>
        /// <param name="type">事件类型</param>
        /// <param name="callback">回调函数</param>
        public static void AddCommonEventListener(this RectTransform target, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
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
            Button button = target.GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(callback);
            }
            else
            {
                Log.Info(target.name + " 丢失了组件 Button！");
            }
        }
        /// <summary>
        /// UGUI Button移除所有点击事件监听
        /// </summary>
        /// <param name="target">事件监听目标</param>
        public static void RemoveAllEventListener(this RectTransform target)
        {
            Button button = target.GetComponent<Button>();
            if (button)
            {
                button.onClick.RemoveAllListeners();
            }
            else
            {
                Log.Info(target.name + " 丢失了组件 Button！");
            }
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标添加鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="callback">点击事件回调</param>
        public static void AddClickListener(this GameObject target, HTFAction callback)
        {
            Main.m_Controller.AddClickListener(target, callback);
        }
        /// <summary>
        /// 为挂载 MouseRayTargetBase 的目标移除鼠标左键点击事件
        /// </summary>
        /// <param name="target">目标</param>
        public static void RemoveClickListener(this GameObject target)
        {
            Main.m_Controller.RemoveClickListener(target);
        }
        #endregion

        #region Json工具
        /// <summary>
        /// Json数据转换为字符串
        /// </summary>
        /// <param name="json">Json数据</param>
        /// <returns>字符串</returns>
        public static string JsonToString(JsonData json)
        {
            if (json == null)
            {
                Log.Error("Json数据为空！");
                return "";
            }
            return json.ToJson();
        }
        /// <summary>
        /// 字符串转换为Json数据
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>Json数据</returns>
        public static JsonData StringToJson(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "")
            {
                return null;
            }
            else
            {
                return JsonMapper.ToObject(value);
            }
        }
        /// <summary>
        /// 在安全模式下获取Json值
        /// </summary>
        /// <param name="json">json数据</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">缺省值</param>
        /// <returns>获取到的键对应的值</returns>
        public static string GetValueInSafe(this JsonData json, string key, string defaultValue)
        {
            if (json.Keys.Contains(key))
            {
                return json[key].ToString();
            }
            else
            {
                return defaultValue;
            }
        }
        #endregion

        #region 动画工具
        /// <summary>
        /// Text文本动画，从null值开始
        /// </summary>
        /// <param name="target">Text对象</param>
        /// <param name="endValue">动画目标值</param>
        /// <param name="duration">动画时间</param>
        /// <returns>Tweener</returns>
        public static Tweener DoTextBeginsNull(this Text target, string endValue, float duration)
        {
            target.text = "";
            return target.DOText(endValue, duration);
        }
        /// <summary>
        /// 追加一个回调 callback 到动画序列的末尾，并附加延时 interval 秒
        /// </summary>
        /// <param name="s">动画序列</param>
        /// <param name="callback">回调</param>
        /// <param name="interval">延时</param>
        /// <returns>动画序列</returns>
        public static Sequence AppendAction(this Sequence s, TweenCallback callback, float interval)
        {
            return s.AppendCallback(callback).AppendInterval(interval);
        }
        /// <summary>
        /// 追加一个延时 interval 到动画序列的末尾，并在延时结束后执行回调 callback
        /// </summary>
        /// <param name="s">动画序列</param>
        /// <param name="interval">延时</param>
        /// <param name="callback">回调</param>
        /// <returns>动画序列</returns>
        public static Sequence AppendAction(this Sequence s, float interval, TweenCallback callback)
        {
            return s.AppendInterval(interval).AppendCallback(callback);
        }
        #endregion

        #region 查找工具
        /// <summary>
        /// 通过子物体名称获取子物体的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="tran">自身</param>
        /// <param name="name">子物体名称</param>
        /// <returns>组件对象</returns>
        public static T GetComponentByChild<T>(this Transform tran, string name) where T : Component
        {
            Transform gObject = tran.Find(name);
            if (gObject == null)
                return null;

            return gObject.GetComponent<T>();
        }
        /// <summary>
        /// 通过子物体名称获取子物体的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="obj">自身</param>
        /// <param name="name">子物体名称</param>
        /// <returns>组件对象</returns>
        public static T GetComponentByChild<T>(this GameObject obj, string name) where T : Component
        {
            Transform gObject = obj.transform.Find(name);
            if (gObject == null)
                return null;

            return gObject.GetComponent<T>();
        }
        /// <summary>
        /// 通过子物体名称获取子物体的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="mono">自身</param>
        /// <param name="name">子物体名称</param>
        /// <returns>组件对象</returns>
        public static T GetComponentByChild<T>(this MonoBehaviour mono, string name) where T : Component
        {
            Transform gObject = mono.transform.Find(name);
            if (gObject == null)
                return null;

            return gObject.GetComponent<T>();
        }
        /// <summary>
        /// 获取直系子物体上的所有组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="tran">自身</param>
        /// <param name="Result">组件列表</param>
        /// <param name="includeInactive">是否包含未激活的子物体</param>
        public static void GetComponentsInSons<T>(this Transform tran, List<T> Result, bool includeInactive = false) where T : Component
        {
            if (Result == null) Result = new List<T>();
            else Result.Clear();

            for (int i = 0; i < tran.childCount; i++)
            {
                Transform child = tran.GetChild(i);
                T t = child.GetComponent<T>();
                if (t)
                {
                    if (child.gameObject.activeSelf)
                    {
                        Result.Add(t);
                    }
                    else
                    {
                        if (includeInactive)
                        {
                            Result.Add(t);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取直系子物体上的所有组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="obj">自身</param>
        /// <param name="Result">组件列表</param>
        /// <param name="includeInactive">是否包含未激活的子物体</param>
        public static void GetComponentsInSons<T>(this GameObject obj, List<T> Result, bool includeInactive = false) where T : Component
        {
            if (Result == null) Result = new List<T>();
            else Result.Clear();

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                T t = child.GetComponent<T>();
                if (t)
                {
                    if (child.gameObject.activeSelf)
                    {
                        Result.Add(t);
                    }
                    else
                    {
                        if (includeInactive)
                        {
                            Result.Add(t);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取直系子物体上的所有组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="mono">自身</param>
        /// <param name="Result">组件列表</param>
        /// <param name="includeInactive">是否包含未激活的子物体</param>
        public static void GetComponentsInSons<T>(this MonoBehaviour mono, List<T> Result, bool includeInactive = false) where T : Component
        {
            if (Result == null) Result = new List<T>();
            else Result.Clear();

            for (int i = 0; i < mono.transform.childCount; i++)
            {
                Transform child = mono.transform.GetChild(i);
                T t = child.GetComponent<T>();
                if (t)
                {
                    if (child.gameObject.activeSelf)
                    {
                        Result.Add(t);
                    }
                    else
                    {
                        if (includeInactive)
                        {
                            Result.Add(t);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 通过组件查找场景中所有的物体，包括隐藏和激活的
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="Result">组件列表</param>
        public static void FindObjectsOfType<T>(List<T> Result) where T : Component
        {
            if (Result == null) Result = new List<T>();
            else Result.Clear();

            List<T> sub = new List<T>();
            GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject rootObj in rootObjs)
            {
                rootObj.transform.GetComponentsInChildren(true, sub);
                Result.AddRange(sub);
            }
        }
        /// <summary>
        /// 获取当前打开的所有场景中的根物体
        /// </summary>
        /// <param name="Result">物体列表</param>
        public static void GetRootGameObjectsInAllScene(List<GameObject> Result)
        {
            if (Result == null) Result = new List<GameObject>();
            else Result.Clear();

            List<GameObject> sub = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                SceneManager.GetSceneAt(i).GetRootGameObjects(sub);
                Result.AddRange(sub);
            }
        }
        /// <summary>
        /// 获取RectTransform组件
        /// </summary>
        public static RectTransform rectTransform(this Transform tran)
        {
            return tran.GetComponent<RectTransform>();
        }
        /// <summary>
        /// 获取RectTransform组件
        /// </summary>
        public static RectTransform rectTransform(this GameObject obj)
        {
            return obj.GetComponent<RectTransform>();
        }
        /// <summary>
        /// 获取RectTransform组件
        /// </summary>
        public static RectTransform rectTransform(this MonoBehaviour mono)
        {
            return mono.GetComponent<RectTransform>();
        }
        /// <summary>
        /// 查找兄弟
        /// </summary>
        /// <param name="tran">自身</param>
        /// <param name="name">名称</param>
        /// <returns>对象实例</returns>
        public static GameObject FindBrother(this Transform tran, string name)
        {
            GameObject gObject = null;
            if (tran.parent)
            {
                Transform tf = tran.parent.Find(name);
                gObject = tf ? tf.gameObject : null;
            }
            else
            {
                GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject rootObj in rootObjs)
                {
                    if (rootObj.name == name)
                    {
                        gObject = rootObj;
                        break;
                    }
                }
            }
            return gObject;
        }
        /// <summary>
        /// 查找兄弟
        /// </summary>
        /// <param name="obj">自身</param>
        /// <param name="name">名称</param>
        /// <returns>对象实例</returns>
        public static GameObject FindBrother(this GameObject obj, string name)
        {
            GameObject gObject = null;
            if (obj.transform.parent)
            {
                Transform tf = obj.transform.parent.Find(name);
                gObject = tf ? tf.gameObject : null;
            }
            else
            {
                GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject rootObj in rootObjs)
                {
                    if (rootObj.name == name)
                    {
                        gObject = rootObj;
                        break;
                    }
                }
            }
            return gObject;
        }
        /// <summary>
        /// 查找兄弟
        /// </summary>
        /// <param name="mono">自身</param>
        /// <param name="name">名称</param>
        /// <returns>对象实例</returns>
        public static GameObject FindBrother(this MonoBehaviour mono, string name)
        {
            GameObject gObject = null;
            if (mono.transform.parent)
            {
                Transform tf = mono.transform.parent.Find(name);
                gObject = tf ? tf.gameObject : null;
            }
            else
            {
                GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject rootObj in rootObjs)
                {
                    if (rootObj.name == name)
                    {
                        gObject = rootObj;
                        break;
                    }
                }
            }
            return gObject;
        }
        /// <summary>
        /// 查找孩子
        /// </summary>
        /// <param name="tran">自身</param>
        /// <param name="name">名称</param>
        /// <returns>对象实例</returns>
        public static GameObject FindChildren(this Transform tran, string name)
        {
            Transform gObject = tran.Find(name);
            if (gObject == null)
                return null;

            return gObject.gameObject;
        }
        /// <summary>
        /// 查找孩子
        /// </summary>
        /// <param name="obj">自身</param>
        /// <param name="name">名称</param>
        /// <returns>对象实例</returns>
        public static GameObject FindChildren(this GameObject obj, string name)
        {
            Transform gObject = obj.transform.Find(name);
            if (gObject == null)
                return null;

            return gObject.gameObject;
        }
        /// <summary>
        /// 查找孩子
        /// </summary>
        /// <param name="mono">自身</param>
        /// <param name="name">名称</param>
        /// <returns>对象实例</returns>
        public static GameObject FindChildren(this MonoBehaviour mono, string name)
        {
            Transform gObject = mono.transform.Find(name);
            if (gObject == null)
                return null;

            return gObject.gameObject;
        }
        /// <summary>
        /// 设置逆向激活
        /// </summary>
        /// <param name="obj">对象</param>
        public static void SetActiveInverse(this GameObject obj)
        {
            obj.SetActive(!obj.activeSelf);
        }
        /// <summary>
        /// 全路径
        /// </summary>
        /// <param name="tran">自身</param>
        /// <returns>在场景中的全路径</returns>
        public static string FullName(this Transform tran)
        {
            List<Transform> tfs = new List<Transform>();
            Transform tf = tran;
            tfs.Add(tf);
            while (tf.parent)
            {
                tf = tf.parent;
                tfs.Add(tf);
            }

            StringBuilder builder = new StringBuilder();
            builder.Append(tfs[tfs.Count - 1].name);
            for (int i = tfs.Count - 2; i >= 0; i--)
            {
                builder.Append("/" + tfs[i].name);
            }
            return builder.ToString();
        }
        #endregion
        
        #region 时间工具
        /// <summary>
        /// 转换为标准时间字符串（yyyy/MM/dd HH:mm:ss）
        /// </summary>
        /// <param name="time">时间对象</param>
        /// <returns>字符串</returns>
        public static string ToDefaultDateString(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }
        #endregion

        #region 数组工具
        /// <summary>
        /// 随机打乱数组
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        public static void Disrupt<T>(this T[] array)
        {
            int index = 0;
            T tmp;
            for (int i = 0; i < array.Length; i++)
            {
                index = UnityEngine.Random.Range(0, array.Length);
                if (index != i)
                {
                    tmp = array[i];
                    array[i] = array[index];
                    array[index] = tmp;
                }
            }
        }
        /// <summary>
        /// 随机打乱集合
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="array">集合</param>
        public static void Disrupt<T>(this List<T> array)
        {
            int index = 0;
            T tmp;
            for (int i = 0; i < array.Count; i++)
            {
                index = UnityEngine.Random.Range(0, array.Count);
                if (index != i)
                {
                    tmp = array[i];
                    array[i] = array[index];
                    array[index] = tmp;
                }
            }
        }
        /// <summary>
        /// 遍历数组
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组对象</param>
        /// <param name="action">执行操作</param>
        public static void Foreach<T>(this T[] array, HTFAction<T, int> action)
        {
            for (int i = 0; i < array.Length; i++)
            {
                action(array[i], i);
            }
        }
        /// <summary>
        /// 遍历集合
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="array">集合对象</param>
        /// <param name="action">执行操作</param>
        public static void Foreach<T>(this List<T> array, HTFAction<T, int> action)
        {
            for (int i = 0; i < array.Count; i++)
            {
                action(array[i], i);
            }
        }
        /// <summary>
        /// 生成一个长度为length的数组，数组中每个数据均为此值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="value">数组值</param>
        /// <param name="length">数组长度</param>
        /// <returns>数组</returns>
        public static T[] GenerateArray<T>(this T value, int length)
        {
            T[] array = new T[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
            return array;
        }
        /// <summary>
        /// 判断数组中是否存在某元素
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="value">元素</param>
        /// <returns>是否存在</returns>
        public static bool Contains(this string[] array, string value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断数组中是否存在某元素
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="value">元素</param>
        /// <returns>是否存在</returns>
        public static bool Contains(this int[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 强制转换List的类型（使用as强转）
        /// </summary>
        /// <typeparam name="TOutput">目标类型</typeparam>
        /// <typeparam name="TInput">原类型</typeparam>
        public static List<TOutput> ConvertAllAS<TOutput, TInput>(this List<TInput> array) where TOutput : class where TInput : class
        {
            if (array == null)
            {
                return null;
            }

            List<TOutput> convertArray = new List<TOutput>();
            for (int i = 0; i < array.Count; i++)
            {
                convertArray.Add(array[i] as TOutput);
            }
            return convertArray;
        }
        #endregion

        #region 特性工具
        /// <summary>
        /// 获取枚举的备注信息
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="inherit">是否包含继承</param>
        /// <returns>备注信息</returns>
        public static string GetRemark(this Enum value, bool inherit = false)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return value.ToString();
            }
            RemarkAttribute remark = fi.GetCustomAttribute<RemarkAttribute>(inherit);
            if (remark != null)
            {
                return remark.Remark;
            }
            else
            {
                return value.ToString();
            }
        }
        /// <summary>
        /// 判断枚举是否标记有指定特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <param name="inherit">是否包含继承</param>
        /// <returns>是否标记该特性</returns>
        public static bool IsExistAttribute<T>(this Enum value, bool inherit = false) where T : Attribute
        {
            return value.IsExistAttribute(typeof(T), inherit);
        }
        /// <summary>
        /// 判断枚举是否标记有指定特性
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="type">特性类型</param>
        /// <param name="inherit">是否包含继承</param>
        /// <returns>是否标记该特性</returns>
        public static bool IsExistAttribute(this Enum value, Type type, bool inherit = false)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return false;
            }
            Attribute attr = fi.GetCustomAttribute(type, inherit);
            if (attr != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取枚举的特性
        /// </summary>
        /// <typeparam name="T">特性类型</typeparam>
        /// <param name="value">枚举值</param>
        /// <param name="inherit">是否包含继承</param>
        /// <returns>特性</returns>
        public static T GetEnumAttribute<T>(this Enum value, bool inherit = false) where T : Attribute
        {
            return value.GetEnumAttribute(typeof(T), inherit) as T;
        }
        /// <summary>
        /// 获取枚举的特性
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="type">特性类型</param>
        /// <param name="inherit">是否包含继承</param>
        /// <returns>特性</returns>
        public static Attribute GetEnumAttribute(this Enum value, Type type, bool inherit = false)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return null;
            }
            Attribute attr = fi.GetCustomAttribute(type, inherit);
            return attr;
        }
        #endregion

        #region UGUI工具
        /// <summary>
        /// 限制Text内容的长度在length以内，超过的部分用replace代替
        /// </summary>
        public static void RestrictLength(this Text tex, int length, string replace)
        {
            if (tex.text.Length > length)
            {
                tex.text = tex.text.Substring(0, length) + replace;
            }
        }
        /// <summary>
        /// 限制Text中指定子字串的字体大小
        /// </summary>
        public static void ToRichSize(this Text tex, string subStr, int size)
        {
            if (subStr.Length <= 0 || !tex.text.Contains(subStr))
            {
                return;
            }

            string valueRich = tex.text;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<size=" + size + ">");
            else return;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</size>");
            else return;

            tex.text = valueRich;
        }
        /// <summary>
        /// 限制Text中指定子字串的字体颜色
        /// </summary>
        public static void ToRichColor(this Text tex, string subStr, Color color)
        {
            if (subStr.Length <= 0 || !tex.text.Contains(subStr))
            {
                return;
            }

            string valueRich = tex.text;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<color=" + color.ToHexSystemString() + ">");
            else return;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</color>");
            else return;

            tex.text = valueRich;
        }
        /// <summary>
        /// 限制Text中的指定子字串的字体加粗
        /// </summary>
        public static void ToRichBold(this Text tex, string subStr)
        {
            if (subStr.Length <= 0 || !tex.text.Contains(subStr))
            {
                return;
            }

            string valueRich = tex.text;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<b>");
            else return;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</b>");
            else return;

            tex.text = valueRich;
        }
        /// <summary>
        /// 限制Text中的指定子字串的字体斜体
        /// </summary>
        public static void ToRichItalic(this Text tex, string subStr)
        {
            if (subStr.Length <= 0 || !tex.text.Contains(subStr))
            {
                return;
            }

            string valueRich = tex.text;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<i>");
            else return;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</i>");
            else return;

            tex.text = valueRich;
        }
        /// <summary>
        /// 清除所有富文本样式
        /// </summary>
        public static void ClearRich(this Text tex)
        {
            string value = tex.text;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '<')
                {
                    for (int j = i + 1; j < value.Length; j++)
                    {
                        if (value[j] == '>')
                        {
                            int count = j - i + 1;
                            value = value.Remove(i, count);
                            i -= 1;
                            break;
                        }
                    }
                }
            }
            tex.text = value;
        }
        /// <summary>
        /// 当前鼠标是否停留在UGUI控件上
        /// </summary>
        /// <returns>是否</returns>
        public static bool IsPointerOverUGUI()
        {
            if (EventSystem.current)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 世界坐标转换为UGUI坐标（只针对框架UI模块下的UI控件）
        /// </summary>
        /// <param name="position">世界坐标</param>
        /// <param name="reference">参照物（要赋值的UGUI控件的根物体）</param>
        /// <param name="uIType">UI类型</param>
        /// <returns>基于参照物的局部UGUI坐标</returns>
        public static Vector2 WorldToUGUIPosition(this Vector3 position, RectTransform reference = null, UIType uIType = UIType.Overlay)
        {
            Vector3 screenPos;
            Vector2 anchoredPos = Vector2.zero;
            switch (uIType)
            {
                case UIType.Overlay:
                    screenPos = Main.m_Controller.MainCamera.WorldToScreenPoint(position);
                    if (screenPos.z < 0)
                    {
                        anchoredPos.Set(-100000, -100000);
                    }
                    else
                    {
                        screenPos.z = 0;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.OverlayUIRoot, screenPos, null, out anchoredPos);
                    }
                    break;
                case UIType.Camera:
                    screenPos = Main.m_UI.UICamera.WorldToScreenPoint(position);
                    if (screenPos.z < 0)
                    {
                        anchoredPos.Set(-100000, -100000);
                    }
                    else
                    {
                        screenPos.z = 0;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.CameraUIRoot, screenPos, Main.m_UI.UICamera, out anchoredPos);
                    }
                    break;
                case UIType.World:
                    anchoredPos = position;
                    break;
            }
            return anchoredPos;
        }
        /// <summary>
        /// 屏幕坐标转换为UGUI坐标（只针对框架UI模块下的UI控件）
        /// </summary>
        /// <param name="position">屏幕坐标</param>
        /// <param name="reference">参照物（要赋值的UGUI控件的根物体）</param>
        /// <param name="uIType">UI类型</param>
        /// <returns>基于参照物的局部UGUI坐标</returns>
        public static Vector2 ScreenToUGUIPosition(this Vector3 position, RectTransform reference = null, UIType uIType = UIType.Overlay)
        {
            Vector2 anchoredPos = Vector2.zero;
            switch (uIType)
            {
                case UIType.Overlay:
                    if (position.z < 0)
                    {
                        anchoredPos.Set(-100000, -100000);
                    }
                    else
                    {
                        position.z = 0;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.OverlayUIRoot, position, null, out anchoredPos);
                    }
                    break;
                case UIType.Camera:
                    if (position.z < 0)
                    {
                        anchoredPos.Set(-100000, -100000);
                    }
                    else
                    {
                        position.z = 0;
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.CameraUIRoot, position, Main.m_UI.UICamera, out anchoredPos);
                    }
                    break;
                case UIType.World:
                    anchoredPos = position;
                    break;
            }
            return anchoredPos;
        }
        /// <summary>
        /// 获取输入框的int类型值，若不是该类型值，则返回-1
        /// </summary>
        /// <param name="input">输入框</param>
        /// <returns>值</returns>
        public static int IntText(this InputField input)
        {
            int value = -1;
            int.TryParse(input.text, out value);
            return value;
        }
        /// <summary>
        /// 获取输入框的float类型值，若不是该类型值，则返回float.NaN
        /// </summary>
        /// <param name="input">输入框</param>
        /// <returns>值</returns>
        public static float FloatText(this InputField input)
        {
            float value = -1f;
            if (float.TryParse(input.text, out value))
            {
                return value;
            }
            else
            {
                return float.NaN;
            }
        }
        /// <summary>
        /// 设置下拉框值，若该下拉框不存在该值，则无效
        /// </summary>
        /// <param name="dropdown">下拉框</param>
        /// <param name="value">目标值</param>
        public static void SetValue(this Dropdown dropdown, string value)
        {
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text == value)
                {
                    dropdown.value = i;
                    return;
                }
            }
        }
        /// <summary>
        /// 加载外部图片，并转换为Sprite
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns>转换后的Sprite</returns>
        public static Sprite LoadSprite(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);

            Texture2D tex = new Texture2D(80, 80);
            tex.LoadImage(buffer);

            stream.Close();

            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        }
        #endregion

        #region 日志工具
        /// <summary>
        /// 打印普通日志
        /// </summary>
        /// <param name="value">日志</param>
        [Obsolete("Use 'Log.Info(string)' instead. It will be removed in the future.")]
        public static void LogInfo(string value)
        {
            value.Info();
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="value">日志</param>
        [Obsolete("Use 'Log.Warning(string)' instead. It will be removed in the future.")]
        public static void LogWarning(string value)
        {
            value.Warning();
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="value">日志</param>
        [Obsolete("Use 'Log.Error(string)' instead. It will be removed in the future.")]
        public static void LogError(string value)
        {
            value.Error();
        }
        #endregion

        #region 系统工具
        /// <summary>
        /// 获取本机Mac地址
        /// </summary>
        /// <returns>Mac地址</returns>
        public static string GetMacAddress()
        {
            try
            {
                NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
                for (int i = 0; i < nis.Length; i++)
                {
                    if (nis[i].Name == "本地连接")
                    {
                        return nis[i].GetPhysicalAddress().ToString();
                    }
                }
                return "null";
            }
            catch
            {
                return "null";
            }
        }
        /// <summary>
        /// 获取与Assets同级的目录
        /// </summary>
        /// <param name="directory">目录名（例如：/Library，获取项目的Library目录）</param>
        /// <returns>目录</returns>
        public static string GetDirectorySameLevelOfAssets(string directory)
        {
            return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + directory;
        }
        /// <summary>
        /// 转换为颜色RGB参数的十六进制字符串
        /// </summary>
        /// <param name="color">颜色值</param>
        /// <returns>十六进制字符串</returns>
        public static string ToHexSystemString(this Color color)
        {
            return "#" + ((int)(color.r * 255)).ToString("x2") +
                ((int)(color.g * 255)).ToString("x2") +
                ((int)(color.b * 255)).ToString("x2") +
                ((int)(color.a * 255)).ToString("x2");
        }
        #endregion

        #region 其他工具
        /// <summary>
        /// 设置自身及所有子物体的层
        /// </summary>
        /// <param name="gameObject">自身</param>
        /// <param name="layer">层</param>
        public static void SetLayerIncludeChildren(this GameObject gameObject, int layer)
        {
            foreach (Transform tran in gameObject.GetComponentsInChildren<Transform>(true))
            {
                tran.gameObject.layer = layer;
            }
        }
        /// <summary>
        /// 激活自身及所有子物体的碰撞器
        /// </summary>
        /// <param name="gameObject">自身</param>
        /// <param name="activateState">激活状态</param>
        public static void ActivateCollider(this GameObject gameObject, bool activateState)
        {
            foreach (Collider coll in gameObject.GetComponentsInChildren<Collider>(true))
            {
                coll.enabled = activateState;
            }
        }
        /// <summary>
        /// 使用 as 强转目标
        /// </summary>
        /// <typeparam name="T">强转的类型</typeparam>
        /// <param name="target">强转的对象</param>
        /// <returns>转换后的对象</returns>
        public static T Cast<T>(this object target) where T : class
        {
            return target as T;
        }
        #endregion
    }
}