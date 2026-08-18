using FastEndpoints;
using FluentValidation;
using VSA.FastEndpoints.OrderMgmt.Features.Customers;
using VSA.FastEndpoints.OrderMgmt.Features.Orders;

namespace VSA.FastEndpoints.OrderMgmt.Features.Orders.PlaceOrder;

public record PlaceOrderRequest(int CustomerId, List<PlaceOrderLineRequest> OrderLines);
public record PlaceOrderLineRequest(int ProductId, int Quantity);
public record PlaceOrderResponse(int Id, int CustomerId, DateTime OrderDate, decimal TotalAmount, List<PlaceOrderLineResponse> OrderLines);
public record PlaceOrderLineResponse(int ProductId, int Quantity, decimal UnitPrice);

public class PlaceOrderValidator : Validator<PlaceOrderRequest>
{
    public PlaceOrderValidator()
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

public class PlaceOrderEndpoint(AppDbContext context) : Endpoint<PlaceOrderRequest, PlaceOrderResponse>
{

    public override void Configure()
    {
        Post("/api/orders");
    }

    public override async Task HandleAsync(PlaceOrderRequest req, CancellationToken ct)
    {
        var customer = await context.Customers.FindAsync(new object[] { req.CustomerId }, ct);

        if (customer is null)
            ThrowError("Customer not found.", statusCode: 404);

        var order = new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow
        };

        decimal totalAmount = 0m;

        foreach (var line in req.OrderLines)
        {
            var product = await context.Products.FindAsync(new object[] { line.ProductId }, ct);

            if (product is null)
                ThrowError("Product not found.", statusCode: 404);

            if (product.StockQuantity < line.Quantity)
                ThrowError("Insufficient stock.", statusCode: 409);

            totalAmount += product.Price * line.Quantity;
            product.StockQuantity -= line.Quantity;

            order.OrderLines.Add(new OrderLine
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = product.Price
            });
        }

        if (customer.DiscountTier == DiscountTier.Premium)
            totalAmount *= 0.9m;

        order.TotalAmount = totalAmount;

        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);

        await Send.CreatedAtAsync<Get.GetOrderEndpoint>(
            new { id = order.Id },
            new PlaceOrderResponse(
                order.Id,
                order.CustomerId,
                order.OrderDate,
                order.TotalAmount,
                order.OrderLines
                    .Select(l => new PlaceOrderLineResponse(l.ProductId, l.Quantity, l.UnitPrice))
                    .ToList()),
            cancellation: ct);
    }
}
