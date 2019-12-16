using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(NetworkManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("")]
    public sealed class NetworkManagerInspector : HTFEditor<NetworkManager>
    {
        private Dictionary<Type, ProtocolChannel> _protocolChannels;

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _protocolChannels = Target.GetType().GetField("_protocolChannels", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as Dictionary<Type, ProtocolChannel>;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Network Manager, implementing basic network client with socket!", MessageType.Info);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            TextField(Target.ServerIP, out Target.ServerIP, "Server IP");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntField(Target.ServerPort, out Target.ServerPort, "Server Port");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            TextField(Target.ClientIP, out Target.ClientIP, "Client IP");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            IntField(Target.ClientPort, out Target.ClientPort, "Client Port");
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled Channels:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            for (int i = 0; i < Target.ChannelTypes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((i + 1) + "." + Target.ChannelTypes[i]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete", "minibutton"))
                {
                    Undo.RecordObject(target, "Delete Channel");
                    Target.ChannelTypes.RemoveAt(i);
                    HasChanged();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Channel", "MiniPopup"))
            {
                GenericMenu gm = new GenericMenu();
                List<Type> types = GlobalTools.GetTypesInRunTimeAssemblies();
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].IsSubclassOf(typeof(ProtocolChannel)))
                    {
                        int j = i;
                        if (Target.ChannelTypes.Contains(types[j].FullName))
                        {
                            gm.AddDisabledItem(new GUIContent(types[j].FullName));
                        }
                        else
                        {
                            gm.AddItem(new GUIContent(types[j].FullName), false, () =>
                            {
                                Undo.RecordObject(target, "Add Channel");
                                Target.ChannelTypes.Add(types[j].FullName);
                                HasChanged();
                            });
                        }
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enabled Channels:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            foreach (var channel in _protocolChannels)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(channel.Key.FullName + " [" + channel.Value.Protocol.ToString() + "]");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("IsConnect:" + channel.Value.IsConnect.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
    }
}