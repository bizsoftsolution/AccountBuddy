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
        BLL.UOM UOM_DALtoBLL(DAL.UOM d)
        {
            BLL.UOM b = d.ToMap(new BLL.UOM());
            b.Company = d.CompanyDetail.ToMap(new BLL.CompanyDetail());
            return b;
        }
        public List<BLL.UOM> UOM_List()
        {
            
            if (Caller.CompanyType == "Company")
            {
                return DB.UOMs.Where(x => x.CompanyId == Caller.CompanyId).ToList()
                              .Select(x => UOM_DALtoBLL(x)).ToList();
            }

            else
            {
                return DB.UOMs.Where(x => x.CompanyId == Caller.UnderCompanyId).ToList()
                              .Select(x => UOM_DALtoBLL(x)).ToList();
            }

        }

        public int UOM_Save(BLL.UOM b)
        {

            
            try
            {
                b.CompanyId = Caller.CompanyId;
                DAL.UOM d = DB.UOMs.Where(x => x.Id == b.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.UOM();
                    DB.UOMs.Add(d);

                    b.ToMap(d);
                    DB.SaveChanges();

                    b.Id = d.Id;
                    LogDetailStore(b, LogDetailType.INSERT);
                }
                else
                {
                    b.ToMap(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.UPDATE);
                }

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).UOM_Save(b);
                return b.Id;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return 0;
        }

        public bool UOM_Delete(int pk)
        {
            
            var rv = false;
            try
            {
                var d = DB.UOMs.Where(x => x.Id == pk).FirstOrDefault();
                if (d.Products != null)
                {
                    if (d != null)
                    {
                        DB.UOMs.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(d.ToMap(new BLL.UOM()), LogDetailType.DELETE);
                    }

                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).UOM_Delete(pk);


                    rv = true;
                }
                else
                {
                    rv = false;
                }

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