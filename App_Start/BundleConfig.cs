using System.Configuration;
using System.Web;
using System.Web.Optimization;

namespace HyperVAdmin
{
    public class BundleConfig
    {
        private static readonly string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        public static void RegisterBundles(BundleCollection bundles)
        {
            Styles.DefaultTagFormat = "<link href=\"" + BaseUrl + "{0}\" rel=\"stylesheet\"/>";
            Scripts.DefaultTagFormat = "<script src=\"" + BaseUrl + "{0}\"></script>";

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Content/Scripts/jQuery/jquery-{version}.js",
                "~/Content/Scripts/Bootstrap/bootstrap.js",
                "~/Content/Scripts/jquery.isloading.js",
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