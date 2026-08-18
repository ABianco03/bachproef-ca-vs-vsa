using Domain;
using FluentValidation;

namespace Application.Customers;

public record CreateCustomerRequest(string Name, string Email, DiscountTier DiscountTier);

public record CreateCustomerResult(int Id, string Name, string Email, DiscountTier DiscountTier);

public interface ICreateCustomerService
{
    Task<CreateCustomerResult> ExecuteAsync(CreateCustomerRequest request);
}

public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty.")
            .EmailAddress().WithMessage("Email must be a valid email address.");
    }
}

public class CreateCustomerService(ICustomerRepository repository, IValidator<CreateCustomerRequest> validator) : ICreateCustomerService
{
    public async Task<CreateCustomerResult> ExecuteAsync(CreateCustomerRequest request)
    {
        await validator.ValidateAndThrowAsync(request);

        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email,
            DiscountTier = request.DiscountTier
        };

        var saved = await repository.AddAsync(customer);

        return new CreateCustomerResult(saved.Id, saved.Name, saved.Email, saved.DiscountTier);
    }
}
