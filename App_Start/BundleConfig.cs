using System.Web.Optimization;

namespace HyperVAdmin
{
#pragma warning disable 1591
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Content/Scripts/jQuery/jquery-{version}.js",
                "~/Content/Scripts/Bootstrap/bootstrap.js",
                "~/Content/Scripts/jquery.isloading.js",
                "~/Content/Scripts/jquery.vibrate.js",
                "~/Content/Scripts/sorttable.js",
                "~/Content/Scripts/waves.js",
                "~/Content/Scripts/general.js"
            ));

            bundles.Add(new StyleBundle("~/Content/Styles/bundle").Include(
                "~/Content/Styles/Bootstrap/bootstrap.css",
                "~/Content/Styles/Bootstrap/bootstrap-theme.css",
                "~/Content/Styles/waves.css",
                "~/Content/Styles/Default.css"
            ));
        }
    }
}