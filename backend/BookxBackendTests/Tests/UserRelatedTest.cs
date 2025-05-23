using BookxProtos;
using BookxBackendTests.Fixtures;
using BookxBackendTests.Helpers;

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
        public async Task SuccessAddOwnedBook(string isbn, int rating, string? comment, bool wouldRecommend)
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

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);
                var newBookReply = await userClient.AddOwnedBookAsync(newBookRequest, metadata);

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

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);
                var newBookReply = await userClient.AddOwnedBookAsync(newBookRequest, metadata);

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

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);
                var newTagReply = await userClient.AddTagAsync(newTagRequest, metadata);

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

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);

                var newTagReply = await userClient.AddTagAsync(newTagRequest, metadata);

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

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);
                var newTagReply = await userClient.AddTagAsync(newTagRequest, metadata);

                Assert.True(newTagReply.Success);

                newTagReply = await userClient.AddTagAsync(newTagRequest, metadata);

                Assert.False(newTagReply.Success);
                Assert.NotNull(newTagReply.FailureMessage);
            }
        }

        [Theory]
        [InlineData("9783641279110", 8, "Some comment", true)]
        [InlineData("9780692818404", 2, null, false)]
        [InlineData("9781639883400", 9, "Some comment", true)]
        public async Task SuccessRemoveOwnedBook(string isbn, int rating, string? comment, bool wouldRecommend)
        {
            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(authClient, LoginUsername, LoginPassword, LoginMailAddress);

                Assert.True(registerReply.ValidRegistration);

                var userClient = new UserService.UserServiceClient(backend.GrpcChannel);

                var newBookReply = await GrpcCallHelper.AddNewBook(userClient, registerReply.Token, isbn, rating, wouldRecommend, comment);

                Assert.True(newBookReply.Success);

                var removeBookRequest = new SingleOwnedBookRequest()
                {
                    Isbn = newBookReply.ObjectId
                };

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);
                var removeBookReply = await userClient.RemoveOwnedBookAsync(removeBookRequest, metadata);

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

                var newBookReply = await GrpcCallHelper.AddNewBook(userClient,
                                                                   registerReply.Token,
                                                                   "9783641279110",
                                                                   2,
                                                                   false,
                                                                   "Some Comment");

                Assert.True(newBookReply.Success);

                // depends on inline data
                var removeBookRequest = new SingleOwnedBookRequest()
                {
                    Isbn = deleteBookIsbn
                };

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);
                var removeBookReply = await userClient.RemoveOwnedBookAsync(removeBookRequest, metadata);

                Assert.False(removeBookReply.Success);
                Assert.NotNull(removeBookReply.FailureMessage);
            }
        }

        [Theory]
        [InlineData("9783641279110", 10, "Some new comment", true)]
        [InlineData("9780692818404", null, "Some new comment", true)]
        [InlineData("9780692818404", 10, null, true)]
        [InlineData("9780692818404", 10, "Some new comment", null)]
        [InlineData("9780692818404", null, null, null)]
        public async Task SuccessEditOwnedBook(string isbn, int? rating, string? comment, bool? wouldRecommend)
        {
            const int initialRating = 5;
            const bool initialWouldRecommend = false;
            const string initialComment = "Some comment";

            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(
                        authClient: authClient,
                        username: LoginUsername,
                        password: LoginPassword,
                        mailAddress: LoginMailAddress
                );

                Assert.True(registerReply.ValidRegistration);

                var userClient = new UserService.UserServiceClient(backend.GrpcChannel);

                var newBookReply = await GrpcCallHelper.AddNewBook(
                        userServiceClient: userClient,
                        jwt: registerReply.Token,
                        isbn: isbn,
                        rating: initialRating,
                        wouldRecommend: initialWouldRecommend,
                        comment: initialComment
                );

                Assert.True(newBookReply.Success);

                var editBookRequest = new EditSingleOwnedBook()
                {
                    Isbn = isbn,
                };

                if (rating != null)
                    editBookRequest.Rating = rating.Value;

                if (comment != null)
                    editBookRequest.Comment = comment;

                if (wouldRecommend != null)
                    editBookRequest.WouldRecommend = wouldRecommend.Value;

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);
                var editBookReply = await userClient.EditOwnedBookAsync(editBookRequest, metadata);

                Assert.True(editBookReply.Success);
            }
        }

        [Theory]
        [InlineData("jfksajfkasklf", 10, "Some new comment", true)]
        public async Task FailEditOwnedBook(string isbn, int? rating, string? comment, bool? wouldRecommend)
        {
            const string initialIsbn = "9783641279110";
            const int initialRating = 5;
            const bool initialWouldRecommend = false;
            const string initialComment = "Some comment";

            using (var backend = GrpcCallHelper.CreateTestBackend())
            {
                var authClient = new Authenticator.AuthenticatorClient(backend.GrpcChannel);

                var registerReply = await GrpcCallHelper.RegisterNewUser(
                        authClient: authClient,
                        username: LoginUsername,
                        password: LoginPassword,
                        mailAddress: LoginMailAddress
                );

                Assert.True(registerReply.ValidRegistration);

                var userClient = new UserService.UserServiceClient(backend.GrpcChannel);

                var newBookReply = await GrpcCallHelper.AddNewBook(
                        userServiceClient: userClient,
                        jwt: registerReply.Token,
                        isbn: initialIsbn,
                        rating: initialRating,
                        wouldRecommend: initialWouldRecommend,
                        comment: initialComment
                );

                Assert.True(newBookReply.Success);

                var editBookRequest = new EditSingleOwnedBook()
                {
                    Isbn = isbn,
                };

                if (rating != null)
                    editBookRequest.Rating = rating.Value;

                if (comment != null)
                    editBookRequest.Comment = comment;

                if (wouldRecommend != null)
                    editBookRequest.WouldRecommend = wouldRecommend.Value;

                var metadata = GrpcCallHelper.CreateAuthMetadata(registerReply.Token);
                var editBookReply = await userClient.EditOwnedBookAsync(editBookRequest, metadata);

                Assert.False(editBookReply.Success);
                Assert.NotNull(editBookReply.FailureMessage);
            }
        }

        #endregion
    }
}
