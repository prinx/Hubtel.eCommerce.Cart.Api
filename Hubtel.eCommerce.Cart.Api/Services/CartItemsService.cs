#nullable disable
using System;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public class CartItemsService : ControllerService, ICartItemsService
    {
        protected readonly CartContext _context;

        public CartItemsService(CartContext context)
        {
            _context = context;
        }

        public void ValidateGetCartItemsQueryString(
            string phoneNumber = default,
            long productId = default,
            int minQuantity = default,
            int maxQuantity = default,
            DateTime startDate = default,
            DateTime endDate = default,
            int page = default,
            int pageSize = default
        )
        {
            if (phoneNumber != default && (phoneNumber.Length > 15 || phoneNumber.Length < 9))
            {
                throw new ArgumentException("Invalid phone number");
            }

            if (productId != default && productId <= 0)
            {
                throw new ArgumentException("Product id must be greater than 0");
            }

            if ((minQuantity != default && minQuantity <= 0) || (maxQuantity != default && maxQuantity <= 0))
            {
                throw new ArgumentException("Any specified item quantity must be greater than 0");
            }

            if (startDate != default && endDate != default && startDate > endDate)
            {
                throw new ArgumentException("Start date must be less than end date");
            }

            ValidatePaginationQueryString(page, pageSize);
        }

        public async Task<Pagination<CartItem>> GetCartItems(
            string phoneNumber = default,
            long productId = default,
            int minQuantity = default,
            int maxQuantity = default,
            DateTime startDate = default,
            DateTime endDate = default,
            int page = default,
            int pageSize = default)
        {
            var query = _context.CartItems
                    .Where(item => phoneNumber == default || (phoneNumber != default && item.User.PhoneNumber == phoneNumber))
                    .Where(item => productId == default || (productId != default && item.ProductId == productId))
                    .Where(item => minQuantity == default || (minQuantity != default && item.Quantity >= minQuantity))
                    .Where(item => maxQuantity == default || (maxQuantity != default && item.Quantity <= maxQuantity))
                    .Where(item => startDate == default || (startDate != default && item.CreatedAt >= startDate))
                    .Where(item => endDate == default || (endDate != default && item.CreatedAt <= endDate))
                    .Include(item => item.Product)
                    .AsQueryable();

            return await PaginationService.Paginate(query, page, pageSize);
        }

        public async Task<CartItem> GetSingleCartItem(long id)
        {
            return await _context.CartItems
                .Include(e => e.Product)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async void UpdateCartItem(long id, CartItemPostDTO cartItem)
        {
            var updatedCartItem = new CartItem
            {
                Id = id,
                ProductId = cartItem.ProductId,
                UserId = cartItem.UserId
            };

            _context.Entry(updatedCartItem).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task<CartItem> RetrieveFullCartItem(CartItemPostDTO cartItem)
        {
            return await _context.CartItems
                .Where(item => item.UserId == cartItem.UserId && item.ProductId == cartItem.ProductId)
                .Include(item => item.Product)
                .Include(item => item.User)
                .FirstOrDefaultAsync();
        }

        public async Task<CartItem> RetrieveFullCartItem(long id)
        {
            return await _context.CartItems.FindAsync(id);
        }

        public bool QuantityFarLessThanCurrentCartItemQuantity(CartItemPostDTO cartItem, CartItem fullItem)
        {
            return cartItem.Quantity < 0 && fullItem.Quantity < (-1 * cartItem.Quantity);
        }

        public bool QuantityNegativeOnCreation(CartItemPostDTO cartItem)
        {
            return cartItem.Quantity <= 0;
        }

        public async void DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async void UpdateCartItemQuantity(CartItem item, int quantity)
        {
            _context.CartItems.Update(item);
            item.Quantity += quantity;

            if (item.Quantity < 0)
            {
                throw new ArgumentException("Invalid product quantity");
            }

            await _context.SaveChangesAsync();
        }

        public async Task<CartItem> CreateCartItem(CartItemPostDTO item)
        {
            var newItem = new CartItem
            {
                UserId = item.UserId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };
            _context.CartItems.Add(newItem);
            await _context.SaveChangesAsync();

            return newItem;
        }

        public async Task ValidatePostRequestBody(CartItemPostDTO cartItem)
        {
            if (cartItem.Quantity == 0)
            {
                throw new ArgumentException("Invalid product quantity.");
            }

            var product = await _context.Products.FindAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new ArgumentException("Invalid product.");
            }

            if (cartItem.Quantity > product.QuantityInStock)
            {
                throw new ArgumentException("Not enough products.");
            }

            var user = await _context.Users.FindAsync(cartItem.UserId);

            if (user == null)
            {
                throw new ArgumentException("Invalid user.");
            }
        }

        public bool CartItemExists(long id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }
    }
}

