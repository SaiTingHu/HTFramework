using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(CameraTarget))]
    public sealed class CameraTargetInspector : HTFEditor<CameraTarget>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            EditorGUILayout.HelpBox("Camera Control Target!", MessageType.Info);
        }
    }
}
