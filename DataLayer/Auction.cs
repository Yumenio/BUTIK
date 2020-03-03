using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Core;

namespace DataLayer
{
    public class Auction:IAuction
    {
        [Key]
        public int AuctionID { get; set; }
        [Display(Name = "Productos")]
        public ICollection<Product> Products { get; set; }
        [Display(Name = "Tiempo de inicio")]
        public DateTime StartTime { get; set; }
    }
}
