using BookxProtos;
using BookxBackendTests.Helpers;

namespace BookxBackendTests.Tests
{
    public class BookRelatedTests
    {
        #region Tests

        [Theory]
        [InlineData("Herman", "Hesse", "Herman.Hesse@mail.com")]
        [InlineData("Ernst Theodor Amadeus", "Hoffmann", "ETA.Hoffmann@mail.com")]
        [InlineData("Friedrich", "Dürrenmatt", "Friedrich.Dürrenmatt@mail.com")]
        public async Task SuccessfullyAddNewAuthor(string username, string password, string mailAddress)
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

        #endregion
    }
}
