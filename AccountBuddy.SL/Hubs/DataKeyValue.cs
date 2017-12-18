using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        private BLL.DataKeyValue DataKeyValueDAL_BLL(DAL.DataKeyValue DataKeyValueFrom)
        {
            BLL.DataKeyValue DataKeyValueTo = DataKeyValueFrom.toCopy<BLL.DataKeyValue>(new BLL.DataKeyValue());            
            return DataKeyValueTo;
        }

        public List<BLL.DataKeyValue> DataKeyValue_List()
        {

            var l1 = DB.DataKeyValues.Where(x => x.CompanyId == Caller.CompanyId).ToList()
                             .Select(x => DataKeyValueDAL_BLL(x)).ToList();
            return l1;
        }
    }
}