using System;
using System.Collections.Generic;

namespace Kinesense.Interfaces
{
    public class Interval : IComparable<DateTime>, IComparable<Interval>
    {
        private DateTime _start;
        public DateTime Start 
        { 
            get { return _start; }
            set { _start = value; }
        }

        private DateTime _end;
        public DateTime End
        {
            get { return _end; }
            set { _end = value; }
        }

        public TimeSpan Duration { get { return _end - _start; } }

        public Interval(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
        }

        public Interval(DateTime start, TimeSpan duration)
        {
            _start = start;
            _end = _start + duration;
        }

        /*public Interval ToLocalTime()
        {
            return new Interval(_start.ToLocalTime(), _end.ToLocalTime());
        }

        public Interval ToUniversalTime()
        {
            return new Interval(_start.ToUniversalTime(), _end.ToUniversalTime());
        }*/

        public static explicit operator Interval(DateTime time)
        {
            return new Interval(time, time);
        }

        #region IComparable<DateTime> Members

        public int CompareTo(DateTime other)
        {
            if (other < _start)
                return 1;
            if (other > _end)
                return -1;
            return 0;
        }

        #endregion

        #region IComparable<Interval> Members

        public int CompareTo(Interval other)
        {
            if (other.End < _start)
                return 1;
            if (other.Start > _end)
                return 1;
            // the intervals overlap
            return 0;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj)) return false;
            return obj.GetType() == typeof(Interval) && this.Equals((Interval)obj);
        }

        public bool Equals(Interval other)
        {
            return other.Start.Equals(_start) && other.End.Equals(_end);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_start.GetHashCode() * 397) ^ _end.GetHashCode();
            }
        }

        public override string ToString()
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss.fffffff} - {1:yyyy-MM-dd HH:mm:ss.fffffff}", _start, _end);
        }

        public string ToString(string seperator)
        {
            return String.Format("{0:yyyy-MM-dd HH:mm:ss.fffffff}{2}{1:yyyy-MM-dd HH:mm:ss.fffffff}", _start, _end, seperator);
        }

        public static int CompareStartDate(Interval a, Interval b)
        {
            if (a.Start < b.Start)
                return -1;
            else if (a.Start > b.Start)
                return +1;
            else
                return 0;
        }

        /// <summary>
        /// returns true if the given time is within this interval
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool Contains(DateTime dt)
        {
            if (dt.CompareTo(this.Start) >= 0 && dt.CompareTo(this.End) <= 0)
                return true;
            else
                return false;
        }

        public bool Contains(Interval iv)
        {
            try
            {
                if (this.Contains(iv.Start) && this.Contains(iv.End))
                    return true;
                else
                    return false;
            }
            catch (Exception er)
            { Kinesense.Interfaces.DebugMessageLogger.LogError(er); }
            return false;
        }

        /// <summary>
        /// returns true if the given interval contains this interval
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        public bool IsWithin(Interval iv)
        {
            if (iv.Start.CompareTo(this.Start) <= 0 && iv.End.CompareTo(this.End) >= 0)
                return true;
            else
                return false;
                
        }

        public bool OverlapsWith(Interval interval)
        {
            if (interval.Start >= this.Start && interval.Start < this.End)
                return true;
            if (interval.End > this.Start && interval.End <= this.End)
                return true;
            if (this.Start >= interval.Start && this.Start < interval.End)
                return true;
            if (this.End > interval.Start && this.End <= interval.End)
                return true;

            else return false;
        }


        public Interval MergeWithAndReturnNew(Interval B)
        {
            Interval toRet;

            bool AStart = false;
            bool AEnd = false;

            if (this.Start <= B.Start)
                AStart = true;

            if (B.End <= this.End)
                AEnd = true;

            toRet = new Interval(AStart ? this.Start : B.Start, AEnd ? this.End : B.End);



            return toRet;
        }
    }
}
