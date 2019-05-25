layui.define(['jquery', 'auth', 'layer'], function (exports) {
    var $ = layui.$,
        auth = layui.auth,
        layer = layui.layer;

    var http = {
        ajax: function (opts) {
            if (typeof (opts.data) == 'function') opts.data = opts.data();

            if (opts.method.toUpperCase() == 'POST') opts.data = JSON.stringify(opts.data);

            var loadingIdx = layer.load(1);

            return $.ajax({
                url: opts.url,
                method: opts.method,
                dataType: 'json',
                data: opts.data,
                contentType: 'application/json; charset=utf-8',
                headers: {
                    Authentication: auth.token()
                },

                statusCode: {
                    401: function () {
                        auth.delToken();
                        layer.alert('请登录');
                    },
                    204: function () {
                        layer.alert("API没有返回内容");
                    },
                    403: function () {
                        layer.alert('没有权限.');
                    },
                    500: function () {
                        layer.alert('服务器内部错误，请稍后再试');
                    },
                    404: function () {
                        layer.alert(opts.url + ' not found.');
                    }
                }
            })
                .done(function (data, textStatus, jqXHR) {
                    if (data.ret != 0) {
                        layer.alert(data.errStr || '出现错误');
                    }
                    else {
                        if (opts.done) opts.done(data.data);
                    }
                })
                .fail(function (jqXHR, textStatus, errorThrown) { layer.alert(errorThrown || '出现错误'); })
                .always(function (data, textStatus, jqXHR) {
                    layer.close(loadingIdx);
                });
        },
        get: function (url, data, callback) {
            var opts = {
                url: url,
                method: 'GET'
            };

            if (typeof (data) == 'function') opts.done = data;
            else {
                opts.data = data;
                opts.done = callback;
            }

            return this.ajax(opts);
        },
        post: function (url, data, callback) {
            var opts = {
                url: url,
                data: data,
                method: 'POST',
                done: callback
            };

            return this.ajax(opts);
        }
    }

    exports('http', http);
});

