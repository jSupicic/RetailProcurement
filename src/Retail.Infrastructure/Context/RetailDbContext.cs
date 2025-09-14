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
        public DbSet<QuarterlyPlanSupplier> QuarterlyPlanSuppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            // QuarterlyPlanSuppliers (many-to-many join table)
            modelBuilder.Entity<QuarterlyPlanSupplier>()
                .HasKey(qps => new { qps.PlanId, qps.SupplierId });

            modelBuilder.Entity<QuarterlyPlanSupplier>()
                .HasOne(qps => qps.QuarterlyPlan)
                .WithMany(qp => qp.QuarterlyPlanSuppliers)
                .HasForeignKey(qps => qps.PlanId);

            modelBuilder.Entity<QuarterlyPlanSupplier>()
                .HasOne(qps => qps.Supplier)
                .WithMany(s => s.QuarterlyPlanSuppliers)
                .HasForeignKey(qps => qps.SupplierId);
        }
    }
}
