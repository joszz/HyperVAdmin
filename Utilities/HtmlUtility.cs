using System.Web.Hosting;

namespace HyperVAdmin.Utilities
{
    /// <summary>
    /// A utility class to help with HTML related tasks.
    /// </summary>
    public static class HtmlUtility
    {
        /// <summary>
        /// Retrieves the VirtualDirectory/Path (also for "IIS Applications") when not hosted in the root of the domain
        /// </summary>
        /// <returns>Either the VirtualDirectory/Path (with trailing slash) or "/" when hosted in the domain</returns>
        public static string GetVirtualApplicationPath()
        {
            string path = HostingEnvironment.ApplicationVirtualPath;

            if (path.Substring(path.Length - 1) != "/")
            {
                path += "/";
            }

            return path;
        }
    }
}