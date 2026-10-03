using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Authorizer.GrpcClient;

public class InterAuthorizerAttribute(): ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var client = context.HttpContext.RequestServices.GetService<IAuthorizerGrpcClient>();
        if (context.HttpContext.Request.Headers.TryGetValue("token", out var token))
        {
            if (await client.Verify(token))
            {
              OnActionExecuted(await next());
            };
        }
        else
        {
            throw new Exception();
        }
    }
}
