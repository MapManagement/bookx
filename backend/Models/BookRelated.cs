namespace Bookx.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly? Birthdate { get; set; }
        public ICollection<Book> Books { get; } = new List<Book>();
    }

    public class Book
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string CoverPath { get; set; }
        public int NumerOfPages { get; set; }
        public string ShopLink { get; set; }
        public string Blurb { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public ICollection<Author> Authors { get; } = new List<Author>();
        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;
        public ICollection<Genre> Genres { get; } = new List<Genre>();

        public ICollection<User> Users { get; } = new List<User>();
        public ICollection<OwnedBook> Owners { get; } = new List<OwnedBook>();
    }

    public class Publisher
    {
        public int Id { get; set; }
        public string Name { get; }
        public ICollection<Book> Books { get; } = new List<Book>();
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; } = new List<Book>();
    }

    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; } = new List<Book>();
    }
}
