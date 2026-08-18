using Application.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ICreateOrderService _createOrderService;
    private readonly IGetOrderService _getOrderService;

    public OrdersController(
        ICreateOrderService createOrderService,
        IGetOrderService getOrderService)
    {
        _createOrderService = createOrderService;
        _getOrderService = getOrderService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResult>> Create(CreateOrderRequest request)
    {
        try
        {
            var result = await _createOrderService.ExecuteAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Insufficient stock",
                detail: exception.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetOrderResult>> GetById(int id)
    {
        var result = await _getOrderService.ExecuteAsync(id);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: $"Order with id {id} was not found.");
        }

        return Ok(result);
    }
}
