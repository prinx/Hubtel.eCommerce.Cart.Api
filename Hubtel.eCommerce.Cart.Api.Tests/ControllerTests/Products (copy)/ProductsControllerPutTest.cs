using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Products
{
    public class ProductsControllerPutTest : CartTest
    {
        public ProductsControllerPutTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void PutProduct_WithValidId_ShouldReturnNotContentResult()
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
            var result = controller.PutProduct(3, product) as NoContentResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

