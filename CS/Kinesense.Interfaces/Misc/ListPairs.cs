using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinesense.Interfaces
{
    /// <summary>
    /// shorthand class for List<KeyValuePair>. This is almost a Dictionary without restrictions on identical keys
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class ListPairs<T, K> : List<KeyValuePair<T, K>>
    {
        public void Add(T t, K k)
        {
            this.Add(new KeyValuePair<T, K>(t, k));
        }
    }
}
