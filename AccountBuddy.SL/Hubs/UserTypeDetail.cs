using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {        
        private BLL.UserTypeDetail UserTypeDetailDAL_BLL(DAL.UserTypeDetail d)
        {
            BLL.UserTypeDetail b = d.toCopy<BLL.UserTypeDetail>(new BLL.UserTypeDetail());
            b.UserTypeFormDetail = d.UserTypeFormDetail.toCopy<BLL.UserTypeFormDetail>(new BLL.UserTypeFormDetail());
            return b;
        }                
    }
}
