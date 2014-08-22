
#define IOS

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
    public class allshipinfoActivity : Activity, IOnRefreshListener
    {
        View avtionbarview;
        refreshlistview refreshlv;
        allshipadapter shipsadapter;
        List<flightnumberEntity> lsflightdata;
        string userId;
        string dataOrder = "asc";
        Handler handler;
        PageEntity pageEntity;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            avtionbarview = common.setActionBar(this, Resource.Layout.maintitlebar);

            SetContentView(Resource.Layout.allshipinfo);
            //获取ID
            userId = this.Intent.GetStringExtra(common.app_userID);

            pageEntity = new PageEntity("0","10");
            

            handler = new Handler((Message message) =>
            {
                switch (message.What)
                {
                    case 1:
                        Toast.MakeText(this, message.Data.GetString("errormessage"), ToastLength.Long).Show();
                        break;
                    case 2:
                        TreatAllShipListJson(message.Data.GetString("jsonStr"));
                        break;
                    default:
                        break;
                }
            });

            
            //eTmessage.FocusChange 
            //设置listview的刷新方法
            refreshlv = (refreshlistview)FindViewById(Resource.Id.rflv_allship_rflvallship);
            refreshlv.SetRefresh(this);

            //TODO 异步获取数据,获取完成后设置 listview adapter
            lsflightdata = new List<flightnumberEntity>();

            shipsadapter = new allshipadapter(this, lsflightdata);

            refreshlv.Adapter = shipsadapter;

            onRefresh();
          

            refreshlv.ItemClick += new EventHandler<AdapterView.ItemClickEventArgs>(refreshlv_ItemClick);
        }

        

        void refreshlv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            flightnumberEntity clickitem=shipsadapter[e.Position];
            if (!clickitem.IsWanCheng.Equals("true"))
            {

                Intent intent = new Intent(this, typeof(shipdetailActivity));

                intent.PutExtra("userID", this.userId);
                intent.PutExtra("articleID", clickitem.ArticleID);

                intent.PutExtra("IsDiaoDu", clickitem.IsDiaoDu);
                intent.PutExtra("IsHaiWu", clickitem.IsHaiWu);
                intent.PutExtra("IsJiWu", clickitem.IsJiWu);
                intent.PutExtra("IsRenShi", clickitem.IsRenShi);
                intent.PutExtra("IsShangWu", clickitem.IsShangWu);

                StartActivity(intent);
            }
            else 
            {
                Intent intent = new Intent(this, typeof(owerinfoactivity));
                intent.PutExtra("userID", userId);
                intent.PutExtra("articleID", clickitem.ArticleID);
                intent.PutExtra("acvitityType", "totalcost");
                StartActivity(intent);
                //Toast.MakeText(this, "正在开发", ToastLength.Short).Show();
            }


        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public void onRefresh()
        {
            if (!pageEntity.PageNext())
                return;

            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(string.Format(dbservice.URL_ALLFLIGHT, userId, pageEntity.PageNumber_int, pageEntity.PageSize_int, dataOrder));
            webreq.BeginGetResponse(new AsyncCallback(ReadWebResponse), webreq);
        }

        private void ReadWebResponse(IAsyncResult result)
        {
            HttpWebRequest webreq = (HttpWebRequest)result.AsyncState;
            try
            {

                HttpWebResponse httpResult = (HttpWebResponse)webreq.EndGetResponse(result);
                if (httpResult.StatusCode == HttpStatusCode.OK)
                {
                    string streamResult = new StreamReader(httpResult.GetResponseStream()).ReadToEnd();
                    Bundle bundleData = new Bundle();
                    bundleData.PutString("jsonStr",streamResult);

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

        private  void TreatAllShipListJson(string streamResult)
        {
            JsonObject jsRoot = (JsonObject)JsonObject.Parse(streamResult);
            BaseHttpResponseHead head = common.JsonParseBaseHTTPHead(jsRoot);
            if (string.IsNullOrEmpty(head.ErrorMessage) && string.IsNullOrEmpty(head.ErrorCode))
            {
                JsonObject jsdata = (JsonObject)jsRoot["data"];
                pageEntity.SetTotal(jsdata["totalNumber"].ToString().TrimRisk());
                JsonArray jsdynamic = (JsonArray)jsdata["dynamic"];
                flightnumberEntity tempflightnumber;
                for (int i = 0; i < jsdynamic.Count; i++)
                {
                    tempflightnumber = new flightnumberEntity();
                    JsonObject temp_jsdynamic = (JsonObject)jsdynamic[i];

                    JsonArray js_ports = (JsonArray)temp_jsdynamic["ports"];

                    for (int j = 0; j < js_ports.Count; j++)
                    {
                        JsonObject temp_port = (JsonObject)js_ports[j];
                        string temp_str_port = temp_port.GetStringByProperty("port_name");
                        tempflightnumber.Ports += (temp_str_port+" ->");
                    }

					tempflightnumber.Ports=tempflightnumber.Ports.Remove(tempflightnumber.Ports.Length - 3, 3);

                    tempflightnumber.Title1 = temp_jsdynamic.GetStringByProperty("title1");
                    tempflightnumber.Title2 = temp_jsdynamic.GetStringByProperty("title2");
                    tempflightnumber.ArticleID = temp_jsdynamic.GetStringByProperty("articleID");
                    tempflightnumber.IsRead = temp_jsdynamic.GetStringByProperty("isRead");
                    //tempflightnumber.IsRead = "false";

                    tempflightnumber.IsWanCheng =temp_jsdynamic.GetStringByProperty("isWancheng");
                    //判断是否完成
                    if (tempflightnumber.IsWanCheng.Equals("true"))
                    {
                        tempflightnumber.Profit = temp_jsdynamic.GetStringByProperty("profit");
                        tempflightnumber.Expend = temp_jsdynamic.GetStringByProperty("expend");
                        tempflightnumber.Freightrevenue = temp_jsdynamic.GetStringByProperty("Freightrevenue");
                    }else
                    {
                        tempflightnumber.IsShangWu = temp_jsdynamic.GetStringByProperty("isShanwu");
                        tempflightnumber.IsDiaoDu = temp_jsdynamic.GetStringByProperty("isDaiodu");
                        tempflightnumber.IsHaiWu = temp_jsdynamic.GetStringByProperty("isHaiwu");
                        tempflightnumber.IsRenShi = temp_jsdynamic.GetStringByProperty("isRenshi");
                        tempflightnumber.IsZhongJingLi = temp_jsdynamic.GetStringByProperty("isZongjingli");
                        tempflightnumber.IsJiWu = "false";//temp_jsdynamic.GetStringByProperty("");
                    }
                    lsflightdata.Add(tempflightnumber);
                }
                //同时 adatper 数据加载完成
                this.shipsadapter.NotifyDataSetChanged();
				this.refreshlv.OnRefreshComplete ();
            }
            else
                Toast.MakeText( this ,"访问远程服务失败！", ToastLength.Short).Show();
        }


        
    }
}