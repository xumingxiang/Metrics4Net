using Metrics.Json;
using Metrics.Reporters;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Metrics.Influxdb
{
    /// <summary>
    /// 描述：对原有Metrics.Influxdb.InfluxdbReport 类的扩展
    /// 作者：徐明祥
    /// 日期：20150531
    /// </summary>
    internal partial class InfluxdbReport : BaseReport
    {
        /// <summary>
        /// 描述：将LogMetric写入Influxdb数据库
        /// 作者：徐明祥
        /// 日期：20150531
        /// </summary>
        /// <param name="logs"></param>
        public override void WriteStore(List<PointMetricEntity> logs)
        {
            if (logs == null || logs.Count == 0) { return; }
            List<InfluxRecord> data = new List<InfluxRecord>();
            foreach (var item in logs)
            {
                IEnumerable<string> columns = new string[] { "value" };

                IEnumerable<JsonValue> points = new JsonValue[] { new DoubleJsonValue(item.Value) };

                if (item.Tags != null && item.Tags.Count > 0)
                {
                    var tagKeys = item.Tags.Keys.ToArray();

                    JsonValue[] tagVals = new JsonValue[item.Tags.Count];

                    columns = Enumerable.Concat(columns, tagKeys);

                    for (int i = 0; i < tagKeys.Length; i++)
                    {
                        var tagVal = item.Tags[tagKeys[i]];
                        tagVals[i] = new StringJsonValue(tagVal);
                    }
                    points = Enumerable.Concat(points, tagVals);
                }
                var record = new InfluxRecord(item.Name, item.TimeStamp, columns, points);
                data.Add(record);
            }

            using (var client = new WebClient())
            {
                var jsonstr = new CollectionJsonValue(data.Select(d => d.Json)).AsJson();
                client.UploadStringAsync(this.influxdb, jsonstr);
            }
        }
    }
}