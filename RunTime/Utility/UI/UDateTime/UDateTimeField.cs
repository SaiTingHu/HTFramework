using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UDateTime 的显示字段
    /// </summary>
    public class UDateTimeField : Selectable, IPointerClickHandler
    {
        /// <summary>
        /// 文本显示框
        /// </summary>
        public Text CaptionText;
        /// <summary>
        /// 日期时间值
        /// </summary>
        public UDateTime Value;
        /// <summary>
        /// 日期拾取器
        /// </summary>
        [SerializeField] protected UDateTimePicker Picker;
        /// <summary>
        /// 日期拾取器的显示位置
        /// </summary>
        [SerializeField] protected Transform PickerPos;
        /// <summary>
        /// 鼠标左键点击事件
        /// </summary>
        public UnityEvent OnClick = new UnityEvent();

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }
        /// <summary>
        /// 更新显示字段
        /// </summary>
        public void UpdateField()
        {
            CaptionText.text = Value.ToString();
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            if (Picker == null)
            {
                Picker = FindObjectOfType<UDateTimePicker>(true);
            }

            if (Picker != null)
            {
                Picker.Open(this, PickerPos.position);
            }
            else
            {
                Log.Error("UDateTimeField：未指定日期拾取器，且未在场景中找到任意日期拾取器！", this);
            }

            OnClick?.Invoke();
        }
    }
}