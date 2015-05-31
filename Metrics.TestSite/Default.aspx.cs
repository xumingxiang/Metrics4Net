using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Metrics.TestSite
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Dictionary<string, string> tag1 = new Dictionary<string, string>();
            tag1.Add("tag1", "bbb");


           
            Random ran = new Random();
            // int RandKey = ran.Next(100, 999);

            for (int i = 0; i < 50; i++)
            {
                int RandKey = ran.Next(100, 150);
                Metric.Point("Grafana_test1", RandKey, tag1);

                int RandKey2 = ran.Next(50, 100);
                Metric.Point("LogMetric4", RandKey2, tag1);

                Thread.Sleep(10);
            }

            watch.Stop();

            Response.Write(watch.ElapsedMilliseconds);
        }
    }
}