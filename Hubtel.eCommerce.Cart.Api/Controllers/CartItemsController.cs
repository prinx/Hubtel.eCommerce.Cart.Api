#nullable disable
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : CustomBaseController
    {
        public CartItemsController(CartContext context, ILogger<CartItemsController> logger) : base(context, logger)
        {
        }

        // GET: api/CartItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems(
            [FromQuery] string phoneNumber = default,
            [FromQuery] long productId = default,
            [FromQuery] int minQuantity = default,
            [FromQuery] int maxQuantity = default,
            [FromQuery] DateTime from = default,
            [FromQuery] DateTime to = default,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            try
            {
                ValidateGetAllCartItemsQueryString(phoneNumber, productId, minQuantity, maxQuantity, from, to);

                var query = _context.CartItems
                    .Where(item => phoneNumber == default || (phoneNumber != default && item.User.PhoneNumber == phoneNumber))
                    .Where(item => productId == default || (productId != default && item.ProductId == productId))
                    .Where(item => minQuantity == default || (minQuantity != default && item.Quantity >= minQuantity))
                    .Where(item => maxQuantity == default || (maxQuantity != default && item.Quantity <= maxQuantity))
                    .Where(item => from == default || (from != default && item.CreatedAt >= from))
                    .Where(item => to == default || (to != default && item.CreatedAt <= to))
                    .Include(item => item.Product)
                    .AsQueryable();

                var pageItems = await PaginationService.Paginate(query, page, pageSize);

                if (pageItems.Items.Count <= 0)
                {
                    _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems: No cart item found.");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
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
            catch (ArgumentException e)
            {
                _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems: {e.Message}");

                return BadRequest(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Success = false,
                    Message = e.Message
                });
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] GET: api/CartItems: An error happened: {e}");
            }
        }

        // GET: api/CartItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(long id)
        {
            try
            {
                var item = await _context.CartItems.FindAsync(id);
                var message = "";

                if (item == null)
                {
                    message = "Cart item not found.";
                    _logger.LogInformation($"[{DateTime.Now}] GET: api/CartItems/{id}: {message}");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
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
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] GET: api/CartItems/{id}: An error happened: {e}");
            }
        }

        // PUT: api/CartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(long id, CartItem cartItem)
        {
            try
            {
                //if (id != cartItem.Id)
                //{
                //    _logger.LogInformation($"[{DateTime.Now}] PUT: api/CartItems/{id}: Id and cart item to update mismatch.");

                //    return BadRequest(new ApiResponseDTO
                //    {
                //        Status = (int)HttpStatusCode.BadRequest,
                //        Success = false,
                //        Message = "Id and cart item to update mismatch."
                //    });
                //}

                var updatedCartItem = new CartItem
                {
                    Id = id,
                    ProductId = cartItem.ProductId,
                    UserId = cartItem.UserId
                };

                _context.Entry(updatedCartItem).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"[{DateTime.Now}] PUT: api/CartItems/{id}: Cart item updated successfuly.");

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
                {
                    _logger.LogInformation($"[{DateTime.Now}] PUT: api/CartItems/{id}: Cart item to update not found.");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "Cart item to update not found."
                    });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] PUT: api/CartItems/{id}: An error happened: {e}");
            }
        }

        // POST: api/CartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CartItem>> PostCartItem(CartItemPostDTO cartItem)
        {
            try
            {
                await ValidatePostRequestBody(cartItem);
            }
            catch (ArgumentException ex)
            {
                _logger.LogInformation($"[{DateTime.Now}] POST: api/CartItems: {ex.Message}");

                return BadRequest(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Success = false,
                    Message = ex.Message,
                    Data = cartItem
                });
            }

            try
            {
                CartItem fullItem = await RetrieveFullCartItem(cartItem);

                if (fullItem != null)
                {
                    UpdateCartItemQuantity(fullItem, cartItem.Quantity);

                    _logger.LogInformation($"[{DateTime.Now}] POST: api/CartItems: Product {fullItem.Product.Name} quantity increased in the cart of user {cartItem.UserId}");

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
                    var newItemId = await CreateCartItem(cartItem);

                    _logger.LogInformation($"[{DateTime.Now}] POST: api/CartItems: New cart item created for user {cartItem.UserId}");

                    return CreatedAtAction(nameof(GetCartItem), new { id = newItemId }, new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.Created,
                        Success = true,
                        Message = "Product(s) added to cart successfully",
                        Data = cartItem
                    });
                }
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] POST: api/CartItems: An error happened: {e}");
            }
        }

        // DELETE: api/CartItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(long id)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(id);

                if (cartItem == null)
                {
                    _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{id}: Cart item does not exist. Cannot delete.");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "Cart item not found."
                    });
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/CartItems/{id}: Cart item deleted successfully.");

                return NoContent();
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] DELETE: api/CartItems/{id}: An error happened: {e}");
            }
        }

        private void ValidateGetAllCartItemsQueryString(
            string phoneNumber = default,
            long productId = default,
            int minQuantity = default,
            int maxQuantity = default,
            DateTime startDate = default,
            DateTime endDate = default
        )
        {
            if (phoneNumber != default && (phoneNumber.Length > 12 || phoneNumber.Length < 9))
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
        }

        private async Task<CartItem> RetrieveFullCartItem(CartItemPostDTO cartItem)
        {
            return await _context.CartItems
                .Where(item => item.UserId == cartItem.UserId && item.ProductId == cartItem.ProductId)
                .Include(item => item.Product)
                .Include(item => item.User)
                .FirstOrDefaultAsync();
        }

        private async void UpdateCartItemQuantity(CartItem item, int quantity)
        {
            _context.CartItems.Update(item);
            item.Quantity += quantity;

            if (item.Quantity < 0)
            {
                throw new ArgumentException("Invalid product quantity");
            }

            await _context.SaveChangesAsync();
        }

        private async Task<long> CreateCartItem(CartItemPostDTO item)
        {
            var newItem = new CartItem
            {
                UserId = item.UserId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };
            _context.CartItems.Add(newItem);
            await _context.SaveChangesAsync();

            return newItem.Id;
        }

        private async Task ValidatePostRequestBody(CartItemPostDTO cartItem)
        {
            if (cartItem.Quantity == 0)
            {
                throw new ArgumentException("Invalid product quantity.");
            }

            Product product = await _context.Products.FindAsync(cartItem.ProductId);

            if (product == null)
            {
                throw new ArgumentException("Invalid product.");
            }

            if (cartItem.Quantity > product.QuantityInStock)
            {
                throw new ArgumentException("Not enough products.");
            }

            User user = await _context.Users.FindAsync(cartItem.UserId);

            if (user == null)
            {
                throw new ArgumentException("Invalid user.");
            }
        }

        private bool CartItemExists(long id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }
    }
}
