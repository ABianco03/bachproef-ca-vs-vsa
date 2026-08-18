using FastEndpoints;
using VSA.FastEndpoints.OrderMgmt.Features.Customers;

namespace VSA.FastEndpoints.OrderMgmt.Features.Customers.Get;

public record GetCustomerRequest(int Id);
public record GetCustomerResponse(int Id, string Name, string Email, DiscountTier DiscountTier);

public class GetCustomerEndpoint(AppDbContext context) : Endpoint<GetCustomerRequest, GetCustomerResponse>
{

    public override void Configure()
    {
        Get("/api/customers/{id}");
    }

    public override async Task HandleAsync(GetCustomerRequest req, CancellationToken ct)
    {
        var customer = await context.Customers.FindAsync(new object[] { req.Id }, ct);

        if (customer is null)
            ThrowError("Customer not found.", statusCode: 404);

        await Send.OkAsync(
            new GetCustomerResponse(customer.Id, customer.Name, customer.Email, customer.DiscountTier),
            ct);
    }
}
