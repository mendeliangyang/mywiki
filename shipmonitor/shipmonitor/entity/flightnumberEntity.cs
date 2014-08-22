using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shipmonitor.entity
{

    public class baseEntity { }



    public class BaseShipDetail :baseEntity
    {
        public string settime { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string totaltime { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string port { get; set; }
        public string dock { get; set; }
        public string load { get; set; }
        public string unload { get; set; }
        public string loadtime { get; set; }
        public string unloadtime { get; set; }
        public string lasttime { get; set; }
        public string voyagetime { get; set; }
        public string totalvoyagetime { get; set; }
        public string cargoportcost { get; set; }
        public string emptyrange { get; set; }
        public string loadrange { get; set; }
        public string istime { get; set; }
        public string attention { get; set; }
        public ContactInfoEntity loadcontact { get; set; }
        public ContactInfoEntity unloadcontact { get; set; }
        public ContactInfoEntity contractcontact { get; set; }
        public ContactInfoEntity ownercontact { get; set; }
        public ContactInfoEntity proxycontact { get; set; }
        public string business { get; set; }
        public string maintenance { get; set; }
        public string dispatch { get; set; }
        public string sea { get; set; }
        public string personnel { get; set; }
        public string manager { get; set; } 
    }

    public class ContactInfoEntity : baseEntity 
    {
        public string contactName { get; set; }
        public string company { get; set; }
        public string phone { get; set; }
        public string telephone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
    }

    public class DockInfoEntity : baseEntity
    {
        public string name { get; set; }
        public string port { get; set; }
        public string linkman { get; set; }
        public string company { get; set; }
        public string phone { get; set; }
        public string tel { get; set; }
        public string fax { get; set; }
        public string mail { get; set; }
    }

    public class flightnumberEntity:baseEntity
    {
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string ArticleID { get; set; }
        public string Profit { get; set; }
        public string Expend { get; set; }
        public string Freightrevenue { get; set; }
        public string IsWanCheng { get; set; }
        public string Ports { get; set; }
        public string Time { get; set; }
        public string IsRead { get; set; }
        public string IsShangWu { get; set; }
        public string IsDiaoDu { get; set; }
        public string IsHaiWu { get; set; }
        public string IsJiWu { get; set; }
        public string IsRenShi { get; set; }
        public string IsZhongJingLi { get; set; }

    }

    public class BaseHttpResponseHead : baseEntity
    {
        public string Success { get; set; }
        public string SuccessStr { get { return "success"; } }
        public string ErrorMessage { get; set; }
        public string ErrorMessageStr { get { return "errorMessage"; } }
        public string ErrorCode { get; set; }
        public string ErrorCodeStr { get { return "errorCode"; } }
    }

    public class PageEntity : baseEntity 
    {
        public string PageSize_str { get; set; }
        public int PageSize_int { get; set; }
        public string PageCount_str { get; set; }
        public int PageCount_int { get; set; }
        public string PageNumber_str { get; set; }
        public int PageNumber_int { get; set; }
        public string Total_str { get; set; }
        public int Total_int { get; set; }

        public PageEntity() { }
        public PageEntity(string pageNumber,string pageSize,string total) 
        {
            this.PageNumber_str = PageNumber_str;
            this.PageSize_str = pageSize;
            this.Total_str = total;

            int pagenumber_temp_i,pageszie_temp_i,total_temp_i;
            if (int.TryParse(pageNumber, out pagenumber_temp_i) && int.TryParse(pageSize, out pageszie_temp_i) && int.TryParse(total, out total_temp_i))
                initPage(pagenumber_temp_i, pageszie_temp_i, total_temp_i);
            else
                Total_int = PageSize_int= PageNumber_int = -1;
        }

        public PageEntity(int pagenumber,int pagesize,int total)
        {
            this.PageNumber_str = pagenumber.ToString();
            this.PageSize_str = pagesize.ToString();
            this.Total_str = total.ToString();

            initPage(pagenumber,pagesize,total);
        }

        public PageEntity(string pageNumber, string pageSize)
        {
            this.PageNumber_str = PageNumber_str;
            this.PageSize_str = pageSize;
            int pagenumber_temp_i, pageszie_temp_i;
            if (int.TryParse(pageNumber, out pagenumber_temp_i) && int.TryParse(pageSize, out pageszie_temp_i))
                initPage(pagenumber_temp_i, pageszie_temp_i);
        }
            
        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <param name="total"></param>
        private void initPage(int pagenumber, int pagesize, int total) 
        {
            this.PageNumber_int = pagenumber;
            this.PageSize_int = pagesize;
            this.Total_int = total;
            RefreshPageCount();
        }
        private void initPage(int pagenumber, int pagesize) 
        {
            this.PageNumber_int = pagenumber;
            this.PageSize_int = pagesize;
        }
        /// <summary>
        /// 从新计算页数
        /// </summary>
        private void RefreshPageCount() 
        {
            this.PageCount_int = Total_int / PageSize_int;
            if (Total_int % PageSize_int != 0)
                PageCount_int++;
            this.PageCount_str = PageCount_int.ToString();
        }

        /// <summary>
        /// 自动计算下一页的页数 ，如果最后一页，pagenumber 不变
        /// </summary>
        public bool PageNext() 
        {

            if (PageNumber_int < PageCount_int)
            {
                PageNumber_int++;
                PageNumber_str=PageNumber_int.ToString();
                return true;
            }
			if (PageNumber_int==0) {
				PageNumber_int++;
				PageNumber_str=PageNumber_int.ToString();
                return true;
			}
            return false;
        }

        /// <summary>
        /// 设置总数 ，自动计算页数
        /// </summary>
        /// <param name="total"></param>
        public void SetTotal(int total) 
        {
            this.Total_int = total;
            this.Total_str = total.ToString();
            RefreshPageCount();
        }
        public void SetTotal(string total)
        {
            int total_temp_i;
            if(int.TryParse(total,out total_temp_i))
                SetTotal(total_temp_i);
        }
        
    }

    public class LeaveMessageEntity : baseEntity
    {
        public string userName { get; set; }
        public string userid { get; set; }
        public string time { get; set; }
        public string message { get; set; }
    }

}
