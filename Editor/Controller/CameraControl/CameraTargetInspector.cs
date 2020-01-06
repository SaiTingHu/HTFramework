using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(CameraTarget))]
    internal sealed class CameraTargetInspector : HTFEditor<CameraTarget>
    {
        protected override bool IsEnableRuntimeData
        {
            get
            {
                return false;
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            EditorGUILayout.HelpBox("Camera Control Target!", MessageType.Info);
        }
    }
}