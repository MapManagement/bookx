using Bookx.Models;
using BookxBackendTests.Factories;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.Client;

namespace BookxBackendTests.Fixtures
{
    public class BookxBackendTestFixture : IDisposable
    {
        #region Properties

        public BookxBackendFactory BookxBackendFactory { get; }
        public HttpClient HttpClient { get; }
        public GrpcChannel GrpcChannel { get; }
        public IServiceScope ServiceScope { get; }
        public BookxContext DbContext { get; }

        #endregion

        #region Constructor

        public BookxBackendTestFixture()
        {
            BookxBackendFactory = new BookxBackendFactory();

            // Create an HttpClient from the factory.
            HttpClient = BookxBackendFactory.CreateDefaultClient();

            // TODO: change address
            GrpcChannel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions
            {
                HttpClient = HttpClient
            });

            // Create a scope to access registered services (like the DbContext).
            ServiceScope = BookxBackendFactory.Services.CreateScope();
            DbContext = ServiceScope.ServiceProvider.GetRequiredService<BookxContext>();

            DbContext.Database.EnsureCreated();
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            ServiceScope.Dispose();
            GrpcChannel.Dispose();
            HttpClient.Dispose();
            BookxBackendFactory.Dispose();
        }

        #endregion
    }
}
