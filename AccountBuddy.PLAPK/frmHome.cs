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


using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

[assembly:Application(Theme = "@style/MyTheme")]
namespace AccountBuddy.PLAPK
{
    [Activity(Label = "Home")]
    public class frmHome : AppCompatActivity
    {
        DrawerLayout drawerLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.frmHome);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            var drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.open_drawer, Resource.String.close_drawer);
            drawerLayout.SetDrawerListener(drawerToggle);
            drawerToggle.SyncState();
        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_generalLedger):
                    frmGeneralLedger gl = new frmGeneralLedger();
                   StartActivity(new Intent(this,gl.Class));

                    break;
                case (Resource.Id.nav_balanceSheet):
                    frmBalanceSheet bs = new frmBalanceSheet();
                    StartActivity(new Intent(this, bs.Class));
                    break;
                case (Resource.Id.nav_trialBalance):
                    frmTrailBalance tb = new frmTrailBalance();
                    StartActivity(new Intent(this, tb.Class));
                    break;
                case (Resource.Id.nav_incomeExpense):
                    frmIncomeExpence ie = new frmIncomeExpence();
                    StartActivity(new Intent(this, ie.Class));
                    break;
                case (Resource.Id.nav_paymentRecipt):
                    frmReciptPayment rp = new frmReciptPayment();
                    StartActivity(new Intent(this, rp.Class));
                    break;
            }

            // Close drawer
            drawerLayout.CloseDrawers();
        }
    }
}