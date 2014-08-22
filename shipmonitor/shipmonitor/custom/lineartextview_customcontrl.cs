using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Widget;
using Android.App;
using Android.Util;
using Android.Views;

namespace shipmonitor.custom
{
    public class lineartextview_customcontrl: LinearLayout
    {
        public TextView tv_value=null;
        public TextView tv_lable=null;
        LinearLayout layout_centext=null;
        LinearLayout layout_view=null;
        LinearLayout layout_parent=null;
		View linearView=null;
        Activity activity=null;
        public lineartextview_customcontrl(Activity context,IAttributeSet attrs):
            base(context, attrs)
        {
            init(context,attrs);
        }

        private void init(Activity context,IAttributeSet attrs,bool istwolinear=false) 
        {
            activity = context;
            LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Android.Content.Context.LayoutInflaterService);

            if (!istwolinear)
                inflater.Inflate(Resource.Layout.lineartextview_customcontrol, this);
            else
                inflater.Inflate(Resource.Layout.twoLinearTextview_custom, this);

            tv_lable = FindViewById<TextView>(Resource.Id.linear_ct_tv_lable);
            tv_value = FindViewById<TextView>(Resource.Id.linear_ct_tv_value);
        }

        public lineartextview_customcontrl(Activity context,bool isTable=false,bool ismarginTop=false,bool istwo=false) 
            :base(context)
		{
            init(context, null, istwo);

            layout_centext = FindViewById<LinearLayout>(Resource.Id.linear_ct_ly_context);
            layout_view = FindViewById<LinearLayout>(Resource.Id.linear_ct_ly_view);
            layout_parent = FindViewById<LinearLayout>(Resource.Id.linear_ct_ly_parent);

            if (isTable)
            {
                //tv_lable.SetTextAppearance(context, Resource.Style.style_detail_textview_lableTitle);
                //tv_value.SetTextAppearance(context, Resource.Style.style_detail_textview_lableTitle);

                Android.Content.Res.Resources resource = activity.BaseContext.Resources;
                Android.Content.Res.ColorStateList csl= resource.GetColorStateList(Resource.Color.lineartextview_titlelabletextcolor);

                tv_value.SetTextColor(csl);
                tv_lable.SetTextColor(csl);

				layout_centext.SetBackgroundResource(Resource.Color.lineartextview_layoutTitlebackground);
                layout_view.Visibility = ViewStates.Gone;

                LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent);
                lp.TopMargin = 7;
                lp.LeftMargin = 4;
                lp.RightMargin = 4;
                //layout_view.LayoutParameters = lp;
                layout_parent.LayoutParameters = lp;
            }
            //if (ismarginTop)
            //{
            //    LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent);
            //    lp.TopMargin = 70;
            //    //layout_view.LayoutParameters = lp;
            //    layout_parent.LayoutParameters = lp;
            //}
        }


        public void SetLableValue(string lable,string value) 
        {
            SetLable(lable);
            SetValue(value);
        }

        public void SetLable(string lable) 
        {
            tv_lable.Text = lable;
        }
        public void SetValue(string value) 
        {
            tv_value.Text = value;
        }

        public void GoneLineView() 
        {
            if (layout_view != null)
            {
                layout_view.Visibility = ViewStates.Gone;
            }
        }

        public void SetLineViewLayoutParams(LinearLayout.LayoutParams layoutparams) 
        {
            if (layout_parent != null)
            {
                layout_parent.LayoutParameters = layoutparams;
            }
        }

        public void SetStyle() 
        {
            
    //<item name="android:textColor">#ffffffff</item>
    //<item name="android:layout_margin">1dp</item>

        }
    }
}
