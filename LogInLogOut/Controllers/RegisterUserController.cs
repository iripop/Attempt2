using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LogInLogOut.Models;
using System.Net.Mail;
using System.Net;

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
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
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