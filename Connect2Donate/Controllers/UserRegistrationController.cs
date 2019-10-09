using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Connect2Donate.Models;
using System.Web.Hosting;
using System.Text;
using System.Net.Mail;
using System.Security.Cryptography;
using Connect2Donate.SecurePassword;
using Connect2Donate.Email;

namespace Connect2Donate.Controllers
{
    public class UserRegistrationController : Controller
    {
        private C2DContext db = new C2DContext();

        // GET: UserRegistration
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegistrationDataModel registrationDataModel)
        {
            if (ModelState.IsValid)
            {
                var emails = from data in db.TblUsers select data.Email;
                foreach (string email in emails)
                {
                    if (email.Equals(registrationDataModel.Email))
                    {

                        return Content("<script language='javascript' type='text/javascript'>alert('Email is already in use! User other Email');$.ajax({url: '/Home/Index',success: function(data) {alert(data);}});</script >");
                    }
                }
                TblUser tblUser = new TblUser();
                tblUser.Name = registrationDataModel.Name;
                tblUser.Email = registrationDataModel.Email;
                tblUser.UserType = registrationDataModel.UserType.ToString();
                tblUser.Password = registrationDataModel.Password;
                tblUser.ValidateEmail = false;

                List<string> encryptedPasswordAndSalt = Password.Ecrypt(tblUser.Password);
                tblUser.Salt = encryptedPasswordAndSalt[0];
                tblUser.Hash = encryptedPasswordAndSalt[1];
                db.TblUsers.Add(tblUser);
                await db.SaveChangesAsync();

                int lastGeneratedId = tblUser.UserId;
                TblAddress tblAddress = new TblAddress();
                tblAddress.Line1 = registrationDataModel.Line1;
                tblAddress.Area = registrationDataModel.Area;
                tblAddress.Province = registrationDataModel.Province;
                tblAddress.PostalCode = registrationDataModel.PostalCode;
                tblAddress.UserId = lastGeneratedId;
                tblAddress.Country = "Canada";
                db.TblAddresses.Add(tblAddress);
                await db.SaveChangesAsync();

                TblContact tblContact = new TblContact();
                tblContact.Number = registrationDataModel.Number;
                tblContact.UserId = lastGeneratedId;
                db.TblContacts.Add(tblContact);
                await db.SaveChangesAsync();


                var sysData = from data in db.TblSysCredentials select data;
                //Send Confirmation EMail
                Email.Email.BuildEmailTemplate(tblUser.UserId, tblUser.Email, sysData.FirstOrDefault().Email, sysData.FirstOrDefault().Password);
                //BuildEmailTemplate(tblUser.UserId);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
       
        public ActionResult Confirm(int regId)
        {
            ViewBag.regId = regId;
            return View();
        }

        public JsonResult RegisterConfirm([Bind(Include = "UserType")]int regId)
        {

            var userData = from data in db.TblUsers where data.UserId.Equals(regId) select data;
            TblUser user = userData.FirstOrDefault();
            // db.Entry(user).State = EntityState.Modified;
            user.ValidateEmail = true;
            //db.TblUsers.Add(user);
            db.SaveChanges();
            var msg = "Your Email Is Verified!Visit Website For Accessing Your Account";
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
