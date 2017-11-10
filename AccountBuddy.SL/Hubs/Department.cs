using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Department
        BLL.Department Department_DALtoBLL(DAL.Department d)
        {
            BLL.Department b = d.toCopy<BLL.Department>(new BLL.Department());
            b.Company = d.CompanyDetail.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
            return b;
        }
        public List<BLL.Department> Department_List()
        {
            if (Caller.CompanyType == "Warehouse")
            {
                return DB.Departments.Where(x => x.CompanyId == Caller.UnderCompanyId).ToList()
                              .Select(x => Department_DALtoBLL(x)).ToList();
            }
            else if (Caller.CompanyType == "Dealer")
            {
                var wh = DB.CompanyDetails.Where(x => x.Id == Caller.UnderCompanyId).FirstOrDefault();
                return DB.Departments.Where(x => x.CompanyId == wh.UnderCompanyId).ToList()
                              .Select(x => Department_DALtoBLL(x)).ToList();
            }
            else
            {
                return DB.Departments.Where(x => x.CompanyId == Caller.CompanyId).ToList()
                              .Select(x => Department_DALtoBLL(x)).ToList();
            }

        }

        public int Department_Save(BLL.Department agp)
        {
            try
            {
                agp.CompanyId = Caller.CompanyId;
                DAL.Department d = DB.Departments.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.Department();
                    DB.Departments.Add(d);

                    agp.toCopy<DAL.Department>(d);
                    DB.SaveChanges();

                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<DAL.Department>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Department_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Department_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Departments.Where(x => x.Id == pk).FirstOrDefault();                
                if (d != null)
                {
                    DB.Departments.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(d.toCopy<BLL.Department>(new BLL.Department()), LogDetailType.DELETE);

                    Clients.Clients(OtherLoginClientsOnGroup).SDepartment_Delete(pk);
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


        #endregion
    }
}