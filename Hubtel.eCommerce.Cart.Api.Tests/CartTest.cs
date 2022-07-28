using Hubtel.eCommerce.Cart.Api.Controllers;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Hubtel.eCommerce.Cart.Api.Tests
{
    public class CartTest : IClassFixture<TestDatabaseFixture>
    {
        public TestDatabaseFixture Fixture { get; }

        //private ILogger _logger;

        public CartTest(TestDatabaseFixture fixture)
        {
            Fixture = fixture;

            //var serviceProvider = new ServiceCollection()
            //    .AddLogging()
            //    .BuildServiceProvider();
            //var factory = serviceProvider.GetService<ILoggerFactory>(); 
            //_logger = factory.CreateLogger<CartItemsControllerTest>();
        }

        public ILogger<T> GetLogger<T>()
        {
            return (new Mock<ILogger<T>>()).Object;
        }
    }
}
