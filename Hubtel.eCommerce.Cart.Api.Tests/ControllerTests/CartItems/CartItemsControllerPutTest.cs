using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.CartItems
{
    public class CartItemsControllerPutTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public CartItemsControllerPutTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void PutCartItem_WithValidId_ShouldReturnNotContentResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var cartItem = new CartItemPostDTO
            {
                ProductId = 2,
                Quantity = 4,
                UserId = 1
            };
            var result = await controller.PutCartItem(3, cartItem) as NoContentResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

