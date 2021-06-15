using Microsoft.AspNetCore.Http;

namespace SignalRToolkit.Filters
{
    public interface IHubFilter
    {
        public void OnException(ExceptionHubContext context);
        public void OnAuthorization(HttpContext context, params string[] attributeData);
    }
}
