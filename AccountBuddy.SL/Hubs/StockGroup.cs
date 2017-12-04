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
            BLL.StockGroup b = d.toCopy<BLL.StockGroup>(new BLL.StockGroup());
            b.Company = d.CompanyDetail.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
            b.UnderStockGroup = d.StockGroup2 == null ? new BLL.StockGroup() : StockGroup_DALtoBLL(d.StockGroup2);
            return b;
        }
        public List<BLL.StockGroup> StockGroup_List()
        {
            if (Caller.CompanyType == "Company")
            {
                return Caller.DB.StockGroups.Where(x => x.CompanyId == Caller.CompanyId).ToList()
                              .Select(x => StockGroup_DALtoBLL(x)).ToList();
            }
            else if (Caller.CompanyType == "Warehouse")
            {
                return Caller.DB.StockGroups.Where(x => x.CompanyId == Caller.UnderCompanyId).ToList()
                              .Select(x => StockGroup_DALtoBLL(x)).ToList();
            }
            else
            {
                List<BLL.StockGroup> c = new List<BLL.StockGroup>();
                var wh = Caller.DB.CompanyDetails.Where(x => x.Id == Caller.UnderCompanyId).FirstOrDefault();
                if (wh != null)
                {
                    c = Caller.DB.StockGroups.Where(x => x.CompanyId == wh.UnderCompanyId).ToList()
                            .Select(x => StockGroup_DALtoBLL(x)).ToList();
                }
                return c;
            }

        }
        public List<BLL.StockGroup> StockGroup_PrimaryList()
        {

            List<BLL.StockGroup> rv = new List<BLL.StockGroup>();
            BLL.StockGroup stg = new BLL.StockGroup();
            var lst = Caller.DB.StockGroups.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var id = Caller.DB.StockGroups.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            foreach (var i in id)
            {
                foreach (var l in lst.Where(x=>x.UnderGroupId!=i.Id).ToList())

                {
                    rv.toCopy<DAL.StockGroup>(l);
                 
                }

            }
            return rv;
        }

        public int StockGroup_Save(BLL.StockGroup agp)
        {
            try
            {
                agp.CompanyId = Caller.CompanyId;
                DAL.StockGroup d = Caller.DB.StockGroups.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.StockGroup();
                    Caller.DB.StockGroups.Add(d);

                    agp.toCopy<DAL.StockGroup>(d);
                    Caller.DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.StockGroup>(d);
                    Caller.DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClients).StockGroup_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool StockGroup_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = Caller.DB.StockGroups.Where(x => x.Id == pk).FirstOrDefault();
                if (d.Products != null && StockGroup_CanDelete(d))
                {
                    if (d != null)
                    {
                        Caller.DB.StockGroups.Remove(d);
                        Caller.DB.SaveChanges();
                        LogDetailStore(d.toCopy<BLL.StockGroup>(new BLL.StockGroup()), LogDetailType.DELETE);
                    }

                    Clients.Clients(OtherLoginClients).SStockGroup_Delete(pk);
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