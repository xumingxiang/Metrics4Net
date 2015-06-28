// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt, utc) { //author: meizz 
    var o = {

        "M+": (utc ? this.getUTCMonth() : this.getMonth()) + 1, //月份 
        "d+": (utc ? this.getUTCDate() : this.getDate()), //日 
        "h+": (utc ? this.getUTCHours() : this.getHours()), //小时 
        "m+": (utc ? this.getUTCMinutes() : this.getMinutes()), //分 
        "s+": (utc ? this.getUTCSeconds() : this.getSeconds()), //秒 
        "q+": Math.floor(((utc ? this.getUTCMonth() : this.getMonth()) + 3) / 3), //季度 
        "S": utc ? this.getUTCMilliseconds() : this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

$(function () {

    var host = "http://192.168.49.132:8086";
    var db = "metrics";
    var u = "root";
    var p = "root";
    var query_base = host + '/db/' + db + '/series?u=' + u + '&p=' + p + '&q=';
    function query() {

        var metric_name = $("#metric_name").val();
        var group_by_time = $("#group_by_time").val() || "1m";
        var aggr = $("#aggr").val();
        var group_by_tag = $("#group_by_tag").val();

        var start_time_str = $("#start_time").val();
        var start_time = new Date(start_time_str);
        var utc_start_time_fm = start_time.Format("yyyy-MM-dd hh:mm:ss", true);

        var end_time_str = $("#end_time").val() || new Date().Format("yyyy-MM-dd hh:mm:ss");
        var end_time = new Date(end_time_str);
        var utc_end_time_fm = end_time.Format("yyyy-MM-dd hh:mm:ss", true);

        if (!metric_name || metric_name == "") {
            alert("Metric Name 不能为空");
            return;
        }

        if (end_time <= start_time) {
            alert("结束时间不能大于开始时间");
            return;
        }

        if (!!myChart) {
            myChart.clear();
            myChart.dispose();
        }
        var myChart = echarts.init(document.getElementById('main'));
        $("#empty_point").hide();
        myChart.showLoading({
            text: '正在努力的读取数据中...',    //loading话术
        });



        var query_str = query_base + 'select ' + aggr + '( value )' + ' from ' + metric_name + ' where ';
        if (start_time != "") {
            query_str += ' time > \'' + utc_start_time_fm + '\' and ';
        }

        query_str += ' time < \'' + utc_end_time_fm + '\' ';
        query_str += ' group%20by%20';
        if (group_by_tag != "") {
            query_str += group_by_tag + '%20%2C';
        }
        query_str += ' time(' + group_by_time + ')';

        $.ajax({
            'type': 'get',
            'url': query_str,
            'success': function (result) {
                if (!result || result.length == 0) {
                    myChart.hideLoading();
                    $("#empty_point").text("没有相关查到数据").show();
                    return;
                }
                var data = result[0];
                var metric_name = data.name;
                var columns = data.columns;
                var points = data.points;
                var points_len = points.length;

                var legend = [];
                var series = [];
                var xAxis = [];
                var xAxis_timestamp = [];
                var group_index = 2;

                for (var j = 0; j < points_len; j++) {
                    var group_val = metric_name;
                    if (columns.length > 2) {
                        group_val = points[j][group_index];
                        group_val = metric_name + "." + group_val;
                    }

                    if (legend.indexOf(group_val) < 0) {
                        legend.push(group_val);
                    }

                    var timestamp = points[points_len - j - 1][0];
                    if (xAxis_timestamp.indexOf(timestamp) < 0) {
                        xAxis_timestamp.push(timestamp);
                    }
                }

                $.each(xAxis_timestamp, function (index, item) {
                    xAxis.push(new Date(item).Format("yy-MM-dd hh:mm:ss"));
                });


                for (var i = 0; i < legend.length; i++) {
                    var legend_item = legend[i];

                    var serie = {};
                    serie.name = legend_item;
                    serie.type = "line";
                    serie.data = [];

                    for (var j = points_len - 1; j >= 0; j--) {
                        var point = points[j];

                        var group_val = point[group_index];
                        if (legend_item == metric_name || legend_item == metric_name + "." + group_val) {
                            serie.data.push(point[1]);
                        }
                    }
                    series.push(serie);
                }

                var option = {
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: legend
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: { show: true },
                            dataView: { show: true, readOnly: false },
                            magicType: { show: true, type: ['line', 'bar'] },
                            restore: { show: true },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            data: xAxis,
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value',
                            splitArea: { show: true }
                        }
                    ],
                    series: series
                };

                myChart.hideLoading();
                myChart.setOption(option);
            },
            'error': function (result) {
                myChart.hideLoading();
                $("#empty_point").text("Sorry，查询失败，请调整参数重试").show();
                return;
            }
        });
    }

    var now = new Date();
    var default_start_time = new Date((now.setHours(now.getHours() - 1))).Format("yyyy-MM-dd hh:mm:ss");
    var default_end_time = new Date().Format("yyyy-MM-dd hh:mm:ss");

    $("#start_time")
        .val(default_start_time)
        .datetimepicker({
            defaultValue: default_start_time,
            dateFormat: 'yy-mm-dd',
            showSecond: true,
            timeFormat: 'HH:mm:ss',
            currentText: '当前',
            closeText: '确定'
        });

    $("#end_time")
        .val(default_end_time)
        .datetimepicker({
            defaultValue: default_end_time,
            dateFormat: 'yy-mm-dd',
            showSecond: true,
            timeFormat: 'HH:mm:ss',
            currentText: '当前',
            closeText: '确定'
        });


    $("#metric_name").autocomplete({
        source: function (request, responseFn) {
            var metric_name = request.term;
            var query_str = query_base + 'list+series+%2F' + metric_name + '%2F';
            $.get(query_str, function (result) {
                var points = result[0].points;
                var series = $.map(points, function (item) {
                    return item[1];
                });
                responseFn(series);
            });
        }
    });

    $("#group_by_tag").autocomplete({
        source: function (request, responseFn) {
            var metric_name = $("#metric_name").val();
            if (!metric_name || metric_name == "") { return false; }
            var kw = request.term;
            var query_str = query_base + 'select+*+from+%22' + metric_name + '%22+limit+1';
            $.get(query_str, function (result) {
                var columns = result[0].columns;
                var series = [];
                for (var i = 3; i < columns.length; i++) {
                    var tag = columns[i];
                    if (tag.indexOf(kw) >= 0) {
                        series.push(tag);
                    }
                }
                responseFn(series);
            });
        }
    });


    $("#btn_query").click(function () {
        query();
    });
});


//http://192.168.49.132:8086/db/metrics/series?p=root&q=list+series+%2Fwebapi_metrics_point_te%2F&u=root
//http://192.168.49.132:8086/db/metrics/series?u=root&p=root&q=select+*+from+%22webapi_metrics_point_test%22+limit+1
//http://192.168.49.132:8086/db/metrics/series?u=root&p=root&q=select+*+from+%22webapi_metrics_point_test%22+limit+1
