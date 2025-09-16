using Microsoft.EntityFrameworkCore;
using Retail.Domain.Entities;

namespace Retail.Infrastructure.Context
{
    public class RetailDbContext : DbContext
    {
        public RetailDbContext(DbContextOptions<RetailDbContext> opts) : base(opts) { }

        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierStoreItem> SupplierStoreItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<QuarterlyPlan> QuarterlyPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreItem>()
                .Property(s => s.Price)
                .HasColumnType("numeric(9,2)");

            // SupplierStoreItems (many-to-many join table)
            modelBuilder.Entity<SupplierStoreItem>()
                .HasKey(ssi => new { ssi.SupplierId, ssi.StoreItemId });

            modelBuilder.Entity<SupplierStoreItem>()
                .HasOne(ssi => ssi.Supplier)
                .WithMany(s => s.SupplierStoreItems)
                .HasForeignKey(ssi => ssi.SupplierId);

            modelBuilder.Entity<SupplierStoreItem>()
                .HasOne(ssi => ssi.StoreItem)
                .WithMany(si => si.SupplierStoreItems)
                .HasForeignKey(ssi => ssi.StoreItemId);

            // Sales
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.StoreItem)
                .WithMany(si => si.Sales)
                .HasForeignKey(s => s.StoreItemId);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Supplier)
                .WithMany(sp => sp.Sales)
                .HasForeignKey(s => s.SupplierId);

            modelBuilder.Entity<QuarterlyPlan>()
                .HasIndex(qp => new { qp.Year, qp.Quarter })
                .IsUnique();
        }
    }
}
