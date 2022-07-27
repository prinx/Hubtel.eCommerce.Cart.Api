using Hubtel.eCommerce.Cart.Api.Extensions;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddSingleton<IPagination<T>, typeof(PaginationService<>)>();
builder.Services.AddSingleton<IExceptionHandlerService, ExceptionHandlerService>();
builder.Services.AddSingleton<ICartItemsService, CartItemsService>();
builder.Services.AddSingleton<IProductsService, ProductsService>();
builder.Services.AddSingleton<IUsersService, UsersService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CartContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Handle exception with default handler middleware
//var exceptionService = app.Services.GetRequiredService<IExceptionHandlerService>();
//app.ConfigureExceptionHandler(exceptionService);

// Handle global exception with custom middleware
app.ConfigureCustomExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
