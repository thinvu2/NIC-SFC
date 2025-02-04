using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.Library.HttpClient.Helpers
{
    public static class DictionaryExtensions
    {
        public static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            return (T)dictionary[key];
        }

        public static bool TryGet<T>(this IDictionary<string, object> dictionary,
                                     string key, out T value)
        {
            object result;
            if (dictionary.TryGetValue(key, out result) && result is T)
            {
                value = (T)result;
                return true;
            }
            value = default(T);
            return false;
        }

        public static void Set(this IDictionary<string, object> dictionary,
                               string key, object value)
        {
            dictionary[key] = value;
        }
    }
}
