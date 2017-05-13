using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogInLogOut.Models;

namespace LogInLogOut.Controllers
{
    public class LogInController : Controller
    {
        // GET: LogIn
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(LogInLogOut.Models.AdminUser userModel)
        {
            using (BloodDonorDBEntities db = new BloodDonorDBEntities())
            {
                var userDetails = db.AdminUsers.Where(x => x.admin_username == userModel.admin_username && x.admin_password == userModel.admin_password).FirstOrDefault();
                if (userDetails==null)
                {
                    userModel.LoginErrorMessage = "Wrong username or paswword";
                    return View("Index", userModel);
                }
                else
                {
                    Session["admin_id"] = userDetails.admin_id;
                    Session["admin_username"] = userDetails.admin_username;
                    return RedirectToAction("Dashboard", "Home");
                }
            }
        }
         
        public ActionResult Logout()
        {
            int adminID = (int)Session["admin_id"];
            Session.Abandon();
            return RedirectToAction("Index", "LogIn");
        }
    }
}