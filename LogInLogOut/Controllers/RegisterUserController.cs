using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogInLogOut.Models;
using System.Net.Mail;
using System.Net;
using System.Web.Security;

namespace LogInLogOut.Controllers
{
    public class RegisterUserController : Controller
    {
        //Add User action
        [HttpGet]
        public ActionResult AddOrEdit(int id=0)
        {
           Staff userModel = new Staff();
            return View(userModel);
        }
        
        //Add User POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit([Bind(Exclude = "IsEmailVerified, ActivationCode")] Staff userModel)
        {
            bool Status = false;
            string message = "";

            //Model Validation
            if (ModelState.IsValid)
            {
                #region//Email already exists
                var IsExist = IsEmailExist(userModel.StaffEmail);
                if (IsExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exists");
                    return View(userModel);
                }
                #endregion

                #region //Generate activation code
                  userModel.ActivationCode = Guid.NewGuid();
                  #endregion

                #region //Password hashing
                userModel.StaffPassword = Crypto.Hash(userModel.StaffPassword);
                userModel.ConfirmPassword = Crypto.Hash(userModel.ConfirmPassword);
                #endregion
                userModel.IsEmailVerified = false;

                #region //Save data to DB
                using (BloodDonorDBEntities db = new BloodDonorDBEntities())
                {
                   /* if (db.Staffs.Any(x => x.staff_username == userModel.staff_username))
                    {
                        ViewBag.DuplicateMessage = "Username already exists";
                        return View("AddOrEdit", userModel);
                    }*/
                    db.Staffs.Add(userModel);
                    db.SaveChanges();

                    //Send email to user
                    SendVerificationLinkEmail(userModel.StaffEmail, userModel.ActivationCode.ToString());
                    message = "Registration successful. Account activation link" + "has been sent to your email address:" + userModel.StaffEmail;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(userModel);

          /*
            ModelState.Clear();
            ViewBag.SuccessMessage = "User created successfuly";
            return View("AddOrEdit", new Staff());*/
        }

        //Verify account
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (BloodDonorDBEntities db= new BloodDonorDBEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false; // added to avoid confirm password does not match issue on save changes
                var v = db.Staffs.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    db.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
                ViewBag.Status = Status;
                return View();
        }

        //LogIn
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        //LogIn POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(StaffLogin login, string ReturnUrl="")
        {
            string message = "";
            using (BloodDonorDBEntities db = new BloodDonorDBEntities())
            {
                var v = db.Staffs.Where(a => a.StaffEmail == login.StaffEmail).FirstOrDefault();
                if (v!=null)
                {
                    if (string.Compare(Crypto.Hash(login.StaffPassword), v.StaffPassword)==0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; //525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.StaffEmail, login.RememberMe, timeout);
                        string encrypt = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypt);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    else
                    {
                        message = "Invalid credentials provided";
                    }
                }
                else
                {
                    message = "Invalid credentials provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        //LogOut
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login","RegisterUser"); //redirect to action login in ResiterUser controller
        }

        [NonAction]
        public bool IsEmailExist(string staffEmail)
        {
            using (BloodDonorDBEntities db = new BloodDonorDBEntities())
            {
                var v = db.Staffs.Where(a => a.StaffEmail == staffEmail).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        protected void SendVerificationLinkEmail(string staffEmail, string activationCode)
        {
            var verifyUrl = "/RegisterUser/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("pop.irina1@gmail.com");
            var toEmail = new MailAddress(staffEmail);
            var fromEmailPassword = "Frunzulitza.1"; //Replace with actual password
            string subject = "Your account is succesfully created";

            string body = "</br></br>We are excited to tell you that your account is" + "successfully created. Please click on the link below to verify your account" + "</br></br> <a href='"+link+"'>"+link+"</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }) 
                smtp.Send(message);
        }
    }
}