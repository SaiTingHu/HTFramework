﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// 步骤控制器
    /// </summary>
    [InternalModule(HTFrameworkModule.StepMaster)]
    public sealed class StepMaster : InternalModuleBase<IStepMasterHelper>
    {
        /// <summary>
        /// 步骤资源
        /// </summary>
        public StepContentAsset ContentAsset;
        /// <summary>
        /// 指引目标的高亮方式
        /// </summary>
        [SerializeField] internal MouseRay.HighlightingType GuideHighlighting = MouseRay.HighlightingType.Flash;
        /// <summary>
        /// 默认高亮颜色
        /// </summary>
        [SerializeField] internal Color NormalColor = Color.cyan;
        /// <summary>
        /// 闪光高亮颜色1
        /// </summary>
        [SerializeField] internal Color FlashColor1 = Color.red;
        /// <summary>
        /// 闪光高亮颜色2
        /// </summary>
        [SerializeField] internal Color FlashColor2 = Color.white;
        /// <summary>
        /// 轮廓发光强度
        /// </summary>
        [SerializeField] internal float OutlineIntensity = 1;
        /// <summary>
        /// 步骤开始事件【任何一个步骤开始后触发，连续跳过步骤时不会触发】
        /// </summary>
        public event HTFAction<StepContent, bool> BeginStepEvent;
        /// <summary>
        /// 步骤执行事件【任何一个步骤执行后触发，跳过步骤不会触发】
        /// </summary>
        public event HTFAction<StepContent, bool> ExecuteStepEvent;
        /// <summary>
        /// 步骤跳过事件【任何一个步骤跳过后触发】
        /// </summary>
        public event HTFAction<StepContent, bool> SkipStepEvent;
        /// <summary>
        /// 步骤立即跳过事件【任何一个步骤立即跳过后触发】
        /// </summary>
        public event HTFAction<StepContent, bool> SkipStepImmediateEvent;
        /// <summary>
        /// 步骤恢复事件【任何一个步骤恢复后触发】
        /// </summary>
        public event HTFAction<StepContent, bool> RestoreStepEvent;
        /// <summary>
        /// 显示提示事件【由操作连线界面的 Prompt 节点触发】
        /// </summary>
        public event HTFAction<string> ShowPromptEvent;
        /// <summary>
        /// 流程开始事件【调用 Begin 开始整个流程后触发】
        /// </summary>
        public event HTFAction BeginEvent;
        /// <summary>
        /// 连续跳过步骤完成事件【连续跳过步骤完成后触发】
        /// </summary>
        public event HTFAction SkipStepDoneEvent;
        /// <summary>
        /// 步骤等待执行时，点击了错误的步骤目标事件【正确目标包含：步骤的当前目标、步骤助手的辅助目标】
        /// </summary>
        public event HTFAction<StepContent> ClickWrongTargetEvent;
        /// <summary>
        /// 流程结束事件【调用 End 结束整个流程或步骤执行完毕后触发】
        /// </summary>
        public event HTFAction EndEvent;

        //所有的 步骤目标 <步骤目标ID、步骤目标>
        private Dictionary<string, StepTarget> _targets = new Dictionary<string, StepTarget>();
        //所有的 自定义执行顺序 <原始步骤ID、目标步骤ID>
        private Dictionary<string, string> _customOrder = new Dictionary<string, string>();
        //所有的 步骤内容列表
        private List<StepContent> _stepContents = new List<StepContent>();
        //所有的 步骤内容 <步骤ID、步骤内容>
        private Dictionary<string, StepContent> _stepContentIDs = new Dictionary<string, StepContent>();
        //所有的 步骤索引 <步骤ID、步骤索引>
        private Dictionary<string, int> _stepContentIndexs = new Dictionary<string, int>();
        //当前的步骤索引
        private int _currentStepIndex = -1;
        //当前的步骤内容
        private StepContent _currentContent;
        //当前的步骤目标
        private StepTarget _currentTarget;
        //当前的步骤助手
        private StepHelper _currentHelper;
        //当前的步骤按钮
        private Button _currentButton;
        //跳过的目标步骤
        private int _skipTargetIndex = 0;
        //步骤控制者运行中
        private bool _running = false;
        //步骤控制者暂停中
        private bool _pause = false;
        //步骤执行中
        private bool _executing = false;
        //本帧中是否触发步骤的执行操作
        private bool _execute = false;
        //UGUI按钮点击触发型步骤，当前是否被点击
        private bool _isButtonClick = false;
        //等待协程
        private Coroutine _waitCoroutine;
        //暂停时等待
        private WaitUntil _pauseWait;

        /// <summary>
        /// 当前是否运行中
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _running;
            }
        }
        /// <summary>
        /// 暂停步骤控制者
        /// </summary>
        public bool Pause
        {
            get
            {
                return _pause;
            }
            set
            {
                _pause = value;
                if (_pause)
                {
                    StopSkip();
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
                return _currentStepIndex;
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
        /// 当前步骤助手
        /// </summary>
        public StepHelper CurrentStepHelper
        {
            get
            {
                return _currentHelper;
            }
        }
        /// <summary>
        /// 当前步骤的任务类型
        /// </summary>
        public StepHelperTask CurrentTask { get; private set; }
        /// <summary>
        /// 当前所有的步骤，包含启用的和未启用的
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
        public int StepCount
        {
            get
            {
                return _stepContents.Count;
            }
        }

        public override void OnInit()
        {
            base.OnInit();

            _pauseWait = new WaitUntil(() => { return !Pause; });
        }
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (_running)
            {
                if (Pause)
                    return;

                if (_currentContent == null)
                    return;

                if (!_executing)
                {
                    if (_currentHelper != null && _currentHelper.IsEnableUpdate)
                    {
                        _currentHelper.OnUpdate();
                    }

                    _execute = false;
                    switch (_currentContent.Trigger)
                    {
                        case StepTrigger.MouseClick:
                            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
                            {
                                if (Main.m_Controller.RayTarget)
                                {
                                    if (Main.m_Controller.RayTargetObj == _currentTarget.gameObject)
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
                            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
                            {
                                if (Main.m_Controller.RayTarget && Main.m_Controller.RayTargetObj != _currentTarget.gameObject && Main.m_Controller.RayTarget.IsStepTarget)
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
                            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
                            {
                                if (Main.m_Controller.RayTarget && Main.m_Controller.RayTargetObj != _currentTarget.gameObject && Main.m_Controller.RayTarget.IsStepTarget)
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
                        float elapseTime = _currentContent.Instant ? 0 : (_currentHelper != null ? _currentHelper.ElapseTime : _currentContent.ElapseTime);

                        ExecuteCurrentStep();

                        _waitCoroutine = Main.Current.StartCoroutine(WaitCoroutine(ChangeNextStep, elapseTime));
                    }
                }
            }
        }
        public override void OnTerminate()
        {
            base.OnTerminate();

            _targets.Clear();
            _stepContents.Clear();
            _stepContentIDs.Clear();
            _stepContentIndexs.Clear();
            ClearCustomOrder();
        }
        public override void OnPause()
        {
            base.OnPause();

            Pause = true;
        }
        public override void OnResume()
        {
            base.OnResume();

            Pause = false;
        }
        
        /// <summary>
        /// 根据步骤ID获取步骤的启用标记
        /// </summary>
        /// <param name="stepID">步骤ID</param>
        /// <returns>是否启用</returns>
        public bool StepIsEnable(string stepID)
        {
            if (_stepContentIDs.ContainsKey(stepID))
            {
                return _stepContentIDs[stepID].IsEnable && _stepContentIDs[stepID].IsEnableRunTime;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据步骤ID获取步骤的真实索引
        /// </summary>
        /// <param name="stepID">步骤ID</param>
        /// <returns>在步骤列表中的真实索引</returns>
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
        /// 通过ID获取步骤目标
        /// </summary>
        /// <param name="id">步骤目标ID</param>
        /// <returns>步骤目标</returns>
        public StepTarget GetTarget(string id)
        {
            if (_targets.ContainsKey(id))
            {
                return _targets[id];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 通过ID获取步骤内容
        /// </summary>
        /// <param name="id">步骤内容ID</param>
        /// <returns>步骤内容</returns>
        public StepContent GetStepContent(string id)
        {
            if (_stepContentIDs.ContainsKey(id))
            {
                return _stepContentIDs[id];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 重新编译步骤内容，在更改步骤资源 ContentAsset 后，必须重新编译一次才可以开始步骤流程
        /// </summary>
        /// <param name="disableStepIDs">禁用的步骤ID集合（当为null时启用所有步骤，禁用的步骤会自动跳过）</param>
        public void RecompileStepContent(HashSet<string> disableStepIDs = null)
        {
            if (ContentAsset)
            {
                #region 搜寻步骤目标
                //搜寻场景中所有步骤目标
                _targets.Clear();
                List<GameObject> rootObjs = new List<GameObject>();
                List<StepTarget> targetCaches = new List<StepTarget>();
                GlobalTools.GetRootGameObjectsInAllScene(rootObjs);
                foreach (GameObject rootObj in rootObjs)
                {
                    targetCaches.Clear();
                    rootObj.transform.GetComponentsInChildren(true, targetCaches);
                    for (int i = 0; i < targetCaches.Count; i++)
                    {
                        if (!_targets.ContainsKey(targetCaches[i].GUID))
                        {
                            _targets.Add(targetCaches[i].GUID, targetCaches[i]);
                        }
                        else
                        {
                            Log.Warning(string.Format("步骤控制者：发现相同GUID的目标！GUID：{0}\r\n目标物体：{1} 和 {2}", targetCaches[i].GUID, _targets[targetCaches[i].GUID].transform.FullName(), targetCaches[i].transform.FullName()));
                        }
                    }
                }
                #endregion

                #region 判断步骤ID是否重复
                _stepContentIDs.Clear();
                for (int i = 0; i < ContentAsset.Content.Count; i++)
                {
                    StepContent content = ContentAsset.Content[i];
                    if (_stepContentIDs.ContainsKey(content.GUID))
                    {
                        Log.Error(string.Format("步骤控制者：发现相同GUID的步骤！GUID：{0}\r\n步骤：{1} 和 {2}", content.GUID, _stepContentIDs[content.GUID].Name, content.Name));
                    }
                    else
                    {
                        _stepContentIDs.Add(content.GUID, content);
                    }
                }
                #endregion

                #region 生成所有步骤信息
                _stepContents.Clear();
                _stepContentIndexs.Clear();
                for (int i = 0; i < ContentAsset.Content.Count; i++)
                {
                    StepContent content = ContentAsset.Content[i];
                    if (_targets.ContainsKey(content.TargetGUID))
                    {
                        content.Target = _targets[content.TargetGUID].gameObject;
                    }
                    else
                    {
                        Log.Error(string.Format("步骤控制者：【步骤：{0}】【{1}】目标没有找到，目标路径：{2}", i, content.Name, content.TargetPath));
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
                            Log.Error(string.Format("步骤控制者：【步骤：{0}】【操作：{1}】目标没有找到，目标路径：{2}", i, operation.Name, operation.TargetPath));
                        }
                    }

                    content.IsEnableRunTime = (disableStepIDs == null || !disableStepIDs.Contains(content.GUID));
                    _stepContents.Add(content);
                    _stepContentIndexs.Add(content.GUID, _stepContents.Count - 1);
                }

                _currentStepIndex = 0;
                _currentContent = null;
                _currentTarget = null;
                _currentHelper = null;
                _running = false;
                _executing = false;
                Pause = false;

                ClearCustomOrder();
                #endregion
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.StepMaster, "步骤控制者：重新编译步骤失败，步骤控制者丢失了步骤资源 StepContentAsset！");
            }
        }
        /// <summary>
        /// 开始步骤流程
        /// </summary>
        /// <param name="isBeginFirstStep">自动开始第一步</param>
        public void Begin(bool isBeginFirstStep = true)
        {
            if (!ContentAsset || ContentAsset.Content.Count <= 0 || _stepContents.Count <= 0)
            {
                throw new HTFrameworkException(HTFrameworkModule.StepMaster, "步骤控制者：当前无法开始步骤流程，请重新编译步骤内容 RecompileStepContent！");
            }

            _currentStepIndex = 0;
            _currentContent = null;
            _currentTarget = null;
            _currentHelper = null;
            _running = true;
            _executing = false;
            Pause = false;

            BeginEvent?.Invoke();

            if (isBeginFirstStep)
            {
                BeginCurrentStep();
            }
        }
        /// <summary>
        /// 结束步骤流程
        /// </summary>
        public void End()
        {
            DestroyHelper();

            _currentStepIndex = 0;
            _currentContent = null;
            _currentTarget = null;
            _running = false;
            _executing = false;
            Pause = false;

            StopSkip();

            if (_waitCoroutine != null)
            {
                Main.Current.StopCoroutine(_waitCoroutine);
                _waitCoroutine = null;
            }

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
                if (Pause)
                    return false;

                if (_currentHelper != null && !_currentHelper.IsAllowSkip)
                    return false;

                Main.Current.StartCoroutine(SkipCurrentStepCoroutine());
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
                if (Pause)
                    return false;

                if (_currentHelper != null && !_currentHelper.IsAllowSkip)
                    return false;

                if (!_stepContentIndexs.ContainsKey(stepID))
                    return false;

                int index = _stepContentIndexs[stepID];
                if (index < _currentStepIndex || index > _stepContents.Count - 1)
                    return false;

                Main.Current.StartCoroutine(SkipStepCoroutine(index));
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 跳过当前步骤（立即模式）
        /// </summary>
        /// <returns>跳过成功/失败</returns>
        public bool SkipCurrentStepImmediate()
        {
            if (_running && !_executing)
            {
                if (Pause)
                    return false;

                if (_currentHelper != null && !_currentHelper.IsAllowSkip)
                    return false;

                SkipCurrentStepImmediateCoroutine();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 跳过到指定步骤（立即模式）
        /// </summary>
        /// <param name="stepID">步骤ID</param>
        /// <returns>跳过成功/失败</returns>
        public bool SkipStepImmediate(string stepID)
        {
            if (_running && !_executing)
            {
                if (Pause)
                    return false;

                if (_currentHelper != null && !_currentHelper.IsAllowSkip)
                    return false;

                if (!_stepContentIndexs.ContainsKey(stepID))
                    return false;

                int index = _stepContentIndexs[stepID];
                if (index < _currentStepIndex || index > _stepContents.Count - 1)
                    return false;

                SkipStepImmediateCoroutine(index);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 停止未完成的连续跳过
        /// </summary>
        public void StopSkip()
        {
            _skipTargetIndex = _currentStepIndex;
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
                if (Pause)
                    return false;

                if (!_stepContentIndexs.ContainsKey(stepID))
                    return false;

                int index = _stepContentIndexs[stepID];
                if (index < 0 || index >= _currentStepIndex)
                    return false;

                CurrentTask = StepHelperTask.Restore;

                while (_currentStepIndex >= index)
                {
                    _currentContent = _stepContents[_currentStepIndex];
                    _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

                    //创建步骤助手
                    if (_currentHelper == null)
                    {
                        _currentHelper = CreateHelper(_currentContent, StepHelperTask.Restore);
                    }

                    RestoreStepEvent?.Invoke(_currentContent, _currentContent.IsEnable && _currentContent.IsEnableRunTime);

                    //助手执行恢复，并销毁
                    RestoreHelper();
                    DestroyHelper();
                    
                    _currentStepIndex -= 1;
                }

                _currentStepIndex = index;
                BeginCurrentStep();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 展示提示【步骤编辑器面板的“提示”节点呼叫】
        /// </summary>
        /// <param name="content">提示内容</param>
        public void ShowPrompt(string content)
        {
            ShowPromptEvent?.Invoke(content);
        }
        /// <summary>
        /// 指引当前步骤
        /// </summary>
        public void Guide()
        {
            if (_currentContent != null)
            {
                if (_currentHelper != null)
                {
                    try
                    {
                        _currentHelper.OnGuide();
                    }
                    catch (Exception e)
                    {
                        Log.Error(string.Format("步骤控制器：指引步骤【{0}.{1}】的助手【{2}】时出错！错误描述：{3}", CurrentStepIndex, CurrentStepContent.Name, CurrentStepContent.Helper, e.ToString()));
                    }
                }
                else
                {
                    GameObject target = _currentContent.Target;
                    Collider collider = target.GetComponent<Collider>();
                    if (collider && collider.enabled)
                    {
                        switch (GuideHighlighting)
                        {
                            case MouseRay.HighlightingType.Normal:
                                target.OpenHighLight(NormalColor);
                                break;
                            case MouseRay.HighlightingType.Flash:
                                target.OpenFlashHighLight(FlashColor1, FlashColor2);
                                break;
                            case MouseRay.HighlightingType.Outline:
                                target.OpenMeshOutline(NormalColor, OutlineIntensity);
                                break;
                        }
                    }

                    Main.m_Controller.Mode = _currentContent.InitialMode;
                    Main.m_Controller.SetLookPoint(target.transform.position + _currentContent.ViewOffset, false);
                    Main.m_Controller.SetLookAngle(_currentContent.BestView);
                }
            }
            else
            {
                ShowPrompt("当前不存在指引目标！");
            }
        }
        /// <summary>
        /// 完成当前步骤（只用于 StateChange 型步骤）
        /// </summary>
        public void CompleteCurrentStep()
        {
            if (_running && !_executing)
            {
                if (Pause)
                    return;

                if (_currentContent.Trigger == StepTrigger.StateChange)
                {
                    _currentTarget.State = StepTargetState.Done;
                }
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

        /// <summary>
        /// 步骤开始
        /// </summary>
        private void BeginCurrentStep()
        {
            _executing = false;
            _currentContent = _stepContents[_currentStepIndex];
            _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

            CurrentTask = StepHelperTask.Execute;

            //UGUI按钮点击型步骤，注册监听
            if (_currentContent.Trigger == StepTrigger.ButtonClick)
            {
                _isButtonClick = false;
                _currentButton = _currentTarget.GetComponent<Button>();
                if (_currentButton)
                {
                    _currentButton.onClick.AddListener(ButtonClickCallback);
                }
                else
                {
                    Log.Error(string.Format("步骤控制器：【步骤：{0}】【{1}】的目标丢失Button组件！", _currentStepIndex + 1, _currentContent.Name));
                }
            }
            //状态改变触发类型的步骤，自动重置状态
            else if (_currentContent.Trigger == StepTrigger.StateChange)
            {
                _currentTarget.State = StepTargetState.Normal;
            }

            //激活的步骤正常启动，未激活的步骤自动跳过
            if (_currentContent.IsEnable && _currentContent.IsEnableRunTime)
            {
                //创建步骤助手
                _currentHelper = CreateHelper(_currentContent, StepHelperTask.Execute);

                BeginStepEvent?.Invoke(_currentContent, true);
            }
            else
            {
                //创建步骤助手
                _currentHelper = CreateHelper(_currentContent, StepHelperTask.SkipImmediate);

                BeginStepEvent?.Invoke(_currentContent, false);

                SkipCurrentStepImmediateCoroutine();
            }
        }
        /// <summary>
        /// 步骤执行
        /// </summary>
        private void ExecuteCurrentStep()
        {
            _executing = true;
            _currentContent.Execute();

            ExecuteStepEvent?.Invoke(_currentContent, _currentContent.IsEnable && _currentContent.IsEnableRunTime);

            //UGUI按钮点击型步骤，取消按钮注册
            if (_currentButton)
            {
                _currentButton.onClick.RemoveListener(ButtonClickCallback);
                _currentButton = null;
            }

            //销毁步骤助手
            DestroyHelper();
        }
        /// <summary>
        /// 跳过当前步骤
        /// </summary>
        private IEnumerator SkipCurrentStepCoroutine()
        {
            _executing = true;

            CurrentTask = StepHelperTask.Skip;

            //UGUI按钮点击型步骤，自动执行按钮事件
            if (_currentContent.Trigger == StepTrigger.ButtonClick)
            {
                if (_currentButton)
                {
                    _currentButton.onClick.Invoke();
                    _currentButton.onClick.RemoveListener(ButtonClickCallback);
                }
                else
                {
                    _currentButton = _currentTarget.GetComponent<Button>();
                    if (_currentButton)
                    {
                        _currentButton.onClick.Invoke();
                    }
                    else
                    {
                        Log.Error(string.Format("步骤控制器：【步骤：{0}】【{1}】的目标丢失Button组件！", _currentStepIndex + 1, _currentContent.Name));
                    }
                }
                _currentButton = null;
            }

            //创建步骤助手
            if (_currentHelper == null)
            {
                _currentHelper = CreateHelper(_currentContent, StepHelperTask.Skip);
            }

            _currentContent.Skip();

            SkipStepEvent?.Invoke(_currentContent, _currentContent.IsEnable && _currentContent.IsEnableRunTime);

            float elapseTime = _currentContent.Instant ? 0 : (_currentHelper != null ? _currentHelper.ElapseTime : _currentContent.ElapseTime);

            //助手执行跳过，等待生命周期结束后销毁助手
            yield return SkipHelper();
            DestroyHelper();

            _waitCoroutine = Main.Current.StartCoroutine(WaitCoroutine(ChangeNextStep, elapseTime));
        }
        /// <summary>
        /// 跳过到指定步骤
        /// </summary>
        /// <param name="index">步骤索引</param>
        private IEnumerator SkipStepCoroutine(int index)
        {
            _executing = true;
            _skipTargetIndex = index;

            CurrentTask = StepHelperTask.Skip;

            while (_currentStepIndex < _skipTargetIndex)
            {
                _currentContent = _stepContents[_currentStepIndex];
                _currentTarget = _currentContent.Target.GetComponent<StepTarget>();
                
                //UGUI按钮点击型步骤，自动执行按钮事件
                if (_currentContent.Trigger == StepTrigger.ButtonClick)
                {
                    if (_currentButton)
                    {
                        _currentButton.onClick.Invoke();
                        _currentButton.onClick.RemoveListener(ButtonClickCallback);
                    }
                    else
                    {
                        _currentButton = _currentTarget.GetComponent<Button>();
                        if (_currentButton)
                        {
                            _currentButton.onClick.Invoke();
                        }
                        else
                        {
                            Log.Error(string.Format("步骤控制器：【步骤：{0}】【{1}】的目标丢失Button组件！", _currentStepIndex + 1, _currentContent.Name));
                        }
                    }
                    _currentButton = null;
                }

                //创建步骤助手
                if (_currentHelper == null)
                {
                    _currentHelper = CreateHelper(_currentContent, StepHelperTask.Skip);
                }

                _currentContent.Skip();

                SkipStepEvent?.Invoke(_currentContent, _currentContent.IsEnable && _currentContent.IsEnableRunTime);

                float elapseTime = _currentContent.Instant ? 0 : (_currentHelper != null ? _currentHelper.ElapseTime : _currentContent.ElapseTime);

                //助手执行跳过，等待生命周期结束后销毁助手
                yield return SkipHelper();
                DestroyHelper();

                yield return YieldInstructioner.GetWaitForSeconds(elapseTime);

                _currentStepIndex += 1;
            }

            SkipStepDoneEvent?.Invoke();

            _waitCoroutine = Main.Current.StartCoroutine(WaitCoroutine(BeginCurrentStep, 0));
        }
        /// <summary>
        /// 跳过当前步骤（立即模式）
        /// </summary>
        private void SkipCurrentStepImmediateCoroutine()
        {
            _executing = true;

            CurrentTask = StepHelperTask.SkipImmediate;

            //UGUI按钮点击型步骤，自动执行按钮事件
            if (_currentContent.Trigger == StepTrigger.ButtonClick)
            {
                if (_currentButton)
                {
                    _currentButton.onClick.Invoke();
                    _currentButton.onClick.RemoveListener(ButtonClickCallback);
                }
                else
                {
                    _currentButton = _currentTarget.GetComponent<Button>();
                    if (_currentButton)
                    {
                        _currentButton.onClick.Invoke();
                    }
                    else
                    {
                        Log.Error(string.Format("步骤控制器：【步骤：{0}】【{1}】的目标丢失Button组件！", _currentStepIndex + 1, _currentContent.Name));
                    }
                }
                _currentButton = null;
            }

            //创建步骤助手
            if (_currentHelper == null)
            {
                _currentHelper = CreateHelper(_currentContent, StepHelperTask.SkipImmediate);
            }

            _currentContent.SkipImmediate();

            SkipStepImmediateEvent?.Invoke(_currentContent, _currentContent.IsEnable && _currentContent.IsEnableRunTime);

            //助手执行跳过，等待生命周期结束后销毁助手
            SkipImmediateHelper();
            DestroyHelper();

            ChangeNextStep();
        }
        /// <summary>
        /// 跳过到指定步骤（立即模式）
        /// </summary>
        /// <param name="index">步骤索引</param>
        private void SkipStepImmediateCoroutine(int index)
        {
            _executing = true;
            _skipTargetIndex = index;

            CurrentTask = StepHelperTask.SkipImmediate;

            while (_currentStepIndex < _skipTargetIndex)
            {
                _currentContent = _stepContents[_currentStepIndex];
                _currentTarget = _currentContent.Target.GetComponent<StepTarget>();
                
                //UGUI按钮点击型步骤，自动执行按钮事件
                if (_currentContent.Trigger == StepTrigger.ButtonClick)
                {
                    if (_currentButton)
                    {
                        _currentButton.onClick.Invoke();
                        _currentButton.onClick.RemoveListener(ButtonClickCallback);
                    }
                    else
                    {
                        _currentButton = _currentTarget.GetComponent<Button>();
                        if (_currentButton)
                        {
                            _currentButton.onClick.Invoke();
                        }
                        else
                        {
                            Log.Error(string.Format("步骤控制器：【步骤：{0}】【{1}】的目标丢失Button组件！", _currentStepIndex + 1, _currentContent.Name));
                        }
                    }
                    _currentButton = null;
                }

                //创建步骤助手
                if (_currentHelper == null)
                {
                    _currentHelper = CreateHelper(_currentContent, StepHelperTask.SkipImmediate);
                }

                _currentContent.SkipImmediate();

                SkipStepImmediateEvent?.Invoke(_currentContent, _currentContent.IsEnable && _currentContent.IsEnableRunTime);

                //助手执行跳过，等待生命周期结束后销毁助手
                SkipImmediateHelper();
                DestroyHelper();
                
                _currentStepIndex += 1;
            }

            BeginCurrentStep();
        }
        /// <summary>
        /// 进入下一步骤
        /// </summary>
        private void ChangeNextStep()
        {
            if (_customOrder.ContainsKey(_currentContent.GUID))
            {
                _currentStepIndex = _stepContentIndexs[_customOrder[_currentContent.GUID]];
                BeginCurrentStep();
            }
            else
            {
                if (_currentStepIndex < _stepContents.Count - 1)
                {
                    _currentStepIndex += 1;
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
        /// <summary>
        /// 创建步骤助手
        /// </summary>
        private StepHelper CreateHelper(StepContent content, StepHelperTask task)
        {
            try
            {
                if (!string.IsNullOrEmpty(content.Helper) && content.Helper != "<None>")
                {
                    Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(content.Helper);
                    if (type != null)
                    {
                        StepHelper helper = Activator.CreateInstance(type) as StepHelper;
                        for (int i = 0; i < content.Parameters.Count; i++)
                        {
                            StepParameter parameter = content.Parameters[i];
                            if (parameter.Type == StepParameter.ParameterType.GameObject)
                            {
                                if (_targets.ContainsKey(parameter.GameObjectGUID))
                                {
                                    parameter.GameObjectValue = _targets[parameter.GameObjectGUID].gameObject;
                                }
                            }
                            helper.Parameters.Add(parameter);
                        }
                        helper.Content = content;
                        helper.Target = content.Target.GetComponent<StepTarget>();
                        helper.Task = task;
                        helper.OnInit();
                        return helper;
                    }
                    else
                    {
                        Log.Error(string.Format("步骤控制器：步骤【{0}.{1}】的助手【{2}】丢失！", CurrentStepIndex, CurrentStepContent.Name, CurrentStepContent.Helper));
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Error(string.Format("步骤控制器：创建步骤【{0}.{1}】的助手【{2}】时出错！错误描述：{3}", CurrentStepIndex, CurrentStepContent.Name, CurrentStepContent.Helper, e.ToString()));
                return null;
            }
        }
        /// <summary>
        /// 跳过步骤助手
        /// </summary>
        private IEnumerator SkipHelper()
        {
            if (_currentHelper != null)
            {
                _currentHelper.Task = StepHelperTask.Skip;
                try
                {
                    _currentHelper.OnSkip();
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("步骤控制器：跳过步骤【{0}.{1}】的助手【{2}】时出错！错误描述：{3}", CurrentStepIndex, CurrentStepContent.Name, CurrentStepContent.Helper, e.ToString()));
                }
                if (_currentHelper.SkipLifeTime > 0)
                {
                    yield return YieldInstructioner.GetWaitForSeconds(_currentHelper.SkipLifeTime);
                }
            }
        }
        /// <summary>
        /// 立即跳过步骤助手
        /// </summary>
        private void SkipImmediateHelper()
        {
            try
            {
                if (_currentHelper != null)
                {
                    _currentHelper.Task = StepHelperTask.SkipImmediate;
                    _currentHelper.OnSkipImmediate();
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("步骤控制器：立即跳过步骤【{0}.{1}】的助手【{2}】时出错！错误描述：{3}", CurrentStepIndex, CurrentStepContent.Name, CurrentStepContent.Helper, e.ToString()));
            }
        }
        /// <summary>
        /// 恢复步骤助手
        /// </summary>
        private void RestoreHelper()
        {
            try
            {
                if (_currentHelper != null)
                {
                    _currentHelper.Task = StepHelperTask.Restore;
                    _currentHelper.OnRestore();
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("步骤控制器：恢复步骤【{0}.{1}】的助手【{2}】时出错！错误描述：{3}", CurrentStepIndex, CurrentStepContent.Name, CurrentStepContent.Helper, e.ToString()));
            }
        }
        /// <summary>
        /// 销毁步骤助手
        /// </summary>
        private void DestroyHelper()
        {
            try
            {
                if (_currentHelper != null)
                {
                    _currentHelper.OnTermination();
                    _currentHelper = null;
                }
            }
            catch (Exception e)
            {
                Log.Error(string.Format("步骤控制器：销毁步骤【{0}.{1}】的助手【{2}】时出错！错误描述：{3}", CurrentStepIndex, CurrentStepContent.Name, CurrentStepContent.Helper, e.ToString()));
            }
        }
        /// <summary>
        /// 等待协程
        /// </summary>
        private IEnumerator WaitCoroutine(HTFAction action, float time)
        {
            yield return YieldInstructioner.GetWaitForSeconds(time);
            yield return _pauseWait;

            action();
        }
    }
}