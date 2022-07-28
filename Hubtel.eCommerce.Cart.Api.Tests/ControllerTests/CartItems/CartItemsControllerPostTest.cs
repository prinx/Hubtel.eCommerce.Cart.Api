using System;
using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Exceptions;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.CartItems
{
    public class CartItemsControllerPostTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public CartItemsControllerPostTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void PostCartItem_NewItem_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var cartItem = new CartItemPostDTO {
                ProductId = 3,
                Quantity = 1,
                UserId = 2
            };
            var result = await controller.PostCartItem(cartItem) as CreatedAtActionResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async void PostCartItem_ExistingItemInCart_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var cartItem = new CartItemPostDTO {
                ProductId = 1,
                Quantity = 4,
                UserId = 3
            };
            var result = await controller.PostCartItem(cartItem) as CreatedAtActionResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async void PostCartItem_ZeroQuantity_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var cartItem = new CartItemPostDTO {
                ProductId = 1,
                Quantity = 0,
                UserId = 1
            };
            //var result = await controller.PostCartItem(cartItem) as BadRequestResult;

            //context.ChangeTracker.Clear();

            // Assert
            //Assert.IsType<BadRequestResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.PostCartItem(cartItem));
        }

        [Fact]
        public async void PostCartItem_InvalidProductId_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var cartItem = new CartItemPostDTO
            {
                ProductId = 0,
                Quantity = 1,
                UserId = 1
            };
            //var result = await controller.PostCartItem(cartItem) as BadRequestResult;

            //context.ChangeTracker.Clear();

            //// Assert
            //Assert.IsType<BadRequestResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.PostCartItem(cartItem));
        }

        [Fact]
        public async void PostCartItem_InvalidUserId_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var cartItem = new CartItemPostDTO
            {
                ProductId = 1,
                Quantity = 1,
                UserId = 0
            };
            //var result = await controller.PostCartItem(cartItem) as BadRequestResult;

            //context.ChangeTracker.Clear();

            //// Assert
            //Assert.IsType<BadRequestResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.PostCartItem(cartItem));
        }
    }
}

