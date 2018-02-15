using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBuddy.BLL
{
	public class CompanyFile : INotifyPropertyChanged
	{

		#region Field
		
		private static ObservableCollection<CompanyFile> _toList;

		
		private int _id;
		private int? _CompanyId;
		private string _AttchmentCode;
		private byte[] _Image;

		#endregion

		#region Property
		public static ObservableCollection<CompanyFile> ToList
		{
			get
			{
				if (_toList == null)
				{
					try
					{
						var l1 = FMCGHubClient.HubCaller.Invoke<List<CompanyFile>>("CompanyFile_List").Result;
						_toList = new ObservableCollection<CompanyFile>(l1);
					}
					catch (Exception ex)
					{
						Common.AppLib.WriteLog(string.Format("CompanyFile ToList-{0}", ex.Message));
					}
				}

				return _toList;
			}
		}

	
		public int Id
		{
			get
			{
				return _id;
			}

			set
			{
				if (_id != value)
				{
					_id = value;
					NotifyPropertyChanged(nameof(Id));
				}
			}
		}

		public int? CompanyId
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

	
		public byte[] Image
		{
			get
			{
				return _Image;
			}

			set
			{
				if (_Image != value)
				{
					_Image = value;
					NotifyPropertyChanged(nameof(Image));
				}
			}
		}


		public string AttchmentCode
		{
			get
			{
				return _AttchmentCode;
			}
			set
			{
				if (_AttchmentCode != value)
				{
					_AttchmentCode = value;
					NotifyPropertyChanged(nameof(AttchmentCode));
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

	
	}
}
