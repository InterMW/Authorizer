using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Authorizer.GrpcClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

AuthorizerGrpcDependencyModule.RegisterClient(builder.Services);

var host = builder.Build();

host.MapControllers();

var cli = host.Services.GetService<IAuthorizerGrpcClient>();


HttpClient sharedClient = new()
{
    BaseAddress = new Uri("http://localhost:5001"),
};
using StringContent jsonContent = new(
        JsonSerializer.Serialize(new
        {
            user = "jbmelberg",
            pass = "test"
        }),
        Encoding.UTF8,
        "application/json");

var key = await sharedClient.PostAsync("login", jsonContent);

var output = await key.Content.ReadAsStringAsync();

Console.WriteLine(output);

var j = await cli.Verify(output);

Console.WriteLine(j);


host.Run();
