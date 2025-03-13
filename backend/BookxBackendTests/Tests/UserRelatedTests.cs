using BookxBackendTests.Factories;
using BookxBackendTests.Fixtures;
using Bookx.Services;
using Bookx.ProtoServices;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookxBackendTests.Tests
{

    public class UserRelatedTests : IClassFixture<BookxBackendTestFixture>
    {
        private readonly BookxBackendTestFixture _fixture;

        public UserRelatedTests(BookxBackendTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task SuccessfulLogin()
        {

        }
    }
}
