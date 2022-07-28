using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.CartItems
{
    public class CartItemsControllerGetSingleTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public CartItemsControllerGetSingleTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        //[Fact]
        //public async void GetCartItem_WithRightId_ShouldReturnOkResult()
        //{
        //    // Arrange
        //    using var context = Fixture.CreateContext();

        //    var service = new CartItemsService(context);
        //    var logger = GetLogger<CartItemsController>();

        //    // Act
        //    var controller = new CartItemsController(service, logger);
        //    var result = await controller.GetCartItem(3) as OkObjectResult;

        //    // Assert
        //    Assert.IsType<OkObjectResult>(result);
        //}

        [Fact]
        public async void GetCartItem_InvalidId_ShouldReturnNotFoundResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var result = await controller.GetCartItem(-2) as NotFoundObjectResult;

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

