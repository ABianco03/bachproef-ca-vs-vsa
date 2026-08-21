using Application.Products;
using Domain;

namespace Application.Orders;

internal class OrderLineProcessor(IProductRepository productRepository) : IOrderLineProcessor
{
    public async Task<decimal> ProcessOrderLineAsync(Product product, int quantity, Customer customer)
    {
        if (product.StockQuantity < quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock for product with id {product.Id}: requested {quantity}, available {product.StockQuantity}.");
        }

        var price = product.Price * quantity;
        if (customer.DiscountTier == DiscountTier.Premium)
        {
            price *= 0.9m;
        }

        product.StockQuantity -= quantity;
        await productRepository.UpdateAsync(product);

        return price;
    }
}
