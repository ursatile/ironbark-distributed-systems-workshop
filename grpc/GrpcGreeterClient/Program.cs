using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeter;

using var channel = GrpcChannel.ForAddress("https://localhost:7234");
var client = new Greeter.GreeterClient(channel);
var i = 1;
Console.WriteLine("1 = en-GB, 2 = en-AU, anything else = default");
while (true) {
    var language = Console.ReadKey().KeyChar switch {
        '1' => "en-GB",
        '2' => "en-AU",
        _ => String.Empty
    };
    var reply = await client.SayHelloAsync(
        new HelloRequest {
             FirstName = "Ironbark",
             LastName = "Group",
             Language = language
        } );

    Console.WriteLine($"Greeting was: {reply.Message}");    
}