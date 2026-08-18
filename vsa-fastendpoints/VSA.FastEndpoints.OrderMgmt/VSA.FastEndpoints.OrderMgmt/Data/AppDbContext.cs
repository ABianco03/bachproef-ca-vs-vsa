using Microsoft.EntityFrameworkCore;
using VSA.FastEndpoints.OrderMgmt.Features.Customers;
using VSA.FastEndpoints.OrderMgmt.Features.Products;
using Order = VSA.FastEndpoints.OrderMgmt.Features.Orders.Order;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
}