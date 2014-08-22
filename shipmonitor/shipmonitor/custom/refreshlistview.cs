using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Widget;
using Android.Views;
using Android.Content;
using Android.Util;

namespace shipmonitor.custom
{
    public class refreshlistview : ListView ,Android.Widget.AbsListView.IOnScrollListener
    {


        /// <summary>
        /// 刷新中
        /// </summary>
        private const int Refreshing = 1;

        /// <summary>
        /// 刷新完成
        /// </summary>
        private const int Refresh_Done = 2;

        /// <summary>
        /// 当前位置  第一个 顶端
        /// </summary>
        private const int ItemFirstPosition=1;

        /// <summary>
        /// 当前位置  低端
        /// </summary>
        private const int ItemBottomPosition = 2;

        //布局
        private LayoutInflater inflater;

        /// <summary>
        /// 起始Y坐标
        /// </summary>
        private int startY;
        /// <summary>
        /// 结束Y坐标
        /// </summary>
        private int endY;
        /// <summary>
        ///  保证在move中判断手势是保持 startY的准确
        /// </summary>
        private bool isRecord=false;

        /// <summary>
        /// 当前状态
        /// </summary>
        private int currentStatus=Refresh_Done;

        /// <summary>
        /// 当前位置
        /// </summary>
        private int currentPosition;

        //回掉
        private IOnRefreshListener refresh;

        #region init
        
        public refreshlistview(Context context):base(context)
        {

            SetOnScrollListener(this);
        }

        public refreshlistview(Context context,IAttributeSet attrs):base(context,attrs)
        {

            SetOnScrollListener(this);
        }
        #endregion

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            if (firstVisibleItem == 0)
            {
                currentPosition = ItemFirstPosition;
            }
            if (visibleItemCount + firstVisibleItem == totalItemCount)
            {
                currentPosition = ItemBottomPosition;
            }
            
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Cancel:

                    break;
                    //如果第一次按下，记录按下的位置
                case MotionEventActions.Down:
                    if (!isRecord)
                    {
                        isRecord = true;
                        startY = (int)e.GetY();
                    }
                    break;
                case MotionEventActions.Move:
                    break;
                case MotionEventActions.Up:
                    if (currentStatus == Refreshing || currentPosition!=ItemBottomPosition )
                        break;
                    if (isRecord)
                    {
                        endY = (int)e.GetY();
						if (startY-endY>0)
                        {
                            currentStatus = Refreshing;
                            onLvRefresh();
                        }
                        isRecord = false;
                    }
                    break;
                default:
                    break;
            }



            return base.OnTouchEvent(e);
        }

        #region event
        

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
			switch (scrollState) {
				case ScrollState.TouchScroll:
                    Log.Debug("-------------------> TouchScroll: ",
				      "SCROLL_STATE_TOUCH_SCROLL");
				break;

                case ScrollState.Fling:
                Log.Debug("------------------->Fling ",
				      "SCROLL_STATE_FLING");
				break;

                case ScrollState.Idle:
                Log.Debug("-------------------> Idle ",
				      "SCROLL_STATE_IDLE");
				break;

			}
        }

        #endregion

        public void SetRefresh(IOnRefreshListener param_refresh) 
        {
            refresh = param_refresh;
        }

        public void OnRefreshComplete()
        {
            currentStatus = Refresh_Done;
        }

        private void onLvRefresh()
        {
            if (refresh != null)
            {
                refresh.onRefresh();
            }
        }


    }

    public interface IOnRefreshListener
    {
        void onRefresh();
    }
}
