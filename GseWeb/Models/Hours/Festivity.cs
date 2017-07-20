using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Hours
{
    [Table("festivity")]
    public class Festivity
    {
        [Key, Column(Order = 0)]
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Descrizione")]
        public string Description { get; set; }
    }
}