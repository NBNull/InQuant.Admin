layui.define([], function (exports) {

    var auth = {
        token: function () {
            var auth = this.getAuth();
            if (!auth)
                return null;
            return auth.token;
        },
        userId: function () {
            var auth = this.getAuth();
            if (!auth)
                return null;
            return auth.userId;
        },
        getAuth: function () {
            return layui.data('auth');
        },
        setAuth: function (userId, token) {
            layui.data('auth', { key: 'userId' , value: userId });
            layui.data('auth', { key: 'token' , value: token });
        },
        delAuth: function () {
            layui.data('auth', null); 
        }
    };

    //输出test接口
    exports('auth', auth);
});

