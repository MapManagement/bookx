using BookxBackendTests.Fixtures;
using BookxProtos;
using Grpc.Core;

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

        internal static async Task<SuccessReply> AddNewBook(UserService.UserServiceClient userServiceClient,
                                                            string jwt,
                                                            string isbn,
                                                            int rating = 5,
                                                            bool wouldRecommend = true,
                                                            string comment = "Some Comment")
        {
            var newBookRequest = new AddSingleOwnedBook()
            {
                Isbn = isbn,
                Rating = rating,
                WouldRecommend = wouldRecommend,
            };

            if (!string.IsNullOrEmpty(comment))
                newBookRequest.Comment = comment;

            var newBookReply = await userServiceClient.AddOwnedBookAsync(newBookRequest, CreateAuthMetadata(jwt));
            return newBookReply;
        }

        internal static Metadata CreateAuthMetadata(string jwt)
        {
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {jwt}");

            return metadata;
        }


        #endregion
    }
}
