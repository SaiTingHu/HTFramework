using System;

namespace HT.Framework
{
    /// <summary>
    /// 日期时间
    /// </summary>
    [Serializable]
    public sealed class UDateTime
    {
        /// <summary>
        /// 年
        /// </summary>
        public int Year = 1970;
        /// <summary>
        /// 月
        /// </summary>
        public int Month = 1;
        /// <summary>
        /// 日
        /// </summary>
        public int Day = 1;
        /// <summary>
        /// 时
        /// </summary>
        public int Hour = 0;
        /// <summary>
        /// 分
        /// </summary>
        public int Minute = 0;
        /// <summary>
        /// 秒
        /// </summary>
        public int Second = 0;
        /// <summary>
        /// 日期显示格式
        /// </summary>
        public string Format = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 从 System.DateTime 获取值
        /// </summary>
        public void FromDateTime(DateTime dateTime)
        {
            Year = dateTime.Year;
            Month = dateTime.Month;
            Day = dateTime.Day;
            Hour = dateTime.Hour;
            Minute = dateTime.Minute;
            Second = dateTime.Second;
        }
        /// <summary>
        /// 转换为 System.DateTime
        /// </summary>
        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, Minute, Second);
        }
        /// <summary>
        /// 拷贝数据到其他日期
        /// </summary>
        /// <param name="dateTime">其他日期</param>
        public void CopyTo(UDateTime dateTime)
        {
            dateTime.Year = Year;
            dateTime.Month = Month;
            dateTime.Day = Day;
            dateTime.Hour = Hour;
            dateTime.Minute = Minute;
            dateTime.Second = Second;
            dateTime.Format = Format;
        }
        public override string ToString()
        {
            return ToDateTime().ToString(Format);
        }
    }
}