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
    IValidator<CreateOrderRequest> validator,
    IOrderLineProcessor orderLineProcessor) : ICreateOrderService
{
    public async Task<CreateOrderResult> ExecuteAsync(CreateOrderRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var customer = await customerRepository.GetByIdAsync(request.CustomerId)
            ?? throw new KeyNotFoundException($"Customer with id {request.CustomerId} was not found.");

        var products = new Dictionary<int, Product>();
        var orderLines = new List<OrderLine>();
        decimal totalAmount = 0;

        foreach (var lineRequest in request.OrderLines)
        {
            var product = await productRepository.GetByIdAsync(lineRequest.ProductId)
                ?? throw new KeyNotFoundException($"Product with id {lineRequest.ProductId} was not found.");

            var price = await orderLineProcessor.ProcessOrderLineAsync(product, lineRequest.Quantity, customer);
            
            totalAmount += price;
            products[lineRequest.ProductId] = product;
            orderLines.Add(new OrderLine
            {
                ProductId = lineRequest.ProductId,
                Quantity = lineRequest.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            OrderLines = orderLines
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
