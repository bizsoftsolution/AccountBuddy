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
        BLL.StockGroup StockGroup_DALtoBLL(DAL.StockGroup d)
        {
            BLL.StockGroup b = d.ToMap(new BLL.StockGroup());
            b.Company = d.CompanyDetail.ToMap(new BLL.CompanyDetail());
            b.UnderStockGroup = d.StockGroup2 == null ? new BLL.StockGroup() : StockGroup_DALtoBLL(d.StockGroup2);
            return b;
        }
        public List<BLL.StockGroup> StockGroup_List()
        {
            
            if (Caller.CompanyType == "Company")
            {
                return DB.StockGroups.Where(x => x.CompanyId == Caller.CompanyId).ToList()
                              .Select(x => StockGroup_DALtoBLL(x)).ToList();
            }
            else 
            {
                return DB.StockGroups.Where(x => x.CompanyId == Caller.UnderCompanyId).ToList()
                              .Select(x => StockGroup_DALtoBLL(x)).ToList();
            }
           
        }
        
        public int StockGroup_Save(BLL.StockGroup b)
        {
            try
            {
                b.CompanyId = Caller.CompanyId;
                DAL.StockGroup d = DB.StockGroups.Where(x => x.Id == b.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.StockGroup();
                    DB.StockGroups.Add(d);

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

                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).StockGroup_Save(b);

                return b.Id;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return 0;
        }

        public bool StockGroup_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.StockGroups.Where(x => x.Id == pk).FirstOrDefault();
                if (d.Products != null && StockGroup_CanDelete(d))
                {
                    if (d != null)
                    {
                        DB.StockGroups.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(d.ToMap(new BLL.StockGroup()), LogDetailType.DELETE);
                    }

                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).SStockGroup_Delete(pk);
                    Clients.All.delete(pk);
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

        public bool StockGroup_CanDelete(DAL.StockGroup l)
        {
            return l.Products.Count() == 0;

        }
        #endregion
    }
}