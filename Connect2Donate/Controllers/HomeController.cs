using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Connect2Donate.Models;
using Connect2Donate.SecurePassword;

namespace Connect2Donate.Controllers
{
    public class HomeController : Controller
    {
        C2DContext db = new C2DContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public ActionResult Authenticate(FormCollection formCollection)
        {
            string enteredEmail = formCollection["Email"];
            bool passwordMatched;
            byte[] storedHashBytes;
            byte[] storedSaltBytes = new byte[16];
            var userEmails = from data in db.TblUsers select data.Email;

            if (enteredEmail == "" || formCollection["Password"] == "")
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Email and Password must not be empty!')</script >");
            }
            foreach (string email in userEmails)
            {
                if (email.Equals(enteredEmail))
                {
                    var user = from data in db.TblUsers where data.Email.Equals(email) select data;
                    var sysData = from data in db.TblSysCredentials select data;
                    if (!user.FirstOrDefault().ValidateEmail)
                    {
                        Email.Email.BuildEmailTemplate(user.FirstOrDefault().UserId, user.FirstOrDefault().Email, sysData.FirstOrDefault().Email, sysData.FirstOrDefault().Password);
                        return Content("<script language='javascript' type='text/javascript'>alert('Account is not verified!Please verify your account from your email.')</script >");
                    }
                    else
                    {
                        var userData = from data in db.TblUsers
                                       where data.Email.Equals(email)
                                       select data;
                        string enteredPassword = formCollection["password"];
                        storedHashBytes = Convert.FromBase64String(userData.FirstOrDefault().Hash);
                        storedSaltBytes = Convert.FromBase64String(userData.FirstOrDefault().Salt);
                        passwordMatched = Password.ComparePassword(enteredPassword, storedHashBytes, storedSaltBytes);

                        if (passwordMatched == true)
                        {
                            if (userData.FirstOrDefault().UserType.Equals("Donor"))
                            {
                                Session["UserId"] = userData.FirstOrDefault().UserId;

                                if (Session["UserId"] != null)
                                    return RedirectToAction("Index", "Response");
                            }
                            else
                            {
                                Session["UserId"] = userData.FirstOrDefault().UserId;

                                if (Session["UserId"] != null)
                                    return RedirectToAction("Index", "Request");
                            }
                        }
                        else
                        {
                            return Content("<script language='javascript' type='text/javascript'>alert('Email or Password does not match!')</script >");
                        }
                    }
                }

            }
            return Content("<script language='javascript' type='text/javascript'>alert('You must register first!')</script >");
        }
    }
}