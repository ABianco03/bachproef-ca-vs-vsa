using Domain;
using FluentValidation;

namespace Application.Customers;

public record UpdateCustomerRequest(string Name, string Email, DiscountTier DiscountTier);

public record UpdateCustomerResult(int Id, string Name, string Email, DiscountTier DiscountTier);

public interface IUpdateCustomerService
{
    Task<UpdateCustomerResult?> ExecuteAsync(int id, UpdateCustomerRequest request);
}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.DiscountTier).IsInEnum();
    }
}

public class UpdateCustomerService(ICustomerRepository repository, IValidator<UpdateCustomerRequest> validator) : IUpdateCustomerService
{
    public async Task<UpdateCustomerResult?> ExecuteAsync(int id, UpdateCustomerRequest request)
    {        
        var customer = await repository.GetByIdAsync(id);
        if (customer is null)
        {
            return null;
        }

        await validator.ValidateAndThrowAsync(request);

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.DiscountTier = request.DiscountTier;

        var saved = await repository.UpdateAsync(customer);

        return new UpdateCustomerResult(saved.Id, saved.Name, saved.Email, saved.DiscountTier);
    }
}
