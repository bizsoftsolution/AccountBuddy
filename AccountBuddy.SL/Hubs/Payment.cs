﻿using System;
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

        public bool Payment_Save(BLL.Payment PO)
        {
            try
            {                
                DAL.Payment d = DB.Payments.Where(x => x.Id == PO.Id).FirstOrDefault();
                if (d == null)
                {
                    d = new DAL.Payment();
                    DB.Payments.Add(d);

                    PO.toCopy<DAL.Payment>(d);

                    foreach (var b_pod in PO.PDetails)
                    {
                        DAL.PaymentDetail d_pod = new DAL.PaymentDetail();
                        b_pod.toCopy<DAL.PaymentDetail>(d_pod);
                        d.PaymentDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {

                    foreach (var d_pod in d.PaymentDetails)
                    {
                        BLL.PaymentDetail b_pod = PO.PDetails.Where(x => x.Id == d_pod.Id).FirstOrDefault();
                        if (b_pod == null) d.PaymentDetails.Remove(d_pod);
                    }
                    PO.toCopy<DAL.Payment>(d);

                    foreach (var b_pod in PO.PDetails)
                    {
                        DAL.PaymentDetail d_pod = d.PaymentDetails.Where(x=> x.Id==b_pod.Id).FirstOrDefault();
                        if (d_pod == null)
                        {
                            d_pod = new DAL.PaymentDetail();
                            d.PaymentDetails.Add(d_pod);
                        }
                        b_pod.toCopy<DAL.PaymentDetail>(d_pod);                        
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }
                
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Payment Payment_Find(string SearchText)
        {
            BLL.Payment PO = new BLL.Payment();
            try
            {

                DAL.Payment d = DB.Payments.Where(x => x.EntryNo == SearchText && x.Ledger.AccountGroup.CompanyId==Caller.CompanyId).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Payment>(PO);
                    PO.LedgerName = (d.Ledger ?? DB.Ledgers.Find(d.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    foreach (var d_pod in d.PaymentDetails)
                    {
                        BLL.PaymentDetail b_pod = new BLL.PaymentDetail();
                        d_pod.toCopy<BLL.PaymentDetail>(b_pod);
                        PO.PDetails.Add(b_pod);
                        b_pod.LedgerName = (d_pod.Ledger ?? DB.Ledgers.Find(d_pod.LedgerId) ?? new DAL.Ledger()).LedgerName;
                    }

                }
            }
            catch (Exception ex) { }
            return PO;
        }

        public bool Payment_Delete(long pk)
        {
            try
            {
                DAL.Payment d = DB.Payments.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    DB.PaymentDetails.RemoveRange(d.PaymentDetails);
                    DB.Payments.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(Payment_DALtoBLL(d), LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        public BLL.Payment Payment_DALtoBLL(DAL.Payment d)
        {
            BLL.Payment P = d.toCopy<BLL.Payment>(new BLL.Payment());
            foreach (var d_Pd in d.PaymentDetails)
            {
                P.PDetails.Add(d_Pd.toCopy<BLL.PaymentDetail>(new BLL.PaymentDetail()));
            }
            return P;
        }

        public bool Find_EntryNo(string entryNo, BLL.Payment PO)

        {
            DAL.Payment d = DB.Payments.Where(x => x.EntryNo == entryNo & x.Id != PO.Id && x.Ledger.AccountGroup.CompanyId==Caller.CompanyId).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        #endregion
    }
}
