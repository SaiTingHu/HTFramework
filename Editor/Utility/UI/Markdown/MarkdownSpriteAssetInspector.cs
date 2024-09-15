using UnityEditor;
using UnityEngine;
using static HT.Framework.MarkdownSpriteAsset;

namespace HT.Framework
{
    [CustomEditor(typeof(MarkdownSpriteAsset))]
    internal sealed class MarkdownSpriteAssetInspector : HTFEditor<MarkdownSpriteAsset>
    {
        private GUIContent _deleteGC;

        protected override void OnDefaultEnable()
        {
            base.OnDefaultEnable();

            _deleteGC = new GUIContent();
            _deleteGC.image = EditorGUIUtility.IconContent("TreeEditor.Trash").image;
            _deleteGC.tooltip = "Delete";
        }
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sprite Number: " + Target.SpriteTargets.Count);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("New Sprite", GUILayout.Width(100)))
            {
                Target.SpriteTargets.Add(new SpriteTarget());
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.SpriteTargets.Count; i++)
            {
                SpriteTarget spriteTarget = Target.SpriteTargets[i];

                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal();
                spriteTarget.ID = EditorGUILayout.TextField("ID", spriteTarget.ID);
                if (GUILayout.Button(_deleteGC, EditorGlobalTools.Styles.InvisibleButton, GUILayout.Width(20)))
                {
                    Target.SpriteTargets.RemoveAt(i);
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                spriteTarget.Target = EditorGUILayout.ObjectField("Sprite", spriteTarget.Target, typeof(Sprite), false) as Sprite;
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            if (Target.SpriteTargets.Count > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Sprite Number: " + Target.SpriteTargets.Count);
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("New Sprite", GUILayout.Width(100)))
                {
                    Target.SpriteTargets.Add(new SpriteTarget());
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }

            if (GUI.changed)
            {
                HasChanged();
            }
        }
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            GUI.Label(new Rect(45, 25, 120, 20), "MarkdownSpriteAsset", "AssetLabel");
        }
    }
}