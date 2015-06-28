using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Metrics
{
    /// <summary>
    /// 多线程消费队列。将输入元素打包输出。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TimerBatchBlock<T>
    {
        /// <summary>
        /// 当前队列长度
        /// </summary>
        public int CurrentQueueLength { get; private set; }

        /// <summary>
        /// Queue中丢弃的数量
        /// </summary>
        public int Lost;

        private Task[] Tasks { get; set; }

        /// <summary>
        /// 打包出队的处理函数
        /// </summary>
        private Action<List<T>> BatchAction { get; set; }

        private ConcurrentQueue<T> s_Queue;

        /// <summary>
        /// 阻塞队列的最大长度
        /// </summary>
        public int QueueMaxLength { get; private set; }

        public ConcurrentQueue<T> Batch { get; set; }

        /// <summary>
        /// 元素包的大小
        /// </summary>
        private int BatchSize { get; set; }

        /// <summary>
        /// 上一次打包处理的时间
        /// </summary>
        private DateTime LastActionTime { get; set; }

        private int BlockElapsed { get; set; }

        /// <summary>
        /// 多线程消费队列
        /// </summary>
        /// <param name="taskNum">处理队列出队的线程数量</param>
        /// <param name="action">处理委托</param>
        /// <param name="queueMaxLength">设置队列最大长度</param>
        /// <param name="batchSize">元素包的大小</param>
        /// <param name="blockElapsed">阻塞的时间，达到该时间间隔，也会出队.单位：毫秒</param>
        public TimerBatchBlock(int taskNum, Action<List<T>> action, int queueMaxLength, int batchSize, int blockElapsed)
        {
            if (queueMaxLength < batchSize)
            {
                throw new ArgumentException("batchSize必须是不大于queueMaxLength的int型整数", "batchSize");
            }

            if (blockElapsed <= 0)
            {
                throw new ArgumentException("blockElapsed必须是大于0的int型整数", "blockElapsed");
            }
            s_Queue = new ConcurrentQueue<T>();
            Batch = new ConcurrentQueue<T>();
            this.LastActionTime = DateTime.Now;
            this.BatchSize = batchSize;
            this.BlockElapsed = blockElapsed;
            this.BatchAction = action;
            this.QueueMaxLength = queueMaxLength;
            this.Tasks = new Task[taskNum];
            for (int i = 0; i < taskNum; i++)
            {
                int temp_i = i;
                this.Tasks[temp_i] = Task.Factory.StartNew(this.DequeueProcess);
            }
        }

        /// <summary>
        /// 入队处理
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            int queueLen = s_Queue.Count;
            this.CurrentQueueLength = queueLen;
            if (queueLen >= this.QueueMaxLength)
            {
                //大于最大长度，扔掉
                for (int i = 0; i < (queueLen - this.QueueMaxLength) + 1; i++)
                {
                    T removedItem;
                    this.s_Queue.TryDequeue(out removedItem);
                    Interlocked.Increment(ref Lost);
                }
            }

            this.s_Queue.Enqueue(item);
        }

        /// <summary>
        /// 出队处理函数
        /// </summary>
        private void DequeueProcess()
        {
            while (true)
            {
                try
                {
                    T item;
                    bool hasItem = s_Queue.TryDequeue(out item);
                    if (hasItem)
                    {
                        this.Batch.Enqueue(item);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }

                    var _now = DateTime.Now;
                    var elapsed = (_now - this.LastActionTime).TotalMilliseconds;

                    int _batchSize = 0;
                    bool execBatchAction = false;
                    if (this.Batch.Count >= this.BatchSize)
                    {
                        _batchSize = this.BatchSize;
                        execBatchAction = true;
                    }
                    else if (this.Batch.Count > 0 && (elapsed > this.BlockElapsed))
                    {
                        _batchSize = this.Batch.Count;
                        execBatchAction = true;
                    }
                    if (execBatchAction)
                    {
                        this.BatchAction(this.Batch.ToList());
                        this.LastActionTime = DateTime.Now;

                        for (int i = 0; i < _batchSize; i++)
                        {
                            T batchItem;
                            this.Batch.TryDequeue(out batchItem);
                        }
                    }

                    //var _now = DateTime.Now;
                    //var elapsed = (_now - this.LastActionTime).TotalMilliseconds;
                    //if (this.Batch.Count > 0 && (this.Batch.Count >= this.BatchSize || elapsed > this.BlockElapsed))
                    //{
                    //    this.BatchAction(this.Batch.ToList());
                    //    this.Batch = new ConcurrentBag<T>();
                    //    this.LastActionTime = DateTime.Now;
                    //}
                }
                catch (ThreadAbortException tae)
                {
                    Thread.ResetAbort();
                    //do exception...
                }
                catch (Exception ex)
                {
                    //do exception...
                }
            }
        }
    }
}