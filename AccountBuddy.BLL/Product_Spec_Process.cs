using AccountBuddy.Common;
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
        private Product _Product;
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
        public Product Product
        {
            get
            {
                return _Product;
            }
            set
            {
                if (_Product != value)
                {
                    _Product = value;
                    SetProduct();

                    NotifyPropertyChanged(nameof(Product));
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
                return FMCGHubClient.HubCaller.Invoke<bool>("Product_Spec_Process_Save", this).Result;
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
                Product_Spec_Process po = FMCGHubClient.HubCaller.Invoke<Product_Spec_Process>("Purchase_Find", Id).Result;
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
                PDetail.Qty = pod.Qty * Qty;
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

        private void SetProduct()
        {
            var p = Product ?? new Product();
            ProductId = p.Id;

            ProductName = p.ProductName;
            Qty = p.Id != 0 ? 1 : 0;
        }

        #endregion

    }
}
