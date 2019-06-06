using System;
using System.Collections;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 主程序
    /// </summary>
    public sealed partial class Main : MonoBehaviour
    {
        #region Static Method
        /// <summary>
        /// 克隆实例
        /// </summary>
        public static T Clone<T>(T original) where T : UnityEngine.Object
        {
            return Instantiate(original);
        }
        /// <summary>
        /// 克隆GameObject实例
        /// </summary>
        public static GameObject CloneGameObject(GameObject original, bool isUI = false)
        {
            GameObject obj = Instantiate(original);
            obj.transform.SetParent(original.transform.parent);
            if (isUI)
            {
                obj.rectTransform().anchoredPosition3D = original.rectTransform().anchoredPosition3D;
                obj.rectTransform().sizeDelta = original.rectTransform().sizeDelta;
                obj.rectTransform().anchorMin = original.rectTransform().anchorMin;
                obj.rectTransform().anchorMax = original.rectTransform().anchorMax;
            }
            else
            {
                obj.transform.localPosition = original.transform.localPosition;
            }
            obj.transform.localRotation = original.transform.localRotation;
            obj.transform.localScale = original.transform.localScale;
            return obj;
        }
        /// <summary>
        /// 杀死实例
        /// </summary>
        public static void Kill(UnityEngine.Object obj)
        {
            Destroy(obj);
        }
        #endregion

        #region License
        //永久授权
        public bool IsPermanentLicense = true;
        public string EndingPrompt = "授权已到期！";
        public int Year = 5000;
        public int Month = 5;
        public int Day = 5;
        
        private DateTime _endingTime;
        private GUIStyle _promptStyle;

        private void LicenseAwake()
        {
            _endingTime = new DateTime(Year, Month, Day);
            _promptStyle = new GUIStyle();
            _promptStyle.alignment = TextAnchor.MiddleCenter;
            _promptStyle.normal.textColor = Color.red;
            _promptStyle.fontSize = 30;
        }
        private void LicenseUpdate()
        {
            if (!IsPermanentLicense)
            {
                if (DateTime.Now > _endingTime)
                {
                    m_Controller.MainCamera.enabled = false;
                    m_UI.HideAll = true;
                }
            }
        }
        private void LicenseOnGUI()
        {
            if (!IsPermanentLicense)
            {
                if (DateTime.Now > _endingTime)
                {
                    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), EndingPrompt, _promptStyle);
                }
            }
        }
        #endregion

        #region MainData
        public string MainDataType = "<None>";

        private MainData _data;

        private void MainDataAwake()
        {
            //加载数据类
            if (MainDataType != "<None>")
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(MainDataType);
                if (type != null)
                {
                    if (type.BaseType == typeof(MainData))
                    {
                        _data = Activator.CreateInstance(type) as MainData;
                        _data.OnInit();
                    }
                    else
                    {
                        GlobalTools.LogError("创建全局数据类失败：数据类 " + MainDataType + " 必须继承至基类：MainData！");
                    }
                }
                else
                {
                    GlobalTools.LogError("创建全局数据类失败：丢失数据类 " + MainDataType + "！");
                }
            }
        }

        /// <summary>
        /// 获取全局主要数据
        /// </summary>
        public T GetMainData<T>() where T : MainData
        {
            if (_data != null)
            {
                return _data as T;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }

    public delegate void HTFAction();
    public delegate void HTFAction<in T>(T arg);
    public delegate void HTFAction<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate IEnumerator CoroutineAction();
    public delegate IEnumerator CoroutineAction<in T>(T arg);
    public delegate IEnumerator CoroutineAction<in T1, in T2>(T1 arg1, T2 arg2);
}
