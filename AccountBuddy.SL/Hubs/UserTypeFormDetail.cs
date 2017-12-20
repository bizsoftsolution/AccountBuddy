using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        public List<BLL.UserTypeFormDetail> UserTypeFormDetail_List()
        {
            return DB.UserTypeFormDetails.Where(x => x.IsActive == true)
                                         .OrderBy(x => x.OrderNo).ToList()
                                         .Select(x => x.ToMap(new BLL.UserTypeFormDetail()))
                                         .ToList(); 
        }
    }
}
