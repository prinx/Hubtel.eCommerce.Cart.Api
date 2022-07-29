#nullable disable
using Microsoft.AspNetCore.Mvc;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using System.Net;
using System.Text.Json;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartItemsService _cartItemsService;
        private readonly ILogger<CartItemsController> _logger;

        public CartsController(ICartItemsService cartItemsService, ILogger<CartItemsController> logger)
        {
            _logger = logger;
            _cartItemsService = cartItemsService;
        }

       // 1. Provide an endpoint to Add items to cart, with specified quantity
       // Adding similar items(same item ID) should increase the quantity - POST

       // POST: api/CartItems
       // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       [HttpPost]
        public async Task<ActionResult> PostCartItem(CartItemPostDTO cartItem)
        {
            await _cartItemsService.ValidatePostRequestBody(cartItem);

            CartItem fullItem = await _cartItemsService.RetrieveFullCartItem(cartItem);

            if (fullItem == null)
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

            fullItem.User = null;
            var updated = await _cartItemsService.UpdateCartItemQuantity(fullItem, cartItem.Quantity);

            if (!updated)
            {
                _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: Error while saving updated cart to database. Payload: {cartItem} Item: {fullItem}");

                var responseData = JsonSerializer.Serialize(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong"
                });

                return new ContentResult
                {
                    Content = responseData,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            _logger.LogInformation($"[{DateTime.Now}] POST: api/CartItems: Product {fullItem.Product.Name} quantity increased in the cart of user {cartItem.UserId}");

            return CreatedAtAction(nameof(GetCartItem), new { id = fullItem.Id }, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.Created,
                Success = true,
                Message = "Product(s) added to cart successfully.",
                Data = fullItem
            });
        }

        // 2.a. Provide an endpoint to remove an item from cart - DELETE verb
        // DELETE: api/CartItems/5
        [HttpDelete("{productId}/{userId}")]
        public async Task<IActionResult> DeleteCartItem(long productId, long userId)
        {
            var cartItem = await _cartItemsService.RetrieveFullCartItem(productId, userId);

            if (cartItem == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{productId}/{userId}: Cart item does not exist. Cannot delete.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Cart item not found."
                });
            }

            var deleted = await _cartItemsService.DeleteCartItem(cartItem);

            cartItem.User = null;

            if (!deleted)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{productId}/{userId}: Error while deleting cart from database. Payload: {cartItem}");

                var responseData = JsonSerializer.Serialize(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong"
                });

                return new ContentResult
                {
                    Content = responseData,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{productId}/{userId}: Cart deleted successfully.");

            return NoContent();
        }

        // 2.b. Provide an endpoint to remove an item from cart - DELETE verb
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

            var deleted = await _cartItemsService.DeleteCartItem(cartItem);

            cartItem.User = null;

            if (!deleted)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{id}: Error while deleting cart from database. Payload: {cartItem}");

                var responseData = JsonSerializer.Serialize(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "Something went wrong"
                });

                return new ContentResult
                {
                    Content = responseData,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{id}: Cart deleted successfully.");

            return NoContent();
        }

        // 3. Provide an endpoint list all cart items (with filters => phoneNumbers, time, quantity, item - GET
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

        // 4.a. Provide endpoint to get single item - GET
        // GET: api/CartItems/4/5
        [HttpGet("{productId}/{userId}")]
        public async Task<ActionResult> GetSingleCartItem(long productId, long userId)
        {
            string message;
            var item = await _cartItemsService.GetSingleCartItem(productId, userId);

            if (item == null)
            {
                message = "Cart item not found.";
                _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems/{productId}/{userId}: {message}");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = message
                });
            }

            message = "Found.";
            _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems/{productId}/{userId}: {message}");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = item
            });
        }

        // 4.b. Provide endpoint to get single item - GET
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
    }
}
