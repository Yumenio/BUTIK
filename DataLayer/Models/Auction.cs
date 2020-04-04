using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Core;

namespace DataLayer.Models
{
    public class Auction : IAuction
    {
        [Key]
        public int AuctionID { get; set; }

        [Display(Name = "Anuncio")]
        [Required]
        public int AnnounceID { get; set; }

        [ForeignKey("AnnounceID")]
        public virtual Announce Announce { get; set; }

        [Display(Name = "Length(Hrs)")]
        [Required]
        public int Length { get; set; }

        public DateTime Start { get; set; }

        [NotMapped]
        public DateTime End { get; set; }

        [Required]
        public int Price { get; set; }
    }
}
