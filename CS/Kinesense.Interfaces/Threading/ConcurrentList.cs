using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Threading
{
    /// <summary>
    /// a wrapped concurrent bag but with 'remove'
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentList<T>
    {
        public ConcurrentList()
        {
            Bag = new ConcurrentBag<T>();
        }

        public ConcurrentBag<T> Bag { get; set; }
        public int Count { get { return Bag.Count; } }

        public void Add(T t)
        {
            Bag.Add(t);
        }

        object _lockObject = new object();

        public void Remove(T t)
        {
            lock(_lockObject)
                Bag = new ConcurrentBag<T>(Bag.Except(new[] { t }));
        }

        public bool Contains(T t)
        {
            foreach(var v in Bag)
                if(v.ToString() == t.ToString())
                {
                    return true;
                }

            return false;
        }
    }    
    
}
