using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.Configuration;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Deneme
{
    [Route("Api/Deneme")]
    [ApiController]
    public class DenemeController : ControllerBase
    {
        //   private readonly IDenemeManager _denemeManager;

        public DenemeController()
        {
            //        _denemeManager = denemeManager;
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
