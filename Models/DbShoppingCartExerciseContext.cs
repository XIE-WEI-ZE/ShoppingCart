using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace prjECommerceDemo.Models;

public partial class DbShoppingCartExerciseContext : DbContext
{
    public DbShoppingCartExerciseContext()
    {
    }

    public DbShoppingCartExerciseContext(DbContextOptions<DbShoppingCartExerciseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TCustomer> TCustomers { get; set; }
    public virtual DbSet<TProduct> TProducts { get; set; }
    public virtual DbSet<TShoppingCart> TShoppingCarts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=dbShoppingCartExercise;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TCustomer>(entity =>
        {
            entity.HasKey(e => e.FId);
            entity.ToTable("tCustomer");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FAccount).HasMaxLength(200).HasColumnName("fAccount");
            entity.Property(e => e.FAddress).HasMaxLength(100).HasColumnName("fAddress");
            entity.Property(e => e.FBirthday).HasColumnName("fBirthday");
            entity.Property(e => e.FCity).HasMaxLength(50).HasColumnName("fCity");
            entity.Property(e => e.FCreatedDate).HasColumnType("datetime").HasColumnName("fCreatedDate");
            entity.Property(e => e.FDistrict).HasMaxLength(50).HasColumnName("fDistrict");
            entity.Property(e => e.FEmail).HasMaxLength(100).HasColumnName("fEmail");
            entity.Property(e => e.FExternalId).HasMaxLength(200).HasColumnName("fExternalId");
            entity.Property(e => e.FGender).HasMaxLength(50).HasColumnName("fGender");
            entity.Property(e => e.FIsEnabled).HasColumnName("fIsEnabled");
            entity.Property(e => e.FLoginProvider).HasMaxLength(50).HasColumnName("fLoginProvider");
            entity.Property(e => e.FName).HasMaxLength(50).HasColumnName("fName");
            entity.Property(e => e.FPassword).HasColumnName("fPassword");
            entity.Property(e => e.FPhone).HasMaxLength(100).HasColumnName("fPhone");
            entity.Property(e => e.FPhoto).HasMaxLength(50).HasColumnName("fPhoto");
            entity.Property(e => e.FRoadAddress).HasMaxLength(200).HasColumnName("fRoadAddress");
            entity.Property(e => e.FSalt).HasColumnName("fSalt");
        });

        modelBuilder.Entity<TProduct>(entity =>
        {
            entity.HasKey(e => e.FId);
            entity.ToTable("tProduct");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FName).HasMaxLength(100).HasColumnName("fName");
            entity.Property(e => e.FQty).HasColumnName("fQty");
            entity.Property(e => e.FCost).HasColumnType("decimal(18, 2)").HasColumnName("fCost");
            entity.Property(e => e.FPrice).HasColumnType("decimal(18, 2)").HasColumnName("fPrice");
            entity.Property(e => e.FImagePath).HasMaxLength(100).HasColumnName("fImagePath");
        });

        modelBuilder.Entity<TShoppingCart>(entity =>
        {
            entity.HasKey(e => e.FId);
            entity.ToTable("tShoppingCart");

            entity.Property(e => e.FId).HasColumnName("fId");
            entity.Property(e => e.FDate).HasColumnName("fDate");
            entity.Property(e => e.FCustomerId).HasColumnName("fCustomerId");
            entity.Property(e => e.FProductId).HasColumnName("fProductId");
            entity.Property(e => e.FCount).HasColumnName("fCount");
            entity.Property(e => e.FPrice).HasColumnType("decimal(18, 2)").HasColumnName("fPrice");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
