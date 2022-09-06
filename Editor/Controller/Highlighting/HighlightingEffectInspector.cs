using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(HighlightingEffect))]
    internal sealed class HighlightingEffectInspector : HTFEditor<HighlightingEffect>
    {
        private static readonly string[] _downsampleOptions = new string[] { "None", "Half", "Quarter" };
        private HTFAction _default;
        private HTFAction _strong;
        private HTFAction _speed;
        private HTFAction _quality;

        protected override bool IsEnableRuntimeData => false;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _default = () =>
            {
                Target.DownSampleFactorProperty = 2;
                Target.BlurIterations = 2;
                Target.BlurMinSpread = 0.65f;
                Target.BlurSpread = 0.25f;
                Target.BlurIntensityProperty = 0.3f;
            };
            _strong = () =>
            {
                Target.DownSampleFactorProperty = 2;
                Target.BlurIterations = 2;
                Target.BlurMinSpread = 0.5f;
                Target.BlurSpread = 0.15f;
                Target.BlurIntensityProperty = 0.325f;
            };
            _speed = () =>
            {
                Target.DownSampleFactorProperty = 2;
                Target.BlurIterations = 1;
                Target.BlurMinSpread = 0.75f;
                Target.BlurSpread = 0.0f;
                Target.BlurIntensityProperty = 0.35f;
            };
            _quality = () =>
            {
                Target.DownSampleFactorProperty = 1;
                Target.BlurIterations = 3;
                Target.BlurMinSpread = 1.0f;
                Target.BlurSpread = 0.0f;
                Target.BlurIntensityProperty = 0.28f;
            };
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

#if UNITY_IPHONE
		    if (PlayerSettings.use32BitDisplayBuffer == false)
		    {
                EditorGUILayout.HelpBox("Highlighting System requires 32-bit display buffer. Set the 'Use 32-bit Display Buffer' checkbox under the 'Resolution and Presentation' section of Player Settings.", MessageType.Error);
		    }
#endif
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            bool useZBuffer = EditorGUILayout.Toggle("Use Z-Buffer", Target.StencilZBufferEnabled);
            if (useZBuffer != Target.StencilZBufferEnabled)
            {
                Undo.RecordObject(Target, "Set Use Z-Buffer");
                Target.StencilZBufferEnabled = useZBuffer;
                HasChanged();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Always enable 'Use Z-Buffer' if you wish to use highlighting occluders in your project. Otherwise - keep it unchecked (in terms of performance optimization).", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Preset:", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Button(_default, "Default");
            Button(_strong, "Strong");
            Button(_speed, "Speed");
            Button(_quality, "Quality");
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            
            Target.DownSampleFactorProperty = EditorGUILayout.Popup("Downsampling:", Target.DownSampleFactorProperty, _downsampleOptions);
            Target.BlurIterations = Mathf.Clamp(EditorGUILayout.IntField("Iterations:", Target.BlurIterations), 0, 50);
            Target.BlurMinSpread = EditorGUILayout.Slider("Min Spread:", Target.BlurMinSpread, 0f, 3f);
            Target.BlurSpread = EditorGUILayout.Slider("Spread:", Target.BlurSpread, 0f, 3f);
            Target.BlurIntensityProperty = EditorGUILayout.Slider("Intensity:", Target.BlurIntensityProperty, 0f, 1f);
        }
    }
}