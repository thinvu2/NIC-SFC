using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.Core.Entities
{
    public class RefreshTokenEntity : BaseEntity
    {

        public string Id { get; set; }
        public string Subject { get; set; }
        public string ClientId { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
        public string ProtectedTicket { get; set; }
    }
}
