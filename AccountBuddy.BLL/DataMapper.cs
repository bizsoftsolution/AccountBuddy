using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public static class DataMapper
    {
        public static CompanyDetail ToMap(this CompanyDetail S, CompanyDetail D)
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
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.LoginAccYear = S.LoginAccYear;
            D.Logo = S.Logo;
            D.MobileNo = S.MobileNo;
            D.Password = S.Password;
            D.PostalCode = S.PostalCode;
            D.TelephoneNo = S.TelephoneNo;
            D.UnderCompanyId = S.UnderCompanyId;
            D.UserId = S.UserId;
			D.CFiles = S.CFiles;

            return D;
        }

        public static CustomFormat ToMap(this CustomFormat S, CustomFormat D)
        {
            D.Company = S.Company;
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
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.NoOfDigitAfterDecimal = S.NoOfDigitAfterDecimal;
            D.SampleCurrency = S.SampleCurrency;
            D.SampleCurrencyNegative = S.SampleCurrencyNegative;
            D.SampleCurrencyPositive = S.SampleCurrencyPositive;

            return D;
        }

        public static UserType ToMap(this UserType S, UserType D)
        {

            D.Company = S.Company;
            D.CompanyId = S.CompanyId;
            D.Description = S.Description;
            D.Id = S.Id;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.lstValidation = S.lstValidation;
            D.TypeOfUser = S.TypeOfUser;
            D.UserTypeDetails = S.UserTypeDetails;

            return D;
        }

        public static UserTypeDetail ToMap(this UserTypeDetail S, UserTypeDetail D)
        {
            D.AllowDelete = S.AllowDelete;
            D.AllowInsert = S.AllowInsert;
            D.AllowUpdate = S.AllowUpdate;
            D.Id = S.Id;
            D.IsViewForm = S.IsViewForm;
            D.UserTypeFormDetail = S.UserTypeFormDetail;
            D.UserTypeFormDetailId = S.UserTypeFormDetailId;
            D.UserTypeId = S.UserTypeId;

            return D;
        }

        public static UserTypeFormDetail ToMap(this UserTypeFormDetail S, UserTypeFormDetail D)
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

        public static UserAccount ToMap(this UserAccount S, UserAccount D)
        {
            D.Id = S.Id;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.LoginId = S.LoginId;
            D.Password = S.Password;
            D.UserName = S.UserName;
            D.UserType = S.UserType;
            D.UserTypeId = D.UserTypeId;

            return D;
        }

        public static StockGroup ToMap(this StockGroup S, StockGroup D)
        {
            D.Company = S.Company;
            D.CompanyId = S.CompanyId;
            D.GroupCode = S.GroupCode;
            D.Id = S.Id;
            D.IsEnabled = S.IsEnabled;
            D.IsPurchase = S.IsPurchase;
            D.IsReadOnly = S.IsReadOnly;
            D.IsSale = S.IsSale;
            D.StockGroupName = S.StockGroupName;
            D.StockGroupNameWithCode = S.StockGroupNameWithCode;
            D.SubStockGroup = S.SubStockGroup;
            D.UnderGroupId = S.UnderGroupId;
            D.UnderStockGroup = S.UnderStockGroup;
            D.underStockGroupName = S.underStockGroupName;

            return D;
        }

        public static AccountGroup ToMap(this AccountGroup S, AccountGroup D)
        {
            D.Company = S.Company;
            D.CompanyId = S.CompanyId;
            D.GroupCode = S.GroupCode;
            D.GroupName = S.GroupName;
            D.GroupNameWithCode = S.GroupNameWithCode;
            D.Id = S.Id;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.SubAccountGroup = S.SubAccountGroup;
            D.UnderAccountGroup = S.UnderAccountGroup;
            D.UnderGroupId = S.UnderGroupId;
            D.underGroupName = S.underGroupName;

            return D;
        }

        public static UOM ToMap(this UOM S, UOM D)
        {
            D.Company = S.Company;
            D.CompanyId = S.CompanyId;
            D.FormalName = S.FormalName;
            D.Id = S.Id;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.Symbol = S.Symbol;
            return D;
        }

        public static Product ToMap(this Product S, Product D)
        {
            D.DiscountAmount = S.DiscountAmount;
            D.Id = S.Id;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.ItemCode = S.ItemCode;
            D.JOQty = S.JOQty;
            D.JRQty = S.JRQty;
            D.MaxSellingRate = S.MaxSellingRate;
            D.MinSellingRate = S.MinSellingRate;
            D.MRP = S.MRP;
            D.OpeningStock = S.OpeningStock;
            D.POQty = S.POQty;
            D.PQty = S.PQty;
            D.ProductImage = S.ProductImage;
            D.ProductName = S.ProductName;
            D.PRQty = S.PRQty;
            D.PurchaseRate = S.PurchaseRate;
            D.ReOrderLevel = S.ReOrderLevel;
            D.SellingRate = S.SellingRate;
            D.SInQty = S.SInQty;
            D.SOQty = S.SOQty;
            D.SOutQty = S.SOutQty;
            D.SPQty = S.SPQty;
            D.SQty = S.SQty;
            D.SRQty = S.SRQty;
            D.SRQtyForSales = S.SRQtyForSales;
            D.SRQtyNotForSales = S.SRQtyNotForSales;
            D.SSQty = S.SSQty;
            D.StockGroup = S.StockGroup;
            D.StockGroupId = S.StockGroupId;
            D.UOM = S.UOM;
            D.UOMId = S.UOMId;
            D.UOMName = S.UOMName;

            return D;
        }

        public static Ledger ToMap(this Ledger S, Ledger D)
        {
            D.Id = S.Id;
            D.AccountGroup = S.AccountGroup;
            D.AccountGroupId = S.AccountGroupId;
            D.AccountName = S.AccountName;
            D.ACType = S.ACType;
            D.AddressLine1 = S.AddressLine1;
            D.AddressLine2 = S.AddressLine2;
            D.CityName = S.CityName;
            D.CreditAmount = S.CreditAmount;
            D.CreditLimit = S.CreditLimit;
            D.CreditLimitType = S.CreditLimitType;
            D.CreditLimitTypeId = S.CreditLimitTypeId;
            D.CreditLimitTypeName = S.CreditLimitTypeName;
            D.EMailId = S.EMailId;
            D.GroupCode = S.GroupCode;
            D.GSTNo = S.GSTNo;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.LedgerCode = S.LedgerCode;
            D.LedgerName = S.LedgerName;
            D.MobileNo = S.MobileNo;
            D.OPBal = S.OPBal;
            D.OPCr = S.OPCr;
            D.OPDr = S.OPDr;
            D.PersonIncharge = S.PersonIncharge;
            D.TelephoneNo = S.TelephoneNo;


            return D;
        }

        public static Bank ToMap(this Bank S, Bank D)
        {
            D.Id = S.Id;
            D.AccountNo = S.AccountNo;
            D.BankAccountName = S.BankAccountName;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.Ledger = S.Ledger;
            D.LedgerId = S.LedgerId;
            return D;
        }

        public static Staff ToMap(this Staff S, Staff D)
        {
            D.Id = S.Id;
            D.DepartmentId = S.DepartmentId;
            D.Designation = S.Designation;
            D.DOB = S.DOB;
            D.DOJ = S.DOJ;
            D.LoginId = S.LoginId;
            D.Salary = S.Salary;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.Ledger = S.Ledger;
            D.LedgerId = S.LedgerId;
            return D;
        }

        public static Supplier ToMap(this Supplier S, Supplier D)
        {
            D.Id = S.Id;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.Ledger = S.Ledger;
            D.LedgerId = S.LedgerId;
            return D;
        }

        public static Customer ToMap(this Customer S, Customer D)
        {
            D.Id = S.Id;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.Ledger = S.Ledger;
            D.LedgerId = S.LedgerId;
            return D;
        }

        public static JobWorker ToMap(this JobWorker S, JobWorker D)
        {
            D.Id = S.Id;
            D.Role = S.Role;
            D.Salary = S.Salary;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.Ledger = S.Ledger;
            D.LedgerId = S.LedgerId;
            return D;
        }

        public static Department ToMap(this Department S, Department D)
        {
            D.Id = S.Id;
            D.Budget = S.Budget;
            D.Company = S.Company;
            D.CompanyId = S.CompanyId;
            D.DepartmentName = S.DepartmentName;
            D.Description = S.Description;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            return D;
        }

        public static TaxMaster ToMap(this TaxMaster S, TaxMaster D)
        {
            D.Id = S.Id;
            D.TaxPercentage = S.TaxPercentage;
            D.Ledger = S.Ledger;
            D.LedgerId = S.LedgerId;
            D.TaxPercentage = S.TaxPercentage;
            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            return D;
        }



        #region Transaction 

        #region PurchaseRequesting
        public static PurchaseRequest ToMap(this PurchaseRequest S, PurchaseRequest D)
        {
            D.Id = S.Id;
            D.AmountInwords = S.AmountInwords;
            D.DiscountAmount = S.DiscountAmount;
            D.Extras = S.Extras;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;

            D.LedgerId = S.LedgerId;
            D.LedgerName = S.LedgerName;
            D.Narration = S.Narration;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.Status = S.Status;
            D.TotalAmount = S.TotalAmount;

            return D;
        }
        public static PurchaseRequestDetail ToMap(this PurchaseRequestDetail S, PurchaseRequestDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemCode = S.ItemCode;
            D.POId = S.POId;

            D.ProductId = S.ProductId;
            D.ProductName = S.ProductName;
            D.Quantity = S.Quantity;

            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.UOMName = S.UOMName;


            return D;
        }
        #endregion

        #region Product_Spec_master
        public static Product_Spec_master ToMap(this Product_Spec_master S, Product_Spec_master D)
        {
            D.Id = S.Id;
            D.ProductId = S.ProductId;
            D.PDetail = S.PDetail;
            D.PDetails = S.PDetails;
            return D;
        }
        public static Product_Spec_Detail ToMap(this Product_Spec_Detail S, Product_Spec_Detail D)
        {
            //D.Product = S.Product;
            D.Id = S.Id;

            D.ProductId = S.ProductId;
            D.ProductName = S.ProductName;
            D.Qty = S.Qty;
            D.Product_Spec_Id = S.Product_Spec_Id;
            D.SNo = S.SNo;
            return D;
        }
        #endregion

        #region Product_Spec_Process
        public static Product_Spec_Process ToMap(this Product_Spec_Process S, Product_Spec_Process D)
        {
            D.Id = S.Id;
            D.ProductId = S.ProductId;
            D.Date = S.Date;
            D.Qty = S.Qty;
            D.PDetail = S.PDetail;
            D.PDetails = S.PDetails;
            return D;
        }
        public static Product_Spec_Process_Detail ToMap(this Product_Spec_Process_Detail S, Product_Spec_Process_Detail D)
        {
            D.Id = S.Id;
            D.ProductId = S.ProductId;
            ///D.ProductName = S.ProductName;
            //D.Product =S.Product;
            D.Qty = S.Qty;
            D.PSId = S.PSId;
            D.SNo = S.SNo;
            return D;
        }
        #endregion

        #region PurchaseOrder
        public static PurchaseOrder ToMap(this PurchaseOrder S, PurchaseOrder D)
        {
            D.Id = S.Id;
            D.AmountInwords = S.AmountInwords;
            D.DiscountAmount = S.DiscountAmount;
            D.Extras = S.Extras;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;
            D.LedgerId = S.LedgerId;
            D.LedgerName = S.LedgerName;
            D.Narration = S.Narration;
            D.PODate = S.PODate;
            D.PODetail = S.PODetail;
            D.PODetails = S.PODetails;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.Status = S.Status;
            D.TotalAmount = S.TotalAmount;


            return D;
        }
        public static PurchaseOrderDetail ToMap(this PurchaseOrderDetail S, PurchaseOrderDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemCode = S.ItemCode;
            D.POId = S.POId;

            D.ProductId = S.ProductId;
            D.ProductName = S.ProductName;
            D.Quantity = S.Quantity;
            D.SNo = S.SNo;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.UOMName = S.UOMName;


            return D;
        }
        #endregion

        #region Purchase
        public static BLL.Purchase ToMap(this Purchase S, Purchase D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            D.Narration = S.Narration;
            D.PurchaseDate = S.PurchaseDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TransactionType = S.TransactionType;
            D.TransactionTypeId = S.TransactionTypeId;

            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;
            D.BankName = S.BankName;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;

            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.TotalAmount = S.TotalAmount;

            D.IsShowChequeDetail = S.IsShowChequeDetail;
            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;
            D.PaidAmount = S.PaidAmount;
            D.PayAmount = S.PayAmount;
            D.PaymentLedgerId = S.PaymentLedgerId;

            return D;
        }
        public static BLL.PurchaseDetail ToMap(this PurchaseDetail S, PurchaseDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.PODId = S.PODId;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName;
            return D;
        }
        #endregion

        #region PurchaseReturn
        public static PurchaseReturn ToMap(this PurchaseReturn S, PurchaseReturn D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            D.PRDate = S.PRDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TransactionType = S.TransactionType;
            D.TransactionTypeId = S.TransactionTypeId;

            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;
            D.BankName = S.BankName;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;

            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.TotalAmount = S.TotalAmount;

            D.IsShowChequeDetail = S.IsShowChequeDetail;
            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;
            D.PaidAmount = S.PaidAmount;
            D.PayAmount = S.PayAmount;

            return D;
        }
        public static PurchaseReturnDetail ToMap(this PurchaseReturnDetail S, BLL.PurchaseReturnDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.IsResale = S.IsResale;
            D.Particulars = S.Particulars;
            D.PDId = S.PDId;
            D.PRId = S.PRId;
            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName;
            return D;
        }
        #endregion

        #region Sales Order
        public static SalesOrder ToMap(this SalesOrder S, SalesOrder D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            D.SODate = S.SODate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;

            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.TotalAmount = S.TotalAmount;

            D.Narration = S.Narration;
            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;
            D.Status = S.Status;
            D.SODetail = S.SODetail;
            D.SODetails = S.SODetails;
            return D;
        }
        public static SalesOrderDetail ToMap(this SalesOrderDetail S, SalesOrderDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.SOId = S.SOId;
            D.UOMName = S.UOMName;

            return D;
        }
        #endregion

        #region Sales
        public static Sale ToMap(this Sale S, Sale D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            D.SalesDate = S.SalesDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.TransactionType = S.TransactionType;
            D.TransactionTypeId = S.TransactionTypeId;

            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.TotalAmount = S.TotalAmount;


            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;
            D.BankName = S.BankName;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;

            D.IsShowChequeDetail = S.IsShowChequeDetail;
            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;
            D.PaidAmount = S.PaidAmount;
            D.PayAmount = S.PayAmount;
            return D;
        }
        public static SalesDetail ToMap(this SalesDetail S, SalesDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;

            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName; return D;
        }
        #endregion

        #region Sales Return
        public static SalesReturn ToMap(this SalesReturn S, SalesReturn D)
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
            D.TransactionTypeId = S.TransactionTypeId;
            D.TransactionType = S.TransactionType;
            return D;
        }
        public static SalesReturnDetail ToMap(this SalesReturnDetail S, SalesReturnDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = (decimal)S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.IsResale = S.IsResale;
            D.Particulars = S.Particulars;
            D.UOMName = S.UOMName; return D;
        }
        #endregion

        #region JobOrder Issue
        public static JobOrderIssue ToMap(this JobOrderIssue S, JobOrderIssue D)
        {
            D.Id = S.Id;
            D.JobWorkerId = S.JobWorkerId;
            D.JODate = S.JODate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;

            D.DiscountAmount = S.DiscountAmount;
            D.Extras = S.Extras;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.TotalAmount = S.TotalAmount;

            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;
            D.Status = S.Status;

            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;

            return D;
        }
        public static JobOrderIssueDetail ToMap(this JobOrderIssueDetail S, JobOrderIssueDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount ?? 0;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName; return D;
        }
        #endregion

        #region JobOrderReceived
        public static JobOrderReceived ToMap(this JobOrderReceived S, JobOrderReceived D)
        {
            D.Id = S.Id;
            D.JobWorkerId = S.JobWorkerId;
            D.JRDate = S.JRDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;

            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.TotalAmount = S.TotalAmount;

            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;

            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;

            return D;
        }
        public static JobOrderReceivedDetail ToMap(this JobOrderReceivedDetail S, JobOrderReceivedDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName; return D;
        }
        #endregion

        #region StockIn
        public static StockIn ToMap(this StockIn S, StockIn D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            D.Date = S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;

            D.ItemAmount = S.ItemAmount;

            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;

            return D;
        }
        public static StockInDetail ToMap(this StockInDetail S, StockInDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;

            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName;
            return D;
        }
        #endregion

        #region StockOut
        public static StockOut ToMap(this StockOut S, StockOut D)
        {
            D.Id = S.Id;
            D.LedgerId = S.LedgerId;
            D.Date = S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;
            D.ItemAmount = S.ItemAmount;
            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;
            D.Type = S.Type;
            return D;
        }
        public static StockOutDetail ToMap(this StockOutDetail S, StockOutDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;
            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName;
            return D;
        }
        #endregion

        #region StockInProcess
        public static StockInProcess ToMap(this StockInProcess S, StockInProcess D)
        {
            D.Id = S.Id;
            D.StaffId = S.StaffId;
            D.SPDate = S.SPDate;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;

            D.DiscountAmount = S.DiscountAmount;
            D.Extras = S.Extras;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.TotalAmount = S.TotalAmount;

            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;

            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;
            return D;

        }
        public static StockInProcessDetail ToMap(this StockInProcessDetail S, StockInProcessDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;

            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName;
            return D;
        }
        #endregion

        #region StockSeperated
        public static StockSeperated ToMap(this StockSeperated S, StockSeperated D)
        {
            D.Id = S.Id;
            D.StaffId = S.StaffId;
            D.Date = S.Date;
            D.RefCode = S.RefCode;
            D.RefNo = S.RefNo;

            D.DiscountAmount = S.DiscountAmount;
            D.ExtraAmount = S.ExtraAmount;
            D.GSTAmount = S.GSTAmount;
            D.ItemAmount = S.ItemAmount;
            D.TotalAmount = S.TotalAmount;

            D.Narration = S.Narration;
            D.AmountInwords = S.AmountInwords;

            D.lblDiscount = S.lblDiscount;
            D.lblExtra = S.lblExtra;

            return D;
        }
        public static StockSeperatedDetail ToMap(this StockSeperatedDetail S, StockSeperatedDetail D)
        {
            D.Product = S.Product;
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.DiscountAmount = S.DiscountAmount;
            D.GSTAmount = S.GSTAmount;
            D.ProductId = S.ProductId;
            D.Quantity = S.Quantity;
            D.UnitPrice = S.UnitPrice;
            D.UOMId = S.UOMId;

            D.ProductName = S.ProductName;
            D.SNo = S.SNo;
            D.UOMName = S.UOMName;

            return D;
        }
        #endregion


        #region Payment
        public static Payment ToMap(this Payment S, Payment D)
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
            D.AmountInwords = S.AmountInwords;

            D.PDetail = S.PDetail;
            D.PDetails = S.PDetails;
            D.PLedger = S.PLedger;


            D.IsEnabled = S.IsEnabled;
            D.IsLedgerEditable = S.IsLedgerEditable;
            D.IsReadOnly = S.IsReadOnly;
            D.IsShowComplete = S.IsShowComplete;
            D.IsShowOnlineDetail = S.IsShowOnlineDetail;
            D.IsShowChequeDetail = S.IsShowChequeDetail;
            D.IsShowTTDetail = S.IsShowTTDetail;
            D.IsShowReturn = S.IsShowReturn;

            return D;
        }
        public static PaymentDetail ToMap(this PaymentDetail S, PaymentDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
           D.LedgerId = S.LedgerId;
            D.Particular = S.Particular;
            D.PaymentId = S.PaymentId;
            D.LedgerName = S.LedgerName;
            D.SNo = S.SNo;
            D.TaxDetails = S.TaxDetails;
            D.GSTStatusId = S.GSTStatusId;
            D.PaymentTaxDetails = S.PaymentTaxDetails;
            D.GSTDRefNo = S.GSTDRefNo;
            D.RefLedgerId = S.RefLedgerId;
            D.IncludingGST = S.IncludingGST;
            D.AllowEdit = S.AllowEdit;
           
            return D;
        }
        public static Payment_Tax_Detail ToMap(this Payment_Tax_Detail S, Payment_Tax_Detail D)
        {
            D.Id = S.Id;
            S.PD_ID = S.PD_ID;
            S.TaxId = S.TaxId;
            S.TaxName = S.TaxName;
            S.TaxPercentage = S.TaxPercentage;
            S.TaxAmount = S.TaxAmount;

            return D;
        }
        #endregion

        #region Receipt
        public static Receipt ToMap(this Receipt S, Receipt D)
        {
            D.Id = S.Id;
            D.EntryNo = S.EntryNo;
            D.Amount = S.Amount;
            D.ChequeDate = S.ChequeDate;
            D.ChequeNo = S.ChequeNo;
            D.CleareDate = S.CleareDate;
            D.ExtraCharge = S.ExtraCharge;
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
            D.AmountInwords = S.AmountInwords;
            D.RDetail = S.RDetail;
            D.RDetails = S.RDetails;
            D.RLedger = S.RLedger;

            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;
            D.IsShowComplete = S.IsShowComplete;
            D.IsShowOnlineDetail = S.IsShowOnlineDetail;
            D.IsShowChequeDetail = S.IsShowChequeDetail;
            D.IsShowTTDetail = S.IsShowTTDetail;
            D.IsShowReturn = S.IsShowReturn;

            return D;
        }
        public static ReceiptDetail ToMap(this ReceiptDetail S, BLL.ReceiptDetail D)
        {
            D.Id = S.Id;
            D.Amount = S.Amount;
            D.LedgerId = S.LedgerId;
            D.Particulars = S.Particulars;
            D.ReceiptId = S.ReceiptId;
            D.LedgerName = S.LedgerName;
            D.SNo = S.SNo;
            D.RefLedgerId = S.RefLedgerId;
            D.TaxDetails = S.TaxDetails;
            D.AllowEdit = S.AllowEdit;
            D.IncludingGST = S.IncludingGST;
            return D;
        }
        #endregion

        #region Journal
        public static Journal ToMap(this Journal S, Journal D)
        {
            D.Id = S.Id;
            D.EntryNo = S.EntryNo;
            D.Amount = S.Amount;
            D.JournalDate = S.JournalDate;
            D.HQNo = S.HQNo;
            D.Particular = S.Particular;
            D.RefCode = S.RefCode;
            D.VoucherNo = S.VoucherNo;

            D.JDetail = S.JDetail;
            D.JDetails = S.JDetails;

            D.IsEnabled = S.IsEnabled;
            D.IsReadOnly = S.IsReadOnly;

            return D;
        }
        public static JournalDetail ToMap(this JournalDetail S, JournalDetail D)
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


            D.IsShowComplete = S.IsShowComplete;
            D.IsShowOnlineDetail = S.IsShowOnlineDetail;
            D.IsShowChequeDetail = S.IsShowChequeDetail;
            D.IsShowTTDetail = S.IsShowTTDetail;
            D.IsShowReturn = S.IsShowReturn;
            D.LedgerName = S.LedgerName;
            D.SNo = S.SNo;
            return D;
        }
        #endregion


        #endregion

    }
}
