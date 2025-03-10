
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace LorePdks.COMMON.Models.ServiceResponse
{
    public class ServiceResponse<T> : IServiceResponse<T>
    {
 
        string _messageType;
        string _messageTypeCode;
        int _code;
        string _messageHeader;
        string _message;
        public bool IsSuccess { get; set; } = true;

        AppExceptionModel _AppException;
        //Data
        public T data { get; set; }
        ////////
        public ServiceResponse()
        {
        }

        public ServiceResponse(T entity)
        {
            _messageType = ServiceResponseMessageType.Success;
        }


        //todo:ae  buna gerek yok aslına ama çok yerde kullanılmış ekledim bunu sonra düelt  //
        public IActionResult OkResult(T data, string message = "İşlem tamamlandı")
        {
            this.data = data;
            this.message = message;
            return new OkObjectResult(this);
        }

        public ServiceResponse(AppException appEx)
        {
            _messageType = ServiceResponseMessageType.Error;
            _message = appEx.appMessage;
            _messageHeader = appEx.messageHeader;
            _messageTypeCode = Convert.ToString(appEx.errorCode);
            _code = Convert.ToInt16(appEx.code);
            this.IsSuccess = false;
            _AppException = new AppExceptionModel()
            {
                code = appEx.code,
                errorCode = appEx.errorCode,
                messageHeader = appEx.messageHeader,
                appMessage = appEx.appMessage
            };
        }

        public string messageHeader
        {
            get { return _messageHeader; }
            set
            {
                _messageHeader = value;
            }
        }

        public int messageAppCode
        {
            get { return _code; }
            set
            {
                _code = value;
            }
        }
        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _messageType = ServiceResponseMessageType.Success;
                _message = value;
            }
        }

        //Nessage Type Setter
        public string messageType
        {
            get
            {
                return _messageType;

            }
            set
            {
                _messageType = value;
                if (messageType == ServiceResponseMessageType.Success)
                {
                    _messageHeader = "Başarılı İşlem";
                }
                if (messageType == ServiceResponseMessageType.Error)
                {
                    this.IsSuccess = false;
                    _messageHeader = "Hatalı İşlem";
                }
                if (messageType == ServiceResponseMessageType.Warning)
                {
                    _messageHeader = "Uyarı Mesajı";
                }
            }
        }


    }


    public static class ServiceResponseMessageType
    {
        public static string Error = "error";
        public static string Success = "success";
        public static string Warning = "warning";
        public static string Info = "info";
    }

    public static class ServiceResponseMessage
    {
        public static string Save = "Kaydedildi";
        public static string Delete = "Silindi";
        public static string List = "Liste çekildi";
    }
    public class AppExceptionModel
    {
        public string messageHeader { get; set; }
        public int code { get; set; }
        public string appMessage { get; set; }
        public string errorCode { get; set; }

    }


}
