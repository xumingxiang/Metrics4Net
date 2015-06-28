using Metrics.MetricData;
using Metrics.Utils;
using System;
using System.Threading;

namespace Metrics.Reporters
{
    public sealed class ScheduledReporter : IDisposable
    {
        private readonly Scheduler scheduler;
        public readonly MetricsReport Report;//TODO:对外开放可读 by xmx
        private readonly MetricsDataProvider metricsDataProvider;
        private readonly Func<HealthStatus> healthStatus;

        public TimeSpan Interval { get; private set; }//TODO:将循环时间对外开发 by 徐明祥 20150531

        public ScheduledReporter(MetricsReport reporter, MetricsDataProvider metricsDataProvider, Func<HealthStatus> healthStatus, TimeSpan interval)
            : this(reporter, metricsDataProvider, healthStatus, interval, new ActionScheduler()) { }

        public ScheduledReporter(MetricsReport report, MetricsDataProvider metricsDataProvider, Func<HealthStatus> healthStatus, TimeSpan interval, Scheduler scheduler)
        {
            this.Interval = interval;
            this.Report = report;
            this.metricsDataProvider = metricsDataProvider;
            this.healthStatus = healthStatus;
            this.scheduler = scheduler;
            this.scheduler.Start(interval, t => RunReport(t));
        }

        private void RunReport(CancellationToken token)
        {
            Report.RunReport(this.metricsDataProvider.CurrentMetricsData, this.healthStatus, token);
        }

        public void Dispose()
        {
            using (this.scheduler) { }
            using (this.Report as IDisposable) { }
        }
    }
}