using Bookx.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BookxBackendTests.Factories
{
    public class BookxBackendFactory : WebApplicationFactory<Program>, IDisposable
    {
        #region Fields

        // database is released from memory as soon as the last connection
        // has been terminated, so keep one in a private field for testing
        private SqliteConnection _dbConnection;

        #endregion

        #region Overrides

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<BookxContext>();
                services.RemoveAll<IDbContextOptionsConfiguration<BookxContext>>();

                _dbConnection = new SqliteConnection("DataSource=:memory:");
                _dbConnection.Open();

                services.AddDbContext<BookxContext>(options =>
                {
                    options.UseSqlite(_dbConnection);
                });

                services.AddGrpcReflection();
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _dbConnection.Dispose();
            }
        }

        #endregion
    }
}
