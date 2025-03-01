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
    public override Task<RequestReply> GetSingleAuthor(SingleAuthorRequest request, ServerCallContext context)
    {
        var dbAuthor = _bookxContext.Find<Author>(request.Id);

        if (dbAuthor == null)
            return Task.FromResult(CreateNotFoundReply($"Couldn't find any author with ID {request.Id}"));

        var protoAuthor = ProtoDbEntityConverter.DbToProtoAuthor(dbAuthor);

        var protoReply = new RequestReply()
        {
            Status = RequestStatus.Found,
            Author = protoAuthor
        };

        return Task.FromResult(protoReply);
    }

    [Authorize]
    public override Task<RequestReply> GetSingleGenre(SingleGenreRequest request, ServerCallContext context)
    {
        var dbGenre = _bookxContext.Find<Genre>(request.Id);

        if (dbGenre == null)
            return Task.FromResult(CreateNotFoundReply($"Couldn't find any genre with ID {request.Id}"));

        var protoGenre = ProtoDbEntityConverter.DbToProtoGenre(dbGenre);

        var protoReply = new RequestReply()
        {
            Status = RequestStatus.Found,
            Genre = protoGenre
        };

        return Task.FromResult(protoReply);
    }

    [Authorize]
    public override Task<RequestReply> GetSingleLanguage(SingleLanguageRequest request, ServerCallContext context)
    {
        var dbLanguage = _bookxContext.Find<Language>(request.Id);

        if (dbLanguage == null)
            return Task.FromResult(CreateNotFoundReply($"Couldn't find any language with ID {request.Id}"));

        var protoLanguage = ProtoDbEntityConverter.DbToProtoLanguage(dbLanguage);

        var protoReply = new RequestReply()
        {
            Status = RequestStatus.Found,
            Language = protoLanguage
        };

        return Task.FromResult(protoReply);
    }

    [Authorize]
    public override Task<RequestReply> GetSinglePublisher(SinglePublisherRequest request, ServerCallContext context)
    {
        var dbPublisher = _bookxContext.Find<Publisher>(request.Id);

        if (dbPublisher == null)
            return Task.FromResult(CreateNotFoundReply($"Couldn't find any publisher with ID {request.Id}"));

        var protoPublisher = ProtoDbEntityConverter.DbToProtoPublisher(dbPublisher);

        var protoReply = new RequestReply()
        {
            Status = RequestStatus.Found,
            Publisher = protoPublisher
        };

        return Task.FromResult(protoReply);
    }

    [Authorize]
    public override Task<RequestReply> GetSingleBook(SingleBookRequest request, ServerCallContext context)
    {
        var dbBook = _bookxContext.Find<Book>(request.Isbn);

        if (dbBook == null)
            return Task.FromResult(CreateNotFoundReply($"Couldn't find any book with ISBN {request.Isbn}"));

        var protoBook = ProtoDbEntityConverter.DbToProtoBook(dbBook);

        var protoReply = new RequestReply()
        {
            Status = RequestStatus.Found,
            Book = protoBook
        };

        return Task.FromResult(protoReply);
    }

    #endregion

    #region Methods

    private RequestReply CreateNotFoundReply(string messageText)
    {
        return new RequestReply()
        {
            Status = RequestStatus.NotFound,
            MessageText = messageText,
            Null = new Empty()
        };
    }

    #endregion
}
