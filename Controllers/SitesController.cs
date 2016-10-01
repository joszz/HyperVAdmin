using Microsoft.Web.Administration;
using System.Web.Mvc;
using System.Linq;
using HyperVAdmin.Models;
using System;
using System.Collections.Generic;

namespace HyperVAdmin.Controllers
{
    public class SitesController : Controller
    {
        private ServerManager manager = new ServerManager();

        public ActionResult Index()
        {
            return View(SiteModel.GetSites());
        }

        [HttpPost]
        public JsonResult GetSites()
        {
            return Json(SiteModel.GetSites());
        }

        [HttpPost]
        public void StopSite(string sitename)
        {
            Site site = manager.Sites.Where(s => s.Name == sitename).FirstOrDefault();

            if (site != null)
            {
                site.Stop();
            }
        }

        [HttpPost]
        public void StartSite(string sitename)
        {
            Site site = manager.Sites.Where(s => s.Name == sitename).FirstOrDefault();

            if (site != null)
            {
                site.Start();
            }
        }
    }
}