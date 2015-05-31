using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metrics.Reporters
{
    /// <summary>
    /// 对原有Metrics.Reporters.BaseReport 的扩展
    /// </summary>
    public abstract partial class BaseReport : MetricsReport
    {
        //<summary>
        //描述：写入存储设备
        //作者：徐明祥
        //日期:20150531
        //</summary>
        public virtual void WriteStore(List<PointMetricEntity> logs)
        {

        }
    }
}
