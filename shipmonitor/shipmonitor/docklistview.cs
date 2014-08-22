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
using System.Net;
using System.IO;
using System.Json;
using shipmonitor.entity;
using shipmonitor.custom;

namespace shipmonitor
{
    [Activity(Label = "My Activity")]
    public class docklistview : Activity
    {
        private string userid;
        private string articleID;
        View actionBar;
        Handler handler;
        List<DockInfoEntity> listdata;
        dockcustomadapter adapter;
		ListView listview;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            actionBar = common.setActionBar(this, Resource.Layout.detailstitlebar);
            ((TextView)actionBar.FindViewById(Resource.Id.detailstilebar_tvtitle)).Text = "港口码头";
            // Create your application here
            SetContentView(Resource.Layout.docklistview);

            userid = this.Intent.GetStringExtra("userID");
            articleID = this.Intent.GetStringExtra("articleID");
            actionBar.FindViewById<ImageButton>(Resource.Id.detailstitlebar_btnback).Click += delegate(object sender, EventArgs e) {Finish();  };

			listview = FindViewById<ListView> (Resource.Id.docklistview_lv);
			listdata = new List<DockInfoEntity>();
            adapter = new dockcustomadapter(this, listdata);
			listview.Adapter = adapter;
            handler = new Handler((Message message) =>
            {
                switch (message.What)
                {
                    case 1:
                        Toast.MakeText(this, message.Data.GetString("errormessage"), ToastLength.Long).Show();
                        break;
                    case 2:
                        TreatInfoJson(message.Data.GetString("jsonStr"));
                        break;
                    default:
                        break;
                }
            });
            onload();
        }

        private void onload()
        {
            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(string.Format(dbservice.URL_GETDOCKLIST, userid, articleID));
            webreq.BeginGetResponse(new AsyncCallback(ReadWebResponse), webreq);
        }
        public void ReadWebResponse(IAsyncResult result)
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


        private void TreatInfoJson(string streamResult)
        {
            JsonObject jsRoot = (JsonObject)JsonObject.Parse(streamResult);
            BaseHttpResponseHead head = common.JsonParseBaseHTTPHead(jsRoot);
            
            int total =0 ;
            int.TryParse(jsRoot["totalNumber"].ToString(),out total);

            
            if (string.IsNullOrEmpty(head.ErrorMessage) && string.IsNullOrEmpty(head.ErrorCode))
            {
                JsonValue datavalue = jsRoot["data"];
                if (datavalue.JsonType == JsonType.Array)
                {
                    JsonArray dataarray = (JsonArray)datavalue;
                    DockInfoEntity dockInfoitem;
                    foreach (var item in dataarray)
                    {
                        dockInfoitem = new DockInfoEntity();
                        dockInfoitem.name = ((JsonObject)item).GetStringByProperty("name");
                        dockInfoitem.port = ((JsonObject)item).GetStringByProperty("port");
                        dockInfoitem.linkman = ((JsonObject)item).GetStringByProperty("linkman");
                        dockInfoitem.company = ((JsonObject)item).GetStringByProperty("company");
                        dockInfoitem.phone = ((JsonObject)item).GetStringByProperty("phone");
                        dockInfoitem.tel = ((JsonObject)item).GetStringByProperty("tel");
                        dockInfoitem.fax = ((JsonObject)item).GetStringByProperty("fax");
                        dockInfoitem.mail = ((JsonObject)item).GetStringByProperty("mail");
                        listdata.Add(dockInfoitem);
                    }
                }
            
                adapter.NotifyDataSetChanged();
            }
            else
                Toast.MakeText(this, "访问远程服务失败！", ToastLength.Short).Show();
        }
    }
}