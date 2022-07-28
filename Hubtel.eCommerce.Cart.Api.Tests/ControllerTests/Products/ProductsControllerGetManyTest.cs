#nullable disable
using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Exceptions;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Products
{
    public class ProductsControllerGetManyTest : CartTest, IClassFixture<TestDatabaseFixture>
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

        //[Fact]
        //public async void GetProducts_WhenNoProductInTable_ShouldReturnNotFoundResult()
        //{
        //    // Arrange
        //    using var context = Fixture.CreateContext();

        //    var service = new ProductsService(context);
        //    var logger = GetLogger<ProductsController>();

        //    // Act
        //    var controller = new ProductsController(service, logger);
        //    var okResult = await controller.GetProducts() as OkObjectResult;
        //    var apiResponse = okResult.Value as ApiResponseDTO;
        //    var paginationObject = apiResponse.Data as Pagination<Product>;
        //    var products = paginationObject.Items;

        //    context.Products.RemoveRange(products);
        //    context.SaveChanges();

        //    var result = await controller.GetProducts() as NotFoundObjectResult;

        //    // Assert
        //    Assert.IsType<NotFoundObjectResult>(result);
        //}

        [Fact]
        public async void GetProducts_InvalidPage_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            //var result = await controller.GetProducts(page: -100) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetProducts(page: -100));
        }

        [Fact]
        public async void GetProducts_NegatifPageSize_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            //var result = await controller.GetProducts(pageSize: -2) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetProducts(pageSize: -2));
        }

        [Fact]
        public async void GetProducts_BigPageSize_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            //var result = await controller.GetProducts(pageSize: 2000) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetProducts(pageSize: 2000));
        }
    }
}

