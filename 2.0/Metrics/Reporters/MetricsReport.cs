using Metrics.MetricData;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Metrics.Reporters
{
    public partial interface MetricsReport : Utils.IHideObjectMembers
    {
        void RunReport(MetricsData metricsData, Func<HealthStatus> healthStatus, CancellationToken token);

        //<summary>
        //描述：写入存储设备
        //作者：徐明祥
        //日期:20150531
        //</summary>
        void WriteStore(List<PointMetricEntity> logs);
    }
}