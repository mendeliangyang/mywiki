using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shipmonitor.entity;
using Android.Widget;
using Android.Views;
using Android.App;

namespace shipmonitor.custom
{
   public class dockcustomadapter : BaseAdapter<DockInfoEntity>
    {
         List<DockInfoEntity> listData;
         Activity activity;
         public dockcustomadapter(Activity context,List<DockInfoEntity> data):base() 
         {
             this.activity = context;
             this.listData = data;
         }
        public override DockInfoEntity this[int position]
        {
            get { return listData[position]; }
        }

        public override int Count
        {
            get { return listData.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            View view = convertView;
            if (view==null)
            {
                view=activity.LayoutInflater.Inflate(Resource.Layout.docklistviewitem, null);
            }
            DockInfoEntity item = listData[position];
            view.FindViewById<TextView>(Resource.Id.docklistitem_tv_name).Text = item.name;
            view.FindViewById<TextView>(Resource.Id.docklistitem_tv_port).Text = item.port;
            view.FindViewById<TextView>(Resource.Id.docklistitem_tv_linkman).Text = item.linkman;
            view.FindViewById<TextView>(Resource.Id.docklistitem_tv_company).Text = item.company;
            view.FindViewById<TextView>(Resource.Id.docklistitem_tv_phone).Text = item.phone;
            view.FindViewById<TextView>(Resource.Id.docklistitem_tv_tel).Text = item.tel;
            view.FindViewById<TextView>(Resource.Id.docklistitem_tv_fax).Text = item.fax;
            view.FindViewById<TextView>(Resource.Id.docklistitem_tv_mail).Text = item.mail;
            return view;
        }
    }
}
