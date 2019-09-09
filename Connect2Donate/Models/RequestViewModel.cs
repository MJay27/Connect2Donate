using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Connect2Donate.Models
{
    public class RequestViewModel
    {
        public IEnumerable<TblRequest> Requests { get; set; }
        public IEnumerable<TblUser> TblUsers { get; set; }

        public IEnumerable<TblRespons> Responses { get; set; }

        [Key]
        public int RequestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Status RequestStatus { get; set; }
        public int UserId { get; set; }
      
        public enum Status
        {
            Activated = 0,
            Completed = 1
        }

    }
}