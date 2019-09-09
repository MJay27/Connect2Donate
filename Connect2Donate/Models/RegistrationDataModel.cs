using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Connect2Donate.Models
{
    public class RegistrationDataModel
    {
        public IEnumerable<TblUser> Users { get; set; }
        public IEnumerable<TblAddress> Addresses { get; set; }
        public IEnumerable<TblAddress> Contacts { get; set; }
        public TblUser User { get; set; }
        public TblAddress Address { get; set; }
        public TblContact Contact { get; set; }
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool ValidateEmail { get; set; }
        public AllUserType UserType { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }


        [NotMapped]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Line1 { get; set; }
        public string Area { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Number { get; set; }

        public enum AllUserType
        {
            Donor,
            Recipient
           
        }
    }
}