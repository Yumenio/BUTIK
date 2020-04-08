using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Models.ViewModels
{
    public class AnnounceViewModel
    {
        public Announce Announce { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<SubCategory> SubCategory { get; set; }
        public IEnumerable<Product> Product { get; set; }
    }
}
