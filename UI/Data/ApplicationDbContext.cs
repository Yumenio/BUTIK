using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DataLayer;

namespace UI.Data
{
    public class ButikDbContext : IdentityDbContext
    {
        public ButikDbContext(DbContextOptions<ButikDbContext> options)
            : base(options)
        {
        }
        public DbSet<DataLayer.Product> Product { get; set; }
    }
    //public class ApplicationDbContext : IdentityDbContext
    //{
    //    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //        : base(options)
    //    {
    //    }
    //}
}
