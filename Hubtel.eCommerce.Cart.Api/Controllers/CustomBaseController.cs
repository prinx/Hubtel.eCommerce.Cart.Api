using System.Net;
using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hubtel.eCommerce.Cart.Api.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        protected readonly CartContext _context;
        protected readonly ILogger<CustomBaseController> _logger;

        public CustomBaseController(CartContext context, ILogger<CustomBaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        protected ObjectResult GenericError(string logMessage, string message = "An error happened.")
        {
            _logger.LogError(logMessage);

            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponseDTO
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Success = false,
                Message = message,
                Data = null
            });
        }
    }
}