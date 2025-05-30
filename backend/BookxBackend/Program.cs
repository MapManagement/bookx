using Bookx.Services;
using Bookx.Models;
using Bookx.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
// TODO: in production?
builder.Services.AddGrpcReflection();
builder.Services.AddDbContext<BookxContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateActor = false,
                ValidateLifetime = true,
                IssuerSigningKey = CryptographyHelper.JwtSecurityKey
            };
        options.MapInboundClaims = false;
    });

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddAuthorization();

var app = builder.Build();

// TODO: in production?
app.MapGrpcReflectionService();

app.UseGrpcWeb();
app.UseCors();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<AuthenticationService>()
    .EnableGrpcWeb()
    .RequireCors("AllowAll");

app.MapGrpcService<BookRelatedService>()
    .EnableGrpcWeb()
    .RequireCors("AllowAll");

app.MapGrpcService<UserRelatedService>()
    .EnableGrpcWeb()
    .RequireCors("AllowAll");

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program { }
