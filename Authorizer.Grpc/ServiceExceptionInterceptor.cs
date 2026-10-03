using Google.Protobuf.WellKnownTypes;
using Google.Rpc;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Authorizer.Grpc;

public class ServerExceptionInterceptor : Interceptor
{
    private readonly ILogger<ServerExceptionInterceptor> _logger;

    public ServerExceptionInterceptor(ILogger<ServerExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.AsyncDuplexStreamingCall(context, continuation);
        }
        catch (System.Exception ex) {throw Exceptor(ex); }
    }

    public override  AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.AsyncServerStreamingCall(request, context, continuation);
        }
        catch (System.Exception ex) {throw Exceptor(ex); }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
        }
        catch (System.Exception ex) {throw Exceptor(ex); }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await base.ServerStreamingServerHandler(request, responseStream, context, continuation);
        }
        catch (System.Exception ex) {throw Exceptor(ex); }
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await base.UnaryServerHandler(request, context, continuation);
        }
        catch (System.Exception ex) {throw Exceptor(ex); }
    }

    private Exception Exceptor(Exception ex)
    {
        Console.WriteLine(ex.Message);
        //Let's see if we can make this work, still have to decide if this is even what we want
        var status = new Google.Rpc.Status
        {
            Code = 999,
            Message = "An error occurred",
            Details =
            {
                Any.Pack( new ErrorInfo
                        {
                            Domain = ex.GetType().Name,
                            Reason = ex.Message,
                        }
                )
            }
        };

        return status.ToRpcException();
    }
}
