using System;
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
        C2DConetxt db = new C2DConetxt();
        // GET: ResetPassword
        public ActionResult Index()
        {
           
                return View();
        }

        //POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TblUser tblUser)
        {
            if (ModelState.IsValid)
            {
                var email = from data in db.TblUsers where data.Email.Equals(tblUser.Email) select data.Email;
                if (Session["UserId"] == null && email.FirstOrDefault() !=null)
                {
                    Email.Email.BuildEmailTemplate(email.FirstOrDefault());
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
                        return RedirectToAction("Index","Home");
                    }
                    else
                    { 
                        //Email and userId not matched
                        //Entered wrong or some other user's email

                        }
              
                }
            }

            return View(tblUser);
        }

        public ActionResult ResetPasswordFromEmail(string email)
        {
            ViewBag.email = email;
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> ResetPasswordFromEmail(string email,string enteredPassword)
        {

            TblUser user = db.TblUsers.Where(x => x.Email == email).FirstOrDefault();
            List<string> encryptedPasswordAndSalt = Password.Ecrypt(enteredPassword);
            user.Salt = encryptedPasswordAndSalt[0];
            user.Hash = encryptedPasswordAndSalt[1];
            db.Entry(user).State = EntityState.Modified;
            user.ValidateEmail = true;
            await db.SaveChangesAsync();

            var msg = "Your Password Is Successfully Changed!";
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
    }
}