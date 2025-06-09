using Grpc.Core;
using Bookx.Models;
using Bookx.Helpers;
using BookxProtos;

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

    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var loginUser = _bookxContext.Users.FirstOrDefault(u => u.Username == request.Username);

        var loginReply = new LoginReply()
        {
            ValidLogin = false,
        };

        if (loginUser == null || string.IsNullOrWhiteSpace(request.Username))
        {
            loginReply.FailureMessage = "Login credentials are wrong.";
            return loginReply;
        }

        bool isValidLogin = CryptographyHelper.VerfiyPasswordHash(
                request.Password,
                loginUser.PasswordSalt,
                loginUser.Password
        );

        if (!isValidLogin)
        {
            loginReply.FailureMessage = "Login credentials are wrong.";
            return loginReply;
        }

        var jwt = await RetrieveNewJwt(loginUser);

        loginReply.ValidLogin = true;
        loginReply.Token = jwt;

        return loginReply;
    }

    public override async Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
    {
        var registerReply = new RegisterReply()
        {
            ValidRegistration = false,
        };

        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.MailAddress))
        {
            registerReply.FailureMessage = "All fields are required.";
            return registerReply;
        }

        var usernameAlreadyTaken = _bookxContext.Users
            .Where(u => u.Username == request.Username)
            .Any();

        var emailAddressAlreadyTaken = _bookxContext.Users
            .Where(u => u.MailAddress == request.MailAddress)
            .Any();

        if (usernameAlreadyTaken)
        {
            registerReply.FailureMessage = "The username is not available.";
            return registerReply;
        }

        if (emailAddressAlreadyTaken)
        {
            registerReply.FailureMessage = "This mail address has already been registered.";
            return registerReply;
        }

        (string passwordHash, string passwordSalt) = CryptographyHelper.CreatePasswordHash(request.Password);

        User newUser = new User()
        {
            Username = request.Username,
            Password = passwordHash,
            PasswordSalt = passwordSalt,
            JoinDatetime = DateTime.Now.ToUniversalTime(),
            MailAddress = request.MailAddress,
            JwtVersion = 0
        };

        _bookxContext.Users.Add(newUser);
        await _bookxContext.SaveChangesAsync();

        var jwt = await RetrieveNewJwt(newUser);

        registerReply.ValidRegistration = true;
        registerReply.Token = jwt;

        return registerReply;
    }

    private async Task<string> RetrieveNewJwt(User loginUser)
    {
        // TODO: race conditions?
        int newJwtVersion = ++loginUser.JwtVersion;
        await _bookxContext.SaveChangesAsync();

        var jwt = CryptographyHelper.GenerateJwt(loginUser.Id, loginUser.Username, newJwtVersion);

        return jwt;
    }
}
