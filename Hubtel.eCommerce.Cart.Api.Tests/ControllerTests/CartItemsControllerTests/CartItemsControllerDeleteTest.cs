using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hubtel.eCommerce.Cart.Api.Tests
{
    public class CartItemsControllerDeleteTest : CartTest
    {
        public CartItemsControllerDeleteTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void DeleteCartItem_WithRightId_ShouldReturnNoContentResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var result = await controller.DeleteCartItem(1) as NoContentResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void DeleteCartItem_InvalidId_ShouldReturnNotFoundResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var result = await controller.DeleteCartItem(100) as NotFoundObjectResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

