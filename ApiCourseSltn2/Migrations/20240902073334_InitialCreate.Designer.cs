﻿// <auto-generated />
using ApiCourseSltn2.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApiCourseSltn2.Migrations
{
    [DbContext(typeof(ProductsContext))]
    [Migration("20240902073334_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ApiCourseSltn2.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            IsActive = true,
                            Price = 1100m,
                            ProductName = "Iphone 11"
                        },
                        new
                        {
                            ProductId = 2,
                            IsActive = false,
                            Price = 1200m,
                            ProductName = "Iphone 12"
                        },
                        new
                        {
                            ProductId = 3,
                            IsActive = true,
                            Price = 1300m,
                            ProductName = "Iphone 13"
                        },
                        new
                        {
                            ProductId = 4,
                            IsActive = true,
                            Price = 1400m,
                            ProductName = "Iphone 14"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
