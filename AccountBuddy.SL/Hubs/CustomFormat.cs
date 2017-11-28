using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Account Group
        BLL.CustomFormat CustomFormatDAL_BLL(DAL.CustomFormat d)
        {
            BLL.CustomFormat b = d.toCopy<BLL.CustomFormat>(new BLL.CustomFormat());
            b.Company = d.CompanyDetail == null ? new BLL.CompanyDetail() : d.CompanyDetail.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
            return b;
        }
        public List<BLL.CustomFormat> CustomFormat_List()
        {
            return Caller.DB.CustomFormats.ToList()
                               .Select(x => CustomFormatDAL_BLL(x)).ToList();
        }

        public int CustomFormat_Save(BLL.CustomFormat agp)
        {
            try
            {
                agp.CompanyId = Caller.CompanyId;
                DAL.CustomFormat d = Caller.DB.CustomFormats.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.CustomFormat();
                    Caller.DB.CustomFormats.Add(d);

                    agp.toCopy<DAL.CustomFormat>(d);
                    Caller.DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.CustomFormat>(d);
                    Caller.DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).CustomFormat_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool CustomFormat_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = Caller.DB.CustomFormats.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    Caller.DB.CustomFormats.Remove(d);
                    Caller.DB.SaveChanges();
                    LogDetailStore(d.toCopy<BLL.CustomFormat>(new BLL.CustomFormat()), LogDetailType.DELETE);
                }
                else
                {
                    rv = false;
                }
                Clients.Clients(OtherLoginClientsOnGroup).CustomFormat_Delete(pk);
                Clients.All.delete(pk);
                rv = true;


            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }
 
        #endregion
    }
}