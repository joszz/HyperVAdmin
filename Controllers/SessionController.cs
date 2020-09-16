using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class SessionController : BaseController
    {
        /// <summary>
        /// Shows the login form. Sets the model to false, indicating no validation errors.
        /// </summary>
        /// <returns>The view with the login form. </returns>
        public ActionResult Index()
        {
            return View(false);
        }

        /// <summary>
        /// Handles the login form post with the received password. Checks for validity and either redirects to Index or
        /// shows the view again with the modal set to true, indicating validation errors.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string password)
        {
            if (VerifyPasswordHash(password))
            {
                Session["loggedin"] = true;
                return RedirectToAction("Index", "Index");
            }

            return View(true);
        }

        /// <summary>
        /// Clears the session, logging the user out and redirects to Index of the session controller.
        /// </summary>
        /// <returns>Redirects to action Index.</returns>
        public ActionResult SignOut()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Checks the provided password against the hashed and salted password in the appsettings.
        /// </summary>
        /// <param name="password">The user provided password to check.</param>
        /// <returns>Whether or not the password was verified successfully</returns>
        private bool VerifyPasswordHash(string password)
        {
            if (ConfigurationManager.AppSettings["Password"] != null && ConfigurationManager.AppSettings["PasswordSalt"] != null)
            {
                byte[] salt = Convert.FromBase64String(ConfigurationManager.AppSettings["PasswordSalt"]);
                byte[] passwordHash = Convert.FromBase64String(ConfigurationManager.AppSettings["Password"]);

                using (var hmac = new HMACSHA512(salt))
                {
                    byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != passwordHash[i])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public ActionResult Manifest()
        {
            HttpContext.Response.Headers["Content-Type"] = "application/manifest+json; charset=UTF-8";
            return View();
        }

        public ActionResult ServiceWorker()
        {
            HttpContext.Response.Headers["Service-Worker-allowed"] = "../";
            return File("~/Content/Scripts/worker.min.js", "text/javascript");
        }
    }
}
