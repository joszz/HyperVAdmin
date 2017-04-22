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
                "~/Content/Scripts/Fancybox/jquery.fancybox.js",
                "~/Content/Scripts/jquery.isloading.js",
                "~/Content/Scripts/jquery.vibrate.js",
                "~/Content/Scripts/jquery.fullscreen.js",
                "~/Content/Scripts/sorttable.js",
                "~/Content/Scripts/Waves/waves.js",
                "~/Content/Scripts/general.js"
            ));

            bundles.Add(new StyleBundle("~/Content/Styles/bundle").Include(
                "~/Content/Styles/Bootstrap.css",
                "~/Content/Styles/Fancybox/jquery.fancybox.css",
                "~/Content/Styles/FontAwesome.css",
                "~/Content/Styles/Waves/waves.css",
                "~/Content/Styles/Default.css"
            ));
        }
    }
}