using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.Library.HttpClient.V2.Logging
{
    public enum SecureLevel
    {
        ALL = 0x11,

        SEE_ONLY = 0x01,
        SET_ONLY = 0x10,

        NONE = 0x00
    }
}
