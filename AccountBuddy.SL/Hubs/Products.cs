using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region Products

        private BLL.Products ProductsDAL_BLL(DAL.Product ProductsFrom)
        {
            BLL.Products ProductsTo = ProductsFrom.toCopy<BLL.Products>(new BLL.Products());

            ProductsTo.AccountGroup = AccountGroupDAL_BLL(ProductsFrom.AccountGroup);

            ProductsTo.UOM = UOMDAL_BLL(ProductsFrom.UOM);

            return ProductsTo;
        }

        public List<BLL.Products> Products_List()
        {
            return DB.Products.Where(x => x.AccountGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => ProductsDAL_BLL(x)).ToList();
        }

       

        public int Products_Save(BLL.Products pro)
        {
            try
            {
                DAL.Product d = DB.Products.Where(x => x.Id == pro.Id).FirstOrDefault();
                if (d == null)
                {
                    d = new DAL.Product();
                    DB.Products.Add(d);

                    pro.toCopy<DAL.Product>(d);

                    DB.SaveChanges();
                    pro.Id = d.Id;

                    LogDetailStore(pro, LogDetailType.INSERT);
                }
                else
                {
                    pro.toCopy<DAL.Product>(d);
                    DB.SaveChanges();
                    LogDetailStore(pro, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Products_Save(pro);

                return pro.Id = d.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Products_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Products.Where(x => x.Id == pk).FirstOrDefault();
               
                    if (d != null)
                    {
                        DB.Products.Remove(d);
                        DB.SaveChanges();
                        LogDetailStore(ProductsDAL_BLL(d), LogDetailType.DELETE);
                    }

                    Clients.Clients(OtherLoginClientsOnGroup).Products_Delete(pk);
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