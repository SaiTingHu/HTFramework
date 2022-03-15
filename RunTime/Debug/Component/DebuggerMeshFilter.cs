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
            _vertexCount = _target.sharedMesh ? _target.sharedMesh.vertexCount.ToString() : "0";
        }
        public override void OnDebuggerGUI()
        {
            ObjectFieldReadOnly("Mesh", _target.sharedMesh);
            StringFieldReadOnly("Vertex Count", _vertexCount);
        }
    }
}