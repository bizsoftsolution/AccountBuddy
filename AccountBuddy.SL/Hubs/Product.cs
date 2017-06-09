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

            ProductsTo.UOM = ProductsFrom.UOM == null ? null: UOM_DALtoBLL(ProductsFrom.UOM);
            ProductsTo.POQty = ProductsFrom.PurchaseOrderDetails.Sum(x => x.Quantity);
            ProductsTo.PQty = ProductsFrom.PurchaseDetails.Sum(x => x.Quantity);
            ProductsTo.PRQty = ProductsFrom.PurchaseReturnDetails.Sum(x => x.Quantity);
            ProductsTo.SOQty = ProductsFrom.SalesOrderDetails.Sum(x => x.Quantity);
            ProductsTo.SQty = ProductsFrom.SalesDetails.Sum(x => x.Quantity);
            ProductsTo.SRQty= ProductsFrom.SalesReturnDetails.Sum(x => x.Quantity);
            return ProductsTo;
        }

        public List<BLL.Product> Product_List()
        {
            return DB.Products.Where(x => x.StockGroup.CompanyDetail.Id == Caller.CompanyId).ToList()
                             .Select(x => Product_DALtoBLL(x)).ToList();
        }
        
        public BLL.Product Product_Save(BLL.Product pro)
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
                }
                if (d.Id != 0)
                {
                    var p = Product_DALtoBLL(d);
                    Clients.Clients(OtherLoginClientsOnGroup).Product_Save(p);

                    return p;
                }
                
            }
            catch (Exception ex) { }
            return new BLL.Product();
        }

        public bool Product_Delete(int pk)
        {
            var rv = false;
            try
            {
                var d = DB.Products.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null )
                {
                    var p = Product_DALtoBLL(d);
                    DB.Products.Remove(d);
                    //Ledger_Delete((int)d.LedgerId);
                    DB.SaveChanges();
                    LogDetailStore(p, LogDetailType.DELETE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Customer_Delete(pk);

                rv = true;

            }
            catch (Exception ex)
            {
                rv = false;
            }
            return rv;
        }

        public bool Product_CanDelete(DAL.Product p)
        {
            bool rv = (p == null) ? false : p.PurchaseOrderDetails.Count() == 0 &&
                   p.PurchaseDetails.Count() == 0 &&
                   p.PurchaseReturnDetails.Count() == 0 &&
                   p.SalesOrderDetails.Count() == 0 &&
                   p.SalesDetails.Count() == 0 && 
                   p.SalesReturnDetails.Count()==0;

            return rv;
        }

        public bool Product_CanDeleteById(int Id)
        {
            return Product_CanDelete(DB.Products.Where(x => x.Id == Id).FirstOrDefault());
        }

        #endregion
    }
}