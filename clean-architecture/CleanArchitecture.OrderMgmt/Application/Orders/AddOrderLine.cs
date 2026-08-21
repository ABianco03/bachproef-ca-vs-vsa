using Application.Customers;
using Application.Products;
using Domain;
using FluentValidation;

namespace Application.Orders;

public record AddOrderLineRequest(int OrderId, int ProductId, int Quantity);

public interface IAddOrderLineService
{
    Task<GetOrderResult> ExecuteAsync(AddOrderLineRequest request);
}

public class AddOrderLineValidator : AbstractValidator<AddOrderLineRequest>
{
    public AddOrderLineValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);
    }
}

public class AddOrderLineService(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    ICustomerRepository customerRepository,
    IValidator<AddOrderLineRequest> validator,
    IOrderLineProcessor orderLineProcessor) : IAddOrderLineService
{
    public async Task<GetOrderResult> ExecuteAsync(AddOrderLineRequest request)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId)
            ?? throw new KeyNotFoundException($"Order with id {request.OrderId} was not found.");

        await validator.ValidateAndThrowAsync(request);

        var product = await productRepository.GetByIdAsync(request.ProductId)
            ?? throw new KeyNotFoundException($"Product with id {request.ProductId} was not found.");

        var customer = await customerRepository.GetByIdAsync(order.CustomerId)
            ?? throw new KeyNotFoundException($"Customer with id {order.CustomerId} was not found.");

        var price = await orderLineProcessor.ProcessOrderLineAsync(product, request.Quantity, customer);

        var orderLine = new OrderLine
        {
            OrderId = order.Id,
            ProductId = product.Id,
            Quantity = request.Quantity,
            UnitPrice = product.Price
        };

        order.OrderLines.Add(orderLine);
        order.TotalAmount += price;

        await orderRepository.UpdateAsync(order);

        return new GetOrderResult(
            order.Id,
            order.CustomerId,
            order.OrderDate,
            order.TotalAmount,
            order.OrderLines
                .Select(line => new GetOrderLineResult(line.ProductId, line.Quantity, line.UnitPrice))
                .ToList());
    }
}
