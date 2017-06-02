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
using Android.Support.V7.App;

namespace AccountBuddy.PLAPK
{
    [Activity(Label = "frmTrailBalance")]
    public class frmTrailBalance : AppCompatActivity
    {
        ListView lstTrialLedger;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.frmTrailBalance);

            lstTrialLedger = FindViewById<ListView>(Resource.Id.lstTrialLedger);

            lstTrialLedger.Adapter = new Adapters.TrialBalanceListAdapter(this,BLL.TrialBalance.ToList( DateTime.Now,DateTime.Now) );


        }
    }
}