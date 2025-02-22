using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.COMMON.Models;
using System.Collections.Immutable;

namespace LorePdks.API.Filters
{
    public class SecurityFilter : IActionFilter
    {
        public string appToken = "";
        public string refreshToken = "";
        public string language = "";
        //private IKisiTokenManager _kisiTokenManager;
        private IHelperManager _helperManager;
        //private IPersmissionManager _persmissionManager;
        public void OnActionExecuting(ActionExecutingContext context)
        {

            var dictionary = new Dictionary<string, string>();
            if (context.HttpContext.Request.RouteValues["action"].ToString() == "save3dOdeme")
            {
                //string jsonString = JsonConvert.SerializeObject(context.HttpContext.Request.Form);
                //context.HttpContext.Items["Netpay3DResponceData"] = context.HttpContext.Request.Form;
                context.HttpContext.Items["Netpay3DResponceDataDictionary"] = context.HttpContext.Request.Form.ToDictionary(x => x.Key, x => x.Value.FirstOrDefault());


                //context.HttpContext.Items["Netpay3DResponceStringData"] = jsonString;
            }

            // if (_helperManager == null)
            // {
            //     _helperManager = (IHelperManager)context.HttpContext.RequestServices.GetService(typeof(IHelperManager));
            // }




            //var getIp= _helperManager.GetIPAddress();



            refreshToken = context.HttpContext.Request.Headers["refreshToken"].ToString();
            language = context.HttpContext.Request.Headers["language"].ToString();
            if (appToken.Contains("admin"))
            {
                appToken = appToken.Replace("admin", "");
            }

            // if (_kisiTokenManager == null)
            // {
            //     _kisiTokenManager = (IKisiTokenManager)context.HttpContext.RequestServices.GetService(typeof(IKisiTokenManager));
            // }
            //
            // if (_helperManager == null)
            // {
            //     _helperManager = (IHelperManager)context.HttpContext.RequestServices.GetService(typeof(IHelperManager));
            // }
            // if (_persmissionManager == null)
            // {
            //     _persmissionManager = (IPersmissionManager)context.HttpContext.RequestServices.GetService(typeof(IPersmissionManager));
            // }
            var isDirectAccess = false;

            //_helperManager.SetDirectAccess(false);

            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                isDirectAccess = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(DirectAccessAttribute), false).Length > 0;
            }
            if (isDirectAccess)
            {
                return;
            }
            string actionName = (string)context.RouteData.Values["action"];
            string controllerName = (string)context.RouteData.Values["controller"];
            //token validation işlemi
            try
            {
                // var kisiToken = _kisiTokenManager.tokenValidate(appToken);
                // if (!_persmissionManager.IsControllerActionPermission(kisiToken.actionPermissionDto, controllerName, actionName))
                //     throw new AppException(500, "Bu Fonksiyona girme yetkiniz yok[" + controllerName + ":" + actionName + "]");
            }
            catch (AppException appEx)
            {

                //  //         context.Result = new RedirectToActionResult("OYSExceptionCacher", "Example",null);
                //  //return;
                //  new ExceptionController().OYSExceptionCacher(appEx);
                //  context.Result = new BadRequestObjectResult(appEx.appMessage);
                //  return;
            }

            foreach (ControllerParameterDescriptor param in context.ActionDescriptor.Parameters)
            {
                if (param.ParameterInfo.CustomAttributes.Any(
                    attr => attr.AttributeType == typeof(FromBodyAttribute))
                )
                {
                    var entity = context.ActionArguments[param.Name];
                    //Burada BeHalhOfUserID'de kullanılabilir.
                    context.HttpContext.Items[controllerName + "_" + actionName] = entity;
                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {


        }
    }
}
