using AccountBuddy.BLL;
using AccountBuddy.Common;
using AccountBuddy.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Product_Spec_Process      
       
        public bool Product_Spec_Process_Save(BLL.Product_Spec_Process P)
        {
            try
            {

                DAL.Product_Spec_Process d = DB.Product_Spec_Process.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.Product_Spec_Process();
                    DB.Product_Spec_Process.Add(d);
                    P.ToMap(d);
                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.Product_Spec_Process_Detail d_pod = new DAL.Product_Spec_Process_Detail();
                        b_pod.ToMap(d_pod);
                        d.Product_Spec_Process_Detail.Add(d_pod);
                    }
                    DB.SaveChanges();
                    P.Id = d.Id;
                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    //foreach (var d_Pd in d.PurchaseDetails.ToList())
                    //{
                    //    BLL.PurchaseDetail b_Pd = P.PDetails.Where(x => x.Id == d_Pd.Id).FirstOrDefault();
                    //    if (b_Pd == null) d.PurchaseDetails.Remove(d_Pd);
                    //}

                    decimal rd = P.PDetails.Select(X => X.PSId).FirstOrDefault();
                    DB.Product_Spec_Process_Detail.RemoveRange(d.Product_Spec_Process_Detail.Where(x => x.PSId == rd).ToList());

                    P.ToMap(d);
                    foreach (var b_Pd in P.PDetails)
                    {
                        // DAL.PurchaseDetail d_Pd = d.PurchaseDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        // if (d_Pd == null)
                        // {
                        DAL.Product_Spec_Process_Detail d_Pd = new DAL.Product_Spec_Process_Detail();
                        d.Product_Spec_Process_Detail.Add(d_Pd);
                        //  }
                        b_Pd.ToMap(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
               return true;
            }

            catch (Exception ex) { Common.AppLib.WriteLog(ex); return false; }

        }

 

        public BLL.Product_Spec_Process Product_Spec_Process(int id)
        {
            BLL.Product_Spec_Process P = new BLL.Product_Spec_Process();
            try
            {

                DAL.Product_Spec_Process d = DB.Product_Spec_Process.Where(x => x.Product.StockGroup.CompanyId == Caller.CompanyId && x.Id == id).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(P);
                    P.ProductName = (d.Product ?? DB.Products.Find(d.ProductId) ?? new DAL.Product()).ProductName;
               
                    int i = 0;
                    foreach (var d_pod in d.Product_Spec_Process_Detail)
                    {
                        BLL.Product_Spec_Process_Detail b_pod = new BLL.Product_Spec_Process_Detail();
                        d_pod.ToMap(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                            }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }

        public bool Product_Spec_Process_Delete(long pk)
        {
            try
            {
                DAL.Product_Spec_Process d = DB.Product_Spec_Process.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = Product_Spec_Process_DALtoBLL(d);
                    DB.Product_Spec_Process_Detail.RemoveRange(d.Product_Spec_Process_Detail);
                    DB.Product_Spec_Process.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                   
                    return true;
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Purchase_RefNoRefresh(Purchase_NewRefNo());

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        public BLL.Product_Spec_Process Product_Spec_Process_DALtoBLL(DAL.Product_Spec_Process d)
        {
            BLL.Product_Spec_Process P = d.ToMap(new BLL.Product_Spec_Process());
            foreach (var d_Pd in d.Product_Spec_Process_Detail)
            {
                P.PDetails.Add(d_Pd.ToMap(new Product_Spec_Process_Detail()));
            }
            return P;
        }
        public BLL.Product_Spec_Process Product_Spec_Process_FindById(int ID)
        {
            BLL.Product_Spec_Process P = new BLL.Product_Spec_Process();
            try
            {

                DAL.Product_Spec_Process d = DB.Product_Spec_Process.Where(x => x.Product.StockGroup.CompanyId == Caller.CompanyId && x.Id == ID).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToMap(P);
                    P.ProductName = (d.Product ?? DB.Products.Find(d.ProductId) ?? new DAL.Product()).ProductName;
                    foreach (var d_pod in d.Product_Spec_Process_Detail)
                    {
                        BLL.Product_Spec_Process_Detail b_pod = new BLL.Product_Spec_Process_Detail();
                        d_pod.ToMap(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                          }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }
        public List<BLL.Product_Spec_Process> PSList()
        {
            return DB.Product_Spec_Process.Where(x => x.Product.StockGroup.CompanyDetail.Id == Caller.UnderCompanyId).ToList()
                                      .Select(x => Product_Spec_Process_DALtoBLL(x)).ToList();
        }


        public List<BLL.Product_Spec_Process> Product_Spec_Process_List(int? ProductId, DateTime dtFrom, DateTime dtTo)
        {
            List<BLL.Product_Spec_Process> lstPurchase = new List<BLL.Product_Spec_Process>();

            BLL.Product_Spec_Process rp = new BLL.Product_Spec_Process();
            try
            {
                foreach (var l in DB.Product_Spec_Process.
                      Where(x => x.Date >= dtFrom && x.Date <= dtTo
                      && (x.ProductId == ProductId || ProductId == null) &&
                      x.Product.StockGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.Product_Spec_Process();
                    rp.Date = l.Date;
            
                    rp.Id = l.Id;
                    rp.ProductId = (int)l.ProductId;
                    rp.ProductName = string.Format("{0}", l.Product.ProductName);

                    lstPurchase.Add(rp);
                    lstPurchase = lstPurchase.OrderBy(x => x.Date).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstPurchase;
        }


        #endregion
    }
}