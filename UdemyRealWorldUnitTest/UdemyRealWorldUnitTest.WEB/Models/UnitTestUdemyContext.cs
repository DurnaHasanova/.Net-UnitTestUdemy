using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using UdemyRealWorldUnitTest.WEB.Models;

#nullable disable

namespace UdemyRealWorldUnitTest.WEB.Models
{
	public partial class UnitTestUdemyContext : DbContext
	{
		public UnitTestUdemyContext()
		{
		}

		public UnitTestUdemyContext(DbContextOptions<UnitTestUdemyContext> options)
			: base(options)
		{
		}

		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<Category> Category { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

			modelBuilder.Entity<Product>(entity =>
			{
				entity.ToTable("Product");

				entity.Property(e => e.Color)
					.HasMaxLength(50)
					.HasColumnName("color");

				entity.Property(e => e.Name)
					.HasMaxLength(200)
					.HasColumnName("name");

				entity.Property(e => e.Price)
					.HasColumnType("decimal(18, 2)")
					.HasColumnName("price");

				entity.Property(e => e.Stock).HasColumnName("stock");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}