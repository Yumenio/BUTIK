﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataLayer
{
    public class ShoppingCart
    {
        [Key]
        public int ShoppingCartID { get; set; }
    }
}
