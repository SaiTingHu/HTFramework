using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(StepMaster))]
    public sealed class StepMasterInspector : ModuleEditor
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
            ObjectField(_target.ContentAsset, out _target.ContentAsset, false, "Asset");
            GUILayout.EndHorizontal();
        }
    }
}
