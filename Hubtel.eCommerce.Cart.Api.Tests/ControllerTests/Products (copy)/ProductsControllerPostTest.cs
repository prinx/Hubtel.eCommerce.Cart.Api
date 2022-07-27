using System;
using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Products
{
    public class ProductsControllerPostTest : CartTest
    {
        public ProductsControllerPostTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void PostProduct_NewItem_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var product = new ProductPostDTO
            {
                Name = "Car",
                QuantityInStock = 36,
                UnitPrice = 20000
            };
            var result = await controller.PostProduct(product) as CreatedAtActionResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async void PostProduct_ExistingProduct_ShouldReturnConflictObjectResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var product = new ProductPostDTO
            {
                Name = "Watch",
                QuantityInStock = 36,
                UnitPrice = 2000
            };
            var result = await controller.PostProduct(product) as ConflictObjectResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async void PostProduct_NameTooShort_ShouldReturnBadRequestResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var product = new ProductPostDTO
            {
                Name = "C",
                QuantityInStock = 36,
                UnitPrice = 20000
            };
            var result = await controller.PostProduct(product) as BadRequestResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void PostProduct_NameTooLong_ShouldReturnBadRequestResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var product = new ProductPostDTO
            {
                Name = "Car".PadLeft(50),
                QuantityInStock = 36,
                UnitPrice = 20000
            };
            var result = await controller.PostProduct(product) as BadRequestResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void PostProduct_UnitPriceNegative_ShouldReturnBadRequestResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new ProductsService(context);
            var logger = GetLogger<ProductsController>();

            // Act
            var controller = new ProductsController(service, logger);
            var product = new ProductPostDTO
            {
                Name = "Car",
                QuantityInStock = 36,
                UnitPrice = -20000
            };
            var result = await controller.PostProduct(product) as BadRequestResult;

            context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}

