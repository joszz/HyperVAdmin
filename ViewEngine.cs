using System.Web.Mvc;

namespace HyperVAdmin
{
    /// <summary>
    /// Custom viewengine, to optimize view resolving.
    /// </summary>
    public class ViewEngine : RazorViewEngine
    {
        /// <summary>
        /// The cosntructor overrides the default view locations and extensions to minimize the paths to look in 
        /// when MVC needs to find a view.
        /// </summary>
        public ViewEngine()
        {
            ViewLocationFormats = new string[] { "~/Views/{1}/{0}.cshtml" };
            MasterLocationFormats = new string[] { "~/Views/Shared/{0}.cshtml" };
            PartialViewLocationFormats = new string[] { "~/Views/{1}/{0}.cshtml", "~/Views/Shared/{0}.cshtml" };
            FileExtensions = new string[] { "cshtml" };
        }
    }
}