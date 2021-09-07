using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 脚本化工具箱
    /// </summary>
    internal static class ScriptableToolkit
    {
        /// <summary>
        /// 获取一个游戏对象所属的预制体，当为空时，则此对象不是预制体
        /// </summary>
        /// <param name="target">目标游戏对象</param>
        /// <returns>所属的预制体</returns>
        public static GameObject GetBelongPrefab(GameObject target)
        {
            if (PrefabUtility.IsPartOfPrefabAsset(target))
            {
                string path = AssetDatabase.GetAssetPath(target);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            if (PrefabUtility.IsPartOfPrefabInstance(target))
            {
                GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target);
                return prefabAsset;
            }
            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(target);
            if (prefabStage != null)
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.prefabAssetPath);
            }
            return null;
        }
        
        /// <summary>
        /// 保存可脚本化子对象，到指定的主对象
        /// </summary>
        /// <param name="obj">子对象</param>
        /// <param name="mainAsset">主对象</param>
        public static void SaveSubScriptableObject(ScriptableObject obj, Object mainAsset)
        {
            if (obj == null || mainAsset == null)
                return;

            obj.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            AssetDatabase.AddObjectToAsset(obj, mainAsset);

            string assetPath = AssetDatabase.GetAssetPath(mainAsset);
            EditorUtility.SetDirty(mainAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// 销毁可脚本化子对象
        /// </summary>
        /// <param name="obj">子对象</param>
        /// <param name="mainAsset">主对象</param>
        public static void DestroySubScriptableObject(ScriptableObject obj, Object mainAsset)
        {
            if (obj == null || mainAsset == null)
                return;

            AssetDatabase.RemoveObjectFromAsset(obj);
            Object.DestroyImmediate(obj);

            string assetPath = AssetDatabase.GetAssetPath(mainAsset);
            EditorUtility.SetDirty(mainAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}