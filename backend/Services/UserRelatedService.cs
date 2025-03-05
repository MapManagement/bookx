using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Bookx.Models;
using Bookx.Helpers;
using Bookx.ProtoServices;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bookx.Services;

[Authorize]
public class UserRelatedService : UserService.UserServiceBase
{
    #region Fields

    private readonly ILogger<UserRelatedService> _logger;
    private readonly BookxContext _bookxContext;

    #endregion

    #region Constructor

    public UserRelatedService(ILogger<UserRelatedService> logger, BookxContext bookxContext)
    {
        _logger = logger;
        _bookxContext = bookxContext;
    }

    #endregion

    #region Overrides

    #region Read Operations

    public override Task<UserRelatedRequestReply> GetSingleOwnedBook(SingleOwnedBookRequest request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return Task.FromResult(CreateNotFoundReply($"Invalid claims in JWT"));

        OwnedBook dbOwnedBook = _bookxContext.OwnedBooks
            .Where(ob => ob.UserId == userId && ob.Id == request.OwnedBookId)
            .FirstOrDefault();

        // TODO: is there an easier way
        _bookxContext
            .Entry(dbOwnedBook)
            .Reference(o => o.Book)
            .Load();

        _bookxContext.Entry(dbOwnedBook.Book)
            .Collection(b => b.Authors)
            .Load();

        _bookxContext.Entry(dbOwnedBook.Book)
            .Collection(b => b.Genres)
            .Load();

        _bookxContext.Entry(dbOwnedBook.Book)
            .Reference(b => b.Publisher)
            .Load();

        _bookxContext.Entry(dbOwnedBook.Book)
                    .Reference(b => b.Language)
                    .Load();


        if (dbOwnedBook == null)
            return Task.FromResult(CreateNotFoundReply($"Couldn't find any owned book with ID {request.OwnedBookId}"));

        ReadSingleOwnedBook protoOwnedBook = ProtoDbEntityConverter.DbToProtoOwnedBook(dbOwnedBook);

        var protoReply = new UserRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            OwnedBook = protoOwnedBook
        };

        return Task.FromResult(protoReply);
    }

    public override Task<UserRelatedRequestReply> GetAllOwnedBooks(Empty request, ServerCallContext context)
    {
        return base.GetAllOwnedBooks(request, context);
    }

    public override Task<UserRelatedRequestReply> GetSingleTag(SingleTagRequest request, ServerCallContext context)
    {
        return base.GetSingleTag(request, context);
    }

    public override Task<UserRelatedRequestReply> GetAllUserTags(Empty request, ServerCallContext context)
    {
        return base.GetAllUserTags(request, context);
    }

    public override Task<UserRelatedRequestReply> GetOwnedBooksByTag(SingleTagRequest request, ServerCallContext context)
    {
        return base.GetOwnedBooksByTag(request, context);
    }

    #endregion

    #region Create Operations

    public override Task<SuccessReply> AddOwnedBook(WriteSingleOwnedBook request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return Task.FromResult(InvalidUserReply($"Invalid claims in JWT"));

        User dbUser = _bookxContext.Users.Find(userId);
        Book book = _bookxContext.Books.Find(request.Isbn);

        // TODO: retrieve book from some kind of API, improve message
        if (book == null)
            return Task.FromResult(InvalidUserReply($"Invalid book"));

        // TODO: improve message
        if (dbUser == null)
            return Task.FromResult(InvalidUserReply($"Invalid user"));

        OwnedBook newBook = new OwnedBook()
        {
            User = dbUser,
            Book = book,
            Rating = request.Rating,
            Comment = request.Comment,
            WouldRecommend = request.WouldRecommend,
            AddedAt = DateTime.UtcNow
        };

        _bookxContext.OwnedBooks.Add(newBook);
        _bookxContext.SaveChanges();

        SuccessReply reply = new SuccessReply()
        {
            Success = true
        };

        return Task.FromResult(reply);
    }

    public override Task<SuccessReply> AddTag(WriteSingleTag request, ServerCallContext context)
    {
        return base.AddTag(request, context);
    }

    #endregion

    #region Delete Operations

    public override Task<SuccessReply> RemoveOwnedBook(SingleOwnedBookRequest request, ServerCallContext context)
    {
        return base.RemoveOwnedBook(request, context);
    }

    public override Task<SuccessReply> RemoveTag(SingleTagRequest request, ServerCallContext context)
    {
        return base.RemoveTag(request, context);
    }

    #endregion

    #region Update Operations

    public override Task<SuccessReply> EditOwnedBook(WriteSingleOwnedBook request, ServerCallContext context)
    {
        return base.EditOwnedBook(request, context);
    }

    public override Task<SuccessReply> EditTag(WriteSingleTag request, ServerCallContext context)
    {
        return base.EditTag(request, context);
    }

    #endregion

    #endregion

    #region Methods

    private UserRelatedRequestReply CreateNotFoundReply(string messageText)
    {
        return new UserRelatedRequestReply()
        {
            Status = RequestStatus.NotFound,
            MessageText = messageText,
            Null = new Empty()
        };
    }

    private SuccessReply InvalidUserReply(string message)
    {
        return new SuccessReply()
        {
            Success = false,
            FailureMessage = message
        };
    }

    private int? GetUserIdFromClaim(ServerCallContext context)
    {
        var userIdClaim = context.GetHttpContext().User.FindFirst(JwtRegisteredClaimNames.Sub);

        int userId;
        var validUserId = Int32.TryParse(userIdClaim?.Value, out userId);

        if (userIdClaim == null || !validUserId)
            return null;

        return userId;
    }

    #endregion
}
