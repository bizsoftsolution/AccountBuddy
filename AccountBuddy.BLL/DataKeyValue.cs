﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class DataKeyValue : INotifyPropertyChanged
    {


        #region fields
        private static List<DataKeyValue> _toList;
        private int _Id;
        private string _DataKey;
        private int _DataValue;
        private int _CompanyId;
        #endregion

        #region Property

        #region Static Property
        #region AccountsGroup

        public static string Administrator_Key
        {
            get
            {
                return "Administrator";
            }
        }
        public static int Administrator_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Administrator_Key).FirstOrDefault().DataValue;
            }
        }

        public static string Primary_Key
        {
            get
            {
                return "Primary";
            }
        }
        public static int Primary_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Primary_Key).FirstOrDefault().DataValue;
            }
        }

        public static string Assets_Key
        {
            get
            {
                return "Assets";
            }
        }
        public static int Assets_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Assets_Key).FirstOrDefault().DataValue;
            }
        }
        public static string Liabilities_Key
        {
            get
            {
                return "Liabilities";
            }
            
        }

        public static int Liabilities_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Liabilities_Key).FirstOrDefault().DataValue;
            }
        }
        public static string Income_Key
        {
            get
            {
                return "Income";
            }

        }
        public static int Income_Value
        {
            get
            {
                return toList.Where(x => x.DataKey ==Income_Key).FirstOrDefault().DataValue;
            }
        }

        public static string Expenses_Key
        {
            get
            {
                return "Expenses";
            }

        }
        public static int Expenses_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Expenses_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SundryCreditors_Key
        {
            get
            {
                return "Sundry Creditors";
            }

        }
        public static int SundryCreditors
        {
            get
            {
                return toList.Where(x => x.DataKey == SundryCreditors_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SundryDebtors_Key
        {
            get
            {
                return "Sundry Debtors";
            }

        }
        public static int SundryDebtors
        {
            get
            {
                return toList.Where(x => x.DataKey == SundryDebtors_Key).FirstOrDefault().DataValue;
            }
        }

        public static string CurrentAssets_Key
        {
            get
            {
                return "Current Assets";
            }

        }
        public static int CurrentAssets_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == CurrentAssets_Key).FirstOrDefault().DataValue;
            }
        }
        public static string CashInHand_Key
        {
            get
            {
                return "Cash-in-Hand";
            }

        }
        public static int CashInHand_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == CashInHand_Key).FirstOrDefault().DataValue;
            }
        }
        public static string CashLedger_Key
        {
            get
            {
                return "Cash Ledger";
            }

        }
        public static int CashLedger_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == CashLedger_Key).FirstOrDefault().DataValue;
            }
        }

        public static string Deposits_Key
        {
            get
            {
                return "Deposits";
            }

        }
        public static int Deposits_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Deposits_Key).FirstOrDefault().DataValue;
            }
        }
        public static string LoansandAdvances_Key
        {
            get
            {
                return "Loans and Advances";
            }

        }
        public static int LoansandAdvances_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == LoansandAdvances_Key).FirstOrDefault().DataValue;
            }
        }
        public static string BankAccounts_Key
        {
            get
            {
                return "Bank Accounts";
            }

        }
        public static int BankAccounts_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == BankAccounts_Key).FirstOrDefault().DataValue;
            }
        }
        public static string StockInHand_Key
        {
            get
            {
                return "Stock-In-Hand";
            }

        }
        public static int StockInHand_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == StockInHand_Key).FirstOrDefault().DataValue;
            }
        }

        public static string FixedAssets_Key
        {
            get
            {
                return "Fixed Assets";
            }

        }
        public static int FixedAssets_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == FixedAssets_Key).FirstOrDefault().DataValue;
            }
        }
        public static string MiscExpenses_Key
        {
            get
            {
                return "Misc. Expenses";
            }

        }
        public static int MiscExpenses_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == MiscExpenses_Key).FirstOrDefault().DataValue;
            }
        }
        public static string Investments_Key
        {
            get
            {
                return "Investments";
            }

        }
        public static int Investments_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Investments_Key).FirstOrDefault().DataValue;
            }
        }

        public static string CurrentLiabilities_Key
        {
            get
            {
                return "Current Liabilities";
            }

        }
        public static int CurrentLiabilities_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == CurrentLiabilities_Key).FirstOrDefault().DataValue;
            }
        }
        public static string DutiesTaxes_Key
        {
            get
            {
                return "Duties & Taxes";
            }

        }
        public static int DutiesTaxes_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == DutiesTaxes_Key).FirstOrDefault().DataValue;
            }
        }
        public static string Provisions_Key
        {
            get
            {
                return "Provisions";
            }

        }
        public static int Provisions_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Provisions_Key).FirstOrDefault().DataValue;
            }
        }
        public static string Loans_Key
        {
            get
            {
                return "Loans";
            }

        }
        public static int Loans_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Loans_Key).FirstOrDefault().DataValue;
            }
        }
        public static string BankODAc_Key
        {
            get
            {
                return "Bank OD A/c";
            }

        }
        public static int BankODAc_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == BankODAc_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SecuredLoans_Key
        {
            get
            {
                return "Secured Loans";
            }

        }
        public static int SecuredLoans_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SecuredLoans_Key).FirstOrDefault().DataValue;
            }
        }
        public static string UnSecuredLoans_Key
        {
            get
            {
                return "UnSecured Loans";
            }

        }
        public static int UnSecuredLoans_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == UnSecuredLoans_Key).FirstOrDefault().DataValue;
            }
        }
        public static string BranchDivisions_Key
        {
            get
            {
                return "Branch /Divisions";
            }

        }
        public static int BranchDivisions_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == BranchDivisions_Key).FirstOrDefault().DataValue;
            }
        }
        public static string CapitalAccount_Key
        {
            get
            {
                return "Capital Account";
            }

        }
        public static int CapitalAccount_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == CapitalAccount_Key).FirstOrDefault().DataValue;
            }
        }
        public static string ReservesSurplus_Key
        {
            get
            {
                return "Reserves & Surplus";
            }

        }
        public static int ReservesSurplus_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == ReservesSurplus_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SuspenseAc_Key
        {
            get
            {
                return "Suspense A/c";
            }

        }
        public static int SuspenseAc_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SuspenseAc_Key).FirstOrDefault().DataValue;
            }
        }
        public static string DirectIncome_Key
        {
            get
            {
                return "Direct Income";
            }

        }
        public static int DirectIncome_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == DirectIncome_Key).FirstOrDefault().DataValue;
            }
        }
        public static string IndirectIncome_Key
        {
            get
            {
                return "Indirect Income";
            }

        }
        public static int IndirectIncome_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == IndirectIncome_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SalesAccount_Key
        {
            get
            {
                return "Sales Account";
            }

        }
        public static int SalesAccount_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SalesAccount_Key).FirstOrDefault().DataValue;
            }
        }
        public static string DirectExpenses_Key
        {
            get
            {
                return "Direct Expenses";
            }

        }
        public static int DirectExpenses_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == DirectExpenses_Key).FirstOrDefault().DataValue;
            }
        }
        public static string IndirectExpense_Key
        {
            get
            {
                return "Indirect Expense";
            }

        }
        public static int IndirectExpense_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == IndirectExpense_Key).FirstOrDefault().DataValue;
            }
        }
        public static string PurchaseAccount_Key
        {
            get
            {
                return "Purchase Account";
            }

        }
        public static int PurchaseAccount_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == PurchaseAccount_Key).FirstOrDefault().DataValue;
            }
        }
        #endregion

        public static List<DataKeyValue> toList
        {
            get
            {
                if (_toList == null) _toList = FMCGHubClient.FMCGHub.Invoke<List<DataKeyValue>>("DataKeyValue_List").Result;
                return _toList;
            }
            set
            {
                _toList = value;
            }
        }
        #endregion

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }

        public string DataKey
        {
            get
            {
                return _DataKey;
            }
            set
            {
                if (_DataKey != value)
                {
                    _DataKey = value;
                    NotifyPropertyChanged(nameof(DataKey));
                }
            }
        }

        public int DataValue
        {
            get
            {
                return _DataValue;
            }
            set
            {
                if (_DataValue != value)
                {
                    _DataValue = value;
                    NotifyPropertyChanged(nameof(DataValue));
                }

            }
        }

        public int CompanyId
        {
            get
            {
                return _CompanyId;
            }
            set
            {
                if (_CompanyId != value)
                {
                    _CompanyId = value;
                    NotifyPropertyChanged(nameof(CompanyId));
                }
            }
        }

        #endregion

        #region Property  Changed Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion
        public static void Init()
        {
            _toList = FMCGHubClient.FMCGHub.Invoke<List<DataKeyValue>>("DataKeyValue_List").Result;
        }
    }
}