using HyperVAdmin.Utilities;
using System.Web;
using System.Web.Mvc;

namespace HyperVAdmin.Attributes
{
    public class CustomAuthentication : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return UserConfig.UsersCollection.Count == 0 || httpContext.Session["user"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Session");
        }
    }
}