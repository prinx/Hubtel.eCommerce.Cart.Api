#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : CustomBaseController
    {
        public ProductsController(CartContext context, ILogger<ProductsController> logger) : base(context, logger)
        {
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            [FromQuery] int page = 1)
        {
            try
            {
                var query = _context.Products.AsQueryable();
                var users = await PaginationService.Paginate(query, page);

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Ok",
                    Data = users
                });
            }
            catch (Exception e)
            {
                return GenericError($"GET: api/Products: An error happened while retrieving cart items from database: {e}");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogInformation($"GET: api/Products/{id}: Product not found.");

                    return NotFound(new
                    {
                        status = HttpStatusCode.NotFound,
                        success = false,
                        message = "Product not found.",
                        data = (object)null
                    });
                }

                return Ok(new
                {
                    status = HttpStatusCode.OK,
                    success = true,
                    message = "Found.",
                    data = product
                });
            }
            catch (Exception e)
            {
                return GenericError($"GET: api/Products/{id}: An error happened: {e}");
            }
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product product)
        {
            try
            {
                var logMessage = "";

                if (id != product.Id)
                {
                    logMessage = "Invalid Product or Id.";
                    _logger.LogInformation($"PUT: api/Products/{id}: {logMessage}");

                    return BadRequest(new
                    {
                        status = HttpStatusCode.BadRequest,
                        success = false,
                        message = logMessage,
                        data = product
                    });
                }

                _context.Entry(product).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        logMessage = "Product not found.";
                        _logger.LogInformation($"PUT: api/Products/{id}: {logMessage}");

                        return NotFound(new
                        {
                            status = HttpStatusCode.NotFound,
                            success = false,
                            message = logMessage,
                            data = product
                        });
                    }
                    else
                    {
                        throw;
                    }
                }

                _logger.LogInformation($"PUT: api/Products/{id}: Product updated successfully.");

                return NoContent();
            }
            catch (Exception e)
            {
                return GenericError($"PUT: api/Products/{id}: An error happened: {e}");
            }
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                product.Name.Trim();

                if (_context.Products.Any(e => product.Name == e.Name))
                {
                    _logger.LogInformation($"POST: api/Products: Product with id {product.Id} already exists. Cannot created new product.");

                    return Conflict(new
                    {
                        status = HttpStatusCode.Conflict,
                        success = false,
                        message = "Product already exists.",
                        data = (object)null
                    });
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"POST: api/Products: Product with name '{product.Name}' created successfully.");

                return CreatedAtAction("GetProduct", new { id = product.Id }, new
                {
                    status = HttpStatusCode.Created,
                    success = true,
                    message = "Product created successfully.",
                    data = product
                });
            }
            catch (Exception e)
            {
                return GenericError($"POST: api/Products: An error happened: {e}");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    _logger.LogInformation($"DELETE: api/Products/{id}: Product with id {id} does not exist. Cannot delete product.");

                    return NotFound(new
                    {
                        status = HttpStatusCode.Created,
                        success = true,
                        message = "Product created successfully.",
                        data = product
                    });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"DELETE: api/Products/{id}: Product with id {id} deleted successfully.");

                return Ok(new
                {
                    status = HttpStatusCode.OK,
                    success = true,
                    message = "Product deleted successfully.",
                    data = (object)null
                });

                //return NoContent();
            }
            catch (Exception e)
            {
                return GenericError($"DELETE: api/Products/{id}: An error happened: {e}");
            }
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
