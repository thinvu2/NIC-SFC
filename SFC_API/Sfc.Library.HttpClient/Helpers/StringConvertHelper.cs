using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.Library.HttpClient.Helpers
{
    public static class StringConvertHelper
    {
        public static string EncodeToBase64(string value)
        {
            var toEncodeAsBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(toEncodeAsBytes);
        }
    }
}
