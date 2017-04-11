using System.Diagnostics;
using System.Web;

namespace HyperVAdmin.Modules
{
    /// <summary>
    /// A HTTPModule to keep trak of duration of page lifecycle
    /// </summary>
    public class TimingModule : IHttpModule
    {
        /// <summary>
        /// Disposes this module
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Bind BeginRequest eventhandler
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        /// <summary>
        /// Start a stopwatch at the beginning of the request to calculate duration of lifecycle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnBeginRequest(object sender, System.EventArgs e)
        {
            var stopwatch = new Stopwatch();
            HttpContext.Current.Items["Stopwatch"] = stopwatch;
            stopwatch.Start();
        }
    }
}