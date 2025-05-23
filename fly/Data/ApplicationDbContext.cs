﻿using fly.Models;
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
        public DbSet<Category> Categorys { get; set; }
        public DbSet<StorageLocation> StorageLocations { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<Restoration> Restorations { get; set; }
        public DbSet<Insurance> Insurances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ExhibitInExhibition: many-to-many Exhibit <-> Exhibition
            modelBuilder.Entity<ExhibitInExhibition>()
                .HasKey(eie => new { eie.ExhibitionId, eie.ExhibitId });

            modelBuilder.Entity<ExhibitInExhibition>()
                .HasOne(eie => eie.Exhibition)
                .WithMany(e => e.ExhibitInExhibitions)
                .HasForeignKey(eie => eie.ExhibitionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExhibitInExhibition>()
                .HasOne(eie => eie.Exhibit)
                .WithMany(e => e.ExhibitInExhibitions)
                .HasForeignKey(eie => eie.ExhibitId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exhibit: Category (many-to-one)
            modelBuilder.Entity<Exhibit>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Exhibit)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

           

            // Movement: FromStorageLocation (many-to-one)
            modelBuilder.Entity<Movement>()
                .HasOne(m => m.FromStorageLocation)
                .WithMany(sl => sl.FromMovements)
                .HasForeignKey(m => m.FromStorageLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Movement: ToStorageLocation (many-to-one)
            modelBuilder.Entity<Movement>()
                .HasOne(m => m.ToStorageLocation)
                .WithMany(sl => sl.ToMovements)
                .HasForeignKey(m => m.ToStorageLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
          .HasOne(i => i.Exhibit)
          .WithMany()
          .HasForeignKey(i => i.ExhibitId)
          .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<StorageLocation>().HasData(
       new StorageLocation { StorageLocationId = 1, Name = "Главное хранилище", Description = "Основное место хранения экспонатов", Address = "ул. Музейная, 15" },
       new StorageLocation { StorageLocationId = 2, Name = "Временное хранилище", Description = "Для временных экспонатов", Address = "ул. Склада, 22" },
       new StorageLocation { StorageLocationId = 3, Name = "Архив", Description = "Архивное помещение", Address = "ул. Архивная, 31" },
       new StorageLocation { StorageLocationId = 4, Name = "Зал 3", Description = "Резервное хранилище", Address = "ул. Архивная, 321" }
   );
            modelBuilder.Entity<Category>().HasData(
    new Category
    {
        CategoryId = 1,
        Name = "Живопись",
        Description = "Картины, написанные маслом, акварелью и другими техниками."
    },
    new Category
    {
        CategoryId = 2,
        Name = "Скульптура",
        Description = "Трёхмерные произведения искусства из различных материалов."
    },
    new Category
    {
        CategoryId = 3,
        Name = "Декоративно-прикладное искусство",
        Description = "Изделия, сочетающие утилитарную функцию и художественную ценность."
    },
    new Category
    {
        CategoryId = 4,
        Name = "Графика",
        Description = "Рисунки, гравюры и литографии на бумаге."
    },
    new Category
    {
        CategoryId = 5,
        Name = "Фотография",
        Description = "Исторические и современные фотографии, представляющие художественную ценность."
    }
);

        }
    }
}