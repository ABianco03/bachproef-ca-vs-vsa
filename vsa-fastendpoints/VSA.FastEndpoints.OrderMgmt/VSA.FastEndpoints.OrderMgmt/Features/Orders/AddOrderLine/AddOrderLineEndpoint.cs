using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VSA.FastEndpoints.OrderMgmt.Features.Customers;
using VSA.FastEndpoints.OrderMgmt.Features.Products;
using VSA.FastEndpoints.OrderMgmt.Features.Orders;

namespace VSA.FastEndpoints.OrderMgmt.Features.Orders.AddOrderLine;

public record AddOrderLineRequest(int OrderId, int ProductId, int Quantity);

public record AddOrderLineResponseLine(int ProductId, int Quantity, decimal UnitPrice);

public record AddOrderLineResponse(
    int Id,
    int CustomerId,
    DateTime OrderDate,
    decimal TotalAmount,
    List<AddOrderLineResponseLine> OrderLines);

public class AddOrderLineValidator : Validator<AddOrderLineRequest>
{
    public AddOrderLineValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

public class AddOrderLineEndpoint(AppDbContext context) : Endpoint<AddOrderLineRequest, AddOrderLineResponse>
{
    public override void Configure()
    {
        Post("/api/orders/{orderId}/lines");
        DontThrowIfValidationFails();
    }

    public override async Task HandleAsync(AddOrderLineRequest req, CancellationToken ct)
    {
        var order = await context.Orders.Include(o => o.OrderLines).FirstOrDefaultAsync(o => o.Id == req.OrderId, ct);
        if (order is null)
            ThrowError("Order not found.", statusCode: 404);

        if (ValidationFailures.Count > 0)
        {
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        var product = await context.Products.FindAsync(new object[] { req.ProductId }, ct);
        if (product is null)
            ThrowError("Product not found.", statusCode: 404);

        var customer = await context.Customers.FindAsync(new object[] { order.CustomerId }, ct);
        if (customer is null)
            ThrowError("Customer not found.", statusCode: 404);

        if (product.StockQuantity < req.Quantity)
            ThrowError("Insufficient stock.", statusCode: 409);

        decimal unitPrice = product.Price;
        decimal lineAmount = unitPrice * req.Quantity;
        if (customer.DiscountTier == DiscountTier.Premium)
            lineAmount *= 0.9m;

        product.StockQuantity -= req.Quantity;

        var orderLine = new OrderLine
        {
            OrderId = order.Id,
            ProductId = product.Id,
            Quantity = req.Quantity,
            UnitPrice = unitPrice
        };

        order.OrderLines.Add(orderLine);
        order.TotalAmount += lineAmount;

        await context.SaveChangesAsync(ct);

        await Send.OkAsync(new AddOrderLineResponse(
            order.Id,
            order.CustomerId,
            order.OrderDate,
            order.TotalAmount,
            order.OrderLines
                .Select(line => new AddOrderLineResponseLine(line.ProductId, line.Quantity, line.UnitPrice))
                .ToList()),
            cancellation: ct);
    }
}