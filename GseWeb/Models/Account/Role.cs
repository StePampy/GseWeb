using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Account
{
    [Table("userroles")]
    public class Role
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Display(Name = "Ruolo")]
        public string Value { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}