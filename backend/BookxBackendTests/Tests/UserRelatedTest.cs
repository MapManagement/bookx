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

        [Theory]
        [InlineData("Favorite", "#34eb4c")]
        [InlineData("Easy to read", "#eb34c6")]
        public async Task SuccessAddTag(string name, string color)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(authClient, LoginUsername, LoginPassword, LoginMailAddress);

                Assert.True(registerReply.ValidRegistration);

                var userClient = new UserService.UserServiceClient(backend.GrpcChannel);

                var newTagRequest = new AddSingleTag()
                {
                    Name = name,
                    Color = color
                };

                var requestMetadata = new Metadata();
                requestMetadata.Add("Authorization", $"Bearer {registerReply.Token}");

                var newTagReply = await userClient.AddTagAsync(newTagRequest, requestMetadata);

                Assert.True(newTagReply.Success);
            }
        }


        [Theory]
        [InlineData("", "")]
        [InlineData("Some Name", "")]
        [InlineData("", "#eb34c6")]
        [InlineData("Some Name", "#eb34c600")]
        [InlineData("Some Name", "eb34c600")]
        public async Task FailAddTag(string name, string color)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(authClient, LoginUsername, LoginPassword, LoginMailAddress);

                Assert.True(registerReply.ValidRegistration);

                var userClient = new UserService.UserServiceClient(backend.GrpcChannel);

                var newTagRequest = new AddSingleTag()
                {
                    Name = name,
                    Color = color
                };

                var requestMetadata = new Metadata();
                requestMetadata.Add("Authorization", $"Bearer {registerReply.Token}");

                var newTagReply = await userClient.AddTagAsync(newTagRequest, requestMetadata);

                Assert.False(newTagReply.Success);
                Assert.NotNull(newTagReply.FailureMessage);
            }
        }

        [Theory]
        [InlineData("Easy to read", "#eb34c6")]
        public async Task FailAddExistingTag(string name, string color)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(authClient, LoginUsername, LoginPassword, LoginMailAddress);

                Assert.True(registerReply.ValidRegistration);

                var userClient = new UserService.UserServiceClient(backend.GrpcChannel);

                var newTagRequest = new AddSingleTag()
                {
                    Name = name,
                    Color = color
                };

                var requestMetadata = new Metadata();
                requestMetadata.Add("Authorization", $"Bearer {registerReply.Token}");

                var newTagReply = await userClient.AddTagAsync(newTagRequest, requestMetadata);

                Assert.True(newTagReply.Success);

                newTagReply = await userClient.AddTagAsync(newTagRequest, requestMetadata);

                Assert.False(newTagReply.Success);
                Assert.NotNull(newTagReply.FailureMessage);
            }
        }

        [Theory]
        [InlineData("9783641279110", 8, "Some comment", true)]
        [InlineData("9780692818404", 2, null, false)]
        [InlineData("9781639883400", 9, "Some comment", true)]
        public async Task SuccessRemoveOwnedBook(string isbn, int rating, string comment, bool wouldRecommend)
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

                var removeBookRequest = new SingleOwnedBookRequest()
                {
                    Isbn = newBookReply.ObjectId
                };

                var removeBookReply = await userClient.RemoveOwnedBookAsync(removeBookRequest, requestMetadata);

                Assert.True(removeBookReply.Success);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("hsgfshaafg9024u59")]
        public async Task FailRemoveOwnedBook(string deleteBookIsbn)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                // static for any inline data
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(authClient, LoginUsername, LoginPassword, LoginMailAddress);

                Assert.True(registerReply.ValidRegistration);

                var userClient = new UserService.UserServiceClient(backend.GrpcChannel);

                var newBookRequest = new AddSingleOwnedBook()
                {
                    Isbn = "9783641279110",
                    Rating = 2,
                    WouldRecommend = false
                };
                newBookRequest.Comment = "Some comment";

                var requestMetadata = new Metadata();
                requestMetadata.Add("Authorization", $"Bearer {registerReply.Token}");

                var newBookReply = await userClient.AddOwnedBookAsync(newBookRequest, requestMetadata);

                Assert.True(newBookReply.Success);

                // depends on inline data
                var removeBookRequest = new SingleOwnedBookRequest()
                {
                    Isbn = deleteBookIsbn
                };

                var removeBookReply = await userClient.RemoveOwnedBookAsync(removeBookRequest, requestMetadata);

                Assert.False(removeBookReply.Success);
                Assert.NotNull(removeBookReply.FailureMessage);
            }
        }



        #endregion
    }
}
