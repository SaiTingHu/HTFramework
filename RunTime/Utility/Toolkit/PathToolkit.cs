using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 路径工具箱
    /// </summary>
    public static class PathToolkit
    {
        private static string _projectPath = null;

        /// <summary>
        /// 项目路径（也即是 Application.dataPath 路径的末尾去掉了 Assets）
        /// </summary>
        public static string ProjectPath
        {
            get
            {
                if (string.IsNullOrEmpty(_projectPath))
                {
                    _projectPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1);
                }
                return _projectPath;
            }
        }
    }
}