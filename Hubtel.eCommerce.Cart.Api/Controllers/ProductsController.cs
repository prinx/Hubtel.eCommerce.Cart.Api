﻿#nullable disable
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
using Hubtel.eCommerce.Cart.Api.Filters;
using System.Text.RegularExpressions;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    [ValidationActionFilter]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductsService productsService, ILogger<ProductsController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 3)
        {
            _productsService.ValidateGetProductsQueryString(page, pageSize);
            var users = await _productsService.GetProducts(page, pageSize);

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

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await _productsService.GetSingleProduct(id);

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

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutProduct(long id, ProductPostDTO product)
        {
            _productsService.ValidateSentProduct(product);

            string logMessage;

            //if (id != product.Id)
            //{
            //    logMessage = "Invalid Product or Id.";
            //    _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: {logMessage}");

            //    return BadRequest(new ApiResponseDTO
            //    {
            //        Status = (int)HttpStatusCode.BadRequest,
            //        Success = false,
            //        Message = logMessage,
            //        Data = product
            //    });
            //}

            try
            {
                _productsService.UpdateProduct(id, product);
                _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: Product updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_productsService.ProductExists(id))
                {
                    logMessage = "Product not found.";
                    _logger.LogInformation($"[{DateTime.Now}] PUT: api/Products/{id}: {logMessage}");

                    return NotFound(new ApiResponseDTO
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = logMessage,
                        Data = product
                    });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks,
        // see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostProduct(ProductPostDTO product)
        {
            product.Name.Trim();

            _productsService.ValidateSentProduct(product);

            if (_productsService.ProductExists(product.Name))
            {
                _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: " +
                    $"Product with Name {product.Name} already exists. ");

                return Conflict(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Message = "Product already exists."
                });
            }

            var newProduct = await _productsService.CreateProduct(product);

            _logger.LogInformation($"[{DateTime.Now}] POST: api/Products: " +
                $"Product with name '{product.Name}' created successfully.");

            return CreatedAtAction("GetProduct", new { id = newProduct.Id }, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.Created,
                Success = true,
                Message = "Product created successfully.",
                Data = newProduct
            });
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var product = await _productsService.RetrieveProduct(id);

            if (product == null)
            {
                _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Products/{id}: " +
                    $"Product with id {id} does not exist. Cannot delete product.");

                return NotFound(new ApiResponseDTO
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Message = "Product not found."
                });
            }

            _productsService.DeleteProduct(product);

            _logger.LogInformation($"[{DateTime.Now}] DELETE: api/Products/{id}: " +
                $"Product with id {id} deleted successfully.");

            return Ok(new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = "Product deleted successfully."
            });

            //return NoContent();
        }
    }
}
