using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperVAdmin.Models
{
    /// <summary>
    /// The Model representing a site.
    /// </summary>
    public class SiteModel
    {
        /// <summary>
        /// IIS ServerManager used to retrieve sites and info.
        /// </summary>
        public static ServerManager Manager = new ServerManager();

        /// <summary>
        /// The name of the Site.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The fullpath of the site's location.
        /// </summary>
        public string PhysicalPath { get; set; }

        private Dictionary<string, string> _bindings = new Dictionary<string, string>();
        /// <summary>
        /// The bindings of the Site.
        /// </summary>
        public Dictionary<string, string> Bindings
        {
            get
            {
                return _bindings;
            }
        }

        /// <summary>
        /// The current state of the Site, such as stopped, started etc.
        /// </summary>
        public ObjectState State { get; set; }

        /// <summary>
        /// The web applications defined for a site.
        /// </summary>
        public List<Application> Applications { get; set; }

        /// <summary>
        /// Retrieves a list of SiteModels.
        /// </summary>
        /// <returns>A list of SiteModels</returns>
        public static List<SiteModel> GetSites()
        {
            ServerManager manager = new ServerManager();
            List<SiteModel> models = new List<SiteModel>();

            foreach (Site site in manager.Sites)
            {
                SiteModel model = new SiteModel
                {
                    Name = site.Name,
                    PhysicalPath = site.Applications[0].VirtualDirectories[0].PhysicalPath,
                    State = site.State,
                    Applications = site.Applications.Where(app => app.Path != "/").ToList()
                };

                foreach (Binding binding in site.Bindings.OrderBy(b => b.Protocol))
                {
                    if (binding.Protocol.ToLowerInvariant() == "http" || binding.Protocol.ToLowerInvariant() == "https")
                    {
                        string url = binding.Protocol + "://" + (binding.Host != string.Empty ? binding.Host : Environment.MachineName);

                        if (binding.EndPoint != null &&
                            !(binding.EndPoint.Port == 80 && binding.Protocol == "http") &&
                            !(binding.EndPoint.Port == 443 && binding.Protocol == "https"))
                        {
                            url += ":" + binding.EndPoint.Port;
                        }

                        if (!model.Bindings.ContainsKey(binding.Protocol))
                        {
                            model.Bindings.Add(binding.Protocol, url);
                        }
                    }
                }

                models.Add(model);
            }

            models = models.OrderBy(s => s.Name).ToList();

            return models;
        }

        /// <summary>
        /// Stops an IIS website.
        /// </summary>
        /// <param name="sitename">The site to stop.</param>
        public static void StopSite(string sitename)
        {
            Site site = Manager.Sites.FirstOrDefault(s => s.Name == sitename);

            if (site != null)
            {
                site.Stop();
            }
        }

        /// <summary>
        /// Starts an IIS website.
        /// </summary>
        /// <param name="sitename">The site to start.</param>
        public static void StartSite(string sitename)
        {
            Site site = Manager.Sites.FirstOrDefault(s => s.Name == sitename);

            if (site != null)
            {
                site.Start();
            }
        }
    }
}