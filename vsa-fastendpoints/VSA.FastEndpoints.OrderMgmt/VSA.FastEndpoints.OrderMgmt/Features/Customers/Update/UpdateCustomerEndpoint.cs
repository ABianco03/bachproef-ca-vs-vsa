using FastEndpoints;
using FluentValidation;
using VSA.FastEndpoints.OrderMgmt.Features.Customers;

namespace VSA.FastEndpoints.OrderMgmt.Features.Customers.Update;

public record UpdateCustomerRequest(int Id, string Name, string Email, DiscountTier DiscountTier);
public record UpdateCustomerResponse(int Id, string Name, string Email, DiscountTier DiscountTier);

public class UpdateCustomerValidator : Validator<UpdateCustomerRequest>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DiscountTier).IsInEnum();
    }
}

public class UpdateCustomerEndpoint(AppDbContext context) : Endpoint<UpdateCustomerRequest, UpdateCustomerResponse>
{

    public override void Configure()
    {
        Put("/api/customers/{id}");
        DontThrowIfValidationFails();
    }

    public override async Task HandleAsync(UpdateCustomerRequest req, CancellationToken ct)
    {
        var customer = await context.Customers.FindAsync(new object[] { req.Id }, ct);
        if (customer is null)
            ThrowError("Customer not found.", statusCode: 404);

        if (ValidationFailures.Count > 0)
        {
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        customer.Name = req.Name;
        customer.Email = req.Email;
        customer.DiscountTier = req.DiscountTier;

        await context.SaveChangesAsync(ct);

        await Send.OkAsync(
            new UpdateCustomerResponse(customer.Id, customer.Name, customer.Email, customer.DiscountTier),
            ct);
    }
}
