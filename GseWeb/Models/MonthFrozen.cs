using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models
{
    [Table("months_frozen")]
    public class MonthFrozen
    {
        [Key, Column(Order = 0)]
        public int Year { get; set; }

        [Key, Column(Order = 1)]
        public int Month { get; set; }
    }
}