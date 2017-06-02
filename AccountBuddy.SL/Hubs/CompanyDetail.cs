using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.SL.Hubs
{
    public partial class ABServerHub
    {
        #region CompanyDetail

        public static List<BLL.CompanyDetail> _listCompany;
        public static List<BLL.CompanyDetail> ListCompany
        {
            get
            {
                if (_listCompany == null)
                {
                    _listCompany = DB.CompanyDetails.Where(x => x.IsActive == true).Select(x => new BLL.CompanyDetail()
                    {
                        Id = x.Id,
                        CompanyName = x.CompanyName,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        CityName = x.CityName,
                        EMailId = x.EMailId,
                        GSTNo = x.GSTNo,
                        Logo = x.Logo,
                        MobileNo = x.MobileNo,
                        PostalCode = x.PostalCode,
                        TelephoneNo = x.TelephoneNo
                    }
                    ).ToList();
                }
                return _listCompany;
            }
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
                int yy = d.Month < 4 ? d.Year - 1 : d.Year;
                YearFrom = yy < YearFrom ? yy : YearFrom;
            }

            if (DB.Receipts.Count() > 0)
            {
                d = DB.Receipts.Min(x => x.ReceiptDate);
                int yy = d.Month < 4 ? d.Year - 1 : d.Year;
                YearFrom = yy < YearFrom ? yy : YearFrom;
            }

            if (DB.Journals.Count() > 0)
            {
                d = DB.Journals.Min(x => x.JournalDate);
                int yy = d.Month < 4 ? d.Year - 1 : d.Year;
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
            return ListCompany;
        }

        public int CompanyDetail_Save(BLL.CompanyDetail sgp)
        {
            try
            {
                sgp.IsActive = true;
                BLL.CompanyDetail b = ListCompany.Where(x => x.Id == sgp.Id).FirstOrDefault();
                DAL.CompanyDetail d = DB.CompanyDetails.Where(x => x.Id == sgp.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.CompanyDetail();
                    ListCompany.Add(b);

                    d = new DAL.CompanyDetail();
                    DB.CompanyDetails.Add(d);

                    sgp.toCopy<DAL.CompanyDetail>(d);

                    DB.SaveChanges();
                    d.toCopy<BLL.CompanyDetail>(b);
                    sgp.Id = d.Id;
                    if (d.Id != 0) CompanySetup(sgp);
                }
                else
                {
                    sgp.toCopy<BLL.CompanyDetail>(b);
                    sgp.toCopy<DAL.CompanyDetail>(d);
                    DB.SaveChanges();
                }

                Clients.Others.CompanyDetail_Save(sgp);

                return sgp.Id;
            }
            catch (Exception ex) { }
            return 0;
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

                //var uac = DB.UserAccounts.Where(x => x.UserType.CompanyId == d.Id);
                //DB.UserAccounts.RemoveRange(uac);
                //DB.SaveChanges();

                Clients.Clients(OtherLoginClientsOnGroup).CompanyDetail_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }

        private void CompanySetup(BLL.CompanyDetail sgp)
        {
            UserSetup(sgp);
            AccountSetup(sgp); 
                       
        }

        void UserSetup(BLL.CompanyDetail sgp)
        {
            DAL.UserAccount ua = new DAL.UserAccount();
            ua.LoginId = sgp.UserId;
            ua.UserName = sgp.UserId;
            ua.Password = sgp.Password;

            DAL.UserType ut = new DAL.UserType();
            ut.TypeOfUser = "Administrator";
            ut.CompanyId = sgp.Id;
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
        }
        void AccountSetup(BLL.CompanyDetail sgp)
        {
            DAL.AccountGroup pr = new DAL.AccountGroup();
            pr.GroupName = "Primary";
            pr.GroupCode = "";
            pr.CompanyId = sgp.Id;
            DB.AccountGroups.Add(pr);
            DB.SaveChanges();

            AccountSetup_Asset(pr);
            AccountSetup_Liabilities(pr);
            AccountSetup_Income(pr);
            AccountSetup_Expense(pr);

            //DAL.Ledger PL = new DAL.Ledger();
            //PL.LedgerName = "Profit & Loss A/C";
            //PL.AccountGroupId = pr.Id;
           
            //DB.Ledgers.Add(PL);
            //DB.SaveChanges();
        }

        void AccountSetup_Asset(DAL.AccountGroup pr)
        {           
            DAL.AccountGroup ast = new DAL.AccountGroup();
            ast.GroupName = "Assets";
            ast.GroupCode = "100";
            ast.CompanyId = pr.CompanyId;
            ast.UnderGroupId = pr.Id;
            DB.AccountGroups.Add(ast);
            DB.SaveChanges();

            #region Current Assets
            DAL.AccountGroup ca = new DAL.AccountGroup();
            ca.GroupName = "Current Assets";
            ca.GroupCode = "110";
            ca.UnderGroupId = ast.Id;
            ca.CompanyId = pr.CompanyId;            
            DB.AccountGroups.Add(ca);
            DB.SaveChanges();


            DAL.AccountGroup ch = new DAL.AccountGroup();
            ch.GroupName = "Cash-in-Hand";
            ch.GroupCode = "111";
            ch.UnderGroupId = ca.Id;
            ch.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(ch);
            DB.SaveChanges();

            DAL.Ledger cL = new DAL.Ledger();
            cL.LedgerName = "Cash Ledger";
            cL.AccountGroupId = ch.Id;
         
            DB.Ledgers.Add(cL);
            DB.SaveChanges();

            DAL.AccountGroup dp = new DAL.AccountGroup();
            dp.GroupName = "Deposits";
            dp.GroupCode = "112";
            dp.UnderGroupId = ca.Id;
            dp.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(dp);
            DB.SaveChanges();

            DAL.AccountGroup la = new DAL.AccountGroup();
            la.GroupName = "Loans and Advances";
            la.GroupCode = "113";
            la.UnderGroupId = ca.Id;
            la.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(la);
            DB.SaveChanges();

            DAL.AccountGroup ba = new DAL.AccountGroup();
            ba.GroupName = "Bank Accounts";
            ba.GroupCode = "114";
            ba.UnderGroupId = ca.Id;
            ba.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(ba);
            DB.SaveChanges();

            DAL.AccountGroup SIH = new DAL.AccountGroup();
            SIH.GroupName = "Stock-In-Hand";
            SIH.GroupCode = "115";
            SIH.UnderGroupId = ca.Id;
            SIH.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(SIH);
            DB.SaveChanges();

            DAL.AccountGroup sd = new DAL.AccountGroup();
            sd.GroupName = "Sundry Debtors";
            sd.GroupCode = "116";
            sd.UnderGroupId = ca.Id;
            sd.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(sd);
            DB.SaveChanges();

           

            #endregion

            #region Fixed Assets

            DAL.AccountGroup fa = new DAL.AccountGroup();
            fa.GroupName = "Fixed Assets";
            fa.GroupCode = "120";
            fa.UnderGroupId = ast.Id;
            fa.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(fa);
            DB.SaveChanges();

            #endregion


            #region Misc. Expenses

            DAL.AccountGroup me = new DAL.AccountGroup();
            me.GroupName = "Misc. Expenses";
            me.GroupCode = "130";
            me.UnderGroupId = ast.Id;
            me.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(me);
            DB.SaveChanges();

            #endregion

            DAL.AccountGroup Inv = new DAL.AccountGroup();
            Inv.GroupName = "Investments";
            Inv.GroupCode = "140";
            Inv.UnderGroupId = ast.Id;
            Inv.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(Inv);
            DB.SaveChanges();

        }

        void AccountSetup_Liabilities(DAL.AccountGroup pr)
        {
            DAL.AccountGroup liab = new DAL.AccountGroup();
            liab.GroupName = "Liabilities";
            liab.GroupCode = "200";
            liab.CompanyId = pr.CompanyId;
            liab.UnderGroupId = pr.Id;
            DB.AccountGroups.Add(liab);
            DB.SaveChanges();

            #region Current Liabilities
            DAL.AccountGroup cl = new DAL.AccountGroup();
            cl.GroupName = "Current Liabilities";
            cl.GroupCode = "210";
            cl.UnderGroupId = liab.Id;
            cl.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(cl);
            DB.SaveChanges();

            DAL.AccountGroup DT = new DAL.AccountGroup();
            DT.GroupName = "Duties & Taxes";
            DT.GroupCode = "211";
            DT.UnderGroupId = cl.Id;
            DT.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(DT);
            DB.SaveChanges();

            DAL.AccountGroup prov = new DAL.AccountGroup();
            prov.GroupName = "Provisions";
            prov.GroupCode = "212";
            prov.UnderGroupId = cl.Id;
            prov.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(prov);
            DB.SaveChanges();

            DAL.AccountGroup sc = new DAL.AccountGroup();
            sc.GroupName = "Sundry Creditors";
            sc.GroupCode = "212";
            sc.UnderGroupId = cl.Id;
            sc.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(sc);
            DB.SaveChanges();


            #region Loans
            DAL.AccountGroup l = new DAL.AccountGroup();
            l.GroupName = "Loans";
            l.GroupCode = "220";
            l.UnderGroupId = liab.Id;
            l.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(l);
            DB.SaveChanges();

            DAL.AccountGroup BOAc = new DAL.AccountGroup();
            BOAc.GroupName = "Bank OD A/c";
            BOAc.GroupCode = "221";
            BOAc.UnderGroupId = l.Id;
            BOAc.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(BOAc);
            DB.SaveChanges();

            DAL.AccountGroup SL = new DAL.AccountGroup();
            SL.GroupName = "Secured Loans";
            SL.GroupCode = "221";
            SL.UnderGroupId = l.Id;
            SL.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(SL);
            DB.SaveChanges();

            DAL.AccountGroup USL = new DAL.AccountGroup();
            USL.GroupName = "UnSecured Loans";
            USL.GroupCode = "222";
            USL.UnderGroupId = l.Id;
            USL.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(USL);
            DB.SaveChanges();
            #endregion


            DAL.AccountGroup BD = new DAL.AccountGroup();
            BD.GroupName = "Branch /Divisions";
            BD.GroupCode = "230";
            BD.UnderGroupId = liab.Id;
            BD.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(BD);
            DB.SaveChanges();



            DAL.AccountGroup Cap = new DAL.AccountGroup();
            Cap.GroupName = "Capital Account";
            Cap.GroupCode = "240";
            Cap.UnderGroupId = liab.Id;
            Cap.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(Cap);
            DB.SaveChanges();

            DAL.AccountGroup RS = new DAL.AccountGroup();
            RS.GroupName = "Reserves & Surplus";
            RS.GroupCode = "250";
            RS.UnderGroupId = liab.Id;
            RS.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(RS);
            DB.SaveChanges();

            DAL.AccountGroup SAC = new DAL.AccountGroup();
            SAC.GroupName = "Suspense A/c";
            SAC.GroupCode = "260";
            SAC.UnderGroupId = liab.Id;
            SAC.CompanyId = pr.CompanyId;
            DB.AccountGroups.Add(SAC);
            DB.SaveChanges();

            #endregion
        }

        void AccountSetup_Income(DAL.AccountGroup pr)
        {
            DAL.AccountGroup Inc = new DAL.AccountGroup();
            Inc.GroupName = "Income";
            Inc.GroupCode = "300";
            Inc.CompanyId = pr.CompanyId;
            Inc.UnderGroupId = pr.Id;
            DB.AccountGroups.Add(Inc);
            DB.SaveChanges();

          

            #region Direct Income

            DAL.AccountGroup DInc = new DAL.AccountGroup();
            DInc.GroupName = "Direct Income";
            DInc.GroupCode = "310";
            DInc.CompanyId = pr.CompanyId;
            DInc.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(DInc);
            DB.SaveChanges();
            #endregion

            #region Indirect Income

            DAL.AccountGroup IndInc = new DAL.AccountGroup();
            IndInc.GroupName = "Indirect Income";
            IndInc.GroupCode = "320";
            IndInc.CompanyId = pr.CompanyId;
            IndInc.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(IndInc);
            DB.SaveChanges();
            #endregion

            DAL.AccountGroup Sa = new DAL.AccountGroup();
            Sa.GroupName = "Sales Account";
            Sa.GroupCode = "330";
            Sa.CompanyId = pr.CompanyId;
            Sa.UnderGroupId = Inc.Id;
            DB.AccountGroups.Add(Sa);
            DB.SaveChanges();
        }

        void AccountSetup_Expense(DAL.AccountGroup pr)
        {
            DAL.AccountGroup Exp = new DAL.AccountGroup();
            Exp.GroupName = "Expenses";
            Exp.GroupCode = "400";
            Exp.CompanyId = pr.CompanyId;
            Exp.UnderGroupId = pr.Id;
            DB.AccountGroups.Add(Exp);
            DB.SaveChanges();

            #region Direct Income

            DAL.AccountGroup DExp = new DAL.AccountGroup();
            DExp.GroupName = "Direct Expenses";
            DExp.GroupCode = "410";
            DExp.CompanyId = pr.CompanyId;
            DExp.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(DExp);
            DB.SaveChanges();
            #endregion

            #region Indirect Income

            DAL.AccountGroup IndExp = new DAL.AccountGroup();
            IndExp.GroupName = "Indirect Expense";
            IndExp.GroupCode = "320";
            IndExp.CompanyId = pr.CompanyId;
            IndExp.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(IndExp);
            DB.SaveChanges();
            #endregion

            DAL.AccountGroup Pur = new DAL.AccountGroup();
            Pur.GroupName = "Purchase Account";
            Pur.GroupCode = "330";
            Pur.CompanyId = pr.CompanyId;
            Pur.UnderGroupId = Exp.Id;
            DB.AccountGroups.Add(Pur);
            DB.SaveChanges();
        }

        #endregion
    }
}
