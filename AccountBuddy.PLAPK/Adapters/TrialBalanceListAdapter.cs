using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AccountBuddy.PLAPK.Adapters
{
    public class TrialBalanceListAdapter : BaseAdapter<BLL.TrialBalance>
    {
        Activity context = null;
        IList<BLL.TrialBalance> lstTrialBalance = new List<BLL.TrialBalance>();

        public TrialBalanceListAdapter(Activity context, IList<BLL.TrialBalance> lstTrialBalance) : base ()
		{
            this.context = context;
            this.lstTrialBalance = lstTrialBalance;
        }


        public override BLL.TrialBalance this[int position] => lstTrialBalance[position];

        public override int Count => lstTrialBalance.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = lstTrialBalance[position];
            var view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.view_trial_balance, parent, false);

            var an = view.FindViewById<TextView>(Resource.Id.txtAccountName);
            var dr = view.FindViewById<TextView>(Resource.Id.txtDebitAmount);
            var cr = view.FindViewById<TextView>(Resource.Id.txtDebitAmount);

            an.Text = item.AccountName;
            dr.Text = item.DrAmt.ToString();
            cr.Text = item.CrAmt.ToString();

            return view;
        }
    }
}