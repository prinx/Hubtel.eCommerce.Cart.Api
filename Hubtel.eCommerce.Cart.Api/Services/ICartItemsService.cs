using System;
using Hubtel.eCommerce.Cart.Api.Models;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public interface ICartItemsService : IControllerService
    {
        public void ValidateGetCartItemsQueryString(
            string? phoneNumber = default,
            long productId = default,
            int minQuantity = default,
            int maxQuantity = default,
            DateTime startDate = default,
            DateTime endDate = default,
            int page = default,
            int pageSize = default
        );

        public Task<Pagination<CartItem>> GetCartItems(
            string? phoneNumber = default,
            long productId = default,
            int minQuantity = default,
            int maxQuantity = default,
            DateTime startDate = default,
            DateTime endDate = default,
            int page = default,
            int pageSize = default
        );

        public Task<CartItem> GetSingleCartItem(long id);

        public void UpdateCartItem(long id, CartItemPostDTO cartItem);

        public Task<CartItem> RetrieveFullCartItem(CartItemPostDTO cartItem);

        public Task<CartItem> RetrieveFullCartItem(long id);

        public bool QuantityFarLessThanCurrentCartItemQuantity(CartItemPostDTO cartItem, CartItem fullItem);

        public bool QuantityNegativeOnCreation(CartItemPostDTO cartItem);

        public void DeleteCartItem(CartItem cartItem);

        public void UpdateCartItemQuantity(CartItem item, int quantity);

        public Task<CartItem> CreateCartItem(CartItemPostDTO item);

        public Task ValidatePostRequestBody(CartItemPostDTO cartItem);

        public bool CartItemExists(long id);
    }
}

