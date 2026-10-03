using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using static Authorizer.AuthorizerService;

namespace Authorizer.GrpcClient;

public interface IAuthorizerGrpcClient
{
    Task<bool> Verify(string token);
}

public class AuthorizerGrpcClient : IAuthorizerGrpcClient 
{
    private readonly AuthorizerServiceClient _service;

    private readonly GrpcChannel _channel;

    public AuthorizerGrpcClient(IConfiguration configuration)
    {
        var uri = configuration.GetConnectionString("AuthorizerGrpc") ?? throw new Exception("Grpc Uri missing for AuthorizerGrpc");

        _channel = GrpcChannel.ForAddress(uri);

        _service = new AuthorizerServiceClient(_channel);
    }

    public async Task<bool> Verify(string token)
    {
        var result = await _service.VerifyAsync( new JWTMessage { Token = token } );

        return result?.IsValid ?? false;
    }
}
