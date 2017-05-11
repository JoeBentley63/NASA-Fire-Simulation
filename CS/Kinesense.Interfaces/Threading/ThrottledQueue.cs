using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kinesense.Interfaces.Threading
{
    public class ThrottledQueue<T> : ConcurrentQueue<T>, IThrottledQueue
    {
        public ThrottledQueue()
        {
            ThrottleSleepRule = ThrottleSleepRule.Variable;
            MinimumSleepDuration = 1;
            MaximumSleepDuration = 1000;
            SleepDurationScalingFactor = 1;
            DelayOnExcessiveQueueLength = true;
        }

        private int _weighting = 1;
        public int Weighting
        {
            get { return _weighting; }
            set { _weighting = value; }
        }

        public int WeightedQueueLength
        {
            get { return _weighting * this.Count; }
        }

        public bool DelayOnExcessiveQueueLength { get; set; }

        private int _WeightedQueueLengthThreshold = 100;
        public int WeightedQueueLengthThreshold
        {
            get { return _WeightedQueueLengthThreshold; }
            set { _WeightedQueueLengthThreshold = value; }
        }

        private int _sampleFrequency = 1;
        public int SampleFrequency
        {
            get { return _sampleFrequency; }
            set { _sampleFrequency = value; }
        }

        public bool MonitoredTryDequeue(out T t)
        {
            bool success = false;

             success = TryDequeue(out t);
            _dequeueEvent.Set();
            
            return success;
        }

        public void ThrottledEnqueue(T t)
        {
            while (WaitForDequeue && this.Count >= MaxQueueLength)
                _dequeueEvent.WaitOne();

            this.Enqueue(t);
            _countEnqueues++;
            
            TestQueueLength();
        }

        public ThrottleSleepRule ThrottleSleepRule { get; set; }

        public int MinimumSleepDuration { get; set; }
        public int MaximumSleepDuration { get; set; }

        public float SleepDurationScalingFactor { get; set; }

        List<IThrottledQueue> _LinkedQueues = new List<IThrottledQueue>();
        public List<IThrottledQueue> LinkedQueues { get { return _LinkedQueues; } }

        int _countEnqueues = 0;
        private void TestQueueLength()
        {
            if(_countEnqueues % _sampleFrequency == 0)
            {
                int sleeptime = GetSleepTime();

                if (LinkedQueues.Count > 0)
                    foreach (var lq in LinkedQueues)
                        sleeptime += lq.GetSleepTime();
                
                if (sleeptime > 0)
                    Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(sleeptime);
            }

        }

        public int GetSleepTime()
        {
            int sleeptime = MinimumSleepDuration;
            if (WeightedQueueLength > WeightedQueueLengthThreshold)
            {
                if (ThrottleSleepRule == ThrottleSleepRule.Variable)
                {
                    sleeptime = MinimumSleepDuration + (int)((WeightedQueueLength - WeightedQueueLengthThreshold) * SleepDurationScalingFactor);
                    if (sleeptime > MaximumSleepDuration)
                        sleeptime = MaximumSleepDuration;
                    if (sleeptime < MinimumSleepDuration)
                        sleeptime = MinimumSleepDuration;
                }
            }
            else
                sleeptime = 0;

            return sleeptime;
        }

        public bool WaitForDequeue { get; set; } = false;
        public int MaxQueueLength { get; set; } = 10;
        AutoResetEvent _dequeueEvent = new AutoResetEvent(false);

    }

    public enum ThrottleSleepRule
    {
        FixedDurationSleep, Variable
    }

    public interface IThrottledQueue
    {
        int GetSleepTime();
    }
}
