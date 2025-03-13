using Bookx.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookxBackendTests.Factories
{
    public class BookxBackendFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<BookxContext>)
                );

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<BookxContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryBookxTestDb");
                });

                services.AddGrpcReflection();
            });
        }
    }
}
