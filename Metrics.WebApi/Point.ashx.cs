using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Metrics.WebApi
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
                int value = Convert.ToInt32(context.Request["value"]);
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
                context.Response.Write("success");
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