using Hubtel.eCommerce.Cart.Api.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hubtel.eCommerce.Cart.Api.Tests
{
    public class CartItemsControllerTest : IClassFixture<TestDatabaseFixture>
    {
        public TestDatabaseFixture Fixture { get; }

        //private ILogger _logger;
        public CartItemsControllerTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;

            //var serviceProvider = new ServiceCollection()
            //    .AddLogging()
            //    .BuildServiceProvider();
            //var factory = serviceProvider.GetService<ILoggerFactory>(); 
            //_logger = factory.CreateLogger<CartItemsControllerTest>();
        }

        [Fact]
        public async void GetCartItems_Should_Return_OK()
        {
            using var context = Fixture.CreateContext();
            var mock = new Mock<ILogger<CartItemsController>>();
            ILogger<CartItemsController>  _logger = mock.Object;
            //_logger = Mock.Of<ILogger>();

            var controller = new CartItemsController(context, _logger);

            var cartItems = await controller.GetCartItems();

            Console.WriteLine(cartItems);

            Assert.Equal(3, 3);
        }
    }
}
