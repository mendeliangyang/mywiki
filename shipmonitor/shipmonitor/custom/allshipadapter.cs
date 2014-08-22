using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Widget;
using shipmonitor.entity;
using Android.Views;
using Android.App;

namespace shipmonitor.custom
{
    public class allshipadapter : BaseAdapter<flightnumberEntity>
    {

        public allshipadapter(Activity parent,List<flightnumberEntity> listdata) 
            :base()
        {
            activity = parent;
            if (listdata == null)
                flightNumberData = new List<flightnumberEntity>();
            flightNumberData = listdata;
        }

        private List<flightnumberEntity> flightNumberData;

        private Activity activity;


        public override flightnumberEntity this[int position]
        {
            get { return flightNumberData[position]; }
        }

        public override int Count
        {
            get { return flightNumberData.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            View view = convertView;
            flightnumberEntity item= flightNumberData[position];
            if (view==null)
            {
                view = activity.LayoutInflater.Inflate(Resource.Layout.allshiplistview, null);
            }

            ((TextView)view.FindViewById(Resource.Id.tv_allshiplistview_title)).Text = item.Title1;
            ((TextView)view.FindViewById(Resource.Id.tv_allshiplistview_number)).Text = item.Title2;
            ((TextView)view.FindViewById(Resource.Id.tv_allshiplistview_time)).Text = item.Ports;

            if (item.IsRead.Equals("false"))
            {
                ((Button)view.FindViewById(Resource.Id.btn_allshiplistview_icn)).SetBackgroundResource(Resource.Drawable.allshiplistview_round_yellow);
            }

            if (!item.IsWanCheng.Equals("true"))
            {
                ((LinearLayout)view.FindViewById(Resource.Id.layout_allship_iscomplete)).Visibility = ViewStates.Visible;
                ((LinearLayout)view.FindViewById(Resource.Id.layout_allship_profit)).Visibility = ViewStates.Gone;

                ((TextView)view.FindViewById(Resource.Id.tv_allship_status)).Text = "航次表未完成";
                if (item.IsShangWu.Equals("true"))
                    ((TextView)view.FindViewById(Resource.Id.tv_allship_shangwu)).SetTextColor(Android.Graphics.Color.GreenYellow);
                if (item.IsDiaoDu.Equals("true"))
                    ((TextView)view.FindViewById(Resource.Id.tv_allship_diaodu)).SetTextColor(Android.Graphics.Color.GreenYellow);
                if (item.IsHaiWu.Equals("true"))
                    ((TextView)view.FindViewById(Resource.Id.tv_allship_haiwu)).SetTextColor(Android.Graphics.Color.GreenYellow);
                if (item.IsJiWu.Equals("true"))
                    ((TextView)view.FindViewById(Resource.Id.tv_allship_jiwu)).SetTextColor(Android.Graphics.Color.GreenYellow);
                if (item.IsRenShi.Equals("true"))
                    ((TextView)view.FindViewById(Resource.Id.tv_allship_renshi)).SetTextColor(Android.Graphics.Color.GreenYellow);
                if (item.IsZhongJingLi.Equals("true"))
                    ((TextView)view.FindViewById(Resource.Id.tv_allship_zongjingli)).SetTextColor(Android.Graphics.Color.GreenYellow);
            }
            else
            {
                
                ((LinearLayout)view.FindViewById(Resource.Id.layout_allship_iscomplete)).Visibility = ViewStates.Gone;
                ((LinearLayout)view.FindViewById(Resource.Id.layout_allship_profit)).Visibility = ViewStates.Visible;
			
                ((TextView)view.FindViewById(Resource.Id.tv_allship_profit)).Text = item.Profit;
                ((TextView)view.FindViewById(Resource.Id.tv_allship_income)).Text = item.Freightrevenue;
                ((TextView)view.FindViewById(Resource.Id.tv_allship_outlay)).Text = item.Expend;

            }
            return view;
        }
    }


}
