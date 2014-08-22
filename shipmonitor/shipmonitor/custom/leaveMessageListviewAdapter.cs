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
using shipmonitor.entity;

namespace shipmonitor
{
	class leaveMessageListviewAdapter :BaseAdapter<LeaveMessageEntity>
	{
        Activity activity;
        List<LeaveMessageEntity> listData;

        public leaveMessageListviewAdapter(Activity act,List<LeaveMessageEntity> data) :base()
        {
            activity = act;
            listData = data;
        }

        public override LeaveMessageEntity this[int position]
        {
            get { return listData[position]; }
        }

        public override int Count
        {
            get { return listData.Count(); }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
			if (listData.Count<=position) {
				return view;
			}
            LeaveMessageEntity item = listData[position];
            if (view == null)
            {
                view = activity.LayoutInflater.Inflate(Resource.Layout.leaveMessageListviewItem, null);
            }
            ((TextView)view.FindViewById(Resource.Id.tv_lmls_username)).Text = item.userName;
            ((TextView)view.FindViewById(Resource.Id.tv_lmls_submittime)).Text = item.time;
            ((TextView)view.FindViewById(Resource.Id.tv_lmls_leavemessage)).Text = item.message;
            return view;
        }
    }
}

