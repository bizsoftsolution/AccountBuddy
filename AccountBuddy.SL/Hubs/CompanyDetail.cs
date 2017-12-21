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
            return d.ToMap(new BLL.CompanyDetail());
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
                Common.AppLib.WriteLog(ex);
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

                    cm.ToMap(d);

                    DB.SaveChanges();
                    cm.Id = d.Id;
                    if (d.Id != 0)
                    {
                        CompanySetup(cm);
                        CurrencySetup(cm);
                        if (d.UnderCompanyId != null)
                        {
                            var lstCompany = DB.CompanyDetails.Where(x => x.Id == d.UnderCompanyId || (x.Id != d.Id && x.UnderCompanyId == d.UnderCompanyId)).ToList();                            
                           int AGId = DB.DataKeyValues.Where(x => x.CompanyId == cm.Id && x.DataKey == BLL.DataKeyValue.BranchDivisions_Key).FirstOrDefault().DataValue;
                            foreach (var c in lstCompany)
                            {
                                DAL.Ledger dl1 = new DAL.Ledger();
                                dl1.LedgerName = string.Format("{0}-{1}", cm.CompanyType == "Company" ? "CM" : (cm.CompanyType == "Warehouse" ? "WH" : "DL"), cm.CompanyName);
                                dl1.AccountGroupId = DB.DataKeyValues.Where(x => x.CompanyId == c.Id && x.DataKey == BLL.DataKeyValue.BranchDivisions_Key).FirstOrDefault().DataValue;
                                dl1.AddressLine1 = cm.AddressLine1;
                                dl1.AddressLine2 = cm.AddressLine2;
                                dl1.CityName = cm.CityName;
                                dl1.EMailId = cm.EMailId;
                                dl1.GSTNo = cm.GSTNo;
                                dl1.MobileNo = cm.MobileNo;
                                dl1.TelephoneNo = cm.TelephoneNo;
                                DB.Ledgers.Add(dl1);
                                DB.SaveChanges();

                                DAL.Ledger dl2 = new DAL.Ledger();
                                dl2.LedgerName = string.Format("{0}-{1}", c.CompanyType == "Company" ? "CM" : (c.CompanyType == "Warehouse" ? "WH" : "DL"), c.CompanyName);
                                dl2.AccountGroupId = AGId;
                                dl2.AddressLine1 = c.AddressLine1;
                                dl2.AddressLine2 = c.AddressLine2;
                                dl2.CityName = c.CityName;
                                dl2.EMailId = c.EMailId;
                                dl2.GSTNo = c.GSTNo;
                                dl2.MobileNo = c.MobileNo;
                                dl2.TelephoneNo = c.TelephoneNo;
                                DB.Ledgers.Add(dl2);
                                DB.SaveChanges();


                            }
                        }
                        
                    }
                }
                else
                {
                    var CName = d.CompanyName;
                    cm.ToMap(d);
                    DB.SaveChanges();

                    var LName = string.Format("{0}-{1}", cm.CompanyType == "Company" ? "CM" : (cm.CompanyType == "Warehouse" ? "WH" : "DL"), CName);
                    var lstLedger = DB.Ledgers.Where(x => x.LedgerName == LName).ToList();
                    foreach (var dl in lstLedger)
                    {
                        dl.LedgerName = string.Format("{0}-{1}", cm.CompanyType == "Company" ? "CM" : (cm.CompanyType == "Warehouse" ? "WH" : "DL"), cm.CompanyName);
                        dl.AddressLine1 = cm.AddressLine1;
                        dl.AddressLine2 = cm.AddressLine2;
                        dl.CityName = cm.CityName;
                        dl.EMailId = cm.EMailId;
                        dl.GSTNo = cm.GSTNo;
                        dl.MobileNo = cm.MobileNo;
                        dl.TelephoneNo = cm.TelephoneNo;
                    }
                    DB.SaveChanges();
                }
                if (OtherClientsOnGroup.Count > 0) Clients.Clients(OtherClientsOnGroup).CompanyDetail_Save(cm);
               

                return cm.Id;
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
            return 0;
        }

        private void CurrencySetup(CompanyDetail cm)
        {
            if(cm.UnderCompanyId==0||cm.UnderCompanyId==null)
            {
                DAL.CustomFormat cf = new DAL.CustomFormat();
                cf.CompanyId = cm.Id;
                cf.CurrencyPositiveSymbolPrefix = "RM ";
                cf.CurrencyPositiveSymbolSuffix = "";
                cf.CurrencyNegativeSymbolPrefix = "RM[-";
                cf.CurrencyNegativeSymbolSuffix = "]";
                cf.CurrencyToWordPrefix = "Ringgits ";
                cf.CurrencyToWordSuffix= "";
                cf.DecimalToWordPrefix = "Cents ";
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
                    LogDetailStore(d.ToMap(new BLL.CompanyDetail()), LogDetailType.DELETE);
                }

                var uac = DB.UserAccounts.Where(x => x.UserType.CompanyId == d.Id);
                DB.UserAccounts.RemoveRange(uac);
                DB.SaveChanges();

                if (OtherClients.Count > 0) Clients.Clients(OtherClients).CompanyDetail_Delete(pk);
               
            }
            catch (Exception ex) { Common.AppLib.WriteLog(ex); }
        }

        private void CompanySetup(BLL.CompanyDetail sgp)
        {
            UserSetup(sgp);
            AccountSetup(sgp);

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


            AccountSetup_Asset(pr);
            AccountSetup_Liabilities(pr);
            AccountSetup_Income(pr);
            AccountSetup_Expense(pr);

            DAL.Ledger PL = new DAL.Ledger();
            PL.LedgerName = BLL.DataKeyValue.Profit_Loss_Ledger_Key;
            PL.AccountGroupId = pr.Id;
            DB.Ledgers.Add(PL);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, PL.LedgerName, PL.Id);



        }

        void AccountSetup_Asset(DAL.AccountGroup pr)
        {
            DAL.AccountGroup ast = new DAL.AccountGroup();
            ast.GroupName = BLL.DataKeyValue.Assets_Key;
            ast.GroupCode = "100";
            ast.CompanyId = pr.CompanyId;
            ast.UnderGroupId = null;
            DB.AccountGroups.Add(ast);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, ast.GroupName, ast.Id);


            #region Current Assets
            DAL.AccountGroup ca = new DAL.AccountGroup();
            ca.GroupName = BLL.DataKeyValue.CurrentAssets_Key;
            ca.GroupCode = "110";
            ca.UnderGroupId = ast.Id;
            ca.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(ca);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, ca.GroupName, ca.Id);


            DAL.AccountGroup ch = new DAL.AccountGroup();
            ch.GroupName = BLL.DataKeyValue.CashInHand_Key;
            ch.GroupCode = "111";
            ch.UnderGroupId = ca.Id;
            ch.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(ch);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, ch.GroupName, ch.Id);

            DAL.Ledger cL = new DAL.Ledger();
            cL.LedgerName = BLL.DataKeyValue.CashLedger_Key;
            cL.AccountGroupId = ch.Id;
            DB.Ledgers.Add(cL);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, cL.LedgerName, cL.Id);

            DAL.AccountGroup dp = new DAL.AccountGroup();
            dp.GroupName = BLL.DataKeyValue.Deposits_Key;
            dp.GroupCode = "112";
            dp.UnderGroupId = ca.Id;
            dp.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(dp);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, dp.GroupName, dp.Id);


            DAL.AccountGroup la = new DAL.AccountGroup();
            la.GroupName = BLL.DataKeyValue.LoansandAdvances_Key;
            la.GroupCode = "113";
            la.UnderGroupId = ca.Id;
            la.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(la);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, la.GroupName, la.Id);


            DAL.AccountGroup ba = new DAL.AccountGroup();
            ba.GroupName = BLL.DataKeyValue.BankAccounts_Key;
            ba.GroupCode = "114";
            ba.UnderGroupId = ca.Id;
            ba.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(ba);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, ba.GroupName, ba.Id);


            DAL.AccountGroup SIH = new DAL.AccountGroup();
            SIH.GroupName = BLL.DataKeyValue.StockInHand_Key;
            SIH.GroupCode = "115";
            SIH.UnderGroupId = ca.Id;
            SIH.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(SIH);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, SIH.GroupName, SIH.Id);

            DAL.Ledger st = new DAL.Ledger();
            st.LedgerName = BLL.DataKeyValue.Stock_In_Hand_Ledger_Key;
            st.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(st);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, st.LedgerName, st.Id);

            DAL.Ledger sti = new DAL.Ledger();
            sti.LedgerName = BLL.DataKeyValue.Stock_Inward_Ledger_Key;
            sti.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(sti);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, sti.LedgerName, sti.Id);

            DAL.Ledger sto = new DAL.Ledger();
            sto.LedgerName = BLL.DataKeyValue.Stock_Outward_Ledger_Key;
            sto.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(sto);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, sto.LedgerName, sto.Id);

            DAL.AccountGroup sd = new DAL.AccountGroup();
            sd.GroupName = BLL.DataKeyValue.SundryDebtors_Key;
            sd.GroupCode = "116";
            sd.UnderGroupId = ca.Id;
            sd.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(sd);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, sd.GroupName, sd.Id);


            DAL.Ledger SP = new DAL.Ledger();
            SP.LedgerName = BLL.DataKeyValue.StockInProcess_Ledger_Key;
            SP.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(SP);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, SP.LedgerName, SP.Id);

            DAL.Ledger SS = new DAL.Ledger();
            SS.LedgerName = BLL.DataKeyValue.StockSeperated_Ledger_Key;
            SS.AccountGroupId = SIH.Id;
            DB.Ledgers.Add(SS);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, SS.LedgerName, SS.Id);



            #endregion

            #region Fixed Assets

            DAL.AccountGroup fa = new DAL.AccountGroup();
            fa.GroupName = BLL.DataKeyValue.FixedAssets_Key;
            fa.GroupCode = "120";
            fa.UnderGroupId = ast.Id;
            fa.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(fa);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, fa.GroupName, fa.Id);

            #endregion


            #region Misc. Expenses

            DAL.AccountGroup me = new DAL.AccountGroup();
            me.GroupName = BLL.DataKeyValue.MiscExpenses_Key;
            me.GroupCode = "130";
            me.UnderGroupId = ast.Id;
            me.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(me);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, me.GroupName, me.Id);

            #endregion

            DAL.AccountGroup Inv = new DAL.AccountGroup();
            Inv.GroupName = BLL.DataKeyValue.Investments_Key;
            Inv.GroupCode = "140";
            Inv.UnderGroupId = ast.Id;
            Inv.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(Inv);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, Inv.GroupName, Inv.Id);

        }

        void AccountSetup_Liabilities(DAL.AccountGroup pr)
        {
            DAL.AccountGroup liab = new DAL.AccountGroup();
            liab.GroupName = BLL.DataKeyValue.Liabilities_Key;
            liab.GroupCode = "200";
            liab.CompanyId = pr.CompanyId;
            liab.UnderGroupId =null;
            DB.AccountGroups.Add(liab);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, liab.GroupName, liab.Id);

            #region Current Liabilities
            DAL.AccountGroup cl = new DAL.AccountGroup();
            cl.GroupName = BLL.DataKeyValue.CurrentLiabilities_Key;
            cl.GroupCode = "210";
            cl.UnderGroupId = liab.Id;
            cl.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(cl);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, cl.GroupName, cl.Id);

            DAL.AccountGroup DT = new DAL.AccountGroup();
            DT.GroupName = BLL.DataKeyValue.DutiesTaxes_Key;
            DT.GroupCode = "211";
            DT.UnderGroupId = cl.Id;
            DT.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(DT);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, DT.GroupName, DT.Id);

            DAL.Ledger IT = new DAL.Ledger();
            IT.LedgerName = BLL.DataKeyValue.Input_Tax_Ledger_Key;
            IT.AccountGroupId = DT.Id;
            DB.Ledgers.Add(IT);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, IT.LedgerName, IT.Id);


            DAL.Ledger OT = new DAL.Ledger();
            OT.LedgerName = BLL.DataKeyValue.Output_Tax_Ledger_Key;
            OT.AccountGroupId = DT.Id;
            DB.Ledgers.Add(OT);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, OT.LedgerName, OT.Id);

            DAL.AccountGroup prov = new DAL.AccountGroup();
            prov.GroupName = BLL.DataKeyValue.Provisions_Key;
            prov.GroupCode = "212";
            prov.UnderGroupId = cl.Id;
            prov.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(prov);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, prov.GroupName, prov.Id);

            DAL.AccountGroup sc = new DAL.AccountGroup();
            sc.GroupName = BLL.DataKeyValue.SundryCreditors_Key;
            sc.GroupCode = "212";
            sc.UnderGroupId = cl.Id;
            sc.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(sc);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, sc.GroupName, sc.Id);


            #region Loans
            DAL.AccountGroup l = new DAL.AccountGroup();
            l.GroupName = BLL.DataKeyValue.Loans_Key;
            l.GroupCode = "220";
            l.UnderGroupId = liab.Id;
            l.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(l);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, l.GroupName, l.Id);


            DAL.AccountGroup BOAc = new DAL.AccountGroup();
            BOAc.GroupName = BLL.DataKeyValue.BankODAc_Key;
            BOAc.GroupCode = "221";
            BOAc.UnderGroupId = l.Id;
            BOAc.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(BOAc);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, BOAc.GroupName, BOAc.Id);

            DAL.AccountGroup SL = new DAL.AccountGroup();
            SL.GroupName = BLL.DataKeyValue.SecuredLoans_Key;
            SL.GroupCode = "221";
            SL.UnderGroupId = l.Id;
            SL.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(SL);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, SL.GroupName, SL.Id);

            DAL.AccountGroup USL = new DAL.AccountGroup();
            USL.GroupName = BLL.DataKeyValue.UnSecuredLoans_Key;
            USL.GroupCode = "222";
            USL.UnderGroupId = l.Id;
            USL.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(USL);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, USL.GroupName, USL.Id);

            #endregion


            DAL.AccountGroup BD = new DAL.AccountGroup();
            BD.GroupName = BLL.DataKeyValue.BranchDivisions_Key;
            BD.GroupCode = "230";
            BD.UnderGroupId = liab.Id;
            BD.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(BD);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, BD.GroupName, BD.Id);



            DAL.AccountGroup Cap = new DAL.AccountGroup();
            Cap.GroupName = BLL.DataKeyValue.CapitalAccount_Key;
            Cap.GroupCode = "240";
            Cap.UnderGroupId = liab.Id;
            Cap.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(Cap);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, Cap.GroupName, Cap.Id);

            DAL.AccountGroup RS = new DAL.AccountGroup();
            RS.GroupName = BLL.DataKeyValue.ReservesSurplus_Key;
            RS.GroupCode = "250";
            RS.UnderGroupId = liab.Id;
            RS.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(RS);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, RS.GroupName, RS.Id);

            DAL.AccountGroup SAC = new DAL.AccountGroup();
            SAC.GroupName = BLL.DataKeyValue.SuspenseAc_Key;
            SAC.GroupCode = "260";
            SAC.UnderGroupId = liab.Id;
            SAC.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(SAC);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, SAC.GroupName, SAC.Id);

            #endregion
        }

        void AccountSetup_Income(DAL.AccountGroup pr)
        {
            DAL.AccountGroup Inc = new DAL.AccountGroup();
            Inc.GroupName = BLL.DataKeyValue.Income_Key;
            Inc.GroupCode = "300";
            Inc.CompanyId = pr.CompanyId;
            Inc.UnderGroupId = null;
            DB.AccountGroups.Add(Inc);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, Inc.GroupName, Inc.Id);



            #region Direct Income

            DAL.AccountGroup DInc = new DAL.AccountGroup();
            DInc.GroupName = BLL.DataKeyValue.DirectIncome_Key;
            DInc.GroupCode = "310";
            DInc.CompanyId = pr.CompanyId;
            DInc.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(DInc);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, DInc.GroupName, DInc.Id);

            #endregion

            #region Indirect Income

            DAL.AccountGroup IndInc = new DAL.AccountGroup();
            IndInc.GroupName = BLL.DataKeyValue.IndirectIncome_Key;
            IndInc.GroupCode = "320";
            IndInc.CompanyId = pr.CompanyId;
            IndInc.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(IndInc);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, IndInc.GroupName, IndInc.Id);

            #endregion

            DAL.AccountGroup Sa = new DAL.AccountGroup();
            Sa.GroupName = BLL.DataKeyValue.SalesAccount_Key;
            Sa.GroupCode = "330";
            Sa.CompanyId = pr.CompanyId;
            Sa.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(Sa);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, Sa.GroupName, Sa.Id);

            DAL.Ledger salL = new DAL.Ledger();
            salL.LedgerName = BLL.DataKeyValue.SalesAccount_Ledger_Key;
            salL.AccountGroupId = Sa.Id;
            DB.Ledgers.Add(salL);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, salL.LedgerName, salL.Id);

            DAL.Ledger SRL = new DAL.Ledger();
            SRL.LedgerName = BLL.DataKeyValue.Sales_Return_Ledger_Key;
            SRL.AccountGroupId = Sa.Id;
            DB.Ledgers.Add(SRL);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, SRL.LedgerName, SRL.Id);

            DAL.Ledger JR = new DAL.Ledger();
            JR.LedgerName = BLL.DataKeyValue.JobOrderReceived_Ledger_Key;
            JR.AccountGroupId = Inc.Id;
            DB.Ledgers.Add(JR);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, JR.LedgerName, JR.Id);


        }

        void AccountSetup_Expense(DAL.AccountGroup pr)
        {
            DAL.AccountGroup Exp = new DAL.AccountGroup();
            Exp.GroupName = BLL.DataKeyValue.Expenses_Key;
            Exp.GroupCode = "400";
            Exp.CompanyId = pr.CompanyId;
            Exp.UnderGroupId = null;
            DB.AccountGroups.Add(Exp);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, Exp.GroupName, Exp.Id);

            #region Direct Expense

            DAL.AccountGroup DExp = new DAL.AccountGroup();
            DExp.GroupName = BLL.DataKeyValue.DirectExpenses_Key;
            DExp.GroupCode = "410";
            DExp.CompanyId = pr.CompanyId;
            DExp.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(DExp);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, DExp.GroupName, DExp.Id);

            #endregion

            #region Indirect Expense

            DAL.AccountGroup IndExp = new DAL.AccountGroup();
            IndExp.GroupName = BLL.DataKeyValue.IndirectExpense_Key;
            IndExp.GroupCode = "320";
            IndExp.CompanyId = pr.CompanyId;
            IndExp.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(IndExp);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, IndExp.GroupName, IndExp.Id);
            #endregion

            DAL.AccountGroup Pur = new DAL.AccountGroup();
            Pur.GroupName = BLL.DataKeyValue.PurchaseAccount_Key;
            Pur.GroupCode = "330";
            Pur.CompanyId = pr.CompanyId;
            Pur.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(Pur);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, Pur.GroupName, Pur.Id);

         

            DAL.Ledger PurL = new DAL.Ledger();
            PurL.LedgerName = BLL.DataKeyValue.PurchaseAccount_Ledger_Key;
            PurL.AccountGroupId = Pur.Id;
            DB.Ledgers.Add(PurL);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, PurL.LedgerName, PurL.Id);

            DAL.Ledger PRL = new DAL.Ledger();
            PRL.LedgerName = BLL.DataKeyValue.Purchase_Return_Ledger_Key;
            PRL.AccountGroupId = Pur.Id;
            DB.Ledgers.Add(PRL);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, PRL.LedgerName, PRL.Id);

            DAL.Ledger JO = new DAL.Ledger();
            JO.LedgerName = BLL.DataKeyValue.JobOrderIssued_Ledger_Key;
            JO.AccountGroupId = Exp.Id;
            DB.Ledgers.Add(JO);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, JO.LedgerName, JO.Id);

            DAL.AccountGroup salary = new DAL.AccountGroup();
            salary.GroupName = BLL.DataKeyValue.Salary_Key;
            salary.GroupCode = "340";
            salary.CompanyId = pr.CompanyId;
            salary.UnderGroupId = IndExp.Id;
            DB.AccountGroups.Add(salary);
            DB.SaveChanges();
            insertDataKeyValue(pr.CompanyId, salary.GroupName, salary.Id);



        }

        #endregion
    }
}
