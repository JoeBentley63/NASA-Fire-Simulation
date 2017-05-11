using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace Kinesense.Interfaces.Classes
{

    public class KinesenseBufferManager
    {
        public KinesenseBufferManager(int startingBuffers, int maxBuffers, int bufferSize)
        {
            MostRecentUse = DateTime.Now;
            BufferSize = bufferSize;
            BufferCount = startingBuffers;
            MaxBuffers = maxBuffers;

            for (int i = 0; i < BufferCount; i++)
                _buffers.Enqueue(new byte[bufferSize]);
        }

        public int MaxBuffers
        {
            get;
            set;
        }

        public int BufferCount
        {
            get;
            private set;
        }

        public int BufferSize
        {
            get;
            private set;
        }

        public int BuffersLeft
        {
            get { return _buffers.Count; }
        }

        public long TotalMemoryUse
        {
            get { return BufferSize * BufferCount; }
        }

        static int _waiting = 0;

        object _countlock = new object();
        int _ticketCount = 0;
        int _usedTickets = 0;

        public DateTime MostRecentUse { get; private set; }

        public void DropBuffers()
        {
            DebugMessageLogger.LogEvent("DropBuffers on {0} Dropping {1} Buffers", this.BufferSize, this.BufferCount);
            while (_buffers.Count > 0)
            {
                byte[] buf = null;
                _buffers.TryDequeue(out buf);
                buf = null;
            }
            GC.Collect();
        }

        public byte[] TakeBuffer(int timeout_ms)
        {

            byte[] bf = null;

            int ticket = -1;

            
            int waitcount = 0;

            while (!_buffers.TryDequeue(out bf))
            {
                System.Diagnostics.Debug.WriteLine("Waiting for Buffer {0}, wait count {1}", BufferSize, waitcount);

                if(ticket == -1)
                lock (_countlock)
                {
                    ticket = _ticketCount++;
                }
                
                _waiting++; 
                waitcount++;
                
                _event.WaitOne(timeout_ms);
                
                _waiting--;

                int diff = ticket - _usedTickets;
                if(diff > 0)
                    Kinesense.Interfaces.Threading.ThreadSleepMonitor.Sleep(diff);

                if ((waitcount > 3 && BufferCount < MaxBuffers)
                    || (waitcount > 10))
                {
                    
                    BufferCount++;
                    DebugMessageLogger.LogEvent("Waiting too long for buffer - making a new one of size {0} ({1}/{2})", this.BufferSize, this.BufferCount, this.MaxBuffers);
                    bf = new byte[this.BufferSize];
                    break;
                }
            }
            if (ticket != -1)
            {
                lock (_countlock)
                {
                    if (_usedTickets < ticket)
                        _usedTickets = ticket;
                }
            }

            MostRecentUse = DateTime.Now;

            Array.Clear(bf, 0, bf.Length);
            return bf;
        }

        static int CountDrops = 0;
        public void ReturnBuffer(byte[] buf)
        {
            if (_buffers.Count > MaxBuffers || buf.Length < this.BufferSize)
            {
                //release mem
                buf = null;
                _event.Set();
                CountDrops++;
                if (CountDrops % 10 == 0)
                    GC.Collect();
            }
            else
            {
                _buffers.Enqueue(buf);
                _event.Set();
            }
        }

        public override string ToString()
        {
            return string.Format("Buffer {0} of Size {1}. Available {2} Waiting {3}",
                BufferCount, BufferSize, BuffersLeft, _waiting);

        }

        ConcurrentQueue<byte[]> _buffers = new ConcurrentQueue<byte[]>();
        AutoResetEvent _event = new AutoResetEvent(false);
    }

    
}
