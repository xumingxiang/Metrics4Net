using System.Collections.Generic;

namespace Metrics
{
    public class PointMetricEntity
    {
        public string Name { get; set; }

        public double Value { get; set; }

        public long TimeStamp { get; set; }

        public Dictionary<string, string> Tags { get; set; }
    }
}