using LorePdks.COMMON.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.Models
{
    public class AppException : Exception
    {
        private MessageCode messageCode;
        private object data;
        public string messageHeader { get; set; } = "Hata";
        public int code { get; set; }
        public string appMessage { get; set; }
        public string errorCode { get; set; }
        public AppException(string v)
        {

        }
        public AppException(int code, string message)
        {
            this.code = code;
            appMessage = message;
            errorCode = code != 0 ? code.ToString() : "";
        }

        public AppException(MessageCode code, string message)
        {
            this.code = Convert.ToInt16(code);
            appMessage = message;
            errorCode = code.ToString();

        }

        public AppException(MessageCode messageCode, object data)
        {
            this.messageCode = messageCode;
            this.data = data;
        }

        public AppException(string messageHeader, MessageCode messageCode, Exception exp)
        {
            this.messageHeader = messageHeader;
            code = Convert.ToInt16(MessageCode.ERROR_500_BIR_HATA_OLUSTU);
            appMessage = exp.Message;
            errorCode = messageCode.ToString();
        }
    }
}
