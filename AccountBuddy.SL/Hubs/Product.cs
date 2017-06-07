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

        private BLL.Product Product_DALtoBLL(DAL.Product ProductsFrom)
        {
            BLL.Product ProductsTo = ProductsFrom.toCopy<BLL.Product>(new BLL.Product());

            ProductsTo.StockGroup = StockGroup_DALtoBLL(ProductsFrom.StockGroup);

            ProductsTo.UOM = ProductsTo.UOM == null ? null: UOM_DALtoBLL(ProductsFrom.UOM);

            return ProductsTo;
        }

        public List<BLL.Product> Product_List()
        {
            return DB.Products.Where(x => x.StockGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Product_DALtoBLL(x)).ToList();
        }
        
        public int Product_Save(BLL.Product pro)
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
                    //Ledger_Save(pro.Ledger);
                    DB.SaveChanges();

                    Clients.Clients(OtherLoginClientsOnGroup).Product_Save(pro);

                    return pro.Id = d.Id;
                }
            }
            catch (Exception ex) { }
            return 0;
        }

        public bool Product_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Products.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null )
                {
                    DB.Products.Remove(d);
                    //Ledger_Delete((int)d.LedgerId);
                    DB.SaveChanges();
                    LogDetailStore(Product_DALtoBLL(d), LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Customer_Delete(pk);
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