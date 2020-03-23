using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderID { get; set; }
        [Required]
        public string UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Total Payment")]
        public double OrderTotal { get; set; }

        [Required]
        [Display(Name = "PickUp Time")]
        public DateTime PickUpTime { get; set; }

        [Required]
        [NotMapped]
        public DateTime PickUpDate { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        [Display(Name = "PickUp Name")]
        public string PickUpName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string TransactionID { get; set; }
    }
}
