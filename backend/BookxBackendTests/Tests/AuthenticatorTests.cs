using BookxProtos;
using BookxBackendTests.Helpers;

namespace BookxBackendTests.Tests
{
    public class AuthenticatorTests
    {
        #region Tests

        [Theory]
        [InlineData("Herman", "Hesse", "Herman.Hesse@mail.com")]
        [InlineData("Ernst Theodor Amadeus", "Hoffmann", "ETA.Hoffmann@mail.com")]
        [InlineData("Friedrich", "Dürrenmatt", "Friedrich.Dürrenmatt@mail.com")]
        public async Task SuccessfulRegister(string username, string password, string mailAddress)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerRequest = new RegisterRequest()
                {
                    Username = username,
                    Password = password,
                    MailAddress = mailAddress
                };

                var registerReply = await authClient.RegisterAsync(registerRequest);

                Assert.NotNull(registerReply);
                Assert.True(registerReply.ValidRegistration);
                Assert.NotNull(registerReply.Token);
            }
        }

        [Theory]
        [InlineData("", "Brecht", "Bertolt.Brecht@mail.com")]
        [InlineData("Patrick", "", "Patrick.Süskind@mail.com")]
        [InlineData("Friedrich", "Schiller", "")]
        [InlineData("", "", "")]
        public async Task FailedRegister(string username, string password, string mailAddress)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerRequest = new RegisterRequest()
                {
                    Username = username,
                    Password = password,
                    MailAddress = mailAddress
                };

                var registerReply = await authClient.RegisterAsync(registerRequest);

                Assert.NotNull(registerReply);
                Assert.False(registerReply.ValidRegistration);
                Assert.False(registerReply.HasToken);
                Assert.NotNull(registerReply.FailureMessage);
            }
        }


        [Theory]
        [InlineData("Franz", "Kafka", "Franz.Kafka@mail.com")]
        public async Task SuccessfulLogin(string username, string password, string mailAddress)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(authClient, username, password, mailAddress);

                Assert.True(registerReply.ValidRegistration);

                var loginRequest = new LoginRequest()
                {
                    Username = username,
                    Password = password
                };

                var loginReply = await authClient.LoginAsync(loginRequest);

                Assert.NotNull(loginReply);
                Assert.True(loginReply.ValidLogin);
                Assert.NotNull(loginReply.Token);
            }
        }

        [Theory]
        [InlineData("", "Maria-Remarque")]
        [InlineData("Erich Maria", "")]
        [InlineData("", "")]
        public async Task FailedLogin(string username, string password)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var loginRequest = new LoginRequest()
                {
                    Username = username,
                    Password = password
                };

                var loginReply = await authClient.LoginAsync(loginRequest);

                Assert.NotNull(loginReply);
                Assert.False(loginReply.ValidLogin);
                Assert.False(loginReply.HasToken);
                Assert.NotNull(loginReply.FailureMessage);
            }
        }

        #endregion
    }
}
