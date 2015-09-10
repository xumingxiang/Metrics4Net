using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Metrics.Web.Api
{
    /// <summary>
    /// Query 的摘要说明
    /// </summary>
    public class Query : IHttpHandler
    {

        string InfluxdbConnectionString = System.Configuration.ConfigurationManager.AppSettings["InfluxdbConnectionString"];

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string queryCmd = context.Request["cmd"];

            string queryUrl = InfluxdbConnectionString + "&q=" + queryCmd;

            string resp = Metrics.Utils.HttpHelper.HttpGet(queryUrl);

            context.Response.Write(resp);
        }



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}