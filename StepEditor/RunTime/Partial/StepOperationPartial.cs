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
        public Vector3 Vector3Result = Vector3.zero;
        public Vector2 Vector2Result = Vector2.zero;
        public Color ColorResult = Color.white;
        public int IntResult = 0;
        public float FloatResult = 0;
        public string StringResult = "<None>";
        public bool BoolResult = false;
        public Ease AnimationEase = Ease.Linear;

        /// <summary>
        /// 克隆
        /// </summary>
        public StepOperation Clone()
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

            operation.Vector3Result = Vector3Result;
            operation.Vector2Result = Vector2Result;
            operation.ColorResult = ColorResult;
            operation.IntResult = IntResult;
            operation.FloatResult = FloatResult;
            operation.StringResult = StringResult;
            operation.BoolResult = BoolResult;
            operation.AnimationEase = AnimationEase;

            return operation;
        }

        public void Execute()
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
                    GlobalTools.LogWarning("[" + OperationType + "] 没有可以执行的 Execute 定义！");
                    break;
            }
        }
        private void MoveExecute()
        {
            if (BoolResult)
            {
                Target.transform.localPosition = Vector3Result;
            }
            else
            {
                Target.transform.DOLocalMove(Vector3Result, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void RotateExecute()
        {
            if (BoolResult)
            {
                Target.transform.DOLocalRotate(Vector3Result, ElapseTime, RotateMode.LocalAxisAdd).SetEase(AnimationEase);
            }
            else
            {
                Target.transform.DOLocalRotate(Vector3Result, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void ScaleExecute()
        {
            if (BoolResult)
            {
                Target.transform.localScale = Vector3Result;
            }
            else
            {
                Target.transform.DOScale(Vector3Result, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void ColorExecute()
        {
            if (BoolResult)
            {
                if (!Target.GetComponent<Renderer>())
                {
                    GlobalTools.LogError("目标 " + Target + " 丢失组件Renderer！无法播放颜色改变动画！");
                    return;
                }
                Target.GetComponent<Renderer>().material.DOColor(ColorResult, ElapseTime).SetEase(AnimationEase);
            }
            else
            {
                if (!Target.GetComponent<Graphic>())
                {
                    GlobalTools.LogError("目标 " + Target + " 丢失组件Graphic！无法播放颜色改变动画！");
                    return;
                }
                Target.GetComponent<Graphic>().DOColor(ColorResult, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void ActiveExecute()
        {
            Target.SetActive(BoolResult);
        }
        private void ActionExecute()
        {
            if (StringResult != "<None>")
            {
                Target.SendMessage(StringResult);
            }
        }
        private void CameraFollowExecute()
        {
            MousePosition.Instance.SetPosition(Vector3Result, true);
            MouseRotation.Instance.SetAngle(Vector2Result, FloatResult, true);
        }
        private void TextMeshExecute()
        {
            if (!Target.GetComponent<TextMesh>())
            {
                GlobalTools.LogError("目标 " + Target + " 丢失组件TextMesh！无法设置TextMesh文本！");
                return;
            }
            Target.GetComponent<TextMesh>().text = StringResult;
        }
        private void PromptExecute()
        {
            Main.m_StepMaster.ShowPrompt(StringResult);
        }
        private void FSMExecute()
        {
            if (StringResult != "<None>")
            {
                Target.GetComponent<FSM>().SwitchState(Type.GetType(StringResult));
            }
        }
        private void DelayExecute()
        {
        }
        private void ActiveComponentExecute()
        {
            Type type = Type.GetType(StringResult.Contains("UnityEngine") ? StringResult + ",UnityEngine" : StringResult);
            if (type != null)
            {
                Component component = Target.GetComponent(type);
                Behaviour behaviour = component as Behaviour;
                Collider collider = component as Collider;
                Renderer renderer = component as Renderer;
                if (behaviour) behaviour.enabled = BoolResult;
                else if (collider) collider.enabled = BoolResult;
                else if (renderer) renderer.enabled = BoolResult;
            }
            else
            {
                GlobalTools.LogError("未获取到类型 " + StringResult + "！");
            }
        }

        public void Skip()
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
                    GlobalTools.LogWarning("[" + OperationType + "] 没有可以执行的 Skip 定义！");
                    break;
            }
        }
        private void MoveSkip()
        {
            if (BoolResult)
            {
                Target.transform.localPosition = Vector3Result;
            }
            else
            {
                Target.transform.DOLocalMove(Vector3Result, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
        }
        private void RotateSkip()
        {
            if (BoolResult)
            {
                Target.transform.DOLocalRotate(Vector3Result, ElapseTime / StepMaster.SkipMultiple, RotateMode.LocalAxisAdd).SetEase(AnimationEase);
            }
            else
            {
                Target.transform.DOLocalRotate(Vector3Result, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
        }
        private void ScaleSkip()
        {
            if (BoolResult)
            {
                Target.transform.localScale = Vector3Result;
            }
            else
            {
                Target.transform.DOScale(Vector3Result, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
        }
        private void ColorSkip()
        {
            if (BoolResult)
            {
                if (!Target.GetComponent<Renderer>())
                {
                    GlobalTools.LogError("目标 " + Target + " 丢失组件Renderer！无法播放颜色改变动画！");
                    return;
                }
                Target.GetComponent<Renderer>().material.DOColor(ColorResult, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
            else
            {
                if (!Target.GetComponent<Graphic>())
                {
                    GlobalTools.LogError("目标 " + Target + " 丢失组件Graphic！无法播放颜色改变动画！");
                    return;
                }
                Target.GetComponent<Graphic>().DOColor(ColorResult, ElapseTime / StepMaster.SkipMultiple).SetEase(AnimationEase);
            }
        }
        private void ActiveSkip()
        {
            Target.SetActive(BoolResult);
        }
        private void ActionSkip()
        {
            if (StringResult != "<None>")
            {
                Target.SendMessage(StringResult);
            }
        }
        private void CameraFollowSkip()
        {
            MousePosition.Instance.SetPosition(Vector3Result, false);
            MouseRotation.Instance.SetAngle(Vector2Result, FloatResult, true);
        }
        private void TextMeshSkip()
        {
            if (!Target.GetComponent<TextMesh>())
            {
                GlobalTools.LogError("目标 " + Target + " 丢失组件TextMesh！无法设置TextMesh文本！");
                return;
            }
            Target.GetComponent<TextMesh>().text = StringResult;
        }
        private void PromptSkip()
        {
            Main.m_StepMaster.ShowPrompt(StringResult);
        }
        private void FSMSkip()
        {
            if (StringResult != "<None>")
            {
                Target.GetComponent<FSM>().SwitchState(Type.GetType(StringResult));
            }
        }
        private void DelaySkip()
        {
        }
        private void ActiveComponentSkip()
        {
            Type type = Type.GetType(StringResult.Contains("UnityEngine") ? StringResult + ",UnityEngine" : StringResult);
            if (type != null)
            {
                Component component = Target.GetComponent(type);
                Behaviour behaviour = component as Behaviour;
                Collider collider = component as Collider;
                Renderer renderer = component as Renderer;
                if (behaviour) behaviour.enabled = BoolResult;
                else if (collider) collider.enabled = BoolResult;
                else if (renderer) renderer.enabled = BoolResult;
            }
            else
            {
                GlobalTools.LogError("未获取到类型 " + StringResult + "！");
            }
        }

#if UNITY_EDITOR
        public void OnEditorGUI()
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
                    GlobalTools.LogWarning("[" + OperationType + "] 没有可以执行的 OnEditorGUI 定义！");
                    break;
            }
        }
        private void MoveGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Move to:", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(60)))
            {
                Vector3Result = Target.transform.localPosition;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Result = EditorGUILayout.Vector3Field("", Vector3Result, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(120));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BoolResult = GUILayout.Toggle(BoolResult, "Transformation");
            GUILayout.EndHorizontal();
        }
        private void RotateGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Rotate to:", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(60)))
            {
                Vector3Result = Target.transform.localRotation.eulerAngles;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Result = EditorGUILayout.Vector3Field("", Vector3Result, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(120));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BoolResult = GUILayout.Toggle(BoolResult, "Is Axis Add");
            GUILayout.EndHorizontal();
        }
        private void ScaleGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Scale to:", GUILayout.Width(60));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Get", "Minibutton", GUILayout.Width(60)))
            {
                Vector3Result = Target.transform.localScale;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Result = EditorGUILayout.Vector3Field("", Vector3Result, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(120));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BoolResult = GUILayout.Toggle(BoolResult, "Transformation");
            GUILayout.EndHorizontal();
        }
        private void ColorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Color to:", GUILayout.Width(60));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ColorResult = EditorGUILayout.ColorField(ColorResult, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ease:", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(120));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bool isRenderer = BoolResult;
            isRenderer = GUILayout.Toggle(isRenderer, "Renderer");
            if (isRenderer != BoolResult)
            {
                BoolResult = isRenderer;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            bool isGraphic = !BoolResult;
            isGraphic = GUILayout.Toggle(isGraphic, "Graphic");
            if (isGraphic != !BoolResult)
            {
                BoolResult = !isGraphic;
            }
            GUILayout.EndHorizontal();
        }
        private void ActiveGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Set Active:", GUILayout.Width(80));
            if (GUILayout.Button(BoolResult ? "true" : "false", "MiniPopup", GUILayout.Width(100)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("true"), BoolResult, () =>
                {
                    BoolResult = true;
                });
                gm.AddItem(new GUIContent("false"), !BoolResult, () =>
                {
                    BoolResult = false;
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
            if (GUILayout.Button(StringResult, "MiniPopup", GUILayout.Width(130)))
            {
                GenericMenu gm = new GenericMenu();
                Component[] monos = Target.GetComponents<Component>();
                foreach (Component mono in monos)
                {
                    MethodInfo[] mis = mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MethodInfo mi in mis)
                    {
                        if (mi.Name.Contains("set_") || mi.Name.Contains("get_"))
                        {
                            continue;
                        }
                        gm.AddItem(new GUIContent(mono.GetType().Name + "/" + mi.Name), StringResult == mi.Name, () =>
                        {
                            StringResult = mi.Name;
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        private void CameraFollowGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Look target:");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Paste", "Minibutton", GUILayout.Width(60)))
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
                        Vector3Result = new Vector3(x, y, z);
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Result = EditorGUILayout.Vector3Field("", Vector3Result, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Look angle:");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Paste", "Minibutton", GUILayout.Width(60)))
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
                        Vector2Result = new Vector3(x, y);
                        FloatResult = z;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector2Result = EditorGUILayout.Vector2Field("", Vector2Result, GUILayout.Width(180));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Look distance:");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            FloatResult = EditorGUILayout.FloatField("", FloatResult, GUILayout.Width(180));
            GUILayout.EndHorizontal();
        }
        private void TextMeshGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("TextMesh to:");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            StringResult = EditorGUILayout.TextField(StringResult);
            GUILayout.EndHorizontal();
        }
        private void PromptGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Prompt:");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            StringResult = EditorGUILayout.TextField(StringResult);
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
            if (GUILayout.Button(StringResult, "MiniPopup", GUILayout.Width(130)))
            {
                FSM fsm = Target.GetComponent<FSM>();
                if (fsm)
                {
                    GenericMenu gm = new GenericMenu();
                    for (int i = 0; i < fsm.States.Count; i++)
                    {
                        int j = i;
                        gm.AddItem(new GUIContent(fsm.States[j]), StringResult == fsm.States[j], () =>
                        {
                            StringResult = fsm.States[j];
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
                GUILayout.Label("Delay time " + ElapseTime + " second!");
                GUILayout.EndHorizontal();
            }
        }
        private void ActiveComponentGUI()
        {
            GUI.enabled = Target;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Component:", GUILayout.Width(80));
            if (GUILayout.Button(StringResult, "MiniPopup", GUILayout.Width(100)))
            {
                GenericMenu gm = new GenericMenu();
                Component[] monos = Target.GetComponents<Component>();
                foreach (Component mono in monos)
                {
                    if (mono is Behaviour || mono is Collider || mono is Renderer)
                    {
                        string type = mono.GetType().FullName;
                        gm.AddItem(new GUIContent(type), StringResult == type, () =>
                        {
                            StringResult = type;
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Set Active:", GUILayout.Width(80));
            if (GUILayout.Button(BoolResult ? "true" : "false", "MiniPopup", GUILayout.Width(100)))
            {
                GenericMenu gm = new GenericMenu();
                gm.AddItem(new GUIContent("true"), BoolResult, () =>
                {
                    BoolResult = true;
                });
                gm.AddItem(new GUIContent("false"), !BoolResult, () =>
                {
                    BoolResult = false;
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
        /// 激活与隐藏
        /// </summary>
        Active,
        /// <summary>
        /// 执行其他自定义动作
        /// </summary>
        Action,
        /// <summary>
        /// 摄像机跟随
        /// </summary>
        CameraFollow,
        /// <summary>
        /// 颜色改变
        /// </summary>
        Color,
        /// <summary>
        /// TextMesh文本改变
        /// </summary>
        TextMesh,
        /// <summary>
        /// 提示
        /// </summary>
        Prompt,
        /// <summary>
        /// FSM切换状态
        /// </summary>
        FSM,
        /// <summary>
        /// 延时
        /// </summary>
        Delay,
        /// <summary>
        /// 激活与隐藏组件
        /// </summary>
        ActiveComponent
    }
}
