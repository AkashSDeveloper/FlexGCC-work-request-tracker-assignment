using Microsoft.EntityFrameworkCore;
using Tracker.Models;
namespace Tracker.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
 public DbSet<WorkRequest> WorkRequests => Set<WorkRequest>();
 protected override void OnModelCreating(ModelBuilder b) {
  var e=b.Entity<WorkRequest>(); e.HasKey(x=>x.Id);
  e.Property(x=>x.Title).HasMaxLength(100).IsRequired();
  e.Property(x=>x.ClientName).HasMaxLength(120).IsRequired();
  e.HasIndex(x=>x.Status); e.HasIndex(x=>new { x.ClientName, x.Title });
 }
}
