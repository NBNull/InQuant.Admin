layui.use(['form', 'element', 'jquery'], function () {
    layer = layui.layer;
    element = layui.element;
    $ = layui.$;

    window.tab = {
        tabExists: function (id) {
            var exists = false;
            layui.each($('.layui-tab-title li'), function (idx, item) {
                if ($(item).attr('lay-id') == id) {
                    exists = true;
                    return false;
                }
            });
            return exists;
        },
        tabAdd: function (title, url, id) {
            if (this.tabExists(id)) {
                this.tabChange(id);
                return;
            }
            element.tabAdd('xbs_tab', {
                title: title
                , content: '<iframe tab-id="' + id + '" frameborder="0" src="' + url + '" scrolling="yes" class="x-iframe"></iframe>'
                , id: id
            });
        }
        , tabDelete: function (id) {
            element.tabDelete('xbs_tab', id); 
        }
        , tabChange: function (id) {
            element.tabChange('xbs_tab', id);
        }
    };

    $('.container .left_open i').click(function (event) {
        if ($('.left-nav').css('left') == '0px') {
            $('.left-nav').animate({ left: '-221px' }, 100);
            $('.page-content').animate({ left: '0px' }, 100);
            $('.page-content-bg').hide();
        } else {
            $('.left-nav').animate({ left: '0px' }, 100);
            $('.page-content').animate({ left: '221px' }, 100);
            if ($(window).width() < 768) {
                $('.page-content-bg').show();
            }
        }

    });

    $('.page-content-bg').click(function (event) {
        $('.left-nav').animate({ left: '-221px' }, 100);
        $('.page-content').animate({ left: '0px' }, 100);
        $(this).hide();
    });

    $('.layui-tab-close').click(function (event) {
        $('.layui-tab-title li').eq(0).find('i').remove();
    });
       
    $('.left-nav #nav li').click(function (event) {

        if ($(this).children('.sub-menu').length) {
            if ($(this).hasClass('open')) {
                $(this).removeClass('open');
                $(this).find('.nav_right').html('&#xe697;');
                $(this).children('.sub-menu').stop().slideUp();
                $(this).siblings().children('.sub-menu').slideUp();
            } else {
                $(this).addClass('open');
                $(this).children('a').find('.nav_right').html('&#xe6a6;');
                $(this).children('.sub-menu').stop().slideDown();
                $(this).siblings().children('.sub-menu').stop().slideUp();
                $(this).siblings().find('.nav_right').html('&#xe697;');
                $(this).siblings().removeClass('open');
            }
        } else {

            var url = $(this).children('a').attr('_href');
            var title = $(this).find('cite').html();
            var index = $('.left-nav #nav li').index($(this));

            for (var i = 0; i < $('.x-iframe').length; i++) {
                if ($('.x-iframe').eq(i).attr('tab-id') == index + 1) {
                    tab.tabChange(index + 1);
                    event.stopPropagation();
                    return;
                }
            }

            tab.tabAdd(title, url, index + 1);
            tab.tabChange(index + 1);
        }

        event.stopPropagation();

    });

    $(document).on('click', '.logout', function () {
        $.get('/api/admin/user/logout', function () {
            location.href = "/user/login";
        });
    });    
});


