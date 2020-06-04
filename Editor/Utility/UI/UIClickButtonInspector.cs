using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(UIClickButton))]
    internal sealed class UIClickButtonInspector : HTFEditor<UIClickButton>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            Toggle(Target.IsEnableDoubleClick, out Target.IsEnableDoubleClick, "Is Enable Double Click");

            if (Target.IsEnableDoubleClick)
            {
                PropertyField("DoubleClickInterval");
                PropertyField("OnMouseLeftDoubleClick");
                PropertyField("OnMouseMiddleClick");
                PropertyField("OnMouseRightClick");
            }
            else
            {
                PropertyField("OnMouseLeftClick");
                PropertyField("OnMouseMiddleClick");
                PropertyField("OnMouseRightClick");
            }
        }
    }
}