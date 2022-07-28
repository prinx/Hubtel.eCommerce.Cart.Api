using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Products
{
    public class ProductsControllerPutTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public ProductsControllerPutTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void PutProduct_WithValidId_ShouldReturnNotContentResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var product = new ProductPostDTO {
                Name = "Car",
                QuantityInStock = 36,
                UnitPrice = 20000
            };
            var result = await controller.PutProduct(3, product) as NoContentResult;

            //context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

