using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;

namespace SignalRToolkit.Filters
{
    public class DefaultHubFilter : IHubFilter
    {
        public void OnAuthorization(HttpContext context, params string[] attributeData)
        {
            if (!context.User.Claims.Any()) throw new Exception("Unauthorized");
        }

        public void OnException(ExceptionHubContext context)
        {
            context.ExceptionResult = context.Exception.Message;
            context.StatusCode = HttpStatusCode.InternalServerError;
        }
    }
}
