using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
    
namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class CartContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        public CartContext(DbContextOptions<CartContext> options) : base(options)
        {
        }

    }
}
