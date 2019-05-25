layui.define(['form', 'table'], function (exports) {
    var table = layui.table
        , form = layui.form;

    var common = {
        isNull: function (v) {
            return v === null || v === undefined;
        },
        /*
         * 渲染表格
         * {
         *  elem: table dom ,eg #table1
         *  url: get data url,
         *  where: query where condition,
         *  done: function call after data loaded ,
         *  cols: colmuns define,
         *  page: 如果等于null或者undefined，开启分页，其他值不分页,
         *  width: 宽度，默认100%，
         *  height: 高度，默认full-180
         *  limit: 分页大小，默认20
         *  parseData: 获取到数据后调用，可调整数据格式
         * }
         */
        renderTable: function (opts) {
            var _index = layer.load(2, { time: 30 * 1000 }); //最长等待30秒 
            table.render({
                elem: opts.elem,
                url: opts.url,
                cols: opts.cols,
                width: opts.width,
                height: opts.height || 'full-180',
                where: opts.where,
                method: 'get',
                limit: opts.limit || 20,
                totalRow: common.isNull(opts.totalRow) ? false : opts.totalRow,
                toolbar: common.isNull(opts.toolbar) ? false : opts.toolbar,
                request: {
                    pageName: 'page'
                    , limitName: 'limit'
                },
                parseData: opts.parseData || function (ret) {
                    return {
                        "count": ret.data ? ret.data.totalCount : 0,
                        "data": ret.data ? ret.data.result : [],
                        "code": ret.ret,
                        'msg': ret.errStr
                    };
                },
                text: {
                    none: '暂无数据'
                },
                loading: true,
                page: opts.page === undefined || opts.page === null ? true : false,
                done: function (res, curr, count) {
                    if (_index !== null && _index !== undefined) {
                        layer.close(_index);
                    }
                    opts.done && opts.done();
                }
            });
        },
        /*
         * 重新加载表格
         * @tableIns render后返回的对象或者table id
         * @where 查询条件
         * @toFirst 如果为空，加载第一页，否则加载当前页
         */
        reloadTable: function (tableIns, where, toFirst) {
            var opts = {
                where: where,
                done: function (res, curr, count) {
                    if (_index !== null && _index !== undefined) {
                        layer.close(_index);
                    }
                }
            };
            if (toFirst) {
                opts.page = {
                    curr: 1
                };
            }

            if (opts.where) {
                for (var f in opts.where) opts.where[f] = opts.where[f] == undefined ? '' : opts.where[f];
            }

            var _index = layer.load(2, { time: 30 * 1000 }); //最长等待30秒 

            if (typeof (tableIns) === 'string') {
                table.reload(tableIns, opts);//按table id reload
            }
            else {
                tableIns.reload(opts);
            }
        },
        fixKOBindIssue: function (form) {
            form.on('radio', function (data) {
                $(data.elem).click();
            });

            form.on('checkbox', function (data) {
                $(data.elem).click().click();//not a good idea, but effective
            });
            form.on('switch', function (data) {
                $(data.elem).click().click();
            });
            form.on('select', function (data) {
                var data_bind = $(data.elem).attr('data-bind');
                var value_reg = /value:\s{0,}([a-zA-Z0-9\$\_\.\(\)]+)/;

                if (!value_reg.test(data_bind))
                    return;

                var raw_value = value_reg.exec(data_bind)[1];
                var ko_ctx = ko.contextFor(data.elem);
                var $data = ko.dataFor(data.elem);//ko $data object
                var word_reg = /(\w+)/;

                if (!raw_value)
                    return;

                if (raw_value.indexOf('.') <= 0) {
                    if (typeof ($data[raw_value]) == 'function') $data[raw_value](data.elem.value);
                    else $data[raw_value] = data.elem.value;
                }
                else {
                    var segs = raw_value.split('.');//xxx().yyy.zzz()
                    for (var i = 0; i < segs.length - 1; i++) {
                        var item = segs[i];
                        if (item.startsWith('$')) {
                            $data = ko_ctx[/([\$a-zA-Z0-9\_]+)/.exec(item)[1]];
                            continue;
                        }
                        var field = word_reg.exec(item)[1];
                        if (item.indexOf('()') > 0) {
                            $data = $data[field]();
                        }
                        else {
                            $data = $data[field];
                        }
                    }

                    var lastField = word_reg.exec(segs[segs.length - 1])[1];
                    $data[lastField](data.elem.value);
                }
            });
        },
        getUrlParameter: function (name) {
            name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
            var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
            var results = regex.exec(location.search);
            return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
        },
        getTimeStr: function (date) {
            return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate() + ' ' + date.getHours() + ':' + date.getMinutes() + ':' + date.getSeconds();
        },
        getDateStr: function (date) {
            return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
        },
        export: function (tableConfig, fileName, paras) {
            var fields = [];
            var titles = [];
            var cols = tableConfig.cols;
            if (cols.length === 1) {
                var fs = cols[0].filter(function (item) { return item.field || item.dataField; });
                fields = fs.map(function (item) { return item.field || item.dataField; });
                titles = fs.map(function (item) { return item.title; });
            }
            else if (cols.length === 2) {
                var c1 = cols[0], c2 = cols[1];
                var i = 0; j = 0;
                for (i; i < c1.length; i++) {
                    if (c1[i].field || c1[i].dataField) {
                        fields.push(c1[i].field || c1[i].dataField);
                        titles.push(c1[i].title);
                    }
                    else if (c1[i].colspan) {
                        var mj = j + parseInt(c1[i].colspan);
                        for (j; j < c2.length && j < mj; j++) {
                            fields.push(c2[j].field || c2[j].dataField);
                            titles.push(c1[i].title + c2[j].title);
                        }
                    }
                }
            }
            else {
                console.log("只支持1、2级表头的导出");
            }

            var m = {
                url: tableConfig.url,
                fileName: fileName,
                fields: fields,
                titles: titles,
                paras: paras
            };

            console.log(m);

            window.open('/api/export?paras=' + encodeURIComponent(JSON.stringify(m)), "_blank");
        }
    };

    //输出test接口
    exports('common', common);
});

