using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Views;
using System.Json;
using shipmonitor.entity;
using System.Reflection;
using Android.OS;
using Android.Widget;

namespace shipmonitor
{

    public static class common
    {

        public const string app_access = "app_access";
        public const string app_userID = "app_userID";

        /// <summary>
        /// 链接超时，给主线程发送消息
        /// </summary>
        /// <param name="handler"></param>
        public static void ServiceTimeOut(Handler handler)
        {
            Message message = new Message();
            message.What = 1;
            Bundle bundle = new Bundle();
            bundle.PutString("errormessage", "访问远程服务失败！");
            message.Data = bundle;
            handler.SendMessage(message);
        }
        /// <summary>
        /// 设置acvitity的actionbar
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="avtionBarLayoutid"></param>
        /// <returns>返回 action 的view 对象，可以自己定义事件处理</returns>
        public static View setActionBar(Activity activity, int avtionBarLayoutid)
        {
            if (activity != null && activity.ActionBar != null)
            {
                activity.ActionBar.SetDisplayShowHomeEnabled(false);
                activity.ActionBar.SetDisplayShowCustomEnabled(true);
                LayoutInflater inflator = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
                View v = inflator.Inflate(avtionBarLayoutid, null);
                ActionBar.LayoutParams layout = new ActionBar.LayoutParams(WindowManagerLayoutParams.FillParent, WindowManagerLayoutParams.FillParent);
                activity.ActionBar.SetCustomView(v, layout);
                return v;
            }
            return null;
        }



        public static T GetControlById<T>(this Activity activity,int controlId) where T : View
        {
            return activity.FindViewById<T>(controlId); 
        }

        /// <summary>
        /// 去除前面前一位和后一位  清除json 解析遗留的 "" 号
        ///  如果长度不够2位，返回原字符串。
        /// </summary>
        /// <returns></returns>
        public static string TrimRisk(this string str) 
        {
            if (str.Length < 2)
                return str;
			if (!str.Contains ("\""))
				return str;
            str = str.Remove(0, 1);
            str = str.Remove(str.Length - 1, 1);
            return str;
        }



        /// <summary>
        /// 解析头
        /// </summary>
        /// <param name="jsRoot"></param>
        /// <returns></returns>
        public static BaseHttpResponseHead JsonParseBaseHTTPHead(JsonObject jsRoot)
        {
            //JsonObject jsRoot =(JsonObject)JsonObject.Parse(jsonStr);
            BaseHttpResponseHead head= createBaseHttpResponseHead();
            head.ErrorCode = jsRoot[head.ErrorCodeStr].ToString().TrimRisk();
            head.ErrorMessage = jsRoot[head.ErrorMessageStr].ToString().TrimRisk();
            head.Success = jsRoot[head.SuccessStr].ToString().TrimRisk();
            /* 
            Type headType= typeof(BaseHttpResponseHead);
            PropertyInfo[] headPropInofs = headType.GetProperties();
            for (int i = 0; i < headPropInofs.Length; i++)
            {
                string temp_proplowername = headPropInofs[i].Name.ToLower();
                headPropInofs[i].SetValue(head, jsRoot[i].ToString().TrimRisk(), null);
            }*/
            return head;
        }


        public static string GetStringByProperty(this JsonObject jsonobject,string propName) 
		{
			if (!jsonobject.Keys.Contains(propName)||jsonobject[propName] == null  )
				return string.Empty;
            return jsonobject[propName].ToString().TrimRisk();
        }
        public static string GetStringByProperty(this JsonObject jsonobject, int index=0)
        {
            if (index >= jsonobject.Keys.Count) 
                return string.Empty;
            return jsonobject[index].ToString().TrimRisk();
        }


        private static object Factory(Type type) 
        {
            switch (type.Name)
            {
                case "BaseHttpResponseHead":
                    return createBaseHttpResponseHead();
                default:
                    return new baseEntity();
            }
        }

        private static BaseHttpResponseHead createBaseHttpResponseHead() 
        {
            return new BaseHttpResponseHead();
        }

    }


    public static class dbservice
    {
        /// <summary>
        /// 登陆
        /// 
        /// {
        ///    "name"  : "name",                 //用户名
        ///    "password"  : "password",         //密码
        ///   "deviceType"   : "deviceType" ,   //iOS\Android\PC
        ///    "deviceID"   : "deviceID" 	,	   //设备ID
        ///    "isMobil":"yes"
        ///}
        ///返回数据:
        ///{
        ///"success":"true/false",                  //返回状态
        ///"errorMessage":"message",
        ///"errorCode":"code",
        ///"data": {
        ///   "status"  : "true/false"    ,    // 登录是否成功
        ///   "access"  :  "1/2/3"   ,  		// 进入模块权限
        ///   "userID":"userID"      ,  //  用户ID
        ///   }
        ///}
        /// </summary>
        public const string URL_SIGN = "http://192.168.1.13:8080/hdwiki/index.php?user-hdnewmovelogin-{0}-{1}-Android-{2}-YES";

        /// <summary>
        /// 请求数据:
        ///{
        ///"userID"  : "userID"  {0} ,   //用户id
        ///"pageNo"  : "pageNo"  {1},
        ///"pageSize"  : "pageSize" {2} ,
        ///"isMobil":"true/false"
        ///"dataSort":"asc/desc"  {3}// （传参时）默认是时间倒序
        /// </summary>
        public const string URL_ALLFLIGHT = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_new_costlist_for_move-{0}-{1}-{2}-yes-{3}";
      


        /// <summary>
        /// 获取留言  0：articleID，1：pageNo ，2：pageSize，3：dataSort
        /// </summary>
        public const string URL_GETlEAVEMESSAGE = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_messages-{0}-{1}-{2}-yes-{3}";
        /// <summary>
        /// 提交留言  0：useid，1：articleID ，2：message
        /// </summary>
        public const string URL_SENDlEAVEMESSAGE = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdsend_message-{0}-{1}-{2}-yes";
    

        /// <summary>  
        /// 获取基本信息
        /// suerid
        /// "articleID":"articleID",
	    ///"type": "7",
	    ///"isMobil": "true/false"
        /// </summary>
		public const string URL_GETBASEINFO = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_cost_for_move-{0}-{1}-7-yes";

        /// <summary>
        /// 获取港口码头信息
        /// userid
        /// "articleID":"articleID",
        ///"type": "7",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETDOCKLIST= "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_voyage_prot_for_move-{0}-{1}-7-yes";

        /// <summary>
        /// 获取货主信息
        /// userid
        /// "articleID":"articleID",
        ///"type": "7",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETOWERINFO = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_client_for_move-{0}-{1}-7-yes";

        /// <summary>
        /// 招待及其它费用
        /// userid
        /// "articleID":"articleID",
        ///"type": "7",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETSERVINGCOST = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_entertain-{0}-171-7-yes";

        /// <summary>
        /// 港口耗时及其它费用
        /// userid
        /// "articleID":"articleID",
        ///"type": "7",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETPROTSERVING = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_port_for_move-{0}-{1}-7-yes";

        /// <summary>
        /// 船员费用/项目开支合计
        /// userid
        /// "articleID":"articleID",
        ///"type": "7",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETEXPENSE = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_other_charges-{0}-{1}-7-yes";

        /// <summary>
        /// 当前情况
        /// userid
        /// "articleID":"articleID",
        ///"type": "7",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETCURRENT = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_case-{0}-{1}-7-yes";


        /// <summary>
        /// 燃油费用
        /// userid
        /// "articleID":"articleID",
        ///"type": "7",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETGAS = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_gas-{0}-{1}-7-yes";


        /// <summary>
        /// 跟进人请求
        /// userid
        /// "articleID":"articleID",
        ///"type": "7",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETPROJECTFOLLW = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdget_ship_managers-{0}-{1}-yes";

        /// <summary>
        /// 费用统计
        /// userid
        /// "articleID":"articleID",
        ///"isMobil": "true/false"
        /// </summary>
        public const string URL_GETTOTALCOST = "http://192.168.1.13:8080/hdwiki/index.php?move_cost-hdcount_cost-{0}-{1}-yes";


    }
}
