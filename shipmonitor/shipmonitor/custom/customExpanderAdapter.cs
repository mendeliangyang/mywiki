using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Widget;
using System.Collections;
using Android.App;
using shipmonitor.entity;

namespace shipmonitor.custom
{
    public class customExpanderAdapter : BaseExpandableListAdapter 
    {
        private List<List<Dictionary<string,DockInfoEntity>>> childrens;
        private List<Dictionary<string,object>> groups;
        Activity activity;


        public customExpanderAdapter(Activity context, List<Dictionary<string, object>> group, List<List<Dictionary<string, PageEntity>>> children) 
        {
            this.activity = context;
            this.groups = group;
            //this.childrens = children;
        }




    //    private List<List<Map<String, Object>>> children;
    //    private List<Map<String, Object>> group;
    //  private String[] childFrom, groupFrom;
    //    private int[] childTo, groupTo;
    //    private int clayout, glayout;
    //   private LayoutInflater inflater;
	


        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            //return (Object)childrens[groupPosition][childPosition];
            return null;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override Android.Views.View GetChildView(int groupPosition, int childPosition, bool isLastChild, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            throw new NotImplementedException();
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return childrens[groupPosition].Count;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            //return (Java.Lang.Object)groups[groupPosition];
            return null;
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override Android.Views.View GetGroupView(int groupPosition, bool isExpanded, Android.Views.View convertView, Android.Views.ViewGroup parent)
        {
            throw new NotImplementedException();
        }

        public override int GroupCount
        {
            get { return groups.Count; }
        }

        public override bool HasStableIds
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            throw new NotImplementedException();
        }
    }
}
