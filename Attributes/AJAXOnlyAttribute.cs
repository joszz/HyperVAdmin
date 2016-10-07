using System.Reflection;
using System.Web.Mvc;

namespace HyperVAdmin.Attributes
{
    /// <summary>
    /// Custom attribute to decorate functions with. Only allows AJAX requests on actions decorated with [AJAXOnly].
    /// </summary>
    public class AJAXOnlyAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// Check whether the request is an AJAX request or not/
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="methodInfo"></param>
        /// <returns>Boolean indicating if current request is an AJAX request</returns>
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}