using GseWeb.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GseWeb.Models.Materials
{
    [Table("materials")]
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Commessa")]
        [Required(ErrorMessage = "Selezionare Commessa")]
        public string OrderId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "Inserire Data")]
        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Fornitore")]
        [Required(ErrorMessage = "Inserire Fornitore")]
        public string Supplier { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Codice")]
        [Required(ErrorMessage = "Inserire Codice Materiale")]
        [RegularExpression(@"^[^\s\,]*$", ErrorMessage = "Il codice materiale non può contenere spazi")]
        public string Code { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Descrizione")]
        public string Description { get; set; }

        [Display(Name = "Quantità Ordinata")]
        [Required(ErrorMessage = "Inserire Quantità Ordinata")]
        public int Amount_Ordered { get; set; }

        [Display(Name = "Quantità Utilizzata")]
        [Required(ErrorMessage = "Inserire Quantità Utilizzata")]
        public int Amount_Used { get; set; }

        [NotMapped]
        [Display(Name = "Quantità Rimanente")]
        public double Amount_NotUsed
        {
            get
            {
                return Amount_Ordered - Amount_Used;
            }
        }

        [Display(Name = "Prezzo Unit.")]
        [Required(ErrorMessage = "Inserire Prezzo Unit.")]
        public double Price { get; set; }

        [NotMapped]
        [Display(Name = "Totale Utilizzato")]
        public double Total_Used
        {
            get
            {
                return Amount_Used * Price;
            }
        }

        [NotMapped]
        [Display(Name = "Totale Rimanente")]
        public double Total_NotUsed
        {
            get
            {
                return Amount_NotUsed * Price;
            }
        }

        [Display(Name = "Ordinato")]
        public bool Ordered { get; set; }

        [Display(Name = "Ricevuto")]
        public bool Recived { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}