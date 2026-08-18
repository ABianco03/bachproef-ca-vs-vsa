using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using VSA.FastEndpoints.OrderMgmt.Features.Orders;

namespace VSA.FastEndpoints.OrderMgmt.Features.Orders.Get;

public record GetOrderRequest(int Id);
public record GetOrderResponse(int Id, int CustomerId, DateTime OrderDate, decimal TotalAmount, List<GetOrderLineResponse> OrderLines);
public record GetOrderLineResponse(int ProductId, int Quantity, decimal UnitPrice);

public class GetOrderEndpoint(AppDbContext context) : Endpoint<GetOrderRequest, GetOrderResponse>
{

    public override void Configure()
    {
        Get("/api/orders/{id}");
    }

    public override async Task HandleAsync(GetOrderRequest req, CancellationToken ct)
    {
        var order = await context.Orders
            .Include(o => o.OrderLines)
            .FirstOrDefaultAsync(o => o.Id == req.Id, ct);

        if (order is null)
            ThrowError("Order not found.", statusCode: 404);

        await Send.OkAsync(
            new GetOrderResponse(
                order.Id,
                order.CustomerId,
                order.OrderDate,
                order.TotalAmount,
                order.OrderLines
                    .Select(l => new GetOrderLineResponse(l.ProductId, l.Quantity, l.UnitPrice))
                    .ToList()),
            ct);
    }
}
