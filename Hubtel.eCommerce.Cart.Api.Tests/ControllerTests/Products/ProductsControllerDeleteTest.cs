using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Products
{
    public class ProductsControllerDeleteTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public ProductsControllerDeleteTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void DeleteProduct_WithRightId_ShouldReturnNoContentResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var result = await controller.DeleteProduct(1) as NoContentResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void DeleteProduct_InvalidId_ShouldReturnNotFoundResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var result = await controller.DeleteProduct(100) as NotFoundObjectResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

