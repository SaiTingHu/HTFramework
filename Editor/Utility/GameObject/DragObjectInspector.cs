using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(DragObject))]
    internal sealed class DragObjectInspector : HTFEditor<DragObject>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            Toggle(Target.IsAllowDrag, out Target.IsAllowDrag, "Is Allow Drag");

            if (Target.IsAllowDrag)
            {
                PropertyField("BeginDragEvent");
                PropertyField("DragingEvent");
                PropertyField("EndDragEvent");
            }
        }
    }
}