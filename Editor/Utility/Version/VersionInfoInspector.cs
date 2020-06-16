using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(VersionInfo))]
    internal sealed class VersionInfoInspector : HTFEditor<VersionInfo>
    {
        private string _versionNumber;

        protected override bool IsEnableRuntimeData
        {
            get
            {
                return false;
            }
        }
        
        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _versionNumber = Target.CurrentVersion.GetFullNumber();
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Version: " + _versionNumber);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Release Notes: ");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label(Target.CurrentVersion.ReleaseNotes);
            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }
    }
}