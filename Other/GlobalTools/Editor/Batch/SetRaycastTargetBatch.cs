using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace HT.Framework
{
    public sealed class SetRaycastTargetBatch : EditorWindow
    {
        private GameObject _root;
        private bool _text = false;
        private bool _image = false;
        private bool _rawImage = false;
        private bool _button = true;
        private bool _toggle = true;
        private bool _slider = true;
        private bool _scrollbar = true;
        private bool _inputField = true;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("设置Root及以下所有Graphic组件的Raycast Target属性", MessageType.Info);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Root：", GUILayout.Width(100));
            _root = EditorGUILayout.ObjectField(_root, typeof(GameObject), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            GUI.enabled = _root;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Text", GUILayout.Width(100));
            _text = GUILayout.Toggle(_text, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Image", GUILayout.Width(100));
            _image = GUILayout.Toggle(_image, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("RawImage", GUILayout.Width(100));
            _rawImage = GUILayout.Toggle(_rawImage, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Button", GUILayout.Width(100));
            _button = GUILayout.Toggle(_button, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Toggle", GUILayout.Width(100));
            _toggle = GUILayout.Toggle(_toggle, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Slider", GUILayout.Width(100));
            _slider = GUILayout.Toggle(_slider, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scrollbar", GUILayout.Width(100));
            _scrollbar = GUILayout.Toggle(_scrollbar, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("InputField", GUILayout.Width(100));
            _inputField = GUILayout.Toggle(_inputField, "");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set"))
            {
                Text[] ts = _root.GetComponentsInChildren<Text>(true);
                foreach (Text t in ts)
                {
                    t.raycastTarget = _text;
                }
                GlobalTools.LogInfo("[" + _root.name + "] Text设置完成！共设置了 " + ts.Length + " 处Raycast Target为" + _text + "！");

                Image[] ims = _root.GetComponentsInChildren<Image>(true);
                foreach (Image i in ims)
                {
                    i.raycastTarget = _image;
                }
                GlobalTools.LogInfo("[" + _root.name + "] Image设置完成！共设置了 " + ims.Length + " 处Raycast Target为" + _image + "！");

                RawImage[] ris = _root.GetComponentsInChildren<RawImage>(true);
                foreach (RawImage ri in ris)
                {
                    ri.raycastTarget = _rawImage;
                }
                GlobalTools.LogInfo("[" + _root.name + "] RawImage设置完成！共设置了 " + ris.Length + " 处Raycast Target为" + _rawImage + "！");

                Button[] bs = _root.GetComponentsInChildren<Button>(true);
                foreach (Button b in bs)
                {
                    if (b.targetGraphic)
                    {
                        b.targetGraphic.raycastTarget = _button;
                    }
                }
                GlobalTools.LogInfo("[" + _root.name + "] Button设置完成！共设置了 " + bs.Length + " 处Raycast Target为" + _button + "！");

                Toggle[] tos = _root.GetComponentsInChildren<Toggle>(true);
                foreach (Toggle t in tos)
                {
                    if (t.graphic)
                    {
                        t.graphic.raycastTarget = _toggle;
                    }
                }
                GlobalTools.LogInfo("[" + _root.name + "] Toggle设置完成！共设置了 " + tos.Length + " 处Raycast Target为" + _toggle + "！");

                Slider[] ss = _root.GetComponentsInChildren<Slider>(true);
                foreach (Slider t in ss)
                {
                    if (t.fillRect)
                    {
                        t.fillRect.GetComponent<Image>().raycastTarget = _slider;
                    }
                }
                GlobalTools.LogInfo("[" + _root.name + "] Slider设置完成！共设置了 " + ss.Length + " 处Raycast Target为" + _slider + "！");

                Scrollbar[] slls = _root.GetComponentsInChildren<Scrollbar>(true);
                foreach (Scrollbar s in slls)
                {
                    if (s.GetComponent<Image>())
                    {
                        s.GetComponent<Image>().raycastTarget = _scrollbar;
                    }
                    if (s.handleRect)
                    {
                        s.handleRect.GetComponent<Image>().raycastTarget = _scrollbar;
                    }
                }
                GlobalTools.LogInfo("[" + _root.name + "] Scrollbar设置完成！共设置了 " + slls.Length + " 处Raycast Target为" + _scrollbar + "！");

                InputField[] ifs = _root.GetComponentsInChildren<InputField>(true);
                foreach (InputField i in ifs)
                {
                    if (i.textComponent)
                    {
                        i.textComponent.raycastTarget = _inputField;
                    }
                }
                GlobalTools.LogInfo("[" + _root.name + "] InputField设置完成！共设置了 " + ifs.Length + " 处Raycast Target为" + _inputField + "！");
            }
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }
    }
}