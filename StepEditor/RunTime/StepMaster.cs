using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 步骤控制者
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class StepMaster : ModuleManager
    {
        public static int SkipMultiple = 1;

        public StepContentAsset ContentAsset;
        public Camera MainCamera;

        /// <summary>
        /// 步骤开始事件
        /// </summary>
        public event Action<StepContent> BeginStepEvent;
        /// <summary>
        /// 步骤执行事件
        /// </summary>
        public event Action<StepContent> ExecuteStepEvent;
        /// <summary>
        /// 步骤跳过事件
        /// </summary>
        public event Action<StepContent> SkipStepEvent;
        /// <summary>
        /// 步骤恢复事件
        /// </summary>
        public event Action<StepContent> RestoreStepEvent;
        /// <summary>
        /// 显示提示事件
        /// </summary>
        public event Action<string> ShowPromptEvent;
        /// <summary>
        /// 流程开始事件
        /// </summary>
        public event Action BeginEvent;
        /// <summary>
        /// 连续跳过步骤完成事件
        /// </summary>
        public event Action SkipStepDoneEvent;
        /// <summary>
        /// 点击了错误的步骤目标事件（正确目标：步骤的当前目标、辅助目标）
        /// </summary>
        public event Action ClickWrongTargetEvent;
        /// <summary>
        /// 流程结束事件
        /// </summary>
        public event Action EndEvent;

        private Dictionary<string, StepTarget> _targets = new Dictionary<string, StepTarget>();
        private List<StepContent> _stepContents = new List<StepContent>();
        private int _currentStep = -1;
        private StepContent _currentContent;
        private StepTarget _currentTarget;
        private StepHelper _currentHelper;
        private Button _currentButton;
        //跳过的目标步骤
        private int _skipIndex = 0;
        //步骤控制者工作中
        private bool _ongoing = false;
        //步骤执行中
        private bool _running = false;
        //本帧中是否触发步骤的执行操作
        private bool _execute = false;
        //UGUI按钮点击触发型步骤，当前是否被点击
        private bool _isButtonClick = false;
        
        public override void Refresh()
        {
            if (_ongoing)
            {
                if (!_running)
                {
                    if (_currentHelper != null)
                    {
                        _currentHelper.OnUpdate();
                    }

                    _execute = false;
                    switch (_stepContents[_currentStep].Trigger)
                    {
                        case StepTrigger.MouseClick:
                            if (Main.m_Input.GetButtonDown("MouseLeft"))
                            {
                                if (Main.m_Controller.RayTarget)
                                {
                                    if (Main.m_Controller.RayTargetObj == _currentContent.Target)
                                    {
                                        _execute = true;
                                    }
                                    else
                                    {
                                        if (Main.m_Controller.RayTarget.IsStepTarget)
                                        {
                                            if (_currentHelper != null)
                                            {
                                                if (!_currentHelper.AuxiliaryTarget.Contains(Main.m_Controller.RayTargetObj))
                                                {
                                                    if (ClickWrongTargetEvent != null)
                                                        ClickWrongTargetEvent();
                                                }
                                            }
                                            else
                                            {
                                                if (ClickWrongTargetEvent != null)
                                                    ClickWrongTargetEvent();
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case StepTrigger.ButtonClick:
                            if (Main.m_Input.GetButtonDown("MouseLeft"))
                            {
                                if (Main.m_Controller.RayTarget && Main.m_Controller.RayTarget.IsStepTarget)
                                {
                                    if (Main.m_Controller.RayTargetObj != _currentContent.Target)
                                    {
                                        if (_currentHelper != null)
                                        {
                                            if (!_currentHelper.AuxiliaryTarget.Contains(Main.m_Controller.RayTargetObj))
                                            {
                                                if (ClickWrongTargetEvent != null)
                                                    ClickWrongTargetEvent();
                                            }
                                        }
                                        else
                                        {
                                            if (ClickWrongTargetEvent != null)
                                                ClickWrongTargetEvent();
                                        }
                                    }
                                }
                            }
                            if (_isButtonClick)
                            {
                                _execute = true;
                            }
                            break;
                        case StepTrigger.StateChange:
                            if (Main.m_Input.GetButtonDown("MouseLeft"))
                            {
                                if (Main.m_Controller.RayTarget && Main.m_Controller.RayTarget.IsStepTarget)
                                {
                                    if (Main.m_Controller.RayTargetObj != _currentContent.Target)
                                    {
                                        if (_currentHelper != null)
                                        {
                                            if (!_currentHelper.AuxiliaryTarget.Contains(Main.m_Controller.RayTargetObj))
                                            {
                                                if (ClickWrongTargetEvent != null)
                                                    ClickWrongTargetEvent();
                                            }
                                        }
                                        else
                                        {
                                            if (ClickWrongTargetEvent != null)
                                                ClickWrongTargetEvent();
                                        }
                                    }
                                }
                            }
                            if (_currentTarget.State == StepTargetState.Done)
                            {
                                _execute = true;
                            }
                            break;
                        case StepTrigger.AutoExecute:
                            _execute = true;
                            break;
                        default:
                            break;
                    }

                    if (_execute)
                    {
                        ExecuteCurrentStep();

                        if (_currentContent.Instant)
                        {
                            ChangeNextStep();
                        }
                        else
                        {
                            this.DelayExecute(() =>
                            {
                                ChangeNextStep();
                            }, _currentContent.ElapseTime);
                        }
                    }
                }
            }
        }

        public override void Termination()
        {
            _targets.Clear();
            _stepContents.Clear();
        }

        /// <summary>
        /// 当前步骤索引
        /// </summary>
        public int CurrentStepIndex
        {
            get
            {
                return _currentStep;
            }
        }
        /// <summary>
        /// 当前步骤内容
        /// </summary>
        public StepContent CurrentStepContent
        {
            get
            {
                return _currentContent;
            }
        }
        /// <summary>
        /// 所有步骤
        /// </summary>
        public List<StepContent> AllStep
        {
            get
            {
                return _stepContents;
            }
        }
        /// <summary>
        /// 步骤数量
        /// </summary>
        public int StepCout
        {
            get
            {
                return _stepContents.Count;
            }
        }

        /// <summary>
        /// 重新编译步骤内容
        /// </summary>
        /// <param name="enabledStepIndex">启用的步骤索引列表（当为null时启用所有步骤）</param>
        public void RecompileStepContent(List<int> enabledStepIndex = null)
        {
            if (ContentAsset)
            {
                _targets.Clear();
                GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject rootObj in rootObjs)
                {
                    StepTarget[] targets = rootObj.transform.GetComponentsInChildren<StepTarget>(true);
                    foreach (StepTarget target in targets)
                    {
                        if (!_targets.ContainsKey(target.GUID))
                        {
                            _targets.Add(target.GUID, target);
                        }
                        else
                        {
                            GlobalTools.LogWarning("发现相同GUID的目标！GUID：" + target.GUID + "！\r\n目标物体：" + _targets[target.GUID].transform.FullName() + " 和 " + target.transform.FullName());
                        }
                    }
                }

                _stepContents.Clear();
                //启用所有步骤
                if (enabledStepIndex == null)
                {
                    for (int i = 0; i < ContentAsset.Content.Count; i++)
                    {
                        StepContent content = ContentAsset.Content[i];
                        if (_targets.ContainsKey(content.TargetGUID))
                        {
                            content.Target = _targets[content.TargetGUID].gameObject;
                        }
                        else
                        {
                            GlobalTools.LogError("【步骤：" + (i + 1) + "】【" + content.Name + "】目标没有找到，目标路径：" + content.TargetPath);
                        }

                        for (int j = 0; j < content.Operations.Count; j++)
                        {
                            StepOperation operation = content.Operations[j];
                            if (_targets.ContainsKey(operation.TargetGUID))
                            {
                                operation.Target = _targets[operation.TargetGUID].gameObject;
                            }
                            else
                            {
                                GlobalTools.LogError("【步骤：" + (i + 1) + "】【操作：" + operation.Name + "】目标没有找到，目标路径：" + operation.TargetPath);
                            }
                        }

                        _stepContents.Add(content);
                    }
                }
                //启用 enabledStepIndex 指定的步骤
                else
                {
                    for (int i = 0; i < enabledStepIndex.Count; i++)
                    {
                        StepContent content = ContentAsset.Content[enabledStepIndex[i]];
                        if (_targets.ContainsKey(content.TargetGUID))
                        {
                            content.Target = _targets[content.TargetGUID].gameObject;
                        }
                        else
                        {
                            GlobalTools.LogError("【步骤：" + (i + 1) + "】【" + content.Name + "】目标没有找到，目标路径：" + content.TargetPath);
                        }

                        for (int j = 0; j < content.Operations.Count; j++)
                        {
                            StepOperation operation = content.Operations[j];
                            if (_targets.ContainsKey(operation.TargetGUID))
                            {
                                operation.Target = _targets[operation.TargetGUID].gameObject;
                            }
                            else
                            {
                                GlobalTools.LogError("【步骤：" + (i + 1) + "】【操作：" + operation.Name + "】目标没有找到，目标路径：" + operation.TargetPath);
                            }
                        }

                        _stepContents.Add(content);
                    }
                }

                _currentStep = 0;
                _currentContent = null;
                _currentTarget = null;
                _ongoing = false;
                _running = false;
            }
            else
            {
                GlobalTools.LogWarning("步骤控制器丢失了步骤资源 Step Content Asset！");
            }
        }
        /// <summary>
        /// 开始整个流程
        /// </summary>
        public void Begin(int beginIndex = 0)
        {
            if (!ContentAsset)
            {
                return;
            }

            _currentStep = ((beginIndex < 0 || beginIndex > _stepContents.Count - 1) ? 0 : beginIndex);
            _currentContent = null;
            _currentTarget = null;
            _ongoing = true;
            _running = false;

            if (BeginEvent != null)
                BeginEvent();
            
            BeginCurrentStep();
        }
        /// <summary>
        /// 结束整个流程
        /// </summary>
        public void End()
        {
            _currentContent = null;
            _currentTarget = null;
            _ongoing = false;
            _running = false;

            if (EndEvent != null)
                EndEvent();
        }

        /// <summary>
        /// 跳过当前步骤
        /// </summary>
        public bool SkipCurrentStep()
        {
            if (_ongoing && !_running)
            {
                StartCoroutine(SkipCurrentStepCoroutine());
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 跳过到指定步骤
        /// </summary>
        public bool SkipStep(int index)
        {
            if (_ongoing && !_running)
            {
                if (index <= _currentStep || index > _stepContents.Count - 1)
                    return false;

                StartCoroutine(SkipStepCoroutine(index));
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 停止未完成的跳过
        /// </summary>
        public void StopSkip()
        {
            _skipIndex = _currentStep;
        }

        /// <summary>
        /// 恢复到指定步骤
        /// </summary>
        public bool RestoreStep(int index)
        {
            if (_ongoing && !_running)
            {
                if (index < 0 || index >= _currentStep)
                    return false;

                while (_currentStep >= index)
                {
                    _currentContent = _stepContents[_currentStep];
                    _currentTarget = _currentContent.Target.GetComponent<StepTarget>();
                    
                    if (RestoreStepEvent != null)
                        RestoreStepEvent(_currentContent);
                    
                    //创建步骤助手
                    if (_currentHelper == null && _currentContent.Helper != "<None>")
                    {
                        Type type = Type.GetType(_currentContent.Helper);
                        if (type != null)
                        {
                            _currentHelper = Activator.CreateInstance(type) as StepHelper;
                            _currentHelper.Target = _currentTarget;
                            _currentHelper.Task = StepHelperTask.Restore;
                            _currentHelper.OnInit();
                        }
                        else
                        {
                            GlobalTools.LogError("【步骤：" + (_currentStep + 1) + "】的助手 " + _currentContent.Helper + " 丢失！");
                        }
                    }
                    //助手执行恢复
                    if (_currentHelper != null)
                    {
                        _currentHelper.Task = StepHelperTask.Restore;
                        _currentHelper.OnRestore();
                        _currentHelper.OnTermination();
                        _currentHelper = null;
                    }
                    
                    _currentStep -= 1;
                }

                _currentStep = index;
                BeginCurrentStep();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 展示提示（“提示”节点调用）
        /// </summary>
        public void ShowPrompt(string content)
        {
            if (ShowPromptEvent != null)
                ShowPromptEvent(content);
        }
        /// <summary>
        /// 指引当前步骤
        /// </summary>
        public void Guide()
        {
            if (CurrentStepContent != null)
            {
                GameObject target = CurrentStepContent.Target;
                if (target.GetComponent<Collider>())
                {
                    target.OpenFlashHighLight(Color.red, Color.white);
                }

                Main.m_Controller.TheControlMode = _currentContent.InitialMode;
                Main.m_Controller.SetLookPoint(target.transform.position + CurrentStepContent.ViewOffset, true);
                Main.m_Controller.SetLookAngle(CurrentStepContent.BestView, true);

                if (_currentHelper != null)
                {
                    _currentHelper.OnGuide();
                }
            }
            else
            {
                ShowPrompt("当前不存在指引目标！");
            }
        }
        /// <summary>
        /// 完成当前步骤（只用于“状态改变”型步骤）
        /// </summary>
        public void CompleteCurrentStep()
        {
            if (CurrentStepContent.Trigger == StepTrigger.StateChange)
            {
                _currentTarget.State = StepTargetState.Done;
            }
        }

        private void BeginCurrentStep()
        {
            _running = false;
            _currentContent = _stepContents[_currentStep];
            _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

            //UGUI按钮点击型步骤，注册监听
            if (_currentContent.Trigger == StepTrigger.ButtonClick)
            {
                _isButtonClick = false;
                _currentButton = _currentContent.Target.GetComponent<Button>();
                if (_currentButton)
                {
                    _currentButton.onClick.AddListener(ButtonClickCallback);
                }
                else
                {
                    GlobalTools.LogError("【步骤：" + (_currentStep + 1) + "】的目标丢失Button组件！");
                }
            }
            //状态改变触发类型的步骤，自动重置状态
            else if (_currentContent.Trigger == StepTrigger.StateChange)
            {
                _currentTarget.State = StepTargetState.Normal;
            }

            //创建步骤助手
            if (_currentContent.Helper != "<None>")
            {
                Type type = Type.GetType(_currentContent.Helper);
                if (type != null)
                {
                    _currentHelper = Activator.CreateInstance(type) as StepHelper;
                    _currentHelper.Target = _currentTarget;
                    _currentHelper.Task = StepHelperTask.Execute;
                    _currentHelper.OnInit();
                }
                else
                {
                    GlobalTools.LogError("【步骤：" + (_currentStep + 1) + "】的助手 " + _currentContent.Helper + " 丢失！");
                }
            }

            Main.m_Controller.TheControlMode = _currentContent.InitialMode;
            Main.m_Controller.SetLookPoint(_currentTarget.transform.position + _currentContent.ViewOffset, true);
            Main.m_Controller.SetLookAngle(_currentContent.BestView, true);

            if (BeginStepEvent != null)
                BeginStepEvent(_currentContent);
        }
        private void ExecuteCurrentStep()
        {
            _running = true;
            _currentContent.Execute(this);

            //UGUI按钮点击型步骤，取消按钮注册
            if (_currentButton)
            {
                _currentButton.onClick.RemoveListener(ButtonClickCallback);
                _currentButton = null;
            }

            //销毁步骤助手
            if (_currentHelper != null)
            {
                _currentHelper.OnTermination();
                _currentHelper = null;
            }

            if (ExecuteStepEvent != null)
                ExecuteStepEvent(_currentContent);
        }
        private IEnumerator SkipCurrentStepCoroutine()
        {
            _running = true;

            Main.m_Controller.TheControlMode = _currentContent.InitialMode;
            Main.m_Controller.SetLookPoint(_currentTarget.transform.position + _currentContent.ViewOffset, false);
            Main.m_Controller.SetLookAngle(_currentContent.BestView, true);

            if (SkipStepEvent != null)
                SkipStepEvent(_currentContent);

            //UGUI按钮点击型步骤，自动执行按钮事件
            if (_currentContent.Trigger == StepTrigger.ButtonClick)
            {
                if (_currentButton)
                {
                    _currentButton.onClick.Invoke();
                }
                else
                {
                    _currentButton = _currentContent.Target.GetComponent<Button>();
                    if (_currentButton)
                    {
                        _currentButton.onClick.Invoke();
                    }
                    else
                    {
                        GlobalTools.LogError("【步骤：" + (_currentStep + 1) + "】的目标丢失Button组件！");
                    }
                }
                _currentButton = null;
            }

            //创建步骤助手
            if (_currentHelper == null && _currentContent.Helper != "<None>")
            {
                Type type = Type.GetType(_currentContent.Helper);
                if (type != null)
                {
                    _currentHelper = Activator.CreateInstance(type) as StepHelper;
                    _currentHelper.Target = _currentTarget;
                    _currentHelper.Task = StepHelperTask.Skip;
                    _currentHelper.OnInit();
                }
                else
                {
                    GlobalTools.LogError("【步骤：" + (_currentStep + 1) + "】的助手 " + _currentContent.Helper + " 丢失！");
                }
            }
            //助手执行跳过，等待生命周期结束后销毁助手
            if (_currentHelper != null)
            {
                _currentHelper.Task = StepHelperTask.Skip;
                _currentHelper.OnSkip();
                yield return new WaitForSeconds(_currentHelper.SkipLifeTime / SkipMultiple);
                _currentHelper.OnTermination();
                _currentHelper = null;
            }

            _currentContent.Skip(this);

            yield return new WaitForSeconds(_currentContent.ElapseTime / SkipMultiple);

            ChangeNextStep();
        }
        private IEnumerator SkipStepCoroutine(int index)
        {
            _running = true;
            _skipIndex = index;

            while (_currentStep < _skipIndex)
            {
                _currentContent = _stepContents[_currentStep];
                _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

                Main.m_Controller.TheControlMode = _currentContent.InitialMode;
                Main.m_Controller.SetLookPoint(_currentTarget.transform.position + _currentContent.ViewOffset, false);
                Main.m_Controller.SetLookAngle(_currentContent.BestView, true);

                if (SkipStepEvent != null)
                    SkipStepEvent(_currentContent);

                //UGUI按钮点击型步骤，自动执行按钮事件
                if (_currentContent.Trigger == StepTrigger.ButtonClick)
                {
                    if (_currentButton)
                    {
                        _currentButton.onClick.Invoke();
                    }
                    else
                    {
                        _currentButton = _currentContent.Target.GetComponent<Button>();
                        if (_currentButton)
                        {
                            _currentButton.onClick.Invoke();
                        }
                        else
                        {
                            GlobalTools.LogError("【步骤：" + (_currentStep + 1) + "】的目标丢失Button组件！");
                        }
                    }
                    _currentButton = null;
                }

                //创建步骤助手
                if (_currentHelper == null && _currentContent.Helper != "<None>")
                {
                    Type type = Type.GetType(_currentContent.Helper);
                    if (type != null)
                    {
                        _currentHelper = Activator.CreateInstance(type) as StepHelper;
                        _currentHelper.Target = _currentTarget;
                        _currentHelper.Task = StepHelperTask.Skip;
                        _currentHelper.OnInit();
                    }
                    else
                    {
                        GlobalTools.LogError("【步骤：" + (_currentStep + 1) + "】的助手 " + _currentContent.Helper + " 丢失！");
                    }
                }
                //助手执行跳过，等待生命周期结束后销毁助手
                if (_currentHelper != null)
                {
                    _currentHelper.Task = StepHelperTask.Skip;
                    _currentHelper.OnSkip();
                    yield return new WaitForSeconds(_currentHelper.SkipLifeTime / SkipMultiple);
                    _currentHelper.OnTermination();
                    _currentHelper = null;
                }

                _currentContent.Skip(this);
                
                yield return new WaitForSeconds(_currentContent.ElapseTime / SkipMultiple);

                _currentStep += 1;
            }

            if (SkipStepDoneEvent != null)
                SkipStepDoneEvent();

            BeginCurrentStep();
        }
        private void ChangeNextStep()
        {
            if (_currentStep < _stepContents.Count - 1)
            {
                _currentStep += 1;
                BeginCurrentStep();
            }
            else
            {
                End();
            }
        }

        /// <summary>
        /// 鼠标点击UGUI按钮触发步骤的回调
        /// </summary>
        private void ButtonClickCallback()
        {
            _isButtonClick = true;
        }
    }
}