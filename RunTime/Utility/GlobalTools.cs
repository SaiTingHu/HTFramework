using DG.Tweening;
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
        /// <param name="delaySeconds">延时等待的秒数</param>
        public static Coroutine DelayExecute(this MonoBehaviour behaviour, HTFAction action, float delaySeconds)
        {
            Coroutine coroutine = behaviour.StartCoroutine(DelayExecute(action, delaySeconds));
            return coroutine;
        }
        private static IEnumerator DelayExecute(HTFAction action, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            action();
        }
        #endregion

        #region 高光工具
        /// <summary>
        /// 开启高光，使用默认发光颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void OpenHighLight(this GameObject target)
        {
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            ho.ConstantOnImmediate(Color.cyan);
        }
        /// <summary>
        /// 开启高光，使用指定发光颜色
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color">发光颜色</param>
        public static void OpenHighLight(this GameObject target, Color color)
        {
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            ho.ConstantOnImmediate(color);
        }
        /// <summary>
        /// 关闭高光
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseHighLight(this GameObject target, bool die)
        {
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) return;

            ho.ConstantOff();
            if (die) ho.Die();
        }
        /// <summary>
        /// 开启闪光，使用默认颜色和频率
        /// </summary>
        /// <param name="target">目标物体</param>
        public static void OpenFlashHighLight(this GameObject target)
        {
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            ho.FlashingOn(Color.red, Color.white, 2);
        }
        /// <summary>
        /// 开启闪光，使用默认频率
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="color1">颜色1</param>
        /// <param name="color2">颜色2</param>
        public static void OpenFlashHighLight(this GameObject target, Color color1, Color color2)
        {
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            ho.FlashingOn(color1, color2, 2);
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
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) ho = target.AddComponent<HighlightableObject>();

            ho.FlashingOn(color1, color2, freq);
        }
        /// <summary>
        /// 关闭闪光
        /// </summary>
        /// <param name="target">目标物体</param>
        /// <param name="die">是否销毁高光实例</param>
        public static void CloseFlashHighLight(this GameObject target, bool die)
        {
            HighlightableObject ho = target.GetComponent<HighlightableObject>();
            if (ho == null) return;

            ho.FlashingOff();
            if (die) ho.Die();
        }
        #endregion

        #region 事件工具
        /// <summary>
        /// EventTrigger添加事件监听
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
        /// Button添加事件监听 (onClick)
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
        /// Toggle添加事件监听 (onValueChanged)
        /// </summary>
        /// <param name="target">事件监听目标</param>
        /// <param name="callback">回调函数</param>
        public static void AddEventListener(this RectTransform target, UnityAction<bool> callback)
        {
            Toggle toggle = target.GetComponent<Toggle>();
            if (toggle)
            {
                toggle.onValueChanged.AddListener(callback);
            }
            else
            {
                LogInfo(target.name + " 丢失了组件 Toggle！");
            }
        }
        /// <summary>
        /// Button移除所有事件监听 (onClick)
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
            return JsonMapper.ToObject(value);
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
        #endregion

        #region 查找工具
        /// <summary>
        /// 通过子物体名称获取子物体的控件
        /// </summary>
        public static T GetComponentByChild<T>(this Transform tran, string name) where T : Component
        {
            Transform gObject = tran.Find(name);
            if (gObject == null)
                return null;

            return gObject.GetComponent<T>();
        }
        /// <summary>
        /// 通过子物体名称获取子物体的控件
        /// </summary>
        public static T GetComponentByChild<T>(this GameObject obj, string name) where T : Component
        {
            Transform gObject = obj.transform.Find(name);
            if (gObject == null)
                return null;

            return gObject.GetComponent<T>();
        }
        /// <summary>
        /// 获取直系子物体上的所有组件
        /// </summary>
        public static List<T> GetComponentsInSons<T>(this Transform tran, bool includeInactive = false) where T : Component
        {
            List<T> components = new List<T>();
            for (int i = 0; i < tran.childCount; i++)
            {
                T t = tran.GetChild(i).GetComponent<T>();
                if (t)
                {
                    if (tran.GetChild(i).gameObject.activeSelf)
                    {
                        components.Add(t);
                    }
                    else
                    {
                        if (includeInactive)
                        {
                            components.Add(t);
                        }
                    }
                }
            }
            return components;
        }
        /// <summary>
        /// 获取直系子物体上的所有组件
        /// </summary>
        public static List<T> GetComponentsInSons<T>(this GameObject obj, bool includeInactive = false) where T : Component
        {
            List<T> components = new List<T>();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                T t = obj.transform.GetChild(i).GetComponent<T>();
                if (t)
                {
                    if (obj.transform.GetChild(i).gameObject.activeSelf)
                    {
                        components.Add(t);
                    }
                    else
                    {
                        if (includeInactive)
                        {
                            components.Add(t);
                        }
                    }
                }
            }
            return components;
        }
        /// <summary>
        /// 通过组件查找场景中所有的物体，包括隐藏和激活的
        /// </summary>
        public static List<T> FindObjectsOfType<T>() where T : Component
        {
            List<T> objs = new List<T>();
            GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject rootObj in rootObjs)
            {
                T[] targets = rootObj.transform.GetComponentsInChildren<T>(true);
                foreach (T target in targets)
                {
                    objs.Add(target);
                }
            }
            return objs;
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
        /// 查找兄弟
        /// </summary>
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
        /// 查找孩子
        /// </summary>
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
        public static GameObject FindChildren(this GameObject obj, string name)
        {
            Transform gObject = obj.transform.Find(name);
            if (gObject == null)
                return null;

            return gObject.gameObject;
        }
        /// <summary>
        /// 设置逆向激活
        /// </summary>
        public static void SetActiveInverse(this GameObject obj)
        {
            obj.SetActive(!obj.activeSelf);
        }
        /// <summary>
        /// 全路径
        /// </summary>
        public static string FullName(this Transform transform)
        {
            List<Transform> tfs = new List<Transform>();
            Transform tf = transform;
            tfs.Add(tf);
            while (tf.parent)
            {
                tf = tf.parent;
                tfs.Add(tf);
            }

            string name = "";
            name += tfs[tfs.Count - 1].name;
            for (int i = tfs.Count - 2; i >= 0; i--)
            {
                name += "/" + tfs[i].name;
            }
            return name;
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
            if (subStr.Length <= 0 && !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = valueRich.IndexOf(head);
            if (left >= 0)
            {
                valueRich = valueRich.Insert(left, "<b><color=" + color.ToHexSystemString() + ">");
            }

            int right = valueRich.IndexOf(end);
            if (right >= left)
            {
                valueRich = valueRich.Insert(right + 1, "</color></b>");
            }

            if (left >= 0 && right >= left)
            {
                return valueRich;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichColor(this string value, string subStr, Color color)
        {
            if (subStr.Length <= 0 && !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = valueRich.IndexOf(head);
            if (left >= 0)
            {
                valueRich = valueRich.Insert(left, "<color=" + color.ToHexSystemString() + ">");
            }

            int right = valueRich.IndexOf(end);
            if (right >= left)
            {
                valueRich = valueRich.Insert(right + 1, "</color>");
            }

            if (left >= 0 && right >= left)
            {
                return valueRich;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichSize(this string value, string subStr, int size)
        {
            if (subStr.Length <= 0 && !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = valueRich.IndexOf(head);
            if (left >= 0)
            {
                valueRich = valueRich.Insert(left, "<size=" + size + ">");
            }

            int right = valueRich.IndexOf(end);
            if (right >= left)
            {
                valueRich = valueRich.Insert(right + 1, "</size>");
            }

            if (left >= 0 && right >= left)
            {
                return valueRich;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichBold(this string value, string subStr)
        {
            if (subStr.Length <= 0 && !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = valueRich.IndexOf(head);
            if (left >= 0)
            {
                valueRich = valueRich.Insert(left, "<b>");
            }

            int right = valueRich.IndexOf(end);
            if (right >= left)
            {
                valueRich = valueRich.Insert(right + 1, "</b>");
            }

            if (left >= 0 && right >= left)
            {
                return valueRich;
            }
            else
            {
                return value;
            }
        }
        /// <summary>
        /// 将指定位置的子字串转换为富文本
        /// </summary>
        public static string ToRichItalic(this string value, string subStr)
        {
            if (subStr.Length <= 0 && !value.Contains(subStr))
            {
                return value;
            }

            string valueRich = value;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = valueRich.IndexOf(head);
            if (left >= 0)
            {
                valueRich = valueRich.Insert(left, "<i>");
            }

            int right = valueRich.IndexOf(end);
            if (right >= left)
            {
                valueRich = valueRich.Insert(right + 1, "</i>");
            }

            if (left >= 0 && right >= left)
            {
                return valueRich;
            }
            else
            {
                return value;
            }
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
            if (subStr.Length <= 0 && !tex.text.Contains(subStr))
            {
                return;
            }

            string value = tex.text;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = value.IndexOf(head);
            if (left >= 0)
            {
                value = value.Insert(left, "<size=" + size + ">");
            }

            int right = value.IndexOf(end);
            if (right >= left)
            {
                value = value.Insert(right + 1, "</size>");
            }

            if (left >= 0 && right >= left)
            {
                tex.text = value;
            }
        }
        /// <summary>
        /// 限制Text中指定子字串的字体颜色
        /// </summary>
        public static void ToRichColor(this Text tex, string subStr, Color color)
        {
            if (subStr.Length <= 0 && !tex.text.Contains(subStr))
            {
                return;
            }

            string value = tex.text;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = value.IndexOf(head);
            if (left >= 0)
            {
                value = value.Insert(left, "<color=" + color.ToHexSystemString() + ">");
            }

            int right = value.IndexOf(end);
            if (right >= left)
            {
                value = value.Insert(right + 1, "</color>");
            }

            if (left >= 0 && right >= left)
            {
                tex.text = value;
            }
        }
        /// <summary>
        /// 限制Text中的指定子字串的字体加粗
        /// </summary>
        public static void ToRichBold(this Text tex, string subStr)
        {
            if (subStr.Length <= 0 && !tex.text.Contains(subStr))
            {
                return;
            }

            string value = tex.text;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = value.IndexOf(head);
            if (left >= 0)
            {
                value = value.Insert(left, "<b>");
            }

            int right = value.IndexOf(end);
            if (right >= left)
            {
                value = value.Insert(right + 1, "</b>");
            }

            if (left >= 0 && right >= left)
            {
                tex.text = value;
            }
        }
        /// <summary>
        /// 限制Text中的指定子字串的字体斜体
        /// </summary>
        public static void ToRichItalic(this Text tex, string subStr)
        {
            if (subStr.Length <= 0 && !tex.text.Contains(subStr))
            {
                return;
            }

            string value = tex.text;
            string head = subStr.Substring(0, 1);
            string end = subStr.Substring(subStr.Length - 1, 1);

            int left = value.IndexOf(head);
            if (left >= 0)
            {
                value = value.Insert(left, "<i>");
            }

            int right = value.IndexOf(end);
            if (right >= left)
            {
                value = value.Insert(right + 1, "</i>");
            }

            if (left >= 0 && right >= left)
            {
                tex.text = value;
            }
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
        /// 世界坐标转换为UGUI坐标（只适用于框架UI模块下的控件）
        /// </summary>
        public static Vector2 ToAnchoredPosition(this Vector3 position)
        {
            Vector3 screenPos = Main.m_Controller.MainCamera.WorldToScreenPoint(position);
            screenPos.z = 0;
            Vector2 anchoredPos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Main.m_UI.UIRoot, screenPos, null, out anchoredPos);
            return anchoredPos;
        }
        #endregion

        #region 日志工具
        /// <summary>
        /// 打印普通日志
        /// </summary>
        public static void LogInfo(string value)
        {
            Debug.Log("<b><color=cyan>[HTFramework.Info " + DateTime.Now.ToString("mm:ss:fff") + "]</color></b> " + value);
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        public static void LogWarning(string value)
        {
            Debug.LogWarning("<b><color=yellow>[HTFramework.Warning " + DateTime.Now.ToString("mm:ss:fff") + "]</color></b> " + value);
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        public static void LogError(string value)
        {
            Debug.LogError("<b><color=red>[HTFramework.Error " + DateTime.Now.ToString("mm:ss:fff") + "]</color></b> " + value);
        }
        #endregion

        #region 反射工具
        /// <summary>
        /// 当前的运行时程序集
        /// </summary>
        private static readonly string[] RunTimeAssemblies = new string[] { "Assembly-CSharp", "HTFramework.RunTime", "UnityEngine" };
        
        /// <summary>
        /// 是否是当前的运行时程序集
        /// </summary>
        public static bool IsRunTimeAssembly(Assembly assembly)
        {
            string name = assembly.GetName().Name;
            for (int i = 0; i < RunTimeAssemblies.Length; i++)
            {
                if (name == RunTimeAssemblies[i])
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 从当前程序域的运行时程序集中获取所有类型
        /// </summary>
        public static List<Type> GetTypesInRunTimeAssemblies()
        {
            List<Type> types = new List<Type>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                if(IsRunTimeAssembly(assemblys[i]))
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
            for (int i = 0; i < RunTimeAssemblies.Length; i++)
            {
                type = Type.GetType(string.Format("{0},{1}", typeName, RunTimeAssemblies[i]));
                if (type != null)
                {
                    return type;
                }
            }
            LogError("获取类型 " + typeName + " 失败！当前运行时程序集中不存在此类型！");
            return null;
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
    }
}

