using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DevExpressCountryManager.Models.Common;
using DevExpressCountryManager.Models.WorldData;

namespace DevExpressCountryManager.Database
{
    public class CountryContext : DbContext
    {
        public DbSet<DXCountry> Countries { get; set; }
        public DbSet<BlobbableImage> BlobbableImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=localhost;database=OpenGSGTest;trusted_connection=true;");
        }
    }
}
