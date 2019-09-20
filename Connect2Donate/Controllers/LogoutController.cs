using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Connect2Donate.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            Session["UserId"] = null;
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }
    }
}