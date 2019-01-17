using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        /// 显示提示事件
        /// </summary>
        public event Action<string> ShowPromptEvent;
        /// <summary>
        /// 流程开始事件
        /// </summary>
        public event Action BeginEvent;
        /// <summary>
        /// 跳过步骤完成事件
        /// </summary>
        public event Action SkipStepDoneEvent;
        /// <summary>
        /// 鼠标点击触发的步骤，点击了错误的目标事件
        /// </summary>
        public event Action ClickWrongTarget;
        /// <summary>
        /// 流程结束事件
        /// </summary>
        public event Action EndEvent;

        private Dictionary<string, StepTarget> _targets = new Dictionary<string, StepTarget>();
        private int _currentStep = -1;
        private StepContent _currentContent;
        private StepTarget _currentTarget;
        private StepHelper _currentHelper;
        //跳过的目标步骤
        private int _skipIndex = 0;
        //步骤控制者工作中
        private bool _ongoing = false;
        //步骤执行中
        private bool _running = false;
        //本帧中是否触发步骤的执行操作
        private bool _execute = false;
        
        public override void Initialization()
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

                for (int i = 0; i < ContentAsset.Content.Count; i++)
                {
                    StepContent content = ContentAsset.Content[i];
                    if (_targets.ContainsKey(content.TargetGUID))
                    {
                        content.Target = _targets[content.TargetGUID].gameObject;
                    }
                    else
                    {
                        GlobalTools.LogError("【步骤：" + (i + 1) + "】" + content.Name + "，目标没有找到，目标路径：" + content.TargetPath);
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
                            GlobalTools.LogError("【步骤：" + (i + 1) + "】" + "操作：" + operation.Name + "，目标没有找到，目标路径：" + operation.TargetPath);
                        }
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
                GlobalTools.LogWarning("步骤控制器丢失了步骤资源 Content Asset！");
            }
        }

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
                    switch (ContentAsset.Content[_currentStep].Trigger)
                    {
                        //鼠标点击触发
                        case StepTrigger.MouseClick:
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (MouseRay.Instance.TargetObj)
                                {
                                    if (MouseRay.Instance.TargetObj == _currentContent.Target)
                                    {
                                        _execute = true;
                                    }
                                    else
                                    {
                                        if (MouseRay.Instance.TargetObj.GetComponent<StepTarget>() && ClickWrongTarget != null)
                                            ClickWrongTarget();
                                    }
                                }
                            }
                            break;
                        //状态改变触发
                        case StepTrigger.StateChange:
                            if (_currentTarget.State == _currentContent.TriggerState)
                            {
                                _execute = true;
                            }
                            break;
                        //自动触发
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
                return ContentAsset.Content;
            }
        }
        /// <summary>
        /// 步骤数量
        /// </summary>
        public int StepCout
        {
            get
            {
                return ContentAsset.Content.Count;
            }
        }
        
        /// <summary>
        /// 开始整个流程
        /// </summary>
        public void Begin()
        {
            if (!ContentAsset)
            {
                return;
            }

            _currentStep = 0;
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
        /// 跳过到指定步骤
        /// </summary>
        public bool SkipStep(int index)
        {
            if (_ongoing && !_running)
            {
                if (index <= _currentStep || index > ContentAsset.Content.Count - 1)
                    return false;

                StartCoroutine(SkipSteps(index));
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
        /// 展示步骤提示
        /// </summary>
        public void ShowPrompt(string content)
        {
            if (ShowPromptEvent != null)
                ShowPromptEvent(content);
        }
        
        private void BeginCurrentStep()
        {
            _running = false;
            _currentContent = ContentAsset.Content[_currentStep];
            _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

            //状态改变触发类型的步骤，自动重置状态
            if (_currentContent.Trigger == StepTrigger.StateChange)
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
                    _currentHelper.OnInit();
                }
                else
                {
                    GlobalTools.LogError("【步骤：" + (_currentStep + 1) + "】的助手 " + _currentContent.Helper + " 丢失！");
                }
            }

            MousePosition.Instance.SetPosition(_currentTarget.transform.position + _currentContent.ViewOffset, true);
            MouseRotation.Instance.SetAngle(_currentContent.BestView, true);

            if (BeginStepEvent != null)
                BeginStepEvent(_currentContent);
        }
        private void ExecuteCurrentStep()
        {
            _running = true;
            _currentContent.Execute(this);

            //销毁步骤助手
            if (_currentHelper != null)
            {
                _currentHelper.OnTermination();
                _currentHelper = null;
            }

            if (ExecuteStepEvent != null)
                ExecuteStepEvent(_currentContent);
        }
        private IEnumerator SkipSteps(int index)
        {
            _running = true;
            _skipIndex = index;

            while (_currentStep < _skipIndex)
            {
                _currentContent = ContentAsset.Content[_currentStep];
                _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

                MousePosition.Instance.SetPosition(_currentTarget.transform.position + _currentContent.ViewOffset, false);
                MouseRotation.Instance.SetAngle(_currentContent.BestView, true);

                if (SkipStepEvent != null)
                    SkipStepEvent(_currentContent);

                //创建步骤助手
                if (_currentHelper == null && _currentContent.Helper != "<None>")
                {
                    Type type = Type.GetType(_currentContent.Helper);
                    if (type != null)
                    {
                        _currentHelper = Activator.CreateInstance(type) as StepHelper;
                        _currentHelper.Target = _currentTarget;
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
            if (_currentStep < ContentAsset.Content.Count - 1)
            {
                _currentStep += 1;
                BeginCurrentStep();
            }
            else
            {
                End();
            }
        }
    }
}