using Domain;

namespace Application.Customers;

public record GetCustomerResult(int Id, string Name, string Email, DiscountTier DiscountTier);

public interface IGetCustomerService
{
    Task<GetCustomerResult?> ExecuteAsync(int id);
}

public class GetCustomerService(ICustomerRepository repository) : IGetCustomerService
{
    public async Task<GetCustomerResult?> ExecuteAsync(int id)
    {
        var customer = await repository.GetByIdAsync(id);

        if (customer is null)
        {
            return null;
        }

        return new GetCustomerResult(customer.Id, customer.Name, customer.Email, customer.DiscountTier);
    }
}
