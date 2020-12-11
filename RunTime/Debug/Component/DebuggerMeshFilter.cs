using UnityEngine;

namespace HT.Framework
{
    [CustomDebugger(typeof(MeshFilter))]
    internal sealed class DebuggerMeshFilter : DebuggerComponentBase
    {
        private MeshFilter _target;
        private string _vertexCount;

        public override void OnEnable()
        {
            _target = Target as MeshFilter;
            _vertexCount = _target.mesh ? _target.mesh.vertexCount.ToString() : "0";
        }

        public override void OnDebuggerGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mesh: " + (_target.mesh ? _target.mesh.name : "None"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Vertex Count: " + _vertexCount);
            GUILayout.EndHorizontal();
        }
    }
}