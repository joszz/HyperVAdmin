﻿using HyperVAdmin.Attributes;
using HyperVAdmin.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    /// <summary>
    /// Controller to manage all Sites related actions.
    /// </summary>
    public class SitesController : BaseController
    {
        /// <summary>
        /// Check if IIS is enabled in web.config, if not throw exception.
        /// </summary>
        public SitesController() : base()
        {
            if (!iisEnabled)
            {
                throw new UnauthorizedAccessException("IIS is not enabled!");
            }
        }

        /// <summary>
        /// Shows a list of all Sites and their related actions.
        /// </summary>
        /// <returns>The view showing all VMs</returns>
        public ActionResult Index()
        {
            return View(SiteModel.GetSites());
        }

        /// <summary>
        /// Gets a JSON array with all Sites. Called by AJAX to refresh content.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSites()
        {
            return Json(SiteModel.GetSites(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Stops a site. Sitename is used to determine which site to stop.
        /// </summary>
        /// <param name="sitename">Which site to stop.</param>
        public void StopSite(string sitename)
        {
            SiteModel.StopSite(HttpUtility.UrlDecode(sitename));
        }

        /// <summary>
        /// Starts a site. Sitename is used to determine which site to start.
        /// </summary>
        /// <param name="sitename">Which site to start.</param>
        public void StartSite(string sitename)
        {
            SiteModel.StartSite(HttpUtility.UrlDecode(sitename));
        }

        /// <summary>
        /// Shows all the applications defined for a website. Used within Fancybox (iFrame).
        /// </summary>
        /// <param name="sitename">The site to show the applications for.</param>
        /// <returns>The view</returns>
        public ActionResult Applications(string sitename)
        {
            if(sitename == null)
            {
                throw new Exception("No sitename defined!");
            }

            SiteModel model = SiteModel.GetSites().FirstOrDefault(site => site.Name.ToLower() == sitename.ToLower());

            if(model == null)
            {
                throw new Exception("Site \"" + sitename + "\" not defined!");
            }

            return View(model);
        }
    }
}