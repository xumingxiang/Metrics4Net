using Metrics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Metrics
{
    internal sealed class PointMetric
    {
        static TimerBatchBlock<PointMetricEntity> block;

        const int taskNum = 1;

        static PointMetric()
        {
            var scheduledReporters = Metric.Config.MetricsReports.ScheduledReporters;
            int blockElapsed = PointMetricConfig.BlockElapsed;
            if (blockElapsed <= 0)
            {
                blockElapsed = (int)scheduledReporters.Average(x => x.Interval.TotalMilliseconds);
            }
            block = new TimerBatchBlock<PointMetricEntity>(taskNum, (batch) =>
             {
                 foreach (var scheduledReporter in scheduledReporters)
                 {
                     scheduledReporter.Report.WriteStore(batch);
                 }
             }, PointMetricConfig.QueueMaxLength, PointMetricConfig.BatchSize, blockElapsed);
        }

        public static void Point(string name, double value, Dictionary<string, string> tags = null)
        {
            var _tags = new Dictionary<string, string>();
            _tags.Add("server_ip", ServerIP);

            if (tags != null && tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    _tags[tag.Key] = tag.Value;
                }
            }
            var logMetricEntity = new PointMetricEntity();
            logMetricEntity.Name = name;
            logMetricEntity.Value = value;
            logMetricEntity.Tags = _tags;
            logMetricEntity.TimeStamp = DateTime.Now.ToUnixTime();
            block.Enqueue(logMetricEntity);
        }

        private static string serverIP;
        private static string ServerIP
        {
            get
            {
                if (string.IsNullOrWhiteSpace(serverIP))
                {
                    serverIP = GetServerIP();
                }
                return serverIP;
            }
        }
        /// <summary>
        /// 获取服务器IP
        /// </summary>
        /// <returns></returns>
        private static string GetServerIP()
        {
            string str = "Did not get to the server IP";
            if (HttpContext.Current != null)
            {
                str = HttpContext.Current.Request.ServerVariables.Get("Local_Addr");
            }
            else
            {
                try
                {
                    string hostName = Dns.GetHostName();
                    var ipAddress = Dns.GetHostEntry(hostName)
                        .AddressList
                        .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                    if (ipAddress != null)
                    {
                        str = ipAddress.ToString();
                    }
                    return string.Empty;
                }
                catch (Exception) { }
            }
            return str;
        }
    }
}
