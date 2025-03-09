namespace Bookx.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime JoinDatetime { get; set; }
        public string MailAddress { get; set; }
        public ICollection<Tag> Tags { get; } = new List<Tag>();
        public ICollection<Book> Books { get; } = new List<Book>();
        public ICollection<OwnedBook> OwnedBooks { get; set; } = new List<OwnedBook>();
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<OwnedBook> OwnedBooks { get; set; } = new List<OwnedBook>();
    }

    public class OwnedBook
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string BookIsbn { get; set; }
        public Book Book { get; set; } = null!;
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool WouldRecommend { get; set; }
        public DateTime AddedAt { get; set; }
        public ICollection<Tag> Tags { get; } = new List<Tag>();
    }
}
