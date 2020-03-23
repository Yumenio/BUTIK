using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class ShoppingCart
    {

        public ShoppingCart()
        {
            Count = 1;
        }
        public int ShoppingCartID { get; set; }

        public string ApplicationUserID { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int AnnounceID { get; set; }

        [NotMapped]
        [ForeignKey("AnnounceID")]
        public virtual Announce Announce { get; set; }
        [Range(1,int.MaxValue,ErrorMessage = "Please enter a value greater than or equal {1}")]
        public int Count { get; set; }
    }
}
