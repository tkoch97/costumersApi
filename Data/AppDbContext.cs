using costumersApi.Models;
using Microsoft.EntityFrameworkCore;

namespace costumersApi.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Costumer> Costumers { get; set; }
  }
}