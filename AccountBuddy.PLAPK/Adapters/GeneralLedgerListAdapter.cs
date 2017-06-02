using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AccountBuddy.BLL;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AccountBuddy.PLAPK.Adapters
{
    public class GeneralLedgerListAdapter : BaseAdapter<BLL.GeneralLedger>
    {
        Activity context = null;
        IList<BLL.GeneralLedger> lstGeneralLedger = new List<BLL.GeneralLedger>();

        public GeneralLedgerListAdapter(Activity context, IList<BLL.GeneralLedger> lstGeneralLedger) : base ()
		{
            this.context = context;
            this.lstGeneralLedger = lstGeneralLedger;
        }


        public override GeneralLedger this[int position] => lstGeneralLedger[position];

        public override int Count => lstGeneralLedger.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {            
            var item = lstGeneralLedger[position];
            var view = convertView ??context.LayoutInflater.Inflate(Resource.Layout.viewGeneralLedger,parent,false);

            var dt = view.FindViewById<TextView>(Resource.Id.txtUserName);
            var ac = view.FindViewById<TextView>(Resource.Id.txtUserName);


            dt.Text = item.EDate != null ? item.EDate.ToString() : "";

            return view;
        }
    }
}