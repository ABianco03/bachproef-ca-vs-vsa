using Domain;

namespace Application.Customers;

public interface IGetAllCustomersService
{
    Task<IEnumerable<GetCustomerResult>> ExecuteAsync();
}

public class GetAllCustomersService(ICustomerRepository repository) : IGetAllCustomersService
{
    public async Task<IEnumerable<GetCustomerResult>> ExecuteAsync()
    {
        var customers = await repository.GetAllAsync();

        return customers.Select(c => new GetCustomerResult(c.Id, c.Name, c.Email, c.DiscountTier));
    }
}
