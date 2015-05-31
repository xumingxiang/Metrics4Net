﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrics
{
    public static class PointMetricConfig
    {
        private static int queueMaxLength;
        /// <summary>
        /// 队列最大长度
        /// </summary>
        public static int QueueMaxLength
        {
            get
            {
                return queueMaxLength <= 0 ? 1000 : queueMaxLength;
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
                return batchSize <= 0 ? 50 : batchSize;
            }
            private set
            {
                batchSize = value;
            }
        }

        /// <summary>
        /// 阻塞时间，单位：毫秒
        /// </summary>
        public  static int BlockElapsed
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
