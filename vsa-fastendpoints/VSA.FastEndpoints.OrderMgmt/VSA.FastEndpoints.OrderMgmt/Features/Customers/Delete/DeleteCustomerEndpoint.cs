using FastEndpoints;
using VSA.FastEndpoints.OrderMgmt.Features.Customers;

namespace VSA.FastEndpoints.OrderMgmt.Features.Customers.Delete;

public record DeleteCustomerRequest(int Id);

public class DeleteCustomerEndpoint(AppDbContext context) : Endpoint<DeleteCustomerRequest>
{

    public override void Configure()
    {
        Delete("/api/customers/{id}");
    }

    public override async Task HandleAsync(DeleteCustomerRequest req, CancellationToken ct)
    {
        var customer = await context.Customers.FindAsync(new object[] { req.Id }, ct);

        if (customer is null)
            ThrowError("Customer not found.", statusCode: 404);

        context.Customers.Remove(customer);
        await context.SaveChangesAsync(ct);

        await Send.NoContentAsync(ct);
    }
}
