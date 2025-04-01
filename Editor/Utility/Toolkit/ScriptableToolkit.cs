using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 脚本化、序列化工具箱
    /// </summary>
    public static class ScriptableToolkit
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
                return AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.assetPath);
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

        /// <summary>
        /// 保存GameObject到文件中
        /// </summary>
        /// <param name="gameObject">目标GameObject</param>
        /// <param name="filePath">文件全路径</param>
        public static void SaveGameObjectToFile(GameObject gameObject, string filePath)
        {
            Component[] components = gameObject.GetComponents<Component>();
            Object[] objects = new Object[components.Length + 1];
            objects[0] = gameObject;
            for (int i = 0; i < components.Length; i++)
            {
                objects[i + 1] = components[i];
            }

            InternalEditorUtility.SaveToSerializedFileAndForget(objects, filePath, true);
        }
        /// <summary>
        /// 从文件加载GameObject
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        public static GameObject LoadGameObjectFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            Object[] objects = InternalEditorUtility.LoadSerializedFileAndForget(filePath);
            if (objects.Length > 0)
            {
                GameObject gameObject = objects[0] as GameObject;
                return gameObject;
            }
            return null;
        }
    }
}