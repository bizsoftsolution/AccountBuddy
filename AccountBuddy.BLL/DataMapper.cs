﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public static class DataMapper
    {
        public static CompanyDetail ToMap(this CompanyDetail S,CompanyDetail D)
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

            return D;   
        }
        public static UserType ToMap(this UserType S,UserType D)
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
    }
}
