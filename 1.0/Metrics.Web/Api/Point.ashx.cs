using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Metrics.Web.Api
{
    /// <summary>
    /// Point 的摘要说明
    /// </summary>
    public class Point : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                context.Response.ContentType = "text/plain";

                string name = context.Request["name"];
                double value = Convert.ToDouble(context.Request["value"]);
                string tags_str = context.Request["tags"];

                var tags = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace(tags_str))
                {
                    var arr_tags = tags_str.Split('&');
                    for (int i = 0; i < arr_tags.Length; i++)
                    {
                        string[] tag = arr_tags[i].Split('=');
                        string tag_key = tag[0];
                        string tag_value = tag[1];
                        if (!string.IsNullOrWhiteSpace(tag_key))
                        {
                            tags[tag_key] = tag_value;
                        }
                    }
                }

                Metrics.Metric.Point(name, value, tags);
                context.Response.AddHeader("Access-Control-Allow-Origin", "*");//允许跨域请求
               // context.Response.AddHeader("Access-Control-Allow-Credentials", "true");//允许跨域操作cookie

                context.Response.Write("point success :" + context.Request.RawUrl);
            }
            catch (Exception ex)
            {
                context.Response.Write(ex);
            }
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