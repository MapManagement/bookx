using Bookx.Services;
using Bookx.Models;
using Bookx.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
// TODO: in production?
builder.Services.AddGrpcReflection();
builder.Services.AddDbContext<BookxContext>();
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

builder.Services.AddAuthorization();

var app = builder.Build();

// TODO: in production?
app.MapGrpcReflectionService();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.MapGrpcService<AuthenticationService>();
app.MapGrpcService<BookRelatedService>();
app.MapGrpcService<UserRelatedService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
