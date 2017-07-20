using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Account
{
    [Table("users")]
    public class User
    {
        [Key, Column(Order = 0)]
        [Required(ErrorMessage = "Please Enter User Id")]
        [Display(Name = "Nome Utente")]
        [RegularExpression(@"^[^\s\,]*$", ErrorMessage = "Nome utente non può contenere spazi")]
        //[ForeignKey("TeamLeaderOrders")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Conferma password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] ***PROBLEMA MYSQL
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        [Display(Name = "Nome")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Last Name")]
        [Display(Name = "Cognome")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter Email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Birth Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Data di nascita")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Please Enter Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefono")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please Select Role")]
        [Display(Name = "Ruolo")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Please Select Profile")]
        [Display(Name = "Profilo Ore")]
        public int Hours_ProfileId { get; set; }

        [Required(ErrorMessage = "Inserire Costo")]
        [Display(Name = "Costo Orario €")]
        public double Cost { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role UserRole { get; set; }

        [ForeignKey("Hours_ProfileId")]
        public virtual Profile HoursProfile { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<Orders.UserOrder> UserOrders { get; set; }

        [InverseProperty("TeamLeader")]
        public virtual ICollection<Orders.Order> TeamLeaderOrders { get; set; }
    }
}