using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.BLL;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {

        #region CompanyDetail

        BLL.CompanyDetail CompanyDetailDAL_BLL(DAL.CompanyDetail d)
        {
            return d.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail());
        }

        public List<string> CompanyDetail_AcYearList()
        {
            List<string> AcYearList = new List<string>();

            DateTime d = DateTime.Now;
            int YearFrom = d.Month < 4 ? d.Year - 1 : d.Year;
            int YearTo = d.Month < 4 ? d.Year : d.Year + 1;

            if (DB.Payments.Count() > 0)
            {
                d = DB.Payments.Min(x => x.PaymentDate);
                int yy = YearFrom = d.Month < 4 ? d.Year - 1 : d.Year;
                YearFrom = yy < YearFrom ? yy : YearFrom;
            }

            if (DB.Receipts.Count() > 0)
            {
                d = DB.Receipts.Min(x => x.ReceiptDate);
                int yy = YearFrom = d.Month < 4 ? d.Year - 1 : d.Year;
                YearFrom = yy < YearFrom ? yy : YearFrom;
            }

            if (DB.Journals.Count() > 0)
            {
                d = DB.Journals.Min(x => x.JournalDate);
                int yy = YearFrom = d.Month < 4 ? d.Year - 1 : d.Year;
                YearFrom = yy < YearFrom ? yy : YearFrom;
            }

            for (int n = YearFrom; n < YearTo; n++)
            {
                AcYearList.Add(string.Format("{0} - {1}", n, n + 1));
            }

            return AcYearList;
        }

        public List<BLL.CompanyDetail> CompanyDetail_List()
        {
            List<BLL.CompanyDetail> rv;
            try
            {
             rv= DB.CompanyDetails.ToList().Select(x => CompanyDetailDAL_BLL(x)).ToList();
                return rv;
            }
            catch(Exception ex)
            {
                return rv=new List<BLL.CompanyDetail>();
            }
        }

        int AccountGroupIdByCompanyAndKey(int CompanyId, string key)
        {
            return DB.DataKeyValues.Where(x => x.CompanyId == CompanyId && x.DataKey == key).FirstOrDefault().DataValue;
        }

        public int CompanyDetail_Save(BLL.CompanyDetail cm)
        {
            try
            {
                cm.IsActive = true;
                DAL.CompanyDetail d = DB.CompanyDetails.Where(x => x.Id == cm.Id).FirstOrDefault();

                if (d == null)
                {
                    d = new DAL.CompanyDetail();
                    DB.CompanyDetails.Add(d);

                    cm.toCopy<DAL.CompanyDetail>(d);

                    DB.SaveChanges();
                    cm.Id = d.Id;
                    if (d.Id != 0)
                    {
                        CompanySetup(cm);
                        CurrencySetup(cm);
                      
                    }
                }
                else
                {
                    var CName = d.CompanyName;
                    cm.toCopy<DAL.CompanyDetail>(d);
                    DB.SaveChanges();
                    
                }

                  Clients.All.CompanyDetail_Save(cm);
               // Clients.Clients(OtherLoginClientsOnGroup).CompanyDetail_Save(cm); 

                return cm.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        private void CurrencySetup(CompanyDetail cm)
        {
            if(cm.UnderCompanyId==0||cm.UnderCompanyId==null)
            {
                DAL.CustomFormat cf = new DAL.CustomFormat();
                cf.CompanyId = cm.Id;
                cf.CurrencyPositiveSymbolPrefix = "RS ";
                cf.CurrencyPositiveSymbolSuffix = "";
                cf.CurrencyNegativeSymbolPrefix = "RS[-";
                cf.CurrencyNegativeSymbolSuffix = "]";
                cf.CurrencyToWordPrefix = "Rupees ";
                cf.CurrencyToWordSuffix= "";
                cf.DecimalToWordPrefix = "Paisas ";
                cf.DecimalToWordSuffix= "";
                cf.DigitGroupingBy = 2;
                cf.CurrencyCaseSensitive = 2;
                cf.DecimalSymbol = ".";
                cf.DigitGroupingSymbol = ",";
                cf.IsDisplayWithOnlyOnSuffix = true;
                cf.NoOfDigitAfterDecimal = 2;

                DB.CustomFormats.Add(cf);
                DB.SaveChanges();

            }
           
        }

        public void CompanyDetail_Delete(int pk)
        {
            try
            {
                var d = DB.CompanyDetails.Where(x => x.Id == pk).FirstOrDefault();
                if (d != null)
                {
                    d.IsActive = false;

                    DB.SaveChanges();
                    LogDetailStore(d.toCopy<BLL.CompanyDetail>(new BLL.CompanyDetail()), LogDetailType.DELETE);
                }

                var uac = DB.UserAccounts.Where(x => x.UserType.CompanyId == d.Id);
                DB.UserAccounts.RemoveRange(uac);
                DB.SaveChanges();

                Clients.Clients(OtherLoginClientsOnGroup).CompanyDetail_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }

        private void CompanySetup(BLL.CompanyDetail sgp)
        {
            UserSetup(sgp);
            AccountSetup(sgp);

            DAL.StockGroup pr = new DAL.StockGroup();
            pr.StockGroupName = BLL.DataKeyValue.StockGroup_Primary_Key;
            pr.GroupCode = "";
            pr.CompanyId = sgp.Id;
            pr.IsPurchase = true;
            pr.IsSale = true;
            DB.StockGroups.Add(pr);
            DB.SaveChanges();
            insertDataKeyValue(sgp.Id, pr.StockGroupName, pr.Id);

            DAL.UOM u = new DAL.UOM();
            u.FormalName = BLL.DataKeyValue.UOM_Key;
            u.Symbol = "UOM";
            u.CompanyId = sgp.Id;
          
            DB.UOMs.Add(u);
            DB.SaveChanges();
            insertDataKeyValue(sgp.Id, u.Symbol, u.Id);

        }

        void UserSetup(BLL.CompanyDetail cmp)
        {
            DAL.UserAccount ua = new DAL.UserAccount();
            ua.LoginId = cmp.UserId;
            ua.UserName = cmp.UserId;
            ua.Password = cmp.Password;

            DAL.UserType ut = new DAL.UserType();
            ut.TypeOfUser = BLL.DataKeyValue.Administrator_Key;
            ut.CompanyId = cmp.Id;
            ut.UserAccounts.Add(ua);

            foreach (var utfd in DB.UserTypeFormDetails)
            {
                DAL.UserTypeDetail utd = new DAL.UserTypeDetail();
                utd.UserTypeFormDetailId = utfd.Id;
                utd.IsViewForm = true;
                utd.AllowInsert = true;
                utd.AllowUpdate = true;
                utd.AllowDelete = true;
                ut.UserTypeDetails.Add(utd);
            }

            DB.UserTypes.Add(ut);
            DB.SaveChanges();

            insertDataKeyValue(cmp.Id, ut.TypeOfUser, ut.Id);

        }

        void insertDataKeyValue(int CompanyId, string DataKey, int DataValue)
        {
            DAL.DataKeyValue dk = new DAL.DataKeyValue();
            dk.CompanyId = CompanyId;
            dk.DataKey = DataKey.Trim(' ');
            dk.DataValue = DataValue;
            DB.DataKeyValues.Add(dk);
            DB.SaveChanges();
        }

        void AccountSetup(BLL.CompanyDetail cmp)
        {
            DAL.AccountGroup pr = new DAL.AccountGroup();
            pr.GroupName = BLL.DataKeyValue.Primary_Key;
            pr.GroupCode = "";
            pr.CompanyId = cmp.Id;
            DB.AccountGroups.Add(pr);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, pr.GroupName, pr.Id);

            DAL.AccountGroup sd = new DAL.AccountGroup();
            sd.GroupName = BLL.DataKeyValue.SundryDebtors_Key;
            sd.GroupCode = "";
            sd.CompanyId = cmp.Id;
            DB.AccountGroups.Add(sd);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, sd.GroupName, sd.Id);

            DAL.AccountGroup sc = new DAL.AccountGroup();
            sc.GroupName = BLL.DataKeyValue.SundryCreditors_Key;
            sc.GroupCode = "";
            sc.CompanyId = cmp.Id;
            DB.AccountGroups.Add(sc);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, sc.GroupName, sc.Id);

            DAL.AccountGroup cla = new DAL.AccountGroup();
            cla.GroupName = BLL.DataKeyValue.Cash_in_Hand_Key;
            cla.GroupCode = "";
            cla.CompanyId = cmp.Id;
            DB.AccountGroups.Add(cla);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, cla.GroupName, cla.Id);

            DAL.Ledger cL = new DAL.Ledger();
            cL.LedgerName = BLL.DataKeyValue.CashLedger_Key;
            cL.AccountGroupId = cla.Id;
            DB.Ledgers.Add(cL);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, cL.LedgerName, cL.Id);

            DAL.Ledger pur = new DAL.Ledger();
            pur.LedgerName = BLL.DataKeyValue.PurchaseAccount_Ledger_Key;
            pur.AccountGroupId = pr.Id;
            DB.Ledgers.Add(pur);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, pur.LedgerName, pur.Id);

            DAL.Ledger sal = new DAL.Ledger();
            sal.LedgerName = BLL.DataKeyValue.SalesAccount_Key;
            sal.AccountGroupId = pr.Id;
            DB.Ledgers.Add(sal);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, sal.LedgerName, sal.Id);

            DAL.AccountGroup ba = new DAL.AccountGroup();
            ba.GroupName = BLL.DataKeyValue.Bank_Accounts_Key;
            ba.GroupCode = "";
            ba.CompanyId = cmp.Id;
            DB.AccountGroups.Add(ba);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, ba.GroupName, ba.Id);

            DAL.Ledger bal = new DAL.Ledger();
            bal.LedgerName = BLL.DataKeyValue.Bank_Accounts_Key;
            bal.AccountGroupId = ba.Id;
            DB.Ledgers.Add(bal);
            DB.SaveChanges();
            insertDataKeyValue(cmp.Id, bal.LedgerName, bal.Id);
        }


        #endregion
    }
}
