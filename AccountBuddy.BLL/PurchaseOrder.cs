﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBuddy.Common;

namespace AccountBuddy.BLL
{
    public class PurchaseOrder : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<PurchaseOrder> _POPendingList;

        private long _Id;
        private DateTime? _PODate;
        private string _RefNo;
        private int _LedgerId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _Extras;
        private decimal? _TotalAmount;
        private string _Narration;
        private string _Status;

        private string _LedgerName;
        private string _AmountInwords;

        private string _SearchText;

        private PurchaseOrderDetail _PODetail;
        private ObservableCollection<PurchaseOrderDetail> _PODetails;
        private string _RefCode;
        private static UserTypeDetail _UserPermission;
        private string _lblDiscount;
        private string _lblExtra;

        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmPurchaseOrder.ToString()).FirstOrDefault();
                }
                return _UserPermission;
            }

            set
            {
                if (_UserPermission != value)
                {
                    _UserPermission = value;
                }
            }
        }

        public static ObservableCollection<PurchaseOrder> POPendingList
        {
            get
            {
                if (_POPendingList == null)
                {
                    try
                    {
                        _POPendingList = new ObservableCollection<PurchaseOrder>();
                        var l1 = FMCGHubClient.HubCaller.Invoke<List<PurchaseOrder>>("PurchaseOrder_POPendingList").Result;
                        _POPendingList = new ObservableCollection<PurchaseOrder>(l1);
                    }
                    catch (Exception ex)
                    {
                        Common.AppLib.WriteLog(string.Format("POPending List_{0}-{1}", ex.Message, ex.InnerException));
                    }
                }
                return _POPendingList;
            }
            set
            {
                _POPendingList = value;
            }
        }

        public long Id
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

        public DateTime? PODate
        {
            get
            {
                return _PODate;
            }
            set
            {
                if (_PODate != value)
                {
                    _PODate = value;
                    NotifyPropertyChanged(nameof(PODate));
                }
            }
        }
        public string RefNo
        {
            get
            {
                return _RefNo;
            }
            set
            {
                if (_RefNo != value)
                {
                    _RefNo = value;
                    NotifyPropertyChanged(nameof(RefNo));
                }
            }
        }
        public string RefCode
        {
            get
            {
                return _RefCode;
            }
            set
            {
                if (_RefCode != value)
                {
                    _RefCode = value;
                    NotifyPropertyChanged(nameof(RefCode));
                }
            }
        }

        public int LedgerId
        {
            get
            {
                return _LedgerId;
            }
            set
            {
                if (_LedgerId != value)
                {
                    _LedgerId = value;
                    NotifyPropertyChanged(nameof(LedgerId));
                }
            }
        }

        public decimal? ItemAmount
        {
            get
            {
                if (_ItemAmount == null) _ItemAmount = 0;
                return _ItemAmount;
            }
            set
            {
                if (_ItemAmount != value)
                {
                    _ItemAmount = value;
                    NotifyPropertyChanged(nameof(ItemAmount));
                    SetAmount();
                }
            }
        }
        public decimal? DiscountAmount
        {
            get
            {
                if (_DiscountAmount == null) _DiscountAmount = 0;
                return _DiscountAmount;
            }
            set
            {
                if (_DiscountAmount != value)
                {
                    _DiscountAmount = value;
                    NotifyPropertyChanged(nameof(DiscountAmount));
                    SetAmount();
                }
            }
        }
        public decimal? GSTAmount
        {
            get
            {
                if (_GSTAmount == null) _GSTAmount = 0;
                return _GSTAmount;
            }
            set
            {
                if (_GSTAmount != value)
                {
                    _GSTAmount = value;
                    NotifyPropertyChanged(nameof(GSTAmount));
                }
            }
        }
        public decimal? Extras
        {
            get
            {
                if (_Extras == null) _Extras = 0;
                return _Extras;
            }
            set
            {
                if (_Extras != value)
                {
                    _Extras = value;
                    NotifyPropertyChanged(nameof(Extras));
                    SetAmount();
                }
            }
        }
        public decimal? TotalAmount
        {
            get
            {
                if (_TotalAmount == null) _TotalAmount = 0;
                return _TotalAmount;
            }
            set
            {
                if (_TotalAmount != value)
                {
                    _TotalAmount = value;
                    NotifyPropertyChanged(nameof(TotalAmount));
                    AmountInwords = value.ToCurrencyInWords();
                }
            }
        }
        public string Narration
        {
            get
            {
                return _Narration;
            }
            set
            {
                if (_Narration != value)
                {
                    _Narration = value;
                    NotifyPropertyChanged(nameof(Narration));
                }
            }
        }

        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    NotifyPropertyChanged(nameof(Status));
                }
            }
        }
        public string LedgerName
        {
            get
            {
                return _LedgerName;
            }
            set
            {
                if (_LedgerName != value)
                {
                    _LedgerName = value;
                    NotifyPropertyChanged(nameof(LedgerName));
                }
            }
        }


        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                if (_SearchText != value)
                {
                    _SearchText = value;
                    NotifyPropertyChanged(nameof(SearchText));
                }
            }
        }

        public string AmountInwords
        {
            get
            {
                if (_AmountInwords == null) _AmountInwords = "";
                return _AmountInwords;
            }
            set
            {
                if (_AmountInwords != value)
                {
                    _AmountInwords = value;
                    NotifyPropertyChanged(nameof(AmountInwords));
                }
            }
        }

        public PurchaseOrderDetail PODetail
        {
            get
            {
                if (_PODetail == null) _PODetail = new PurchaseOrderDetail();
                return _PODetail;
            }
            set
            {
                if (_PODetail != value)
                {
                    _PODetail = value;
                    NotifyPropertyChanged(nameof(PODetail));
                }
            }
        }

        public ObservableCollection<PurchaseOrderDetail> PODetails
        {
            get
            {
                if (_PODetails == null) _PODetails = new ObservableCollection<PurchaseOrderDetail>();
                return _PODetails;
            }
            set
            {
                if (_PODetails != value)
                {
                    _PODetails = value;
                    NotifyPropertyChanged(nameof(PODetails));
                }
            }
        }
        public string lblDiscount
        {
            get
            {
                return _lblDiscount;
            }
            set
            {
                if (_lblDiscount != value)
                {
                    _lblDiscount = value;
                    NotifyPropertyChanged(nameof(lblDiscount));

                }
            }
        }

        public string lblExtra
        {
            get
            {
                return _lblExtra;
            }
            set
            {
                if (_lblExtra != value)
                {
                    _lblExtra = value;
                    NotifyPropertyChanged(nameof(lblExtra));

                }
            }
        }
        #endregion

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String ProperName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ProperName));
        }
        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion

        #region Methods

        #region Master
        public bool Save()
        {
            try
            {
                return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseOrder_Save", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }

        public bool MakePurchase()
        {
            try
            {
                return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseOrder_MakePurchase", this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }

        public void Clear()
        {
            try
            {
                new PurchaseOrder().toCopy<PurchaseOrder>(this);
                _PODetail = new PurchaseOrderDetail();
                _PODetails = new ObservableCollection<PurchaseOrderDetail>();
                PODate = DateTime.Now;
                RefNo = FMCGHubClient.HubCaller.Invoke<string>("PurchaseOrder_NewRefNo").Result;
                NotifyAllPropertyChanged();
            }
            catch(Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        public void setLabel()
        {
            lblDiscount = string.Format("{0}({1})", "Discount Amount", AppLib.CurrencyPositiveSymbolPrefix);
            lblExtra = string.Format("{0}({1})", "Extra Amount", AppLib.CurrencyPositiveSymbolPrefix);

        }
        private void MaxRef()
        {
            RefNo = FMCGHubClient.HubCaller.Invoke<string>("PurchaseOrder_MaxRef").Result;
        }

        public bool Find()
        {
            try
            {
                PurchaseOrder po = FMCGHubClient.HubCaller.Invoke<PurchaseOrder>("PurchaseOrder_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<PurchaseOrder>(this);
                this.PODetails = po.PODetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }

        public bool Delete()
        {
            try
            {
                return FMCGHubClient.HubCaller.Invoke<bool>("PurchaseOrder_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); return false;
            }
        }
        #endregion

        #region Detail

        public void SaveDetail()
        {
            try
            {
                PurchaseOrderDetail pod = PODetails.Where(x => x.SNo == PODetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new PurchaseOrderDetail();
                    PODetails.Add(pod);
                }
                else
                {
                    PODetail.Quantity += pod.Quantity;

                }

                PODetail.toCopy<PurchaseOrderDetail>(pod);
                ItemAmount = PODetails.Sum(x => x.Amount);
                SetAmount();
                ClearDetail();

            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Purchase Order save Detail_{0}", ex.Message));
            }
        }

        public void ClearDetail()
        {
            PurchaseOrderDetail pod = new PurchaseOrderDetail();
            pod.SNo = PODetails.Count == 0 ? 1 : PODetails.Max(x => x.SNo) + 1;
            pod.toCopy<PurchaseOrderDetail>(PODetail);

        }

        public void DeleteDetail(int SNo)
        {
            PurchaseOrderDetail pod = PODetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                PODetails.Remove(pod);
                ItemAmount = PODetails.Sum(x => x.Amount);
                SetAmount();
                ClearDetail();
            }
        }
        #endregion


        private void SetAmount()
        {

            GSTAmount = ((ItemAmount ?? 0) - (DiscountAmount ?? 0)) * Common.AppLib.GSTPer;
            TotalAmount = (ItemAmount ?? 0) - (DiscountAmount ?? 0) + GSTAmount + (Extras ?? 0);
            setLabel();
        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<bool>("Find_PORef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex); rv = true;
            }
            return rv;
        }

        public static List<PurchaseOrder> PO_List(int? LedgerId, DateTime dtFrom, DateTime dtTo, string BillNo, decimal amtFrom, decimal amtTo)
        {
            List<PurchaseOrder> rv = new List<PurchaseOrder>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<PurchaseOrder>>("PurchaseOrder_List", LedgerId, dtFrom, dtTo, BillNo, amtFrom, amtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Purchase Order List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }
        #endregion
    }
}
