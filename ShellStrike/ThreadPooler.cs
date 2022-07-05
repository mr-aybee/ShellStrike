using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShellStrike
{
    public class ThreadPooler
    {

        Queue<object> Queue { get; set; }

        public event EventHandler<int> OnActiveThreadCountChanged;
        private int _ActiveThreadCount { get; set; }
        public int ActiveThreadCount
        {
            get => _ActiveThreadCount;
            set
            {
                _ActiveThreadCount = value;
                OnActiveThreadCountChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<int> OnQueueCountChanged;
        private int _QueueCount { get; set; }
        public int QueueCount
        {
            get => _QueueCount;
            set
            {
                _QueueCount = value;
                OnQueueCountChanged?.Invoke(this, value);
            }
        }

        private int _MaxThreadLimit { get; set; }
        public int MaxThreadLimit
        {
            get => _MaxThreadLimit;
            set => _MaxThreadLimit = value;
        }

        private bool _WaitOnDequeue { get; set; } = true;
        public bool WaitOnDequeue
        {
            get => _WaitOnDequeue;
            set
            {
                _WaitOnDequeue = value;
            }
        }

        private int _DequeueWait { get; set; } = 50;
        public int DequeueWait
        {
            get => _DequeueWait;
            set
            {
                _DequeueWait = value;
            }
        }

        Thread BGWorker { get; set; }

        public ThreadPooler(int maxThreadLimit)
        {
            Queue = new Queue<object>();
            MaxThreadLimit = maxThreadLimit;
        }

        public void AddToQueue(object obj)
        {
            Queue.Enqueue(obj);
            QueueCount = Queue.Count;
        }

        public void EmptyQueue(object obj)
        {
            Queue.Clear();
            QueueCount = Queue.Count;
        }

        public void AddRangeToQueue(object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                Queue.Enqueue(objs[i]);
            }
            QueueCount = Queue.Count;
        }

        public void StartExecution(Action<object> Method)
        {
            try
            {
                BGWorker = new Thread(() =>
                {
                    while (true)
                    {
                        if (WaitOnDequeue)
                            Thread.Sleep(DequeueWait);
                        if (ActiveThreadCount >= MaxThreadLimit || QueueCount == 0)
                        {
                            Thread.Sleep(700);
                            continue;
                        }
                        var obj = Queue.Dequeue();
                        QueueCount = Queue.Count;
                        ActiveThreadCount++;
                        new Thread(() =>
                        {
                            Method(obj);
                            ActiveThreadCount--;
                        }).Start();
                    }
                });
                BGWorker.Start();
            }
            catch (Exception t) { throw t; }
        }

        public void Abort()
        {
            if (BGWorker.IsAlive)
                BGWorker.Abort();
        }

    }
}
