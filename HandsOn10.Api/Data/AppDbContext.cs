using HandsOn10.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HandsOn10.Api.Data;
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
 public DbSet<AppUser> Users => Set<AppUser>();
 public DbSet<AccessRequest> Requests => Set<AccessRequest>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<AppUser>().HasIndex(x=>x.Username).IsUnique();
  b.Entity<AppUser>().Property(x=>x.Username).HasMaxLength(80);
  b.Entity<AccessRequest>().HasOne<AppUser>().WithMany().HasForeignKey(x=>x.OwnerId).OnDelete(DeleteBehavior.Restrict);
 }
}
