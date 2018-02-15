using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region Payment
        public string Payment_NewRefNo()
        {
            DateTime dt = DateTime.Now;
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Payment, dt, dt.Month);
            long No = 0;

            var d = DB.Payments.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.EntryNo.StartsWith(Prefix))
                                     .OrderByDescending(x => x.EntryNo)
                                     .FirstOrDefault();

            if (d != null) No = Convert.ToInt64(d.EntryNo.Substring(Prefix.Length), 16);

            return string.Format("{0}{1:X5}", Prefix, No + 1);
        }
        public string Payment_NewRefNo(DateTime dt)
        {
            string Prefix = string.Format("{0}{1:yy}{2:X}", BLL.FormPrefix.Payment, dt, dt.Month);
            long No = 0;

            var d1 = DB.Payments.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId && x.EntryNo.StartsWith(Prefix) && x.PaymentDate.Month == dt.Month).Select(x => x.EntryNo).ToList();
            if (d1.Count() > 0)
            {
                No = d1.Select(x => Convert.ToInt64(x.Substring(Prefix.Length), 16)).Max();
            }

            return string.Format("{0}{1:x5}", Prefix, No + 1);
        }
        public bool Payment_Save(BLL.Payment PO)
        {
            try
            {
                DAL.Payment d = DB.Payments.Where(x => x.Id == PO.Id).FirstOrDefault();
                if (d == null)
                {
                    d = new DAL.Payment();
                    DB.Payments.Add(d);

                    PO.ToMap(d);
                    foreach (var b_pod in PO.PDetails)
                    {
                        DAL.PaymentDetail d_pod = new DAL.PaymentDetail();
                        //if (b_pod.GSTStatusId != 0)
                        //{
                        //    b_pod.ToMap(d_pod);
                        //    d.PaymentDetails.Add(d_pod);
                        //    DB.SaveChanges();
                        //    if (b_pod.PaymentTaxDetails.Count != 0 || b_pod.GSTStatusId == 0)
                        //    {
                        //        foreach (var ptd in b_pod.PaymentTaxDetails)
                        //        {
                        //            DAL.Payment_Tax_Detail pt = new DAL.Payment_Tax_Detail();
                        //            pt.TaxAmount = ptd.TaxAmount;
                        //            pt.TaxId = ptd.TaxId;
                        //            pt.TaxPercentage = ptd.TaxPercentage;
                        //            pt.PD_ID = d_pod.Id;
                        //            DB.Payment_Tax_Detail.Add(pt);
                        //        }
                        //    }
                        //}
                        DAL.PaymentDetail d_pd = new DAL.PaymentDetail();
                        b_pod.ToMap(d_pd);
                        d.PaymentDetails.Add(d_pd);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {
                    //decimal pd = PO.PDetails.Select(X => X.PaymentId).FirstOrDefault();
                    //DB.PaymentDetails.RemoveRange(d.PaymentDetails.Where(x => x.PaymentId == pd).ToList());
                    //DB.SaveChanges();
                    //PO.ToMap(d);

                    //foreach (var b_pod in PO.PDetails)
                    //{
                    //    DAL.PaymentDetail d_pod = new DAL.PaymentDetail();
                    //    if (b_pod.GSTStatusId != 0)
                    //    {
                    //        b_pod.ToMap(d_pod);
                    //        d.PaymentDetails.Add(d_pod);
                    //        DB.SaveChanges();
                    //        if (b_pod.PaymentTaxDetails.Count != 0 || b_pod.GSTStatusId == 0)
                    //        {
                    //            foreach (var ptd in b_pod.PaymentTaxDetails)
                    //            {
                    //                DAL.Payment_Tax_Detail pt = new DAL.Payment_Tax_Detail();
                    //                pt.TaxAmount = ptd.TaxAmount;
                    //                pt.TaxId = ptd.TaxId;
                    //                pt.TaxPercentage = ptd.TaxPercentage;
                    //                pt.PD_ID = d_pod.Id;
                    //                DB.Payment_Tax_Detail.Add(pt);
                    //            }
                    //        }
                    //    }
                    //    DB.SaveChanges();
                    //}
                    //DB.SaveChanges();
                    //LogDetailStore(PO, LogDetailType.UPDATE);

                    decimal rd = PO.PDetails.Select(X => X.PaymentId).FirstOrDefault();
                    DB.PaymentDetails.RemoveRange(d.PaymentDetails.Where(x => x.PaymentId == rd).ToList());

                    PO.ToMap(d);

                    foreach (var b_pod in PO.PDetails)
                    {

                        DAL.PaymentDetail d_pod = new DAL.PaymentDetail();
                        d.PaymentDetails.Add(d_pod);
                        b_pod.ToMap(d_pod);
                    }
                    DB.SaveChanges();

                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Payment_RefNoRefresh(Payment_NewRefNo());
                Journal_SaveByPayment(PO);
                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }

        public BLL.Payment Payment_Find(string EntryNo)
        {
            BLL.Payment PO = new BLL.Payment();
            try
            {
                DAL.Payment d = DB.Payments.Where(x => x.EntryNo == EntryNo && x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
                DB.Entry(d).Reload();
                var tl = DB.TaxMasters.Where(x => x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList();
                if (d != null)
                {
                    d.ToMap(PO);
                    PO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    int i = 0;
                    foreach (var d_pod in d.PaymentDetails)
                    {
                        BLL.PaymentDetail b_pod = new BLL.PaymentDetail();
                        d_pod.ToMap(b_pod);
                        PO.PDetails.Add(b_pod);
                        b_pod.SNo = ++i;
                  
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                        var ptd = d.PaymentDetails.Where(x => x.RefLedgerId == b_pod.LedgerId).ToList();
                        if (ptd.Count != 0)
                        {
                            b_pod.IncludingGST = true;
                        }
                        if (b_pod.RefLedgerId == 0)
                        {
                            b_pod.AllowEdit = true;
                            foreach (var v in ptd)
                            {
                                b_pod.TaxDetails.Add(new BLL.TaxMaster
                                {
                                    Id = TaxIdByCompany_LedgerId(Caller.CompanyId, v.LedgerId),
                                    Ledger = LedgerDAL_BLL(v.Ledger),
                                    LedgerId = v.LedgerId,
                                    Status = true,
                                    TaxAmount = v.Amount,
                                    TaxPercentage = TaxPercentByCompany_LedgerId(Caller.CompanyId, v.LedgerId),
                                    TaxName = string.Format("{0}({1})", v.Ledger.LedgerName, TaxPercentByCompany_LedgerId(Caller.CompanyId, v.LedgerId)),

                                });
                            }
                            var t2 = tl.Where(p => !b_pod.TaxDetails.Any(p2 => p2.Ledger.Id == p.Ledger.Id)).ToList();

                            foreach (var t1 in t2)
                            {
                                b_pod.TaxDetails.Add(new BLL.TaxMaster()
                                {
                                    Id = TaxIdByCompany_LedgerId(Caller.CompanyId, t1.LedgerId),
                                    LedgerId = t1.LedgerId,
                                    Status = false,
                                    Ledger = LedgerDAL_BLL(t1.Ledger),
                                    TaxPercentage = t1.TaxPercentage,
                                    TaxAmount = 0,
                                    TaxName = string.Format("{0}({1})", t1.Ledger.LedgerName, t1.TaxPercentage.ToString()),

                                });
                            }
                        }

                        //var pt = d_pod.Payment_Tax_Detail.ToList();

                        //if (pt.Count > 0)
                        //{
                        //    foreach (var t in pt)
                        //    {
                        //        BLL.PaymentDetail pd = new BLL.PaymentDetail();
                        //        pd.Amount = t.TaxAmount;
                        //        pd.GSTStatusId = 0;
                        //        pd.LedgerId = t.Ledger.Id;
                        //        pd.LedgerName = t.Ledger.LedgerName;
                        //        pd.Particular = "GST";
                        //        PO.PDetails.Add(pd);
                        //        pd.GSTDRefNo = b_pod.SNo;
                        //    }

                        //    foreach (var d1 in pt)
                        //    {
                        //        BLL.Payment_Tax_Detail b1 = new BLL.Payment_Tax_Detail();
                        //        b1.Id = d1.Id;
                        //        b1.Ledger = LedgerDAL_BLL(d1.Ledger);
                        //        b1.PD_ID = d1.PD_ID;
                        //        b1.TaxAmount = d1.TaxAmount;
                        //        b1.TaxId = d1.TaxId;
                        //        b1.TaxPercentage = d1.TaxPercentage;
                        //        b1.TaxName = d1.Ledger.LedgerName;
                        //        b_pod.PaymentTaxDetails.Add(b1);

                        //        var lID = TaxIdByCompany_LedgerId(Caller.CompanyId, d1.Ledger.Id);
                        //        var ptdt = tl.Where(x => x.Id == lID).FirstOrDefault();
                        //        b_pod.TaxDetails.Add(new BLL.TaxMaster()
                        //        {
                        //            Id = lID,
                        //            LedgerId = d1.Ledger.Id,
                        //            Status = true,
                        //            Ledger = LedgerDAL_BLL(d1.Ledger),
                        //            TaxPercentage = TaxPercentByCompany_LedgerId(Caller.CompanyId, d1.Ledger.Id),
                        //            TaxAmount = d1.TaxAmount,
                        //            TaxName = string.Format("{0}({1})", d1.Ledger.LedgerName, d1.TaxPercentage.ToString()),
                        //        });

                        //    }
                        //    var t2 = tl.Where(p => !b_pod.TaxDetails.Any(p2 => p2.Ledger.Id == p.Ledger.Id)).ToList();
                        //    foreach (var t1 in t2)
                        //    {
                        //        b_pod.TaxDetails.Add(new BLL.TaxMaster()
                        //        {
                        //            Id = TaxIdByCompany_LedgerId(Caller.CompanyId, t1.LedgerId),
                        //            LedgerId = t1.LedgerId,
                        //            Status = false,
                        //            Ledger = LedgerDAL_BLL(t1.Ledger),
                        //            TaxPercentage = t1.TaxPercentage,
                        //            TaxAmount = 0,
                        //            TaxName = string.Format("{0}({1})", t1.Ledger.LedgerName, t1.TaxPercentage.ToString()),

                        //        });
                        //    }
                        //}
                        //else
                        //{
                        //    foreach (var td in tl)
                        //    {
                        //        b_pod.TaxDetails.Add(new BLL.TaxMaster()
                        //        {
                        //            Id = td.Id,
                        //            LedgerId = td.Ledger.Id,
                        //            Status = false,
                        //            Ledger = LedgerDAL_BLL(td.Ledger),
                        //            TaxPercentage = td.TaxPercentage,
                        //            TaxAmount = 0,
                        //            TaxName = string.Format("{0}({1})", td.Ledger.LedgerName, td.TaxPercentage.ToString()),
                        //        });
                        //    }

                        //}

                    }

                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return PO;
        }

        public bool Payment_Delete(long pk)
        {
            try
            {
                DAL.Payment d = DB.Payments.Where(x => x.Id == pk).FirstOrDefault();
                var pd = d.PaymentDetails.FirstOrDefault().Id;
                var ptd = DB.Payment_Tax_Detail.Where(x => x.PD_ID == pd).ToList();
                if (d != null)
                {
                    var P = Payment_DALtoBLL(d);
                    DB.Payment_Tax_Detail.RemoveRange(ptd);
                    DB.PaymentDetails.RemoveRange(d.PaymentDetails);
                    DB.Payments.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                    Journal_DeleteByPayment(P);
                    if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).Payment_RefNoRefresh(Payment_NewRefNo());
                }

                return true;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return false;
        }
        public BLL.Payment Payment_DALtoBLL(DAL.Payment d)
        {
            BLL.Payment P = d.ToMap(new BLL.Payment());
            foreach (var d_Pd in d.PaymentDetails)
            {
                P.PDetails.Add(d_Pd.ToMap(new BLL.PaymentDetail()));
            }
            return P;
        }

        public bool Find_EntryNo(string entryNo, BLL.Payment PO)

        {
            DAL.Payment d = DB.Payments.Where(x => x.EntryNo == entryNo & x.Id != PO.Id && x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public List<BLL.Payment> Payment_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string EntryNo, string Status, decimal amtFrom, decimal amtTo)
        {

            List<BLL.Payment> lstPayment = new List<BLL.Payment>();
            BLL.Payment rp = new BLL.Payment();
            try
            {
                foreach (var l in DB.Payments.
                      Where(x => x.PaymentDate >= dtFrom && x.PaymentDate <= dtTo
                      && (x.LedgerId == LedgerId || LedgerId == null)
                      && (EntryNo == "" || x.EntryNo == EntryNo)
                      && (Status == "" || x.Status == Status)
                      && (x.Amount >= amtFrom && x.Amount <= amtTo) &&
                      x.Ledger.AccountGroup.CompanyId == Caller.CompanyId).ToList())
                {
                    rp = new BLL.Payment();
                    rp.Amount = l.Amount;
                    rp.ChequeDate = l.ChequeDate;
                    rp.ChequeNo = l.ChequeNo;
                    rp.ClearDate = l.ClearDate;
                    rp.EntryNo = l.EntryNo;
                    rp.ExtraCharge = l.ExtraCharge;
                    rp.Id = l.Id;
                    rp.LedgerId = l.LedgerId;
                    rp.LedgerName = string.Format("{0}-{1}", l.Ledger.AccountGroup.GroupCode, l.Ledger.LedgerName);
                    rp.Particulars = l.Particulars;
                    rp.PaymentDate = l.PaymentDate;
                    rp.PaymentMode = l.PaymentMode;
                    rp.PayTo = l.PayTo;
                    rp.RefCode = l.RefCode;
                    rp.RefNo = l.RefNo;
                    rp.Status = l.Status;
                    rp.VoucherNo = l.VoucherNo;
                    lstPayment.Add(rp);
                    lstPayment = lstPayment.OrderBy(x => x.PaymentDate).ToList();
                }

            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return lstPayment;
        }


        #endregion
    }
}
