using Application.Products;
using Domain;

namespace Application.Orders;

public interface IOrderLineProcessor
{
    Task<decimal> ProcessOrderLineAsync(Product product, int quantity, Customer customer);
}
