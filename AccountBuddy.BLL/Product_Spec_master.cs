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
    public class Product_Spec_master : INotifyPropertyChanged
    {

        #region Field

        private long _Id;
        private int _ProductId;
        private Product_Spec_Detail _PDetail;
        private ObservableCollection<Product_Spec_Detail> _PDetails;
        private static UserTypeDetail _UserPermission;
        private Product _Product;
        private string _ProductName;
        private int _RNo;
        private static ObservableCollection<Product_Spec_master> _toList;


        #endregion

        #region Property
        public static UserTypeDetail UserPermission
        {
            get
            {
                if (_UserPermission == null)
                {
                    _UserPermission = UserAccount.User.UserType == null ? new UserTypeDetail() : UserAccount.User.UserType.UserTypeDetails.Where(x => x.UserTypeFormDetail.FormName == Forms.frmPurchase.ToString()).FirstOrDefault();
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
        public Product_Spec_Detail PDetail
        {
            get
            {
                if (_PDetail == null) _PDetail = new Product_Spec_Detail();
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
        public ObservableCollection<Product_Spec_Detail> PDetails
        {
            get
            {
                if (_PDetails == null) _PDetails = new ObservableCollection<Product_Spec_Detail>();
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
        public int RNo
        {
            get
            {
                return _RNo;
            }
            set
            {
                if (_RNo != value)
                {
                    _RNo = value;
                    NotifyPropertyChanged(nameof(RNo));
                }
            }
        }
        public static ObservableCollection<Product_Spec_master> toList
        {
            get
            {
                if (_toList == null)
                {
                    try
                    {
                        _toList = new ObservableCollection<Product_Spec_master>(FMCGHubClient.HubCaller.Invoke<List<Product_Spec_master>>("Product_Spec_master_List").Result);
                       
                    }
                    catch (Exception ex)
                    {
                        Common.AppLib.WriteLog(string.Format("Ledger List={0}", ex.Message));
                    }
                }
                return _toList;
            }
            set
            {
                _toList = value;
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
                var d = FMCGHubClient.HubCaller.Invoke<Product_Spec_master>("Product_Spec_master_Save", this).Result;
                if (d.Id != 0)
                {
                    if (Id == 0)
                    {
                        toList.Add(d);
                    }
                    else
                    {
                        var d1 = toList.Where(x => x.Id == d.Id).FirstOrDefault();
                        d.ToMap(d1);
                    }
                    return true;
                }
               
            }

            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
              
            }
            return false;
        }
        public void Clear()
        {
            try
            {
               new Product_Spec_master().ToMap(this);
                this.PDetail = new Product_Spec_Detail();
                this.PDetails = new ObservableCollection<Product_Spec_Detail>();

                    NotifyAllPropertyChanged();
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(ex);
            }
        }
        
        public bool Find(long id)
        {
            try
            {
                Product_Spec_master po = FMCGHubClient.HubCaller.Invoke<Product_Spec_master>("Find", id).Result;
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
        public bool Delete()
        {
            try
            {
                var d = toList.Where(x => x.Id == Id).FirstOrDefault();

                var rv = FMCGHubClient.HubCaller.Invoke<bool>("Product_Spec_master_Delete", this.Id).Result;
                if (rv == true)
                {
                    toList.Remove(d);
                }
                return rv;
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
                Product_Spec_Detail pod = PDetails.Where(x => x.SNo == PDetail.SNo).FirstOrDefault();

                if (pod == null)
                {
                    pod = new Product_Spec_Detail();
                    PDetails.Add(pod);
                }

                PDetail.ToMap(pod);
                ClearDetail();
               
            }

        }

        public void ClearDetail()
        {
            Product_Spec_Detail pod = new Product_Spec_Detail();
            pod.SNo = PDetails.Count == 0 ? 1 : PDetails.Max(x => x.SNo) + 1;
            pod.ToMap(PDetail);
        }

        public void DeleteDetail(int SNo)
        {
            Product_Spec_Detail pod = PDetails.Where(x => x.SNo == SNo).FirstOrDefault();

            if (pod != null)
            {
                PDetails.Remove(pod);
                ClearDetail();
             
            }
        }

        #endregion
        
        public static List<Product_Spec_master> ToList()
        {
            List<Product_Spec_master> rv = new List<Product_Spec_master>();
            try
            {
                rv = FMCGHubClient.HubCaller.Invoke<List<Product_Spec_master>>("Product_Spec_master_List").Result;
            }
            catch (Exception ex)
            {
                Common.AppLib.WriteLog(string.Format("Product_Spec_master_List List= {0}-{1}", ex.Message, ex.InnerException));
            }
            return rv;

        }

        private void SetProduct()
        {
            var p = Product ?? new Product();
            ProductId = p.Id;

            ProductName = p.ProductName;
            
            //  DiscountAmount = p.DiscountAmount;
        }

        #endregion

    }
}
