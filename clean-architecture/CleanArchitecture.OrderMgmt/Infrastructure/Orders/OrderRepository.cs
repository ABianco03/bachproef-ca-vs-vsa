using Application.Orders;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Orders;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public async Task<Order> AddAsync(Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await context.Orders
            .Include(order => order.OrderLines)
            .FirstOrDefaultAsync(order => order.Id == id);
    }
}
