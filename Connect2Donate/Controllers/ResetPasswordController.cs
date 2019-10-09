﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Connect2Donate.Models;
using Connect2Donate.SecurePassword;
using Connect2Donate.Email;
namespace Connect2Donate.Controllers
{
    public class ResetPasswordController : Controller
    {
        C2DContext db = new C2DContext();
        // GET: ResetPassword
        public ActionResult Index()
        {

            return View();
        }

        //POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Exclude = "UserId,Name,Hash,Salt,UserType,Password")]TblUser tblUser)
        {

            var email = from data in db.TblUsers where data.Email.Equals(tblUser.Email) select data.Email;
            var sysData = from data in db.TblSysCredentials select data;
            if (Session["UserId"] == null && email.FirstOrDefault() != null)
            {
                Email.Email.BuildEmailTemplate(email.FirstOrDefault(), sysData.FirstOrDefault().Email, sysData.FirstOrDefault().Password);
                ViewBag.SendEmail = "EmailSent";
                return RedirectToAction("ForgetPassword");
            }
            else
            {
                List<string> encryptedPasswordAndSalt = Password.Ecrypt(tblUser.Password);
                TblUser user = (from x in db.TblUsers
                                where x.Email == tblUser.Email
                                select x).FirstOrDefault();
                if (user.UserId == Convert.ToInt32(Session["UserId"]))
                {
                    user.Salt = encryptedPasswordAndSalt[0];
                    user.Hash = encryptedPasswordAndSalt[1];
                    await db.SaveChangesAsync();
                    Session["UserId"] = null;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Email and userId not matched
                    //Entered wrong or some other user's email
                    return Content("<script language='javascript' type='text/javascript'>alert('Email does not match with registered email!')</script >");

                }
            }
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }
        public ActionResult ResetPasswordFromEmail(string regEmail)
        {
            ViewBag.email = regEmail;
            Session["ForgetPWDEmail"] = regEmail;
            return View();
        }

        public async Task<JsonResult> ResetPasswordUsingEmail(string enteredPassword)
        {
            string email = Convert.ToString(Session["ForgetPWDEmail"]);

            var userData = from data in db.TblUsers where data.Email.Equals(email) select data;
            TblUser user = userData.FirstOrDefault();
            List<string> encryptedPasswordAndSalt = Password.Ecrypt(enteredPassword);
            user.Salt = encryptedPasswordAndSalt[0];
            user.Hash = encryptedPasswordAndSalt[1];
            await db.SaveChangesAsync();

            var msg = "Your Password Is Successfully Changed!";
            return Json(msg, JsonRequestBehavior.AllowGet);

        }
    }
}