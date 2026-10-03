using Authorizer.GrpcCommon;
using DomainService;
using Grpc.Core;

namespace Authorizer.Grpc;

public class AuthorizerGrpcServer : AuthorizerServiceBaseCommon
{
    private readonly IAuthorizerDomainService _domainService;

    public AuthorizerGrpcServer(IAuthorizerDomainService domainService)
    {
        _domainService = domainService;
    }

    public override async Task<JWTValid> Verify(JWTMessage request, ServerCallContext context)
    {
        return new JWTValid
        {
            IsValid = await _domainService.IsValid(request.Token)
        };
    }

    // The following are defined becuase I use the "generate overrides"
    // function to fill out the generated stuff
    public override string? ToString() => base.ToString();

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

}
