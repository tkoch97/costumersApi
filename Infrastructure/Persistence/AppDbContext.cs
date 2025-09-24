using costumersApi.Models;
using Microsoft.EntityFrameworkCore;

namespace costumersApi.Infrastructure.Persistence
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Costumer> Costumers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Costumer>().Property(c => c.CreatedAt)
      .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
  }
}