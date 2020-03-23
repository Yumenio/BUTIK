using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Models
{
    public class Announce
    {
        public int AnnounceID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public int Price { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }
        [Display(Name = "Product")]
        public int ProductID { get; set; }
    }
}
