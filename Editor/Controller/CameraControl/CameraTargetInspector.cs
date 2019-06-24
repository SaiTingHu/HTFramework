using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(CameraTarget))]
    public sealed class CameraTargetInspector : ModuleEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Camera Control Target!", MessageType.Info);
        }
    }
}
