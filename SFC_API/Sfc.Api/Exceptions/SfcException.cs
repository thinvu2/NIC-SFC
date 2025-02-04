using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sfc.Api
{
    public class SfcException : Exception
    {
        public SfcException(string message) : base(message) { }
        public SfcException(string result, string message, string data) : base(message)
        {

        }
    }
}