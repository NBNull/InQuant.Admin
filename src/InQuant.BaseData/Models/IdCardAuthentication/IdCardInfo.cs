namespace InQuant.BaseData.Models.IdCardAuthentication
{
    public class IdCardInfo
    {
        public string Name { get; set; }
        public string IdNo { get; set; }
        public string RespMessage { get; set; }

        /// <summary>
        /// 0000	身份证信息匹配	身份证信息匹配
        /// 0001	开户名不能为空 开户名不能为空
        /// 0002	开户名不能包含特殊字符 开户名不能包含特殊字符
        /// 0003	身份证号不能为空 身份证号不能为空
        /// 0004	身份证号格式错误 身份证号格式错误
        /// 0007	无此身份证号码 该身份证号码不存在
        /// 0008	身份证信息不匹配 身份证信息不匹配
        /// </summary>
        public string RespCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Birthday { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
    }
}
