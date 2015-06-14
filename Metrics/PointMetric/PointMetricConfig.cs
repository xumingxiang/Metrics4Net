namespace Metrics
{
    public static class PointMetricConfig
    {
        private const int Default_QueueMaxLength = 2000;
        private const int Default_BatchSize = 100;

        private static int queueMaxLength;

        /// <summary>
        /// 队列最大长度
        /// </summary>
        public static int QueueMaxLength
        {
            get
            {
                return queueMaxLength <= 0 ? Default_QueueMaxLength : queueMaxLength;
            }
            private set
            {
                queueMaxLength = value;
            }
        }

        private static int batchSize;

        /// <summary>
        /// 批量包大小
        /// </summary>
        public static int BatchSize
        {
            get
            {
                return batchSize <= 0 ? Default_BatchSize : batchSize;
            }
            private set
            {
                batchSize = value;
            }
        }

        /// <summary>
        /// 阻塞时间，单位：毫秒
        /// </summary>
        public static int BlockElapsed
        {
            get;
            private set;
        }

        public static void SetConfig(int queueMaxLength = 0, int batchSize = 0, int blockElapsed = 0)
        {
            QueueMaxLength = queueMaxLength;
            BatchSize = batchSize;
            BlockElapsed = blockElapsed;
        }
    }
}