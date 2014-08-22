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
using System.Json;
using shipmonitor.custom;
using shipmonitor.entity;
using System.Net;
using System.IO;

namespace shipmonitor
{
    [Activity(Label = "My Activity")]
    public class servingcostactivity : Activity
    {
        private string userid;
        private string articleID;
        View actionbarView;
        Handler handler;
        LinearLayout linearlayout;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            actionbarView = common.setActionBar(this, Resource.Layout.detailstitlebar);

            ((TextView)actionbarView.FindViewById(Resource.Id.detailstilebar_tvtitle)).Text = "招待/其他费用";
            
            actionbarView.FindViewById<ImageButton>(Resource.Id.detailstitlebar_btnback).Click += delegate(object sender, EventArgs e)
            {
                this.Finish();
            };
            SetContentView(Resource.Layout.owerinfoactivity);

            userid = Intent.GetStringExtra("userID");
            articleID = Intent.GetStringExtra("articleID");

            linearlayout = FindViewById<LinearLayout>(Resource.Id.owner_linearlayout);


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
            onload();
        }
        private void onload()
        {
            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(string.Format(dbservice.URL_GETSERVINGCOST, userid, articleID));
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
                JsonObject jsdata = (JsonObject)jsRoot["data"];

                int totalnumber=0,totaldetail=0;
                int.TryParse(jsdata.GetStringByProperty("totalNumber"), out totalnumber);
                string toatal = jsdata.GetStringByProperty("toatal");

                JsonValue jsdynamicvalue = jsdata["dynamic"];

                lineartextview_customcontrl temp_control;
                ViewGroup.LayoutParams layoutparams = new ViewGroup.LayoutParams(WindowManagerLayoutParams.FillParent, WindowManagerLayoutParams.WrapContent);

                if (jsdynamicvalue.JsonType== JsonType.Array)
                {
                    JsonArray jsdynamicarray = (JsonArray)jsdynamicvalue;
                    foreach (var item in jsdynamicarray)
                    {
                        JsonObject item_object = (JsonObject)item;
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("港口", item_object.GetStringByProperty("port"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("码头", item_object.GetStringByProperty("dock"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("招待费用", item_object.GetStringByProperty("servecost"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("其他费用", item_object.GetStringByProperty("othercost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        int.TryParse(item_object.GetStringByProperty("totalNumber"), out totaldetail);

                        for (int i = 0; i < totaldetail; i++)
                        {
                            temp_control = new lineartextview_customcontrl(this);
                            JsonValue detailvalue= item_object[i.ToString()];
                            if (detailvalue.JsonType == JsonType.Array)
                            {
                                JsonArray detailvalue_array = (JsonArray)detailvalue;

                                for (int j = 1; j < detailvalue_array.Count; j++)
                                {
                                    //JsonObject temp_detailobject = (JsonObject)detailvalue_array[j];
                                    if (j==1)
                                    {
                                        temp_control = new lineartextview_customcontrl(this);
										temp_control.SetLableValue("招代费项目名称", detailvalue_array[j]);
                                        temp_control.tv_value.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextSize(Android.Util.ComplexUnitType.Dip, 9); 
                                        temp_control.tv_value.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.GoneLineView();
                                        linearlayout.AddView(temp_control, layoutparams);   
                                    }
                                    if (j == 2)
                                    {
                                        temp_control = new lineartextview_customcontrl(this);
                                        temp_control.SetLableValue("数量", detailvalue_array[j]); 
                                        temp_control.tv_value.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.tv_value.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.GoneLineView();
                                        linearlayout.AddView(temp_control, layoutparams);
                                    }
                                    if (j == 3)
                                    {
                                        temp_control = new lineartextview_customcontrl(this);
                                        temp_control.SetLableValue("单位", detailvalue_array[j]);
                                        temp_control.tv_value.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.tv_value.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.GoneLineView();
                                        linearlayout.AddView(temp_control, layoutparams);
                                    }
                                    if (j == 4)
                                    {
                                        temp_control = new lineartextview_customcontrl(this);
                                        temp_control.SetLableValue("总价", detailvalue_array[j]);
                                        temp_control.tv_value.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.tv_value.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.GoneLineView();
                                        linearlayout.AddView(temp_control, layoutparams);
                                    }
                                    if (j == 5)
                                    {
                                        temp_control = new lineartextview_customcontrl(this);
                                        temp_control.SetLableValue("接收方", detailvalue_array[j]);
                                        temp_control.tv_value.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.tv_value.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.GoneLineView();
                                        linearlayout.AddView(temp_control, layoutparams);
                                    } if (j == 6)
                                    {
                                        temp_control = new lineartextview_customcontrl(this);
                                        temp_control.SetLableValue("种类", detailvalue_array[j]);
                                        temp_control.tv_value.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.tv_value.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.GoneLineView();
                                        linearlayout.AddView(temp_control, layoutparams);
                                    } if (j == 7)
                                    {
                                        temp_control = new lineartextview_customcontrl(this);
                                        temp_control.SetLableValue("效果", detailvalue_array[j]);
                                        temp_control.tv_value.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.tv_value.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.GoneLineView();
                                        linearlayout.AddView(temp_control, layoutparams);
                                    } if (j == 8)
                                    {
                                        temp_control = new lineartextview_customcontrl(this,false,false,true);
                                        temp_control.SetLableValue("备注", detailvalue_array[j]);
                                        temp_control.tv_value.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextColor(Android.Graphics.Color.DarkGray);
                                        temp_control.tv_lable.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        temp_control.tv_value.SetTextSize(Android.Util.ComplexUnitType.Dip, 9);
                                        linearlayout.AddView(temp_control, layoutparams);
                                    }
                                    
                                }
                            }
                        }
                    }
                }







            }
            else
                Toast.MakeText(this, "访问远程服务失败！", ToastLength.Short).Show();
        }

    }
}