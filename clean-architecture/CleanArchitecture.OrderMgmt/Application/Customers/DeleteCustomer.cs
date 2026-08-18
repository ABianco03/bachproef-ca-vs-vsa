using Domain;

namespace Application.Customers;

public interface IDeleteCustomerService
{
    Task<bool> ExecuteAsync(int id);
}

public class DeleteCustomerService(ICustomerRepository repository) : IDeleteCustomerService
{
    public async Task<bool> ExecuteAsync(int id)
    {
        var customer = await repository.GetByIdAsync(id);

        if (customer is null)
        {
            return false;
        }

        await repository.DeleteAsync(customer);

        return true;
    }
}
