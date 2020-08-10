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

                "~/Content/Scripts/jquery.isloading.js",
                "~/Content/Scripts/jquery.vibrate.js",
                "~/Content/Scripts/sorttable.js",
                "~/Content/Scripts/Waves/waves.js",
                "~/Content/Scripts/general.js"
            ));

            bundles.Add(new StyleBundle("~/Content/Styles/bundle").Include(
                "~/Content/Vendor/fancybox/jquery.fancybox.css",
                "~/Content/Styles/Waves/waves.css",
                "~/Content/Styles/Default.css"
            ));
        }
    }
}