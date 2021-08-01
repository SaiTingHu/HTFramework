using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 任务内容序列化资源
    /// </summary>
    [CreateAssetMenu(menuName = "HTFramework/Task Content Asset", order = 0)]
    [Serializable]
    public sealed class TaskContentAsset : DataSetBase
    {
        [SerializeField] internal List<TaskContentBase> Content = new List<TaskContentBase>();
        [SerializeField] internal int TaskIDSign = 1;
        [SerializeField] internal string TaskIDName = "Task";
        [SerializeField] internal int TaskPointIDSign = 1;
        [SerializeField] internal string TaskPointIDName = "TaskPoint";

#if UNITY_EDITOR
        /// <summary>
        /// 生成序列化子对象
        /// </summary>
        /// <param name="obj">子对象</param>
        /// <param name="mainAsset">主对象</param>
        internal static void GenerateSerializeSubObject(UnityEngine.Object obj, UnityEngine.Object mainAsset)
        {
            obj.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(obj, mainAsset);

            string assetPath = AssetDatabase.GetAssetPath(mainAsset);
            AssetDatabase.SetMainObject(mainAsset, assetPath);
            EditorUtility.SetDirty(mainAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// 销毁序列化子对象
        /// </summary>
        /// <param name="obj">子对象</param>
        /// <param name="mainAsset">主对象</param>
        internal static void DestroySerializeSubObject(UnityEngine.Object obj, UnityEngine.Object mainAsset)
        {
            AssetDatabase.RemoveObjectFromAsset(obj);
            DestroyImmediate(obj);

            string assetPath = AssetDatabase.GetAssetPath(mainAsset);
            EditorUtility.SetDirty(mainAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 是否存在丢失脚本的对象
        /// </summary>
        internal bool IsExistMissed()
        {
            for (int i = 0; i < Content.Count; i++)
            {
                TaskContentBase content = Content[i];
                if (content == null)
                {
                    return true;
                }
                else
                {
                    for (int j = 0; j < content.Points.Count; j++)
                    {
                        TaskPointBase point = content.Points[j];
                        if (point == null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
#endif
    }
}