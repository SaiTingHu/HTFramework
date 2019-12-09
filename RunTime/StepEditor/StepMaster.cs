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
    public sealed class StepMaster : ModuleManagerBase
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
        /// 指引目标的高亮方式
        /// </summary>
        public MouseRay.HighlightingType GuideHighlighting = MouseRay.HighlightingType.Flash;
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

        //所有的 StepTarget <步骤目标ID、步骤目标>
        private Dictionary<string, StepTarget> _targets = new Dictionary<string, StepTarget>();
        //所有的 自定义执行顺序 <原始步骤ID、目标步骤ID>
        private Dictionary<string, string> _customOrder = new Dictionary<string, string>();
        //所有的 步骤内容
        private List<StepContent> _stepContents = new List<StepContent>();
        //所有的 步骤启用标记 <步骤ID、启用标记>
        private Dictionary<string, bool> _stepContentEnables = new Dictionary<string, bool>();
        //所有的 步骤内容 <步骤ID、步骤内容>
        private Dictionary<string, StepContent> _stepContentIDs = new Dictionary<string, StepContent>();
        //所有的 步骤索引 <步骤ID、步骤索引>
        private Dictionary<string, int> _stepContentIndexs = new Dictionary<string, int>();
        private int _currentStepIndex = -1;
        private StepContent _currentContent;
        private StepTarget _currentTarget;
        private StepHelper _currentHelper;
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

        public override void OnInitialization()
        {
            base.OnInitialization();

            _pauseWait = new WaitUntil(() => { return !_pause; });
        }

        public override void OnRefresh()
        {
            base.OnRefresh();

            if (_running)
            {
                if (_pause)
                    return;

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
                            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
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
                            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
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
                            if (Main.m_Input.GetButtonDown(InputButtonType.MouseLeft))
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
                            _waitCoroutine = Main.Current.StartCoroutine(WaitCoroutine(ChangeNextStep, 0));
                        }
                        else
                        {
                            _waitCoroutine = Main.Current.StartCoroutine(WaitCoroutine(ChangeNextStep, _currentContent.ElapseTime));
                        }
                    }
                }
            }
        }

        public override void OnTermination()
        {
            base.OnTermination();

            _targets.Clear();
            _stepContents.Clear();
            _stepContentEnables.Clear();
            _stepContentIDs.Clear();
            _stepContentIndexs.Clear();
            ClearCustomOrder();
        }

        public override void OnPause()
        {
            base.OnPause();

            Pause = true;
        }

        public override void OnUnPause()
        {
            base.OnUnPause();

            Pause = false;
        }

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
        /// <summary>
        /// 根据步骤ID获取步骤的启用标记
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
        /// 重新编译步骤内容，在更改步骤资源 ContentAsset 后，必须重新编译一次才可以开始任务流程
        /// </summary>
        /// <param name="disableStepIDs">禁用的步骤ID集合（当为null时启用所有步骤，禁用的步骤会自动跳过）</param>
        public void RecompileStepContent(HashSet<string> disableStepIDs = null)
        {
            if (ContentAsset)
            {
                #region 搜寻步骤目标
                _targets.Clear();
                //搜寻框架下所有步骤目标
                List<StepTarget> targetCaches = new List<StepTarget>();
                Main.Current.transform.GetComponentsInChildren(true, targetCaches);
                for (int i = 0; i < targetCaches.Count; i++)
                {
                    if (!_targets.ContainsKey(targetCaches[i].GUID))
                    {
                        _targets.Add(targetCaches[i].GUID, targetCaches[i]);
                    }
                    else
                    {
                        GlobalTools.LogWarning(string.Format("步骤控制者：发现相同GUID的目标！GUID：{0}\r\n目标物体：{1} 和 {2}", targetCaches[i].GUID, _targets[targetCaches[i].GUID].transform.FullName(), targetCaches[i].transform.FullName()));
                    }
                }
                //搜寻场景中所有步骤目标
                GameObject[] rootObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
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
                            GlobalTools.LogWarning(string.Format("步骤控制者：发现相同GUID的目标！GUID：{0}\r\n目标物体：{1} 和 {2}", targetCaches[i].GUID, _targets[targetCaches[i].GUID].transform.FullName(), targetCaches[i].transform.FullName()));
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
                        GlobalTools.LogError(string.Format("步骤控制者：发现相同GUID的步骤！GUID：{0}\r\n步骤：{1} 和 {2}", content.GUID, _stepContentIDs[content.GUID].Name, content.Name));
                    }
                    else
                    {
                        _stepContentIDs.Add(content.GUID, content);
                    }
                }
                #endregion

                #region 生成所有步骤信息
                _stepContents.Clear();
                _stepContentEnables.Clear();
                _stepContentIndexs.Clear();
                //启用所有步骤
                if (disableStepIDs == null || disableStepIDs.Count == 0)
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
                            GlobalTools.LogError(string.Format("步骤控制者：【步骤：{0}】【{1}】目标没有找到，目标路径：{2}", i, content.Name, content.TargetPath));
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
                                GlobalTools.LogError(string.Format("步骤控制者：【步骤：{0}】【操作：{1}】目标没有找到，目标路径：{2}", i, operation.Name, operation.TargetPath));
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
                //禁用 disableStepIDs 指定的步骤
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
                            GlobalTools.LogError(string.Format("步骤控制者：【步骤：{0}】【{1}】目标没有找到，目标路径：{2}", i, content.Name, content.TargetPath));
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
                                GlobalTools.LogError(string.Format("步骤控制者：【步骤：{0}】【操作：{1}】目标没有找到，目标路径：{2}", i, operation.Name, operation.TargetPath));
                            }
                        }

                        _stepContents.Add(content);
                        if (!_stepContentEnables.ContainsKey(content.GUID))
                        {
                            _stepContentEnables.Add(content.GUID, !disableStepIDs.Contains(content.GUID));
                            _stepContentIndexs.Add(content.GUID, _stepContents.Count - 1);
                        }
                    }
                }
                
                _currentStepIndex = 0;
                _currentContent = null;
                _currentTarget = null;
                _currentHelper = null;
                _running = false;
                _pause = false;
                _executing = false;

                ClearCustomOrder();
                #endregion
            }
            else
            {
                throw new HTFrameworkException(HTFrameworkModule.StepEditor, "步骤控制者：重新编译步骤失败，步骤控制者丢失了步骤资源 StepContentAsset！");
            }
        }
        /// <summary>
        /// 开始任务流程
        /// </summary>
        public void Begin()
        {
            if (!ContentAsset || ContentAsset.Content.Count <= 0 || _stepContents.Count <= 0)
            {
                throw new HTFrameworkException(HTFrameworkModule.StepEditor, "步骤控制者：当前无法开始任务流程，请重新编译步骤内容 RecompileStepContent！");
            }

            _currentStepIndex = 0;
            _currentContent = null;
            _currentTarget = null;
            _currentHelper = null;
            _running = true;
            _pause = false;
            _executing = false;

            BeginEvent?.Invoke();

            BeginCurrentStep();
        }
        /// <summary>
        /// 结束任务流程
        /// </summary>
        public void End()
        {
            _currentStepIndex = 0;
            _currentContent = null;
            _currentTarget = null;
            _currentHelper = null;
            _running = false;
            _pause = false;
            _executing = false;

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
                if (_pause)
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
                if (_pause)
                    return false;

                if (!_stepContentIndexs.ContainsKey(stepID))
                    return false;

                int index = _stepContentIndexs[stepID];
                if (index <= _currentStepIndex || index > _stepContents.Count - 1)
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
                if (_pause)
                    return false;

                if (!_stepContentIndexs.ContainsKey(stepID))
                    return false;

                int index = _stepContentIndexs[stepID];
                if (index < 0 || index >= _currentStepIndex)
                    return false;

                while (_currentStepIndex >= index)
                {
                    _currentContent = _stepContents[_currentStepIndex];
                    _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

                    RestoreStepEvent?.Invoke(_currentContent, _stepContentEnables.ContainsKey(_currentContent.GUID) ? _stepContentEnables[_currentContent.GUID] : false);

                    //创建步骤助手
                    if (_currentHelper == null && _currentContent.Helper != "<None>")
                    {
                        Type type = GlobalTools.GetTypeInRunTimeAssemblies(_currentContent.Helper);
                        if (type != null)
                        {
                            _currentHelper = Activator.CreateInstance(type) as StepHelper;
                            _currentHelper.Parameters = _currentContent.Parameters;
                            for (int i = 0; i < _currentHelper.Parameters.Count; i++)
                            {
                                if (_currentHelper.Parameters[i].Type == StepParameter.ParameterType.GameObject)
                                {
                                    if (_targets.ContainsKey(_currentHelper.Parameters[i].GameObjectGUID))
                                    {
                                        _currentHelper.Parameters[i].GameObjectValue = _targets[_currentHelper.Parameters[i].GameObjectGUID].gameObject;
                                    }
                                }
                            }
                            _currentHelper.Content = _currentContent;
                            _currentHelper.Target = _currentTarget;
                            _currentHelper.Task = StepHelperTask.Restore;
                            _currentHelper.OnInit();
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("步骤控制者：【步骤：{0}】的助手 {1} 丢失！", _currentStepIndex + 1, _currentContent.Helper));
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
                GameObject target = _currentContent.Target;
                if (target.GetComponent<Collider>())
                {
                    switch (GuideHighlighting)
                    {
                        case MouseRay.HighlightingType.Normal:
                            target.OpenHighLight();
                            break;
                        case MouseRay.HighlightingType.Flash:
                            target.OpenFlashHighLight();
                            break;
                        case MouseRay.HighlightingType.Outline:
                            target.OpenMeshOutline();
                            break;
                    }
                }

                Main.m_Controller.TheControlMode = _currentContent.InitialMode;
                Main.m_Controller.SetLookPoint(target.transform.position + _currentContent.ViewOffset);
                Main.m_Controller.SetLookAngle(_currentContent.BestView);

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
        /// 完成当前步骤（只用于 StateChange 型步骤）
        /// </summary>
        public void CompleteCurrentStep()
        {
            if (_running && !_executing)
            {
                if (_pause)
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

        //步骤开始
        private void BeginCurrentStep()
        {
            _executing = false;
            _currentContent = _stepContents[_currentStepIndex];
            _currentTarget = _currentContent.Target.GetComponent<StepTarget>();

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
                    GlobalTools.LogError(string.Format("步骤控制器：【步骤：{0}】【{1}】的目标丢失Button组件！", _currentStepIndex + 1, _currentContent.Name));
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
                    _currentHelper.Parameters = _currentContent.Parameters;
                    for (int i = 0; i < _currentHelper.Parameters.Count; i++)
                    {
                        if (_currentHelper.Parameters[i].Type == StepParameter.ParameterType.GameObject)
                        {
                            if (_targets.ContainsKey(_currentHelper.Parameters[i].GameObjectGUID))
                            {
                                _currentHelper.Parameters[i].GameObjectValue = _targets[_currentHelper.Parameters[i].GameObjectGUID].gameObject;
                            }
                        }
                    }
                    _currentHelper.Content = _currentContent;
                    _currentHelper.Target = _currentTarget;
                    _currentHelper.Task = StepHelperTask.Execute;
                    _currentHelper.OnInit();
                }
                else
                {
                    GlobalTools.LogError(string.Format("步骤控制器：【步骤：{0}】【{1}】的助手 {2} 丢失！", _currentStepIndex + 1, _currentContent.Name, _currentContent.Helper));
                }
            }
            
            //未激活的步骤自动跳过
            if (_stepContentEnables.ContainsKey(_currentContent.GUID))
            {
                BeginStepEvent?.Invoke(_currentContent, _stepContentEnables[_currentContent.GUID]);
                if (!_stepContentEnables[_currentContent.GUID])
                {
                    Main.Current.StartCoroutine(SkipCurrentStepCoroutine());
                }
            }
            else
            {
                BeginStepEvent?.Invoke(_currentContent, false);
            }
        }
        //步骤执行
        private void ExecuteCurrentStep()
        {
            _executing = true;
            _currentContent.Execute();

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
        //跳过当前步骤
        private IEnumerator SkipCurrentStepCoroutine()
        {
            _executing = true;
            _currentContent.Skip();

            SkipStepEvent?.Invoke(_currentContent, _stepContentEnables.ContainsKey(_currentContent.GUID) ? _stepContentEnables[_currentContent.GUID] : false);

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
                        GlobalTools.LogError(string.Format("步骤控制器：【步骤：{0}】【{1}】的目标丢失Button组件！", _currentStepIndex + 1, _currentContent.Name));
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
                    _currentHelper.Parameters = _currentContent.Parameters;
                    for (int i = 0; i < _currentHelper.Parameters.Count; i++)
                    {
                        if (_currentHelper.Parameters[i].Type == StepParameter.ParameterType.GameObject)
                        {
                            if (_targets.ContainsKey(_currentHelper.Parameters[i].GameObjectGUID))
                            {
                                _currentHelper.Parameters[i].GameObjectValue = _targets[_currentHelper.Parameters[i].GameObjectGUID].gameObject;
                            }
                        }
                    }
                    _currentHelper.Content = _currentContent;
                    _currentHelper.Target = _currentTarget;
                    _currentHelper.Task = StepHelperTask.Skip;
                    _currentHelper.OnInit();
                }
                else
                {
                    GlobalTools.LogError(string.Format("步骤控制器：【步骤：{0}】【{1}】的助手 {2} 丢失！", _currentStepIndex + 1, _currentContent.Name, _currentContent.Helper));
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

            _waitCoroutine = Main.Current.StartCoroutine(WaitCoroutine(ChangeNextStep, _currentContent.ElapseTime / SkipMultiple));
        }
        //跳过到指定步骤
        private IEnumerator SkipStepCoroutine(int index)
        {
            _executing = true;
            _skipTargetIndex = index;

            while (_currentStepIndex < _skipTargetIndex)
            {
                _currentContent = _stepContents[_currentStepIndex];
                _currentTarget = _currentContent.Target.GetComponent<StepTarget>();
                _currentContent.Skip();

                SkipStepEvent?.Invoke(_currentContent, _stepContentEnables.ContainsKey(_currentContent.GUID) ? _stepContentEnables[_currentContent.GUID] : false);

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
                        _currentButton = _currentContent.Target.GetComponent<Button>();
                        if (_currentButton)
                        {
                            _currentButton.onClick.Invoke();
                        }
                        else
                        {
                            GlobalTools.LogError(string.Format("步骤控制器：【步骤：{0}】【{1}】的目标丢失Button组件！", _currentStepIndex + 1, _currentContent.Name));
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
                        _currentHelper.Parameters = _currentContent.Parameters;
                        for (int i = 0; i < _currentHelper.Parameters.Count; i++)
                        {
                            if (_currentHelper.Parameters[i].Type == StepParameter.ParameterType.GameObject)
                            {
                                if (_targets.ContainsKey(_currentHelper.Parameters[i].GameObjectGUID))
                                {
                                    _currentHelper.Parameters[i].GameObjectValue = _targets[_currentHelper.Parameters[i].GameObjectGUID].gameObject;
                                }
                            }
                        }
                        _currentHelper.Content = _currentContent;
                        _currentHelper.Target = _currentTarget;
                        _currentHelper.Task = StepHelperTask.Skip;
                        _currentHelper.OnInit();
                    }
                    else
                    {
                        GlobalTools.LogError(string.Format("步骤控制器：【步骤：{0}】【{1}】的助手 {2} 丢失！", _currentStepIndex + 1, _currentContent.Name, _currentContent.Helper));
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

                _currentStepIndex += 1;
            }

            SkipStepDoneEvent?.Invoke();

            _waitCoroutine = Main.Current.StartCoroutine(WaitCoroutine(BeginCurrentStep, 0));
        }
        //进入下一步骤
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