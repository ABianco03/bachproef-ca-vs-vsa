using Application.Customers;
using Application.Products;
using Domain;
using FluentValidation;

namespace Application.Orders;

public record CreateOrderLineRequest(int ProductId, int Quantity);

public record CreateOrderRequest(int CustomerId, List<CreateOrderLineRequest> OrderLines);

public record CreateOrderLineResult(int ProductId, int Quantity, decimal UnitPrice);

public record CreateOrderResult(int Id, int CustomerId, DateTime OrderDate, decimal TotalAmount, List<CreateOrderLineResult> OrderLines);

public interface ICreateOrderService
{
    Task<CreateOrderResult> ExecuteAsync(CreateOrderRequest request);
}

public class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.OrderLines).NotEmpty();
        RuleForEach(x => x.OrderLines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).GreaterThan(0);
            line.RuleFor(l => l.Quantity).GreaterThan(0);
        });
    }
}

public class CreateOrderService(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IProductRepository productRepository,
    IValidator<CreateOrderRequest> validator) : ICreateOrderService
{
    public async Task<CreateOrderResult> ExecuteAsync(CreateOrderRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var customer = await customerRepository.GetByIdAsync(request.CustomerId)
            ?? throw new KeyNotFoundException($"Customer with id {request.CustomerId} was not found.");

        var products = new Dictionary<int, Product>();

        foreach (var line in request.OrderLines)
        {
            var product = await productRepository.GetByIdAsync(line.ProductId)
                ?? throw new KeyNotFoundException($"Product with id {line.ProductId} was not found.");

            if (product.StockQuantity < line.Quantity)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for product with id {line.ProductId}: requested {line.Quantity}, available {product.StockQuantity}.");
            }

            products[line.ProductId] = product;
        }

        var totalAmount = request.OrderLines.Sum(line => products[line.ProductId].Price * line.Quantity);

        if (customer.DiscountTier == DiscountTier.Premium)
        {
            totalAmount *= 0.9m;
        }

        foreach (var line in request.OrderLines)
        {
            var product = products[line.ProductId];
            product.StockQuantity -= line.Quantity;
            await productRepository.UpdateAsync(product);
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            OrderLines = request.OrderLines
                .Select(line => new OrderLine
                {
                    ProductId = line.ProductId,
                    Quantity = line.Quantity,
                    UnitPrice = products[line.ProductId].Price
                })
                .ToList()
        };

        var saved = await orderRepository.AddAsync(order);

        return new CreateOrderResult(
            saved.Id,
            saved.CustomerId,
            saved.OrderDate,
            saved.TotalAmount,
            saved.OrderLines
                .Select(line => new CreateOrderLineResult(line.ProductId, line.Quantity, line.UnitPrice))
                .ToList());
    }
}
