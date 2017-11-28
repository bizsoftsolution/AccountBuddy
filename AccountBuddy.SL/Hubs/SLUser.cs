using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public class SLUser
    {
        private DAL.DBFMCGEntities _db;

        public string ConnectionId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public int StaffId { get; set; }
        public double HierarchicalOrderNo { get; set; }
        public bool IsAdmin { get; set; }
        public int? UnderCompanyId { get; set; }
        public string CompanyType { get; set; }
        public string AccYear { get; set; }

        public DAL.DBFMCGEntities DB
        {
            get
            {
                if (_db == null)
                {
                    _db = new DAL.DBFMCGEntities();
                }
                return _db;
            }

            set
            {
                _db = value;
            }
        }


    }
}
