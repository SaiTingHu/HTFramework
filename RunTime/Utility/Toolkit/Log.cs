using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 日志工具箱
    /// </summary>
    public static class Log
    {
#if UNITY_EDITOR
        private const string InfoPrefix = "<b><color=cyan>[HTFramework.Info]</color></b> ";
        private const string WarningPrefix = "<b><color=yellow>[HTFramework.Warning]</color></b> ";
        private const string ErrorPrefix = "<b><color=red>[HTFramework.Error]</color></b> ";
#endif
        private static ILogHandler _handler;

        /// <summary>
        /// 日志处理器
        /// </summary>
        private static ILogHandler Handler
        {
            get
            {
                if (_handler == null)
                {
                    _handler = Debug.unityLogger.logHandler;
                }
                return _handler;
            }
        }

        /// <summary>
        /// 转换为超链接文本（仅在编辑器中控制台生效）
        /// </summary>
        /// <param name="content">原文本</param>
        /// <param name="href">超链接，可为网络地址或资源路径</param>
        /// <param name="line">超链接为资源路径时，链接的行数</param>
        /// <returns>超链接文本</returns>
        public static string Hyperlink(this string content, string href, int line = 0)
        {
#if UNITY_EDITOR
            return $"<a href=\"{href}\" line=\"{line}\">{content}</a>";
#else
            return content;
#endif
        }
        /// <summary>
        /// 生成链接到此对象脚本文件的超链接文本（仅在编辑器中控制台生效）
        /// </summary>
        /// <param name="behaviour">脚本对象</param>
        /// <param name="line">链接的行数</param>
        /// <returns>超链接文本</returns>
        public static string HyperlinkFile(this MonoBehaviour behaviour, int line = 0)
        {
            if (behaviour == null)
                return null;

            string name = behaviour.GetType().FullName;
#if UNITY_EDITOR
            UnityEditor.MonoScript monoScript = UnityEditor.MonoScript.FromMonoBehaviour(behaviour);
            string path = UnityEditor.AssetDatabase.GetAssetPath(monoScript);
            return $"<a href=\"{path}\" line=\"{line}\">{name}</a>";
#else
            return name;
#endif
        }
        /// <summary>
        /// 生成链接到此对象脚本文件的超链接文本（仅在编辑器中控制台生效）
        /// </summary>
        /// <param name="scriptableObject">脚本对象</param>
        /// <param name="line">链接的行数</param>
        /// <returns>超链接文本</returns>
        public static string HyperlinkFile(this ScriptableObject scriptableObject, int line = 0)
        {
            if (scriptableObject == null)
                return null;

            string name = scriptableObject.GetType().FullName;
#if UNITY_EDITOR
            UnityEditor.MonoScript monoScript = UnityEditor.MonoScript.FromScriptableObject(scriptableObject);
            string path = UnityEditor.AssetDatabase.GetAssetPath(monoScript);
            return $"<a href=\"{path}\" line=\"{line}\">{name}</a>";
#else
            return name;
#endif
        }
        /// <summary>
        /// 打印信息日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void Info(this string content)
        {
            Info(content, null);
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void Warning(this string content)
        {
            Warning(content, null);
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void Error(this string content)
        {
            Error(content, null);
        }
        /// <summary>
        /// 打印信息日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="context">上下文目标</param>
        public static void Info(this string content, Object context)
        {
#if UNITY_EDITOR
            Handler.LogFormat(LogType.Log, context, InfoPrefix + content);
#else
            if (Main.Current.IsEnabledLogInfo)
            {
                Handler.LogFormat(LogType.Log, context, content);
            }
#endif
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="context">上下文目标</param>
        public static void Warning(this string content, Object context)
        {
#if UNITY_EDITOR
            Handler.LogFormat(LogType.Warning, context, WarningPrefix + content);
#else
            if (Main.Current.IsEnabledLogWarning)
            {
                Handler.LogFormat(LogType.Warning, context, content);
            }
#endif
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="context">上下文目标</param>
        public static void Error(this string content, Object context)
        {
#if UNITY_EDITOR
            Handler.LogFormat(LogType.Error, context, ErrorPrefix + content);
#else
            if (Main.Current.IsEnabledLogError)
            {
                Handler.LogFormat(LogType.Error, context, content);
            }
#endif
        }
    }
}