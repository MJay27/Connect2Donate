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
    public class UserAccountDetailsController : Controller
    {
        private C2DContext db = new C2DContext();

     

        // GET: UserAccountDetails/Details/5
        public async Task<ActionResult> Details()
        {
            RegistrationDataModel dataModel = new RegistrationDataModel();
            int id = Convert.ToInt32(Session["UserId"]);
            if (id.Equals(null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            dataModel.User = await db.TblUsers.FindAsync(id);
            var address = from data in db.TblAddresses where (data.UserId.Equals(id)) select data;
            dataModel.Address = address.FirstOrDefault();

            var contact = from data in db.TblContacts where (data.UserId.Equals(id)) select data;
            dataModel.Contact = contact.FirstOrDefault();
            if (dataModel == null)
            {
                return HttpNotFound();
            }
            return View(dataModel);
        }

        // GET: UserAccountDetails/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            RegistrationDataModel dataModel = new RegistrationDataModel();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dataModel.User = await db.TblUsers.FindAsync(id);

            var address = from data in db.TblAddresses where (data.UserId == id) select data;
            dataModel.Address = address.FirstOrDefault();

            var contact = from data in db.TblContacts where (data.UserId ==id ) select data;
            dataModel.Contact = contact.FirstOrDefault();
            if (dataModel == null)
            {
                return HttpNotFound();
            }
            return View(dataModel);
        }

        // POST: UserAccountDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RegistrationDataModel dataModel)
        {
            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(Session["UserId"]);
                TblAddress address = (from x in db.TblAddresses
                              where x.UserId == id
                              select x).FirstOrDefault();
                address.Line1 = dataModel.Address.Line1;
                address.Area = dataModel.Address.Area;
                address.Province = dataModel.Address.Province;
                address.PostalCode = dataModel.Address.PostalCode;

                await db.SaveChangesAsync();
                TblContact contact = (from x in db.TblContacts
                                      where x.UserId == id
                                      select x).FirstOrDefault();
                contact.Number = dataModel.Contact.Number;
                await db.SaveChangesAsync();
                TblUser user = (from x in db.TblUsers
                                      where x.UserId == id
                                      select x).FirstOrDefault();
                user.Name = dataModel.User.Name;

                await db.SaveChangesAsync();
                return RedirectToAction("Details");
            }
            return View(dataModel);
        }

        // GET: UserAccountDetails/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TblUser tblUser = await db.TblUsers.FindAsync(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            return View(tblUser);
        }

        // POST: UserAccountDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TblUser tblUser = await db.TblUsers.FindAsync(id);
            db.TblUsers.Remove(tblUser);
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
