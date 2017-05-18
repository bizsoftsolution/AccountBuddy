using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public class SLUser
    {
        public string ConnectionId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string AccYear { get; set; }
    }
}
