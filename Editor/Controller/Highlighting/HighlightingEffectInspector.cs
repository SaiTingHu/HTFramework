using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(HighlightingEffect))]
    public sealed class HighlightingEffectInspector : HTFEditor<HighlightingEffect>
    {
        private static string[] _downsampleOptions = new string[] { "None", "Half", "Quarter" };
        
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

#if UNITY_IPHONE
		    if (Handheld.use32BitDisplayBuffer == false)
		    {
			    EditorGUILayout.HelpBox("Highlighting System requires 32-bit display buffer. Set the 'Use 32-bit Display Buffer' checkbox under the 'Resolution and Presentation' section of Player Settings.", MessageType.Error);
		    }
#endif
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            bool useZBuffer = EditorGUILayout.Toggle("Use Z-Buffer", Target.stencilZBufferEnabled);
            if (useZBuffer != Target.stencilZBufferEnabled)
            {
                Undo.RecordObject(Target, "Set Use Z-Buffer");
                Target.stencilZBufferEnabled = useZBuffer;
                HasChanged();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Always enable 'Use Z-Buffer' if you wish to use highlighting occluders in your project. Otherwise - keep it unchecked (in terms of performance optimization).", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Preset:", "BoldLabel");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Button(Default, "Default");
            Button(Strong, "Strong");
            Button(Speed, "Speed");
            Button(Quality, "Quality");
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            Target.downsampleFactor = EditorGUILayout.Popup("Downsampling:", Target.downsampleFactor, _downsampleOptions);
            Target.iterations = Mathf.Clamp(EditorGUILayout.IntField("Iterations:", Target.iterations), 0, 50);
            Target.blurMinSpread = EditorGUILayout.Slider("Min Spread:", Target.blurMinSpread, 0f, 3f);
            Target.blurSpread = EditorGUILayout.Slider("Spread:", Target.blurSpread, 0f, 3f);
            Target.blurIntensity = EditorGUILayout.Slider("Intensity:", Target.blurIntensity, 0f, 1f);
        }

        private void Default()
        {
            Target.downsampleFactor = 2;
            Target.iterations = 2;
            Target.blurMinSpread = 0.65f;
            Target.blurSpread = 0.25f;
            Target.blurIntensity = 0.3f;
        }
        private void Strong()
        {
            Target.downsampleFactor = 2;
            Target.iterations = 2;
            Target.blurMinSpread = 0.5f;
            Target.blurSpread = 0.15f;
            Target.blurIntensity = 0.325f;
        }
        private void Speed()
        {
            Target.downsampleFactor = 2;
            Target.iterations = 1;
            Target.blurMinSpread = 0.75f;
            Target.blurSpread = 0.0f;
            Target.blurIntensity = 0.35f;
        }
        private void Quality()
        {
            Target.downsampleFactor = 1;
            Target.iterations = 3;
            Target.blurMinSpread = 1.0f;
            Target.blurSpread = 0.0f;
            Target.blurIntensity = 0.28f;
        }
    }
}