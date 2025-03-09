using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Bookx.Models;
using Bookx.Helpers;
using Bookx.ProtoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

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
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Language)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Authors)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Genres)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Publisher)
            .Single(ob => ob.UserId == userId && ob.Id == request.OwnedBookId);

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
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return Task.FromResult(CreateNotFoundReply($"Invalid claims in JWT"));

        List<OwnedBook> dbOwnedBooks = _bookxContext.OwnedBooks
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Language)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Authors)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Genres)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Publisher)
            .Include(ob => ob.Tags)
            .Where(ob => ob.UserId == userId)
            .ToList();

        var protoOwnedBooks = new ReadMultipleOwnedBooks();

        foreach (OwnedBook dbOwnedBook in dbOwnedBooks)
        {
            ReadSingleOwnedBook protoOwnedBook = ProtoDbEntityConverter.DbToProtoOwnedBook(dbOwnedBook);

            protoOwnedBooks.OwnedBooks.Add(protoOwnedBook);
        }

        var protoReply = new UserRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            MultipleOwnedBooks = protoOwnedBooks
        };

        return Task.FromResult(protoReply);
    }

    public override Task<UserRelatedRequestReply> GetSingleTag(SingleTagRequest request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return Task.FromResult(CreateNotFoundReply($"Invalid claims in JWT"));

        Tag dbTag = _bookxContext.Tags
            .Single(t => t.UserId == userId && t.Id == request.TagId);

        if (dbTag == null)
            return Task.FromResult(CreateNotFoundReply($"Couldn't find any tag with ID {request.TagId}"));

        ReadSingleTag protoTag = ProtoDbEntityConverter.DbToProtoTag(dbTag);

        var protoReply = new UserRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            Tag = protoTag
        };

        return Task.FromResult(protoReply);
    }

    public override Task<UserRelatedRequestReply> GetAllUserTags(Empty request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return Task.FromResult(CreateNotFoundReply($"Invalid claims in JWT"));

        List<Tag> dbTags = _bookxContext.Tags
            .Where(t => t.UserId == userId)
            .ToList();

        var protoTags = new ReadMultipleTags();

        foreach (Tag dbTag in dbTags)
        {
            ReadSingleTag protoTag = ProtoDbEntityConverter.DbToProtoTag(dbTag);

            protoTags.UserTags.Add(protoTag);
        }

        var protoReply = new UserRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            MultipleTags = protoTags
        };

        return Task.FromResult(protoReply);
    }

    public override Task<UserRelatedRequestReply> GetOwnedBooksByTag(SingleTagRequest request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return Task.FromResult(CreateNotFoundReply($"Invalid claims in JWT"));

        List<OwnedBook> dbOwnedBooks = _bookxContext.OwnedBooks
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Language)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Authors)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Genres)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Publisher)
            .Include(ob => ob.Tags)
            .Where(ob => ob.UserId == userId && ob.Tags.Select(t => t.Id).Contains(request.TagId))
            .ToList();

        var protoOwnedBooks = new ReadMultipleOwnedBooks();

        foreach (OwnedBook dbOwnedBook in dbOwnedBooks)
        {
            ReadSingleOwnedBook protoOwnedBook = ProtoDbEntityConverter.DbToProtoOwnedBook(dbOwnedBook);

            protoOwnedBooks.OwnedBooks.Add(protoOwnedBook);
        }

        var protoReply = new UserRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            MultipleOwnedBooks = protoOwnedBooks
        };

        return Task.FromResult(protoReply);
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
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return Task.FromResult(InvalidUserReply($"Invalid claims in JWT"));

        User dbUser = _bookxContext.Users.Find(userId);

        // TODO: improve message
        if (dbUser == null)
            return Task.FromResult(InvalidUserReply($"Invalid user"));

        bool tagNameExists = _bookxContext.Tags
            .Any(t => t.Name.ToLower() == request.Name.ToLower() && t.UserId == userId);

        if (tagNameExists)
        {
            return Task.FromResult(InvalidUserReply($"Tag name is already in use"));
        }

        Tag newTag = new Tag()
        {
            Name = request.Name,
            Color = request.Color,
            User = dbUser
        };

        _bookxContext.Tags.Add(newTag);
        _bookxContext.SaveChanges();

        SuccessReply reply = new SuccessReply()
        {
            Success = true
        };

        return Task.FromResult(reply);
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
