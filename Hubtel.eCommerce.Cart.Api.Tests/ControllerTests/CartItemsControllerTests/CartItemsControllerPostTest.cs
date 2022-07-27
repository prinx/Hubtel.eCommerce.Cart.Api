using System;
using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hubtel.eCommerce.Cart.Api.Tests
{
    public class CartItemsControllerPostTest : CartTest
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
                ProductId = 4,
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
        public async void PostCartItem_ZeroQuantity_ShouldReturnBadRequestResult()
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
            var result = await controller.PostCartItem(cartItem) as BadRequestResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void PostCartItem_InvalidProductId_ShouldReturnBadRequestResult()
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
            var result = await controller.PostCartItem(cartItem) as BadRequestResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void PostCartItem_InvalidUserId_ShouldReturnBadRequestResult()
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
            var result = await controller.PostCartItem(cartItem) as BadRequestResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void PostCartItem_QuantityFarLessThanCurrentCartItemQuantity_ShouldReturnBadRequestResult()
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
                Quantity = -1000,
                UserId = 1
            };
            var result = await controller.PostCartItem(cartItem) as BadRequestResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void PostCartItem_QuantityNegativeOnCreation_ShouldReturnBadRequestResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var cartItem = new CartItemPostDTO
            {
                ProductId = 4,
                Quantity = -10,
                UserId = 2
            };
            var result = await controller.PostCartItem(cartItem) as BadRequestResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}

