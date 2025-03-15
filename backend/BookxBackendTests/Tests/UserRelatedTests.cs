using BookxBackendTests.Fixtures;
using BookxProtos;
using Xunit;

namespace BookxBackendTests.Tests
{
    public class UserRelatedTests : IClassFixture<BookxBackendTestFixture>
    {
        private readonly BookxBackendTestFixture _fixture;
        private readonly Authenticator.AuthenticatorClient _authenticatorGrpcClient;
        private readonly UserService.UserServiceClient _userRelatedGrcpClient;

        public UserRelatedTests(BookxBackendTestFixture fixture)
        {
            _fixture = fixture;
            _authenticatorGrpcClient = new Authenticator.AuthenticatorClient(_fixture.GrpcChannel);
            _userRelatedGrcpClient = new UserService.UserServiceClient(_fixture.GrpcChannel);
        }

        [Fact]
        public async Task SuccessfulRegister()
        {
            var registerRequest = new RegisterRequest()
            {
                Username = "Franz",
                Password = "Kafka",
                MailAddress = "Franz.Kafka@mail.com"
            };

            var registerReply = await _authenticatorGrpcClient.RegisterAsync(registerRequest);

            Assert.NotNull(registerReply);
            Assert.True(registerReply.ValidRegistration);
            Assert.NotNull(registerReply.Token);
        }
    }
}
