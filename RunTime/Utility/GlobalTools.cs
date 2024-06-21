using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 全局工具
    /// </summary>
    public static class GlobalTools
    {
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
        /// <param name="result">组件列表</param>
        /// <param name="includeInactive">是否包含未激活的子物体</param>
        public static void GetComponentsInSons<T>(this Transform tran, List<T> result, bool includeInactive = false) where T : Component
        {
            if (result == null) result = new List<T>();
            else result.Clear();

            for (int i = 0; i < tran.childCount; i++)
            {
                Transform child = tran.GetChild(i);
                T t = child.GetComponent<T>();
                if (t)
                {
                    if (child.gameObject.activeSelf)
                    {
                        result.Add(t);
                    }
                    else
                    {
                        if (includeInactive)
                        {
                            result.Add(t);
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
        /// <param name="result">组件列表</param>
        /// <param name="includeInactive">是否包含未激活的子物体</param>
        public static void GetComponentsInSons<T>(this GameObject obj, List<T> result, bool includeInactive = false) where T : Component
        {
            if (result == null) result = new List<T>();
            else result.Clear();

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                T t = child.GetComponent<T>();
                if (t)
                {
                    if (child.gameObject.activeSelf)
                    {
                        result.Add(t);
                    }
                    else
                    {
                        if (includeInactive)
                        {
                            result.Add(t);
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
        /// <param name="result">组件列表</param>
        /// <param name="includeInactive">是否包含未激活的子物体</param>
        public static void GetComponentsInSons<T>(this MonoBehaviour mono, List<T> result, bool includeInactive = false) where T : Component
        {
            if (result == null) result = new List<T>();
            else result.Clear();

            for (int i = 0; i < mono.transform.childCount; i++)
            {
                Transform child = mono.transform.GetChild(i);
                T t = child.GetComponent<T>();
                if (t)
                {
                    if (child.gameObject.activeSelf)
                    {
                        result.Add(t);
                    }
                    else
                    {
                        if (includeInactive)
                        {
                            result.Add(t);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 通过组件查找场景中所有的物体，包括隐藏和激活的
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="result">组件列表</param>
        public static void FindObjectsOfType<T>(List<T> result) where T : Component
        {
            if (result == null) result = new List<T>();
            else result.Clear();

            List<T> sub = new List<T>();
            GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject rootObj in rootObjs)
            {
                rootObj.transform.GetComponentsInChildren(true, sub);
                result.AddRange(sub);
            }
        }
        /// <summary>
        /// 获取当前打开的所有场景中的根物体
        /// </summary>
        /// <param name="result">物体列表</param>
        public static void GetRootGameObjectsInAllScene(List<GameObject> result)
        {
            if (result == null) result = new List<GameObject>();
            else result.Clear();

            List<GameObject> sub = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                SceneManager.GetSceneAt(i).GetRootGameObjects(sub);
                result.AddRange(sub);
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
                GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
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
                GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
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
                GameObject[] rootObjs = SceneManager.GetActiveScene().GetRootGameObjects();
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

            StringToolkit.BeginConcat();
            StringToolkit.Concat(tfs[tfs.Count - 1].name);
            for (int i = tfs.Count - 2; i >= 0; i--)
            {
                StringToolkit.Concat("/");
                StringToolkit.Concat(tfs[i].name);
            }
            return StringToolkit.EndConcat();
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
        public static bool Contains<T>(this T[] array, T value) where T : class
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
        /// <summary>
        /// 强制转换数组的类型（使用as强转）
        /// </summary>
        /// <typeparam name="TOutput">目标类型</typeparam>
        /// <typeparam name="TInput">原类型</typeparam>
        public static TOutput[] ConvertAllAS<TOutput, TInput>(this TInput[] array) where TOutput : class where TInput : class
        {
            if (array == null)
            {
                return null;
            }

            TOutput[] convertArray = new TOutput[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                convertArray[i] = array[i] as TOutput;
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

        #region 系统工具
        /// <summary>
        /// 获取本机物理地址
        /// </summary>
        /// <returns>物理地址</returns>
        public static string GetMacAddress()
        {
            try
            {
                NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
                for (int i = 0; i < nis.Length; i++)
                {
                    string address = nis[i].GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(address))
                    {
                        return address;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 转换为颜色RGB参数的十六进制字符串
        /// </summary>
        /// <param name="color">颜色值</param>
        /// <returns>十六进制字符串</returns>
        public static string ToHexSystemString(this Color color)
        {
            return string.Format("#{0}{1}{2}{3}"
                , ((int)(color.r * 255)).ToString("x2")
                , ((int)(color.g * 255)).ToString("x2")
                , ((int)(color.b * 255)).ToString("x2")
                , ((int)(color.a * 255)).ToString("x2"));
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
        /// <summary>
        /// 加载外部图片，并转换为Sprite
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <returns>转换后的Sprite</returns>
        public static Sprite LoadSprite(string path)
        {
            if (!File.Exists(path))
                return null;

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);

            Texture2D texture = new Texture2D(80, 80);
            texture.LoadImage(buffer);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

            stream.Close();

            return sprite;
        }
        /// <summary>
        /// 设置Transform的位置
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="location">位置</param>
        public static void SetLocation(this Transform target, Location location)
        {
            if (target == null || location == null)
                return;

            target.localPosition = location.Position;
            target.localEulerAngles = location.Rotation;
            target.localScale = location.Scale;
        }
        /// <summary>
        /// 获取Transform的位置
        /// </summary>
        /// <param name="target">目标</param>
        /// <returns>位置</returns>
        public static Location GetLocation(this Transform target)
        {
            if (target == null)
                return null;

            Location location = new Location();
            location.Position = target.localPosition;
            location.Rotation = target.localEulerAngles;
            location.Scale = target.localScale;
            return location;
        }
        #endregion
    }
}