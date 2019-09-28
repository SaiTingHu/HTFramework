using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
        /// <param name="action">执行的代码</param>
        /// <param name="delaySeconds">延时的秒数</param>
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
        /// 等待执行
        /// </summary>
        /// <param name="action">执行的代码</param>
        /// <param name="waitUntil">等待的WaitUntil</param>
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

        #region 高光工具
        private static List<HighlightableObject> HOCache = new List<HighlightableObject>();
        private static HashSet<HighlightableObject> HighLightObjects = new HashSet<HighlightableObject>();
        private static HashSet<HighlightableObject> FlashHighLightObjects = new HashSet<HighlightableObject>();

        /// <summary>
        /// 开启高光，使用默认发光颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void OpenHighLight(this GameObject target)
        {
            target.OpenHighLight(Color.cyan);
        }
        /// <summary>
        /// 开启高光，使用指定发光颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color">发光颜色</param>
        public static void OpenHighLight(this GameObject target, Color color)
        {
            HOCache.Clear();
            target.transform.GetComponentsInChildren(true, HOCache);
            for (int i = 0; i < HOCache.Count; i++)
            {
                if (HOCache[i].gameObject != target)
                {
                    HOCache[i].ConstantOff();
                    HOCache[i].Die();
                }
            }

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            if (!HighLightObjects.Contains(ho))
            {
                HighLightObjects.Add(ho);
            }
            ho.ConstantOnImmediate(color);
        }
        /// <summary>
        /// 关闭高光
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseHighLight(this GameObject target, bool die = false)
        {
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) return;

            if (HighLightObjects.Contains(ho))
            {
                HighLightObjects.Remove(ho);
            }
            ho.ConstantOff();
            if (die) ho.Die();
        }
        /// <summary>
        /// 关闭所有的高光
        /// </summary>
        public static void CloseAllHighLight(bool die = false)
        {
            foreach (HighlightableObject ho in HighLightObjects)
            {
                if (ho)
                {
                    ho.ConstantOff();
                    if (die) ho.Die();
                }
            }
            HighLightObjects.Clear();
        }

        /// <summary>
        /// 开启闪光，使用默认颜色和频率
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void OpenFlashHighLight(this GameObject target)
        {
            target.OpenFlashHighLight(Color.red, Color.white, 2);
        }
        /// <summary>
        /// 开启闪光，使用默认频率
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color1">颜色1</param>
        /// <param name="color2">颜色2</param>
        public static void OpenFlashHighLight(this GameObject target, Color color1, Color color2)
        {
            target.OpenFlashHighLight(color1, color2, 2);
        }
        /// <summary>
        /// 开启闪光，使用指定频率
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color1">颜色1</param>
        /// <param name="color2">颜色2</param>
        /// <param name="freq">频率</param>
        public static void OpenFlashHighLight(this GameObject target, Color color1, Color color2, float freq)
        {
            HOCache.Clear();
            target.transform.GetComponentsInChildren(true, HOCache);
            for (int i = 0; i < HOCache.Count; i++)
            {
                if (HOCache[i].gameObject != target)
                {
                    HOCache[i].FlashingOff();
                    HOCache[i].Die();
                }
            }

            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            if (!FlashHighLightObjects.Contains(ho))
            {
                FlashHighLightObjects.Add(ho);
            }
            ho.FlashingOn(color1, color2, freq);
        }
        /// <summary>
        /// 关闭闪光
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseFlashHighLight(this GameObject target, bool die = false)
        {
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) return;

            if (FlashHighLightObjects.Contains(ho))
            {
                FlashHighLightObjects.Remove(ho);
            }
            ho.FlashingOff();
            if (die) ho.Die();
        }
        /// <summary>
        /// 关闭所有的闪光
        /// </summary>
        public static void CloseAllFlashHighLight(bool die = false)
        {
            foreach (HighlightableObject ho in FlashHighLightObjects)
            {
                if (ho)
                {
                    ho.FlashingOff();
                    if (die) ho.Die();
                }
            }
            FlashHighLightObjects.Clear();
        }
        #endregion

        #region 事件工具
        /// <summary>
        /// UGUI 控件添加事件监听
        /// </summary>
        /// <param name="target">事件监听目标</param>
        /// <param name="type">事件类型</param>
        /// <param name="callback">回调函数</param>
        public static void AddEventListener(this RectTransform target, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            EventTrigger trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
            }

            //定义一个事件
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
                LogInfo(target.name + " 丢失了组件 Button！");
            }
        }
        /// <summary>
        /// UGUI Button移除所有事件监听
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
                LogInfo(target.name + " 丢失了组件 Button！");
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
        public static string JsonToString(JsonData json)
        {
            if (json == null)
            {
                LogError("Json数据为空！");
                return "";
            }
            return json.ToJson();
        }
        /// <summary>
        /// 字符串转换为Json数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
            GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject rootObj in rootObjs)
            {
                rootObj.transform.GetComponentsInChildren(true, sub);
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

        #region 网络工具
        /// <summary>
        /// 加载外部图片
        /// </summary>
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

        #region 打开或保存文件
        /// <summary>
        /// 文件窗口参数
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public sealed class OpenAndSaveFileAttribute
        {
            public int structSize = 0;
            public IntPtr dlgOwner = IntPtr.Zero;
            public IntPtr instance = IntPtr.Zero;
            public string filter = null;
            public string customFilter = null;
            public int maxCustFilter = 0;
            public int filterIndex = 0;
            public string file = null;
            public int maxFile = 0;
            public string fileTitle = null;
            public int maxFileTitle = 0;
            public string initialDir = null;
            public string title = null;
            public int flags = 0;
            public short fileOffset = 0;
            public short fileExtension = 0;
            public string defExt = null;
            public IntPtr custData = IntPtr.Zero;
            public IntPtr hook = IntPtr.Zero;
            public string templateName = null;
            public IntPtr reservedPtr = IntPtr.Zero;
            public int reservedInt = 0;
            public int flagsEx = 0;
        }

        /// <summary>
        /// 打开或保存文件
        /// </summary>
        public sealed class OpenAndSaveFile
        {
            /// <summary>
            /// 打开一个文件打开界面
            /// </summary>
            /// <param name="ofn"></param>
            /// <returns></returns>
            [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern bool GetOpenFileName([In, Out] OpenAndSaveFileAttribute ofn);

            /// <summary>
            /// 打开一个文件保存界面
            /// </summary>
            /// <param name="ofn"></param>
            /// <returns></returns>
            [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern bool GetSaveFileName([In, Out] OpenAndSaveFileAttribute ofn);
        }
        #endregion

        #region 时间工具
        /// <summary>
        /// 转换为标准时间字符串
        /// </summary>
        public static string ToDefaultDateString(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }
        #endregion

        #region 字符串工具
        private static StringBuilder StringInstance = new StringBuilder();
        
        /// <summary>
        /// 字符串拼接
        /// </summary>
        /// <param name="str">待拼接的字符串</param>
        /// <returns>拼接成功的字符串</returns>
        public static string StringConcat(params string[] str)
        {
            StringInstance.Clear();
            for (int i = 0; i < str.Length; i++)
            {
                StringInstance.Append(str[i]);
            }
            return StringInstance.ToString();
        }
        /// <summary>
        /// 字符串拼接
        /// </summary>
        /// <param name="str">待拼接的字符串</param>
        /// <returns>拼接成功的字符串</returns>
        public static string StringConcat(List<string> str)
        {
            StringInstance.Clear();
            for (int i = 0; i < str.Count; i++)
            {
                StringInstance.Append(str[i]);
            }
            return StringInstance.ToString();
        }
        /// <summary>
        /// 转换成枚举
        /// </summary>
        public static EnumType ToEnum<EnumType>(this string value, EnumType defaultValue)
        {
            if (!string.IsNullOrEmpty(value))
            {
                try
                {
                    return (EnumType)Enum.Parse(typeof(EnumType), value);
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
        /// <summary>
        /// 转换成三维向量，格式：x,y,z
        /// </summary>
        public static Vector3 ToVector3(this string value)
        {
            value = value.Replace("f", "");
            string[] values = value.Split(',');
            if (values.Length == 3)
            {
                return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
            }

            return Vector3.zero;
        }
        /// <summary>
        /// 转换成四元素
        /// </summary>
        public static Quaternion ToQuaternion(this Vector3 value)
        {
            return Quaternion.Euler(value);
        }
        /// <summary>
        /// MD5算法加密
        /// </summary>
        public static string MD5Encrypt(string value)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //将字符串转换为字节数组
            byte[] fromData = Encoding.UTF8.GetBytes(value);
            //计算字节数组的哈希值
            byte[] toData = md5.ComputeHash(fromData);

            return Convert.ToBase64String(toData);
        }
        /// <summary>
        /// 256位AES加密
        /// </summary>
        public static string AESEncrypt(string value)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes("12AE5C7VV01JK45L7OP0R2WE5AS8XD12");
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        /// <summary>
        /// 判断数组中是否存在某元素
        /// </summary>
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
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichBoldColor(this string value, string subStr, Color color)
        {
            if (subStr.Length <= 0 || !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<b><color=" + color.ToHexSystemString() + ">");
            else return value;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</color></b>");
            else return value;

            return valueRich;
        }
        /// <summary>
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichColor(this string value, string subStr, Color color)
        {
            if (subStr.Length <= 0 || !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<color=" + color.ToHexSystemString() + ">");
            else return value;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</color>");
            else return value;

            return valueRich;
        }
        /// <summary>
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichSize(this string value, string subStr, int size)
        {
            if (subStr.Length <= 0 || !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<size=" + size + ">");
            else return value;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</size>");
            else return value;

            return valueRich;
        }
        /// <summary>
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichBold(this string value, string subStr)
        {
            if (subStr.Length <= 0 || !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<b>");
            else return value;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</b>");
            else return value;

            return valueRich;
        }
        /// <summary>
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichItalic(this string value, string subStr)
        {
            if (subStr.Length <= 0 || !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            int index = valueRich.IndexOf(subStr);
            if (index >= 0) valueRich = valueRich.Insert(index, "<i>");
            else return value;

            index = valueRich.IndexOf(subStr) + subStr.Length;
            if (index >= 0) valueRich = valueRich.Insert(index, "</i>");
            else return value;

            return valueRich;
        }
        #endregion

        #region 数组工具
        /// <summary>
        /// 打乱数组
        /// </summary>
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
        /// 打乱集合
        /// </summary>
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
        /// 生成一个长度为length的数组，数组中每个数据均为此值
        /// </summary>
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
        /// 强制转换List的类型（使用as强转）
        /// </summary>
        /// <typeparam name="TOutput">目标类型</typeparam>
        /// <typeparam name="TInput">原类型</typeparam>
        public static List<TOutput> ConvertAllAS<TOutput, TInput>(this List<TInput> array) where TOutput : class where TInput : class
        {
            if (array == null || array.Count == 0)
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

        #region 枚举工具
        /// <summary>
        /// 获取枚举的备注信息
        /// </summary>
        public static string GetRemark(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return value.ToString();
            }
            object[] attributes = fi.GetCustomAttributes(typeof(RemarkAttribute), false);
            if (attributes.Length > 0)
            {
                return ((RemarkAttribute)attributes[0]).Remark;
            }
            else
            {
                return value.ToString();
            }
        }
        /// <summary>
        /// 判断枚举是否标记有指定特性
        /// </summary>
        public static bool IsExistAttribute<T>(this Enum value) where T : Attribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return false;
            }
            object[] attributes = fi.GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取枚举标记的指定的第一个特性
        /// </summary>
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return null;
            }
            object[] attributes = fi.GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                return attributes[0] as T;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取枚举标记的指定的第一个特性
        /// </summary>
        public static object GetAttribute(this Enum value, Type type)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return null;
            }
            object[] attributes = fi.GetCustomAttributes(type, false);
            if (attributes.Length > 0)
            {
                return attributes[0];
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region UGUI工具
        private static Dictionary<Button, TweenerCore<Color, Color, ColorOptions>> TwinkleButtons = new Dictionary<Button, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Toggle, TweenerCore<Color, Color, ColorOptions>> TwinkleToggles = new Dictionary<Toggle, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Image, TweenerCore<Color, Color, ColorOptions>> TwinkleImages = new Dictionary<Image, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Text, TweenerCore<Color, Color, ColorOptions>> TwinkleTexts = new Dictionary<Text, TweenerCore<Color, Color, ColorOptions>>();
        private static Dictionary<Dropdown, TweenerCore<Color, Color, ColorOptions>> TwinkleDropdowns = new Dictionary<Dropdown, TweenerCore<Color, Color, ColorOptions>>();

        /// <summary>
        /// 开启按钮闪烁（只在Normal状态）
        /// </summary>
        public static void OpenTwinkle(this Button button, Color color, float time = 0.5f)
        {
            if (!TwinkleButtons.ContainsKey(button))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                    () =>
                    {
                        return button.colors.normalColor;
                    },
                    (c) =>
                    {
                        ColorBlock block = button.colors;
                        block.normalColor = c;
                        button.colors = block;
                    }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = button.colors.normalColor;
                TwinkleButtons.Add(button, tweener);
            }
        }
        /// <summary>
        /// 关闭按钮闪烁
        /// </summary>
        public static void CloseTwinkle(this Button button)
        {
            if (TwinkleButtons.ContainsKey(button))
            {
                ColorBlock block = button.colors;
                block.normalColor = TwinkleButtons[button].startValue;

                TwinkleButtons[button].Kill();
                TwinkleButtons.Remove(button);
                
                button.colors = block;
            }
        }
        /// <summary>
        /// 开启开关闪烁（只在Normal状态）
        /// </summary>
        public static void OpenTwinkle(this Toggle toggle, Color color, float time = 0.5f)
        {
            if (!TwinkleToggles.ContainsKey(toggle))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return toggle.colors.normalColor;
                },
                (c) =>
                {
                    ColorBlock block = toggle.colors;
                    block.normalColor = c;
                    toggle.colors = block;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = toggle.colors.normalColor;
                TwinkleToggles.Add(toggle, tweener);
            }
        }
        /// <summary>
        /// 关闭开关闪烁
        /// </summary>
        public static void CloseTwinkle(this Toggle toggle)
        {
            if (TwinkleToggles.ContainsKey(toggle))
            {
                ColorBlock block = toggle.colors;
                block.normalColor = TwinkleToggles[toggle].startValue;

                TwinkleToggles[toggle].Kill();
                TwinkleToggles.Remove(toggle);
                
                toggle.colors = block;
            }
        }
        /// <summary>
        /// 开启图片闪烁
        /// </summary>
        public static void OpenTwinkle(this Image image, Color color, float time = 0.5f)
        {
            if (!TwinkleImages.ContainsKey(image))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return image.color;
                },
                (c) =>
                {
                    image.color = c;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = image.color;
                TwinkleImages.Add(image, tweener);
            }
        }
        /// <summary>
        /// 关闭图片闪烁
        /// </summary>
        public static void CloseTwinkle(this Image image)
        {
            if (TwinkleImages.ContainsKey(image))
            {
                Color normalColor = TwinkleImages[image].startValue;

                TwinkleImages[image].Kill();
                TwinkleImages.Remove(image);

                image.color = normalColor;
            }
        }
        /// <summary>
        /// 开启文本框闪烁
        /// </summary>
        public static void OpenTwinkle(this Text text, Color color, float time = 0.5f)
        {
            if (!TwinkleTexts.ContainsKey(text))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return text.color;
                },
                (c) =>
                {
                    text.color = c;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = text.color;
                TwinkleTexts.Add(text, tweener);
            }
        }
        /// <summary>
        /// 关闭文本框闪烁
        /// </summary>
        public static void CloseTwinkle(this Text text)
        {
            if (TwinkleTexts.ContainsKey(text))
            {
                Color normalColor = TwinkleTexts[text].startValue;

                TwinkleTexts[text].Kill();
                TwinkleTexts.Remove(text);

                text.color = normalColor;
            }
        }
        /// <summary>
        /// 开启下拉框闪烁（只在Normal状态）
        /// </summary>
        public static void OpenTwinkle(this Dropdown dropdown, Color color, float time = 0.5f)
        {
            if (!TwinkleDropdowns.ContainsKey(dropdown))
            {
                TweenerCore<Color, Color, ColorOptions> tweener = DOTween.To(
                () =>
                {
                    return dropdown.colors.normalColor;
                },
                (c) =>
                {
                    ColorBlock block = dropdown.colors;
                    block.normalColor = c;
                    dropdown.colors = block;
                }, color, time).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

                tweener.startValue = dropdown.colors.normalColor;
                TwinkleDropdowns.Add(dropdown, tweener);
            }
        }
        /// <summary>
        /// 关闭下拉框闪烁
        /// </summary>
        public static void CloseTwinkle(this Dropdown dropdown)
        {
            if (TwinkleDropdowns.ContainsKey(dropdown))
            {
                ColorBlock block = dropdown.colors;
                block.normalColor = TwinkleDropdowns[dropdown].startValue;

                TwinkleDropdowns[dropdown].Kill();
                TwinkleDropdowns.Remove(dropdown);

                dropdown.colors = block;
            }
        }
        /// <summary>
        /// 关闭所有控件的闪烁
        /// </summary>
        public static void CloseAllTwinkle()
        {
            foreach (KeyValuePair<Button, TweenerCore<Color, Color, ColorOptions>> button in TwinkleButtons)
            {
                ColorBlock block = button.Key.colors;
                block.normalColor = button.Value.startValue;
                button.Value.Kill();
                button.Key.colors = block;
            }
            foreach (KeyValuePair<Toggle, TweenerCore<Color, Color, ColorOptions>> toggle in TwinkleToggles)
            {
                ColorBlock block = toggle.Key.colors;
                block.normalColor = toggle.Value.startValue;
                toggle.Value.Kill();
                toggle.Key.colors = block;
            }
            foreach (KeyValuePair<Image, TweenerCore<Color, Color, ColorOptions>> image in TwinkleImages)
            {
                Color normalColor = image.Value.startValue;
                image.Value.Kill();
                image.Key.color = normalColor;
            }
            foreach (KeyValuePair<Text, TweenerCore<Color, Color, ColorOptions>> text in TwinkleTexts)
            {
                Color normalColor = text.Value.startValue;
                text.Value.Kill();
                text.Key.color = normalColor;
            }
            foreach (KeyValuePair<Dropdown, TweenerCore<Color, Color, ColorOptions>> dropdown in TwinkleDropdowns)
            {
                ColorBlock block = dropdown.Key.colors;
                block.normalColor = dropdown.Value.startValue;
                dropdown.Value.Kill();
                dropdown.Key.colors = block;
            }
            TwinkleButtons.Clear();
            TwinkleToggles.Clear();
            TwinkleImages.Clear();
            TwinkleTexts.Clear();
            TwinkleDropdowns.Clear();
        }

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
                    screenPos.z = 0;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.OverlayUIRoot, screenPos, null, out anchoredPos);
                    break;
                case UIType.Camera:
                    screenPos = Main.m_UI.UICamera.WorldToScreenPoint(position);
                    screenPos.z = 0;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.CameraUIRoot, screenPos, Main.m_UI.UICamera, out anchoredPos);
                    break;
                case UIType.World:
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
                    position.z = 0;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.OverlayUIRoot, position, null, out anchoredPos);
                    break;
                case UIType.Camera:
                    position.z = 0;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(reference != null ? reference : Main.m_UI.CameraUIRoot, position, Main.m_UI.UICamera, out anchoredPos);
                    break;
                case UIType.World:
                    break;
            }
            return anchoredPos;
        }
        /// <summary>
        /// 获取输入框的int类型值，若不是该类型值，则返回-1
        /// </summary>
        public static int IntText(this InputField input)
        {
            int value = -1;
            int.TryParse(input.text, out value);
            return value;
        }
        /// <summary>
        /// 获取输入框的float类型值，若不是该类型值，则返回-1f
        /// </summary>
        public static float FloatText(this InputField input)
        {
            float value = -1f;
            float.TryParse(input.text, out value);
            return value;
        }
        /// <summary>
        /// 设置下拉框值
        /// </summary>
        public static void SetValue(this Dropdown dropdown, string value)
        {
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (dropdown.options[i].text == value)
                {
                    dropdown.value = i;
                }
            }
        }
        #endregion

        #region 日志工具
        /// <summary>
        /// 打印普通日志
        /// </summary>
        public static void LogInfo(string value)
        {
            Debug.Log("<b><color=cyan>[HTFramework.Info]</color></b> " + value);
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        public static void LogWarning(string value)
        {
            Debug.LogWarning("<b><color=yellow>[HTFramework.Warning]</color></b> " + value);
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        public static void LogError(string value)
        {
            Debug.LogError("<b><color=red>[HTFramework.Error]</color></b> " + value);
        }
        #endregion

        #region 反射工具
        /// <summary>
        /// 当前的运行时程序集
        /// </summary>
        private static readonly HashSet<string> RunTimeAssemblies = new HashSet<string>() { "Assembly-CSharp", "HTFramework.RunTime", "HTFramework.AI.RunTime", "HTFramework.Auxiliary.RunTime", "UnityEngine" };
        /// <summary>
        /// 从当前程序域的运行时程序集中获取所有类型
        /// </summary>
        public static List<Type> GetTypesInRunTimeAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if (RunTimeAssemblies.Contains(assemblys[i].GetName().Name))
                {
                    types.AddRange(assemblys[i].GetTypes());
                }
            }
            return types;
        }
        /// <summary>
        /// 从当前程序域的运行时程序集中获取指定类型
        /// </summary>
        public static Type GetTypeInRunTimeAssemblies(string typeName)
        {
            Type type = null;
            foreach (string assembly in RunTimeAssemblies)
            {
                type = Type.GetType(typeName + "," + assembly);
                if (type != null)
                {
                    return type;
                }
            }
            LogError("获取类型 " + typeName + " 失败！当前运行时程序集中不存在此类型！");
            return null;
        }
        #endregion

        #region 数学工具
        /// <summary>
        /// 限制value的值在最小值与最大值之间
        /// </summary>
        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            value.x = Mathf.Clamp(value.x, min.x, max.x);
            value.y = Mathf.Clamp(value.y, min.y, max.y);
            value.z = Mathf.Clamp(value.z, min.z, max.z);
            return value;
        }
        /// <summary>
        /// 限制value的值在最小值与最大值之间
        /// </summary>
        public static Vector3 Clamp(Vector3 value, float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            value.x = Mathf.Clamp(value.x, minX, maxX);
            value.y = Mathf.Clamp(value.y, minY, maxY);
            value.z = Mathf.Clamp(value.z, minZ, maxZ);
            return value;
        }
        /// <summary>
        /// 是否约等于另一个浮点数
        /// </summary>
        /// <param name="sourceValue">源浮点数</param>
        /// <param name="targetValue">目标浮点数</param>
        /// <returns>是否约等于</returns>
        public static bool Approximately(this float sourceValue, float targetValue)
        {
            return Mathf.Approximately(sourceValue, targetValue);
        }
        #endregion

        #region 系统工具
        /// <summary>
        /// 获取本机Mac地址
        /// </summary>
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
        /// 获取与Assets同级的目录，也就是发布之后与EXE同级
        /// </summary>
        public static string GetDirectorySameLevelOfAssets(string directory)
        {
            return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + directory;
        }
        /// <summary>
        /// 获取颜色RGB参数的十六进制字符串
        /// </summary>
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
        /// 使用 as 强转目标
        /// </summary>
        /// <typeparam name="T">强转的类型</typeparam>
        /// <returns>结果</returns>
        public static T Cast<T>(this object target) where T : class
        {
            return target as T;
        }
        #endregion
    }
}