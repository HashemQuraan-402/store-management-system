using Microsoft.EntityFrameworkCore;
using StoreManagement.Models.Entities;

namespace StoreManagement.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Item> Items  => Set<Item>();
        public DbSet<SupplyDocument> SupplyDocuments => Set<SupplyDocument>();


        // review this in anthor time ==> ( very important )
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users: UserName must be unique (used for login).
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();


            // Warehouse: name must be unique across the system (client + server validation).
            modelBuilder.Entity<Warehouse>()
                .HasIndex(w => w.WarehouseName)
                .IsUnique();

            modelBuilder.Entity<Warehouse>()
                .HasOne(w => w.CreatedBy)
                .WithMany(u => u.Warehouses)
                .HasForeignKey( w => w.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);


            // Warehouse -> Items (one warehouse has many items), delete items when warehouse is deleted.
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Warehouse)
                .WithMany(w => w.Items)
                .HasForeignKey(i =>  i.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            // SupplyDocument relationships
            modelBuilder.Entity<SupplyDocument>()
                .HasOne(sd => sd.CreatedBy)
                .WithMany(u => u.SupplyDocuments)
                .HasForeignKey(sd => sd.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupplyDocument>()
                .HasOne(sd => sd.Warehouse)
                .WithMany(w => w.SupplyDocuments)
                .HasForeignKey(sd => sd.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SupplyDocument>()
                .HasOne(sd => sd.Item)
                .WithMany(i => i.supplyDocuments)
                .HasForeignKey(sd => sd.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            

        }
    }
}
