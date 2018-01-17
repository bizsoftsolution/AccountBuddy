﻿using AccountBuddy.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
    public class Product_Spec_Process : INotifyPropertyChanged
    {

        #region Field

        private long _Id;
        private DateTime _Date;
        private int _ProductId;
        private int _Qty;
        private string _ProductName;



        private Product_Spec_Process_Detail _PDetail;
        private ObservableCollection<Product_Spec_Process_Detail> _PDetails;
        private static UserTypeDetail _UserPermission;
        private Product_Spec_master _Product_Spec_master;
        public static ObservableCollection<Product_Spec_Process> _toList;


        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmProductSpecificationProcess.ToString()).FirstOrDefault();
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

        public DateTime Date
        {
            get
            {
                return _Date;
            }
            set
            {
                if (_Date != value)
                {
                    _Date = value;
                    NotifyPropertyChanged(nameof(Date));
                }
            }
        }
        public int ProductId
        {
            get
            {
                return _ProductId;
            }
            set
            {
                if (_ProductId != value)
                {
                    _ProductId = value;
                    NotifyPropertyChanged(nameof(ProductId));
                }
            }
        }
        public string ProductName
        {
            get
            {
                return _ProductName;
            }
            set
            {
                if (_ProductName != value)
                {
                    _ProductName = value;
                    NotifyPropertyChanged(nameof(ProductName));
                }
            }
        }
        public int Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                if (_Qty != value)
                {
                    _Qty = value;
                    NotifyPropertyChanged(nameof(Qty));
                }
            }
        }
        public Product_Spec_master Product_Spec_master
        {
            get
            {
                return _Product_Spec_master;
            }
            set
            {
                if (_Product_Spec_master != value)
                {
                    _Product_Spec_master = value;
                    SetProduct();

                    NotifyPropertyChanged(nameof(Product_Spec_master));
                }
            }
        }

        public Product_Spec_Process_Detail PDetail
        {
            get
            {
                if (_PDetail == null) _PDetail = new Product_Spec_Process_Detail();
                return _PDetail;
            }
            set
            {
                if (_PDetail != value)
                {
                    _PDetail = value;
                    NotifyPropertyChanged(nameof(PDetail));
                }
            }
        }
        public long PaymentLedgerId { get; set; }
        public ObservableCollection<Product_Spec_Process_Detail> PDetails
        {
            get
            {
                if (_PDetails == null) _PDetails = new ObservableCollection<Product_Spec_Process_Detail>();
                return _PDetails;
            }
            set
            {
                if (_PDetails != value)
                {
                    _PDetails = value;
                    NotifyPropertyChanged(nameof(PDetails));
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
                BLL.Product_Spec_Process s = new Product_Spec_Process();
                this.ToMap(s);
                return FMCGHubClient.HubCaller.Invoke<bool>("Product_Spec_Process_Save", s).Result;
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
                new Product_Spec_Process().ToMap(this);
                this.PDetail = new Product_Spec_Process_Detail();
                this.PDetails = new ObservableCollection<Product_Spec_Process_Detail>();

                Date = DateTime.Now;
                NotifyAllPropertyChanged();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }

        public bool Find()
        {
            try
            {
                Product_Spec_Process po = FMCGHubClient.HubCaller.Invoke<Product_Spec_Process>("Product_Spec_Process_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.PDetails = po.PDetails;
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
                return false;
            }
        }
        public bool FindById(int Id)
        {
            try
            {
                Product_Spec_Process po = FMCGHubClient.HubCaller.Invoke<Product_Spec_Process>("Product_Spec_Process_FindById", Id).Result;
                if (po.Id == 0) return false;
                po.ToMap(this);
                this.PDetails = po.PDetails;
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
                return FMCGHubClient.HubCaller.Invoke<bool>("Product_Spec_Process_Delete", this.Id).Result;
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
            if (PDetail.ProductId != 0)
            {
                Product_Spec_Process_Detail pod = PDetails.Where(x => x.SNo == PDetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new Product_Spec_Process_Detail();
                    PDetails.Add(pod);
                }
                PDetail.Qty = PDetail.Qty * Qty;
                PDetail.ToMap(pod);
                ClearDetail();

            }

        }

        public void ClearDetail()
        {
            Product_Spec_Process_Detail pod = new Product_Spec_Process_Detail();
            pod.SNo = PDetails.Count == 0 ? 1 : PDetails.Max(x => x.SNo) + 1;
            pod.ToMap(PDetail);
        }

        public void DeleteDetail(int SNo)
        {
            Product_Spec_Process_Detail pod = PDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                PDetails.Remove(pod);
                ClearDetail();

            }
        }

        public static void Init()
        {
            _toList = null;
        }

        #endregion
        public static ObservableCollection<Product_Spec_Process> toList
        {
            get
            {
                if (_toList == null)
                {
                    try
                    {
                        _toList = new ObservableCollection<Product_Spec_Process>(FMCGHubClient.HubCaller.Invoke<List<Product_Spec_Process>>("PSList").Result);

                    }
                    catch (Exception ex)
                    {
                        Common.AppLib.WriteLog(string.Format("Product_Spec_Process List={0}", ex.Message));
                    }
                }
                return _toList;
            }
            set
            {
                _toList = value;
            }
        }

        public static List<Product_Spec_Process> ToList(int? ProductId,DateTime dtFrom,DateTime dtTo)
        {
            List<Product_Spec_Process> rv = new List<Product_Spec_Process>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<Product_Spec_Process>>("Product_Spec_Process_List", ProductId,dtFrom,dtTo).Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Product_Spec_Process List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }

        public void AddDetail(long Id)
        {
            var s = Product_Spec_master.PSD_List.Where(x => x.Product_Spec_Id== Id).ToList();
            Product_Spec_Process_Detail psd = new Product_Spec_Process_Detail();
            PDetails.Clear();
            foreach(var pd in s)
            {
                psd = new Product_Spec_Process_Detail();
                //psd.Product = Product.toList.Where(x=>x.Id==pd.ProductId).FirstOrDefault();
                psd.ProductId = pd.ProductId;
                psd.ProductName = pd.ProductName;
                psd.Qty = pd.Qty * Qty;
                PDetails.Add(psd);
                psd.ToMap(PDetail);
            }
           
            //ClearDetail();
        }

        private void SetProduct()
        {
            var p = Product_Spec_master ?? new Product_Spec_master();
            ProductId = p.ProductId;

            ProductName = p.ProductName;
            Qty = p.ProductId != 0 ? 1 : 0;
            
        }

        #endregion

    }
}
