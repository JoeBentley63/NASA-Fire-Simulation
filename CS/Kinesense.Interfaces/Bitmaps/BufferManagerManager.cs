using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Kinesense.Interfaces.Classes
{
    public class BufferManagerManager
    {
        public static bool BufferManagerOn = false;

        public double GetBufferMemoryUse()
        {
            double total = 0;
            foreach (var b in _bufferManagers)
            {
                total += b.Value.TotalMemoryUse;
            }
            return total;
        }

        public double ProcessMemoryLoad()
        {
            var p = System.Diagnostics.Process.GetCurrentProcess();

            double memlim = Environment.Is64BitProcess ? Int64.MaxValue : 1500000000;
            double privateBytesUsed = p.PrivateMemorySize64;
            double GCMem = GC.GetTotalMemory(false);

            double totalMemLimit = ((privateBytesUsed + GCMem) * 100) / memlim;

            return totalMemLimit;
        }

        Dictionary<long, KinesenseBufferManager> _bufferManagers = new Dictionary<long, KinesenseBufferManager>();
        List<long> Sizes = new List<long>();

        static object lockObj = new object();

        public int StandardBufferCount = 4;

        private void PopulateSizes()
        {
            Sizes.Add(65536);
            Sizes.Add(524288);
            Sizes.Add(1048576);

            foreach (long s in Sizes)
                _bufferManagers.Add(s, new KinesenseBufferManager(StandardBufferCount, 10, (int)s));

            _bufferManagers[(long)65536].MaxBuffers = 35;
        }

        public enum BufferNeededFor { Allocation, Deallocation };

        public long GetClosestBuffer(long size, BufferNeededFor neededfor)
        {
            if (neededfor == BufferNeededFor.Allocation)
            {
                foreach (long s in Sizes)
                    if (s >= size)
                        return s;

                if (size > 10000000)
                {
                    Sizes.Add(size);
                    _bufferManagers.Add(size, new KinesenseBufferManager(1, 4, (int)size));
                }
                else
                {
                    size = (long)(size * 1.3d);

                    Sizes.Add(size);
                    _bufferManagers.Add(size, new KinesenseBufferManager(StandardBufferCount, 10, (int)size));

                    TestBuffersAndDropOldOnes();
                }
            }
            else
            {
                foreach (long s in Sizes.Reverse<long>())
                    if (s <= size)
                        return s;
                size = -1; //no buffer available
            }

            return size;

        }

        void TestBuffersAndDropOldOnes()
        {
            try
            {
                foreach (var buf in _bufferManagers)
                {
                    if ((DateTime.Now - buf.Value.MostRecentUse).TotalMinutes > 10)
                        buf.Value.DropBuffers();
                }
            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }
        }

        static int _testMemoryFrequency = 100;

        static int _countAllocations = 0;

        static int _bufferSizeTestThreshold = 1000000;


        public byte[] GetBuffer(long length)
        {
            if (!BufferManagerOn)
                return new byte[length];

            byte[] buffer = null;

            if (Sizes.Count == 0)
                PopulateSizes();

            try
            {
                if (length > _bufferSizeTestThreshold)
                {
                    _countAllocations++;
                    if (_countAllocations % _testMemoryFrequency == 0)
                    {
                        double d = this.ProcessMemoryLoad();
                        if (d > 50)
                        {
                            DebugMessageLogger.LogEvent("BufferManager. Process Memory Load is {0}%, introducing a short sleep", d);
                            Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep((int)d);
                        }
                    }
                }

                long size = GetClosestBuffer(length, BufferNeededFor.Allocation);

                buffer = _bufferManagers[size].TakeBuffer(50);

                //foreach(byte b in buffer)
                //    if (b != 0)
                //    {
                //    }

            }
            catch (Exception ee)
            {
                DebugMessageLogger.LogError(ee);
            }

            return buffer;
        }

        public byte[] ReturnBuffer(byte[] buf)
        {
            if (!BufferManagerOn)
            {
                buf = null;
                return null;
            }

            long buflen = buf.Length;

            long size = GetClosestBuffer(buflen, BufferNeededFor.Deallocation);
            _bufferManagers[size].ReturnBuffer(buf);

            return null;
        }
    }
}
