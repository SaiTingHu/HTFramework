using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(UIClickButton))]
    internal sealed class UIClickButtonInspector : HTFEditor<UIClickButton>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(UIClickButton.IsEnableDoubleClick), "Enable Double Click");

            if (Target.IsEnableDoubleClick)
            {
                PropertyField(nameof(UIClickButton.DoubleClickInterval));
                PropertyField(nameof(UIClickButton.OnMouseLeftDoubleClick));
                PropertyField(nameof(UIClickButton.OnMouseMiddleClick));
                PropertyField(nameof(UIClickButton.OnMouseRightClick));
            }
            else
            {
                PropertyField(nameof(UIClickButton.OnMouseLeftClick));
                PropertyField(nameof(UIClickButton.OnMouseMiddleClick));
                PropertyField(nameof(UIClickButton.OnMouseRightClick));
            }
        }
    }
}