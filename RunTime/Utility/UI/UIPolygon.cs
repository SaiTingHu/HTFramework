using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    /// <summary>
    /// 多边形点击区域
    /// </summary>
    [AddComponentMenu("HTFramework/UI/UIPolygon")]
    [RequireComponent(typeof(PolygonCollider2D))]
    [DisallowMultipleComponent]
    public sealed class UIPolygon : Image
    {
        private PolygonCollider2D _polygon = null;
        private PolygonCollider2D polygon
        {
            get
            {
                if (_polygon == null)
                    _polygon = GetComponent<PolygonCollider2D>();
                return _polygon;
            }
        }

        public UIPolygon()
        {
            useLegacyMeshGeneration = true;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (!eventCamera)
                return false;

            return polygon.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPoint));
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            transform.localPosition = Vector3.zero;
            float w = (rectTransform.sizeDelta.x * 0.5f) + 0.1f;
            float h = (rectTransform.sizeDelta.y * 0.5f) + 0.1f;
            polygon.points = new Vector2[]
            {
            new Vector2(-w,-h),
            new Vector2(w,-h),
            new Vector2(w,h),
            new Vector2(-w,h)
            };
        }
#endif
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UIPolygon), true)]
    public class UIPolygonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("多边形触发器，请点击编辑按钮编辑PolygonCollider2D的区域", MessageType.Info);
        }
    }
#endif
}