using BookxBackendTests.Fixtures;
using BookxProtos;

namespace BookxBackendTests.Tests
{
    public class AuthenticatorTests : IClassFixture<BookxBackendTestFixture>
    {
        private readonly BookxBackendTestFixture _fixture;
        private readonly Authenticator.AuthenticatorClient _authenticatorGrpcClient;

        public AuthenticatorTests(BookxBackendTestFixture fixture)
        {
            _fixture = fixture;
            _authenticatorGrpcClient = new Authenticator.AuthenticatorClient(_fixture.GrpcChannel);
        }

        #region Methods

        private async Task<RegisterReply> RegisterNewUser(string username, string password, string mailAddress)
        {
            var registerRequest = new RegisterRequest()
            {
                Username = username,
                Password = password,
                MailAddress = mailAddress
            };

            return await _authenticatorGrpcClient.RegisterAsync(registerRequest);
        }

        #endregion

        #region Tests

        [Theory]
        [InlineData("Herman", "Hesse", "Herman.Hesse@mail.com")]
        [InlineData("Ernst Theodor Amadeus", "Hoffmann", "ETA.Hoffmann@mail.com")]
        [InlineData("Friedrich", "Dürrenmatt", "Friedrich.Dürrenmatt@mail.com")]
        public async Task SuccessfulRegister(string username, string password, string mailAddress)
        {
            var registerRequest = new RegisterRequest()
            {
                Username = username,
                Password = password,
                MailAddress = mailAddress
            };

            var registerReply = await _authenticatorGrpcClient.RegisterAsync(registerRequest);

            Assert.NotNull(registerReply);
            Assert.True(registerReply.ValidRegistration);
            Assert.NotNull(registerReply.Token);
        }

        [Theory]
        [InlineData("", "Brecht", "Bertolt.Brecht@mail.com")]
        [InlineData("Patrick", "", "Patrick.Süskind@mail.com")]
        [InlineData("Friedrich", "Schiller", "")]
        [InlineData("", "", "")]
        public async Task FailedRegister(string username, string password, string mailAddress)
        {
            var registerRequest = new RegisterRequest()
            {
                Username = username,
                Password = password,
                MailAddress = mailAddress
            };

            var registerReply = await _authenticatorGrpcClient.RegisterAsync(registerRequest);

            Assert.NotNull(registerReply);
            Assert.False(registerReply.ValidRegistration);
            Assert.False(registerReply.HasToken);
            Assert.NotNull(registerReply.FailureMessage);
        }


        [Theory]
        [InlineData("Franz", "Kafka", "Franz.Kafka@mail.com")]
        public async Task SuccessfulLogin(string username, string password, string mailAddress)
        {
            var registerReply = await RegisterNewUser(username, password, mailAddress);

            Assert.True(registerReply.ValidRegistration);

            var loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };

            var loginReply = await _authenticatorGrpcClient.LoginAsync(loginRequest);

            Assert.NotNull(loginReply);
            Assert.True(loginReply.ValidLogin);
            Assert.NotNull(loginReply.Token);
        }

        [Theory]
        [InlineData("", "Maria-Remarque")]
        [InlineData("Erich Maria", "")]
        [InlineData("", "")]
        public async Task FailedLogin(string username, string password)
        {
            var loginRequest = new LoginRequest()
            {
                Username = username,
                Password = password
            };

            var loginReply = await _authenticatorGrpcClient.LoginAsync(loginRequest);

            Assert.NotNull(loginReply);
            Assert.False(loginReply.ValidLogin);
            Assert.False(loginReply.HasToken);
            Assert.NotNull(loginReply.FailureMessage);
        }

        #endregion
    }
}
