using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using VSA.FastEndpoints.OrderMgmt.Features.Customers;

namespace VSA.FastEndpoints.OrderMgmt.Features.Customers.GetAll;

public record GetAllCustomersResponse(int Id, string Name, string Email, DiscountTier DiscountTier);

public class GetAllCustomersEndpoint(AppDbContext context) : EndpointWithoutRequest<List<GetAllCustomersResponse>>
{

    public override void Configure()
    {
        Get("/api/customers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var customers = await context.Customers
            .Select(c => new GetAllCustomersResponse(c.Id, c.Name, c.Email, c.DiscountTier))
            .ToListAsync(ct);

        await Send.OkAsync(customers, ct);
    }
}
