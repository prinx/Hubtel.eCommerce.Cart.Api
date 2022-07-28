using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Users
{
    public class UsersControllerDeleteTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public UsersControllerDeleteTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        //[Fact]
        //public async void DeleteUser_WithRightId_ShouldReturnNoContentResult()
        //{
        //    // Arrange
        //    using var context = Fixture.CreateContext();

        //    var service = new UsersService(context);
        //    var logger = GetLogger<UsersController>();

        //    // Act
        //    var controller = new UsersController(service, logger);
        //    var result = await controller.DeleteUser(1) as NoContentResult;

        //    //context.ChangeTracker.Clear();

        //    // Assert
        //    Assert.IsType<NoContentResult>(result);
        //}

        [Fact]
        public async void DeleteUser_InvalidId_ShouldReturnNotFoundResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            var result = await controller.DeleteUser(100) as NotFoundObjectResult;

            //context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}

