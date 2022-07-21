namespace Hubtel.eCommerce.Cart.Api.Models
{
    public class Cart
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public long UserId { get; set; }
        public User? User { get; set; }

        public List<CartItem>? CartItems { get; set; }

    }
}
