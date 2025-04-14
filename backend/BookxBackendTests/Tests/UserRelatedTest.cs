using BookxProtos;
using BookxBackendTests.Fixtures;
using BookxBackendTests.Helpers;
using Grpc.Core;

namespace BookxBackendTests.Tests
{
    public class UserRelatedTests
    {
        #region Fields

        const string LoginUsername = "Book";
        const string LoginPassword = "Reader";
        const string LoginMailAddress = "book@reader.com";

        #endregion

        #region Methods

        private BookxBackendTestFixture CreateTestBackend()
        {
            return new BookxBackendTestFixture();
        }

        #endregion

        #region Tests

        [Theory]
        [InlineData("9783641279110", 8, "Some comment", true)]
        public async Task SuccessfulAddOwnedBook(string isbn, int rating, string comment, bool wouldRecommend)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(authClient, LoginUsername, LoginPassword, LoginMailAddress);

                Assert.True(registerReply.ValidRegistration);

                var userClient = new UserService.UserServiceClient(backend.GrpcChannel);

                var newBookRequest = new AddSingleOwnedBook()
                {
                    Isbn = isbn,
                    Rating = rating,
                    Comment = comment,
                    WouldRecommend = wouldRecommend
                };

                var requestMetadata = new Metadata();
                requestMetadata.Add("Authorization", $"Bearer {registerReply.Token}");

                var newBookReply = await userClient.AddOwnedBookAsync(newBookRequest, requestMetadata);

                Assert.True(newBookReply.Success);
            }
        }


        #endregion
    }
}
