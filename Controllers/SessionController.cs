using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace HyperVAdmin.Controllers
{
    public class SessionController : BaseController
    {
        public ActionResult Index()
        {
            return View(false);
        }

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
    }
}
