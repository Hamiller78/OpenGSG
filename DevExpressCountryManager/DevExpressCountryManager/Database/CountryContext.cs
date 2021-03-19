using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DevExpressCountryManager.Models.Common;
using DevExpressCountryManager.Models.WorldData;

namespace DevExpressCountryManager.Database
{
    public partial class CountryContext : DbContext
    {
        public DbSet<DXCountry> Countries { get; set; }
        public DbSet<BlobbableImage> Flags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = MainWindow.Configuration.GetSection("AzureSqlConnection").Value;

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
