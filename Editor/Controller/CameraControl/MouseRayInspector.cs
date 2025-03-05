using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(MouseRay))]
    internal sealed class MouseRayInspector : HTFEditor<MouseRay>
    {
        protected override bool IsEnableRuntimeData => false;

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            PropertyField(nameof(MouseRay.IsOpenRay), "Open Ray");

            GUI.enabled = Target.IsOpenRay;

            PropertyField(nameof(MouseRay.IsOpenPrompt), "Open Prompt");
            PropertyField(nameof(MouseRay.IsOpenHighlight), "Open Highlight");
            PropertyField(nameof(MouseRay.Is3DRay), "3D Ray");
            PropertyField(nameof(MouseRay.RayMaxDistance), "Ray Max Distance");
            PropertyField(nameof(MouseRay.ActivatedLayer), "Activated Layer");
            
            if (Target.IsOpenHighlight)
            {
                PropertyField(nameof(MouseRay.TriggerHighlighting), "Trigger Highlighting");
                PropertyField(nameof(MouseRay.IsAutoDie), "Highlight Auto Die");
                
                switch (Target.TriggerHighlighting)
                {
                    case MouseRay.HighlightingType.Normal:
                        PropertyField(nameof(MouseRay.NormalColor), "Normal Color");
                        break;
                    case MouseRay.HighlightingType.Flash:
                        PropertyField(nameof(MouseRay.FlashColor1), "Flash Color 1");
                        PropertyField(nameof(MouseRay.FlashColor2), "Flash Color 2");
                        break;
                    case MouseRay.HighlightingType.Outline:
                        PropertyField(nameof(MouseRay.NormalColor), "Outline Color");
                        PropertyField(nameof(MouseRay.OutlineIntensity), "Outline Intensity");
                        break;
                }
            }

            if (Target.IsOpenPrompt)
            {
                PropertyField(nameof(MouseRay.RayHitBG), "Ray Hit BG");
                PropertyField(nameof(MouseRay.RayHitText), "Ray Hit Text");
                PropertyField(nameof(MouseRay.RayHitImageType), "Ray Hit Image Type");
                PropertyField(nameof(MouseRay.BGPosOffset), "BG Pos Offset");
                PropertyField(nameof(MouseRay.BGWidthOffset), "BG Width Offset");
            }
        }
    }
}