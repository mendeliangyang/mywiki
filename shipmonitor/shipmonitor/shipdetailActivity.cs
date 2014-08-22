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

namespace shipmonitor
{
    [Activity(Label = "My Activity")]
    public class shipdetailActivity : Activity
    {
        View actionbarView;
        ImageButton btnback;
        ImageButton btn_levea;
        ImageButton btn_baseinfo;

        private string userid;
        private string articleID;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            actionbarView = common.setActionBar(this,Resource.Layout.detailstitlebar);

            SetContentView(Resource.Layout.shipdetail);
            userid = Intent.GetStringExtra("userID");
            articleID = Intent.GetStringExtra("articleID");


             btnback= actionbarView.FindViewById<ImageButton>(Resource.Id.detailstitlebar_btnback);
             btnback.Click += new EventHandler(btnback_Click);
             btn_levea = (ImageButton)this.FindViewById(Resource.Id.btn_shipdetail_leavemessage);
             btn_levea.Click += new EventHandler(btn_levea_Click);

            //手动绘制界面
            // FindViewById<ImageButton>(Resource.Id.btn_shipdetail_baseinfo).Click += delegate(object sender, EventArgs e) { startActivity(typeof(baseInfoActivity)); };
            //使用代码加载界面
             FindViewById<ImageButton>(Resource.Id.btn_shipdetail_baseinfo).Click += delegate(object sender, EventArgs e) { startActivity(typeof(owerinfoactivity), "baseinfo"); };
            //listview 页面加载
             //FindViewById<ImageButton>(Resource.Id.btn_shipdetail_port).Click += delegate(object sender, EventArgs e) { startActivity(typeof(docklistview)); };
            //custom 加载
             FindViewById<ImageButton>(Resource.Id.btn_shipdetail_port).Click += delegate(object sender, EventArgs e) { startActivity(typeof(owerinfoactivity), "dockport"); };


             FindViewById<ImageButton>(Resource.Id.btn_shipdetail_owner).Click += delegate(object sender, EventArgs e) { 
				startActivity(typeof(owerinfoactivity)); 
			};

			
			FindViewById<ImageButton>(Resource.Id.btn_shipdetail_service).Click += delegate(object sender, EventArgs e) { 
				startActivity(typeof(servingcostactivity)); 
			};



            FindViewById<ImageButton>(Resource.Id.btn_shipdetail_protservice).Click += delegate(object sender, EventArgs e)
            {
                startActivity(typeof(owerinfoactivity),"portsetvice");
            };

            FindViewById<ImageButton>(Resource.Id.btn_shipdetail_gas).Click += delegate(object sender, EventArgs e)
            {
                startActivity(typeof(owerinfoactivity), "gas");
            };

            FindViewById<ImageButton>(Resource.Id.btn_shipdetail_current).Click += delegate(object sender, EventArgs e)
            {
                startActivity(typeof(owerinfoactivity), "current");
            };

            FindViewById<ImageButton>(Resource.Id.btn_shipdetail_expense).Click += delegate(object sender, EventArgs e)
            {
                startActivity(typeof(owerinfoactivity), "expense");
            };

            FindViewById<ImageButton>(Resource.Id.btn_shipdetail_projectFollow).Click += delegate(object sender, EventArgs e)
            {
                //startActivity(typeof(owerinfoactivity), "projectFollow");

                Intent intent = new Intent(this, typeof(owerinfoactivity));
                intent.PutExtra("userID", userid);
                intent.PutExtra("articleID", articleID);

                intent.PutExtra("IsDiaoDu", Intent.GetStringExtra("IsDiaoDu"));
                intent.PutExtra("IsHaiWu", Intent.GetStringExtra("IsHaiWu"));
                intent.PutExtra("IsJiWu", Intent.GetStringExtra("IsJiWu"));
                intent.PutExtra("IsRenShi", Intent.GetStringExtra("IsRenShi"));
                intent.PutExtra("IsShangWu", Intent.GetStringExtra("IsShangWu"));

                intent.PutExtra("acvitityType", "projectFollow");

                StartActivity(intent);
            };


        }

        void btn_levea_Click(object sender, EventArgs e)
        {
            startActivity(typeof(leaveMessageActivity));
        }




        private void startActivity(Type teyp, string acvitityType="") 
        {
            Intent intent = new Intent(this, teyp);
            intent.PutExtra("userID", userid);
            intent.PutExtra("articleID", articleID);
            intent.PutExtra("acvitityType",acvitityType);
            StartActivity(intent);
        }



        #region event
        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnback_Click(object sender, EventArgs e)
        {
            Finish();
        }
        #endregion
        
    }
}