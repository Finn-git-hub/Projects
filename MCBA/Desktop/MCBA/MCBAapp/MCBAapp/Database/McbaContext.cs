using Microsoft.EntityFrameworkCore;
using MCBAapp.Models;
namespace MCBAapp.Database;

public class McbaContext : DbContext 
{
    public McbaContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Payee> Payees { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    
    //fluent-api

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        builder.Entity<Account>().HasOne(x => x.Customer).WithMany(x => x.Accounts);
        builder.Entity<BillPay>().HasOne(x => x.Account).WithMany(x => x.BillPays);
        builder.Entity<Payee>().HasMany(x => x.BillPays).WithOne(x => x.Payee);
        builder.Entity<Transaction>().HasOne(x => x.Account).WithMany(x => x.Transactions);
        builder.Entity<Login>().HasOne(x => x.Customer).WithOne(X => X.Login);

    }
}