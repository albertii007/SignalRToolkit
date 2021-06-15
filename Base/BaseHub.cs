using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using SignalRToolkit.Filters;
using SignalRToolkit.ResponseResult;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SignalRToolkit.Base
{
    public class BaseHub : Hub
    {
        private ExceptionHubContext _exceptionContext = new ExceptionHubContext();
        private HubStorageHandler HubStorageHandler => Context.GetHttpContext().RequestServices.GetService<HubStorageHandler>();

        protected async Task<HubResponseResult<TResult>> Result<TResult>(Func<Task<TResult>> action, [CallerMemberName] string caller = "")
        {
            HttpContext httpContext = Context.GetHttpContext();

            IHubFilter hubFilter = httpContext.RequestServices.GetRequiredService<IHubFilter>();

            await OnAuthorizationProcess(caller, httpContext, hubFilter);

            if (_exceptionContext.Exception != null) return SetExceptionResult<TResult>();

            Task<TResult> data = action();

            CheckForExceptions(data, hubFilter);

            if (_exceptionContext.Exception != null) return SetExceptionResult<TResult>();

            return SetSuccessResult(await data);

        }

        private void CheckForExceptions<TResult>(Task<TResult> task, IHubFilter filter)
        {
            if (task.Exception == null) return;

            _exceptionContext.Exception = task.Exception;

            filter.OnException(_exceptionContext);
        }

        private HubResponseResult<TResult> SetSuccessResult<TResult>(TResult result)
        {
            return new HubResponseResult<TResult>
            {
                HasException = false,
                Result = result
            };
        }

        private HubResponseResult<TResult> SetExceptionResult<TResult>()
        {
            return new HubResponseResult<TResult>
            {
                ExceptionMessage = _exceptionContext.ExceptionResult,
                StatusCode = (int)_exceptionContext.StatusCode,
                HasException = true,
            };
        }

        private Task OnAuthorizationProcess(string caller, HttpContext context, IHubFilter hubFilter)
        {
            var store = HubStorageHandler._strictMethods.Where(x => x.Key == caller).FirstOrDefault();

            if (store.Key != null)
            {
                try { hubFilter.OnAuthorization(context, store.Value); }

                catch (Exception ex)
                {
                    _exceptionContext.Exception = ex;

                    hubFilter.OnException(_exceptionContext);
                }
            }

            return Task.CompletedTask;
        }

        protected string DefaultResponse([CallerMemberName] string methodName = "")
        {
            return $"{methodName}Response";
        }
    }
}
