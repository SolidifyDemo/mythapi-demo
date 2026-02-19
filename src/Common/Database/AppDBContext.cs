using Microsoft.EntityFrameworkCore;
using Mythology = MythApi.Common.Database.Models.Mythology;
using God = MythApi.Common.Database.Models.God;
using MythApi.Common.Database.Models;

namespace MythApi.Common.Database;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<God> Gods { get; set; } = null!;
    public DbSet<Mythology> Mythologies { get; set; } = null!;
    public DbSet<Alias> Aliases { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // Map entities to tables
        modelBuilder.Entity<Mythology>().ToTable("Mythology");
        modelBuilder.Entity<God>().ToTable("God");
        modelBuilder.Entity<Alias>().ToTable("Alias");
        
        modelBuilder.Entity<God>()
            .HasMany(e => e.Aliases)
            .WithOne()
            .HasForeignKey(e => e.GodId)
            .IsRequired()
            ;
        
        modelBuilder.Entity<Mythology>()
            .HasMany(e => e.Gods)
            .WithOne()
            .HasForeignKey(e => e.MythologyId)
            .IsRequired()
            ;

        base.OnModelCreating(modelBuilder);
    }
}