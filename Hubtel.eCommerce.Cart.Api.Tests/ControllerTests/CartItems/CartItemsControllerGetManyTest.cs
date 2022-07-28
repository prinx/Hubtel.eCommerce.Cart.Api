#nullable disable
using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Exceptions;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.CartItems
{
    public class CartItemsControllerGetManyTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public CartItemsControllerGetManyTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        //[Fact]
        //public async void GetCartItems_WhenItemsInTable_ShouldReturnOkResult()
        //{
        //    // Arrange
        //    using var context = Fixture.CreateContext();

        //    var service = new CartItemsService(context);
        //    var logger = GetLogger<CartItemsController>();

        //    // Act
        //    var controller = new CartItemsController(service, logger);
        //    var result = await controller.GetCartItems() as OkObjectResult;

        //    // Assert
        //    Assert.IsType<OkObjectResult>(result);
        //}

        [Fact]
        public async void GetCartItems_WhenNoItemInTable_ShouldReturnNotFoundResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            var okResult = await controller.GetCartItems() as OkObjectResult;
            var apiResponse = okResult.Value as ApiResponseDTO;
            var paginationObject = apiResponse.Data as Pagination<CartItem>;
            var items = paginationObject.Items;

            context.CartItems.RemoveRange(items);
            context.SaveChanges();

            var result = await controller.GetCartItems() as NotFoundObjectResult;

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async void GetCartItems_InvalidPhoneNumber_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            //var result = await controller.GetCartItems(phoneNumber: "") as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(() => controller.GetCartItems(phoneNumber: ""));
        }

        [Fact]
        public async void GetCartItems_InvalidProductId_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            //var result = await controller.GetCartItems(productId: -2) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetCartItems(productId: -2));
        }

        [Fact]
        public async void GetCartItems_InvalidMinQuantity_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            //var result = await controller.GetCartItems(minQuantity: -2) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetCartItems(minQuantity: -2));
        }

        [Fact]
        public async void GetCartItems_InvalidMaxQuantity_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            //var result = await controller.GetCartItems(maxQuantity: -2) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetCartItems(maxQuantity: -2));
        }

        [Fact]
        public async void GetCartItems_InvalidDates_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            //var result = await controller.GetCartItems(from: DateTime.Now, to: DateTime.Today) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetCartItems(from: DateTime.Now, to: DateTime.Today));
        }

        [Fact]
        public async void GetCartItems_InvalidPage_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            //var result = await controller.GetCartItems(page: -100) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetCartItems(page: -100));
        }

        [Fact]
        public async void GetCartItems_NegatifPageSize_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            //var result = await controller.GetCartItems(pageSize: -2) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetCartItems(pageSize: -2));
        }

        [Fact]
        public async void GetCartItems_BigPageSize_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new CartItemsService(context);
            var logger = GetLogger<CartItemsController>();

            // Act
            var controller = new CartItemsController(service, logger);
            //var result = await controller.GetCartItems(pageSize: 2000) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetCartItems(pageSize: 2000));
        }
    }
}

