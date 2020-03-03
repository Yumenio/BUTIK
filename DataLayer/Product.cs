using System;
using System.ComponentModel.DataAnnotations;
using Core;

namespace DataLayer
{
    public class Product:IProduct
    {
        [Key]
        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        //public int Price { get; set; }

        [Display(Name = "Product Type")]
        public string Type { get; set; }

        [Display(Name = "Price")]
        public int Price { get; set; }
    }
}
