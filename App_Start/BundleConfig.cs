using System.Web;
using System.Web.Optimization;

namespace HyperVAdmin
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Content/Scripts/jQuery/jquery-{version}.js",
                "~/Content/Scripts/Bootstrap/bootstrap.js",
                "~/Content/Scripts/general.js"
            ));

            bundles.Add(new StyleBundle("~/Content/default").Include(
                "~/Content/Styles/Bootstrap/bootstrap.css",
                "~/Content/Styles/Bootstrap/bootstrap-theme.css",
                "~/Content/Styles/Default.css"
            ));
        }
    }
}