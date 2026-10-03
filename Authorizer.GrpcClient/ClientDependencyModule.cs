using Microsoft.Extensions.DependencyInjection;

namespace Authorizer.GrpcClient;

public static class AuthorizerGrpcDependencyModule
{
    public static void RegisterClient(IServiceCollection services)
    {
        services.AddTransient<IAuthorizerGrpcClient, AuthorizerGrpcClient>();
    }
}
