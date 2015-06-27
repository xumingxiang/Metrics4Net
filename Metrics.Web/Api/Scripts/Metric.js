(function () {

    var createAjax = function () {
        var xhr = null;
        try {//IE系列浏览器
            xhr = new ActiveXObject("microsoft.xmlhttp");
        } catch (e1) {
            try {//非IE浏览器
                xhr = new XMLHttpRequest();
            } catch (e2) {
                window.alert("您的浏览器不支持ajax，请更换！");
            }
        }
        return xhr;
    };
    var ajax = function (conf) {
        var type = conf.type;//type参数,可选 
        var url = conf.url;//url参数，必填 
        var data = conf.data;//data参数可选，只有在post请求时需要 
        var dataType = conf.dataType;//datatype参数可选 
        var success = conf.success;//回调函数可选
        if (type == null) {//type参数可选，默认为get
            type = "get";
        }
        if (dataType == null) {//dataType参数可选，默认为text
            dataType = "text";
        }
        var xhr = createAjax();
        xhr.open(type, url, true);
        if (type == "GET" || type == "get") {
            xhr.send(null);
        } else if (type == "POST" || type == "post") {
            xhr.setRequestHeader("content-type", "application/x-www-form-urlencoded");
            xhr.send(data);
        }
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                if (dataType == "text" || dataType == "TEXT") {
                    if (success != null) {//普通文本
                        success(xhr.responseText);
                    }
                } else if (dataType == "xml" || dataType == "XML") {
                    if (success != null) {//接收xml文档
                        success(xhr.responseXML);
                    }
                } else if (dataType == "json" || dataType == "JSON") {
                    if (success != null) {//将json字符串转换为js对象
                        success(eval("(" + xhr.responseText + ")"));
                    }
                }
            }
        };
    }

    var metric = {};

    metric.point = function (name, value, tags) {

        var tags_str = "";

        if (!!tags) {
            for (var key in tags) {
                var val = tags[key];
                tags_str += key + "=" + val + "&";
            }
            tags_str = tags_str.substring(0, tags_str.length - 1);
        }

        ajax({
            type: "post",//post或者get，非必须
            url: "Point.ashx",//必须的
            data: "name=" + name + "&value=" + (value || 1) + "&tags=" + encodeURIComponent(tags_str),//非必须
            dataType: "json",//text/xml/json，非必须
            success: function (result) {//回调函数，非必须
                //  alert(result);
            }
        });
    }

    window.metric = metric;
})();



