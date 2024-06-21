using System.Text;
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
        /// <param name="context">上下文目标</param>
        /// <param name="isLinkStackTrace">为日志内容中的堆栈跟踪信息建立超链接</param>
        public static void Info(this string content, Object context = null, bool isLinkStackTrace = false)
        {
#if UNITY_EDITOR
            if (isLinkStackTrace)
            {
                content = LinkStackTrace(content);
            }
            Debug.Log(InfoPrefix + content, context);
#else
            if (Main.Current.IsEnabledLogInfo)
            {
                Debug.Log(content, context);
            }
#endif
        }
        /// <summary>
        /// 打印警告日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="context">上下文目标</param>
        /// <param name="isLinkStackTrace">为日志内容中的堆栈跟踪信息建立超链接</param>
        public static void Warning(this string content, Object context = null, bool isLinkStackTrace = false)
        {
#if UNITY_EDITOR
            if (isLinkStackTrace)
            {
                content = LinkStackTrace(content);
            }
            Debug.LogWarning(WarningPrefix + content, context);
#else
            if (Main.Current.IsEnabledLogWarning)
            {
                Debug.LogWarning(content, context);
            }
#endif
        }
        /// <summary>
        /// 打印错误日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="context">上下文目标</param>
        /// <param name="isLinkStackTrace">为日志内容中的堆栈跟踪信息建立超链接</param>
        public static void Error(this string content, Object context = null, bool isLinkStackTrace = false)
        {
#if UNITY_EDITOR
            if (isLinkStackTrace)
            {
                content = LinkStackTrace(content);
            }
            Debug.LogError(ErrorPrefix + content, context);
#else
            if (Main.Current.IsEnabledLogError)
            {
                Debug.LogError(content, context);
            }
#endif
        }

        private static string LinkStackTrace(string content)
        {
            StringBuilder sb = new StringBuilder();
            string[] texts = content.Split('\n');
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].Contains(" at ") && texts[i].Contains(" in "))
                {
                    string path;
                    int line;
                    SplitPathAndLine(texts[i], out path, out line);
                    if (!string.IsNullOrEmpty(path))
                    {
                        string str = $"{path}:{line}";
                        texts[i] = texts[i].Replace(str, Hyperlink(str, path, line));
                    }
                }
                sb.Append(texts[i]);
                sb.Append('\n');
            }
            return sb.ToString();
        }
        private static void SplitPathAndLine(string text, out string path, out int line)
        {
            string[] texts = text.Split(" in ");
            if (texts.Length == 2)
            {
                texts = texts[1].Split(':');
                if (texts.Length >= 2)
                {
                    path = texts[texts.Length - 2].Trim();
                    path = path.Substring(path.IndexOf("\\Assets\\") + 1);
                    int.TryParse(texts[texts.Length - 1].Trim(), out line);
                }
                else
                {
                    path = null;
                    line = 0;
                }
            }
            else
            {
                path = null;
                line = 0;
            }
        }
    }
}