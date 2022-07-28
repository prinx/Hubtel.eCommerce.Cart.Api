#nullable disable
using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Exceptions;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Users
{
    public class UsersControllerGetManyTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public UsersControllerGetManyTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void GetUsers_WhenUsersInTable_ShouldReturnOkResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            var result = await controller.GetUsers() as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        //[Fact]
        //public async void GetUsers_WhenNoUserInTable_ShouldReturnNotFoundResult()
        //{
        //    // Arrange
        //    using var context = Fixture.CreateContext();

        //    var service = new UsersService(context);
        //    var logger = GetLogger<UsersController>();

        //    // Act
        //    var controller = new UsersController(service, logger);
        //    var okResult = await controller.GetUsers() as OkObjectResult;
        //    var apiResponse = okResult.Value as ApiResponseDTO;
        //    var paginationObject = apiResponse.Data as Pagination<User>;
        //    var users = paginationObject.Items;

        //    context.Users.RemoveRange(users);
        //    context.SaveChanges();

        //    var result = await controller.GetUsers() as NotFoundObjectResult;

        //    // Assert
        //    Assert.IsType<NotFoundObjectResult>(result);
        //}

        [Fact]
        public async void GetUsers_InvalidPage_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            //var result = await controller.GetUsers(page: -100) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetUsers(page: -100));
        }

        [Fact]
        public async void GetUsers_NegatifPageSize_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            //var result = await controller.GetUsers(pageSize: -2) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetUsers(pageSize: -2));
        }

        [Fact]
        public async void GetUsers_BigPageSize_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            //var result = await controller.GetUsers(pageSize: 2000) as BadRequestObjectResult;

            // Assert
            //Assert.IsType<BadRequestObjectResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.GetUsers(pageSize: 2000));
        }
    }
}

