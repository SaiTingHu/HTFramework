namespace HT.Framework
{
    /// <summary>
    /// 监控数据
    /// </summary>
    public struct MonitorData
    {
        /// <summary>
        /// 监控名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 消耗时间（秒）
        /// </summary>
        public float Elapsed;
        /// <summary>
        /// 堆内存增量（B）
        /// </summary>
        public long MemoryIncrement;
        /// <summary>
        /// 触发GC次数
        /// </summary>
        public int GCCount;

        public override string ToString()
        {
            long garbage = MemoryIncrement < 0 ? 0 : MemoryIncrement;
            string unit = "B";
            if (garbage > 1000f)
            {
                garbage = garbage / 1000;
                unit = "KB";
                if (garbage > 1000f)
                {
                    garbage = garbage / 1000;
                    unit = "MB";
                }
            }

            return string.Format("监控名称：{0}    消耗时间：{1}秒    产生的堆内存垃圾：{2}{3}    触发GC次数：{4}", Name, Elapsed, garbage, unit, GCCount);
        }
    }
}