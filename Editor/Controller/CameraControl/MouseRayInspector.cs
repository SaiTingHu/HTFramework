using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MouseRay))]
    public sealed class MouseRayInspector : HTFEditor<MouseRay>
    {
        protected override bool IsEnableRuntimeData
        {
            get
            {
                return false;
            }
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            Toggle(Target.IsOpenRay, out Target.IsOpenRay, "Is Open Ray");
            GUILayout.EndHorizontal();

            GUI.enabled = Target.IsOpenRay;

            GUILayout.BeginHorizontal();
            Toggle(Target.IsOpenPrompt, out Target.IsOpenPrompt, "Is Open Prompt");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            PropertyField("ActivatedLayer");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.TriggerHighlighting, out Target.TriggerHighlighting, "Trigger Highlighting");
            GUILayout.EndHorizontal();

            switch (Target.TriggerHighlighting)
            {
                case MouseRay.HighlightingType.Normal:
                    GUILayout.BeginHorizontal();
                    ColorField(Target.NormalColor, out Target.NormalColor, "Normal Color");
                    GUILayout.EndHorizontal();
                    break;
                case MouseRay.HighlightingType.Flash:
                    GUILayout.BeginHorizontal();
                    ColorField(Target.FlashColor1, out Target.FlashColor1, "Flash Color 1");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    ColorField(Target.FlashColor2, out Target.FlashColor2, "Flash Color 2");
                    GUILayout.EndHorizontal();
                    break;
                case MouseRay.HighlightingType.Outline:
                    GUILayout.BeginHorizontal();
                    ColorField(Target.NormalColor, out Target.NormalColor, "Outline Color");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    FloatField(Target.OutlineIntensity, out Target.OutlineIntensity, "Outline Intensity");
                    GUILayout.EndHorizontal();
                    break;
            }

            GUILayout.BeginHorizontal();
            ObjectField(Target.RayHitBG, out Target.RayHitBG, true, "Ray Hit BG");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ObjectField(Target.RayHitText, out Target.RayHitText, true, "Ray Hit Text");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.RayHitImageType, out Target.RayHitImageType, "Ray Hit Image Type");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector2Field(Target.BGPosOffset, out Target.BGPosOffset, "BG Pos Offset");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatField(Target.BGWidthOffset, out Target.BGWidthOffset, "BG Width Offset");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntField(Target.ScreenWidthHalf, out Target.ScreenWidthHalf, "Screen Width Half");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntField(Target.ScreenHeightHalf, out Target.ScreenHeightHalf, "Screen Height Half");
            GUILayout.EndHorizontal();
        }
    }
}