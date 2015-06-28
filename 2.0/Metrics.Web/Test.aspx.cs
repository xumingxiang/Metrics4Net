using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Metrics.WebApi
{
    public partial class Test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Dictionary<string, string> tag1 = new Dictionary<string, string>();
            tag1.Add("tag1", "bbb");

            Random ran = new Random();

            int RandKey = ran.Next(0, 200);

            Thread.Sleep(RandKey);

            Metric.Point("plu_test_request_count");

            Metric.Point("plu_test_request_count", tag1);

            Metric.Point("plu_test_request_count", 1, tag1);

            Metric.Point("plu_test_request_time", RandKey, tag1);

            watch.Stop();

            Response.Write(watch.ElapsedMilliseconds);
            Response.Write(Request.AnonymousID);
        }
    }
}