using Grpc.Core;
using GrpcGreeter;

namespace GrpcGreeter.Services;

public class GreeterService : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        var name = $"{request.FirstName} {request.LastName}";
        var message = request.Language switch {
            "en-GB" => $"Good morning, {name}",
            "en-AU" => $"G'day, {name}",
            _ => $"Hello, {name}"
        };
        return Task.FromResult(new HelloReply
        {
            Message = message
        });
    }
}
