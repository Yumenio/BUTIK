using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataLayer.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryID { get; set; }
        [Required]
        [Display(Name = "SubCategory")]
        public string Name { get; set; }
        [Required]
        [Display(Name ="CategoryID")]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
    }
}
