using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsID { get; set; }
        [Required]
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual OrderHeader OrderHeader { get; set; }

        [Required]
        public int AnnounceID { get; set; }
        [ForeignKey("AnnounceID")]
        public virtual Announce Announce { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
    }
}
