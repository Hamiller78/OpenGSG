﻿// <auto-generated />
using DevExpressCountryManager.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevExpressCountryManager.Migrations
{
    [DbContext(typeof(CountryContext))]
    [Migration("20210315155917_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DevExpressCountryManager.Models.WorldData.DXCountry", b =>
                {
                    b.Property<string>("Tag")
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<string>("Allegiance")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Government")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Leader")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LongName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Tag");

                    b.ToTable("Countries");
                });
#pragma warning restore 612, 618
        }
    }
}
