using System;
using System.Collections.Generic;
using System.Text;

namespace InQuant.BaseData.Models
{
    /// <summary>
    /// 行情api返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QuotationRpsModel<T>
    {
        public T Data { get; set; }

        public string ErrCode { get; set; }

        public string ErrStr { get; set; }

        public string Ret { get; set; }
    }
}
