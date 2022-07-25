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

                        context.AddRange();
                        
                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public CartContext CreateContext()
            => new CartContext(
                new DbContextOptionsBuilder<CartContext>()
                    .UseNpgsql(ConnectionString)
                    .Options);
    }
}
