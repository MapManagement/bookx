using Grpc.Core;
using Bookx.Models;
using Bookx.Helpers;
using Bookx.ProtoServices;

namespace Bookx.Services;

public class AuthenticationService : Authenticator.AuthenticatorBase
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly BookxContext _bookxContext;

    public AuthenticationService(ILogger<AuthenticationService> logger)
    {
        _logger = logger;
    }

    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        return base.Login(request, context);
    }


}
