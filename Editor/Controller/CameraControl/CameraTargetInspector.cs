using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(CameraTarget))]
    internal sealed class CameraTargetInspector : HTFEditor<CameraTarget>
    {
        protected override bool IsEnableRuntimeData => false;

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            EditorGUILayout.HelpBox("Camera Watch Target!", MessageType.Info);
        }
    }
}