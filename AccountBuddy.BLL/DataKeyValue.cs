using System;
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


        #region Sales A/C Ledger
        public static string SalesAccount_Ledger_Key
        {
            get
            {
                return "Sales Account";
            }

        }
        public static int SalesAccount_Ledger_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SalesAccount_Ledger_Key).FirstOrDefault().DataValue;
            }
        }

        #endregion

        public static string SundryDebtors_Key
        {
            get
            {
                return "Sundry Debtors";
            }

        }
        public static int SundryDebtors_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SundryDebtors_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SundryCreditors_Key
        {
            get
            {
                return "Sundry Creditors";
            }

        }
        public static int SundryCreditors_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SundryCreditors_Key).FirstOrDefault().DataValue;
            }
        }
        #region Input_Tax Ledger
        public static string Input_Tax_Ledger_Key
        {
            get
            {
                return "Input_Tax";
            }

        }
        public static int Input_Tax_Ledger_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Input_Tax_Ledger_Key).FirstOrDefault().DataValue;
            }
        }
        #endregion

        #region Output_Tax Ledger
        public static string Output_Tax_Ledger_Key
        {
            get
            {
                return "Output_Tax";
            }

        }
        public static int Output_Tax_Ledger_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Output_Tax_Ledger_Key).FirstOrDefault().DataValue;
            }
        }
        #endregion

        #region Purchase_Ac_Ledger
        public static string PurchaseAccount_Ledger_Key
        {
            get
            {
                return "Purchase A/C";
            }

        }
        public static int PurchaseAccount_Ledger_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == PurchaseAccount_Ledger_Key).FirstOrDefault().DataValue;
            }
        }
        #endregion

        #region Stock Group
        public static string StockGroup_Primary_Key
        {
            get
            {
                return "StockGroup_Primary";
            }
        }
        public static int StockGroup_Primary_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == StockGroup_Primary_Key).FirstOrDefault().DataValue;
            }
        }
        #endregion

        #region UOM
        public static string UOM_Key
        {
            get
            {
                return "UOM";
            }
        }
        public static int UOM_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == UOM_Key).FirstOrDefault().DataValue;
            }
        }

        #endregion
        public static string Cash_in_Hand_Key
        {
            get
            {
                return "Cash-in-Hand";
            }
        }
        public static int Cash_in_Hand_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Cash_in_Hand_Key).FirstOrDefault().DataValue;
            }
        }
        public static string Bank_Accounts_Key
        {
            get
            {
                return "Bank Accounts";
            }
        }
        public static int Bank_Accounts_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Bank_Accounts_Key).FirstOrDefault().DataValue;
            }
        }
        public static string Salary_Account_Key
        {
            get
            {
                return "Salary Account";
            }
        }
        public static int Salary_Account_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Salary_Account_Key).FirstOrDefault().DataValue;
            }
        }

        #region Tax
        #region Account Group
        public static string Tax_And_Duties_Key
        {
            get
            {
                return "Tax And Duties";
            }

        }
        public static int Tax_And_Duties_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Tax_And_Duties_Key).FirstOrDefault().DataValue;
            }
        }
        public static string IGST_Key
        {
            get
            {
                return "IGST";
            }

        }
        public static int IGST_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == IGST_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SGST_Key
        {
            get
            {
                return "SGST";
            }

        }
        public static int SGST_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SGST_Key).FirstOrDefault().DataValue;
            }
        }
        public static string CGST_Key
        {
            get
            {
                return "CGST";
            }

        }
        public static int CGST_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == CGST_Key).FirstOrDefault().DataValue;
            }
        }
        #endregion
        #region Ledger
        #region Input
        public static string IGST_In_Key
        {
            get
            {
                return "IGST_In";
            }

        }
        public static int IGST_In_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == IGST_In_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SGST_In_Key
        {
            get
            {
                return "SGST_In";
            }

        }
        public static int SGST_In_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SGST_In_Key).FirstOrDefault().DataValue;
            }
        }
        public static string CGST_In_Key
        {
            get
            {
                return "CGST_In";
            }

        }
        public static int CGST_In_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == CGST_In_Key).FirstOrDefault().DataValue;
            }
        }
        #endregion

        #region Output
        public static string IGST_Out_Key
        {
            get
            {
                return "IGST_Out";
            }

        }
        public static int IGST_Out_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == IGST_Out_Key).FirstOrDefault().DataValue;
            }
        }
        public static string SGST_Out_Key
        {
            get
            {
                return "SGST_Out";
            }

        }
        public static int SGST_Out_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == SGST_Out_Key).FirstOrDefault().DataValue;
            }
        }
        public static string CGST_Out_Key
        {
            get
            {
                return "CGST_Out";
            }

        }
        public static int CGST_Out_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == CGST_Out_Key).FirstOrDefault().DataValue;
            }
        }

        #endregion
        #endregion

        #endregion

        #region Sales Return
        public static string Sales_Return_AC_Key
        {
            get
            {
                return "Sales Return A/c";
            }
        }
        public static int Sales_Return_AC_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Sales_Return_AC_Key).FirstOrDefault().DataValue;
            }
        }

        #endregion

        #region Purchase Return 
        public static string Purchase_Return_AC_Key
        {
            get
            {
                return "Purchase Return A/c";
            }
        }
        public static int Purchase_Return_AC_Value
        {
            get
            {
                return toList.Where(x => x.DataKey == Purchase_Return_AC_Key).FirstOrDefault().DataValue;
            }
        }

        #endregion

        #endregion

        public static List<DataKeyValue> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = FMCGHubClient.FMCGHub.Invoke<List<DataKeyValue>>("DataKeyValue_List").Result;
                }

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
