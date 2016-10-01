using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperVAdmin.Models
{
    public class SiteModel
    {
        public string Name { get; set; }
        public string PhysicalPath { get; set; }

        private Dictionary<string, string> _bindings = new Dictionary<string, string>();
        public Dictionary<string, string> Bindings
        {
            get
            {
                return _bindings;
            }
        }

        public ObjectState State { get; set; }

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