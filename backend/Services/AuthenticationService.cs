using Grpc.Core;
using Bookx.Models;
using Bookx.Helpers;
using Bookx.ProtoServices;

namespace Bookx.Services;

public class AuthenticationService : Authenticator.AuthenticatorBase
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly BookxContext _bookxContext;

    public AuthenticationService(ILogger<AuthenticationService> logger, BookxContext bookxContext)
    {
        _logger = logger;
        _bookxContext = bookxContext;
    }

    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var loginUser = _bookxContext.Users
            .Where(u => u.Username == request.Username)
            .FirstOrDefault();

        var loginReply = new LoginReply()
        {
            ValidLogin = false,
            Token = string.Empty
        };

        if (loginUser is null)
        {
            loginReply.FailureMessage = "No username has been specified.";
            return Task.FromResult(loginReply);
        }

        bool isValidLogin = CryptographyHelper.VerfiyPasswordHash(
                request.Password,
                loginUser.PasswordSalt,
                loginUser.Password
        );

        if (!isValidLogin)
        {
            loginReply.FailureMessage = "Login credentials are wrong.";
            return Task.FromResult(loginReply);
        }

        var jwt = CryptographyHelper.GenerateJwt(request.Username);

        loginReply.ValidLogin = true;
        loginReply.Token = jwt;

        return Task.FromResult(loginReply);
    }
}
