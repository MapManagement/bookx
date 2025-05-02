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
        [InlineData("9780692818404", 2, null, false)]
        [InlineData("9781639883400", 9, "Some comment", true)]
        public async Task SuccessAddOwnedBook(string isbn, int rating, string comment, bool wouldRecommend)
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
                    WouldRecommend = wouldRecommend
                };

                if (!string.IsNullOrEmpty(comment))
                    newBookRequest.Comment = comment;

                var requestMetadata = new Metadata();
                requestMetadata.Add("Authorization", $"Bearer {registerReply.Token}");

                var newBookReply = await userClient.AddOwnedBookAsync(newBookRequest, requestMetadata);

                Assert.True(newBookReply.Success);
            }
        }

        [Theory]
        [InlineData("836412791", 1, "Some comment", true)]
        [InlineData("", 1, "Some comment", true)]
        public async Task FailAddOwnedBook(string isbn, int rating, string comment, bool wouldRecommend)
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
                    WouldRecommend = wouldRecommend
                };

                if (!string.IsNullOrEmpty(comment))
                    newBookRequest.Comment = comment;

                var requestMetadata = new Metadata();
                requestMetadata.Add("Authorization", $"Bearer {registerReply.Token}");

                var newBookReply = await userClient.AddOwnedBookAsync(newBookRequest, requestMetadata);

                Assert.False(newBookReply.Success);
                Assert.NotNull(newBookReply.FailureMessage);
            }
        }

        #endregion
    }
}
