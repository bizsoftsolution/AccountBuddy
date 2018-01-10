using AccountBuddy.BLL;
using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Purchase      


        public BLL.Product_Spec_master Product_Spec_master_Save(AccountBuddy.BLL.Product_Spec_master P)
        {
            try
            {

                AccountBuddy.DAL.Product_Spec_Master d = DB.Product_Spec_Master.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.Product_Spec_Master();
                    DB.Product_Spec_Master.Add(d);
                    P.ToCopy(d);
                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.Product_Spec_Detail d_pod = new DAL.Product_Spec_Detail();
                        b_pod.ToCopy(d_pod);
                        d.Product_Spec_Detail.Add(d_pod);
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

                    decimal rd = P.PDetails.Select(X => X.Product_Spec_Id).FirstOrDefault();
                    DB.Product_Spec_Detail.RemoveRange(d.Product_Spec_Detail.Where(x => x.Product_Spec_Id == rd).ToList());

                    P.ToCopy(d);
                    foreach (var b_Pd in P.PDetails)
                    {
                        // DAL.PurchaseDetail d_Pd = d.PurchaseDetails.Where(x => x.Id == b_Pd.Id).FirstOrDefault();
                        // if (d_Pd == null)
                        // {
                        DAL.Product_Spec_Detail d_Pd = new DAL.Product_Spec_Detail();
                        d.Product_Spec_Detail.Add(d_Pd);
                        //  }
                        b_Pd.ToCopy(d_Pd);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }
                if (d.Id != 0)
                {
                    var p = Product_Spec_master_DALtoBLL(d);
                    return p;
                }
                return new BLL.Product_Spec_master();
            }

            catch (Exception ex) { Common.AppLib.WriteLog(ex); return new BLL.Product_Spec_master(); }

        }


        public BLL.Product_Spec_master Find(int Id)
        {
            BLL.Product_Spec_master P = new BLL.Product_Spec_master();
            try
            {
                DAL.Product_Spec_Master d = DB.Product_Spec_Master.Where(x => x.Product.StockGroup.CompanyId == Caller.CompanyId && x.Id == Id).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.ToCopy(P);
                    P.ProductName = (d.Product ?? DB.Products.Find(d.ProductId) ?? new DAL.Product()).ProductName;

                    int i = 0;
                    foreach (var d_pod in d.Product_Spec_Detail)
                    {
                        BLL.Product_Spec_Detail b_pod = new BLL.Product_Spec_Detail();
                        d_pod.ToCopy(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                    }

                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return P;
        }

        public bool Product_Spec_master_Delete(long pk)
        {
            try
            {
                DAL.Product_Spec_Master d = DB.Product_Spec_Master.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    var P = Product_Spec_master_DALtoBLL(d);
                    DB.Product_Spec_Detail.RemoveRange(d.Product_Spec_Detail);
                    DB.Product_Spec_Master.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);

                    return true;
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        public BLL.Product_Spec_master Product_Spec_master_DALtoBLL(DAL.Product_Spec_Master d)
        {
            BLL.Product_Spec_master P = d.ToCopy(new BLL.Product_Spec_master());
            DAL.Product t= DB.Products.Where(x => x.Id == d.ProductId).FirstOrDefault();
            P.ProductName =t.ProductName;
       
            foreach (var d_Pd in d.Product_Spec_Detail)
            {
                P.PDetails.Add(d_Pd.ToCopy(new BLL.Product_Spec_Detail()));
            }
            return P;
        }
        public List<BLL.Product_Spec_master> List()
        {
            return DB.Product_Spec_Master.Where(x => x.Product.StockGroup.CompanyDetail.Id == Caller.UnderCompanyId).ToList()
                                      .Select(x => Product_Spec_master_DALtoBLL(x)).ToList();

        }


        public List<BLL.Product_Spec_master> Product_Spec_master_List()
        {
            List<BLL.Product_Spec_master> lstProduct_Spec = new List<BLL.Product_Spec_master>();

            BLL.Product_Spec_master rp = new BLL.Product_Spec_master();
            try
            {
                int i = 0;
                foreach (var l in DB.Product_Spec_Master.
                      Where(x => x.Product.StockGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.Product_Spec_master();
                    rp.Id = l.Id;
                    rp.ProductId = l.ProductId;
                    rp.ProductName = string.Format("{0}", l.Product.ProductName);
                    rp.RNo = ++i;
                    lstProduct_Spec.Add(rp);
                    lstProduct_Spec = lstProduct_Spec.OrderBy(x => x.ProductName).ToList();
                }
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstProduct_Spec;
        }


        #endregion
    }
}
