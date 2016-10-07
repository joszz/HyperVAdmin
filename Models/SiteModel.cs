﻿using Microsoft.Web.Administration;
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
                    State = site.State
                };

                foreach (Binding binding in site.Bindings.OrderBy(b => b.Protocol))
                {
                    string url = binding.Protocol + "://" + Environment.MachineName;

                    if (!(binding.EndPoint.Port == 80 && binding.Protocol == "http") && !(binding.EndPoint.Port == 443 && binding.Protocol == "https"))
                    {
                        url += ":" + binding.EndPoint.Port;
                    }

                    model.Bindings.Add(binding.Protocol, url);

                }

                models.Add(model);
            }

            return models;
        }
    }
}