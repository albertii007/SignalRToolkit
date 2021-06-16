# SignalRToolkit

Register services on  IServiceCollection services:
{
  services.RegisterSignalR();
  or 
  services.RegisterSignalR(typeof(FilterClass));
}


Register Hubs on  IEndpointRouteBuilder routeBuilder:
{
  routeBuilder.RegisterSignalRControllers<BaseClass>(basePath:"/hubs");
  or
  routeBuilder.RegisterSignalRControllers<HubBase>(basePath:"/hubs", Assembly assembly);
  
  //info: default assembly is typeof(HubBase).Assembly;
}
  
  FilterClass:
  public class FilterClass : IHubFilter 
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
  
  HubBase:
  
  public class HubBase : BaseHub
  {
   //example: protected IMyRepo MyRepo = Context.GetHttpContext().RequestServices.GetRequiredService<IMyRepo>();
  }
  
  [RouteHub("/users")]
  [AuthorizeHub]
  UsersHubController:
  public class UsersHubController : HubBase
  {
     [RouteHub("/users")]
    public class UserHubController : BaseHub
    {
        public async Task GetUsers(GetUsersQuery query)
        {
            var result = await Result(async () => await MyRepo.GetUsers());
           
            await Clients.Caller.SendAsync(ResponseHub(), result);
        }
    }
  }

