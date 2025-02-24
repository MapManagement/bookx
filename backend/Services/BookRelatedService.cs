using Grpc.Core;
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
    public override Task<SingleAuthor> GetSingleAuthor(SingleAuthorRequest request, ServerCallContext context)
    {
        var dbAuthor = _bookxContext.Find<Author>(request.Id);
        var protoAuthor = ProtoDbEntityConverter.DbToProtoAuthor(dbAuthor);

        return Task.FromResult(protoAuthor);
    }

    [Authorize]
    public override Task<SingleGenre> GetSingleGenre(SingleGenreRequest request, ServerCallContext context)
    {
        var dbGenre = _bookxContext.Find<Genre>(request.Id);
        var protoGenre = ProtoDbEntityConverter.DbToProtoGenre(dbGenre);

        return Task.FromResult(protoGenre);
    }

    [Authorize]
    public override Task<SingleLanguage> GetSingleLanguage(SingleLanguageRequest request, ServerCallContext context)
    {
        var dbLanguage = _bookxContext.Find<Language>(request.Id);
        var protoLanguage = ProtoDbEntityConverter.DbToProtoLanguage(dbLanguage);

        return Task.FromResult(protoLanguage);
    }

    [Authorize]
    public override Task<SinglePublisher> GetSinglePublisher(SinglePublisherRequest request, ServerCallContext context)
    {
        var dbPublisher = _bookxContext.Find<Publisher>(request.Id);
        var protoPublisher = ProtoDbEntityConverter.DbToProtoPublisher(dbPublisher);

        return Task.FromResult(protoPublisher);
    }

    [Authorize]
    public override Task<SingleBook> GetSingleBook(SingleBookRequest request, ServerCallContext context)
    {
        var dbBook = _bookxContext.Find<Book>(request.Isbn);
        var protoBook = ProtoDbEntityConverter.DbToProtoBook(dbBook);

        return Task.FromResult(protoBook);
    }

    #endregion
}
