using BookxBackendTests.Fixtures;
using BookxProtos;

namespace BookxBackendTests.Helpers
{
    public static class GrpcCallHelper
    {
        #region Methods

        internal static BookxBackendTestFixture CreateTestBackend()
        {
            return new BookxBackendTestFixture();
        }

        internal static async Task<RegisterReply> RegisterNewUser(Authenticator.AuthenticatorClient authClient,
                                                                  string username,
                                                                  string password,
                                                                  string mailAddress)
        {
            var registerRequest = new RegisterRequest()
            {
                Username = username,
                Password = password,
                MailAddress = mailAddress
            };

            return await authClient.RegisterAsync(registerRequest);
        }


        #endregion
    }
}
