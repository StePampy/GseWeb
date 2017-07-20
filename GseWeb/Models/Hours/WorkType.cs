using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Hours
{
    [Table("work_type")]
    public class WorkType
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Causale")]
        public string Description { get; set; }
        public virtual ICollection<Hour> Days { get; set; }
    }
}