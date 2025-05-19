
using fly.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fly.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Exhibit> Exhibit { get; set; }
        public DbSet<ExhibitInExhibition> ExhibitInExhibition { get; set; }
        public DbSet<Exhibition> Exhibition { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Podrazdelenie> Podrazdelenies { get; set; }
        public DbSet<CustomUser> CustomUsers { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<StorageLocation> StorageLocations { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<Restoration> Restorations { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи многие-ко-многим для ExhibitInExhibition
            modelBuilder.Entity<ExhibitInExhibition>()
                .HasKey(eie => new { eie.ExhibitionId, eie.ExhibitId });

            modelBuilder.Entity<ExhibitInExhibition>()
                .HasOne(eie => eie.Exhibition)
                .WithMany(e => e.ExhibitInExhibitions)
                .HasForeignKey(eie => eie.ExhibitionId);

            modelBuilder.Entity<ExhibitInExhibition>()
                .HasOne(eie => eie.Exhibit)
                .WithMany(e => e.ExhibitInExhibitions)
                .HasForeignKey(eie => eie.ExhibitId);

            // Настройка отношений для Movement
            modelBuilder.Entity<Movement>()
                .HasOne(m => m.Exhibit)
                .WithMany(e => e.Movements)
                .HasForeignKey(m => m.ExhibitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movement>()
                .HasOne(m => m.FromStorageLocation)
                .WithMany(sl => sl.FromMovements)
                .HasForeignKey(m => m.FromStorageLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movement>()
                .HasOne(m => m.ToStorageLocation)
                .WithMany(sl => sl.ToMovements)
                .HasForeignKey(m => m.ToStorageLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
