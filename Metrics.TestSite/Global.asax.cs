using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using Metrics.TestSite;

namespace Metrics.TestSite
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            Metric.Config.WithReporting(report => report.WithInfluxDb("192.168.49.130", 8086, "root", "root", "metrics", TimeSpan.FromSeconds(2)));

            // PointMetricConfig.SetConfig(2000, 200, 2000);//PointMetric 的配置，取消注释将覆盖默认配置，默认配置参数分别为1000,50,1000
        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码

        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码

        }
    }
}
