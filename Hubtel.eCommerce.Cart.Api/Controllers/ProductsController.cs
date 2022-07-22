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
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            try
            {
                var query = _context.Products.AsQueryable();
                var users = await PaginationService.Paginate(query, page, pageSize);

                if (users.Items.Count <= 0)
                {
                    _logger.LogInformation($"[{DateTime.Now}] GET: api/Products: No product found.");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "No product found.",
                        Data = users
                    });
                }

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
                return GenericError($"[{DateTime.Now}] GET: api/Products: An error happened: {e}");
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
                    _logger.LogInformation($"[{DateTime.Now}] GET: api/Products/{id}: Product not found.");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Product not found."
                    });
                }

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Found.",
                    Data = product
                });
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] GET: api/Products/{id}: An error happened: {e}");
            }
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, ProductPostDTO product)
        {
            try
            {
                var logMessage = "";

                //if (id != product.Id)
                //{
                //    logMessage = "Invalid Product or Id.";
                //    _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: {logMessage}");

                //    return BadRequest(new
                //    {
                //        status = HttpStatusCode.BadRequest,
                //        success = false,
                //        message = logMessage,
                //        data = product
                //    });
                //}

                var updatedProduct = new Product
                {
                    Id = id,
                    Name = product.Name,
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock
                };

                _context.Entry(updatedProduct).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        logMessage = "Product not found.";
                        _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: {logMessage}");

                        return NotFound(new ApiResponseDTO
                        {
                            Status = (int)HttpStatusCode.NotFound,
                            Message = logMessage,
                            Data = updatedProduct
                        });
                    }
                    else
                    {
                        throw;
                    }
                }

                _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: Product updated successfully.");

                return NoContent();
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] PUT: api/Products/{id}: An error happened: {e}");
            }
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductPostDTO product)
        {
            try
            {
                product.Name.Trim();
                ValidateSentProduct(product);

                if (_context.Products.Any(e => product.Name == e.Name))
                {
                    _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: Product with Name {product.Name} already exists. Cannot created new product.");

                    return Conflict(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.Conflict,
                        Message = "Product already exists."
                    });
                }

                var newProduct = new Product
                {
                    Name = product.Name,
                    UnitPrice = product.UnitPrice,
                    QuantityInStock = product.QuantityInStock
                };

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: Product with name '{product.Name}' created successfully.");

                return CreatedAtAction("GetProduct", new { id = newProduct.Id }, new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Product created successfully.",
                    Data = newProduct
                });
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] POST: api/Products: An error happened: {e}");
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
                    _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Products/{id}: Product with id {id} does not exist. Cannot delete product.");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Product not found."
                    });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Products/{id}: Product with id {id} deleted successfully.");

                return Ok(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Product deleted successfully."
                });

                //return NoContent();
            }
            catch (Exception e)
            {
                return GenericError($"[{DateTime.Now}] DELETE: api/Products/{id}: An error happened: {e}");
            }
        }

        private void ValidateSentProduct(ProductPostDTO product)
        {
            if (product.UnitPrice < 0)
            {
                throw new ArgumentException("Product unit price invalid");
            }

            if (product.Name.Length <= 1)
            {
                throw new ArgumentException("Product name too short");
            }

            if (product.Name.Length > 50)
            {
                throw new ArgumentException("Product name too long");
            }
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
