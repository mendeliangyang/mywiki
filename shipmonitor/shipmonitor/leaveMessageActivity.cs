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
using shipmonitor.custom;
using shipmonitor.entity;
using System.Net;
using System.IO;
using System.Json;

namespace shipmonitor
{
    [Activity(Label = "My Activity")]
    public class leaveMessageActivity : Activity, IOnRefreshListener, Android.Views.View.IOnKeyListener
    {
        private refreshlistview listview;
        leaveMessageListviewAdapter adapter;
        List<LeaveMessageEntity> listdata;
        PageEntity page;
        private string userid;
        private string articleID;
        string dataOrder = "desc";
        Handler handler;
        Button btnback;
        Button btn_submit;

        EditText eTmessage;
        //int fristEditTextFoucs = 0;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            View actionbarView= common.setActionBar(this, Resource.Layout.detailstitlebar);
            ((TextView)actionbarView.FindViewById(Resource.Id.detailstilebar_tvtitle)).Text = "查看留言";

            SetContentView(Resource.Layout.leaveMessage);
			btn_submit = (Button)FindViewById(Resource.Id.leavemessage_btn_submit);
            btnSubmitGetFocus();

            userid = this.Intent.GetStringExtra("userID");
            articleID = this.Intent.GetStringExtra("articleID");

            
            listview = (refreshlistview)FindViewById(Resource.Id.leavemessage_lsmessage);

            actionbarView.FindViewById<ImageButton>(Resource.Id.detailstitlebar_btnback).Click += delegate(object sender, EventArgs e) { Finish(); };



            eTmessage = (EditText)FindViewById(Resource.Id.edite_leaveMessage_inputmessage);
            eTmessage.FocusChange += new EventHandler<View.FocusChangeEventArgs>(eTmessage_FocusChange);

            eTmessage.SetOnKeyListener(this);


            
            btn_submit.Click += new EventHandler(btn_submit_Click);
            page = new PageEntity("0", "5");

            listdata = new List<LeaveMessageEntity>();

            adapter = new leaveMessageListviewAdapter(this,listdata);

            listview.Adapter = adapter;

            listview.SetRefresh(this);
            handler = new Handler((Message message) =>
            {
                switch (message.What)
                {
                    case 1:
                        setBtn_SubmitEnable(true);
                        Toast.MakeText(this, message.Data.GetString("errormessage"), ToastLength.Long).Show();
                        break;
                    case 2:
                        TreatAllShipListJson(message.Data.GetString("jsonStr"));
                        break;
                    case 3:
						TreatSendJson(message.Data.GetString("jsonStr"));
                        break;
                    default:
                        break;
                }
            });
            onRefresh();
        }

        //提交评论
        void btn_submit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(eTmessage.Text.Trim()))
            {
                Toast.MakeText(this, "请输入评论信息！", ToastLength.Short).Show();
                return;
            }
            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(string.Format(dbservice.URL_SENDlEAVEMESSAGE, userid, articleID,eTmessage.Text.Trim()));
            webreq.BeginGetResponse(new AsyncCallback(SendWebResponse), webreq);
            setBtn_SubmitEnable(false);
        }

        public void SendWebResponse(IAsyncResult result)
        {
            HttpWebRequest webreq = (HttpWebRequest)result.AsyncState;
            try
            {

                HttpWebResponse httpResult = (HttpWebResponse)webreq.EndGetResponse(result);
                if (httpResult.StatusCode == HttpStatusCode.OK)
                {
                    string streamResult = new StreamReader(httpResult.GetResponseStream()).ReadToEnd();
                    Bundle bundleData = new Bundle();
                    bundleData.PutString("jsonStr", streamResult);

                    Message message = new Message();
                    message.What = 3;
                    message.Data = bundleData;
                    handler.SendMessage(message);
                }
                else
                    common.ServiceTimeOut(handler);

            }
            catch (Exception e)
            {
                common.ServiceTimeOut(handler);
            }
        }

      
        //message 获取焦点
        void eTmessage_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (eTmessage.IsFocused)
            {
                eTmessage.SetTextColor(Android.Graphics.Color.Blue);
            }
            //if (fristEditTextFoucs == 0)
            //{
            //    eTmessage.Text = string.Empty;
            //    fristEditTextFoucs++;
            //    eTmessage.FocusChange -= eTmessage_FocusChange;
            //}
        }



        public void onRefresh()
        {
           if(!page.PageNext())
               return;
            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(string.Format(dbservice.URL_GETlEAVEMESSAGE, articleID, page.PageNumber_int, page.PageSize_int, dataOrder));
            webreq.BeginGetResponse(new AsyncCallback(ReadWebResponse), webreq);
        }

        public void ReadWebResponse (IAsyncResult result)
        {
            HttpWebRequest webreq = (HttpWebRequest)result.AsyncState;
            try
            {

                HttpWebResponse httpResult = (HttpWebResponse)webreq.EndGetResponse(result);
                if (httpResult.StatusCode == HttpStatusCode.OK)
                {
                    string streamResult = new StreamReader(httpResult.GetResponseStream()).ReadToEnd();
                    Bundle bundleData = new Bundle();
                    bundleData.PutString("jsonStr", streamResult);

                    Message message = new Message();
                    message.What = 2;
                    message.Data = bundleData;
                    handler.SendMessage(message);
                }
                else
                    common.ServiceTimeOut(handler);

            }
            catch (Exception e)
            {
                common.ServiceTimeOut(handler);
            }
        }

        private void TreatSendJson(string streamResult)
        {
            JsonObject jsRoot = (JsonObject)JsonObject.Parse(streamResult);
            BaseHttpResponseHead head = common.JsonParseBaseHTTPHead(jsRoot);
            if (!string.IsNullOrEmpty(head.ErrorMessage) || !string.IsNullOrEmpty(head.ErrorCode))
                Toast.MakeText(this, "访问远程服务失败！", ToastLength.Short).Show();
            else
            {
                //刷新数据
				
				listdata.Clear ();
                HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(string.Format(dbservice.URL_GETlEAVEMESSAGE, articleID, page.PageNumber_int, page.PageSize_int, dataOrder));
                webreq.BeginGetResponse(new AsyncCallback(ReadWebResponse), webreq);
            }
            setBtn_SubmitEnable(true);
        }
        private void setBtn_SubmitEnable(bool eanble) 
        {
            if (eanble)
            {
                btn_submit.Enabled = eanble;
                btn_submit.Text = "完成";
            }
            else 
            {
                btn_submit.Enabled = false;
                btn_submit.Text = "提交评论……";
            }
        }
        private void btnSubmitGetFocus() 
        {
            btn_submit.Focusable = true;
            btn_submit.FocusableInTouchMode = true;
            btn_submit.RequestFocus();
            btn_submit.RequestFocusFromTouch();
        }
        private void TreatAllShipListJson(string streamResult)
        {
	        JsonObject jsRoot = (JsonObject)JsonObject.Parse(streamResult);
            BaseHttpResponseHead head = common.JsonParseBaseHTTPHead(jsRoot);
            if (string.IsNullOrEmpty(head.ErrorMessage) && string.IsNullOrEmpty(head.ErrorCode))
            {
                JsonObject jsdata = (JsonObject)jsRoot["data"];
                page.SetTotal(jsdata["totalNumber"].ToString().TrimRisk());
                JsonArray jsdynamic = (JsonArray)jsdata["dynamic"];
                LeaveMessageEntity leaveMessageItem ;
                for (int i = 0; i < jsdynamic.Count; i++)
                {
                    leaveMessageItem = new LeaveMessageEntity();
                    JsonObject temp_jsdynamic = (JsonObject)jsdynamic[i];
                    leaveMessageItem.userName = temp_jsdynamic.GetStringByProperty("userName");
                    leaveMessageItem.time = temp_jsdynamic.GetStringByProperty("time");
                    leaveMessageItem.message = temp_jsdynamic.GetStringByProperty("message");
                    listdata.Add(leaveMessageItem);
                }
                //同时 adatper 数据加载完成
                this.adapter.NotifyDataSetChanged();
                this.listview.OnRefreshComplete();
            }
            else
                Toast.MakeText(this, "访问远程服务失败！", ToastLength.Short).Show();
        }

        public bool OnKey(View v, Keycode keyCode, KeyEvent e)
        {
            if (Keycode.Enter==keyCode&&e.Action==KeyEventActions.Down)
            {
                btnSubmitGetFocus();
            }
            return false;
        }
    }
}