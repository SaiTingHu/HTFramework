using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HT.Framework
{
    /// <summary>
    /// UDateTime 日期拾取器
    /// </summary>
    public class UDateTimePicker : HTBehaviour
    {
        /// <summary>
        /// 年份和月份显示框
        /// </summary>
        public Text Txt_Show;
        /// <summary>
        /// 上一年份按钮
        /// </summary>
        public Button Btn_LastYear;
        /// <summary>
        /// 下一年份按钮
        /// </summary>
        public Button Btn_NextYear;
        /// <summary>
        /// 上一月份按钮
        /// </summary>
        public Button Btn_LastMonth;
        /// <summary>
        /// 下一月份按钮
        /// </summary>
        public Button Btn_NextMonth;
        /// <summary>
        /// 现在按钮
        /// </summary>
        public Button Btn_Now;
        /// <summary>
        /// 保存按钮
        /// </summary>
        public Button Btn_Save;
        /// <summary>
        /// 取消按钮
        /// </summary>
        public Button Btn_Cancel;
        /// <summary>
        /// 日期按钮模板
        /// </summary>
        public Button Btn_Day;
        /// <summary>
        /// 第一个日期按钮位置
        /// </summary>
        public Vector2 FirstDayPos;
        /// <summary>
        /// 日期按钮间的间距
        /// </summary>
        public float Spacing;
        /// <summary>
        /// 小时输入框
        /// </summary>
        public InputField Ifd_Hour;
        /// <summary>
        /// 分钟输入框
        /// </summary>
        public InputField Ifd_Minute;
        /// <summary>
        /// 秒输入框
        /// </summary>
        public InputField Ifd_Second;

        protected bool _isGeneratedDays = false;
        protected List<Button> _btn_Days = new List<Button>();
        protected UDateTimeField _field;
        protected UDateTime _select = new UDateTime();
        protected int _showYear;
        protected int _showMonth;
        protected int _showDays;
        protected DayOfWeek _firstDayWeek;

        protected override void Awake()
        {
            base.Awake();

            Btn_LastYear.onClick.AddListener(() =>
            {
                _showYear -= 1;
                if (_showYear < 1) _showYear = 1;
                UpdateShowDay();
            });
            Btn_NextYear.onClick.AddListener(() =>
            {
                _showYear += 1;
                if (_showYear > 9999) _showYear = 9999;
                UpdateShowDay();
            });
            Btn_LastMonth.onClick.AddListener(() =>
            {
                _showMonth -= 1;
                if (_showMonth < 1) _showMonth = 12;
                UpdateShowDay();
            });
            Btn_NextMonth.onClick.AddListener(() =>
            {
                _showMonth += 1;
                if (_showMonth > 12) _showMonth = 1;
                UpdateShowDay();
            });
            Btn_Now.onClick.AddListener(() =>
            {
                _showYear = DateTime.Now.Year;
                _showMonth = DateTime.Now.Month;
                _select.FromDateTime(DateTime.Now);
                UpdateShowDay();
                UpdateShowTime();
            });
            Btn_Save.onClick.AddListener(() =>
            {
                _select.CopyTo(_field.Value);
                _field.UpdateField();
                Close();
            });
            Btn_Cancel.onClick.AddListener(() =>
            {
                Close();
            });
            Ifd_Hour.onValueChanged.AddListener((value) =>
            {
                if (int.TryParse(value, out int hour))
                {
                    if (hour < 0)
                    {
                        Ifd_Hour.text = "0";
                    }
                    else if (hour > 23)
                    {
                        Ifd_Hour.text = "23";
                    }
                    else
                    {
                        _select.Hour = hour;
                    }
                }
            });
            Ifd_Minute.onValueChanged.AddListener((value) =>
            {
                if (int.TryParse(value, out int minute))
                {
                    if (minute < 0)
                    {
                        Ifd_Minute.text = "0";
                    }
                    else if (minute > 59)
                    {
                        Ifd_Minute.text = "59";
                    }
                    else
                    {
                        _select.Minute = minute;
                    }
                }
            });
            Ifd_Second.onValueChanged.AddListener((value) =>
            {
                if (int.TryParse(value, out int second))
                {
                    if (second < 0)
                    {
                        Ifd_Second.text = "0";
                    }
                    else if (second > 59)
                    {
                        Ifd_Second.text = "59";
                    }
                    else
                    {
                        _select.Second = second;
                    }
                }
            });
        }

        /// <summary>
        /// 打开日期拾取器
        /// </summary>
        /// <param name="field"></param>
        /// <param name="pos"></param>
        public virtual void Open(UDateTimeField field, Vector2 pos)
        {
            gameObject.SetActive(true);
            transform.position = pos;

            _field = field;
            _field.Value.CopyTo(_select);
            _showYear = _select.Year;
            _showMonth = _select.Month;

            GeneratedDays();
            UpdateShowDay();
            UpdateShowTime();
        }
        /// <summary>
        /// 关闭日期拾取器
        /// </summary>
        public virtual void Close()
        {
            gameObject.SetActive(false);

            _field = null;
        }

        /// <summary>
        /// 生成所有日期按钮
        /// </summary>
        protected virtual void GeneratedDays()
        {
            if (!_isGeneratedDays)
            {
                _isGeneratedDays = true;
                Vector2 pos = FirstDayPos;
                for (int i = 0; i < 6; i++)
                {
                    pos.x = FirstDayPos.x;
                    for (int j = 0; j < 7; j++)
                    {
                        GameObject obj = Main.CloneGameObject(Btn_Day.gameObject, true);
                        Button button = obj.GetComponent<Button>();
                        button.rectTransform().anchoredPosition = pos;
                        button.onClick.AddListener(() =>
                        {
                            _select.Year = _showYear;
                            _select.Month = _showMonth;
                            if (int.TryParse(button.GetComponentByChild<Text>("Text").text, out int d))
                            {
                                _select.Day = d;
                                UpdateShowDay(true);
                            }
                        });
                        _btn_Days.Add(button);
                        pos.x += Spacing;
                    }
                    pos.y -= Spacing;
                }
            }
        }
        /// <summary>
        /// 更新显示的日期按钮
        /// </summary>
        /// <param name="isOnlyUpdateSelectDay">仅更新已变动的日期</param>
        protected virtual void UpdateShowDay(bool isOnlyUpdateSelectDay = false)
        {
            if (!isOnlyUpdateSelectDay)
            {
                _showDays = DateTime.DaysInMonth(_showYear, _showMonth);
                _firstDayWeek = new DateTime(_showYear, _showMonth, 1).DayOfWeek;
                Txt_Show.text = $"{_showYear}年{_showMonth}月";
            }

            int firstDay = (int)_firstDayWeek;
            int day = 1;
            for (int i = 0; i < _btn_Days.Count; i++)
            {
                Text text = _btn_Days[i].GetComponentByChild<Text>("Text");
                if (i < firstDay || day > _showDays)
                {
                    if (!isOnlyUpdateSelectDay)
                    {
                        _btn_Days[i].interactable = false;
                        text.text = null;
                    }
                }
                else
                {
                    if (!isOnlyUpdateSelectDay)
                    {
                        _btn_Days[i].interactable = true;
                        text.text = day.ToString();
                    }
                    text.color = (_showYear == _select.Year && _showMonth == _select.Month && day == _select.Day) ? Color.red : Color.black;
                    day++;
                }
            }
        }
        /// <summary>
        /// 更新显示的时间
        /// </summary>
        protected virtual void UpdateShowTime()
        {
            Ifd_Hour.text = _select.Hour.ToString();
            Ifd_Minute.text = _select.Minute.ToString();
            Ifd_Second.text = _select.Second.ToString();
        }
    }
}