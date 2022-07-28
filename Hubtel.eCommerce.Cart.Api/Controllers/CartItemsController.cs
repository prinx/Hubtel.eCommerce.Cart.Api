#nullable disable
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using Hubtel.eCommerce.Cart.Api.Filters;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ValidationActionFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        protected readonly ICartItemsService _cartItemsService;
        protected readonly ILogger<CartItemsController> _logger;

        public CartItemsController(ICartItemsService cartItemsService, ILogger<CartItemsController> logger)
        {
            _logger = logger;
            _cartItemsService = cartItemsService;
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult> GetCartItems(
            [FromQuery] string phoneNumber = default,
            [FromQuery] long productId = default,
            [FromQuery] int minQuantity = default,
            [FromQuery] int maxQuantity = default,
            [FromQuery] DateTime from = default,
            [FromQuery] DateTime to = default,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            _cartItemsService.ValidateGetCartItemsQueryString(phoneNumber, productId, minQuantity, maxQuantity, from, to, page, pageSize);

            var pageItems = await _cartItemsService.GetCartItems(phoneNumber, productId, minQuantity, maxQuantity, from, to, page, pageSize);

            if (pageItems.Items.Count <= 0)
            {
                _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems: No cart item found.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "No cart item found.",
                    Data = pageItems
                });
            }

            var message = $"{pageItems.Items.Count} cart item(s) Found.";
                
            _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems: {message}");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = pageItems
            });
        }

        // GET: api/CartItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCartItem(long id)
        {
            string message;
            var item = await _cartItemsService.GetSingleCartItem(id);

            if (item == null)
            {
                message = "Cart item not found.";
                _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems/{id}: {message}");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = message
                });
            }

            message = "Found.";
            _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems/{id}: {message}");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = item
            });
        }

        // PUT: api/CartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(long id, CartItemPostDTO cartItem)
        {
            await _cartItemsService.ValidatePostRequestBody(cartItem);
            
            try
            {
                //if (id != cartItem.Id)
                //{
                //    _logger.LogInformation($"[{DateTime.Now}] PUT: api/CartItems/{id}: Id and cart item to update mismatch.");

                //    return BadRequest(new ApiResponseDTO
                //    {
                //        Status = (int)HttpStatusCode.BadRequest,
                //        Message = "Id and cart item to update mismatch."
                //    });
                //}

                await _cartItemsService.UpdateCartItem(id, cartItem);

                _logger.LogInformation($"[{DateTime.Now}] PUT: api/CartItems/{id}: Cart item updated successfuly.");

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_cartItemsService.CartItemExists(id))
                {
                    _logger.LogInformation($"[{DateTime.Now}] PUT: api/CartItems/{id}: Cart item to update not found.");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Cart item to update not found."
                    });
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/CartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostCartItem(CartItemPostDTO cartItem)
        {
            await _cartItemsService.ValidatePostRequestBody(cartItem);

            CartItem fullItem = await _cartItemsService.RetrieveFullCartItem(cartItem);

            if (fullItem != null)
            {
                await _cartItemsService.UpdateCartItemQuantity(fullItem, cartItem.Quantity);

                _logger.LogInformation($"[{DateTime.Now}] POST: api/CartItems: Product {fullItem.Product.Name} quantity increased in the cart of user {cartItem.UserId}");
                fullItem.User = null;

                return CreatedAtAction(nameof(GetCartItem), new { id = fullItem.Id }, new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Product(s) added to cart successfully.",
                    Data = fullItem
                });
            }
            else
            {
                var newItem = await _cartItemsService.CreateCartItem(cartItem);

                _logger.LogInformation($"[{DateTime.Now}] POST: api/CartItems: New cart item created for user {cartItem.UserId}");
                newItem.User = null;

                return CreatedAtAction(nameof(GetCartItem), new { id = newItem.Id }, new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Product(s) added to cart successfully",
                    Data = newItem
                });
            }
        }

        // DELETE: api/CartItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(long id)
        {
            var cartItem = await _cartItemsService.RetrieveFullCartItem(id);

            if (cartItem == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{id}: Cart item does not exist. Cannot delete.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Cart item not found."
                });
            }

            await _cartItemsService.DeleteCartItem(cartItem);

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{id}: Cart item deleted successfully.");

            return NoContent();
        }
    }
}
