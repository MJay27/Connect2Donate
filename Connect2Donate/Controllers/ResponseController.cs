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
    public class ResponseController : Controller
    {
        private C2DConetxt db = new C2DConetxt();

        // GET: TblRespons
        public async Task<ActionResult> Index(string search)
        {
            //var tblResponses = db.TblResponses.Include(t => t.TblRequest).Include(t => t.TblUser);
            var tblRequests = db.TblRequests.Include(t => t.TblUser).Where(t=> t.Title.Contains(search) || search == null);
            
            return View(await tblRequests.ToListAsync());
        }
        // Overload
        public async Task<ActionResult> Index_Search(string text)
        {
            string txt = text;
            var tblRequests = Search(text);
            if(tblRequests!=null)
                return View("Index",await tblRequests);
            return View("Index");
        }

        // GET: TblRespons/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblRespons tblRespons = await db.TblResponses.FindAsync(id);
            if (tblRespons == null)
            {
                return HttpNotFound();
            }
            return View(tblRespons);
        }

        // GET: TblRespons/Create
        public ActionResult Create()
        {
            ViewBag.RequestId = new SelectList(db.TblRequests, "RequestId", "Title");
            ViewBag.UserId = new SelectList(db.TblUsers, "UserId", "Name");
            return View();
        }

        // POST: TblRespons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ResponseId,RequestId,UserId,Comment")] TblRespons tblRespons)
        {
            if (ModelState.IsValid)
            {
                db.TblResponses.Add(tblRespons);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.RequestId = new SelectList(db.TblRequests, "RequestId", "Title", tblRespons.RequestId);
            ViewBag.UserId = new SelectList(db.TblUsers, "UserId", "Name", tblRespons.UserId);
            return View(tblRespons);
        }

        // GET: TblRespons/Edit/5
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
            Session["RequestId"] = id;
            
          
            return View();
        }

        // POST: TblRespons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Comment")] TblRespons tblRespons)
        {
            if (ModelState.IsValid)
            {
                if (Session["RequestId"] != null && Session["UserId"] != null)
                {
                    tblRespons.RequestId = Convert.ToInt32(Session["RequestId"]);
                    tblRespons.UserId = Convert.ToInt32(Session["UserId"]);
                    db.TblResponses.Add(tblRespons);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View();
            }
            return View("Index",tblRespons);
        }

        // GET: TblRespons/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblRespons tblRespons = await db.TblResponses.FindAsync(id);
            if (tblRespons == null)
            {
                return HttpNotFound();
            }
            return View(tblRespons);
        }

        // POST: TblRespons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TblRespons tblRespons = await db.TblResponses.FindAsync(id);
            db.TblResponses.Remove(tblRespons);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private async Task<List<TblRequest>> Search(string text)
        {
            var searchedResult = from data in db.TblRequests where data.Title.Contains(text) select data;
            if (searchedResult.Any())
            {
                List<TblRequest> list = await searchedResult.ToListAsync();
                return (list);
            }
            return null;

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
