using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hubtel.eCommerce.Cart.Api.Tests
{
    public class CartItemsControllerPutTest : CartTest
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
            var cartItem = new CartItemPostDTO {
                ProductId = 2,
                Quantity = 10,
                UserId = 1
            };
            var result = await controller.PutCartItem(3, cartItem) as NoContentResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

