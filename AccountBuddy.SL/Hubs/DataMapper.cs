using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountBuddy.SL.Hubs
{
    public static class DataMapper
    {
        #region Master

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

        #region CustomFormat
        public static DAL.CustomFormat ToMap(this BLL.CustomFormat S, DAL.CustomFormat D)
        {            
            D.CompanyId = S.CompanyId;
            D.CurrencyCaseSensitive = S.CurrencyCaseSensitive;
            D.CurrencyNegativeSymbolPrefix = S.CurrencyNegativeSymbolPrefix;
            D.CurrencyNegativeSymbolSuffix = S.CurrencyNegativeSymbolSuffix;
            D.CurrencyPositiveSymbolPrefix = S.CurrencyPositiveSymbolPrefix;
            D.CurrencyPositiveSymbolSuffix = S.CurrencyPositiveSymbolSuffix;
            D.CurrencyToWordPrefix = S.CurrencyToWordPrefix;
            D.CurrencyToWordSuffix = S.CurrencyToWordSuffix;
            D.DecimalSymbol = S.DecimalSymbol;
            D.DecimalToWordPrefix = S.DecimalToWordPrefix;
            D.DecimalToWordSuffix = S.DecimalToWordSuffix;
            D.DigitGroupingBy = S.DigitGroupingBy;
            D.DigitGroupingSymbol = S.DigitGroupingSymbol;
            D.Id = S.Id;
            D.IsDisplayWithOnlyOnSuffix = S.IsDisplayWithOnlyOnSuffix;
            D.NoOfDigitAfterDecimal = S.NoOfDigitAfterDecimal;
         
            return D;
        }
        public static BLL.CustomFormat ToMap(this DAL.CustomFormat S, BLL.CustomFormat D)
        {
            D.CompanyId = S.CompanyId??0;
            D.CurrencyCaseSensitive = S.CurrencyCaseSensitive??0;
            D.CurrencyNegativeSymbolPrefix = S.CurrencyNegativeSymbolPrefix;
            D.CurrencyNegativeSymbolSuffix = S.CurrencyNegativeSymbolSuffix;
            D.CurrencyPositiveSymbolPrefix = S.CurrencyPositiveSymbolPrefix;
            D.CurrencyPositiveSymbolSuffix = S.CurrencyPositiveSymbolSuffix;
            D.CurrencyToWordPrefix = S.CurrencyToWordPrefix;
            D.CurrencyToWordSuffix = S.CurrencyToWordSuffix;
            D.DecimalSymbol = S.DecimalSymbol;
            D.DecimalToWordPrefix = S.DecimalToWordPrefix;
            D.DecimalToWordSuffix = S.DecimalToWordSuffix;
            D.DigitGroupingBy = S.DigitGroupingBy??0;
            D.DigitGroupingSymbol = S.DigitGroupingSymbol;
            D.Id = S.Id;
            D.IsDisplayWithOnlyOnSuffix = S.IsDisplayWithOnlyOnSuffix??false;
            D.NoOfDigitAfterDecimal = S.NoOfDigitAfterDecimal??0;

            return D;
        }
        #endregion

        #region CreditLimitType
        public static DAL.CreditLimitType ToMap(this BLL.CreditLimitType S, DAL.CreditLimitType D)
        {
            D.Id = S.Id;
            D.LimitType = S.LimitType;
            return D;
        }
        public static BLL.CreditLimitType ToMap(this DAL.CreditLimitType S, BLL.CreditLimitType D)
        {
            D.Id = S.Id;
            D.LimitType = S.LimitType;            
            return D;
        }
        #endregion
        #region DataKeyValue

        public static DAL.DataKeyValue ToMap(this BLL.DataKeyValue S, DAL.DataKeyValue D)
        {
            D.CompanyId = S.CompanyId;
            D.DataKey = S.DataKey;
            D.DataValue = S.DataValue;
            D.Id = S.Id;          
            return D;
        }

        public static BLL.DataKeyValue ToMap(this DAL.DataKeyValue S, BLL.DataKeyValue D)
        {
            D.CompanyId = S.CompanyId;
            D.DataKey = S.DataKey;
            D.DataValue = S.DataValue;
            D.Id = S.Id;
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
            D.CreditAmount = S.CreditAmount ?? 0;
            D.CreditLimit = S.CreditLimit ?? 0;
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

        #endregion

        #region Transaction 

        #region PurchaseOrder
        public static BLL.PurchaseOrder ToMap(DAL.PurchaseOrder S, BLL.PurchaseOrder D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.Extras = S.Extras;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.PODate = S.PODate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.PurchaseOrder ToMap(BLL.PurchaseOrder S, DAL.PurchaseOrder D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount ?? 0;
            D.Extras = S.Extras ?? 0;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ItemAmount = S.ItemAmount ?? 0;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.PODate = (DateTime)S.PODate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount ?? 0;

            return D;
        }

        public static BLL.PurchaseOrderDetail ToMap(DAL.PurchaseOrderDetail S, BLL.PurchaseOrderDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.POId = S.POId;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.PurchaseOrderDetail ToMap(BLL.PurchaseOrderDetail S, DAL.PurchaseOrderDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.POId = S.POId;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        #endregion

        #region Purchase
        public static BLL.Purchase ToMap(DAL.Purchase S, BLL.Purchase D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.PurchaseDate = S.PurchaseDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.Purchase ToMap(BLL.Purchase S, DAL.Purchase D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.PurchaseDate = (DateTime)S.PurchaseDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }

        public static BLL.PurchaseDetail ToMap(DAL.PurchaseDetail S, BLL.PurchaseDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.PODId = S.PODId;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.PurchaseDetail ToMap(BLL.PurchaseDetail S, DAL.PurchaseDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.PODId = S.PODId;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        #endregion

        #region PurchaseReturn
        public static BLL.PurchaseReturn ToMap(DAL.PurchaseReturn S, BLL.PurchaseReturn D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.PRDate = S.PRDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.PurchaseReturn ToMap(BLL.PurchaseReturn S, DAL.PurchaseReturn D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.PRDate = (DateTime)S.PRDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }

        public static BLL.PurchaseReturnDetail ToMap(DAL.PurchaseReturnDetail S, BLL.PurchaseReturnDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.PurchaseReturnDetail ToMap(BLL.PurchaseReturnDetail S, DAL.PurchaseReturnDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        #endregion

        #region Sales Order
        public static BLL.SalesOrder ToMap(DAL.SalesOrder S, BLL.SalesOrder D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.SODate = S.SODate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.SalesOrder ToMap(BLL.SalesOrder S, DAL.SalesOrder D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount ?? 0;
            D.ExtraAmount = S.ExtraAmount ?? 0;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ItemAmount = S.ItemAmount ?? 0;
            D.LedgerId = S.LedgerId ?? 0;
            D.Narration = S.Narration;
            D.SODate = (DateTime)S.SODate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount ?? 0;

            return D;
        }

        public static BLL.SalesOrderDetail ToMap(DAL.SalesOrderDetail S, BLL.SalesOrderDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.SalesOrderDetail ToMap(BLL.SalesOrderDetail S, DAL.SalesOrderDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount ?? 0;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ProductId = S.ProductId ?? 0;
            D.Quantity = S.Quantity ?? 0;
            D.UnitPrice = S.UnitPrice ?? 0;
            D.UOMId = S.UOMId ?? 0;
            return D;
        }
        #endregion

        #region Sales
        public static BLL.Sale ToMap(DAL.Sale S, BLL.Sale D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.SalesDate = S.SalesDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.Sale ToMap(BLL.Sale S, DAL.Sale D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.SalesDate = (DateTime)S.SalesDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }

        public static BLL.SalesDetail ToMap(DAL.SalesDetail S, BLL.SalesDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.SalesDetail ToMap(BLL.SalesDetail S, DAL.SalesDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        #endregion

        #region Sales Return
        public static BLL.SalesReturn ToMap(DAL.SalesReturn S, BLL.SalesReturn D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.SRDate = S.SRDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.SalesReturn ToMap(BLL.SalesReturn S, DAL.SalesReturn D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.SRDate = (DateTime)S.SRDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }

        public static BLL.SalesReturnDetail ToMap(DAL.SalesReturnDetail S, BLL.SalesReturnDetail D)
        {
            D.Id = S.Id;
            D.Amount = (decimal)S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.SalesReturnDetail ToMap(BLL.SalesReturnDetail S, DAL.SalesReturnDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        #endregion

        #region JobOrder Issue
        public static BLL.JobOrderIssue ToMap(DAL.JobOrderIssue S, BLL.JobOrderIssue D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.Extras = S.Extras;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.JobWorkerId = S.JobWorkerId;
            D.Narration = S.Narration;
            D.JODate = S.JODate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;


            return D;
        }
        public static DAL.JobOrderIssue ToMap(BLL.JobOrderIssue S, DAL.JobOrderIssue D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.Extras = S.Extras ?? 0;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ItemAmount = S.ItemAmount ?? 0;
            D.JobWorkerId = S.JobWorkerId ?? 0;
            D.Narration = S.Narration;
            D.JODate = (DateTime)S.JODate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount ?? 0;

            return D;
        }

        public static BLL.JobOrderIssueDetail ToMap(DAL.JobOrderIssueDetail S, BLL.JobOrderIssueDetail D)
        {
            D.Id = S.Id;
            D.Amount = (decimal)S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.JobOrderIssueDetail ToMap(BLL.JobOrderIssueDetail S, DAL.JobOrderIssueDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount ?? 0;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ProductId = S.ProductId ?? 0;
            D.Quantity = S.Quantity ?? 0;
            D.UnitPrice = S.UnitPrice ?? 0;
            D.UOMId = S.UOMId ?? 0;
            return D;
        }
        #endregion

        #region JobOrderReceived
        public static BLL.JobOrderReceived ToMap(DAL.JobOrderReceived S, BLL.JobOrderReceived D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.JobWorkerId = S.JobWorkerId;
            D.Narration = S.Narration;
            D.JRDate = S.JRDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.JobOrderReceived ToMap(BLL.JobOrderReceived S, DAL.JobOrderReceived D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount ?? 0;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ExtraAmount = S.ExtraAmount ?? 0;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ItemAmount = S.ItemAmount ?? 0;
            D.JobWorkerId = S.JobWorkerId ?? 0;
            D.Narration = S.Narration;
            D.JRDate = (DateTime)S.JRDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount ?? 0;

            return D;
        }

        public static BLL.JobOrderReceivedDetail ToMap(DAL.JobOrderReceivedDetail S, BLL.JobOrderReceivedDetail D)
        {
            D.Id = S.Id;
            D.Amount = (decimal)S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.JobOrderReceivedDetail ToMap(BLL.JobOrderReceivedDetail S, DAL.JobOrderReceivedDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount ?? 0;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ProductId = S.ProductId ?? 0;
            D.Quantity = S.Quantity ?? 0;
            D.UnitPrice = S.UnitPrice ?? 0;
            D.UOMId = S.UOMId ?? 0;
            return D;
        }
        #endregion

        #region StockIn
        public static BLL.StockIn ToMap(DAL.StockIn S, BLL.StockIn D)
        {
            D.Id = S.Id;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.Date = S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.ItemAmount = S.ItemAmount;

            return D;
        }
        public static DAL.StockIn ToMap(BLL.StockIn S, DAL.StockIn D)
        {
            D.Id = S.Id;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.Date = (DateTime)S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.ItemAmount = S.ItemAmount;

            return D;
        }

        public static BLL.StockInDetail ToMap(DAL.StockInDetail S, BLL.StockInDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.StockInDetail ToMap(BLL.StockInDetail S, DAL.StockInDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        #endregion

        #region StockOut
        public static BLL.StockOut ToMap(DAL.StockOut S, BLL.StockOut D)
        {
            D.Id = S.Id;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.Date = S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.ItemAmount = S.ItemAmount;

            return D;
        }
        public static DAL.StockOut ToMap(BLL.StockOut S, DAL.StockOut D)
        {
            D.Id = S.Id;
            D.ItemAmount = S.ItemAmount;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.Date = (DateTime)S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.ItemAmount = S.ItemAmount;

            return D;
        }

        public static BLL.StockOutDetail ToMap(DAL.StockOutDetail S, BLL.StockOutDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.StockOutDetail ToMap(BLL.StockOutDetail S, DAL.StockOutDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        #endregion

        #region StockInProcess
        public static BLL.StockInProcess ToMap(DAL.StockInProcess S, BLL.StockInProcess D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.Extras;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.StaffId = S.StaffId;
            D.Narration = S.Narration;
            D.SPDate = S.SPDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.StockInProcess ToMap(BLL.StockInProcess S, DAL.StockInProcess D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount ?? 0;
            D.Extras = S.ExtraAmount ?? 0;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ItemAmount = S.ItemAmount ?? 0;
            D.StaffId = S.StaffId ?? 0;
            D.Narration = S.Narration;
            D.SPDate = (DateTime)S.SPDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount ?? 0;

            return D;
        }

        public static BLL.StockInProcessDetail ToMap(DAL.StockInProcessDetail S, BLL.StockInProcessDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.StockInProcessDetail ToMap(BLL.StockInProcessDetail S, DAL.StockInProcessDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount ?? 0;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ProductId = S.ProductId ?? 0;
            D.Quantity = S.Quantity ?? 0;
            D.UnitPrice = S.UnitPrice ?? 0;
            D.UOMId = S.UOMId ?? 0;
            return D;
        }
        #endregion

        #region StockSeperated
        public static BLL.StockSeperated ToMap(DAL.StockSeparated S, BLL.StockSeperated D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.StaffId = S.StaffId;
            D.Narration = S.Narration;
            D.Date = S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static DAL.StockSeparated ToMap(BLL.StockSeperated S, DAL.StockSeparated D)
        {
            D.Id = S.Id;
            D.DiscountAmount = S.DiscountAmount ?? 0;
            D.ExtraAmount = S.ExtraAmount ?? 0;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ItemAmount = S.ItemAmount ?? 0;
            D.StaffId = S.StaffId ?? 0;
            D.Narration = S.Narration;
            D.Date = (DateTime)S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TotalAmount = S.TotalAmount ?? 0;

            return D;
        }

        public static BLL.StockSeperatedDetail ToMap(DAL.StockSeperatedDetail S, BLL.StockSeperatedDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            return D;
        }
        public static DAL.StockSeperatedDetail ToMap(BLL.StockSeperatedDetail S, DAL.StockSeperatedDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount ?? 0;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount ?? 0;
            D.ProductId = S.ProductId ?? 0;
            D.Quantity = S.Quantity ?? 0;
            D.UnitPrice = S.UnitPrice ?? 0;
            D.UOMId = S.UOMId ?? 0;
            return D;
        }
        #endregion

        #region Payment
        public static BLL.Payment ToMap(DAL.Payment S, BLL.Payment D)
        {
            D.Id = S.Id;
            D.EntryNo = S.EntryNo;
            D.Amount = S.Amount;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;
            D.ClearDate = S.ClearDate;
            D.ExtraCharge = S.ExtraCharge;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.PaymentDate = S.PaymentDate;
            D.PaymentMode = S.PaymentMode;
            D.PayTo = S.PayTo;
            D.RefNo = S.RefNo;
            D.Status = S.Status;
            D.VoucherNo = S.VoucherNo;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            return D;
        }
        public static DAL.Payment ToMap(BLL.Payment S, DAL.Payment D)
        {
            D.Id = S.Id;
            D.EntryNo = S.EntryNo;
            D.Amount = S.Amount;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;
            D.ClearDate = S.ClearDate;
            D.ExtraCharge = S.ExtraCharge;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.PaymentDate = S.PaymentDate;
            D.PaymentMode = S.PaymentMode;
            D.PayTo = S.PayTo;
            D.RefNo = S.RefNo;
            D.Status = S.Status;
            D.VoucherNo = S.VoucherNo;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            return D;
        }

        public static BLL.PaymentDetail ToMap(DAL.PaymentDetail S, BLL.PaymentDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.LedgerId = S.LedgerId;
            D.Particular = S.Particular;
            D.PaymentId = S.PaymentId;
            return D;
        }
        public static DAL.PaymentDetail ToMap(BLL.PaymentDetail S, DAL.PaymentDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.LedgerId = S.LedgerId;
            D.Particular = S.Particular;
            D.PaymentId = S.PaymentId;
            return D;
        }
        #endregion

        #region Receipt
        public static BLL.Receipt ToMap(DAL.Receipt S, BLL.Receipt D)
        {
            D.Id = S.Id;
            D.EntryNo = S.EntryNo;
            D.Amount = S.Amount;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;
            D.CleareDate = S.CleareDate;
            D.ExtraCharge = S.Extracharge;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.ReceiptDate = S.ReceiptDate;
            D.ReceiptMode = S.ReceiptMode;
            D.ReceivedFrom = S.ReceivedFrom;
            D.RefNo = S.RefNo;
            D.Status = S.Status;
            D.VoucherNo = S.VoucherNo;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            return D;
        }
        public static DAL.Receipt ToMap(BLL.Receipt S, DAL.Receipt D)
        {
            D.Id = S.Id;
            D.EntryNo = S.EntryNo;
            D.Amount = S.Amount;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;
            D.CleareDate = S.CleareDate;
            D.Extracharge = S.ExtraCharge;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.ReceiptDate = S.ReceiptDate;
            D.ReceiptMode = S.ReceiptMode;
            D.ReceivedFrom = S.ReceivedFrom;
            D.RefNo = S.RefNo;
            D.Status = S.Status;
            D.VoucherNo = S.VoucherNo;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            return D;
        }

        public static BLL.ReceiptDetail ToMap(DAL.ReceiptDetail S, BLL.ReceiptDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.ReceiptId = S.ReceiptId;
            return D;
        }
        public static DAL.ReceiptDetail ToMap(BLL.ReceiptDetail S, DAL.ReceiptDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.ReceiptId = S.ReceiptId;
            return D;
        }
        #endregion

        #region Journal
        public static BLL.Journal ToMap(DAL.Journal S, BLL.Journal D)
        {
            D.Id = S.Id;
            D.EntryNo = S.EntryNo;
            D.Amount = S.Amount;
            D.JournalDate = S.JournalDate;
            D.HQNo = S.HQNo;
            D.Particular = S.Particular;
            D.RefCode = S.RefCode;
            D.VoucherNo = S.VoucherNo;
            
            return D;
        }
        public static DAL.Journal ToMap(BLL.Journal S, DAL.Journal D)
        {
            D.Id = S.Id;
            D.EntryNo = S.EntryNo;
            D.Amount = S.Amount;
            D.JournalDate = S.JournalDate;
            D.HQNo = S.HQNo;
            D.Particular = S.Particular;
            D.RefCode = S.RefCode;
            D.VoucherNo = S.VoucherNo;
            return D;
        }

        public static BLL.JournalDetail ToMap(DAL.JournalDetail S, BLL.JournalDetail D)
        {
            D.Id = S.Id;
            D.CrAmt = S.CrAmt;
            D.DrAmt = S.DrAmt;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;
            D.ClearDate = S.ClearDate;
            D.ExtraCharge = S.ExtraCharge;
            D.JournalId = S.JournalId;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.JournalId = S.JournalId;
            D.RefNo = S.RefNo;
            D.Status = S.Status;
            D.TransactionMode = S.TransactionMode;
            
            return D;
        }
        public static DAL.JournalDetail ToMap(BLL.JournalDetail S, DAL.JournalDetail D)
        {
            D.Id = S.Id;
            D.CrAmt = S.CrAmt;
            D.DrAmt = S.DrAmt;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;
            D.ClearDate = S.ClearDate;
            D.ExtraCharge = S.ExtraCharge;
            D.JournalId = S.JournalId;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.JournalId = S.JournalId;
            D.RefNo = S.RefNo;
            D.Status = S.Status;
            D.TransactionMode = S.TransactionMode;
            return D;
        }
        #endregion

        #endregion


    }
}