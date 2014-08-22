using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace shipmonitor
{
    public class WebRequestHelp
    {
       public delegate void HandleResponse(string result);

        HandleResponse handleResponse = null;

        public WebRequestHelp(string url, HandleResponse handle)
        {
            handleResponse = handle;
            HttpRequest(url);
        }

        private void HttpRequest(string url)
        {
            HttpWebRequest webreq = (HttpWebRequest)HttpWebRequest.Create(url);
            webreq.BeginGetResponse(new AsyncCallback(ReadWebResponse), webreq);
        }

        private void ReadWebResponse(IAsyncResult result)
        {
            HttpWebRequest webreq = (HttpWebRequest)result.AsyncState;
            try
            {

                HttpWebResponse httpResult = (HttpWebResponse)webreq.EndGetResponse(result);
                if (httpResult.StatusCode == HttpStatusCode.OK)
                {
                    string streamResult = new StreamReader(httpResult.GetResponseStream()).ReadToEnd();

                    if (handleResponse!=null)
                    {
                        handleResponse(streamResult);
                    }
                }
                else
                    //common.ServiceTimeOut(handler);
                    ;

            }
            catch (Exception)
            {
                //common.ServiceTimeOut(handler);
            }
        }
    }
}
