using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictonaryExtentions
{ 
    public static class DictonaryExtentions
    {
        public static void JustAdd<TDictionary, TKey, TValue>(this TDictionary dic, TKey key, TValue value) where TDictionary : Dictionary<TKey, HashSet<TValue>>
        {
            HashSet<TValue> grouping;
            if (!dic.TryGetValue(key, out grouping))
            {
                grouping = new HashSet<TValue>() { value };
                dic.Add(key, grouping);
            }
            grouping.Add(value);
        }
    }
}
