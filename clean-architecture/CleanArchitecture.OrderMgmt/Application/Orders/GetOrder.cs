using Domain;

namespace Application.Orders;

public record GetOrderLineResult(int ProductId, int Quantity, decimal UnitPrice);

public record GetOrderResult(int Id, int CustomerId, DateTime OrderDate, decimal TotalAmount, List<GetOrderLineResult> OrderLines);

public interface IGetOrderService
{
    Task<GetOrderResult?> ExecuteAsync(int id);
}

public class GetOrderService(IOrderRepository repository) : IGetOrderService
{
    public async Task<GetOrderResult?> ExecuteAsync(int id)
    {
        var order = await repository.GetByIdAsync(id);

        if (order is null)
        {
            return null;
        }

        return new GetOrderResult(
            order.Id,
            order.CustomerId,
            order.OrderDate,
            order.TotalAmount,
            order.OrderLines
                .Select(line => new GetOrderLineResult(line.ProductId, line.Quantity, line.UnitPrice))
                .ToList());
    }
}
