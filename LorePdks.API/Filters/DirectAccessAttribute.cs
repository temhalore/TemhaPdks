using Microsoft.AspNetCore.Mvc.Filters;
using LorePdks.BAL.Managers.Helper.Interfaces;

namespace LorePdks.API.Filters
{
    public class DirectAccessAttribute : ActionFilterAttribute
    {
        private IHelperManager _helperManager;
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {

        }

        public override void OnActionExecuting(ActionExecutingContext actionExecutedContext)
        {
            if (_helperManager == null)
            {
                _helperManager = (IHelperManager)actionExecutedContext.HttpContext.RequestServices.GetService(typeof(IHelperManager));
            }
        }
    }
}
