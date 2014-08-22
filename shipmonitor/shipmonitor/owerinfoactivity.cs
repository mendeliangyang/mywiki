using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Net;
using System.IO;
using System.Json;
using shipmonitor.entity;
using shipmonitor.custom;
using Android.Content;

namespace shipmonitor
{
    [Activity(Label = "My Activity")]
    public class owerinfoactivity : Activity
    {
        private string userid;
        private string articleID;
        View actionbarView;
        Handler handler;
		LinearLayout linearlayout;
        string acvitityType;
        string temp_url = dbservice.URL_GETOWERINFO;

        Android.Content.Res.Resources resource = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            actionbarView = common.setActionBar(this, Resource.Layout.detailstitlebar);

            userid = Intent.GetStringExtra("userID");
            articleID = Intent.GetStringExtra("articleID");
            acvitityType = Intent.GetStringExtra("acvitityType");

            #region url title
            
            string title_temp = "货物货主";

            //URL_GETDOCKLIST
            if (acvitityType.Equals("baseinfo"))
            {
                title_temp = "基本信息";
                temp_url = dbservice.URL_GETBASEINFO;
            }
            else if (acvitityType.Equals("dockport"))
            {
                title_temp = "港口码头";
                temp_url = dbservice.URL_GETDOCKLIST;
            }
            else if (acvitityType.Equals("portsetvice"))
            {
                title_temp = "港口耗时/其他费用";
                temp_url = dbservice.URL_GETPROTSERVING;
            }
            else if (acvitityType.Equals("gas"))
            {
                title_temp = "燃油费用";
                temp_url = dbservice.URL_GETGAS;
            }
            else if (acvitityType.Equals("current"))
            {
                title_temp = "当前情况";
                temp_url = dbservice.URL_GETCURRENT;

            }
            else if (acvitityType.Equals("expense"))
            {
                title_temp = "船员费用/开支合计";
                temp_url = dbservice.URL_GETEXPENSE;
            }
            else if (acvitityType.Equals("projectFollow"))
            {
                title_temp = "项目跟进人";
                temp_url = dbservice.URL_GETPROJECTFOLLW;
            }
            else if (acvitityType.Equals("totalcost"))
            {
                title_temp = "费用统计";
                temp_url = dbservice.URL_GETTOTALCOST;
                //设置显示明细
                actionbarView.FindViewById<View>(Resource.Id.detailstilebar_view_donedetail).Visibility= ViewStates.Visible;

                ImageButton moredetail= actionbarView.FindViewById<ImageButton>(Resource.Id.detailstitlebar_btn_donedetail);
                moredetail.Visibility = ViewStates.Visible;
                moredetail.Click += delegate(object sender, EventArgs e) 
                {
                    Intent intent = new Intent(this, typeof(shipdetailActivity));

                    intent.PutExtra("userID", userid);
                    intent.PutExtra("articleID", articleID);

                    StartActivity(intent);
                };
            }
            
            

            #endregion

            ((TextView)actionbarView.FindViewById(Resource.Id.detailstilebar_tvtitle)).Text = title_temp;
            
            actionbarView.FindViewById<ImageButton>(Resource.Id.detailstitlebar_btnback).Click += delegate(object sender, EventArgs e)
            {
                this.Finish();
            };
            SetContentView(Resource.Layout.owerinfoactivity);


            linearlayout = FindViewById<LinearLayout>(Resource.Id.owner_linearlayout);

            resource = BaseContext.Resources;
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
            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(string.Format(temp_url, userid, articleID));
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
                TreatJson(jsRoot);
                
            }
            else
                Toast.MakeText(this, "访问远程服务失败！", ToastLength.Short).Show();
        }

        public void TreatJson(JsonObject jsRoot) 
        {
			lineartextview_customcontrl temp_control;
			ViewGroup.LayoutParams layoutparams = new ViewGroup.LayoutParams(WindowManagerLayoutParams.FillParent, WindowManagerLayoutParams.WrapContent);
            LinearLayout.LayoutParams margin_bottom_ten = new LinearLayout.LayoutParams(WindowManagerLayoutParams.FillParent, WindowManagerLayoutParams.WrapContent);
            //margin_bottom_ten.BottomMargin = 10;
            margin_bottom_ten.TopMargin = 10;

            if (acvitityType.Equals("baseinfo"))
            {
                JsonObject jsdata1 = (JsonObject)jsRoot["data1"];

                JsonObject jsdata2 = (JsonObject)jsRoot["data2"];

                JsonObject jsdata3 = (JsonObject)jsRoot["data3"];

                #region 航次信息
                
                CreatAddTextview("航次信息", string.Empty, true);
                CreatAddTextview("航次创建时间", jsdata1.GetStringByProperty("settime"));
                CreatAddTextview("航名", jsdata1.GetStringByProperty("name"));
                CreatAddTextview("航次编号", jsdata1.GetStringByProperty("id"));
                CreatAddTextview("总耗时", jsdata1.GetStringByProperty("totaltime"));
                CreatAddTextview("开始时间", jsdata1.GetStringByProperty("starttime"));
                CreatAddTextview("结束时间", jsdata1.GetStringByProperty("endtime"));
                CreatAddTextview("本航次开航港口", jsdata1.GetStringByProperty("port"));
                CreatAddTextview("本航次开航码头", jsdata1.GetStringByProperty("dock"));
                CreatAddTextview("装港", jsdata1.GetStringByProperty("load"));
                CreatAddTextview("卸港", jsdata1.GetStringByProperty("unload"));
                CreatAddTextview("总装货时间", jsdata1.GetStringByProperty("loadtime"));
                CreatAddTextview("总卸货时间", jsdata1.GetStringByProperty("unloadtime"));
                CreatAddTextview("最后离港时间", jsdata1.GetStringByProperty("lasttime"));
                CreatAddTextview("该航次总耗时", jsdata1.GetStringByProperty("voyagetime"));
                CreatAddTextview("总航行时间", jsdata1.GetStringByProperty("totalvoyagetime"));
                CreatAddTextview("总作业时间", jsdata1.GetStringByProperty("bustime"));
                CreatAddTextview("总等待时间", jsdata1.GetStringByProperty("waittime"));
                //备注
                temp_control = new lineartextview_customcontrl(this, false, false, true);
                temp_control.SetLableValue("备注", jsdata1.GetStringByProperty("remark"));
                temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                linearlayout.AddView(temp_control, layoutparams);

                CreatAddTextview("港建费支付", jsdata1.GetStringByProperty("buildpay"));
                CreatAddTextview("货物港务费", jsdata1.GetStringByProperty("cargoportcost"));
                CreatAddTextview("重载航程", jsdata1.GetStringByProperty("loadrange"));
                CreatAddTextview("空载航程", jsdata1.GetStringByProperty("emptyrange"));
                CreatAddTextview("总航程", jsdata1.GetStringByProperty("totalrange"));
                CreatAddTextview("赶船期", jsdata1.GetStringByProperty("istime"));
                //注意事项
                temp_control = new lineartextview_customcontrl(this, false, false, true);
                temp_control.SetLableValue("注意事项", jsdata1.GetStringByProperty("attention"));
                temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                //temp_control.SetLineViewLayoutParams(margin_bottom_ten);
                linearlayout.AddView(temp_control, layoutparams);

                #endregion

                #region 联系人信息  
                for (int i = 0; i < 5; i++)
                {
                    JsonValue loadcontactjsono_value = jsdata2[i.ToString()];
                    if (loadcontactjsono_value.JsonType == JsonType.Array)
                    {
                        JsonArray loadcontactjson = (JsonArray)loadcontactjsono_value;
                        string linkman = loadcontactjson[0].ToString().TrimRisk();
                        if (i == 0)
                            CreatAddTextview("装货港联系人", linkman, true);
                        else if (i == 1)
                            CreatAddTextview("卸货港联系人", linkman, true);
                        else if (i == 2)
                            CreatAddTextview("合同联系人", linkman, true);
                        else if (i == 3)
                            CreatAddTextview("货主联系人", linkman, true);
                        else if (i == 4)
                            CreatAddTextview("货代联系人", linkman, true);

                        CreatAddTextview("公司", loadcontactjson[1].ToString().TrimRisk());
                        CreatAddTextview("电话", loadcontactjson[2].ToString().TrimRisk());
                        CreatAddTextview("固话", loadcontactjson[3].ToString().TrimRisk());
                        CreatAddTextview("传真", loadcontactjson[4].ToString().TrimRisk());

                        temp_control= CreatAddTextview("邮箱", loadcontactjson[5].ToString().TrimRisk());
                        //temp_control.SetLineViewLayoutParams(margin_bottom_ten);
                    }
                    else
                    {
                        if (i == 0)
                            CreatAddTextview("装货港联系人", string.Empty, true);
                        else if (i == 1)
                            CreatAddTextview("卸货港联系人", string.Empty, true);
                        else if (i == 2)
                            CreatAddTextview("合同联系人", string.Empty, true);
                        else if (i == 3)
                            CreatAddTextview("货主联系人", string.Empty, true);
                        else if (i == 4)
                            CreatAddTextview("货代联系人", string.Empty, true);

                        CreatAddTextview("公司", string.Empty);
                        CreatAddTextview("电话", string.Empty);
                        CreatAddTextview("固话", string.Empty);
                        CreatAddTextview("传真", string.Empty);
                        temp_control=CreatAddTextview("邮箱", string.Empty);
                       // temp_control.SetLineViewLayoutParams(margin_bottom_ten);

                    }
                }
                #endregion

                #region 各部门意见
                CreatAddTextview("各部门意见", string.Empty, true);

                temp_control = new lineartextview_customcontrl(this, false, false, true);
                temp_control.SetLableValue("商务意见", jsdata3.GetStringByProperty("business")); 
                temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this, false, false, true);
                temp_control.SetLableValue("机务意见", jsdata3.GetStringByProperty("maintenance"));
                temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this, false, false, true);
                temp_control.SetLableValue("调度意见", jsdata3.GetStringByProperty("dispatch"));
                temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this, false, false, true);
                temp_control.SetLableValue("海务意见", jsdata3.GetStringByProperty("sea"));
                temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this, false, false, true);
                temp_control.SetLableValue("人事意见", jsdata3.GetStringByProperty("personnel"));
                temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this, false, false, true);
                temp_control.SetLableValue("调度经理意见", jsdata3.GetStringByProperty("manager"));
                temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                linearlayout.AddView(temp_control, layoutparams);
               
                #endregion
            }
            else if (acvitityType.Equals("dockport"))
            {
                #region 港口码头
                JsonValue datavalue = jsRoot["data"];
                if (datavalue.JsonType == JsonType.Array)
                {
                    JsonArray dataarray = (JsonArray)datavalue;
                    foreach (var item in dataarray)
                    {
                        JsonObject item_object= ((JsonObject)item);

                        temp_control = new lineartextview_customcontrl(this, true);
                        temp_control.SetLableValue(item_object.GetStringByProperty("name"), item_object.GetStringByProperty("port"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("船舶代理联系人", item_object.GetStringByProperty("linkman"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("公司", item_object.GetStringByProperty("company"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("电话", item_object.GetStringByProperty("phone"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("固话", item_object.GetStringByProperty("tel"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("传真", item_object.GetStringByProperty("fax"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("邮箱", item_object.GetStringByProperty("mail"));
                        linearlayout.AddView(temp_control, layoutparams);
                    }
                }
                #endregion
            }
            else if (acvitityType.Equals("gas"))
            {
                #region 耗油

                JsonObject jsdata = (JsonObject)jsRoot["data"];
                int total = 0;
                int.TryParse(jsdata.GetStringByProperty("totalNumber"), out total);
                string totalOutlay = jsdata.GetStringByProperty("totalOutlay");
                for (int i = 0; i < total; i++)
                {
                    JsonValue json_gas = jsdata[i.ToString()];
                    if (json_gas.JsonType == JsonType.Array)
                    {
                        temp_control = new lineartextview_customcontrl(this, true);
                        temp_control.SetLableValue("油品", json_gas[0]);
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("油耗", json_gas[1]);
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("单价", json_gas[2]);
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("单位", json_gas[3]);
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("开支（元）", json_gas[4]);
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this, false, true);
                        temp_control.SetLableValue("所有油品合计（元）", totalOutlay);
                        linearlayout.AddView(temp_control, layoutparams);
                    }
                }
                #endregion
            }
            else if (acvitityType.Equals("current"))
            {
                #region 当前情况

                JsonObject jsdata = (JsonObject)jsRoot["data"];
                int total = 0;
                int.TryParse(jsdata.GetStringByProperty("totalNumber"), out total);

                for (int i = 0; i < total; i++)
                {
                    JsonValue jsdynamic = jsdata[i.ToString()];
                    if (jsdynamic.JsonType == JsonType.Array)
                    {
                        JsonArray jsdynamicarray = (JsonArray)jsdynamic;

                        temp_control = new lineartextview_customcontrl(this, true);
                        temp_control.SetLableValue("序号" + jsdynamicarray[0], string.Empty);
                        linearlayout.AddView(temp_control, layoutparams);


                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("日期", jsdynamicarray[1]);
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this, false, false, true);
                        temp_control.SetLableValue("情况", jsdynamicarray[2]);
                        temp_control.tv_value.MovementMethod = Android.Text.Method.ScrollingMovementMethod.Instance;
                        linearlayout.AddView(temp_control, layoutparams);
                    }
                }


                #endregion
            }
            else if (acvitityType.Equals("expense"))
            {
                #region 船员费用

                #region 船员费用

                JsonObject jsdata1 = (JsonObject)jsRoot["data1"];

                temp_control = new lineartextview_customcontrl(this, true);
                temp_control.SetLableValue("船员费用", string.Empty);
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("船员离船工资", jsdata1.GetStringByProperty("disembark"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("船员离船交通费", jsdata1.GetStringByProperty("carfare"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("上船等船工资", jsdata1.GetStringByProperty("boarding"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("船员住宿补贴", jsdata1.GetStringByProperty("subsidy"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("客餐费", jsdata1.GetStringByProperty("mealfee"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("办证换证费用", jsdata1.GetStringByProperty("card"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("工伤治疗费", jsdata1.GetStringByProperty("treat"));
                linearlayout.AddView(temp_control, layoutparams);

                #endregion

                #region 项目开支

                JsonObject jsdata2 = (JsonObject)jsRoot["data2"];
                temp_control = new lineartextview_customcontrl(this, true);
                temp_control.SetLableValue("项目开支(元)", string.Empty);
                linearlayout.AddView(temp_control, layoutparams);

                JsonValue jsdynamic = jsdata2["0"];

                if (jsdynamic.JsonType == JsonType.Array)
                {
                    JsonArray jsdynamicarray = (JsonArray)jsdynamic;

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("伙食", jsdynamicarray[0]);
                    linearlayout.AddView(temp_control, layoutparams);

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("工资", jsdynamicarray[1]);
                    linearlayout.AddView(temp_control, layoutparams);

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("差旅费及证件", jsdynamicarray[2]);
                    linearlayout.AddView(temp_control, layoutparams);

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("洗舱费", jsdynamicarray[3]);
                    linearlayout.AddView(temp_control, layoutparams);

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("维修费", jsdynamicarray[4]);
                    linearlayout.AddView(temp_control, layoutparams);

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("检验费", jsdynamicarray[5]);
                    linearlayout.AddView(temp_control, layoutparams);

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("备件及材物料", jsdynamicarray[6]);
                    linearlayout.AddView(temp_control, layoutparams);

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("船舶及船员保险", jsdynamicarray[7]);
                    linearlayout.AddView(temp_control, layoutparams);

                    temp_control = new lineartextview_customcontrl(this);
                    temp_control.SetLableValue("合计", jsdynamicarray[8]);
                    linearlayout.AddView(temp_control, layoutparams);

                }

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("本次航线合计(元)", jsdata2.GetStringByProperty("cost"));
                linearlayout.AddView(temp_control, layoutparams);

                #endregion


                #endregion
            }
            else if (acvitityType.Equals("projectFollow"))
            {
                #region 项目跟进人

                JsonObject jsdata = (JsonObject)jsRoot["data"];

                JsonValue jsdynamic = jsdata["dynamic"];
                if (jsdynamic.JsonType == JsonType.Array)
                {
                    JsonArray jsdynamicarray = (JsonArray)jsdynamic;
                    string type, typevalue = string.Empty, temp_isdone = "true";
                    foreach (var item in jsdynamicarray)
                    {
                        JsonObject temp_object = (JsonObject)item;
                        type = temp_object.GetStringByProperty("type");
                        switch (type)
                        {
                            case "jiwu": typevalue = "机务"; temp_isdone = Intent.GetStringExtra("IsJiWu");
                                break;
                            case "haiwu": typevalue = "海务"; temp_isdone = Intent.GetStringExtra("IsHaiWu");
                                break;
                            case "renshi": typevalue = "人事"; temp_isdone = Intent.GetStringExtra("IsRenShi");
                                break;
                            case "diaodu": typevalue = "调度"; temp_isdone = Intent.GetStringExtra("IsDiaoDu");
                                break;
                            case "shangwu": typevalue = "商务"; temp_isdone = Intent.GetStringExtra("IsShangWu");
                                break;
                            default:
                                continue;
                        }

                        temp_control = new lineartextview_customcontrl(this, true);
                        temp_control.SetLableValue(typevalue, string.Empty);
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("负责人", temp_object.GetStringByProperty("name"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("短号", temp_object.GetStringByProperty("duanhao"));
                        linearlayout.AddView(temp_control, layoutparams);
                        temp_control = new lineartextview_customcontrl(this); 
                        temp_control.SetLableValue("状态", temp_isdone.Equals("true") ? "已完成" : "未完成");
                        if (temp_isdone.Equals("true"))
                            temp_control.tv_value.SetTextColor(Android.Graphics.Color.Green);
                        else
                            temp_control.tv_value.SetTextColor(Android.Graphics.Color.Red);
                        linearlayout.AddView(temp_control, layoutparams);


                    }
                }
                #endregion
            }
            else if (acvitityType.Equals("portsetvice"))
            {
                #region 港口耗时
                JsonObject jsdata = (JsonObject)jsRoot["data"];

                JsonValue jsdynamic = jsdata["dynamic"];
                if (jsdynamic.JsonType == JsonType.Array)
                {
                    JsonArray jsdynamicarray = (JsonArray)jsdynamic;
                    foreach (var item in jsdynamicarray)
                    {
                        JsonObject temp_object = (JsonObject)item;
                        temp_control = new lineartextview_customcontrl(this, true);
                        temp_control.SetLableValue("港口及代理费", string.Empty);
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("港口", temp_object.GetStringByProperty("port"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("码头", temp_object.GetStringByProperty("dock"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("原因", temp_object.GetStringByProperty("reson"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("该码头总耗时", temp_object.GetStringByProperty("docktime"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("船舶代理", temp_object.GetStringByProperty("agent"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("港口及代理费（元）", temp_object.GetStringByProperty("portagentcost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("航行时间", temp_object.GetStringByProperty("seawaytime"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("等待时间", temp_object.GetStringByProperty("waittime"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("作业耗时", temp_object.GetStringByProperty("bustime"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("滞港时间", temp_object.GetStringByProperty("stoptime"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("作业速率", temp_object.GetStringByProperty("rate"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("上次离港时间", temp_object.GetStringByProperty("lastleavetime"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("到锚地时间", temp_object.GetStringByProperty("anchortime1"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("起锚进港时间", temp_object.GetStringByProperty("anchortime2"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("靠泊时间", temp_object.GetStringByProperty("anchortime3"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("作业开始时间", temp_object.GetStringByProperty("bustimestart"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("作业完成时间", temp_object.GetStringByProperty("bustimeend"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("岸量", temp_object.GetStringByProperty("coastnumber"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("船板量", temp_object.GetStringByProperty("decknumber"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("比重", temp_object.GetStringByProperty("proportion"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("体积", temp_object.GetStringByProperty("bulk"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("舱容", temp_object.GetStringByProperty("holdcapacity"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("利用率%", temp_object.GetStringByProperty("useratio"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("安检费", temp_object.GetStringByProperty("securitycheck"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("船代代理费", temp_object.GetStringByProperty("agentcost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("引航费", temp_object.GetStringByProperty("pilotcost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("清污协议费", temp_object.GetStringByProperty("clearcost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("垃圾费", temp_object.GetStringByProperty("wastecost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("服务费", temp_object.GetStringByProperty("servercost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("港务费", temp_object.GetStringByProperty("portcost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("围油栏费", temp_object.GetStringByProperty("oilfencecost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("污水处理费用", temp_object.GetStringByProperty("sewagecost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("解缆费", temp_object.GetStringByProperty("cablecost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("维修费", temp_object.GetStringByProperty("maintaincost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("交通费", temp_object.GetStringByProperty("trafficcost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("进港拖轮费", temp_object.GetStringByProperty("pullincost"));
                        linearlayout.AddView(temp_control, layoutparams);

                        temp_control = new lineartextview_customcontrl(this);
                        temp_control.SetLableValue("出港拖轮费", temp_object.GetStringByProperty("pullcoutcost"));
                        linearlayout.AddView(temp_control, layoutparams);
                    }

                }
                #endregion
            }
            else if (acvitityType.Equals("totalcost"))
            {
                #region 费用统计
                JsonObject jsdata = (JsonObject)jsRoot["data"];

                temp_control = new lineartextview_customcontrl(this, true);
                temp_control.SetLableValue("运费"+"  "+jsdata.GetStringByProperty("freight")+"￥",string.Empty );
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("货量", jsdata.GetStringByProperty("goodsquantity"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("运费", jsdata.GetStringByProperty("freightunit"));
                //temp_control.SetLineViewLayoutParams(margin_bottom_ten);
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this, true);
                temp_control.SetLableValue("支出" + "  " + jsdata.GetStringByProperty("spending") + "￥", string.Empty);
                linearlayout.AddView(temp_control, layoutparams);


                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("港口及代理费用", jsdata.GetStringByProperty("portsagencyfee"));
                linearlayout.AddView(temp_control, layoutparams);


                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("燃油费", jsdata.GetStringByProperty("fuelsurcharge"));
                linearlayout.AddView(temp_control, layoutparams);


                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("招待费", jsdata.GetStringByProperty("entertainment"));
                linearlayout.AddView(temp_control, layoutparams);


                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("船员费用", jsdata.GetStringByProperty("crewfee"));
                linearlayout.AddView(temp_control, layoutparams);


                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("项目开支", jsdata.GetStringByProperty("projectcost"));

                //temp_control.SetLineViewLayoutParams(margin_bottom_ten);

                linearlayout.AddView(temp_control, layoutparams);


                temp_control = new lineartextview_customcontrl(this, true);
                temp_control.SetLableValue("利润" + "  " + jsdata.GetStringByProperty("profits") + "￥", string.Empty);
                linearlayout.AddView(temp_control, layoutparams);


                #endregion
            }
            else
            {
                #region owner

                JsonObject jsdata1 = (JsonObject)jsRoot["data1"];
                #region 基本信息

                temp_control = new lineartextview_customcontrl(this, true);
                temp_control.SetLableValue("基本信息", string.Empty);
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("货主", jsdata1.GetStringByProperty("ownerofcargo"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("货代", jsdata1.GetStringByProperty("freightforward"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("货种", jsdata1.GetStringByProperty("type1"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("合同受载期", jsdata1.GetStringByProperty("time1"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("合同正负值", jsdata1.GetStringByProperty("value"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("申报货种", jsdata1.GetStringByProperty("type2"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("合同损耗率", jsdata1.GetStringByProperty("rate"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("交接方式", jsdata1.GetStringByProperty("way"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("计划货量", jsdata1.GetStringByProperty("ownerofcargo"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("滞欺费", jsdata1.GetStringByProperty("charge"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("计划货量", jsdata1.GetStringByProperty("plannumber"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("运价", jsdata1.GetStringByProperty("price"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("币种", jsdata1.GetStringByProperty("currency"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("汇率", jsdata1.GetStringByProperty("exchangerate"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("是否开票", jsdata1.GetStringByProperty("ticket"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("计划佣金", jsdata1.GetStringByProperty("planbrokerage"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("预计费用收入总计", jsdata1.GetStringByProperty("planincome"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("中介费", jsdata1.GetStringByProperty("agencyfee1"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("中介费给", jsdata1.GetStringByProperty("agencyfee2"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("实际货量", jsdata1.GetStringByProperty("realnumber"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("实际佣金", jsdata1.GetStringByProperty("realbrokerage"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("利息", jsdata1.GetStringByProperty("interest"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("折旧", jsdata1.GetStringByProperty("depreciation"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("运费收入总计", jsdata1.GetStringByProperty("realincome"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("成本合计", jsdata1.GetStringByProperty("cost"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("毛利", jsdata1.GetStringByProperty("grossmargin"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this, false, true);
                temp_control.SetLableValue("利润", jsdata1.GetStringByProperty("profit"));
                linearlayout.AddView(temp_control, layoutparams);

                #endregion

                #region 货主要求
                JsonObject jsdata2 = (JsonObject)jsRoot["data2"];

                temp_control = new lineartextview_customcontrl(this, true);
                temp_control.SetLableValue("货主要求", string.Empty);
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("舱壁试验", jsdata2.GetStringByProperty("experiment"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("蒸舱", jsdata2.GetStringByProperty("steam"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("加温", jsdata2.GetStringByProperty("warming"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("要求航线温度", jsdata2.GetStringByProperty("routetemperature"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("要求卸货温度", jsdata2.GetStringByProperty("unloadtemperature"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("航次加温费用", jsdata2.GetStringByProperty("warmingcost"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("洗舱", jsdata2.GetStringByProperty("wash"));
                linearlayout.AddView(temp_control, layoutparams);
                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("洗舱时间", jsdata2.GetStringByProperty("washtime"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("上次航次货种", jsdata2.GetStringByProperty("type"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("开始洗舱时间", jsdata2.GetStringByProperty("starttime"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("结束洗舱时间", jsdata2.GetStringByProperty("endtime"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("洗舱用时", jsdata2.GetStringByProperty("totaltime"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("谁要求商检", jsdata2.GetStringByProperty("who"));

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("商检公司", jsdata2.GetStringByProperty("company"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("支付商检费用", jsdata2.GetStringByProperty("paycheckcost"));
                linearlayout.AddView(temp_control, layoutparams);

                temp_control = new lineartextview_customcontrl(this);
                temp_control.SetLableValue("商检费用", jsdata2.GetStringByProperty("checkcost"));
                linearlayout.AddView(temp_control, layoutparams);

                #endregion
                #endregion
            }
        }

        private lineartextview_customcontrl CreatAddTextview(string lable, string vlaue,bool istitle=false) 
        {
            ViewGroup.LayoutParams layoutparams = new ViewGroup.LayoutParams(WindowManagerLayoutParams.FillParent, WindowManagerLayoutParams.WrapContent);
            lineartextview_customcontrl temp_control = new lineartextview_customcontrl(this, istitle);
            temp_control.SetLableValue(lable, vlaue);
            linearlayout.AddView(temp_control, layoutparams);
            return temp_control;
        }

       

    }
}
