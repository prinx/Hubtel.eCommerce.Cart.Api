using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Exceptions;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Tests.ControllerTests.Users
{
    public class UsersControllerPostTest : CartTest, IClassFixture<TestDatabaseFixture>
    {
        public UsersControllerPostTest(TestDatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void PostUser_NewItem_ShouldReturnCreatedAtActionResult()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            var user = new UserPostDTO
            {
                Name = "Damien",
                PhoneNumber = "+233000000003"
            };
            var result = await controller.PostUser(user) as CreatedAtActionResult;

            //context.ChangeTracker.Clear();

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        //[Fact]
        //public async void PostUser_ExistingUser_ShouldReturnConflictObjectResult()
        //{
        //    // Arrange
        //    using var context = Fixture.CreateContext();

        //    var service = new UsersService(context);
        //    var logger = GetLogger<UsersController>();

        //    // Act
        //    var controller = new UsersController(service, logger);
        //    var user = new UserPostDTO
        //    {
        //        Name = "Damien",
        //        PhoneNumber = "+233000000002"
        //    };
        //    var result = await controller.PostUser(user) as ConflictObjectResult;

        //    //context.ChangeTracker.Clear();

        //    // Assert
        //    Assert.IsType<ConflictObjectResult>(result);
        //}

        [Fact]
        public async void PostUser_NameTooShort_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            var user = new UserPostDTO
            {
                Name = "D",
                PhoneNumber = "+233000000003"
            };
            //var result = await controller.PostUser(user) as BadRequestResult;

            //context.ChangeTracker.Clear();

            // Assert
            //Assert.IsType<BadRequestResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.PostUser(user));
        }

        [Fact]
        public async void PostUser_NameTooLong_ShouldThrowInvalidRequestInputException()
        {
            // Arrange
            using var context = Fixture.CreateContext();

            var service = new UsersService(context);
            var logger = GetLogger<UsersController>();

            // Act
            var controller = new UsersController(service, logger);
            var user = new UserPostDTO
            {
                Name = "DamienDamienDamienDamienDamienDamienDamienDamienDamien",
                PhoneNumber = "+233000000003"
            };
            //var result = await controller.PostUser(user) as BadRequestResult;

            //context.ChangeTracker.Clear();

            // Assert
            //Assert.IsType<BadRequestResult>(result);
            await Assert.ThrowsAsync<InvalidRequestInputException>(async () => await controller.PostUser(user));
        }
    }
}

