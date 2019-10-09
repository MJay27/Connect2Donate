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
                var user = from data in db.TblUsers where data.UserId.Equals(id) select data;
                RequestViewModel requestViewModel = new RequestViewModel();

                if (user.FirstOrDefault().UserType.Equals("Donor"))
                {
                    //Deleting all the responses
                    var responsesList = from responses in db.TblResponses where responses.UserId.Equals(id) select responses;

                    requestViewModel.Responses = await responsesList.ToListAsync();
                    if (requestViewModel.Responses.Any())
                    {
                        foreach (TblRespons response in requestViewModel.Responses)
                        {
                            db.TblResponses.Remove(response);
                        }
                    }
                    await db.SaveChangesAsync();

                    var add = from address in db.TblAddresses where address.UserId.Equals(id) select address;
                    db.TblAddresses.Remove(add.FirstOrDefault());
                    await db.SaveChangesAsync();

                    var contact = from phone in db.TblContacts where phone.UserId.Equals(id) select phone;
                    db.TblContacts.Remove(contact.FirstOrDefault());
                    await db.SaveChangesAsync();

                    var userInfo = from data in db.TblUsers where data.UserId.Equals(id) select data;
                    db.TblUsers.Remove(userInfo.FirstOrDefault());


                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", "Logout");
                }
                else
                {
                    var requestsIdList = from requests in db.TblRequests where requests.UserId.Equals(id) select requests.RequestId;
                    foreach (int reqId in requestsIdList)
                    {
                        var responsesList = from responses in db.TblResponses where responses.RequestId.Equals(reqId) select responses;
                        requestViewModel.Responses = await responsesList.ToListAsync();
                        foreach (TblRespons response in requestViewModel.Responses)
                        {
                            db.TblResponses.Remove(response);
                        }
                    }
                    await db.SaveChangesAsync();

                    var requestsList = from data in db.TblRequests where data.UserId.Equals(id) select data;
                    requestViewModel.Requests = await requestsList.ToListAsync();
                    foreach (TblRequest request in requestViewModel.Requests)
                    {
                        db.TblRequests.Remove(request);
                    }

                    await db.SaveChangesAsync();

                    var add = from address in db.TblAddresses where address.UserId.Equals(id) select address;
                    db.TblAddresses.Remove(add.FirstOrDefault());
                    await db.SaveChangesAsync();

                    var contact = from phone in db.TblContacts where phone.UserId.Equals(id) select phone;
                    db.TblContacts.Remove(contact.FirstOrDefault());
                    await db.SaveChangesAsync();

                    var userInfo = from data in db.TblUsers where data.UserId.Equals(id) select data;
                    db.TblUsers.Remove(userInfo.FirstOrDefault());


                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", "Logout");
                }
            }
            return View();
        }
    }
}