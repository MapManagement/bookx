using Microsoft.EntityFrameworkCore;

namespace Bookx.Models
{
    public class BookxContext : DbContext
    {
        #region Properties

        // book related
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Language> Languages { get; set; }

        // user related
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<OwnedBook> OwnedBooks { get; set; }

        #endregion

        #region Constructor

        public BookxContext(DbContextOptions<BookxContext> options) : base(options)
        {
        }

        #endregion

        #region Overrides

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasKey(b => b.Isbn);

            modelBuilder.Entity<Author>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Author>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books)
                .WithMany(b => b.Authors);

            modelBuilder.Entity<Genre>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Genre>()
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Genre>()
                .HasMany(g => g.Books)
                .WithMany(b => b.Genres);

            modelBuilder.Entity<Publisher>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Publisher>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Publisher>()
                .HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId)
                .IsRequired();

            modelBuilder.Entity<Language>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Language>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Language>()
                .HasMany(l => l.Books)
                .WithOne(b => b.Language)
                .HasForeignKey(b => b.LanguageId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.MailAddress)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Books)
                .WithMany(b => b.Users)
                .UsingEntity<OwnedBook>(
                    u => u.HasOne(o => o.Book).WithMany(b => b.Owners).HasForeignKey(o => o.BookIsbn),
                    b => b.HasOne(o => o.User).WithMany(u => u.OwnedBooks).HasForeignKey(o => o.UserId),
                    o => o.HasKey(o => new { o.UserId, o.BookIsbn })
                );

            modelBuilder.Entity<OwnedBook>()
                .HasKey(o => new { o.UserId, o.BookIsbn });

            modelBuilder.Entity<OwnedBook>()
                .HasMany(o => o.Tags)
                .WithMany(t => t.OwnedBooks);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tags)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Tag>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Tag>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Tag>()
                .HasIndex(t => new { t.UserId, t.Name })
                .IsUnique();
        }

        #endregion

        #region Methods

        public Author DuplicateAuthorByName(string firstName, string lastName)
        {
            var authors = this.Authors.Where(
                    a => a.FirstName.ToLower() == firstName.ToLower()
                    && a.LastName.ToLower() == lastName.ToLower()
            );

            if (authors.Any())
                return authors.First();

            return null;
        }

        public Genre DuplicateGenreByName(string genreName)
        {
            var genres = this.Genres
                .Where(g => g.Name.ToLower() == genreName.ToLower());

            if (genres.Any())
                return genres.First();

            return null;
        }

        public Publisher DuplicatePublisherByName(string publisherName)
        {
            var publishers = this.Publishers
                            .Where(p => p.Name.ToLower() == publisherName.ToLower());

            if (publishers.Any())
                return publishers.First();

            return null;
        }

        public Language DuplicateLanguageByName(string languageName)
        {
            var languages = this.Languages
                .Where(l => l.Name.ToLower() == languageName.ToLower());

            if (languages.Any())
                return languages.First();

            return null;
        }

        #endregion
    }
}
