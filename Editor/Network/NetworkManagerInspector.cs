using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(NetworkManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("")]
    public sealed class NetworkManagerInspector : HTFEditor<NetworkManager>
    {
        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Network Manager, implementing basic network client with socket!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Send Helper", GUILayout.Width(100));
            if (GUILayout.Button(Target.SendMessageHelperType, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    if (typeof(ISendMessageHelper).IsAssignableFrom(types[i]) && typeof(ISendMessageHelper) != types[i])
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), Target.SendMessageHelperType == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set SendMessageHelper");
                            Target.SendMessageHelperType = types[j].FullName;
                            HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Receive Helper", GUILayout.Width(100));
            if (GUILayout.Button(Target.ReceiveMessageHelperType, "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    if (typeof(IReceiveMessageHelper).IsAssignableFrom(types[i]) && typeof(IReceiveMessageHelper) != types[i])
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(types[j].FullName), Target.ReceiveMessageHelperType == types[j].FullName, () =>
                        {
                            Undo.RecordObject(target, "Set ReceiveMessageHelper");
                            Target.ReceiveMessageHelperType = types[j].FullName;
                            HasChanged();
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Server IP", GUILayout.Width(100));
            TextField(Target.ServerIP, out Target.ServerIP, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Port", GUILayout.Width(100));
            IntField(Target.Port, out Target.Port, "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Protocol", GUILayout.Width(100));
            EnumPopup(Target.Protocol, out Target.Protocol, "");
            GUILayout.EndHorizontal();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("No Runtime Data!");
            GUILayout.EndHorizontal();
        }
    }
}