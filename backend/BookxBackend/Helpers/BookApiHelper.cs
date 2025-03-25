using Bookx.Models;

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

    public static async Task<Book> RetrieveBookByIsbn(string isbn)
    {
        var path = $"volumes?q=isbn:{isbn}";
        HttpResponseMessage response = await _httpClient.GetAsync(path);

        if (!response.IsSuccessStatusCode)
            return null;

        return await JsonBookParser(response);

    }

    private static async Task<Book> JsonBookParser(HttpResponseMessage bookResponse)
    {
        var googleBook = await bookResponse.Content.ReadFromJsonAsync<GoogleBooksResponse>();

        if (googleBook == null || googleBook.Items.Count == 0)
            return null;

        if (googleBook.Items.Count > 1)
            return null;

        var singleGoogleBook = googleBook.Items[0];

        var dbBook = new Book()
        {
            Isbn = FindIsbn13(singleGoogleBook),
            Title = singleGoogleBook.VolumeInfo.Title,
            CoverPath = singleGoogleBook.VolumeInfo.ImageLinks.Thumbnail,
            NumerOfPages = singleGoogleBook.VolumeInfo.PageCount,
            ShopLink = singleGoogleBook.VolumeInfo.InfoLink,
            Blurb = singleGoogleBook.VolumeInfo.Description,
            ReleaseDate = DateOnly.Parse(singleGoogleBook.VolumeInfo.PublishedDate),
        };

        ConvertToDbAuthors(singleGoogleBook).ForEach(a => dbBook.Authors.Add(a));
        ConvertToDbGenres(singleGoogleBook).ForEach(g => dbBook.Genres.Add(g));
        // TODO: Language

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

    private static List<Author> ConvertToDbAuthors(BookItem googleBook)
    {
        if (googleBook == null)
            return null;

        // TODO: check for existence
        var dbAuthors = new List<Author>();

        foreach (var googleAuthor in googleBook.VolumeInfo.Authors)
        {
            var splittedName = googleAuthor.Split(" ", 2);

            // TODO: birthdate
            var dbAuthor = new Author()
            {
                FirstName = splittedName[0],
                LastName = splittedName[1]
            };

            dbAuthors.Add(dbAuthor);
        }

        return dbAuthors;
    }

    private static List<Genre> ConvertToDbGenres(BookItem googleBook)
    {
        if (googleBook == null)
            return null;

        // TODO: check for existence
        var dbGenres = new List<Genre>();

        foreach (var googleCategory in googleBook.VolumeInfo.Categories)
        {
            var dbGenre = new Genre()
            {
                Name = googleCategory
            };

            dbGenres.Add(dbGenre);
        }

        return dbGenres;
    }

    private static Publisher ConvertToDbPublisher(BookItem googleBook)
    {
        if (googleBook?.VolumeInfo?.Publisher == null)
            return null;

        // TODO: check for existence
        var dbPublisher = new Publisher()
        {
            Name = googleBook.VolumeInfo.Publisher
        };

        return dbPublisher;
    }
}
