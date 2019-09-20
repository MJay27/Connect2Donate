using Connect2Donate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Connect2Donate.Controllers
{
    public class RecipientController : Controller
    {
        C2DContext db = new C2DContext();
        // GET: Recipient
        public async Task<ActionResult> Index()
        {
            int userId = Convert.ToInt32(TempData["UserId"]);
            var tblRequests = from data in db.TblRequests.Include(t => t.TblUser) where data.UserId.Equals(userId) select data;
            return View(await tblRequests.ToListAsync());

        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RequestViewModel requestViewModel)
        {
            if (ModelState.IsValid)
            {
                TblRequest tblRequest = new TblRequest();
                tblRequest.Title = requestViewModel.Title;
                tblRequest.Description = requestViewModel.Description;
                tblRequest.Status = Convert.ToBoolean(requestViewModel.RequestStatus);
                tblRequest.UserId = Convert.ToInt32(TempData["UserId"]);
                db.TblRequests.Add(tblRequest);
                await db.SaveChangesAsync();
                return View("Index");
            }
            return View();
        }
        // GET: TblRequests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblRequest tblRequest = await db.TblRequests.FindAsync(id);
            if (tblRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.TblRequests, "UserId", "Title", tblRequest.UserId);
            return View(tblRequest);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RequestViewModel requestViewModel)
        {
            TblRequest tblRequest = new TblRequest();

            if (ModelState.IsValid)
            {
                db.Entry(tblRequest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tblRequest);
        }
    }
}