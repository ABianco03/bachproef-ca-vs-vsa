using Application.Customers;
using Application.Orders;
using Application.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateProductService, CreateProductService>();
        services.AddScoped<IGetProductService, GetProductService>();
        services.AddScoped<IGetAllProductsService, GetAllProductsService>();
        services.AddScoped<IUpdateProductService, UpdateProductService>();
        services.AddScoped<IDeleteProductService, DeleteProductService>();

        services.AddScoped<ICreateCustomerService, CreateCustomerService>();
        services.AddScoped<IGetCustomerService, GetCustomerService>();
        services.AddScoped<IGetAllCustomersService, GetAllCustomersService>();
        services.AddScoped<IUpdateCustomerService, UpdateCustomerService>();
        services.AddScoped<IDeleteCustomerService, DeleteCustomerService>();

        services.AddScoped<ICreateOrderService, CreateOrderService>();
        services.AddScoped<IGetOrderService, GetOrderService>();
        services.AddScoped<IAddOrderLineService, AddOrderLineService>();
        services.AddScoped<IOrderLineProcessor, OrderLineProcessor>();

        return services;
    }
}