using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Users
{
    public class UsersControllerPutTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public UsersControllerPutTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void PutUser_WithValidId_ShouldReturnNotContentResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            var user = new UserPostDTO {
                Name = "Damien",
                PhoneNumber = "+233000000000"
            };
            var result = await controller.PutUser(3, user) as NoContentResult;

            //context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}

