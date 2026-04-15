using System.Runtime.InteropServices;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 剪切板工具箱
    /// </summary>
    public static class ClipboardToolkit
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        internal static extern void CopyToClipboardWebGL(string content);
#endif

        /// <summary>
        /// 复制文本内容到剪切板
        /// </summary>
        /// <param name="content">文本内容</param>
        public static void CopyToClipboard(string content)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            GUIUtility.systemCopyBuffer = content;
#elif UNITY_WEBGL
            CopyToClipboardWebGL(content);
#else
            Log.Warning("复制文本内容到剪切板失败：当前平台不支持该操作。");
#endif
        }
    }
}