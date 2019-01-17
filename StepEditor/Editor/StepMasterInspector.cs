using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepMaster))]
    public sealed class StepMasterInspector : Editor
    {
        private StepMaster _target;

        private void OnEnable()
        {
            _target = target as StepMaster;

        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Step Master, the stepflow controller!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.ContentAsset = EditorGUILayout.ObjectField("Asset", _target.ContentAsset, typeof(StepContentAsset), false) as StepContentAsset;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _target.MainCamera = EditorGUILayout.ObjectField("Camera", _target.MainCamera, typeof(Camera), false) as Camera;
            GUILayout.EndHorizontal();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }
    }
}
