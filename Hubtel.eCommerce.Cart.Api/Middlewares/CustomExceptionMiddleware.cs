using Hubtel.eCommerce.Cart.Api.Models;
using Hubtel.eCommerce.Cart.Api.Services;
using System.Net;

namespace Hubtel.eCommerce.Cart.Api.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionHandlerService _exceptionService;

        public CustomExceptionMiddleware(RequestDelegate next, IExceptionHandlerService exceptionService)
        {
            _next = next;
            _exceptionService = exceptionService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _exceptionService.LogException(httpContext, ex);
                await _exceptionService.HandleExceptionAsync(httpContext, ex);
            }
        }
    }
}
