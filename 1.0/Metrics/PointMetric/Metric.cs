using System.Collections.Generic;

namespace Metrics
{
    /// <summary>
    /// 描述:对原有Metric的扩展
    /// 作者:徐明祥
    /// 日期:20150531
    /// </summary>
    public static partial class Metric
    {
        public static void Point(string name, double value = 1, Dictionary<string, string> tags = null)
        {
            PointMetric.Point(name, value, tags);
        }

        public static void Point(string name, Dictionary<string, string> tags = null)
        {
            Point(name, 1, tags);
        }

        public static void Point(string name)
        {
            Point(name, 1, null);
        }
    }
}