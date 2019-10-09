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
    public class RequestController : Controller
    {
        private C2DContext db = new C2DContext();

        // GET: Request
        public async Task<ActionResult> Index()
        {
            if (Session["UserId"] != null)
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                var tblRequests = from data in db.TblRequests.Include(t => t.TblUser)
                                  where data.UserId.Equals(userId)
                                  select data;
                return View(await tblRequests.ToListAsync());
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Request/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequestViewModel viewModel = new RequestViewModel();
            var request = await db.TblRequests.FindAsync(id);
            viewModel.RequestId = request.RequestId;
            viewModel.Title = request.Title;
            viewModel.Description = request.Description;

            var list = from data in db.TblResponses where data.RequestId.Equals(request.RequestId) select data;
            // ViewBag.DonorName = from names in db.TblUsers where names.UserId.Equals();
            viewModel.Responses = await list.ToListAsync();



            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        // GET: Request/Create
        public ActionResult Create()
        {
            // ViewBag.UserId = new SelectList(db.TblUsers, "UserId", "Name");
            return View();
        }

        // POST: Request/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Title,Description,Status")] TblRequest tblRequest)
        {
            if (ModelState.IsValid)
            {
                tblRequest.UserId = Convert.ToInt32(Session["UserId"]);
                db.TblRequests.Add(tblRequest);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.TblUsers, "UserId", "Name", tblRequest.UserId);
            return View(tblRequest);
        }

        // GET: Request/Edit/5
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

            return View(tblRequest);
        }

        // POST: Request/Edit/5
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

            return View(tblRequest);
        }

        // GET: Request/Delete/5
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

        // POST: Request/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var responsesList = from responses in db.TblResponses where responses.RequestId.Equals(id) select responses;
            RequestViewModel requestViewModel = new RequestViewModel();
            requestViewModel.Responses = await responsesList.ToListAsync();
            foreach (TblRespons response in requestViewModel.Responses)
            {
                db.TblResponses.Remove(response);
            }
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
