using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Products
{
    public class ProductsControllerGetManyTest : CartTest
    {
        public ProductsControllerGetManyTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void GetProducts_WhenProductsInTable_ShouldReturnOkResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var result = await controller.GetProducts() as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetProducts_WhenNoProductInTable_ShouldReturnNotFoundResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var result = await controller.GetProducts() as NotFoundObjectResult;

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async void GetProducts_InvalidPage_ShouldReturnBadRequestResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var result = await controller.GetProducts(page: -100) as BadRequestObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetProducts_NegatifPageSize_ShouldReturnBadRequestResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var result = await controller.GetProducts(pageSize: -2) as BadRequestObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetProducts_BigPageSize_ShouldReturnBadRequestResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var result = await controller.GetProducts(pageSize: 2000) as BadRequestObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

