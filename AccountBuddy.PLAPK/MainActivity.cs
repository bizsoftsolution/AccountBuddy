using Android.App;
using Android.Widget;
using Android.OS;
using System.Linq;

namespace AccountBuddy.PLAPK
{
    [Activity(Label = "AccountBuddy.PLAPK", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Spinner year, company;
        EditText uname, pass;
        Button signin, clear;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            uname = (EditText)FindViewById(Resource.Id.txtUserName);
            pass = (EditText)FindViewById(Resource.Id.txtPassword);

            signin = (Button)FindViewById(Resource.Id.btnSignIn);
            clear = (Button)FindViewById(Resource.Id.btnClear);

            year = (Spinner)FindViewById(Resource.Id.cmbYear);
            company = (Spinner)FindViewById(Resource.Id.cmbCompany);

            signin.Click += Signin_Click;
            clear.Click += Clear_Click;

            try
            {

                var l1 = BLL.CompanyDetail.toList.Select(x => x.CompanyName).ToList();
                var l2 = BLL.CompanyDetail.AcYearList;


                ArrayAdapter<Android.Resource.String> adapter1 = new ArrayAdapter<Android.Resource.String>(this, Android.Resource.Layout.SimpleDropDownItem1Line);
                adapter1.AddAll(l1);

                company.Adapter = adapter1;

                ArrayAdapter<Android.Resource.String> adapter2 = new ArrayAdapter<Android.Resource.String>(this, Android.Resource.Layout.SimpleDropDownItem1Line);
                adapter2.AddAll(l2);

                year.Adapter = adapter2;


            }
            catch (Java.Lang.Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
            }

        }

        private void Clear_Click(object sender, System.EventArgs e)
        {
            uname.Text = "";
            pass.Text = "";
        }

        private void Signin_Click(object sender, System.EventArgs e)
        {
            string RValue = BLL.UserAccount.Login(year.SelectedItem.ToString(), company.SelectedItem.ToString(), uname.Text, pass.Text);
            if (RValue == "")
            {
                this.Finish();
                frmHome m = new frmHome();
                StartActivity(new Android.Content.Intent(this, m.Class));
            }
            else
            {
                msg(RValue);
            }
        }

        void msg(string m)
        {

            Toast.MakeText(this, m, ToastLength.Long).Show();

        }
    }
}

