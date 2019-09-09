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
        private C2DConetxt db = new C2DConetxt();

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

                //Send Confirmation EMail
                Email.Email.BuildEmailTemplate(tblUser.UserId, tblUser.Email);
                //BuildEmailTemplate(tblUser.UserId);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        private void BuildEmailTemplate(int regId)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplet/" + "EmailBody" + ".cshtml"));
            var regInfo = db.TblUsers.Where(x => x.UserId == regId).FirstOrDefault();
            var url = "http://localhost:28871/" + "UserRegistration/Confirm?regId=" + regId;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your account is successfully created!", body, regInfo.Email);
        }

        public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, subject, bcc, cc, body;
            from = "mjay2911@gmail.com";
            to = sendTo.Trim();
            subject = subjectText;
            bcc = "";
            cc = "";
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.To.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.To.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        private static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("mjay2911@gmail.com", "Jexumgmail2796$");
            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ActionResult Confirm(int regId)
        {
            ViewBag.regId = regId;
            return View();
        }

        public async Task<JsonResult> RegisterConfirm([Bind(Include = "UserType")]int regId)
        {

            TblUser user = db.TblUsers.Where(x => x.UserId == regId).FirstOrDefault();
            db.Entry(user).State = EntityState.Modified;
            user.ValidateEmail = true;
            await db.SaveChangesAsync();

            var msg = "Your Email Is Verified!Visit Website For Accessing Your Account";
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        // Password Encryption
        public List<string> EcryptPassword(string plaintextpassword)
        {
            byte[] salt = new byte[16];

            /*Generating Salt*/
            try
            {
                using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
                {
                    csprng.GetBytes(salt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Random number generator not available.",
                    ex
                );
            }

            /*Generating Hash*/
            var pbkdf2 = new Rfc2898DeriveBytes(plaintextpassword, salt, 10000);

            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            List<string> saltAndHash = new List<string>();
            saltAndHash.Add(Convert.ToBase64String(salt));
            saltAndHash.Add(Convert.ToBase64String(hashBytes));
            return saltAndHash;

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
