using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Tests
{

    public class TestDatabaseFixture
    {
        private const string ConnectionString = @"Server=localhost;Port=5432;Database=test_hubtel_ecommerce;User Id=postgres;Password=postgres";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        context.AddRange(
                            new Product { Name = "Watch", UnitPrice = 50, QuantityInStock = 5 },
                            new Product { Name = "TV", UnitPrice = 2000, QuantityInStock = 7 },
                            new Product { Name = "Keyboard", UnitPrice = 1000, QuantityInStock = 12 }
                        );

                        context.AddRange(
                            new User { Name = "Nuna", PhoneNumber = "+233000000000" },
                            new User { Name = "Nuna", PhoneNumber = "+233000000001" },
                            new User { Name = "Nuna", PhoneNumber = "+233000000002" }
                        );

                        context.AddRange(
                            new CartItem { ProductId = 1, Quantity = 2, UserId = 3 },
                            new CartItem { ProductId = 2, Quantity = 3, UserId = 1 },
                            new CartItem { ProductId = 3, Quantity = 4, UserId = 2 }
                        );

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public CartContext CreateContext()
        {
            var context = new CartContext(
                new DbContextOptionsBuilder<CartContext>()
                    .UseNpgsql(ConnectionString)
                    .Options);

            context.Database.BeginTransaction();

            return context;
        }
    }
}
