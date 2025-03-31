using MCBAapp.Models;
using Microsoft.EntityFrameworkCore;

namespace MCBAapp.Data;

public class AdminApiContext : DbContext
{
    public AdminApiContext(DbContextOptions<AdminApiContext> options) : base(options) {}
    
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    
}