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

namespace shipmonitor
{
    [Activity(Label = "My Activity")]
    public class baseInfoActivity : Activity
    {
        View actionBar;
        private string userid;
        private string articleID;
        Handler handler;

        BaseShipDetail baseshipDetail;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            actionBar = common.setActionBar(this, Resource.Layout.detailstitlebar);
            ((TextView)actionBar.FindViewById(Resource.Id.detailstilebar_tvtitle)).Text = "基本信息/部门意见";
            actionBar.FindViewById<ImageButton>(Resource.Id.detailstitlebar_btnback).Click += delegate(object sender, EventArgs e)
            {
                this.Finish();
            };
            // Create your application here
            SetContentView(Resource.Layout.baseInfolayout);
            userid = this.Intent.GetStringExtra("userID");
            articleID = this.Intent.GetStringExtra("articleID");


            handler = new Handler((Message message) =>
            {
                switch (message.What)
                {
                    case 1:
                        Toast.MakeText(this, message.Data.GetString("errormessage"), ToastLength.Long).Show();
                        break;
                    case 2:
                        TreatBaseInfoJson(message.Data.GetString("jsonStr"));
                        break;
                    default:
                        break;
                }
            });
			onload ();
        }
        private void onload() 
        {
            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(string.Format(dbservice.URL_GETBASEINFO,userid,articleID));
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



        private void TreatBaseInfoJson(string streamResult)
        {
            JsonObject jsRoot = (JsonObject)JsonObject.Parse(streamResult);
            BaseHttpResponseHead head = common.JsonParseBaseHTTPHead(jsRoot);
            if (string.IsNullOrEmpty(head.ErrorMessage) && string.IsNullOrEmpty(head.ErrorCode))
            {
                JsonObject jsdata1 = (JsonObject)jsRoot["data1"];

                JsonObject jsdata2 = (JsonObject)jsRoot["data2"];

                JsonObject jsdata3 = (JsonObject)jsRoot["data3"];

                baseshipDetail = new BaseShipDetail();

                #region data1
                
                FindViewById<TextView>(Resource.Id.basedetail_tv_settime).Text = baseshipDetail.settime = jsdata1.GetStringByProperty("settime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_name).Text=baseshipDetail.name = jsdata1.GetStringByProperty("name");
                FindViewById<TextView>(Resource.Id.basedetail_tv_id).Text = baseshipDetail.id = jsdata1.GetStringByProperty("id");
                FindViewById<TextView>(Resource.Id.basedetail_tv_totaltime).Text = baseshipDetail.totaltime = jsdata1.GetStringByProperty("totaltime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_starttime).Text = baseshipDetail.starttime = jsdata1.GetStringByProperty("starttime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_endtime).Text = baseshipDetail.endtime = jsdata1.GetStringByProperty("endtime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_port).Text = baseshipDetail.port = jsdata1.GetStringByProperty("port");
                FindViewById<TextView>(Resource.Id.basedetail_tv_dock).Text = baseshipDetail.dock = jsdata1.GetStringByProperty("dock");
                FindViewById<TextView>(Resource.Id.basedetail_tv_load).Text = baseshipDetail.load = jsdata1.GetStringByProperty("load");
                FindViewById<TextView>(Resource.Id.basedetail_tv_unload).Text = baseshipDetail.unload = jsdata1.GetStringByProperty("unload");
                FindViewById<TextView>(Resource.Id.basedetail_tv_loadtime).Text = baseshipDetail.loadtime = jsdata1.GetStringByProperty("loadtime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_unloadtime).Text = baseshipDetail.unloadtime = jsdata1.GetStringByProperty("unloadtime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_lasttime).Text = baseshipDetail.lasttime = jsdata1.GetStringByProperty("lasttime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_voyagetime).Text = baseshipDetail.voyagetime = jsdata1.GetStringByProperty("voyagetime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_totalvoyagetime).Text = baseshipDetail.totalvoyagetime = jsdata1.GetStringByProperty("totalvoyagetime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_cargoportcost).Text = baseshipDetail.cargoportcost = jsdata1.GetStringByProperty("cargoportcost");
                FindViewById<TextView>(Resource.Id.basedetail_tv_emptyrange).Text = baseshipDetail.emptyrange = jsdata1.GetStringByProperty("emptyrange");
                FindViewById<TextView>(Resource.Id.basedetail_tv_loadrange).Text = baseshipDetail.loadrange = jsdata1.GetStringByProperty("loadrange");
                FindViewById<TextView>(Resource.Id.basedetail_tv_istime).Text = baseshipDetail.istime = jsdata1.GetStringByProperty("istime");
                FindViewById<TextView>(Resource.Id.basedetail_tv_attention).Text = baseshipDetail.attention = jsdata1.GetStringByProperty("attention");

                #endregion

                #region data2
                JsonValue loadcontactjsono_value = jsdata2["0"];

                if (loadcontactjsono_value.JsonType== JsonType.Array)
                {
                    JsonArray loadcontactjson = (JsonArray)loadcontactjsono_value;

                    baseshipDetail.loadcontact = new ContactInfoEntity();
                      FindViewById<TextView>(Resource.Id.basedetail_tv_loadname).Text=baseshipDetail.loadcontact.contactName = loadcontactjson[0].ToString().TrimRisk();
                      FindViewById<TextView>(Resource.Id.basedetail_tv_loadcompany).Text = baseshipDetail.loadcontact.company = loadcontactjson[1].ToString().TrimRisk();
                      FindViewById<TextView>(Resource.Id.basedetail_tv_loadphone).Text = baseshipDetail.loadcontact.phone = loadcontactjson[2].ToString().TrimRisk();
                      FindViewById<TextView>(Resource.Id.basedetail_tv_loadtelepone).Text = baseshipDetail.loadcontact.telephone = loadcontactjson[3].ToString().TrimRisk();
                      FindViewById<TextView>(Resource.Id.basedetail_tv_loadfax).Text = baseshipDetail.loadcontact.fax = loadcontactjson[4].ToString().TrimRisk();
                      FindViewById<TextView>(Resource.Id.basedetail_tv_loademail).Text = baseshipDetail.loadcontact.email = loadcontactjson[5].ToString().TrimRisk();
                }

                JsonValue unloadcontactjson_value = jsdata2["1"];
                if (unloadcontactjson_value.JsonType==JsonType.Array)
                {
                    JsonArray unloadcontactjson = (JsonArray)unloadcontactjson_value;
                    baseshipDetail.unloadcontact = new ContactInfoEntity();
                    FindViewById<TextView>(Resource.Id.basedetail_tv_unloadname).Text = baseshipDetail.unloadcontact.contactName = unloadcontactjson[0].ToString().TrimRisk();
                    FindViewById<TextView>(Resource.Id.basedetail_tv_unloadcompany).Text = baseshipDetail.unloadcontact.company = unloadcontactjson[1].ToString().TrimRisk();
                    FindViewById<TextView>(Resource.Id.basedetail_tv_unloadphone).Text = baseshipDetail.unloadcontact.phone = unloadcontactjson[2].ToString().TrimRisk();
                    FindViewById<TextView>(Resource.Id.basedetail_tv_unloadtelepone).Text = baseshipDetail.unloadcontact.telephone = unloadcontactjson[3].ToString().TrimRisk();
                    FindViewById<TextView>(Resource.Id.basedetail_tv_unloadfax).Text = baseshipDetail.unloadcontact.fax = unloadcontactjson[4].ToString().TrimRisk();
                    FindViewById<TextView>(Resource.Id.basedetail_tv_unloademail).Text = baseshipDetail.unloadcontact.email = unloadcontactjson[5].ToString().TrimRisk();
                }


                JsonValue contractcontactjson_value = (JsonArray)jsdata2["2"];
                if (contractcontactjson_value.JsonType==JsonType.Array)
                {
                    JsonArray contractcontactjson = (JsonArray)contractcontactjson_value;
                    baseshipDetail.contractcontact= new ContactInfoEntity();
                    {
                        FindViewById<TextView>(Resource.Id.basedetail_tv_contractname).Text=baseshipDetail.contractcontact.contactName = contractcontactjson[0].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_contractcompany).Text=baseshipDetail.contractcontact.company = contractcontactjson[1].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_contractphone).Text = baseshipDetail.contractcontact.phone = contractcontactjson[2].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_contracttelepone).Text = baseshipDetail.contractcontact.telephone = contractcontactjson[3].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_contractfax).Text = baseshipDetail.contractcontact.fax = contractcontactjson[4].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_contractemail).Text = baseshipDetail.contractcontact.email = contractcontactjson[5].ToString().TrimRisk();
                    };
                }
                JsonValue ownercontactjson_value = jsdata2["3"];
                if (ownercontactjson_value.JsonType==JsonType.Array)
                {
                    JsonArray ownercontactjson = (JsonArray)ownercontactjson_value;
                    baseshipDetail.ownercontact = new ContactInfoEntity();
                    {
                        FindViewById<TextView>(Resource.Id.basedetail_tv_ownername).Text = baseshipDetail.ownercontact.contactName = ownercontactjson[0].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_ownercompany).Text = baseshipDetail.ownercontact.company = ownercontactjson[1].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_ownerphone).Text = baseshipDetail.ownercontact.phone = ownercontactjson[2].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_ownertelepone).Text = baseshipDetail.ownercontact.telephone = ownercontactjson[3].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_ownerfax).Text = baseshipDetail.ownercontact.fax = ownercontactjson[4].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_owneremail).Text = baseshipDetail.ownercontact.email = ownercontactjson[5].ToString().TrimRisk();
                    };
                }

                JsonValue proxycontactjson_value = jsdata2["4"];
                if (proxycontactjson_value.JsonType == JsonType.Array)
                {
                    JsonArray proxycontactjson = (JsonArray)proxycontactjson_value;
                    baseshipDetail.proxycontact= new ContactInfoEntity();
                    {
                        FindViewById<TextView>(Resource.Id.basedetail_tv_proxyname).Text=baseshipDetail.proxycontact.contactName = proxycontactjson[0].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_proxycompany).Text = baseshipDetail.proxycontact.company = proxycontactjson[1].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_proxyphone).Text = baseshipDetail.proxycontact.phone = proxycontactjson[2].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_proxytelepone).Text = baseshipDetail.proxycontact.telephone = proxycontactjson[3].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_proxyfax).Text = baseshipDetail.proxycontact.fax = proxycontactjson[4].ToString().TrimRisk();
                        FindViewById<TextView>(Resource.Id.basedetail_tv_proxyemail).Text = baseshipDetail.proxycontact.email = proxycontactjson[5].ToString().TrimRisk();
                    };
                }
                #endregion

                #region data3
                FindViewById<TextView>(Resource.Id.basedetail_tv_business).Text = baseshipDetail.business = jsdata3.GetStringByProperty("business");
                FindViewById<TextView>(Resource.Id.basedetail_tv_maintenance).Text = baseshipDetail.maintenance = jsdata3.GetStringByProperty("maintenance");
                FindViewById<TextView>(Resource.Id.basedetail_tv_dispatch).Text = baseshipDetail.dispatch = jsdata3.GetStringByProperty("dispatch");
                FindViewById<TextView>(Resource.Id.basedetail_tv_sea).Text = baseshipDetail.sea = jsdata3.GetStringByProperty("sea");
                FindViewById<TextView>(Resource.Id.basedetail_tv_personnel).Text = baseshipDetail.personnel = jsdata3.GetStringByProperty("personnel");
                FindViewById<TextView>(Resource.Id.basedetail_tv_manager).Text = baseshipDetail.manager = jsdata3.GetStringByProperty("manager");
                #endregion
            }
            else
                Toast.MakeText(this, "访问远程服务失败！", ToastLength.Short).Show();
        }
    }
}