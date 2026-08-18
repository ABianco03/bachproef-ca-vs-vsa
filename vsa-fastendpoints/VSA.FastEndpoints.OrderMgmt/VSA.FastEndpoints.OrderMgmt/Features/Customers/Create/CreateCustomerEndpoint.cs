using FastEndpoints;
using FluentValidation;
using VSA.FastEndpoints.OrderMgmt.Features.Customers;

namespace VSA.FastEndpoints.OrderMgmt.Features.Customers.Create;
public record CreateCustomerRequest(string Name, string Email, DiscountTier DiscountTier);
public record CreateCustomerResponse(int Id, string Name, string Email, DiscountTier DiscountTier);

public class CreateCustomerValidator : Validator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DiscountTier).IsInEnum();
    }
}

public class CreateCustomerEndpoint(AppDbContext context) : Endpoint<CreateCustomerRequest, CreateCustomerResponse>
{

    public override void Configure()
    {
        Post("/api/customers");
    }

    public override async Task HandleAsync(CreateCustomerRequest req, CancellationToken ct)
    {
        var customer = new Customer
        {
            Name = req.Name,
            Email = req.Email,
            DiscountTier = req.DiscountTier
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync(ct);

        await Send.CreatedAtAsync<CreateCustomerEndpoint>(
            new { id = customer.Id },
            new CreateCustomerResponse(customer.Id, customer.Name, customer.Email, customer.DiscountTier),
            cancellation: ct);
    }
}
