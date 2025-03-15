using BookxBackendTests.Fixtures;
using BookxProtos;

namespace BookxBackendTests.Tests
{
    public class AuthenticatorTests : IClassFixture<BookxBackendTestFixture>
    {
        private readonly BookxBackendTestFixture _fixture;
        private readonly Authenticator.AuthenticatorClient _authenticatorGrpcClient;
        private readonly UserService.UserServiceClient _userRelatedGrcpClient;

        public AuthenticatorTests(BookxBackendTestFixture fixture)
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

        [Fact]
        public async Task EmptyUsernameRegister()
        {
            var registerRequest = new RegisterRequest()
            {
                Username = string.Empty,
                Password = "Kafka",
                MailAddress = "Franz.Kafka@mail.com"
            };

            var registerReply = await _authenticatorGrpcClient.RegisterAsync(registerRequest);

            Assert.NotNull(registerReply);
            Assert.False(registerReply.ValidRegistration);
            Assert.False(registerReply.HasToken);
            Assert.NotNull(registerReply.FailureMessage);
        }

        [Fact]
        public async Task EmptyPasswordRegister()
        {
            var registerRequest = new RegisterRequest()
            {
                Username = "Franz",
                Password = string.Empty,
                MailAddress = "Franz.Kafka@mail.com"
            };

            var registerReply = await _authenticatorGrpcClient.RegisterAsync(registerRequest);

            Assert.NotNull(registerReply);
            Assert.False(registerReply.ValidRegistration);
            Assert.False(registerReply.HasToken);
            Assert.NotNull(registerReply.FailureMessage);
        }

        [Fact]
        public async Task EmptyMailAddressRegister()
        {
            var registerRequest = new RegisterRequest()
            {
                Username = "Franz",
                Password = "Kafka",
                MailAddress = string.Empty
            };

            var registerReply = await _authenticatorGrpcClient.RegisterAsync(registerRequest);

            Assert.NotNull(registerReply);
            Assert.False(registerReply.ValidRegistration);
            Assert.False(registerReply.HasToken);
            Assert.NotNull(registerReply.FailureMessage);
        }

        [Fact]
        public async Task EmptyRegister()
        {
            var registerRequest = new RegisterRequest()
            {
                Username = string.Empty,
                Password = string.Empty,
                MailAddress = string.Empty
            };

            var registerReply = await _authenticatorGrpcClient.RegisterAsync(registerRequest);

            Assert.NotNull(registerReply);
            Assert.False(registerReply.ValidRegistration);
            Assert.False(registerReply.HasToken);
            Assert.NotNull(registerReply.FailureMessage);
        }
    }
}
