using Bookx.Models;
using System.Globalization;

namespace Bookx.Helpers;

public static class BookApiHelper
{
    private const string GoogleApiBaseUrl = "https://www.googleapis.com/books/v1/";

    private static HttpClient _httpClient;

    static BookApiHelper()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(GoogleApiBaseUrl);
    }

    public static async Task<Book> RetrieveBookByIsbn(string isbn, BookxContext dbContext)
    {
        var path = $"volumes?q=isbn:{isbn}";
        HttpResponseMessage response = await _httpClient.GetAsync(path);

        if (!response.IsSuccessStatusCode)
            return null;

        var exactResponse = await RetrieveExactBookLink(response);

        if (exactResponse == null || !exactResponse.IsSuccessStatusCode)
            return null;

        return await JsonBookParser(exactResponse, dbContext);
    }

    private static async Task<HttpResponseMessage> RetrieveExactBookLink(HttpResponseMessage booksResponse)
    {
        var googleBooks = await booksResponse.Content.ReadFromJsonAsync<GoogleBooksResponse>();

        if (googleBooks?.Items?.Count == null || googleBooks.Items.Count == 0)
            return null;

        if (googleBooks.Items.Count > 1)
            return null;

        var singleGoogleBook = googleBooks.Items[0];

        if (string.IsNullOrEmpty(singleGoogleBook.SelfLink))
            return null;

        return await _httpClient.GetAsync(singleGoogleBook.SelfLink);
    }

    private static async Task<Book> JsonBookParser(HttpResponseMessage bookResponse,
            BookxContext dbContext)
    {
        var singleGoogleBook = await bookResponse.Content.ReadFromJsonAsync<BookItem>();

        if (singleGoogleBook == null)
            return null;

        var dbBook = new Book()
        {
            Isbn = FindIsbn13(singleGoogleBook),
            Title = singleGoogleBook.VolumeInfo.Title,
            CoverPath = singleGoogleBook?.VolumeInfo?.ImageLinks?.Thumbnail,
            NumerOfPages = singleGoogleBook.VolumeInfo.PageCount,
            ShopLink = singleGoogleBook.VolumeInfo.InfoLink,
            Blurb = singleGoogleBook.VolumeInfo.Description,
        };

        DateOnly releaseDate;

        if (!DateOnly.TryParseExact(singleGoogleBook.VolumeInfo.PublishedDate, "yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate))
        {
            DateOnly.TryParseExact(singleGoogleBook.VolumeInfo.PublishedDate, "yyyy-dd-MM",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate);
        }

        dbBook.ReleaseDate = releaseDate;

        ConvertToDbAuthors(singleGoogleBook, dbContext).ForEach(a => dbBook.Authors.Add(a));
        ConvertToDbGenres(singleGoogleBook, dbContext).ForEach(g => dbBook.Genres.Add(g));

        dbBook.Language = ConvertToDbLanguage(singleGoogleBook, dbContext);
        dbBook.Publisher = ConvertToDbPublisher(singleGoogleBook, dbContext);

        return dbBook;
    }

    private static string FindIsbn13(BookItem googleBook)
    {
        if (googleBook == null)
            return null;

        var identifiers = googleBook.VolumeInfo.IndustryIdentifiers;

        if (identifiers.Count == 0)
            return null;

        foreach (var identifier in identifiers)
        {
            if (identifier.Type == "ISBN_13")
                return identifier.Identifier;
        }

        return identifiers[0].Identifier;
    }

    private static List<Author> ConvertToDbAuthors(BookItem googleBook, BookxContext dbContext)
    {
        // TODO: check for existence
        var dbAuthors = new List<Author>();

        if (googleBook == null)
            return dbAuthors;

        foreach (var googleAuthor in googleBook.VolumeInfo.Authors)
        {
            var splittedName = googleAuthor.Split(" ", 2);
            string firstName = splittedName[0];
            string lastName = splittedName[1];

            var findAuthor = dbContext.DuplicateAuthorByName(firstName, lastName);
            Author dbAuthor;

            if (findAuthor == null)
            {
                // TODO: birthdate
                dbAuthor = new Author()
                {
                    FirstName = firstName,
                    LastName = lastName
                };
            }
            else
            {
                dbAuthor = findAuthor;
            }

            dbAuthors.Add(dbAuthor);
        }

        return dbAuthors;
    }

    private static List<Genre> ConvertToDbGenres(BookItem googleBook, BookxContext dbContext)
    {
        var dbGenres = new List<Genre>();

        if (googleBook?.VolumeInfo?.Categories == null || googleBook.VolumeInfo.Categories.Count < 1)
            return dbGenres;

        foreach (var googleCategory in googleBook.VolumeInfo.Categories)
        {
            var findGenre = dbContext.DuplicateGenreByName(googleCategory);
            Genre dbGenre;

            if (findGenre == null)
            {

                dbGenre = new Genre()
                {
                    Name = googleCategory
                };
            }
            else
            {
                dbGenre = findGenre;
            }

            dbGenres.Add(dbGenre);
        }

        return dbGenres;
    }

    private static Publisher ConvertToDbPublisher(BookItem googleBook, BookxContext dbContext)
    {
        if (googleBook?.VolumeInfo?.Publisher == null)
            return null;

        var findPublisher = dbContext.DuplicatePublisherByName(googleBook.VolumeInfo.Publisher);
        Publisher dbPublisher;

        if (findPublisher == null)
        {
            dbPublisher = new Publisher()
            {
                Name = googleBook.VolumeInfo.Publisher
            };
        }
        else
        {
            dbPublisher = findPublisher;
        }

        return dbPublisher;
    }

    private static Language ConvertToDbLanguage(BookItem googleBook, BookxContext dbContext)
    {
        if (googleBook?.VolumeInfo?.Language == null)
            return null;

        var findLanguage = dbContext.DuplicateLanguageByName(googleBook.VolumeInfo.Language);
        Language dbLanguage;

        if (findLanguage == null)
        {
            dbLanguage = new Language()
            {
                Name = googleBook.VolumeInfo.Language
            };
        }
        else
        {
            dbLanguage = findLanguage;
        }

        return dbLanguage;
    }
}
