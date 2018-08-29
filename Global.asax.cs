using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HyperVAdmin
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

#pragma warning disable 1591
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Removing all the view engines
            ViewEngines.Engines.Clear();

            //Add Razor Engine (which we are using)
            ViewEngines.Engines.Add(new ViewEngine());

            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_EndRequest()
        {
            // removing excessive headers. They don't need to see this.
            Response.Headers.Remove("Server");
        }
    }
#pragma warning restore 1591
}