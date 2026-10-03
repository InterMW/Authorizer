using Authorizer.Domain;
using Authorizer.Grpc;
using DomainService;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(name: "MyPolicy",
            builder => builder.AllowAnyHeader()
                              .AllowAnyMethod()
                              .SetIsOriginAllowed((host) => true)
                              .AllowCredentials()
            );
    }
);

builder.Services.AddGrpc(options => 
                                {
                                    options.Interceptors.Add<ServerExceptionInterceptor>();
                                });

builder.Services.AddTransient<IAuthorizerDomainService, AuthorizerDomainService>();
builder.Services.AddOptions<JWTOptions>()
      .BindConfiguration(JWTOptions.Section)
      .ValidateDataAnnotations();

var app = builder.Build();
app.MapGrpcService<AuthorizerGrpcServer>().RequireHost("*:6000");
app.UseCors("MyPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapPost("/login", ([FromBody] LoginInfo login , [FromServices]IAuthorizerDomainService domainservice) => 
    {
      return domainservice.Login(login.Username, login.Password);
    });


app.Run();

public class LoginInfo
{
    [JsonPropertyName("user")]
    public string Username { get; set;}
    [JsonPropertyName("pass")]
    public string Password { get; set;}
}

