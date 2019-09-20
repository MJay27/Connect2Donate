using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Connect2Donate.Models;

namespace Connect2Donate.Email
{
    public class Email
    {
       
        public static void BuildEmailTemplate(string regEmail, string sysEmail, string sysPassword)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplet/" + "EmailResetPasswordBody" + ".cshtml"));

            var url = "http://localhost:28871/" + "ResetPassword/ResetPasswordFromEmail?regEmail=" + regEmail;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Password Change Request", body, regEmail, sysEmail, sysPassword);
        }
        public static void BuildEmailTemplate(int regId, string regEmail, string sysEmail, string sysPassword)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplet/" + "EmailBody" + ".cshtml"));
            
            var url = "http://localhost:28871/" + "UserRegistration/Confirm?regId=" + regId;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your account is successfully created!", body, regEmail, sysEmail, sysPassword);
        }

        public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo, string sysEmail, string sysPassowrd)
        {
            string from, to, subject, bcc, cc, body;
            
            from = sysEmail;
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
            SendEmail(mail,sysEmail,sysPassowrd);
        }

        private static void SendEmail(MailMessage mail, string sysEmail, string sysPassword)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            if (sysEmail != null && sysPassword != null)
            {
                client.Credentials = new System.Net.NetworkCredential(sysEmail, sysPassword);
                try
                {
                    client.Send(mail);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
      
    }
}