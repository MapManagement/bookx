using Bookx.Models;
using Bookx.Services;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Bookx.Authorization;

public class AccessOwnedLibraryAuthorizationHandler : AuthorizationHandler<AccessOwnedLibraryRequirement>
{
    private readonly ILogger<UserRelatedService> _logger;
    private readonly BookxContext _bookxContext;


    public AccessOwnedLibraryAuthorizationHandler(
            ILogger<UserRelatedService> logger,
            BookxContext bookxContext)
    {
        _logger = logger;
        _bookxContext = bookxContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessOwnedLibraryRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub);
        var userJwtVersionClaim = context.User.FindFirst(JwtRegisteredClaimNames.Jti);

        int userId;
        var validUserId = Int32.TryParse(userIdClaim?.Value, out userId);

        int userJwtVersion;
        var validUserJwtVersion = Int32.TryParse(userJwtVersionClaim?.Value, out userJwtVersion);

        if (userIdClaim == null || !validUserId || userJwtVersionClaim == null || !validUserJwtVersion)
        {
            context.Fail();
            return;
        }

        var authenticatedUser = await _bookxContext.Users.FindAsync(userId);

        if (authenticatedUser.JwtVersion <= userJwtVersion)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
