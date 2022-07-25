namespace Hubtel.eCommerce.Cart.Api.Services
{
    public interface IExceptionHandlerService
    {

        public void LogException(HttpContext httpContext, Exception exception);

        public Task HandleExceptionAsync(HttpContext httpContext, Exception exception);
    }
}
