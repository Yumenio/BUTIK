using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Models
{
    //clase menuitem
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        [ForeignKey("SubCategoryID")]
        public virtual SubCategory SubCategory { get; set; }

        [Display(Name = "SubCategory")]
        public int SubCategoryID { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Price should be greater than ${1}")]
        public double Price { get; set; }
    }
}
