using System;

namespace InQuant.Framework.Exceptions
{
    public class HopexException : Exception
    {
        public HopexException(string errMsg) : this(string.Empty, errMsg) { }

        public HopexException(string errCode, string errMsg) : base(errMsg)
        {
            ErrCode = errCode;
            ErrMsg = errMsg;
        }

        public string ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}
