using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(DragObject))]
    internal sealed class DragObjectInspector : HTFEditor<DragObject>
    {
        protected override bool IsEnableRuntimeData => false;

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(DragObject.IsAllowDrag), "Is Allow Drag");

            if (Target.IsAllowDrag)
            {
                PropertyField(nameof(DragObject.BeginDragEvent));
                PropertyField(nameof(DragObject.DragingEvent));
                PropertyField(nameof(DragObject.EndDragEvent));
            }
        }
    }
}