using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces.Threading
{
    public class ConcurrentStringList
    {
        object _object = new object();
        List<string> _list = new List<string>();

        public void Add(string s)
        {
            if (s == null)
                return;
            lock (_object)
            {
                _list.Add(s.ToLowerInvariant());
            }
        }

        public void Remove(string s)
        {
            if (s == null)
                return;

            string sl = s.ToLowerInvariant();
            lock (_object)
            {

                if(_list.Contains(sl))
                    _list.Remove(sl);
            }
        }

        public bool Contains(string s)
        {
            string tocompare = s.ToLowerInvariant();
            lock (_object)
            {
                //foreach (var i in _list)
                //    if (i.Contains(s))
                //        return true;
                return _list.Contains(tocompare);
            }
            return false;
        }
    }
}
