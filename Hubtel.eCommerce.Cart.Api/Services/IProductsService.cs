using System;
using Hubtel.eCommerce.Cart.Api.Models;

namespace Hubtel.eCommerce.Cart.Api.Services
{
    public interface IProductsService : IControllerService
    {
        public void ValidateGetProductsQueryString(int page = default, int pageSize = default);

        public Task<Pagination<Product>> GetProducts(int page, int pageSize);

        public Task<Product> GetSingleProduct(long id);

        public Task UpdateProduct(long id, ProductPostDTO product);

        public Task<Product> CreateProduct(ProductPostDTO product);

        public Task<Product> RetrieveProduct(long id);

        public void DeleteProduct(Product product);

        public void ValidateSentProduct(ProductPostDTO product);

        public bool ProductExists(long id);

        public bool ProductExists(string name);
    }
}

