using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Orders
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
    }
}
