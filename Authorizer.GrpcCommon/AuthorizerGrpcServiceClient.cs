using Grpc.Core;

namespace Authorizer.GrpcCommon;

public class AuthorizerGrpcServiceClient : AuthorizerService.AuthorizerServiceClient
{
    public AuthorizerGrpcServiceClient(ChannelBase channel) : base(channel) { }
}
