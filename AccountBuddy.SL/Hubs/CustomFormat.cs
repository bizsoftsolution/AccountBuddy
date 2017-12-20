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
            return DB.CustomFormats.ToList()
                               .Select(x => CustomFormatDAL_BLL(x)).ToList();
        }

        public int CustomFormat_Save(BLL.CustomFormat agp)
        {
            try
            {
                agp.CompanyId = Caller.CompanyId;
                DAL.CustomFormat d = DB.CustomFormats.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.CustomFormat();
                    DB.CustomFormats.Add(d);

                    agp.toCopy<DAL.CustomFormat>(d);
                    DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.CustomFormat>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).CustomFormat_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return 0;
        }

        public bool CustomFormat_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.CustomFormats.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    DB.CustomFormats.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(d.toCopy<BLL.CustomFormat>(new BLL.CustomFormat()), LogDetailType.DELETE);
                }
                else
                {
                    rv = false;
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).CustomFormat_Delete(pk);
               
                rv = true;


            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                rv = false;
            }
            return rv;
        }
 
        #endregion
    }
}