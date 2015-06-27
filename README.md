# PointMetric文件夹下的内容为扩展接口
对客户端来讲，主要增加了Metric.Point 打点方法。
该方法有别与其他5种打点类型
* 1：按需打点，即在代码经过的时候才会打点，而其他5种是以委托的形式注册，启动后后台线程会一直执行委托打点，写库
* 2：相比其他5种Metric.Point 对代码入侵较少
* 3：Metric.Point 方法有字典型Tag参数，可以记录更多信息，方便在dashbord中使用各种group by 、where 等统计、筛选



# 对原有框架源码的改动
* 1，Metrics.Influxdb.InfluxdbReport					改成部分类(partial)，以便扩展，详见本目录下InfluxdbReport.cs文件
* 2，Metrics.Reporters.BaseReport					    增加 public virtual void WriteStore(List<PointMetricEntity> logs) 实现方法
* 3，Metrics.Metric									改成部分类(partial)，以便扩展，详见本目录下Metric.cs文件
* 4，Metrics.Reporters.ScheduledReporter.report		字段(属性)名称改成Report，访问权限改成对外只读；增加Interval属性，对外开放只读，在构造函数内赋值
* 5，Metrics.Reporters.MetricsConfig.reports	        字段(属性)名称改成MetricsReports，访问权限改成对外只读
* 6，Metrics.Reporters.MetricsReport			增加 void WriteStore(List<PointMetricEntity> logs); 接口

为了尽量减少对原框架的侵入，所有扩展尽量放在PointMetric文件夹下

原框架地址：https://github.com/etishor/Metrics.NET

# 使用Metrics4Net InfluxDb grafana 构建指标监控系统
https://github.com/xumingxiang/Metrics4Net/wiki/%E4%BD%BF%E7%94%A8Metrics4Net-InfluxDb-grafana-%E6%9E%84%E5%BB%BA%E6%8C%87%E6%A0%87%E7%9B%91%E6%8E%A7%E7%B3%BB%E7%BB%9F

# C#应用程序调用示例
```
 public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Metric.Config.WithReporting(report => report.WithInfluxDb("192.168.49.132", 8086, "root", "root", "metrics", TimeSpan.FromSeconds(5)));
        }
        .....
    }
  
    .................
    .................
  
  Dictionary<string, string> tag1 = new Dictionary<string, string>();
  tag1.Add("tag1", "bbb");
  Metric.Point("plu_test_request_count", 1, tag1);
```
  
# WebApi调用
```
post:
/Api/Point.ashx
FormData: 
name:webapi_metrics_point_test
value:99.02877185959369
tags:tag1=bbb&tag2=tag2_val
```
  
# JS调用
```
  <script src="Scripts/Metric.js"></script>
  <script type="text/javascript">
    metric.point("webapi_metrics_point_test", 123, { "tag1": "bbb", "tag2": "tag2_val" });
  </script>
```

