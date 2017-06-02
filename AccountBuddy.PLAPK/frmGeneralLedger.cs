
using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace AccountBuddy.PLAPK
{
    [Activity(Label = "frmGeneralLedger", Theme = "@style/MyTheme")]
    public class frmGeneralLedger : AppCompatActivity
    {

        //protected ListView lsvGeneralLedger;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.frmGeneralLedger);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            
        }

        //protected override void OnResume()
        //{
        //    base.OnResume();
        //    lsvGeneralLedger.Adapter = new GeneralLedgerListAdapter(this, BLL.GeneralLedger.ToList(1, DateTime.Now, DateTime.Now));
        //}
    }
}