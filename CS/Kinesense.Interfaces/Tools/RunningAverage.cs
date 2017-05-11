using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    public class RunningAverage : Queue<double>
    {
        public double GetAverage()
        {
            double avg = 0;
            foreach (double d in this)
                avg += d;

            return avg / this.Count;
        }

        public void Add(double d)
        {
            if (this.Count >= this.Capacity)
                this.Dequeue();
            this.Enqueue(d);
        }

        public int Capacity { get; set; }

        public RunningAverage(int capacity)
            : base(capacity)
        {
            this.Capacity = capacity;
        }
    }
}
