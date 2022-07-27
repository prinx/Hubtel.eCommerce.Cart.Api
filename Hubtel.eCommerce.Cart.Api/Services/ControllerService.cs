using System;
namespace Hubtel.eCommerce.Cart.Api.Services
{
    public class ControllerService : IControllerService
    {
        public void ValidatePaginationQueryString(int page = default, int pageSize = default)
        {
            if (page <= 0)
            {
                throw new ArgumentException("Invalid page");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentException("Invalid page size");
            }

            if (pageSize > 1000)
            {
                throw new ArgumentException("Page size too big");
            }
        }
    }
}

