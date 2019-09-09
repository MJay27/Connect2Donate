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

namespace Connect2Donate.Controllers
{
    public class TblRequestsController : Controller
    {
        private C2DConetxt db = new C2DConetxt();

        // GET: TblRequests
        public async Task<ActionResult> Index()
        {
            var tblRequests = db.TblRequests.Include(t => t.TblUser);
            return View(await tblRequests.ToListAsync());
        }

        // GET: TblRequests/Details/5
        public async Task<ActionResult> Details(int? id)
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
            return View(tblRequest);
        }

        // GET: TblRequests/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.TblUsers, "UserId", "Name");
            return View();
        }

        // POST: TblRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RequestId,Title,Description,Status,UserId")] TblRequest tblRequest)
        {
            if (ModelState.IsValid)
            {
                db.TblRequests.Add(tblRequest);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.TblUsers, "UserId", "Name", tblRequest.UserId);
            return View(tblRequest);
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
            ViewBag.UserId = new SelectList(db.TblUsers, "UserId", "Name", tblRequest.UserId);
            return View(tblRequest);
        }

        // POST: TblRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RequestId,Title,Description,Status,UserId")] TblRequest tblRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblRequest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.TblUsers, "UserId", "Name", tblRequest.UserId);
            return View(tblRequest);
        }

        // GET: TblRequests/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            return View(tblRequest);
        }

        // POST: TblRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TblRequest tblRequest = await db.TblRequests.FindAsync(id);
            db.TblRequests.Remove(tblRequest);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
