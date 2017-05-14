using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogInLogOut.Models;
using System.Web.Security;

namespace LogInLogOut.Controllers
{
    public class LogInAdminController : Controller
    {
     
        //LogIn
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //LogIn POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authorize(LogInLogOut.Models.AdminLogin userModel)
        {
            using (BloodDonorDBEntities db = new BloodDonorDBEntities())
            {
                var userDetails = db.AdminUsers.Where(x => x.AdminEmail == userModel.AdminEmail && x.AdminPassword == userModel.AdminPassword).FirstOrDefault();
                if (userDetails==null)
                {
                    userModel.LoginErrorMessage = "Wrong username or paswword";
                    return View("Index", userModel);
                }
                else
                {
                    Session["admin_id"] = userDetails.admin_id;
                    Session["AdminEmail"] = userDetails.AdminEmail;
                    return RedirectToAction("Dashboard", "Home");
                }
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            int adminID = (int)Session["admin_id"];
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Authorize", "LogIn");
        }
    }
}