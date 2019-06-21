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
        /// <summary>
        /// 步骤跳过时的速度
        /// </summary>
        public static int SkipMultiple = 1;

        /// <summary>
        /// 步骤资源
        /// </summary>
        public StepContentAsset ContentAsset;
        /// <summary>
        /// 步骤开始事件【任何一个步骤开始后触发】
        /// </summary>
        public event HTFAction<StepContent, bool> BeginStepEvent;
        /// <summary>
        /// 步骤执行事件【任何一个步骤执行后触发】
        /// </summary>
        public event HTFAction<StepContent, bool> ExecuteStepEvent;
        /// <summary>
        /// 步骤跳过事件【任何一个步骤跳过后触发】
        /// </summary>
        public event HTFAction<StepContent, bool> SkipStepEvent;
        /// <summary>
        /// 步骤恢复事件【任何一个步骤恢复后触发】
        /// </summary>
        public event HTFAction<StepContent, bool> RestoreStepEvent;
        /// <summary>
        /// 显示提示事件
        /// </summary>
        public event HTFAction<string> ShowPromptEvent;
        /// <summary>
        /// 流程开始事件【调用 Begin 开始整个流程后触发】
        /// </summary>
        public event HTFAction BeginEvent;
        /// <summary>
        /// 连续跳过步骤完成事件【执行连续跳过步骤完成后触发】
        /// </summary>
        public event HTFAction SkipStepDoneEvent;
        /// <summary>
        /// 步骤等待执行时，点击了错误的步骤目标事件【正确目标：步骤的当前目标、辅助目标】
        /// </summary>
        public event HTFAction<StepContent> ClickWrongTargetEvent;
        /// <summary>
        /// 流程结束事件【调用 End 结束整个流程或步骤执行完毕后触发】
        /// </summary>
        public event HTFAction EndEvent;

        //所有的 StepTarget
        private Dictionary<string, StepTarget> _targets = new Dictionary<string, StepTarget>();
        //所有的 自定义执行顺序
        private Dictionary<string, string> _customOrder = new Dictionary<string, string>();
        //所有的 步骤内容
        private List<StepContent> _stepContents = new List<StepContent>();
        //所有的 步骤启用标记（步骤ID、启用标记）
        private Dictionary<string, bool> _stepContentEnables = new Dictionary<string, bool>();
        //所有的 步骤内容（步骤ID、步骤内容）
        private Dictionary<string, StepContent> _stepContentIDs = new Dictionary<string, StepContent>();
        //所有的 步骤索引（步骤ID、步骤索引）
        private Dictionary<string, int> _stepContentIndexs = new Dictionary<string, int>();
        private int _currentStep = -1;
        private StepContent _currentContent;
        private StepTarget _currentTarget;
        private StepHelper _currentHelper;
        private Button _currentButton;
        //跳过的目标步骤
        private int _skipIndex = 0;
        //步骤控制者工作中
        private bool _running = false;
        //步骤执行中
        private bool _executing = false;
        //本帧中是否触发步骤的执行操作
        private bool _execute = false;
        //UGUI按钮点击触发型步骤，当前是否被点击
        private bool _isButtonClick = false;

        public override void Refresh()
        {
            base.Refresh();

            if (_running)
            {
                if (!_executing)
                {
                    if (_currentHelper != null)
                    {
                        _currentHelper.OnUpdate();
                    }

                    _execute = false;
                    switch (_currentContent.Trigger)
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
                                                    ClickWrongTargetEvent?.Invoke(_currentContent);
                                                }
                                            }
                                            else
                                            {
                                                ClickWrongTargetEvent?.Invoke(_currentContent);
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case StepTrigger.ButtonClick:
                            if (Main.m_Input.GetButtonDown("MouseLeft"))
                            {
                                if (Main.m_Controller.RayTarget && Main.m_Controller.RayTargetObj != _currentContent.Target && Main.m_Controller.RayTarget.IsStepTarget)
                                {
                                    if (_currentHelper != null)
                                    {
                                        if (!_currentHelper.AuxiliaryTarget.Contains(Main.m_Controller.RayTargetObj))
                                        {
                                            ClickWrongTargetEvent?.Invoke(_currentContent);
                                        }
                                    }
                                    else
                                    {
                                        ClickWrongTargetEvent?.Invoke(_currentContent);
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
                                if (Main.m_Controller.RayTarget && Main.m_Controller.RayTargetObj != _currentContent.Target && Main.m_Controller.RayTarget.IsStepTarget)
                                {
                                    if (_currentHelper != null)
                                    {
                                        if (!_currentHelper.AuxiliaryTarget.Contains(Main.m_Controller.RayTargetObj))
                                        {
                                            ClickWrongTargetEvent?.Invoke(_currentContent);
                                        }
                                    }
                                    else
                                    {
                                        ClickWrongTargetEvent?.Invoke(_currentContent);
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
                            Main.Current.DelayExecute(() =>
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
            base.Termination();

            _targets.Clear();
            _stepContents.Clear();
            _stepContentEnables.Clear();
            _stepContentIDs.Clear();
            _stepContentIndexs.Clear();
            ClearCustomOrder();
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
        /// 步骤是否启用
        /// </summary>
        /// <param name="stepID">步骤ID</param>
        /// <returns>是否启用</returns>
        public bool StepIsEnable(string stepID)
        {
            if (_stepContentEnables.ContainsKey(stepID))
            {
                return _stepContentEnables[stepID];
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 步骤的真实索引
        /// </summary>
        /// <param name="stepID">步骤ID</param>
        /// <returns>真实索引</returns>
        public int StepIndex(string stepID)
        {
            if (_stepContentIndexs.ContainsKey(stepID))
            {
                return _stepContentIndexs[stepID];
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 重新编译步骤内容
        /// </summary>
        /// <param name="prohibitStepID">禁用的步骤ID列表（当为null时启用所有步骤，禁用的步骤会自动跳过）</param>
        public void RecompileStepContent(HashSet<string> prohibitStepID = null)
        {
            if (ContentAsset)
            {
                //搜寻所有目标
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
                            GlobalTools.LogWarning(string.Format("发现相同GUID的目标！GUID：{0}\r\n目标物体：{1} 和 {2}", target.GUID, _targets[target.GUID].transform.FullName(), target.transform.FullName()));
                        }
                    }
                }

                //判断步骤ID是否重复
                _stepContentIDs.Clear();
                for (int i = 0; i < ContentAsset.Content.Count; i++)
                {
                    StepContent content = ContentAsset.Content[i];
                    if (_stepContentIDs.ContainsKey(content.GUID))
                    {
                        GlobalTools.LogError(string.Format("发现相同GUID的步骤！GUID：{0}\r\n步骤：{1} 和 {2}", content.GUID, _stepContentIDs[content.GUID].Name, content.Name));
                    }
                    else
                    {
                        _stepContentIDs.Add(content.GUID, content);
                    }
                }

                _stepContents.Clear();
                _stepContentEnables.Clear();
                _stepContentIndexs.Clear();
                //启用所有步骤
                if (prohibitStepID == null)
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
                            GlobalTools.LogError(string.Format("【步骤：{0}】【{1}】目标没有找到，目标路径：{2}", i, content.Name, content.TargetPath));
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
                                GlobalTools.LogError(string.Format("【步骤：{0}】【操作：{1}】目标没有找到，目标路径：{2}", i, operation.Name, operation.TargetPath));
                            }
                        }

                        _stepContents.Add(content);
                        if (!_stepContentEnables.ContainsKey(content.GUID))
                        {
                            _stepContentEnables.Add(content.GUID, true);
                            _stepContentIndexs.Add(content.GUID, _stepContents.Count - 1);
                        }
                    }
                }
                //禁用 prohibitStepID 指定的步骤
                else
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
                            GlobalTools.LogError(string.Format("【步骤：{0}】【{1}】目标没有找到，目标路径：{2}", i, content.Name, content.TargetPath));
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
                                GlobalTools.LogError(string.Format("【步骤：{0}】【操作：{1}】目标没有找到，目标路径：{2}", i, operation.Name, operation.TargetPath));
                            }
                        }

                        _stepContents.Add(content);
                        if (!_stepContentEnables.ContainsKey(content.GUID))
                        {
                            _stepContentEnables.Add(content.GUID, !prohibitStepID.Contains(ContentAsset.Content[i].GUID));
                            _stepContentIndexs.Add(content.GUID, _stepContents.Count - 1);
                        }
                    }
                }
                
                _currentStep = 0;
                _currentContent = null;
                _currentTarget = null;
                _running = false;
                _executing = false;

                ClearCustomOrder();
            }
            else
            {
                GlobalTools.LogError("步骤控制器丢失了步骤资源 Step Content Asset！");
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
            _running = true;
            _executing = false;

            BeginEvent?.Invoke();

            BeginCurrentStep();
        }
        /// <summary>
        /// 结束整个流程
        /// </summary>
        public void End()
        {
            _currentContent = null;
            _currentTarget = null;
            _running = false;
            _executing = false;

            EndEvent?.Invoke();
        }

        /// <summary>
        /// 跳过当前步骤
        /// </summary>
        /// <returns>跳过成功/失败</returns>
        public bool SkipCurrentStep()
        {
            if (_running && !_executing)
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
        /// <param name="stepID">步骤ID</param>
        /// <returns>跳过成功/失败</returns>
        public bool SkipStep(string stepID)
        {
            if (_running && !_executing)
            {
                if (!_stepContentIndexs.ContainsKey(stepID))
                    return false;

                int index = _stepContentIndexs[stepID];
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
        /// <param name="stepID">步骤ID</param>
        /// <returns>恢复成功/失败</returns>
        public bool RestoreStep(string stepID)
        {
            if (_running && !_executing)
            {
                if (!_stepContentIndexs.ContainsKey(stepID))
                    return false;

                int index = _stepContentIndexs[stepID];
                if (index < 0 || index >= _currentStep)
                    return false;

                while (_currentStep >= index)
                {
                    _currentContent = _stepContents[_currentStep];
                    _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

                    RestoreStepEvent?.Invoke(_currentContent, _stepContentEnables.ContainsKey(_currentContent.GUID) ? _stepContentEnables[_currentContent.GUID] : false);

                    //创建步骤助手
                    if (_currentHelper == null && _currentContent.Helper != "<None>")
                    {
                        Type type = GlobalTools.GetTypeInRunTimeAssemblies(_currentContent.Helper);
                        if (type != null)
                        {
                            _currentHelper = Activator.CreateInstance(type) as StepHelper;
                            _currentHelper.Target = _currentTarget;
                            _currentHelper.Task = StepHelperTask.Restore;
                            _currentHelper.OnInit();
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("【步骤：{0}】的助手 {1} 丢失！", _currentStep + 1, _currentContent.Helper));
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
        /// 展示提示【“提示”节点呼叫】
        /// </summary>
        public void ShowPrompt(string content)
        {
            ShowPromptEvent?.Invoke(content);
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
                    target.OpenFlashHighLight();
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

        /// <summary>
        /// 新增自定义执行顺序（originalStepID 步骤执行完毕后，进入 targetStepID 步骤）
        /// </summary>
        /// <param name="originalStepID">原始步骤ID</param>
        /// <param name="targetStepID">跳跃到的步骤ID</param>
        public void AddCustomOrder(string originalStepID, string targetStepID)
        {
            if (!_customOrder.ContainsKey(originalStepID))
            {
                _customOrder.Add(originalStepID, targetStepID);
            }
        }
        /// <summary>
        /// 删除自定义执行顺序
        /// </summary>
        /// <param name="originalStepID">原始步骤ID</param>
        public void RemoveCustomOrder(string originalStepID)
        {
            if (_customOrder.ContainsKey(originalStepID))
            {
                _customOrder.Remove(originalStepID);
            }
        }
        /// <summary>
        /// 清空所有自定义顺序
        /// </summary>
        public void ClearCustomOrder()
        {
            _customOrder.Clear();
        }

        private void BeginCurrentStep()
        {
            _executing = false;
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
                    GlobalTools.LogError(string.Format("【步骤：{0}】的目标丢失Button组件！", _currentStep + 1));
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
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(_currentContent.Helper);
                if (type != null)
                {
                    _currentHelper = Activator.CreateInstance(type) as StepHelper;
                    _currentHelper.Target = _currentTarget;
                    _currentHelper.Task = StepHelperTask.Execute;
                    _currentHelper.OnInit();
                }
                else
                {
                    GlobalTools.LogError(string.Format("【步骤：{0}】的助手 {1} 丢失！", _currentStep + 1, _currentContent.Helper));
                }
            }

            Main.m_Controller.TheControlMode = _currentContent.InitialMode;
            Main.m_Controller.SetLookPoint(_currentTarget.transform.position + _currentContent.ViewOffset, true);
            Main.m_Controller.SetLookAngle(_currentContent.BestView, true);

            if (_stepContentEnables.ContainsKey(_currentContent.GUID))
            {
                BeginStepEvent?.Invoke(_currentContent, _stepContentEnables[_currentContent.GUID]);
                if (!_stepContentEnables[_currentContent.GUID])
                {
                    StartCoroutine(SkipCurrentStepCoroutine());
                }
            }
            else
            {
                BeginStepEvent?.Invoke(_currentContent, false);
            }
        }
        private void ExecuteCurrentStep()
        {
            _executing = true;
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

            ExecuteStepEvent?.Invoke(_currentContent, _stepContentEnables.ContainsKey(_currentContent.GUID) ? _stepContentEnables[_currentContent.GUID] : false);
        }
        private IEnumerator SkipCurrentStepCoroutine()
        {
            _executing = true;
            _currentContent.Skip(this);

            Main.m_Controller.TheControlMode = _currentContent.InitialMode;
            Main.m_Controller.SetLookPoint(_currentTarget.transform.position + _currentContent.ViewOffset, false);
            Main.m_Controller.SetLookAngle(_currentContent.BestView, true);

            SkipStepEvent?.Invoke(_currentContent, _stepContentEnables.ContainsKey(_currentContent.GUID) ? _stepContentEnables[_currentContent.GUID] : false);

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
                        GlobalTools.LogError(string.Format("【步骤：{0}】的目标丢失Button组件！", _currentStep + 1));
                    }
                }
                _currentButton = null;
            }

            //创建步骤助手
            if (_currentHelper == null && _currentContent.Helper != "<None>")
            {
                Type type = GlobalTools.GetTypeInRunTimeAssemblies(_currentContent.Helper);
                if (type != null)
                {
                    _currentHelper = Activator.CreateInstance(type) as StepHelper;
                    _currentHelper.Target = _currentTarget;
                    _currentHelper.Task = StepHelperTask.Skip;
                    _currentHelper.OnInit();
                }
                else
                {
                    GlobalTools.LogError(string.Format("【步骤：{0}】的助手 {1} 丢失！", _currentStep + 1, _currentContent.Helper));
                }
            }
            //助手执行跳过，等待生命周期结束后销毁助手
            if (_currentHelper != null)
            {
                _currentHelper.Task = StepHelperTask.Skip;
                _currentHelper.OnSkip();
                if (_currentHelper.SkipLifeTime > 0)
                {
                    yield return YieldInstructioner.GetWaitForSeconds(_currentHelper.SkipLifeTime / SkipMultiple);
                }
                _currentHelper.OnTermination();
                _currentHelper = null;
            }
            
            yield return YieldInstructioner.GetWaitForSeconds(_currentContent.ElapseTime / SkipMultiple);

            ChangeNextStep();
        }
        private IEnumerator SkipStepCoroutine(int index)
        {
            _executing = true;
            _skipIndex = index;

            while (_currentStep < _skipIndex)
            {
                _currentContent = _stepContents[_currentStep];
                _currentTarget = _currentContent.Target.GetComponent<StepTarget>();
                _currentContent.Skip(this);

                Main.m_Controller.TheControlMode = _currentContent.InitialMode;
                Main.m_Controller.SetLookPoint(_currentTarget.transform.position + _currentContent.ViewOffset, false);
                Main.m_Controller.SetLookAngle(_currentContent.BestView, true);

                SkipStepEvent?.Invoke(_currentContent, _stepContentEnables.ContainsKey(_currentContent.GUID) ? _stepContentEnables[_currentContent.GUID] : false);

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
                            GlobalTools.LogError(string.Format("【步骤：{0}】的目标丢失Button组件！", _currentStep + 1));
                        }
                    }
                    _currentButton = null;
                }

                //创建步骤助手
                if (_currentHelper == null && _currentContent.Helper != "<None>")
                {
                    Type type = GlobalTools.GetTypeInRunTimeAssemblies(_currentContent.Helper);
                    if (type != null)
                    {
                        _currentHelper = Activator.CreateInstance(type) as StepHelper;
                        _currentHelper.Target = _currentTarget;
                        _currentHelper.Task = StepHelperTask.Skip;
                        _currentHelper.OnInit();
                    }
                    else
                    {
                        GlobalTools.LogError(string.Format("【步骤：{0}】的助手 {1} 丢失！", _currentStep + 1, _currentContent.Helper));
                    }
                }
                //助手执行跳过，等待生命周期结束后销毁助手
                if (_currentHelper != null)
                {
                    _currentHelper.Task = StepHelperTask.Skip;
                    _currentHelper.OnSkip();
                    if (_currentHelper.SkipLifeTime > 0)
                    {
                        yield return YieldInstructioner.GetWaitForSeconds(_currentHelper.SkipLifeTime / SkipMultiple);
                    }
                    _currentHelper.OnTermination();
                    _currentHelper = null;
                }
                
                yield return YieldInstructioner.GetWaitForSeconds(_currentContent.ElapseTime / SkipMultiple);

                _currentStep += 1;
            }

            SkipStepDoneEvent?.Invoke();

            BeginCurrentStep();
        }
        private void ChangeNextStep()
        {
            if (_customOrder.ContainsKey(_currentContent.GUID))
            {
                _currentStep = _stepContentIndexs[_customOrder[_currentContent.GUID]];
                BeginCurrentStep();
            }
            else
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