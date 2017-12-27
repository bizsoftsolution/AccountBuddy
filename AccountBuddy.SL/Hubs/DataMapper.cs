using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public static class DataMapper
    {
        #region CompanyDetails
        public static DAL.CompanyDetail ToMap(this BLL.CompanyDetail S, DAL.CompanyDetail D)
        {
            D.Id = S.Id;
            D.AddressLine1 = S.AddressLine1;
            D.AddressLine2 = S.AddressLine2;
            D.CityName = S.CityName;
            D.CompanyName = S.CompanyName;
            D.CompanyType = S.CompanyType;
            D.EMailId = S.EMailId;
            D.GSTNo = S.GSTNo;
            D.IsActive = S.IsActive;
            D.Logo = S.Logo;
            D.MobileNo = S.MobileNo;
            D.PostalCode = S.PostalCode;
            D.TelephoneNo = S.TelephoneNo;
            D.UnderCompanyId = S.UnderCompanyId;

            return D;
        }
        public static BLL.CompanyDetail ToMap(this DAL.CompanyDetail S, BLL.CompanyDetail D)
        {
            D.Id = S.Id;
            D.AddressLine1 = S.AddressLine1;
            D.AddressLine2 = S.AddressLine2;
            D.CityName = S.CityName;
            D.CompanyName = S.CompanyName;
            D.CompanyType = S.CompanyType;
            D.EMailId = S.EMailId;
            D.GSTNo = S.GSTNo;
            D.IsActive = S.IsActive;
            D.Logo = S.Logo;
            D.MobileNo = S.MobileNo;
            D.PostalCode = S.PostalCode;
            D.TelephoneNo = S.TelephoneNo;
            D.UnderCompanyId = S.UnderCompanyId;

            return D;
        }
        #endregion

        #region UserType
        public static BLL.UserType ToMap(this DAL.UserType S, BLL.UserType D)
        {
            D.CompanyId = S.CompanyId;
            D.Description = S.Description;
            D.Id = S.Id;
            D.TypeOfUser = S.TypeOfUser;
            return D;
        }
        public static DAL.UserType ToMap(this BLL.UserType S, DAL.UserType D)
        {
            D.CompanyId = S.CompanyId;
            D.Description = S.Description;
            D.Id = S.Id;
            D.TypeOfUser = S.TypeOfUser;
            return D;
        }
        #endregion

        #region UserTypeDetail
        public static BLL.UserTypeDetail ToMap(this DAL.UserTypeDetail S, BLL.UserTypeDetail D)
        {
            D.AllowDelete = S.AllowDelete;
            D.AllowInsert = S.AllowInsert;
            D.AllowUpdate = S.AllowUpdate;
            D.Id = S.Id;
            D.IsViewForm = S.IsViewForm;
            D.UserTypeFormDetailId = S.UserTypeFormDetailId;
            D.UserTypeId = S.UserTypeId;

            return D;
        }
        public static DAL.UserTypeDetail ToMap(this BLL.UserTypeDetail S, DAL.UserTypeDetail D)
        {
            D.AllowDelete = S.AllowDelete;
            D.AllowInsert = S.AllowInsert;
            D.AllowUpdate = S.AllowUpdate;
            D.Id = S.Id;
            D.IsViewForm = S.IsViewForm;
            D.UserTypeFormDetailId = S.UserTypeFormDetailId;
            D.UserTypeId = S.UserTypeId;

            return D;
        }
        #endregion

        #region UserTypeFormDetail
        public static BLL.UserTypeFormDetail ToMap(this DAL.UserTypeFormDetail S, BLL.UserTypeFormDetail D)
        {
            D.Description = S.Description;
            D.FormName = S.FormName;
            D.FormType = S.FormType;
            D.Id = S.Id;
            D.IsActive = S.IsActive;
            D.IsDelete = S.IsDelete;
            D.IsInsert = S.IsInsert;
            D.IsMenu = S.IsMenu;
            D.IsUpdate = S.IsUpdate;
            return D;
        }
        public static DAL.UserTypeFormDetail ToMap(this BLL.UserTypeFormDetail S, DAL.UserTypeFormDetail D)
        {
            D.Description = S.Description;
            D.FormName = S.FormName;
            D.FormType = S.FormType;
            D.Id = S.Id;
            D.IsActive = S.IsActive;
            D.IsDelete = S.IsDelete;
            D.IsInsert = S.IsInsert;
            D.IsMenu = S.IsMenu;
            D.IsUpdate = S.IsUpdate;
            return D;
        }
        #endregion

        #region UserAccount
        public static BLL.UserAccount ToMap(this DAL.UserAccount S, BLL.UserAccount D)
        {
            D.Id = S.Id;
            D.LoginId = S.LoginId;
            D.Password = S.Password;
            D.UserName = S.UserName;
            D.UserTypeId = D.UserTypeId;

            return D;
        }
        public static DAL.UserAccount ToMap(this BLL.UserAccount S, DAL.UserAccount D)
        {
            D.Id = S.Id;
            D.LoginId = S.LoginId;
            D.Password = S.Password;
            D.UserName = S.UserName;

            D.UserTypeId = S.UserTypeId;

            return D;
        }
        #endregion

        #region StockGroup
        public static BLL.StockGroup ToMap(this DAL.StockGroup S, BLL.StockGroup D)
        {
            D.CompanyId = S.CompanyId;
            D.GroupCode = S.GroupCode;
            D.Id = S.Id;
            D.IsPurchase = S.IsPurchase ?? false;
            D.IsSale = S.IsSale ?? false;
            D.StockGroupName = S.StockGroupName;
            D.UnderGroupId = S.UnderGroupId;
            return D;
        }
        public static DAL.StockGroup ToMap(this BLL.StockGroup S, DAL.StockGroup D)
        {
            D.CompanyId = S.CompanyId;
            D.GroupCode = S.GroupCode;
            D.Id = S.Id;
            D.IsPurchase = S.IsPurchase;
            D.IsSale = S.IsSale;
            D.StockGroupName = S.StockGroupName;
            D.UnderGroupId = S.UnderGroupId;
            return D;
        }
        #endregion

        #region AccountGroup
        public static BLL.AccountGroup ToMap(this DAL.AccountGroup S, BLL.AccountGroup D)
        {
            D.CompanyId = S.CompanyId;
            D.GroupCode = S.GroupCode;
            D.GroupName = S.GroupName;
            D.Id = S.Id;
            D.UnderGroupId = S.UnderGroupId;
            return D;
        }
        public static DAL.AccountGroup ToMap(this BLL.AccountGroup S, DAL.AccountGroup D)
        {
            D.CompanyId = S.CompanyId;
            D.GroupCode = S.GroupCode;
            D.GroupName = S.GroupName;
            D.Id = S.Id;
            D.UnderGroupId = S.UnderGroupId;
            return D;
        }
        #endregion

        #region UOM
        public static BLL.UOM ToMap(this DAL.UOM S, BLL.UOM D)
        {

            D.CompanyId = S.CompanyId;
            D.FormalName = S.FormalName;
            D.Id = S.Id;
            D.Symbol = S.Symbol;

            return D;
        }
        public static DAL.UOM ToMap(this BLL.UOM S, DAL.UOM D)
        {
            D.CompanyId = S.CompanyId;
            D.FormalName = S.FormalName;
            D.Id = S.Id;
            D.Symbol = S.Symbol;
            return D;
        }
        #endregion

        #region Product
        public static BLL.Product ToMap(this DAL.Product S, BLL.Product D)
        {
            D.Id = S.Id;
            D.ItemCode = S.ItemCode;
            D.MaxSellingRate = S.MaxSellingRate;
            D.MinSellingRate = S.MinSellingRate;
            D.MRP = S.MRP;
            D.ProductImage = S.ProductImage;
            D.ProductName = S.ProductName;
            D.PurchaseRate = S.PurchaseRate;
            D.SellingRate = S.SellingRate;
            D.StockGroupId = S.StockGroupId;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.Product ToMap(this BLL.Product S, DAL.Product D)
        {
            D.Id = S.Id;
            D.ItemCode = S.ItemCode;
            D.MaxSellingRate = S.MaxSellingRate;
            D.MinSellingRate = S.MinSellingRate;
            D.MRP = S.MRP;
            D.ProductImage = S.ProductImage;
            D.ProductName = S.ProductName;
            D.PurchaseRate = S.PurchaseRate;
            D.SellingRate = S.SellingRate;
            D.StockGroupId = S.StockGroupId;
            D.UOMId = S.UOMId;
            return D;
        }
        #endregion

        #region Ledger
        public static BLL.Ledger ToMap(this DAL.Ledger S, BLL.Ledger D)
        {
            D.Id = S.Id;
            D.AccountGroupId = S.AccountGroupId;
            D.AddressLine1 = S.AddressLine1;
            D.AddressLine2 = S.AddressLine2;
            D.CityName = S.CityName;
            D.CreditAmount = S.CreditAmount.Value;
            D.CreditLimit = S.CreditLimit.Value;
            D.CreditLimitTypeId = S.CreditLimitTypeId;
            D.EMailId = S.EMailId;
            D.LedgerCode = S.LedgerCode;
            D.GSTNo = S.GSTNo;
            D.LedgerName = S.LedgerName;
            D.MobileNo = S.MobileNo;
            D.OPCr = S.OPCr;
            D.OPDr = S.OPDr;
            D.PersonIncharge = S.PersonIncharge;
            D.TelephoneNo = S.TelephoneNo;
            return D;
        }
        public static DAL.Ledger ToMap(this BLL.Ledger S, DAL.Ledger D)
        {
            D.Id = S.Id;
            D.AccountGroupId = S.AccountGroupId.Value;
            D.AddressLine1 = S.AddressLine1;
            D.AddressLine2 = S.AddressLine2;
            D.CityName = S.CityName;
            D.CreditAmount = S.CreditAmount;
            D.CreditLimit = S.CreditLimit;
            D.CreditLimitTypeId = S.CreditLimitTypeId;
            D.EMailId = S.EMailId;
            D.LedgerCode = S.LedgerCode;
            D.GSTNo = S.GSTNo;
            D.LedgerName = S.LedgerName;
            D.MobileNo = S.MobileNo;
            D.OPCr = S.OPCr;
            D.OPDr = S.OPDr;
            D.PersonIncharge = S.PersonIncharge;
            D.TelephoneNo = S.TelephoneNo;
            return D;
        }
        #endregion

        #region Bank
        public static BLL.Bank ToMap(this DAL.Bank S, BLL.Bank D)
        {
            D.Id = S.Id;
            D.AccountNo = S.AccountNo;
            D.BankAccountName = S.BankAccountName;
            D.LedgerId = S.LedgerId;
            return D;
        }
        public static DAL.Bank ToMap(this BLL.Bank S, DAL.Bank D)
        {
            D.Id = S.Id;
            D.AccountNo = S.AccountNo;
            D.BankAccountName = S.BankAccountName;
            D.LedgerId = S.LedgerId;
            return D;
        }
        #endregion

        #region Staff
        public static BLL.Staff ToMap(this DAL.Staff S, BLL.Staff D)
        {
            D.Id = S.Id;
            D.DepartmentId = S.DepartmentId.Value;
            D.Designation = S.Designation;
            D.DOB = S.DOB.Value;
            D.DOJ = S.DOJ.Value;
            D.LoginId = S.LoginId.Value;
            D.Salary = S.Salary.Value;
            D.LedgerId = S.LedgerId.Value;
            return D;
        }
        public static DAL.Staff ToMap(this BLL.Staff S, DAL.Staff D)
        {
            D.Id = S.Id;
            D.DepartmentId = S.DepartmentId;
            D.Designation = S.Designation;
            D.DOB = S.DOB;
            D.DOJ = S.DOJ;
            D.LoginId = S.LoginId;
            D.Salary = S.Salary;
            D.LedgerId = S.LedgerId;
            return D;
        }
        #endregion

        #region Supplier
        public static BLL.Supplier ToMap(this DAL.Supplier S, BLL.Supplier D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            return D;
        }
        public static DAL.Supplier ToMap(this BLL.Supplier S, DAL.Supplier D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            return D;
        }
        #endregion

        #region Customer
        public static BLL.Customer ToMap(this DAL.Customer S, BLL.Customer D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            return D;
        }
        public static DAL.Customer ToMap(this BLL.Customer S, DAL.Customer D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            return D;
        }
        #endregion

        #region JobWorker
        public static BLL.JobWorker ToMap(this DAL.JobWorker S, BLL.JobWorker D)
        {
            D.Id = S.Id;
            D.Role = S.Role;
            D.Salary = S.Salary.Value;
            D.LedgerId = S.LedgerId.Value;
            return D;
        }
        public static DAL.JobWorker ToMap(this BLL.JobWorker S, DAL.JobWorker D)
        {
            D.Id = S.Id;
            D.Role = S.Role;
            D.Salary = S.Salary;
            D.LedgerId = S.LedgerId;
            return D;
        }
        #endregion

        #region Department
        public static BLL.Department ToMap(this DAL.Department S, BLL.Department D)
        {
            D.Id = S.Id;
            D.Budget = S.Budget;
            D.CompanyId = S.CompanyId.Value;
            D.DepartmentName = S.DepartmentName;
            D.Description = S.Description;
            return D;
        }
        public static DAL.Department ToMap(this BLL.Department S, DAL.Department D)
        {
            D.Id = S.Id;
            D.Budget = S.Budget;
            D.CompanyId = S.CompanyId;
            D.DepartmentName = S.DepartmentName;
            D.Description = S.Description;
            return D;
        }
        #endregion
    }
}