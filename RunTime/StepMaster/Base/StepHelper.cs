using System.Collections.Generic;
using UnityEngine;

namespace HT.Framework
{
    /// <summary>
    /// 步骤的助手，步骤开始时自动创建，结束后自动销毁
    /// </summary>
    public abstract class StepHelper
    {
        /// <summary>
        /// 步骤内容
        /// </summary>
        public StepContent Content { get; internal set; }
        /// <summary>
        /// 步骤目标
        /// </summary>
        public StepTarget Target { get; internal set; }
        /// <summary>
        /// 助手当前执行的任务类型
        /// </summary>
        public StepHelperTask Task { get; internal set; }
        /// <summary>
        /// 步骤的参数
        /// </summary>
        public List<StepParameter> Parameters { get; private set; } = new List<StepParameter>();
        /// <summary>
        /// 步骤辅助目标
        /// </summary>
        public HashSet<GameObject> AuxiliaryTarget { get; private set; } = new HashSet<GameObject>();
        /// <summary>
        /// 是否启用帧更新
        /// </summary>
        public bool IsEnableUpdate { get; set; } = true;
        /// <summary>
        /// 是否允许跳过
        /// </summary>
        public bool IsAllowSkip { get; set; } = true;
        /// <summary>
        /// 步骤执行到进入下一步的时间
        /// </summary>
        public virtual float ElapseTime
        {
            get
            {
                return Content.ElapseTime;
            }
        }
        /// <summary>
        /// 跳过时生命周期（仅在跳过时生效）
        /// </summary>
        public virtual float SkipLifeTime
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void OnInit()
        {

        }
        /// <summary>
        /// 跳过步骤（仅在跳过时执行）
        /// </summary>
        public virtual void OnSkip()
        {

        }
        /// <summary>
        /// 跳过步骤，立即模式（仅在立即跳过时执行）
        /// </summary>
        public virtual void OnSkipImmediate()
        {

        }
        /// <summary>
        /// 恢复步骤（仅在倒退时执行）
        /// </summary>
        public virtual void OnRestore()
        {

        }
        /// <summary>
        /// 步骤指引（由步骤控制者呼叫）
        /// </summary>
        public virtual void OnGuide()
        {
            Collider collider = Target.GetComponent<Collider>();
            if (collider && collider.enabled)
            {
                switch (Main.m_StepMaster.GuideHighlighting)
                {
                    case MouseRay.HighlightingType.Normal:
                        Target.gameObject.OpenHighLight(Main.m_StepMaster.NormalColor);
                        break;
                    case MouseRay.HighlightingType.Flash:
                        Target.gameObject.OpenFlashHighLight(Main.m_StepMaster.FlashColor1, Main.m_StepMaster.FlashColor2);
                        break;
                    case MouseRay.HighlightingType.Outline:
                        Target.gameObject.OpenMeshOutline(Main.m_StepMaster.NormalColor, Main.m_StepMaster.OutlineIntensity);
                        break;
                }
            }

            Main.m_Controller.Mode = Content.InitialMode;
            Main.m_Controller.SetLookPoint(Target.transform.position + Content.ViewOffset, false);
            Main.m_Controller.SetLookAngle(Content.BestView);
        }
        /// <summary>
        /// 帧更新（仅在步骤执行前生效）
        /// </summary>
        public virtual void OnUpdate()
        {

        }
        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void OnTermination()
        {

        }
        /// <summary>
        /// 步骤执行
        /// </summary>
        protected virtual void Execute()
        {
            Target.State = StepTargetState.Done;
        }

        /// <summary>
        /// 是否存在指定名称、类型的参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <returns>是否存在</returns>
        public bool IsExistParameter(string parameterName, StepParameter.ParameterType parameterType)
        {
            StepParameter stepParameter = Parameters.Find((p) => { return p.Name == parameterName && p.Type == parameterType; });
            return stepParameter != null;
        }
        /// <summary>
        /// 是否存在指定名称的参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistParameter(string parameterName)
        {
            StepParameter stepParameter = Parameters.Find((p) => { return p.Name == parameterName; });
            return stepParameter != null;
        }
        /// <summary>
        /// 通过名称、类型获取所有参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <param name="stepParameters">输出的参数列表</param>
        public void GetParameters(string parameterName, StepParameter.ParameterType parameterType, List<StepParameter> stepParameters)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Name == parameterName && Parameters[i].Type == parameterType)
                {
                    stepParameters.Add(Parameters[i]);
                }
            }
        }
        /// <summary>
        /// 通过名称获取所有参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="stepParameters">输出的参数列表</param>
        public void GetParameters(string parameterName, List<StepParameter> stepParameters)
        {
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Name == parameterName)
                {
                    stepParameters.Add(Parameters[i]);
                }
            }
        }
        /// <summary>
        /// 通过名称、类型获取参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <returns>参数</returns>
        public StepParameter GetParameter(string parameterName, StepParameter.ParameterType parameterType)
        {
            StepParameter stepParameter = Parameters.Find((p) => { return p.Name == parameterName && p.Type == parameterType; });
            if (stepParameter != null)
            {
                return stepParameter;
            }
            else
            {
                Log.Error($"步骤：{Content.Name}[ID:{Content.GUID}]未获取到参数[{parameterName}]！");
                return null;
            }
        }
        /// <summary>
        /// 通过名称获取参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public StepParameter GetParameter(string parameterName)
        {
            StepParameter stepParameter = Parameters.Find((p) => { return p.Name == parameterName; });
            if (stepParameter != null)
            {
                return stepParameter;
            }
            else
            {
                Log.Error($"步骤：{Content.Name}[ID:{Content.GUID}]未获取到参数[{parameterName}]！");
                return null;
            }
        }
        /// <summary>
        /// 通过名称获取String参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public string GetStringParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.String);
            return (stepParameter != null) ? stepParameter.StringValue : null;
        }
        /// <summary>
        /// 通过名称获取Integer参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public int GetIntegerParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.Integer);
            return (stepParameter != null) ? stepParameter.IntegerValue : 0;
        }
        /// <summary>
        /// 通过名称获取Float参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public float GetFloatParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.Float);
            return (stepParameter != null) ? stepParameter.FloatValue : 0f;
        }
        /// <summary>
        /// 通过名称获取Boolean参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public bool GetBooleanParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.Boolean);
            return (stepParameter != null) ? stepParameter.BooleanValue : false;
        }
        /// <summary>
        /// 通过名称获取Vector2参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Vector2 GetVector2Parameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.Vector2);
            return (stepParameter != null) ? stepParameter.Vector2Value : Vector2.zero;
        }
        /// <summary>
        /// 通过名称获取Vector3参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Vector3 GetVector3Parameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.Vector3);
            return (stepParameter != null) ? stepParameter.Vector3Value : Vector3.zero;
        }
        /// <summary>
        /// 通过名称获取Color参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Color GetColorParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.Color);
            return (stepParameter != null) ? stepParameter.ColorValue : Color.white;
        }
        /// <summary>
        /// 通过名称获取GameObject参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public GameObject GetGameObjectParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.GameObject);
            return (stepParameter != null) ? stepParameter.GameObjectValue : null;
        }
        /// <summary>
        /// 通过名称获取Texture参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Texture GetTextureParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.Texture);
            return (stepParameter != null) ? stepParameter.TextureValue : null;
        }
        /// <summary>
        /// 通过名称获取AudioClip参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public AudioClip GetAudioClipParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.AudioClip);
            return (stepParameter != null) ? stepParameter.AudioClipValue : null;
        }
        /// <summary>
        /// 通过名称获取Material参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public Material GetMaterialParameter(string parameterName)
        {
            StepParameter stepParameter = GetParameter(parameterName, StepParameter.ParameterType.Material);
            return (stepParameter != null) ? stepParameter.MaterialValue : null;
        }
    }

    /// <summary>
    /// 步骤助手的任务
    /// </summary>
    public enum StepHelperTask
    {
        /// <summary>
        /// 常规执行任务
        /// </summary>
        Execute,
        /// <summary>
        /// 跳过任务
        /// </summary>
        Skip,
        /// <summary>
        /// 跳过任务（立即模式）
        /// </summary>
        SkipImmediate,
        /// <summary>
        /// 恢复任务
        /// </summary>
        Restore
    }
}