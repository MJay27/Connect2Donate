using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Connect2Donate.Models;

namespace Connect2Donate.Controllers
{
    public class DeactivateController : Controller
    {
        C2DContext db = new C2DContext();
        // GET: Deactivate
        public async Task<ActionResult> Index()
        {
            if (Session["UserId"] != null)
            {
                int id = Convert.ToInt32(Session["UserId"]);

                var responsesList = from responses in db.TblResponses where responses.UserId.Equals(id) select responses;
                RequestViewModel requestViewModel = new RequestViewModel();
                requestViewModel.Responses = await responsesList.ToListAsync();
                if (requestViewModel.Responses.Any())
                {
                    foreach (TblRespons response in requestViewModel.Responses)
                    {
                        db.TblResponses.Remove(response);
                    }
                }

                var requestsList = from requests in db.TblRequests where requests.UserId.Equals(id) select requests; 
                requestViewModel.Requests = await requestsList.ToListAsync();
                if (requestViewModel.Requests.Any())
                {
                    foreach (TblRequest requests in requestViewModel.Requests)
                    {
                        db.TblRequests.Remove(requests);
                    }
                }
                var add = from address in db.TblAddresses where address.UserId.Equals(id) select address;
                db.TblAddresses.Remove(add.FirstOrDefault())
;
                var contact = from phone in db.TblContacts where phone.UserId.Equals(id) select phone;
                db.TblContacts.Remove(contact.FirstOrDefault());

                var user = from data in db.TblUsers where data.UserId.Equals(id) select data;
                db.TblUsers.Remove(user.FirstOrDefault());

                await db.SaveChangesAsync();
                return RedirectToAction("Index","Logout");
            }
            return View();
        }
    }
}