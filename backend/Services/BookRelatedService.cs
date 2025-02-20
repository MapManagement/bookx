namespace Bookx.Services;

public class BookRelatedService : Books.BooksBase
{
    private readonly ILogger<BookRelatedService> _logger;
    public BookRelatedService(ILogger<BookRelatedService> logger)
    {
        _logger = logger;
    }

    /*public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }*/
}
