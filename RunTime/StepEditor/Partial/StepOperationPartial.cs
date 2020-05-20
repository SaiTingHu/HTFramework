using UnityEngine;
using System.Reflection;
using DG.Tweening;
using System;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HT.Framework
{
    public sealed partial class StepOperation
    {
        public Vector3 Vector3Value = Vector3.zero;
        public Vector2 Vector2Value = Vector2.zero;
        public Color ColorValue = Color.white;
        public int IntValue = 0;
        public float FloatValue = 0;
        public string StringValue = "<None>";
        public bool BoolValue = false;
        public Ease AnimationEase = Ease.Linear;

        public Vector3 Vector3Value2 = Vector3.zero;
        public string StringValue2 = "<None>";
        public bool BoolValue2 = false;

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns>新的对象</returns>
        internal StepOperation Clone()
        {
            StepOperation operation = new StepOperation();
            operation.GUID = Guid.NewGuid().ToString();
            operation.Anchor = Anchor;
            operation.ElapseTime = ElapseTime;
            operation.Instant = Instant;
            operation.Target = Target;
            operation.TargetGUID = TargetGUID;
            operation.TargetPath = TargetPath;
            operation.Name = Name;
            operation.OperationType = OperationType;

            operation.Vector3Value = Vector3Value;
            operation.Vector2Value = Vector2Value;
            operation.ColorValue = ColorValue;
            operation.IntValue = IntValue;
            operation.FloatValue = FloatValue;
            operation.StringValue = StringValue;
            operation.BoolValue = BoolValue;
            operation.AnimationEase = AnimationEase;

            operation.Vector3Value2 = Vector3Value2;
            operation.StringValue2 = StringValue2;
            operation.BoolValue2 = BoolValue2;

            return operation;
        }

        internal void Execute()
        {
            switch (OperationType)
            {
                case StepOperationType.Move:
                    MoveExecute();
                    break;
                case StepOperationType.Rotate:
                    RotateExecute();
                    break;
                case StepOperationType.Scale:
                    ScaleExecute();
                    break;
                case StepOperationType.Color:
                    ColorExecute();
                    break;
                case StepOperationType.Active:
                    ActiveExecute();
                    break;
                case StepOperationType.Action:
                    ActionExecute();
                    break;
                case StepOperationType.ActionArgs:
                    ActionArgsExecute();
                    break;
                case StepOperationType.CameraFollow:
                    CameraFollowExecute();
                    break;
                case StepOperationType.TextMesh:
                    TextMeshExecute();
                    break;
                case StepOperationType.Prompt:
                    PromptExecute();
                    break;
                case StepOperationType.FSM:
                    FSMExecute();
                    break;
                case StepOperationType.Delay:
                    DelayExecute();
                    break;
                case StepOperationType.ActiveComponent:
                    ActiveComponentExecute();
                    break;
                default:
                    Log.Warning("步骤控制者：[" + OperationType + " 操作] 没有可以执行的 Execute 定义！");
                    break;
            }
        }
        private void MoveExecute()
        {
            if (BoolValue)
            {
                Target.transform.localPosition = Vector3Value;
            }
            else
            {
                Target.transform.DOLocalMove(Vector3Value, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void RotateExecute()
        {
            if (BoolValue2)
            {
                Target.transform.localRotation = Quaternion.Euler(Vector3Value);
            }
            else
            {
                if (BoolValue)
                {
                    Target.transform.DOLocalRotate(Vector3Value, ElapseTime, RotateMode.LocalAxisAdd).SetEase(AnimationEase);
                }
                else
                {
                    Target.transform.DOLocalRotate(Vector3Value, ElapseTime).SetEase(AnimationEase);
                }
            }
        }
        private void ScaleExecute()
        {
            if (BoolValue)
            {
                Target.transform.localScale = Vector3Value;
            }
            else
            {
                Target.transform.DOScale(Vector3Value, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void ColorExecute()
        {
            if (BoolValue)
            {
                if (!Target.GetComponent<Renderer>())
                {
                    Log.Error("步骤控制者：目标 " + Target.name + " 丢失组件Renderer！无法播放颜色改变动画！");
                    return;
                }
                Target.GetComponent<Renderer>().material.DOColor(ColorValue, ElapseTime).SetEase(AnimationEase);
            }
            if (BoolValue2)
            {
                if (!Target.GetComponent<Graphic>())
                {
                    Log.Error("步骤控制者：目标 " + Target.name + " 丢失组件Graphic！无法播放颜色改变动画！");
                    return;
                }
                Target.GetComponent<Graphic>().DOColor(ColorValue, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void ActiveExecute()
        {
            Target.SetActive(BoolValue);
        }
        private void ActionExecute()
        {
            if (StringValue != "<None>")
            {
                Target.SendMessage(StringValue);
            }
        }
        private void ActionArgsExecute()
        {
            if (StringValue != "<None>")
            {
                Target.SendMessage(StringValue, StringValue2);
            }
        }
        private void CameraFollowExecute()
        {
            Main.m_Controller.SetLookPoint(Vector3Value, true);
            Main.m_Controller.SetLookAngle(Vector2Value, FloatValue, true);
        }
        private void TextMeshExecute()
        {
            if (!Target.GetComponent<TextMesh>())
            {
                Log.Error("步骤控制者：目标 " + Target.name + " 丢失组件TextMesh！无法设置TextMesh文本！");
                return;
            }
            Target.GetComponent<TextMesh>().text = StringValue;
        }
        private void PromptExecute()
        {
            Main.m_StepMaster.ShowPrompt(StringValue);
        }
        private void FSMExecute()
        {
            if (StringValue != "<None>")
            {
                Target.GetComponent<FSM>().SwitchState(ReflectionToolkit.GetTypeInRunTimeAssemblies(StringValue));
            }
        }
        private void DelayExecute()
        {
        }
        private void ActiveComponentExecute()
        {
            Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(StringValue);
            if (type != null)
            {
                Component component = Target.GetComponent(type);
                Behaviour behaviour = component as Behaviour;
                Collider collider = component as Collider;
                Renderer renderer = component as Renderer;
                if (behaviour) behaviour.enabled = BoolValue;
                else if (collider) collider.enabled = BoolValue;
                else if (renderer) renderer.enabled = BoolValue;
            }
            else
            {

                Log.Error("步骤控制者：未获取到组件类型 " + StringValue + " ！");
            }
        }

        internal void Skip()
        {
            switch (OperationType)
            {
                case StepOperationType.Move:
                    MoveSkip();
                    break;
                case StepOperationType.Rotate:
                    RotateSkip();
                    break;
                case StepOperationType.Scale:
                    ScaleSkip();
                    break;
                case StepOperationType.Color:
                    ColorSkip();
                    break;
                case StepOperationType.Active:
                    ActiveSkip();
                    break;
                case StepOperationType.Action:
                    ActionSkip();
                    break;
                case StepOperationType.ActionArgs:
                    ActionArgsSkip();
                    break;
                case StepOperationType.CameraFollow:
                    CameraFollowSkip();
                    break;
                case StepOperationType.TextMesh:
                    TextMeshSkip();
                    break;
                case StepOperationType.Prompt:
                    PromptSkip();
                    break;
                case StepOperationType.FSM:
                    FSMSkip();
                    break;
                case StepOperationType.Delay:
                    DelaySkip();
                    break;
                case StepOperationType.ActiveComponent:
                    ActiveComponentSkip();
                    break;
                default:
                    Log.Warning("步骤控制者：[" + OperationType + " 操作] 没有可以执行的 Skip 定义！");
                    break;
            }
        }
        private void MoveSkip()
        {
            if (BoolValue)
            {
                Target.transform.localPosition = Vector3Value;
            }
            else
            {
                Target.transform.DOLocalMove(Vector3Value, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
        }
        private void RotateSkip()
        {
            if (BoolValue2)
            {
                Target.transform.localRotation = Quaternion.Euler(Vector3Value);
            }
            else
            {
                if (BoolValue)
                {
                    Target.transform.DOLocalRotate(Vector3Value, ElapseTime / StepMaster.SkipMultiple, RotateMode.LocalAxisAdd).SetEase(AnimationEase);
                }
                else
                {
                    Target.transform.DOLocalRotate(Vector3Value, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
                }
            }
        }
        private void ScaleSkip()
        {
            if (BoolValue)
            {
                Target.transform.localScale = Vector3Value;
            }
            else
            {
                Target.transform.DOScale(Vector3Value, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
        }
        private void ColorSkip()
        {
            if (BoolValue)
            {
                if (!Target.GetComponent<Renderer>())
                {
                    Log.Error("步骤控制者：目标 " + Target.name + " 丢失组件Renderer！无法播放颜色改变动画！");
                    return;
                }
                Target.GetComponent<Renderer>().material.DOColor(ColorValue, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
            if (BoolValue2)
            {
                if (!Target.GetComponent<Graphic>())
                {
                    Log.Error("步骤控制者：目标 " + Target.name + " 丢失组件Graphic！无法播放颜色改变动画！");
                }
                Target.GetComponent<Graphic>().DOColor(ColorValue, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
        }
        private void ActiveSkip()
        {
            Target.SetActive(BoolValue);
        }
        private void ActionSkip()
        {
            if (StringValue != "<None>")
            {
                Target.SendMessage(StringValue);
            }
        }
        private void ActionArgsSkip()
        {
            if (StringValue != "<None>")
            {
                Target.SendMessage(StringValue, StringValue2);
            }
        }
        private void CameraFollowSkip()
        {
            Main.m_Controller.SetLookPoint(Vector3Value, false);
            Main.m_Controller.SetLookAngle(Vector2Value, FloatValue, true);
        }
        private void TextMeshSkip()
        {
            if (!Target.GetComponent<TextMesh>())
            {
                Log.Error("步骤控制者：目标 " + Target.name + " 丢失组件TextMesh！无法设置TextMesh文本！");
                return;
            }
            Target.GetComponent<TextMesh>().text = StringValue;
        }
        private void PromptSkip()
        {
            Main.m_StepMaster.ShowPrompt(StringValue);
        }
        private void FSMSkip()
        {
            if (StringValue != "<None>")
            {
                Target.GetComponent<FSM>().SwitchState(ReflectionToolkit.GetTypeInRunTimeAssemblies(StringValue));
            }
        }
        private void DelaySkip()
        {
        }
        private void ActiveComponentSkip()
        {
            Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(StringValue);
            if (type != null)
            {
                Component component = Target.GetComponent(type);
                Behaviour behaviour = component as Behaviour;
                Collider collider = component as Collider;
                Renderer renderer = component as Renderer;
                if (behaviour) behaviour.enabled = BoolValue;
                else if (collider) collider.enabled = BoolValue;
                else if (renderer) renderer.enabled = BoolValue;
            }
            else
            {
                Log.Error("步骤控制者：未获取到组件类型 " + StringValue + " ！");
            }
        }

#if UNITY_EDITOR
        internal void OnEditorGUI()
        {
            switch (OperationType)
            {
                case StepOperationType.Move:
                    MoveGUI();
                    break;
                case StepOperationType.Rotate:
                    RotateGUI();
                    break;
                case StepOperationType.Scale:
                    ScaleGUI();
                    break;
                case StepOperationType.Color:
                    ColorGUI();
                    break;
                case StepOperationType.Active:
                    ActiveGUI();
                    break;
                case StepOperationType.Action:
                    ActionGUI();
                    break;
                case StepOperationType.ActionArgs:
                    ActionArgsGUI();
                    break;
                case StepOperationType.CameraFollow:
                    CameraFollowGUI();
                    break;
                case StepOperationType.TextMesh:
                    TextMeshGUI();
                    break;
                case StepOperationType.Prompt:
                    PromptGUI();
                    break;
                case StepOperationType.FSM:
                    FSMGUI();
                    break;
                case StepOperationType.Delay:
                    DelayGUI();
                    break;
                case StepOperationType.ActiveComponent:
                    ActiveComponentGUI();
                    break;
                default:
                    Log.Warning("步骤控制者：[" + OperationType + " 操作] 没有可以执行的 OnEditorGUI 定义！");
                    break;
            }
        }
        private void MoveGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Move to:", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUI.enabled = Target;
            if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(60)))
            {
                Vector3Value = Target.transform.localPosition;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUI.enabled = !BoolValue;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, "Transformation");
            GUILayout.EndHorizontal();
        }
        private void RotateGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotate to:", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUI.enabled = Target;
            if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(60)))
            {
                Vector3Value = Target.transform.localRotation.eulerAngles;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUI.enabled = !BoolValue2;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, "Is Axis Add");
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            BoolValue2 = GUILayout.Toggle(BoolValue2, "Transformation");
            GUILayout.EndHorizontal();
        }
        private void ScaleGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Scale to:", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            GUI.enabled = Target;
            if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(60)))
            {
                Vector3Value = Target.transform.localScale;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUI.enabled = !BoolValue;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, "Transformation");
            GUILayout.EndHorizontal();
        }
        private void ColorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Color to:", GUILayout.Width(60));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ColorValue = EditorGUILayout.ColorField(ColorValue, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(120));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, "Act Renderer");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BoolValue2 = GUILayout.Toggle(BoolValue2, "Act Graphic");
            GUILayout.EndHorizontal();
        }
        private void ActiveGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Set Active:", GUILayout.Width(80));
            if (GUILayout.Button(BoolValue ? "true" : "false", "MiniPopup", GUILayout.Width(100)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("true"), BoolValue, () =>
                {
                    BoolValue = true;
                });
                gm.AddItem(new GUIContent("false"), !BoolValue, () =>
                {
                    BoolValue = false;
                });
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
        }
        private void ActionGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = Target;
            GUILayout.Label("Action:", GUILayout.Width(50));
            if (GUILayout.Button(StringValue, "MiniPopup", GUILayout.Width(130)))
            {
                GenericMenu gm = new GenericMenu();
                Component[] monos = Target.GetComponents<Component>();
                foreach (Component mono in monos)
                {
                    MethodInfo[] mis = mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MethodInfo mi in mis)
                    {
                        if (mi.Name.Contains("set_") || mi.Name.Contains("get_") || mi.GetParameters().Length > 0 || mi.ReturnType.Name != "Void")
                        {
                            continue;
                        }
                        gm.AddItem(new GUIContent(mono.GetType().Name + "/" + mi.Name + "()"), StringValue == mi.Name, () =>
                        {
                            StringValue = mi.Name;
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        private void ActionArgsGUI()
        {
            GUI.enabled = Target;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Action:", GUILayout.Width(50));
            if (GUILayout.Button(StringValue, "MiniPopup", GUILayout.Width(130)))
            {
                GenericMenu gm = new GenericMenu();
                Component[] monos = Target.GetComponents<Component>();
                foreach (Component mono in monos)
                {
                    MethodInfo[] mis = mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MethodInfo mi in mis)
                    {
                        ParameterInfo[] pis = mi.GetParameters();
                        if (mi.Name.Contains("set_") || mi.Name.Contains("get_") || pis.Length != 1 || pis[0].ParameterType.Name != "String" || mi.ReturnType.Name != "Void")
                        {
                            continue;
                        }
                        gm.AddItem(new GUIContent(mono.GetType().Name + "/" + mi.Name + "(string)"), StringValue == mi.Name, () =>
                        {
                            StringValue = mi.Name;
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Args:", GUILayout.Width(50));
            StringValue2 = EditorGUILayout.TextField(StringValue2);
            GUILayout.EndHorizontal();

            GUI.enabled = true;
        }
        private void CameraFollowGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Look point:");
            GUILayout.FlexibleSpace();
            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button("Get", "Minibuttonleft", GUILayout.Width(40)))
            {
                if (Main.m_Controller)
                {
                    Vector3Value = Main.m_Controller.LookPoint;
                }
            }
            GUI.enabled = true;
            if (GUILayout.Button("Paste", "Minibuttonright", GUILayout.Width(60)))
            {
                string[] vector3 = GUIUtility.systemCopyBuffer.Split(',');
                if (vector3.Length == 3)
                {
                    float x, y, z;
                    vector3[0] = vector3[0].Replace("f", "");
                    vector3[1] = vector3[1].Replace("f", "");
                    vector3[2] = vector3[2].Replace("f", "");
                    if (float.TryParse(vector3[0], out x) && float.TryParse(vector3[1], out y) && float.TryParse(vector3[2], out z))
                    {
                        Vector3Value = new Vector3(x, y, z);
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Look angle:");
            GUILayout.FlexibleSpace();
            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button("Get", "Minibuttonleft", GUILayout.Width(40)))
            {
                if (Main.m_Controller)
                {
                    Vector2Value = new Vector3(Main.m_Controller.LookAngle.x, Main.m_Controller.LookAngle.y);
                    FloatValue = Main.m_Controller.LookAngle.z;
                }
            }
            GUI.enabled = true;
            if (GUILayout.Button("Paste", "Minibuttonright", GUILayout.Width(60)))
            {
                string[] vector3 = GUIUtility.systemCopyBuffer.Split(',');
                if (vector3.Length == 3)
                {
                    float x, y, z;
                    vector3[0] = vector3[0].Replace("f", "");
                    vector3[1] = vector3[1].Replace("f", "");
                    vector3[2] = vector3[2].Replace("f", "");
                    if (float.TryParse(vector3[0], out x) && float.TryParse(vector3[1], out y) && float.TryParse(vector3[2], out z))
                    {
                        Vector2Value = new Vector3(x, y);
                        FloatValue = z;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector2Value = EditorGUILayout.Vector2Field("", Vector2Value, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Look distance:");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatValue = EditorGUILayout.FloatField("", FloatValue, GUILayout.Width(180));
            GUILayout.EndHorizontal();
        }
        private void TextMeshGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("TextMesh to:");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            StringValue = EditorGUILayout.TextField(StringValue);
            GUILayout.EndHorizontal();
        }
        private void PromptGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Prompt:");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            StringValue = EditorGUILayout.TextField(StringValue);
            GUILayout.EndHorizontal();
        }
        private void FSMGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("FSM switch state to:");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = Target;
            GUILayout.Label("State:", GUILayout.Width(50));
            if (GUILayout.Button(StringValue, "MiniPopup", GUILayout.Width(130)))
            {
                FSM fsm = Target.GetComponent<FSM>();
                if (fsm)
                {
                    GenericMenu gm = new GenericMenu();
                    for (int i = 0; i < fsm.States.Count; i++)
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(fsm.States[j]), StringValue == fsm.States[j], () =>
                        {
                            StringValue = fsm.States[j];
                        });
                    }
                    gm.ShowAsContext();
                }
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        private void DelayGUI()
        {
            if (Instant)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.red;
                GUILayout.Label("Delay time is invalid!");
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.cyan;
                GUILayout.Label("Delay time " + ElapseTime + " second!");
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }
        }
        private void ActiveComponentGUI()
        {
            GUI.enabled = Target;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Component:", GUILayout.Width(80));
            if (GUILayout.Button(StringValue, "MiniPopup", GUILayout.Width(100)))
            {
                GenericMenu gm = new GenericMenu();
                Component[] monos = Target.GetComponents<Component>();
                foreach (Component mono in monos)
                {
                    if (mono is Behaviour || mono is Collider || mono is Renderer)
                    {
                        string type = mono.GetType().FullName;
                        gm.AddItem(new GUIContent(type), StringValue == type, () =>
                        {
                            StringValue = type;
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Set Active:", GUILayout.Width(80));
            if (GUILayout.Button(BoolValue ? "true" : "false", "MiniPopup", GUILayout.Width(100)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("true"), BoolValue, () =>
                {
                    BoolValue = true;
                });
                gm.AddItem(new GUIContent("false"), !BoolValue, () =>
                {
                    BoolValue = false;
                });
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }
#endif
    }

    /// <summary>
    /// 步骤操作类型
    /// </summary>
    public enum StepOperationType
    {
        /// <summary>
        /// 移动
        /// </summary>
        Move,
        /// <summary>
        /// 旋转
        /// </summary>
        Rotate,
        /// <summary>
        /// 缩放
        /// </summary>
        Scale,
        /// <summary>
        /// 颜色改变
        /// </summary>
        Color,
        /// <summary>
        /// 延时
        /// </summary>
        Delay,
        /// <summary>
        /// 激活与隐藏
        /// </summary>
        Active,
        /// <summary>
        /// 执行自定义方法 void Action()
        /// </summary>
        Action,
        /// <summary>
        /// 执行自定义方法 void Action(string args)
        /// </summary>
        ActionArgs,
        /// <summary>
        /// FSM切换状态
        /// </summary>
        FSM,
        /// <summary>
        /// TextMesh文本改变
        /// </summary>
        TextMesh,
        /// <summary>
        /// 提示
        /// </summary>
        Prompt,
        /// <summary>
        /// 摄像机跟随
        /// </summary>
        CameraFollow,
        /// <summary>
        /// 激活与隐藏组件
        /// </summary>
        ActiveComponent
    }
}