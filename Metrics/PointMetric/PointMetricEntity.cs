using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
