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

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name is too much long!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is not valid.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "User Type is required.")]
        public bool ValidateEmail { get; set; }
        public AllUserType UserType { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }


        [NotMapped]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, ErrorMessage = "Password is too much long!")]
        public string Password { get; set; }
        
        [NotMapped]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Address Line1 is required.")]
        public string Line1 { get; set; }

        [Required(ErrorMessage = "Area is required.")]
        [StringLength(25, ErrorMessage = "Area is too much long!")]
        public string Area { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        [StringLength(25, ErrorMessage = "Province is too much long!")]
        public string Province { get; set; }

        [Required(ErrorMessage = "PostalCode is required.")]
        [RegularExpression("^(?!.*[DFIOQU])[A-VXY][0-9][A-Z] ?[0-9][A-Z][0-9]$", ErrorMessage = "Postal Code is not Valid!")]
        public string PostalCode { get; set; }
        public string Country { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(10, ErrorMessage = "Phone number is not valid!")]
        [RegularExpression("^[0-9]*$",ErrorMessage = "Phone number is not valid!")]
        public string Number { get; set; }

        public enum AllUserType
        {
            Donor,
            Recipient
           
        }
    }
}