using UnityEngine;
using UnityEditor;

namespace HT.Framework
{
    [CustomEditor(typeof(HighlightingEffect))]
    public sealed class HighlightingEffectInspector : ModuleEditor
    {
        private string[] downsampleOptions = new string[] { "None", "Half", "Quarter" };

        private HighlightingEffect _he;

        protected override void OnEnable()
        {
            _he = target as HighlightingEffect;
        }

        public override void OnInspectorGUI()
        {
#if UNITY_IPHONE
		    if (Handheld.use32BitDisplayBuffer == false)
		    {
			    EditorGUILayout.HelpBox("Highlighting System requires 32-bit display buffer. Set the 'Use 32-bit Display Buffer' checkbox under the 'Resolution and Presentation' section of Player Settings.", MessageType.Error);
		    }
#endif
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            bool useZBuffer = EditorGUILayout.Toggle("Use Z-Buffer", _he.stencilZBufferEnabled);
            if (useZBuffer != _he.stencilZBufferEnabled)
            {
                Undo.RecordObject(_he, "Set Use Z-Buffer");
                _he.stencilZBufferEnabled = useZBuffer;
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
            Button(Default, "Strong");
            Button(Default, "Speed");
            Button(Default, "Quality");
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            _he.downsampleFactor = EditorGUILayout.Popup("Downsampling:", _he.downsampleFactor, downsampleOptions);
            _he.iterations = Mathf.Clamp(EditorGUILayout.IntField("Iterations:", _he.iterations), 0, 50);
            _he.blurMinSpread = EditorGUILayout.Slider("Min Spread:", _he.blurMinSpread, 0f, 3f);
            _he.blurSpread = EditorGUILayout.Slider("Spread:", _he.blurSpread, 0f, 3f);
            _he.blurIntensity = EditorGUILayout.Slider("Intensity:", _he.blurIntensity, 0f, 1f);
        }

        private void Default()
        {
            _he.downsampleFactor = 2;
            _he.iterations = 2;
            _he.blurMinSpread = 0.65f;
            _he.blurSpread = 0.25f;
            _he.blurIntensity = 0.3f;
        }
        private void Strong()
        {
            _he.downsampleFactor = 2;
            _he.iterations = 2;
            _he.blurMinSpread = 0.5f;
            _he.blurSpread = 0.15f;
            _he.blurIntensity = 0.325f;
        }
        private void Speed()
        {
            _he.downsampleFactor = 2;
            _he.iterations = 1;
            _he.blurMinSpread = 0.75f;
            _he.blurSpread = 0.0f;
            _he.blurIntensity = 0.35f;
        }
        private void Quality()
        {
            _he.downsampleFactor = 1;
            _he.iterations = 3;
            _he.blurMinSpread = 1.0f;
            _he.blurSpread = 0.0f;
            _he.blurIntensity = 0.28f;
        }
    }
}