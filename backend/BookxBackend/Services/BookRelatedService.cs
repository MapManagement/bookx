using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Bookx.Models;
using Bookx.Helpers;
using Bookx.ProtoServices;
using Microsoft.AspNetCore.Authorization;

namespace Bookx.Services;

public class BookRelatedService : BookService.BookServiceBase
{
    #region Fields
    private readonly ILogger<BookRelatedService> _logger;
    private readonly BookxContext _bookxContext;

    #endregion

    #region Constructor

    public BookRelatedService(ILogger<BookRelatedService> logger, BookxContext bookxContext)
    {
        _logger = logger;
        _bookxContext = bookxContext;
    }

    #endregion

    #region Overrides

    [Authorize]
    public override async Task<BookRelatedRequestReply> GetSingleAuthor(SingleAuthorRequest request, ServerCallContext context)
    {
        var dbAuthor = await _bookxContext.FindAsync<Author>(request.Id);

        if (dbAuthor == null)
            return CreateNotFoundReply($"Couldn't find any author with ID {request.Id}");

        var protoAuthor = ProtoDbEntityConverter.DbToProtoAuthor(dbAuthor);

        var protoReply = new BookRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            Author = protoAuthor
        };

        return protoReply;
    }

    [Authorize]
    public override async Task<BookRelatedRequestReply> GetSingleGenre(SingleGenreRequest request, ServerCallContext context)
    {
        var dbGenre = await _bookxContext.FindAsync<Genre>(request.Id);

        if (dbGenre == null)
            return CreateNotFoundReply($"Couldn't find any genre with ID {request.Id}");

        var protoGenre = ProtoDbEntityConverter.DbToProtoGenre(dbGenre);

        var protoReply = new BookRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            Genre = protoGenre
        };

        return protoReply;
    }

    [Authorize]
    public override async Task<BookRelatedRequestReply> GetSingleLanguage(SingleLanguageRequest request, ServerCallContext context)
    {
        var dbLanguage = await _bookxContext.FindAsync<Language>(request.Id);

        if (dbLanguage == null)
            return CreateNotFoundReply($"Couldn't find any language with ID {request.Id}");

        var protoLanguage = ProtoDbEntityConverter.DbToProtoLanguage(dbLanguage);

        var protoReply = new BookRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            Language = protoLanguage
        };

        return protoReply;
    }

    [Authorize]
    public override async Task<BookRelatedRequestReply> GetSinglePublisher(SinglePublisherRequest request, ServerCallContext context)
    {
        var dbPublisher = await _bookxContext.FindAsync<Publisher>(request.Id);

        if (dbPublisher == null)
            return CreateNotFoundReply($"Couldn't find any publisher with ID {request.Id}");

        var protoPublisher = ProtoDbEntityConverter.DbToProtoPublisher(dbPublisher);

        var protoReply = new BookRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            Publisher = protoPublisher
        };

        return protoReply;
    }

    [Authorize]
    public override async Task<BookRelatedRequestReply> GetSingleBook(SingleBookRequest request, ServerCallContext context)
    {
        var dbBook = await _bookxContext.FindAsync<Book>(request.Isbn);

        if (dbBook == null)
            return CreateNotFoundReply($"Couldn't find any book with ISBN {request.Isbn}");

        var protoBook = ProtoDbEntityConverter.DbToProtoBook(dbBook);

        var protoReply = new BookRelatedRequestReply()
        {
            Status = RequestStatus.Found,
            Book = protoBook
        };

        return protoReply;
    }

    #endregion

    #region Methods

    private BookRelatedRequestReply CreateNotFoundReply(string messageText)
    {
        return new BookRelatedRequestReply()
        {
            Status = RequestStatus.NotFound,
            MessageText = messageText,
            Null = new Empty()
        };
    }

    #endregion
}
