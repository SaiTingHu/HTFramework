using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System;
using System.Reflection;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
#endif

namespace HT.Framework
{
    public sealed partial class StepOperation
    {
        #region Value
        [SerializeField] internal Vector3 Vector3Value = Vector3.zero;
        [SerializeField] internal Vector2 Vector2Value = Vector2.zero;
        [SerializeField] internal Color ColorValue = Color.white;
        [SerializeField] internal int IntValue = 0;
        [SerializeField] internal float FloatValue = 0;
        [SerializeField] internal string StringValue = "<None>";
        [SerializeField] internal bool BoolValue = false;
        [SerializeField] internal Ease AnimationEase = Ease.Linear;
        #endregion

        #region Value2
        [SerializeField] internal Vector3 Vector3Value2 = Vector3.zero;
        [SerializeField] internal float FloatValue2 = 0;
        [SerializeField] internal string StringValue2 = "<None>";
        [SerializeField] internal bool BoolValue2 = false;
        #endregion

        #region Value3
        [SerializeField] internal Vector3 Vector3Value3 = Vector3.zero;
        #endregion

        #region Execute
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
                case StepOperationType.Transform:
                    TransformExecute();
                    break;
                case StepOperationType.ChangeParent:
                    ChangeParentExecute();
                    break;
                case StepOperationType.PlayTimeline:
                    PlayTimelineExecute();
                    break;
                default:
                    Log.Warning($"步骤控制者：[{OperationType} 操作] 没有可以执行的 Execute 定义！");
                    break;
            }
        }
        private void MoveExecute()
        {
            if (BoolValue)
            {
                Target.transform.DOComplete();
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
                Target.transform.DOComplete();
                if (BoolValue)
                {
                    Target.transform.localRotation = (Target.transform.localRotation.eulerAngles + Vector3Value).ToQuaternion();
                }
                else
                {
                    Target.transform.localRotation = Vector3Value.ToQuaternion();
                }
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
                Target.transform.DOComplete();
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
                Renderer renderer = Target.GetComponent<Renderer>();
                if (!renderer)
                {
                    Log.Error($"步骤控制者：目标 {Target.name} 丢失组件Renderer！无法播放颜色改变动画！");
                    return;
                }
                renderer.material.DOColor(ColorValue, ElapseTime).SetEase(AnimationEase);
            }
            if (BoolValue2)
            {
                Graphic graphic = Target.GetComponent<Graphic>();
                if (!graphic)
                {
                    Log.Error($"步骤控制者：目标 {Target.name} 丢失组件Graphic！无法播放颜色改变动画！");
                    return;
                }
                graphic.DOColor(ColorValue, ElapseTime).SetEase(AnimationEase);
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
            if (FloatValue2 <= 0)
            {
                Main.m_Controller.SetLookPoint(Vector3Value, false);
                Main.m_Controller.SetLookAngle(Vector2Value, FloatValue, true);
            }
            else
            {
                Main.m_Controller.SetLookPoint(Vector3Value, true, FloatValue2);
                Main.m_Controller.SetLookAngle(Vector2Value, FloatValue, true);
            }
        }
        private void TextMeshExecute()
        {
            if (!Target.GetComponent<TextMesh>())
            {
                Log.Error($"步骤控制者：目标 {Target.name} 丢失组件TextMesh！无法设置TextMesh文本！");
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
                Target.GetComponent<FSM>().SwitchState(ReflectionToolkit.GetTypeInRunTimeAssemblies(StringValue, false));
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

                Log.Error($"步骤控制者：未获取到组件类型 {StringValue} ！");
            }
        }
        private void TransformExecute()
        {
            if (BoolValue)
            {
                Target.transform.DOComplete();
                Target.transform.localPosition = Vector3Value;
                Target.transform.localRotation = Vector3Value2.ToQuaternion();
                Target.transform.localScale = Vector3Value3;
            }
            else
            {
                Target.transform.DOLocalMove(Vector3Value, ElapseTime).SetEase(AnimationEase);
                Target.transform.DOLocalRotate(Vector3Value2, ElapseTime).SetEase(AnimationEase);
                Target.transform.DOScale(Vector3Value3, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void ChangeParentExecute()
        {
            StepTarget parent = Main.m_StepMaster.GetTarget(StringValue);
            if (parent != null && parent.gameObject != Target)
            {
                Target.transform.DOComplete();
                Target.transform.SetParent(parent.transform);
            }
        }
        private void PlayTimelineExecute()
        {
            PlayableDirector director = Target.GetComponent<PlayableDirector>();
            if (director && director.playableAsset)
            {
                director.Play();
            }
        }
        #endregion

        #region Skip
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
                case StepOperationType.Transform:
                    TransformSkip();
                    break;
                case StepOperationType.ChangeParent:
                    ChangeParentSkip();
                    break;
                case StepOperationType.PlayTimeline:
                    PlayTimelineSkip();
                    break;
                default:
                    Log.Warning($"步骤控制者：[{OperationType} 操作] 没有可以执行的 Skip 定义！");
                    break;
            }
        }
        private void MoveSkip()
        {
            if (BoolValue)
            {
                Target.transform.DOComplete();
                Target.transform.localPosition = Vector3Value;
            }
            else
            {
                Target.transform.DOLocalMove(Vector3Value, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void RotateSkip()
        {
            if (BoolValue2)
            {
                Target.transform.DOComplete();
                if (BoolValue)
                {
                    Target.transform.localRotation = (Target.transform.localRotation.eulerAngles + Vector3Value).ToQuaternion();
                }
                else
                {
                    Target.transform.localRotation = Vector3Value.ToQuaternion();
                }
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
        private void ScaleSkip()
        {
            if (BoolValue)
            {
                Target.transform.DOComplete();
                Target.transform.localScale = Vector3Value;
            }
            else
            {
                Target.transform.DOScale(Vector3Value, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void ColorSkip()
        {
            if (BoolValue)
            {
                Renderer renderer = Target.GetComponent<Renderer>();
                if (!renderer)
                {
                    Log.Error($"步骤控制者：目标 {Target.name} 丢失组件Renderer！无法播放颜色改变动画！");
                    return;
                }
                renderer.material.DOColor(ColorValue, ElapseTime).SetEase(AnimationEase);
            }
            if (BoolValue2)
            {
                Graphic graphic = Target.GetComponent<Graphic>();
                if (!graphic)
                {
                    Log.Error($"步骤控制者：目标 {Target.name} 丢失组件Graphic！无法播放颜色改变动画！");
                    return;
                }
                graphic.DOColor(ColorValue, ElapseTime).SetEase(AnimationEase);
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
            if (FloatValue2 <= 0)
            {
                Main.m_Controller.SetLookPoint(Vector3Value, false);
                Main.m_Controller.SetLookAngle(Vector2Value, FloatValue, true);
            }
            else
            {
                Main.m_Controller.SetLookPoint(Vector3Value, true, FloatValue2);
                Main.m_Controller.SetLookAngle(Vector2Value, FloatValue, true);
            }
        }
        private void TextMeshSkip()
        {
            if (!Target.GetComponent<TextMesh>())
            {
                Log.Error($"步骤控制者：目标 {Target.name} 丢失组件TextMesh！无法设置TextMesh文本！");
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
                Target.GetComponent<FSM>().SwitchState(ReflectionToolkit.GetTypeInRunTimeAssemblies(StringValue, false));
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
                Log.Error($"步骤控制者：未获取到组件类型 {StringValue} ！");
            }
        }
        private void TransformSkip()
        {
            if (BoolValue)
            {
                Target.transform.DOComplete();
                Target.transform.localPosition = Vector3Value;
                Target.transform.localRotation = Vector3Value2.ToQuaternion();
                Target.transform.localScale = Vector3Value3;
            }
            else
            {
                Target.transform.DOLocalMove(Vector3Value, ElapseTime).SetEase(AnimationEase);
                Target.transform.DOLocalRotate(Vector3Value2, ElapseTime).SetEase(AnimationEase);
                Target.transform.DOScale(Vector3Value3, ElapseTime).SetEase(AnimationEase);
            }
        }
        private void ChangeParentSkip()
        {
            StepTarget parent = Main.m_StepMaster.GetTarget(StringValue);
            if (parent != null && parent.gameObject != Target)
            {
                Target.transform.DOComplete();
                Target.transform.SetParent(parent.transform);
            }
        }
        private void PlayTimelineSkip()
        {
            PlayableDirector director = Target.GetComponent<PlayableDirector>();
            if (director && director.playableAsset)
            {
                director.Play();
            }
        }
        #endregion

        #region SkipImmediate
        internal void SkipImmediate()
        {
            switch (OperationType)
            {
                case StepOperationType.Move:
                    MoveSkipImmediate();
                    break;
                case StepOperationType.Rotate:
                    RotateSkipImmediate();
                    break;
                case StepOperationType.Scale:
                    ScaleSkipImmediate();
                    break;
                case StepOperationType.Color:
                    ColorSkipImmediate();
                    break;
                case StepOperationType.Active:
                    ActiveSkipImmediate();
                    break;
                case StepOperationType.Action:
                    ActionSkipImmediate();
                    break;
                case StepOperationType.ActionArgs:
                    ActionArgsSkipImmediate();
                    break;
                case StepOperationType.CameraFollow:
                    CameraFollowSkipImmediate();
                    break;
                case StepOperationType.TextMesh:
                    TextMeshSkipImmediate();
                    break;
                case StepOperationType.Prompt:
                    PromptSkipImmediate();
                    break;
                case StepOperationType.FSM:
                    FSMSkipImmediate();
                    break;
                case StepOperationType.Delay:
                    DelaySkipImmediate();
                    break;
                case StepOperationType.ActiveComponent:
                    ActiveComponentSkipImmediate();
                    break;
                case StepOperationType.Transform:
                    TransformSkipImmediate();
                    break;
                case StepOperationType.ChangeParent:
                    ChangeParentSkipImmediate();
                    break;
                case StepOperationType.PlayTimeline:
                    PlayTimelineSkipImmediate();
                    break;
                default:
                    Log.Warning($"步骤控制者：[{OperationType} 操作] 没有可以执行的 SkipImmediate 定义！");
                    break;
            }
        }
        private void MoveSkipImmediate()
        {
            Target.transform.localPosition = Vector3Value;
        }
        private void RotateSkipImmediate()
        {
            if (BoolValue)
            {
                Target.transform.localRotation = (Target.transform.localRotation.eulerAngles + Vector3Value).ToQuaternion();
            }
            else
            {
                Target.transform.localRotation = Vector3Value.ToQuaternion();
            }
        }
        private void ScaleSkipImmediate()
        {
            Target.transform.localScale = Vector3Value;
        }
        private void ColorSkipImmediate()
        {
            if (BoolValue)
            {
                Renderer renderer = Target.GetComponent<Renderer>();
                if (!renderer)
                {
                    Log.Error($"步骤控制者：目标 {Target.name} 丢失组件Renderer！无法播放颜色改变动画！");
                    return;
                }
                renderer.material.color = ColorValue;
            }
            if (BoolValue2)
            {
                Graphic graphic = Target.GetComponent<Graphic>();
                if (!graphic)
                {
                    Log.Error($"步骤控制者：目标 {Target.name} 丢失组件Graphic！无法播放颜色改变动画！");
                    return;
                }
                graphic.color = ColorValue;
            }
        }
        private void ActiveSkipImmediate()
        {
            Target.SetActive(BoolValue);
        }
        private void ActionSkipImmediate()
        {
            if (StringValue != "<None>")
            {
                Target.SendMessage(StringValue);
            }
        }
        private void ActionArgsSkipImmediate()
        {
            if (StringValue != "<None>")
            {
                Target.SendMessage(StringValue, StringValue2);
            }
        }
        private void CameraFollowSkipImmediate()
        {
            Main.m_Controller.SetLookPoint(Vector3Value, false);
            Main.m_Controller.SetLookAngle(Vector2Value, FloatValue, false);
        }
        private void TextMeshSkipImmediate()
        {
            if (!Target.GetComponent<TextMesh>())
            {
                Log.Error($"步骤控制者：目标 {Target.name} 丢失组件TextMesh！无法设置TextMesh文本！");
                return;
            }
            Target.GetComponent<TextMesh>().text = StringValue;
        }
        private void PromptSkipImmediate()
        {
            Main.m_StepMaster.ShowPrompt(StringValue);
        }
        private void FSMSkipImmediate()
        {
            if (StringValue != "<None>")
            {
                Target.GetComponent<FSM>().SwitchState(ReflectionToolkit.GetTypeInRunTimeAssemblies(StringValue, false));
            }
        }
        private void DelaySkipImmediate()
        {
        }
        private void ActiveComponentSkipImmediate()
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
                Log.Error($"步骤控制者：未获取到组件类型 {StringValue} ！");
            }
        }
        private void TransformSkipImmediate()
        {
            Target.transform.localPosition = Vector3Value;
            Target.transform.localRotation = Vector3Value2.ToQuaternion();
            Target.transform.localScale = Vector3Value3;
        }
        private void ChangeParentSkipImmediate()
        {
            StepTarget parent = Main.m_StepMaster.GetTarget(StringValue);
            if (parent != null && parent.gameObject != Target)
            {
                Target.transform.SetParent(parent.transform);
            }
        }
        private void PlayTimelineSkipImmediate()
        {
            PlayableDirector director = Target.GetComponent<PlayableDirector>();
            if (director && director.playableAsset)
            {
                director.Play();
                director.time = director.duration;
                director.Stop();
            }
        }
        #endregion

        #region EditorOnly
#if UNITY_EDITOR
        internal GameObject GameObjectValue;

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
            operation.FloatValue2 = FloatValue2;
            operation.StringValue2 = StringValue2;
            operation.BoolValue2 = BoolValue2;

            operation.Vector3Value3 = Vector3Value3;

            return operation;
        }

        internal void OnEditorGUI(HTFFunc<string, string> getWord)
        {
            switch (OperationType)
            {
                case StepOperationType.Move:
                    MoveGUI(getWord);
                    break;
                case StepOperationType.Rotate:
                    RotateGUI(getWord);
                    break;
                case StepOperationType.Scale:
                    ScaleGUI(getWord);
                    break;
                case StepOperationType.Color:
                    ColorGUI(getWord);
                    break;
                case StepOperationType.Active:
                    ActiveGUI(getWord);
                    break;
                case StepOperationType.Action:
                    ActionGUI(getWord);
                    break;
                case StepOperationType.ActionArgs:
                    ActionArgsGUI(getWord);
                    break;
                case StepOperationType.CameraFollow:
                    CameraFollowGUI(getWord);
                    break;
                case StepOperationType.TextMesh:
                    TextMeshGUI(getWord);
                    break;
                case StepOperationType.Prompt:
                    PromptGUI(getWord);
                    break;
                case StepOperationType.FSM:
                    FSMGUI(getWord);
                    break;
                case StepOperationType.Delay:
                    DelayGUI(getWord);
                    break;
                case StepOperationType.ActiveComponent:
                    ActiveComponentGUI(getWord);
                    break;
                case StepOperationType.Transform:
                    TransformGUI(getWord);
                    break;
                case StepOperationType.ChangeParent:
                    ChangeParentGUI(getWord);
                    break;
                case StepOperationType.PlayTimeline:
                    PlayTimelineGUI(getWord);
                    break;
                default:
                    Log.Warning($"步骤控制者：[{OperationType} 操作] 没有可以执行的 OnEditorGUI 定义！");
                    break;
            }
        }
        private void MoveGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Move To") + ":", GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            GUI.enabled = Target;
            if (GUILayout.Button(getWord("Get"), GUILayout.Width(60)))
            {
                Vector3Value = Target.transform.localPosition;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (PreviewTarget) Vector3Value = PreviewTarget.transform.localPosition;
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(185));
            if (PreviewTarget) PreviewTarget.transform.localPosition = Vector3Value;
            GUILayout.EndHorizontal();

            GUI.enabled = !BoolValue;
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Ease") + ":", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(125));
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, getWord("Transformation"));
            GUILayout.EndHorizontal();
        }
        private void RotateGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Rotate To") + ":", GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            GUI.enabled = Target;
            if (GUILayout.Button(getWord("Get"), GUILayout.Width(60)))
            {
                Vector3Value = Target.transform.localRotation.eulerAngles;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (PreviewTarget)
            {
                if (BoolValue)
                {
                    Vector3Value = PreviewTarget.transform.localRotation.eulerAngles - Target.transform.localRotation.eulerAngles;
                }
                else
                {
                    Vector3Value = PreviewTarget.transform.localRotation.eulerAngles;
                }
            }
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(185));
            if (PreviewTarget)
            {
                if (BoolValue)
                {
                    PreviewTarget.transform.localRotation = (Target.transform.localRotation.eulerAngles + Vector3Value).ToQuaternion();
                }
                else
                {
                    PreviewTarget.transform.localRotation = Vector3Value.ToQuaternion();
                }
            }
            GUILayout.EndHorizontal();

            GUI.enabled = !BoolValue2;
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Ease") + ":", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(125));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, getWord("Is Axis Add"));
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            BoolValue2 = GUILayout.Toggle(BoolValue2, getWord("Transformation"));
            GUILayout.EndHorizontal();
        }
        private void ScaleGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Scale To") + ":", GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            GUI.enabled = Target;
            if (GUILayout.Button(getWord("Get"), GUILayout.Width(60)))
            {
                Vector3Value = Target.transform.localScale;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (PreviewTarget) Vector3Value = PreviewTarget.transform.localScale;
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(185));
            if (PreviewTarget) PreviewTarget.transform.localScale = Vector3Value;
            GUILayout.EndHorizontal();

            GUI.enabled = !BoolValue;
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Ease") + ":", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(125));
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, getWord("Transformation"));
            GUILayout.EndHorizontal();
        }
        private void ColorGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Color To") + ":");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (PreviewTarget)
            {
                if (BoolValue)
                {
                    Renderer renderer = PreviewTarget.GetComponent<Renderer>();
                    if (renderer)
                    {
                        ColorValue = renderer.sharedMaterial.color;
                    }
                }
                if (BoolValue2)
                {
                    Graphic graphic = PreviewTarget.GetComponent<Graphic>();
                    if (graphic)
                    {
                        ColorValue = graphic.color;
                    }
                }
            }
            ColorValue = EditorGUILayout.ColorField(ColorValue, GUILayout.Width(185));
            if (PreviewTarget)
            {
                if (BoolValue)
                {
                    Renderer renderer = PreviewTarget.GetComponent<Renderer>();
                    if (renderer)
                    {
                        renderer.sharedMaterial.color = ColorValue;
                    }
                }
                if (BoolValue2)
                {
                    Graphic graphic = PreviewTarget.GetComponent<Graphic>();
                    if (graphic)
                    {
                        graphic.color = ColorValue;
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Ease") + ":", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(125));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, getWord("Act Renderer"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            BoolValue2 = GUILayout.Toggle(BoolValue2, getWord("Act Graphic"));
            GUILayout.EndHorizontal();
        }
        private void ActiveGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Set Active") + ":", GUILayout.Width(80));
            if (GUILayout.Button(BoolValue ? "true" : "false", EditorStyles.popup, GUILayout.Width(105)))
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
        private void ActionGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = Target;
            GUILayout.Label(getWord("Action") + ":", GUILayout.Width(50));
            string value = StringValue == "<None>" ? getWord(StringValue) : StringValue;
            if (GUILayout.Button(value, EditorStyles.popup, GUILayout.Width(135)))
            {
                GenericMenu gm = new GenericMenu();
                Component[] monos = Target.GetComponents<Component>();
                foreach (Component mono in monos)
                {
                    MethodInfo[] mis = mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MethodInfo mi in mis)
                    {
                        if (mi.Name.StartsWith("set_") || mi.Name.StartsWith("get_")
                            || mi.Name.Contains("<") || mi.Name.Contains(">")
                            || mi.GetParameters().Length > 0 || mi.ReturnType.Name != "Void")
                            continue;
                        
                        if (mi.Name == "Awake" || mi.Name == "Start" || mi.Name == "Update"
                            || mi.Name == "OnEnable" || mi.Name == "OnDisable" || mi.Name == "OnDestroy"
                            || mi.Name == "OnGUI" || mi.Name == "Finalize")
                            continue;

                        gm.AddItem(new GUIContent($"{mono.GetType().FullName}/void {mi.Name}()"), StringValue == mi.Name, () =>
                        {
                            StringValue = mi.Name;
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.FlexibleSpace();
            GUI.enabled = Target && StringValue != "<None>";
            if (GUILayout.Button(getWord("Edit"), GUILayout.Width(40)))
            {
                OpenScriptAction(Target, StringValue);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        private void ActionArgsGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = Target;
            GUILayout.Label(getWord("Action") + ":", GUILayout.Width(50));
            string value = StringValue == "<None>" ? getWord(StringValue) : StringValue;
            if (GUILayout.Button(value, EditorStyles.popup, GUILayout.Width(135)))
            {
                GenericMenu gm = new GenericMenu();
                Component[] monos = Target.GetComponents<Component>();
                foreach (Component mono in monos)
                {
                    MethodInfo[] mis = mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MethodInfo mi in mis)
                    {
                        ParameterInfo[] pis = mi.GetParameters();
                        if (mi.Name.StartsWith("set_") || mi.Name.StartsWith("get_")
                            || mi.Name.Contains("<") || mi.Name.Contains(">")
                            || pis.Length != 1 || pis[0].ParameterType != typeof(string) || mi.ReturnType.Name != "Void")
                            continue;

                        if (mi.Name == "SendMessage" || mi.Name == "SendMessageUpwards" || mi.Name == "BroadcastMessage")
                            continue;

                        gm.AddItem(new GUIContent($"{mono.GetType().FullName}/void {mi.Name}(string {pis[0].Name})"), StringValue == mi.Name, () =>
                        {
                            StringValue = mi.Name;
                        });
                    }
                }
                gm.ShowAsContext();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Args") + ":", GUILayout.Width(50));
            StringValue2 = EditorGUILayout.TextField(StringValue2);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.FlexibleSpace();
            GUI.enabled = Target && StringValue != "<None>";
            if (GUILayout.Button(getWord("Edit"), GUILayout.Width(40)))
            {
                OpenScriptActionArgs(Target, StringValue);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        private void CameraFollowGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Look Point") + ":");
            GUILayout.FlexibleSpace();
            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button(getWord("Get"), "Buttonleft"))
            {
                if (Main.m_Controller)
                {
                    Vector3Value = Main.m_Controller.LookPoint;
                }
            }
            GUI.enabled = true;
            if (GUILayout.Button(getWord("Paste"), "Buttonright"))
            {
                string value = GUIUtility.systemCopyBuffer;
                if (value.StartsWith("Vector3("))
                {
                    value = value.Replace("Vector3(", "");
                    value = value.Replace(")", "");

                    string[] vector3 = value.Split(',');
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
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(185));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Look Angle") + ":");
            GUILayout.FlexibleSpace();
            GUI.enabled = EditorApplication.isPlaying;
            if (GUILayout.Button(getWord("Get"), "Buttonleft"))
            {
                if (Main.m_Controller)
                {
                    Vector2Value = new Vector3(Main.m_Controller.LookAngle.x, Main.m_Controller.LookAngle.y);
                    FloatValue = Main.m_Controller.LookAngle.z;
                }
            }
            GUI.enabled = true;
            if (GUILayout.Button(getWord("Paste"), "Buttonright"))
            {
                string value = GUIUtility.systemCopyBuffer;
                if (value.StartsWith("Vector3("))
                {
                    value = value.Replace("Vector3(", "");
                    value = value.Replace(")", "");

                    string[] vector3 = value.Split(',');
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
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Vector2Value = EditorGUILayout.Vector2Field("", Vector2Value, GUILayout.Width(185));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Look Distance") + ":", GUILayout.Width(100));
            FloatValue = EditorGUILayout.FloatField("", FloatValue, GUILayout.Width(85));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Follow Time") + ":", GUILayout.Width(100));
            FloatValue2 = EditorGUILayout.FloatField("", FloatValue2, GUILayout.Width(85));
            GUILayout.EndHorizontal();
        }
        private void TextMeshGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("TextMesh To") + ":");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            StringValue = EditorGUILayout.TextField(StringValue);
            GUILayout.EndHorizontal();
        }
        private void PromptGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Prompt") + ":");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            StringValue = EditorGUILayout.TextField(StringValue);
            GUILayout.EndHorizontal();
        }
        private void FSMGUI(HTFFunc<string, string> getWord)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("FSM Switch State To") + ":");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = Target;
            GUILayout.Label(getWord("State") + ":", GUILayout.Width(50));
            string value = StringValue == "<None>" ? getWord(StringValue) : StringValue;
            if (GUILayout.Button(value, EditorStyles.popup, GUILayout.Width(135)))
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
        private void DelayGUI(HTFFunc<string, string> getWord)
        {
            if (Instant)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.red;
                GUILayout.Label(getWord("Delay Time Is Invalid"));
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.cyan;
                GUILayout.Label($"{getWord("Delay Time")} {ElapseTime} {getWord("Second")}");
                GUI.color = Color.white;
                GUILayout.EndHorizontal();
            }
        }
        private void ActiveComponentGUI(HTFFunc<string, string> getWord)
        {
            GUI.enabled = Target;
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Component") + ":", GUILayout.Width(80));
            string value = StringValue == "<None>" ? getWord(StringValue) : StringValue;
            if (GUILayout.Button(value, EditorStyles.popup, GUILayout.Width(105)))
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
            GUILayout.Label(getWord("Set Active") + ":", GUILayout.Width(80));
            if (GUILayout.Button(BoolValue ? "true" : "false", EditorStyles.popup, GUILayout.Width(105)))
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
        private void TransformGUI(HTFFunc<string, string> getWord)
        {
            if (PreviewTarget)
            {
                Vector3Value = PreviewTarget.transform.localPosition;
                Vector3Value2 = PreviewTarget.transform.localRotation.eulerAngles;
                Vector3Value3 = PreviewTarget.transform.localScale;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("P:", GUILayout.Width(20));
            Vector3Value = EditorGUILayout.Vector3Field("", Vector3Value, GUILayout.Width(165));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("R:", GUILayout.Width(20));
            Vector3Value2 = EditorGUILayout.Vector3Field("", Vector3Value2, GUILayout.Width(165));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("S:", GUILayout.Width(20));
            Vector3Value3 = EditorGUILayout.Vector3Field("", Vector3Value3, GUILayout.Width(165));
            GUILayout.EndHorizontal();

            if (PreviewTarget)
            {
                PreviewTarget.transform.localPosition = Vector3Value;
                PreviewTarget.transform.localRotation = Vector3Value2.ToQuaternion();
                PreviewTarget.transform.localScale = Vector3Value3;
            }

            GUI.enabled = !BoolValue;
            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Ease") + ":", GUILayout.Width(60));
            AnimationEase = (Ease)EditorGUILayout.EnumPopup("", AnimationEase, GUILayout.Width(125));
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            BoolValue = GUILayout.Toggle(BoolValue, getWord("Transformation"));
            GUILayout.FlexibleSpace();
            GUI.enabled = Target;
            if (GUILayout.Button(getWord("Get"), GUILayout.Width(60)))
            {
                Vector3Value = Target.transform.localPosition;
                Vector3Value2 = Target.transform.localRotation.eulerAngles;
                Vector3Value3 = Target.transform.localScale;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
        private void ChangeParentGUI(HTFFunc<string, string> getWord)
        {
            #region 父级目标物体丢失，根据目标GUID重新搜寻
            if (StringValue != "<None>")
            {
                if (!IsParentMatched())
                {
                    PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                    if (prefabStage != null)
                    {
                        GameObjectValue = prefabStage.prefabContentsRoot.FindChildren(StringValue2);
                        if (!IsParentMatched())
                        {
                            GameObjectValue = null;
                            StepTarget[] targets = prefabStage.prefabContentsRoot.GetComponentsInChildren<StepTarget>(true);
                            foreach (StepTarget target in targets)
                            {
                                if (target.GUID == StringValue && !target.GetComponent<StepPreview>())
                                {
                                    GameObjectValue = target.gameObject;
                                    StringValue2 = target.transform.FullName();
                                    StringValue2 = StringValue2.Substring(StringValue2.IndexOf("/") + 1);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        GameObjectValue = GameObject.Find(StringValue2);
                        if (!IsParentMatched())
                        {
                            GameObjectValue = null;
                            StepTarget[] targets = UnityEngine.Object.FindObjectsOfType<StepTarget>(true);
                            foreach (StepTarget target in targets)
                            {
                                if (target.GUID == StringValue && !target.GetComponent<StepPreview>())
                                {
                                    GameObjectValue = target.gameObject;
                                    StringValue2 = target.transform.FullName();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            GUILayout.BeginHorizontal();
            GUILayout.Label(getWord("Parent") + ":", GUILayout.Width(50));
            GUI.color = GameObjectValue ? Color.white : Color.gray;
            GameObject parent = EditorGUILayout.ObjectField(GameObjectValue, typeof(GameObject), true, GUILayout.Width(135)) as GameObject;
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(getWord("GUID") + ":", "Label", GUILayout.Width(45)))
            {
                GenericMenu gm = new GenericMenu();
                if (StringValue == "<None>")
                {
                    gm.AddDisabledItem(new GUIContent(getWord("Copy")));
                }
                else
                {
                    gm.AddItem(new GUIContent(getWord("Copy")), false, () =>
                    {
                        GUIUtility.systemCopyBuffer = StringValue;
                    });
                }
                if (string.IsNullOrEmpty(GUIUtility.systemCopyBuffer))
                {
                    gm.AddDisabledItem(new GUIContent(getWord("Paste")));
                }
                else
                {
                    gm.AddItem(new GUIContent(getWord("Paste")), false, () =>
                    {
                        StringValue = GUIUtility.systemCopyBuffer;
                    });
                }
                gm.ShowAsContext();
            }
            GUILayout.Label(StringValue == "<None>" ? getWord(StringValue) : StringValue, GUILayout.Width(90));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(getWord("Clear"), GUILayout.Width(45)))
            {
                parent = GameObjectValue = null;
                StringValue = "<None>";
                StringValue2 = "<None>";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            #region 父级目标改变
            if (parent != GameObjectValue)
            {
                if (parent)
                {
                    StepTarget target = parent.GetComponent<StepTarget>();
                    if (!target)
                    {
                        target = parent.AddComponent<StepTarget>();
                        EditorUtility.SetDirty(parent);
                    }
                    if (target.GUID == "<None>")
                    {
                        target.GUID = Guid.NewGuid().ToString();
                        EditorUtility.SetDirty(target);
                    }
                    GameObjectValue = parent;
                    StringValue = target.GUID;
                    StringValue2 = parent.transform.FullName();
                }
            }
            #endregion
        }
        private void PlayTimelineGUI(HTFFunc<string, string> getWord)
        {
            PlayableDirector director = null;
            if (Target)
            {
                director = Target.GetComponent<PlayableDirector>();
            }

            if (director && director.playableAsset)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(getWord("Initial Time") + ":", GUILayout.Width(80));
                GUILayout.Label(director.initialTime.ToString("F2") + "s");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(getWord("Duration") + ":", GUILayout.Width(80));
                GUILayout.Label(director.duration.ToString("F2") + "s");
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(getWord("Initial Time") + ":", GUILayout.Width(80));
                GUILayout.Label("-");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(getWord("Duration") + ":", GUILayout.Width(80));
                GUILayout.Label("-");
                GUILayout.EndHorizontal();
            }
        }

        internal void InitPreviewTarget(StepContent stepContent)
        {
            switch (OperationType)
            {
                case StepOperationType.Move:
                    MovePreviewTarget();
                    break;
                case StepOperationType.Rotate:
                    RotatePreviewTarget();
                    break;
                case StepOperationType.Scale:
                    ScalePreviewTarget();
                    break;
                case StepOperationType.Color:
                    ColorPreviewTarget();
                    break;
                case StepOperationType.Active:
                case StepOperationType.Action:
                case StepOperationType.ActionArgs:
                case StepOperationType.CameraFollow:
                case StepOperationType.TextMesh:
                case StepOperationType.Prompt:
                case StepOperationType.FSM:
                case StepOperationType.Delay:
                case StepOperationType.ActiveComponent:
                case StepOperationType.PlayTimeline:
                    break;
                case StepOperationType.Transform:
                    TransformPreviewTarget();
                    break;
                case StepOperationType.ChangeParent:
                    ChangeParentPreviewTarget();
                    break;
                default:
                    Log.Warning($"步骤控制者：[{OperationType} 操作] 没有可以执行的 InitPreviewTarget 定义！");
                    break;
            }
        }
        private void MovePreviewTarget()
        {
            PreviewTarget.transform.localPosition = Vector3Value;
        }
        private void RotatePreviewTarget()
        {
            if (BoolValue)
            {
                PreviewTarget.transform.localRotation = Quaternion.Euler(PreviewTarget.transform.localRotation.eulerAngles + Vector3Value);
            }
            else
            {
                PreviewTarget.transform.localRotation = Vector3Value.ToQuaternion();
            }
        }
        private void ScalePreviewTarget()
        {
            PreviewTarget.transform.localScale = Vector3Value;
        }
        private void ColorPreviewTarget()
        {
            if (BoolValue)
            {
                Renderer renderer = PreviewTarget.GetComponent<Renderer>();
                if (renderer && renderer.sharedMaterial)
                {
                    Material material = Main.Clone(renderer.sharedMaterial);
                    material.hideFlags = HideFlags.HideAndDontSave;
                    material.color = ColorValue;
                    renderer.sharedMaterial = material;
                }
            }
            if (BoolValue2)
            {
                Graphic graphic = PreviewTarget.GetComponent<Graphic>();
                if (graphic)
                {
                    graphic.color = ColorValue;
                }
            }
        }
        private void TransformPreviewTarget()
        {
            PreviewTarget.transform.localPosition = Vector3Value;
            PreviewTarget.transform.localRotation = Vector3Value2.ToQuaternion();
            PreviewTarget.transform.localScale = Vector3Value3;
        }
        private void ChangeParentPreviewTarget()
        {
            if (GameObjectValue)
            {
                PreviewTarget.transform.SetParent(GameObjectValue.transform);
            }
        }
        private bool IsParentMatched()
        {
            if (GameObjectValue == null)
                return false;

            StepTarget target = GameObjectValue.GetComponent<StepTarget>();
            return target && StringValue == target.GUID;
        }

        private void OpenScriptAction(GameObject target, string methodName)
        {
            Component[] monos = target.GetComponents<Component>();
            foreach (Component mono in monos)
            {
                if (mono as MonoBehaviour == null)
                    continue;

                MethodInfo[] mis = mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (MethodInfo mi in mis)
                {
                    if (mi.Name == methodName && mi.ReturnType.Name == "Void" && mi.GetParameters().Length == 0)
                    {
                        string methodInfo = $"void {methodName}()";
                        MonoScript monoScript = MonoScript.FromMonoBehaviour(mono as MonoBehaviour);
                        string path = AssetDatabase.GetAssetPath(monoScript);
                        path = path.Substring(path.IndexOf('/'));
                        path = Application.dataPath + path;
                        if (File.Exists(path))
                        {
                            string[] lines = File.ReadAllLines(path);
                            for (int i = 0; i < lines.Length; i++)
                            {
                                if (lines[i].Contains(methodInfo))
                                {
                                    AssetDatabase.OpenAsset(monoScript, i + 1);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        private void OpenScriptActionArgs(GameObject target, string methodName)
        {
            Component[] monos = target.GetComponents<Component>();
            foreach (Component mono in monos)
            {
                if (mono as MonoBehaviour == null)
                    continue;

                MethodInfo[] mis = mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (MethodInfo mi in mis)
                {
                    ParameterInfo[] pis = mi.GetParameters();
                    if (mi.Name == methodName && mi.ReturnType.Name == "Void" && pis.Length == 1 && pis[0].ParameterType == typeof(string))
                    {
                        string methodInfo = $"void {methodName}(string {pis[0].Name})";
                        MonoScript monoScript = MonoScript.FromMonoBehaviour(mono as MonoBehaviour);
                        string path = AssetDatabase.GetAssetPath(monoScript);
                        path = path.Substring(path.IndexOf('/'));
                        path = Application.dataPath + path;
                        if (File.Exists(path))
                        {
                            string[] lines = File.ReadAllLines(path);
                            for (int i = 0; i < lines.Length; i++)
                            {
                                if (lines[i].Contains(methodInfo))
                                {
                                    AssetDatabase.OpenAsset(monoScript, i + 1);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
#endif
        #endregion
    }

    /// <summary>
    /// 步骤操作类型
    /// </summary>
    public enum StepOperationType
    {
        /// <summary>
        /// 移动
        /// </summary>
        [Remark("移动")]
        Move,
        /// <summary>
        /// 旋转
        /// </summary>
        [Remark("旋转")]
        Rotate,
        /// <summary>
        /// 缩放
        /// </summary>
        [Remark("缩放")]
        Scale,
        /// <summary>
        /// 颜色
        /// </summary>
        [Remark("颜色")]
        Color,
        /// <summary>
        /// 延时
        /// </summary>
        [Remark("延时")]
        Delay,
        /// <summary>
        /// 激活
        /// </summary>
        [Remark("激活")]
        Active,
        /// <summary>
        /// 呼叫方法 void Action()
        /// </summary>
        [Remark("行为")]
        Action,
        /// <summary>
        /// 呼叫方法 void Action(string args)
        /// </summary>
        [Remark("行为（带参数）")]
        ActionArgs,
        /// <summary>
        /// 状态机
        /// </summary>
        [Remark("状态机")]
        FSM,
        /// <summary>
        /// 网格文本
        /// </summary>
        [Remark("网格文本")]
        TextMesh,
        /// <summary>
        /// 提示
        /// </summary>
        [Remark("提示")]
        Prompt,
        /// <summary>
        /// 摄像机跟随
        /// </summary>
        [Remark("摄像机跟随")]
        CameraFollow,
        /// <summary>
        /// 激活组件
        /// </summary>
        [Remark("激活组件")]
        ActiveComponent,
        /// <summary>
        /// 变换（移动、旋转、缩放）
        /// </summary>
        [Remark("变换")]
        Transform,
        /// <summary>
        /// 改变父级
        /// </summary>
        [Remark("改变父级")]
        ChangeParent,
        /// <summary>
        /// 播放时间线
        /// </summary>
        [Remark("播放时间线")]
        PlayTimeline
    }
}