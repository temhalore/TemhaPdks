using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.Configuration;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.Enums;
using LorePdks.BAL.Managers.Deneme;
using LorePdks.BAL.Managers.Deneme.Interfaces;
using LorePdks.COMMON.Logging;

namespace LorePdks.API.Controllers.Deneme
{
    [Route("Api/Deneme")]
    [ApiController]
    public class DenemeController (ILogger<DenemeController> _logger, IDenemeManager _denemeManager ) : ControllerBase
    {
     

        [HttpGet("test-log")]
        public IActionResult TestLog()
        {
            AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Info($"AppEnums.LOG_INDEX_TIP.LOGIN aaaaaaaaaaaaaaaaa");


            AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.GENEL).Info($"AppEnums.LOG_INDEX_TIP.GENEL aaaaaaaaaaa");

            AppLogService.Instance.Info("AppLogService.Instance.Info aaaaaaaaaaaaaaaa");

            AppLogService.Instance.Error("AppLogService.Instance.Error aaaaaaaaaaaaaaaa");

            AppLogService.Instance.Warn("AppLogService.Instance.Warn aaaaaaaaaaaaaaaa");

            AppLogService.Instance.Debug("AppLogService.Instance.Debug aaaaaaaaaaaaaaaa");

           var aa =  _denemeManager.getTestString();

            _logger.LogTrace("This is a trace log");
            _logger.LogDebug("This is a debug log");
            _logger.LogInformation("This is an information log");
            _logger.LogWarning("This is a warning log");
            _logger.LogError("This is an error log");
            _logger.LogCritical("This is a critical log");


            AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.LOGIN).Info($"AppEnums.LOG_INDEX_TIP.LOGIN aaaaaaaaaaaaaaaaa2");


            AppLogService.Instance.SetLogIndexTip(AppEnums.LOG_INDEX_TIP.GENEL).Info($"AppEnums.LOG_INDEX_TIP.GENEL aaaaaaaaaaa2");

            AppLogService.Instance.Info("AppLogService.Instance.Info aaaaaaaaaaaaaaaa2");

            AppLogService.Instance.Error("AppLogService.Instance.Error aaaaaaaaaaaaaaaa2");

            AppLogService.Instance.Warn("AppLogService.Instance.Warn aaaaaaaaaaaaaaaa2");

            AppLogService.Instance.Debug("AppLogService.Instance.Debug aaaaaaaaaaaaaaaa2");

            var aa2 = _denemeManager.getTestString();

            _logger.LogTrace("This is a trace log2");
            _logger.LogDebug("This is a debug log2");
            _logger.LogInformation("This is an information log2");
            _logger.LogWarning("This is a warning log2");
            _logger.LogError("This is an error log2");
            _logger.LogCritical("This is a critical log2");


            return Ok(aa);
        }

        //TODO: bunu muhakak kap
        [HttpGet]
        [Route("GetEnvironmentVariables")]
        public IActionResult getEnviromentName()
        {
    
            var response = new ServiceResponse<object>();
            try
            {

                //var data =
                //    new {
                //        aa = $"CoreConfig.ConfigName:{CoreConfig.ConfigName}, Environment.GetEnvironmentVariable(\"ASPNETCORE_ENVIRONMENT\"): {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}"
                //        ,
                //        EnvironmentGetEnvironmentVariables = Environment.GetEnvironmentVariables()
                //    };


                // data.EnvironmentName = CoreConfig.EnvironmentName; Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")


                //response.data = $"CoreConfig.ConfigName:{CoreConfig.ConfigName}, Environment.GetEnvironmentVariable(\"ASPNETCORE_ENVIRONMENT\"): {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}"
                response.data = $"ConfigName:{CoreConfig.ConfigName}"
                        ;
                //response.data = new { CoreConfigEnvironmentName = CoreConfig.EnvironmentName, EnvironmentGetEnvironmentVariables = Environment.GetEnvironmentVariables() };
                return Ok(response);
            }
            catch (AppException appEx)
            {
                response = new ServiceResponse<object>(appEx);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.messageType = ServiceResponseMessageType.Error.ToString();
                return Ok(response);
            }
        }



        [HttpPost]
        [Route("getSingleValuePost")]
        public IActionResult getSingleValuePost(SingleValueDTO<string> request)
        {

            var response = new ServiceResponse<object>();
            try
            {
                response.data = request.Value;
                return Ok(response);
            }
            catch (AppException appEx)
            {
                response = new ServiceResponse<object>(appEx);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.messageType = ServiceResponseMessageType.Error.ToString();
                return Ok(response);
            }
        }


        [HttpGet]
        [Route("GetValue")]
        public IActionResult GetValue()
        {
            var response = new ServiceResponse<object>();
            try
            {
                response.data = "selam canımmm!!!";
                return Ok(response);
            }
            catch (AppException appEx)
            {
                response = new ServiceResponse<object>(appEx);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.messageType = ServiceResponseMessageType.Error.ToString();
                return Ok(response);
            }
        }

        /// <summary>
        /// örnek bir get vlue lu metod çağrılması :https://localhost:7104/Api/Deneme/getMetodDeneme?request=adasdas
        /// 
        /// call get metod da buna benzer bir metod isteyeceğiz bunu kendileri yazacak biz sadece odemeKey göndereceğiz.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("postMetodDeneme")]
        // public IActionResult getMetodDeneme([FromUri] SingleValueDTO<long> request)
        public IActionResult postMetodDeneme(string odemeKey)
        {
            var response = new ServiceResponse<object>();
            try
            {
                response.data = odemeKey;
                return Ok(response);
            }
            catch (AppException appEx)
            {
                response = new ServiceResponse<object>(appEx);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.messageType = ServiceResponseMessageType.Error.ToString();
                return Ok(response);
            }
        }
    }
}
