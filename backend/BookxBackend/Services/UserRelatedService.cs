using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Bookx.Models;
using Bookx.Helpers;
using BookxProtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace Bookx.Services;

[Authorize]
public class UserRelatedService : UserService.UserServiceBase
{
    #region Fields

    private const string InvalidUserIdClaimMessage = "Invalid user ID claim in JWT";

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

    public override async Task<UserRelatedRequestReply> GetSingleOwnedBook(SingleOwnedBookRequest request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return CreateNotFoundReply(InvalidUserIdClaimMessage);

        OwnedBook dbOwnedBook = await _bookxContext.OwnedBooks
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Language)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Authors)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Genres)
            .Include(ob => ob.Book)
                .ThenInclude(b => b.Publisher)
            .SingleAsync(ob => ob.UserId == userId && ob.BookIsbn == request.Isbn);

        if (dbOwnedBook == null)
            return CreateNotFoundReply($"Couldn't find any owned book with ISBN \"{request.Isbn}\"");

        ReadSingleOwnedBook protoOwnedBook = ProtoDbEntityConverter.DbToProtoOwnedBook(dbOwnedBook);

        var protoReply = new UserRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            OwnedBook = protoOwnedBook
        };

        return protoReply;
    }

    public override async Task<UserRelatedRequestReply> GetAllOwnedBooks(Empty request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return CreateNotFoundReply(InvalidUserIdClaimMessage);

        List<OwnedBook> dbOwnedBooks = await _bookxContext.OwnedBooks
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
            .ToListAsync();

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

        return protoReply;
    }

    public override async Task<UserRelatedRequestReply> GetSingleTag(SingleTagRequest request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return CreateNotFoundReply(InvalidUserIdClaimMessage);

        Tag dbTag = await _bookxContext.Tags
            .SingleAsync(t => t.UserId == userId && t.Id == request.TagId);

        if (dbTag == null)
            return CreateNotFoundReply($"Couldn't find any tag with ID {request.TagId}");

        ReadSingleTag protoTag = ProtoDbEntityConverter.DbToProtoTag(dbTag);

        var protoReply = new UserRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            Tag = protoTag
        };

        return protoReply;
    }

    public override async Task<UserRelatedRequestReply> GetAllUserTags(Empty request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return CreateNotFoundReply(InvalidUserIdClaimMessage);

        List<Tag> dbTags = await _bookxContext.Tags
            .Where(t => t.UserId == userId)
            .ToListAsync();

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

        return protoReply;
    }

    public override async Task<UserRelatedRequestReply> GetOwnedBooksByTag(SingleTagRequest request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return CreateNotFoundReply(InvalidUserIdClaimMessage);

        List<OwnedBook> dbOwnedBooks = await _bookxContext.OwnedBooks
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
            .ToListAsync();

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

        return protoReply;
    }

    #endregion

    #region Create Operations

    public override async Task<SuccessReply> AddOwnedBook(AddSingleOwnedBook request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return InvalidInputReply(InvalidUserIdClaimMessage);

        User dbUser = await _bookxContext.Users.FindAsync(userId);
        Book book = await _bookxContext.Books.FindAsync(request.Isbn);

        if (book == null)
        {
            book = await BookApiHelper.RetrieveBookByIsbn(request.Isbn, _bookxContext);

            if (book == null)
                return InvalidInputReply("Invalid ISBN");
        }

        if (dbUser == null)
            return InvalidInputReply("No");

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
        await _bookxContext.SaveChangesAsync();

        SuccessReply reply = new SuccessReply()
        {
            Success = true,
            ObjectId = newBook.BookIsbn
        };

        return reply;
    }

    public override async Task<SuccessReply> AddTag(AddSingleTag request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return InvalidInputReply(InvalidUserIdClaimMessage);

        User dbUser = _bookxContext.Users.Find(userId);

        if (dbUser == null)
            return InvalidInputReply(BuildInvalidUserIdMessage(userId.Value));

        if (string.IsNullOrWhiteSpace(request.Name))
            return InvalidInputReply("Tag name cannot be empty or only whitespace");

        bool tagNameExists = await _bookxContext.Tags
            .AnyAsync(t => t.Name.ToLower() == request.Name.ToLower() && t.UserId == userId);

        if (tagNameExists)
            return InvalidInputReply($"Tag name \"{request.Name}\" is already in use");

        if (!IsHexColorValid(request.Color))
            return InvalidInputReply($"String \"{request.Color}\" is no valid hexadecimal color code");

        Tag newTag = new Tag()
        {
            Name = request.Name,
            Color = request.Color,
            User = dbUser
        };

        _bookxContext.Tags.Add(newTag);
        await _bookxContext.SaveChangesAsync();

        SuccessReply reply = new SuccessReply()
        {
            Success = true,
            ObjectId = newTag.Id.ToString()
        };

        return reply;
    }

    #endregion

    #region Delete Operations

    public override async Task<SuccessReply> RemoveOwnedBook(SingleOwnedBookRequest request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return InvalidInputReply(InvalidUserIdClaimMessage);

        if (string.IsNullOrWhiteSpace(request.Isbn))
            return InvalidInputReply("ID of owned book cannot be null");

        User dbUser = await _bookxContext.Users.FindAsync(userId);

        if (dbUser == null)
            return InvalidInputReply(BuildInvalidUserIdMessage(userId.Value));

        OwnedBook ownedBook = await _bookxContext.OwnedBooks
            .SingleOrDefaultAsync(ob => ob.BookIsbn == request.Isbn && ob.UserId == userId);

        if (ownedBook == null)
            return InvalidInputReply($"Couldn't find any owned book with ISBN \"{request.Isbn}\"");

        _bookxContext.OwnedBooks.Remove(ownedBook);
        await _bookxContext.SaveChangesAsync();

        SuccessReply reply = new SuccessReply()
        {
            Success = true
        };

        return reply;
    }

    public override async Task<SuccessReply> RemoveTag(SingleTagRequest request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return InvalidInputReply(InvalidUserIdClaimMessage);

        User dbUser = await _bookxContext.Users.FindAsync(userId);
        Tag tag = await _bookxContext.Tags
            .SingleOrDefaultAsync(t => t.Id == request.TagId && t.UserId == userId);

        if (tag == null)
            return InvalidInputReply($"Couldn't find any tag with ID {request.TagId}");

        if (dbUser == null)
            return InvalidInputReply(BuildInvalidUserIdMessage(userId.Value));

        _bookxContext.Tags.Remove(tag);
        await _bookxContext.SaveChangesAsync();

        SuccessReply reply = new SuccessReply()
        {
            Success = true
        };

        return reply;
    }

    #endregion

    #region Update Operations

    public override async Task<SuccessReply> EditOwnedBook(EditSingleOwnedBook request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return InvalidInputReply(InvalidUserIdClaimMessage);

        User dbUser = await _bookxContext.Users.FindAsync(userId);
        OwnedBook ownedBook = await _bookxContext.OwnedBooks
            .SingleOrDefaultAsync(ob => ob.BookIsbn == request.Isbn && ob.UserId == userId);

        if (ownedBook == null)
            return InvalidInputReply($"Couldn't find any owned book with ISBN \"{request.Isbn}\"");

        if (dbUser == null)
            return InvalidInputReply(BuildInvalidUserIdMessage(userId.Value));

        ownedBook.Rating = request.Rating;
        ownedBook.Comment = request.Comment;
        ownedBook.WouldRecommend = request.WouldRecommend;

        await _bookxContext.SaveChangesAsync();

        SuccessReply reply = new SuccessReply()
        {
            Success = true
        };

        return reply;
    }

    public override async Task<SuccessReply> EditTag(EditSingleTag request, ServerCallContext context)
    {
        int? userId = GetUserIdFromClaim(context);

        if (userId == null)
            return InvalidInputReply(InvalidUserIdClaimMessage);

        User dbUser = await _bookxContext.Users.FindAsync(userId);
        Tag tag = await _bookxContext.Tags
            .SingleOrDefaultAsync(t => t.Id == request.Id && t.UserId == userId);

        if (tag == null)
            return InvalidInputReply($"Couldn't find any tag with ID {request.Id}");

        if (dbUser == null)
            return InvalidInputReply(BuildInvalidUserIdMessage(userId.Value));

        tag.Name = request.Name;
        tag.Color = request.Color;

        await _bookxContext.SaveChangesAsync();

        SuccessReply reply = new SuccessReply()
        {
            Success = true
        };

        return reply;
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

    private SuccessReply InvalidInputReply(string messageText)
    {
        return new SuccessReply()
        {
            Success = false,
            FailureMessage = messageText
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

    private string BuildInvalidUserIdMessage(int userId)
    {
        return $"The user with the ID {userId} has not been found";
    }

    private bool IsHexColorValid(string hexColor)
    {
        if (string.IsNullOrWhiteSpace(hexColor))
            return false;

        Regex validHexColor = new Regex(@"^#([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$");

        return validHexColor.IsMatch(hexColor);
    }

    #endregion
}
