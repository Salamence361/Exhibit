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
        public DbSet<Visitor> Visitor { get; set; }
        public DbSet<Visit> Visit { get; set; }
        public DbSet<Museum> Museum { get; set; }
        public DbSet<Podrazdelenie> Podrazdelenies { get; set; }
        public DbSet<CustomUser> CustomUsers { get; set; }
    }
}
