using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using System.Collections.Generic;

using System.Threading.Tasks;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Json;

namespace shipmonitor
{
    [Activity(Label = "shipmonitor", MainLauncher = true)]
    public class mainActivity : Activity
    {
        Button btnsignin;
        EditText txtUserName;
        EditText txtUserPass;
        CheckBox cbkeeppass;
        CheckBox cbkeepsign;
        Handler handler;
        string app_userID;
        string app_access;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //隐藏actionbar
            //ActionBar.SetDisplayHomeAsUpEnabled(false);
            //ActionBar.SetDisplayShowHomeEnabled(false);
            //ActionBar.SetDisplayShowCustomEnabled(false);
            ActionBar.Hide();

            SetContentView(Resource.Layout.Main);

            btnsignin = (Button)FindViewById(Resource.Id.main_btnsignin);
            txtUserName = (EditText)FindViewById(Resource.Id.main_txtusername);
            txtUserPass = (EditText)FindViewById(Resource.Id.main_txtpassword);
            cbkeeppass = (CheckBox)FindViewById(Resource.Id.main_cbkeeppass);
            cbkeepsign = (CheckBox)FindViewById(Resource.Id.main_cbkeepsign);

            btnsignin.Click += new EventHandler(btnsignin_Click);
            cbkeepsign.Click += new EventHandler(cbkeepsign_Click);

            handler = new Handler((Message message) => 
            {
                switch (message.What)
                {
                    case 1:
                        Startallship(message.Data);
                        btnsignin.Enabled = true;
                        break;
                    case 2:
                        Toast.MakeText(this, message.Data.GetString("errormessage"), ToastLength.Long).Show();
                        btnsignin.Enabled = true;
                        break;
                    default:
                        break;
                }
            });
        }

        


        #region event
        /// <summary>
        /// 保持登陆，
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbkeepsign_Click(object sender, EventArgs e)
        {
            if (cbkeepsign.Checked)
                cbkeeppass.Checked = true;

        }

        /// <summary>
        /// 登陆事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnsignin_Click(object sender, EventArgs e)
        {
            string username=  txtUserName.Text;
            string userpass= txtUserPass.Text;
            //username = "陈少明";
            //userpass = "12900733";
            //登陆。判断是否登陆成功。
            LoadXamarin(username, userpass);
            Toast.MakeText(this, "正在登陆请稍等", ToastLength.Short).Show();
            btnsignin.Enabled = false;
            //记住密码，或自动登陆
            //cbkeepsign.Checked
            //cbkeeppass.Checked 

            

        }
        #endregion

        public void LoadXamarin(string name,string pass)
        {
            //创建一个请求
            var httpReq = (HttpWebRequest)HttpWebRequest.Create(new Uri(string.Format(dbservice.URL_SIGN,name,pass,"1234567")));
            httpReq.BeginGetResponse(new AsyncCallback(ReadXamarin), httpReq);
        }

        //异步回调方法
        public void ReadXamarin(IAsyncResult asyn)
        {
            var httpReq = (HttpWebRequest)asyn.AsyncState;
            try
            {
                using (var httpRes = (HttpWebResponse)httpReq.EndGetResponse(asyn))
                {
                    //判断是否成功获取响应
                    if (httpRes.StatusCode == HttpStatusCode.OK)
                    {
                        //读取响应
                        Stream result = httpRes.GetResponseStream();

                        JsonObject jsresult = (JsonObject)JsonObject.Load(result);

                        Bundle bundleData = new Bundle();


                        string errmessage = jsresult["errorMessage"].ToString().TrimRisk();
                        string success = jsresult["success"].ToString().TrimRisk();
                        string errorCode = jsresult["errorCode"].ToString().TrimRisk();

                        bundleData.PutString("errmessage", errmessage);
                        bundleData.PutString("success", success);
                        bundleData.PutString("errorCode", errorCode);
                        //登陆失败不解析 data 
                        if (string.IsNullOrEmpty(errmessage) && string.IsNullOrEmpty(errorCode))
                        {
                            JsonObject data = (JsonObject)jsresult["data"];
                            bundleData.PutString("data_status", data["status"].ToString().TrimRisk());
                            bundleData.PutString("data_access", data["access"].ToString().TrimRisk());
                            bundleData.PutString("data_userID", data["userID"].ToString().TrimRisk());
                        }

                        //切换到UI线程，否则无法对控件进行操作
                        Message message = new Message();
                        message.What = 1;
                        message.Data = bundleData;
                        handler.SendMessage(message);
                    }
                    else
                        HTTPTimeOut();
                }
            }
            catch (Exception)
            {
                HTTPTimeOut();
            }
            //获取响应
            
        }

        private void HTTPTimeOut()
        {
            Message message = new Message();
            message.What = 2;
            Bundle bundle = new Bundle();
            bundle.PutString("errormessage", "访问远程服务失败！");
            message.Data = bundle;
            handler.SendMessage(message);
        }

        public void Startallship(Bundle result) 
        {
            string errmessage = result.GetString("errmessage");
            string success = result.GetString("success");
            string errorCode = result.GetString("errorCode");

            if (string.IsNullOrEmpty(errmessage) && string.IsNullOrEmpty(errorCode))
            {
                string data_status= result.GetString("data_status");
                app_access=result.GetString("data_access");
                app_userID= result.GetString("data_userID");
                //登陆成功
                if (data_status.Equals("true"))
                {
                    Intent intent = new Intent(this, typeof(allshipinfoActivity));
                    intent.PutExtra("app_access", app_access);
                    intent.PutExtra("app_userID", app_userID);
                    StartActivity(intent);
                    return;
                }
            }
            Toast.MakeText(this, errmessage, ToastLength.Long).Show();
        }
    }
}

