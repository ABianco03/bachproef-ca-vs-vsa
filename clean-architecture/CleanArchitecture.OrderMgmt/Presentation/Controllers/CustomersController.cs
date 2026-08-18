using Application.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICreateCustomerService _createCustomerService;
    private readonly IGetCustomerService _getCustomerService;
    private readonly IGetAllCustomersService _getAllCustomersService;
    private readonly IUpdateCustomerService _updateCustomerService;
    private readonly IDeleteCustomerService _deleteCustomerService;

    public CustomersController(
        ICreateCustomerService createCustomerService,
        IGetCustomerService getCustomerService,
        IGetAllCustomersService getAllCustomersService,
        IUpdateCustomerService updateCustomerService,
        IDeleteCustomerService deleteCustomerService)
    {
        _createCustomerService = createCustomerService;
        _getCustomerService = getCustomerService;
        _getAllCustomersService = getAllCustomersService;
        _updateCustomerService = updateCustomerService;
        _deleteCustomerService = deleteCustomerService;
    }

    [HttpPost]
    public async Task<ActionResult<CreateCustomerResult>> Create(CreateCustomerRequest request)
    {

        var result = await _createCustomerService.ExecuteAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCustomerResult>> GetById(int id)
    {
        var result = await _getCustomerService.ExecuteAsync(id);

        if (result is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: $"Customer with id {id} was not found.");
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCustomerResult>>> GetAll()
    {
        var result = await _getAllCustomersService.ExecuteAsync();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdateCustomerResult>> Update(int id, UpdateCustomerRequest request)
    {
        var result = await _updateCustomerService.ExecuteAsync(id, request);

        if (result is null)
        {
            return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Resource not found",
            detail: $"Customer with id {id} was not found.");
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _deleteCustomerService.ExecuteAsync(id);

        if (!deleted)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource not found",
                detail: $"Customer with id {id} was not found.");
        }

        return NoContent();
    }
}
