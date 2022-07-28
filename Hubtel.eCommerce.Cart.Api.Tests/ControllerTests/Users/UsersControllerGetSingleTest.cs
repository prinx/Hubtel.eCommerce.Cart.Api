using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Users
{
    public class UsersControllerGetSingleTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public UsersControllerGetSingleTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void GetUser_WithRightId_ShouldReturnOkResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            var result = await controller.GetUser(3) as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetUser_InvalidId_ShouldReturnNotFoundResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            var result = await controller.GetUser(-2) as NotFoundObjectResult;

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

