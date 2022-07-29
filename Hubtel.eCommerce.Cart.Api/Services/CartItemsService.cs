#nullable disable
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Exceptions;

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
                throw new InvalidRequestInputException("Invalid phone number");
            }

            if (productId != default && productId <= 0)
            {
                throw new InvalidRequestInputException("Product id must be greater than 0");
            }

            if ((minQuantity != default && minQuantity <= 0) || (maxQuantity != default && maxQuantity <= 0))
            {
                throw new InvalidRequestInputException("Any specified item quantity must be greater than 0");
            }

            if (startDate != default && endDate != default && startDate > endDate)
            {
                throw new InvalidRequestInputException("Start date must be less than end date");
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
            var items = _context.CartItems;

            if (phoneNumber != default)
            {
                items.Where(e => e.User.PhoneNumber == phoneNumber);
            }

            if (productId != default)
            {
                items.Where(e => e.User.PhoneNumber == phoneNumber);
            }

            if (productId != default)
            {
                items.Where(e => e.ProductId == productId);
            }

            if (minQuantity != default)
            {
                items.Where(e => e.Quantity >= minQuantity);
            }

            if (maxQuantity != default)
            {
                items.Where(e => e.Quantity <= maxQuantity);
            }

            if (startDate != default)
            {
                items.Where(e => e.Quantity <= maxQuantity);
            }

            if (startDate != default)
            {
                items.Where(e => e.CreatedAt >= startDate);
            }

            if (endDate != default)
            {
                items.Where(e => e.CreatedAt <= endDate);
            }

            items.Include(item => item.Product);

            var query = items.AsQueryable();

            return await PaginationService.Paginate(query, page, pageSize);
        }

        public async Task<CartItem> GetSingleCartItem(long id)
        {
            return await _context.CartItems
                .Include(e => e.Product)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<CartItem> GetSingleCartItem(long productId, long userId)
        {
            return await _context.CartItems
                .Include(e => e.Product)
                .FirstOrDefaultAsync(e => e.ProductId == productId && e.UserId == userId);
        }

        public async Task<bool> UpdateCartItem(long id, CartItemPostDTO cartItem)
        {
            var updatedCartItem = new CartItem
            {
                Id = id,
                Quantity = cartItem.Quantity,
                ProductId = cartItem.ProductId,
                UserId = cartItem.UserId
            };

            _context.Entry(updatedCartItem).State = EntityState.Modified;

            var changedRow = await _context.SaveChangesAsync();

            return changedRow == 1;
        }

        public async Task<CartItem> RetrieveFullCartItem(CartItemPostDTO cartItem)
        {
            return await _context.CartItems
                .Include(e => e.Product)
                .FirstOrDefaultAsync(e => e.UserId == cartItem.UserId && e.ProductId == cartItem.ProductId);
        }

        public async Task<CartItem> RetrieveFullCartItem(long productId, long userId)
        {
            return await _context.CartItems
                .Include(e => e.Product)
                .FirstOrDefaultAsync(e => e.ProductId == productId && e.UserId == userId);
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

        public async Task<bool> DeleteCartItem(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);

            var changedRow = await _context.SaveChangesAsync();

            return changedRow == 1;
        }

        public async Task<bool> UpdateCartItemQuantity(CartItem item, int quantity)
        {
            _context.CartItems.Update(item);

            item.Quantity = quantity;

            var changedRow = await _context.SaveChangesAsync();

            return changedRow == 1;
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
            if (cartItem.Quantity <= 0)
            {
                throw new InvalidRequestInputException("Invalid product quantity.");
            }

            var product = await _context.Products.FindAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new InvalidRequestInputException("Invalid product.");
            }

            if (cartItem.Quantity > product.QuantityInStock)
            {
                throw new InvalidRequestInputException("Not enough products.");
            }

            var user = await _context.Users.FindAsync(cartItem.UserId);

            if (user == null)
            {
                throw new InvalidRequestInputException("Invalid user.");
            }
        }

        public bool CartItemExists(long id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }
    }
}

