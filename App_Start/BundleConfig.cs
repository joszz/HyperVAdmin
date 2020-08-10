using System.Web.Optimization;

namespace HyperVAdmin
{
#pragma warning disable 1591
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Content/Vendor/jquery/jquery.js",
                "~/Content/Vendor/fancybox/jquery.fancybox.js",
                "~/Content/Vendor/jquery-fullscreen-plugin/jquery.fullscreen.js",
                "~/Content/Vendor/jquery.isloading/jquery.isloading.js",
                "~/Content/Vendor/jquery.vibrate/jquery.vibrate.js",
                "~/Content/Vendor/waves/waves.js",
                "~/Content/Vendor/sorttable/sorttable.js",
                "~/Content/Vendor/bootstrap/js/dist/util.js",
                "~/Content/Vendor/bootstrap/js/dist/collapse.js",

                "~/Content/Scripts/general.js"
            ));

            bundles.Add(new StyleBundle("~/Content/Styles/bundle").Include(
                "~/Content/Vendor/fancybox/jquery.fancybox.css",
                "~/Content/Vendor/waves/waves.css",
                "~/Content/Styles/Default.css"
            ));
        }
    }
}