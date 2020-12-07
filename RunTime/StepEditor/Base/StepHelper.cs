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
        public List<StepParameter> Parameters { get; internal set; } = null;
        /// <summary>
        /// 步骤辅助目标
        /// </summary>
        public HashSet<GameObject> AuxiliaryTarget { get; private set; } = new HashSet<GameObject>();
        /// <summary>
        /// 是否启用帧刷新
        /// </summary>
        public bool IsEnableUpdate { get; set; } = true;
        /// <summary>
        /// 是否允许跳过
        /// </summary>
        public bool IsAllowSkip { get; set; } = true;
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

        }
        /// <summary>
        /// 帧刷新（仅在步骤执行前生效）
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
            if (Parameters == null)
            {
                return false;
            }
            else
            {
                StepParameter stepParameter = Parameters.Find((p) => { return p.Name == parameterName && p.Type == parameterType; });
                return stepParameter != null;
            }
        }
        /// <summary>
        /// 是否存在指定名称的参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistParameter(string parameterName)
        {
            if (Parameters == null)
            {
                return false;
            }
            else
            {
                StepParameter stepParameter = Parameters.Find((p) => { return p.Name == parameterName; });
                return stepParameter != null;
            }
        }
        /// <summary>
        /// 通过名称、类型获取所有参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <param name="parameterType">参数类型</param>
        /// <param name="stepParameters">输出的参数列表</param>
        public void GetParameters(string parameterName, StepParameter.ParameterType parameterType, List<StepParameter> stepParameters)
        {
            if (Parameters != null)
            {
                for (int i = 0; i < Parameters.Count; i++)
                {
                    if (Parameters[i].Name == parameterName && Parameters[i].Type == parameterType)
                    {
                        stepParameters.Add(Parameters[i]);
                    }
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
            if (Parameters != null)
            {
                for (int i = 0; i < Parameters.Count; i++)
                {
                    if (Parameters[i].Name == parameterName)
                    {
                        stepParameters.Add(Parameters[i]);
                    }
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
            if (Parameters == null)
            {
                Log.Error(string.Format("步骤：{0}[ID:{1}]未获取到参数[{2}]！", Content.Name, Content.GUID, parameterName));
                return null;
            }
            else
            {
                StepParameter stepParameter = Parameters.Find((p) => { return p.Name == parameterName && p.Type == parameterType; });
                if (stepParameter != null)
                {
                    return stepParameter;
                }
                else
                {
                    Log.Error(string.Format("步骤：{0}[ID:{1}]未获取到参数[{2}]！", Content.Name, Content.GUID, parameterName));
                    return null;
                }
            }
        }
        /// <summary>
        /// 通过名称获取参数
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数</returns>
        public StepParameter GetParameter(string parameterName)
        {
            if (Parameters == null)
            {
                Log.Error(string.Format("步骤：{0}[ID:{1}]未获取到参数[{2}]！", Content.Name, Content.GUID, parameterName));
                return null;
            }
            else
            {
                StepParameter stepParameter = Parameters.Find((p) => { return p.Name == parameterName; });
                if (stepParameter != null)
                {
                    return stepParameter;
                }
                else
                {
                    Log.Error(string.Format("步骤：{0}[ID:{1}]未获取到参数[{2}]！", Content.Name, Content.GUID, parameterName));
                    return null;
                }
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
            return (stepParameter != null) ? stepParameter.StringValue : "";
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
        /// 恢复任务
        /// </summary>
        Restore
    }
}